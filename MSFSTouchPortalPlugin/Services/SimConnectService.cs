/*
This file is part of the MSFS Touch Portal Plugin project.
https://github.com/mpaperno/MSFSTouchPortalPlugin

COPYRIGHT:
(c) 2020 Tim Lewis;
(c) Maxim Paperno; All Rights Reserved.

This file may be used under the terms of the GNU General Public License (GPL)
as published by the Free Software Foundation, either version 3 of the Licenses,
or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

A copy of the GNU GPL is included with this project
and is also available at <http://www.gnu.org/licenses/>.
*/

using Microsoft.Extensions.Logging;
using Microsoft.FlightSimulator.SimConnect;
using MSFSTouchPortalPlugin.Enums;
using MSFSTouchPortalPlugin.Interfaces;
using MSFSTouchPortalPlugin.Types;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using WASimCommander.CLI;
using WASimCommander.CLI.Enums;
using WASimCommander.CLI.Structs;
using WASMLib = WASimCommander.CLI.Client.WASimClient;
// yeah so we got some name conflicts
using WasimLogLevel =  WASimCommander.CLI.Enums.LogLevel;
using ILogLevel = Microsoft.Extensions.Logging.LogLevel;
using WasmUptPeriod = WASimCommander.CLI.Enums.UpdatePeriod;
using SimConUpdPeriod = MSFSTouchPortalPlugin.Enums.UpdatePeriod;

namespace MSFSTouchPortalPlugin.Services
{
  /// <summary>
  /// Wrapper for SimConnect
  /// </summary>
  internal class SimConnectService : ISimConnectService {
    #region DLL Imports
    // Set up tracking requests by their SendID for diagnostic purposes with Simconnect_OnRecvException()
    // https://docs.flightsimulator.com/html/Programming_Tools/SimConnect/API_Reference/Debug/SimConnect_GetLastSentPacketID.htm
    IntPtr _hSimConnect = IntPtr.Zero;  // native SimConnect handle pointer
    // Import methods from the actual SimConnect client which aren't available in the C# wrapper.
    [DllImport("SimConnect.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    static extern int /* HRESULT */ SimConnect_GetLastSentPacketID(IntPtr hSimConnect, out uint /* DWORD */ dwSendID);
    // Get a FieldInfo object on the SimConnect.hSimConnect private field variable so that we can query its value to get the handle to the actual SimConnect client.
    static readonly FieldInfo _fiSimConnect = typeof(SimConnect).GetField("hSimConnect", BindingFlags.NonPublic | BindingFlags.Instance);
    #endregion DLL Imports

    #region Public
    public event DataUpdateEventHandler OnDataUpdateEvent;
    public event RecvEventEventHandler OnEventReceived;
    public event ConnectEventHandler OnConnect;
    public event DisconnectEventHandler OnDisconnect;
    public event ExceptionEventHandler OnException;
    public event LocalVarsListUpdatedHandler OnLVarsListUpdated;

    // hResult
    public const uint S_OK = 0;
    public const uint E_FAIL = 0x80004005;         // general SimConnect failure
    public const uint E_INVALIDARG = 0x80070057;   // returned by SimConnect if the config index is invalid
    public const uint E_TIMEOUT = 0x000005B4;      // we return this if SimConnect() was created OK but no OnOpen was received
    public const uint E_DISCONNECT = 0xC000013C;   // STATUS_REMOTE_DISCONNECT returned by SimConnect if network connection is lost
    // SIMCONNECT_RECV_EVENT.dwData value for "View" event, from SimConnect.h
    public const uint VIEW_EVENT_DATA_COCKPIT_2D = 0x00000001;      // 2D Panels in cockpit view
    public const uint VIEW_EVENT_DATA_COCKPIT_3D = 0x00000002;      // Virtual (3D) panels in cockpit view
    public const uint VIEW_EVENT_DATA_ORTHOGONAL = 0x00000004;      // Orthogonal (Map) view

    public bool IsConnected => _connected;
    public WasmModuleStatus WasmStatus { get; private set; } = WasmModuleStatus.Unknown;

    public SimConnectService(ILogger<SimConnectService> logger, IReflectionService reflectionService) {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _reflectionService = reflectionService ?? throw new ArgumentNullException(nameof(reflectionService));
      SetupWasm();
    }

    #endregion Public

    const int SIM_CONNECT_TIMEOUT_MS  = 5000;   // SimConnect "Open" event timeout after successfully creating SimConnect() instance (should really be almost instant if everything is OK)
    const int MSG_RCV_WAIT_TIME_MS    = 5000;   // SimConnect.ReceiveMessage() wait time
    // maximum stored requests for error tracking (adding is fast but search is 0(n)), should be large enough to handle flood of requests at initial connection
    const int MAX_STORED_REQUEST_RECORDS = 500;

    const uint NOTIFICATION_PRIORITY = 10000000;  // notification group priority for SimConnect.SetNotificationGroupPriority()
    const int WM_USER_SIMCONNECT = 0x0402;        // user event ID for SimConnect

    // enable AddNotification(), SetNotificationGroupPriorities(), and Simconnect_OnRecvEvent() for events we initiate; currently this serves no purpose except possible debug info.
    static readonly bool DEBUG_NOTIFICATIONS = false;

    enum DataProvider : byte { SimConnect, WASimClient };

    readonly ILogger<SimConnectService> _logger;
    readonly IReflectionService _reflectionService;

    bool _connected;
    bool _connecting;
    int _reqTrackIndex = 0;  // current write slot index in _requestTracking array
    uint _wasmClientId = 0xFF700C49;   // TODO: make top two bytes dynamic per plugin instance
    bool WasmConnected => WasmStatus == WasmModuleStatus.Connected;
    bool WasmInitialized => WasmStatus > WasmModuleStatus.NotFound;
    Task _messageWaitTask;
    SimConnect _simConnect;
    WASMLib _wlib = null;
    readonly EventWaitHandle _scReady = new EventWaitHandle(false, EventResetMode.AutoReset);
    readonly AutoResetEvent _scQuit = new(false);
    readonly Dictionary<Definition, DataProvider> _addedDefinitions = new();  // keep track of added SimVar definitions to avoid redundant registrations
    readonly RequestTrackingData[] _requestTracking = new RequestTrackingData[MAX_STORED_REQUEST_RECORDS];  // rolling buffer array for request tracking

    // SimConnect method delegates, for centralized interaction in InvokeSimMethod()
    Action<Enum>             ClearDataDefinitionDelegate;
    Action<Enum, uint>       SetNotificationGroupPriorityDelegate;
    Action<uint, Enum>       AIReleaseControlDelegate;
    Action<Enum, string>     MapClientEventToSimEventDelegate;
    Action<Enum, Enum, bool> AddClientEventToNotificationGroupDelegate;
    Action<Enum, string>     SubscribeToSystemEventDelegate;
    Action<Enum, Enum, uint, SIMCONNECT_SIMOBJECT_TYPE>   RequestDataOnSimObjectTypeDelegate;
    Action<uint, Enum, uint, Enum, SIMCONNECT_EVENT_FLAG> TransmitClientEventDelegate;
    Action<Enum, uint, SIMCONNECT_DATA_SET_FLAG, object>  SetDataOnSimObjectDelegate;
    Action<Enum, string, string, SIMCONNECT_DATATYPE, float, uint> AddToDataDefinitionDelegate;
    Action<Enum, Enum, uint, SIMCONNECT_PERIOD, SIMCONNECT_DATA_REQUEST_FLAG, uint, uint, uint> RequestDataOnSimObjectDelegate;
    readonly Dictionary<Type, Action<Enum> > _registerDataDelegates = new();

    uint StartSimConnect(uint configIndex) {
      if (_connecting || _connected)
        return IsConnected? S_OK : E_FAIL;

      uint ret = E_FAIL;
      _connecting = true;
      _logger.LogInformation("Connecting to SimConnect...");

      _wlib?.setNetworkConfigurationId((int)configIndex);

      try {
        _simConnect = new SimConnect("Touch Portal Plugin", IntPtr.Zero, WM_USER_SIMCONNECT, _scReady, configIndex);

        // Set up minimum handlers to receive connection notification
        _simConnect.OnRecvOpen += new SimConnect.RecvOpenEventHandler(Simconnect_OnRecvOpen);
        _scQuit.Reset();
        _messageWaitTask = Task.Run(ReceiveMessages);

        // Make sure we actually connect
        var sw = System.Diagnostics.Stopwatch.StartNew();
        while (!_connected && sw.ElapsedMilliseconds <= SIM_CONNECT_TIMEOUT_MS)
          Thread.Sleep(1);

        ret = _connected ? S_OK : E_TIMEOUT;
        if (ret == E_TIMEOUT)
          StopSimConnect();
      }
      catch (Exception e) {
        _logger.LogDebug("Connection to SimConnect failed: [{0:X}] {0}", e.HResult, e.Message);
        StopSimConnect();
        unchecked { ret = (uint)e.HResult; }
      }

      _connecting = false;
      return ret;
    }

    void OnConnected(SIMCONNECT_RECV_OPEN data) {
      _connected = true;
      _logger.LogInformation("SimConnect Connected.");

      // System Events
      _simConnect.OnRecvQuit      += new SimConnect.RecvQuitEventHandler(Simconnect_OnRecvQuit);
      _simConnect.OnRecvException += new SimConnect.RecvExceptionEventHandler(Simconnect_OnRecvException);

      // Sim mapped events
      _simConnect.OnRecvEvent         += Simconnect_OnRecvEvent;
      _simConnect.OnRecvEventFilename += Simconnect_OnRecvFilename;

      // Sim Data
      _simConnect.OnRecvSimobjectData       += new SimConnect.RecvSimobjectDataEventHandler(Simconnect_OnRecvSimObjectData);
      _simConnect.OnRecvSimobjectDataBytype += new SimConnect.RecvSimobjectDataBytypeEventHandler(Simconnect_OnRecvSimobjectDataBytype);

      // Method delegates
      MapClientEventToSimEventDelegate          = _simConnect.MapClientEventToSimEvent;
      TransmitClientEventDelegate               = _simConnect.TransmitClientEvent;
      AddClientEventToNotificationGroupDelegate = _simConnect.AddClientEventToNotificationGroup;
      SetNotificationGroupPriorityDelegate      = _simConnect.SetNotificationGroupPriority;
      ClearDataDefinitionDelegate               = _simConnect.ClearDataDefinition;
      AddToDataDefinitionDelegate               = _simConnect.AddToDataDefinition;
      RequestDataOnSimObjectDelegate            = _simConnect.RequestDataOnSimObject;
      RequestDataOnSimObjectTypeDelegate        = _simConnect.RequestDataOnSimObjectType;
      SetDataOnSimObjectDelegate                = _simConnect.SetDataOnSimObject;
      AIReleaseControlDelegate                  = _simConnect.AIReleaseControl;
      SubscribeToSystemEventDelegate            = _simConnect.SubscribeToSystemEvent;

      _registerDataDelegates.Clear();
      _registerDataDelegates.Add(typeof(double),    _simConnect.RegisterDataDefineStruct<double>);
      _registerDataDelegates.Add(typeof(uint),      _simConnect.RegisterDataDefineStruct<uint>);
      _registerDataDelegates.Add(typeof(long),      _simConnect.RegisterDataDefineStruct<long>);
      _registerDataDelegates.Add(typeof(StringVal), _simConnect.RegisterDataDefineStruct<StringVal>);

      SetupRequestTracking();
      //_simConnect.Text(SIMCONNECT_TEXT_TYPE.PRINT_BLACK, 5, Events.StartupMessage, "TouchPortal Connected");  // not currently supported in MSFS SDK

      ConnectWasm();

      OnConnect?.Invoke(new SimulatorInfo(data));
    }

    void StopSimConnect() {
      if (_messageWaitTask != null) {
        _scQuit.Set();  // trigger message wait task to exit
        var sw = System.Diagnostics.Stopwatch.StartNew();
        while (_messageWaitTask.Status == TaskStatus.Running && sw.ElapsedMilliseconds <= MSG_RCV_WAIT_TIME_MS)
          Thread.Sleep(2);
        if (_messageWaitTask.Status == TaskStatus.Running)
          _logger.LogWarning("Message wait task timed out while stopping.");
        try { _messageWaitTask.Dispose(); }
        catch { /* ignore in case it hung */ }
      }

      if (WasmConnected) {
        _wlib?.disconnectSimulator();
        WasmStatus = WasmModuleStatus.Found;
      }

      if (_simConnect != null) {
        // Dispose serves the same purpose as SimConnect_Close()
        try {
          _simConnect.Dispose();
          _logger.LogDebug("SimConnect disposed");
        }
        catch (Exception e) {
          _logger.LogWarning(e, "Exception while trying to dispose SimConnect client.");
        }
      }
      _simConnect = null;
      _messageWaitTask = null;
      _addedDefinitions.Clear();
      _connected = false;
    }

    // runs in separate task/thread
    private void ReceiveMessages() {
      _logger.LogDebug("ReceiveMessages task started.");
      int sig;
      var waitHandles = new WaitHandle[] { _scReady, _scQuit };
      try {
        while (_connected || _connecting) {
          sig = WaitHandle.WaitAny(waitHandles, MSG_RCV_WAIT_TIME_MS);
          if (sig == 0 && _simConnect != null)
            _simConnect.ReceiveMessage();    // note that this calls our event handlers synchronously on this same thread.
          else if (sig != WaitHandle.WaitTimeout)
            break;
        }
      }
      catch (ObjectDisposedException) { /* ignore but exit */ }
      catch (Exception e) {
        _logger.LogError(e, $"ReceiveMessages task exception {e.HResult:X}, disconnecting.");
        Task.Run(Disconnect);  // async to avoid deadlock
        // COMException (0xC000014B) = broken pipe (sim crashed/network loss on a Pipe type connection)
      }
      _logger.LogDebug("ReceiveMessages task stopped.");
    }

    // Centralized SimConnect method handler
    private bool InvokeSimMethod(Delegate method, params object[] args) {
      if (method == null || !_connected)
        return false;
      try {
        _logger.LogTrace("Invoking: {methodName}({args})", method.Method.Name, string.Join(", ", args));
        method.DynamicInvoke(args);
        AddRequestRecord(method.Method, args);
        return true;
      }
      catch (COMException e) {
        _logger.LogWarning("SimConnect returned an error: [{hResult:X}] {message} <site: '{targetSite}'; source: '{source}'", e.HResult, e.Message, e.TargetSite, e.Source);
      }
      catch (Exception e) {
        _logger.LogError(e, "Method invocation failed with system exception: [{hResult:X}] {message}", e.HResult, e.Message);
      }
      return false;
    }

    //  WASM

    void SetupWasm() {
      _wlib = new WASMLib(_wasmClientId);
      _wlib.OnClientEvent += WASMLib_OnClientEvent;
      _wlib.OnLogRecordReceived += WASMLib_OnLogEntryReceived;
      _wlib.OnDataReceived += WASMLib_OnValueChanged;
      _wlib.OnListResults += WASMLib_OnListResults;
      _wlib.setLogLevel(GetWasimLogLevel(), LogFacility.Remote, LogSource.Client);
      _wlib.setLogLevel(WasimLogLevel.None, LogFacility.Console, LogSource.Client);

      // subscribe to ATC MODEL changes to get notified of possible changes to available L vars.
      _wlib.saveDataRequest(new DataRequest(0, CalcResultType.String, "(A:ATC MODEL,String)", 36, WasmUptPeriod.Millisecond, 30000, 0.0f));
    }

    void ConnectWasm() {
      if (_wlib == null || _wlib.connectServer() != HR.OK) {
        WasmStatus = WasmModuleStatus.NotFound;
        _logger.LogWarning((int)EventIds.PluginError, "WASM Module not found or couldn't connect, integration disabled.");
        return;
      }
      WasmStatus = WasmModuleStatus.Connected;
      _logger.LogInformation((int)EventIds.PluginInfo, "Connected to WASimConnect Server v" + _wlib.serverVersion().ToString("X8"));
    }

    void WASMLib_OnClientEvent(ClientEvent ev) {
      _logger.LogInformation($"WASimClient Event: {ev}");
    }

    void WASMLib_OnListResults(ListResult lr) {
      _logger.LogDebug($"Got WASM list results of type: {lr.listType}, {lr.list.Count} item(s).");
      OnLVarsListUpdated?.Invoke(lr.list);
    }

    void WASMLib_OnValueChanged(DataRequestRecord dr) {
      _logger.LogTrace("Got WASM data for Request: {dr}", dr.ToString());
      if (dr.requestId == 0)
        RequestLookupList(LookupItemType.LocalVariable);  // request local vars list when the "ATC MODEL" changes.
      else
        OnDataUpdateEvent?.Invoke((Definition)dr.requestId, (Definition)dr.requestId, dr);
    }

    void WASMLib_OnLogEntryReceived(LogRecord log, LogSource src) {
      _logger.Log(WasmLogLevelToLoggerLevel(log.level), "[WASM] {src} - {log}", src, log);
    }

    // convert WASimCommander LogLevel to ILogger LogLevel
    static ILogLevel WasmLogLevelToLoggerLevel(WasimLogLevel level) {
      return level switch {
        WasimLogLevel.Trace => ILogLevel.Trace,
        WasimLogLevel.Debug => ILogLevel.Debug,
        WasimLogLevel.Info => ILogLevel.Information,
        WasimLogLevel.Warning => ILogLevel.Warning,
        WasimLogLevel.Error => ILogLevel.Error,
        WasimLogLevel.Critical => ILogLevel.Critical,
        _ => ILogLevel.None
      };
    }

    // convert ILogger LogLevel to WASimCommander LogLevel
    WasimLogLevel GetWasimLogLevel() {
      // 'cuz.. yea... having an ILogger.CurrentLevel property is too much to ask for I guess...
      if (_logger.IsEnabled(ILogLevel.Trace))
        return WasimLogLevel.Trace;
      if (_logger.IsEnabled(ILogLevel.Debug))
        return WasimLogLevel.Debug;
      if (_logger.IsEnabled(ILogLevel.Information))
        return WasimLogLevel.Info;
      if (_logger.IsEnabled(ILogLevel.Warning))
        return WasimLogLevel.Warning;
      if (_logger.IsEnabled(ILogLevel.Error))
        return WasimLogLevel.Error;
      if (_logger.IsEnabled(ILogLevel.Critical))
        return WasimLogLevel.Critical;
      return WasimLogLevel.None;
    }

    static WasmUptPeriod PluginPeriodToWasmPeriod(SimConUpdPeriod p, ref uint interval) {
      if (p == SimConUpdPeriod.Second)
        interval *= 1000;
      return p switch {
        SimConUpdPeriod.Never => WasmUptPeriod.Never,
        SimConUpdPeriod.Once => WasmUptPeriod.Once,
        SimConUpdPeriod.Millisecond or SimConUpdPeriod.Second => WasmUptPeriod.Millisecond,
        _ => WasmUptPeriod.Tick,
      };
    }

    bool RegisterToWasm(SimVarItem simVar) {
      if (!WasmInitialized) {
        _logger.LogError("Cannot request '{varType}' type variable w/out WASM module.", simVar.VariableType);
        return false;
      }
      uint interval = simVar.UpdateInterval;
      WasmUptPeriod period = PluginPeriodToWasmPeriod(simVar.UpdatePeriod, ref interval);
      var dr = new DataRequest() {
        requestId = (uint)simVar.Def,
        requestType = (simVar.VariableType == 'Q' ? RequestType.Calculated : RequestType.Named),
        varTypePrefix = (sbyte)(simVar.VariableType == 'Q' ? 0 : simVar.VariableType),
        calcResultType = simVar.CalcResultType,
        valueSize = simVar.DataSize,
        period = period,
        interval = interval,
        deltaEpsilon = simVar.DeltaEpsilon,
        nameOrCode = new(simVar.SimVarName),
        unitName = new(simVar.Unit)
      };
      HR hr;
      if ((hr = _wlib.saveDataRequest(dr)) != HR.OK) {
        _logger.LogError("Could not complete request due to WASimClient error '{hr}', check log messages.", hr.ToString());
        return false;
      }
      _addedDefinitions.Add(simVar.Def, DataProvider.WASimClient);
      return true;
    }

    #region Public Interface Methods

    public uint Connect(uint configIndex = 0) {
      return StartSimConnect(configIndex);
    }

    public void Disconnect() {
      StopSimConnect();
      // Invoke Handler
      OnDisconnect?.Invoke();
    }

    public bool MapClientEventToSimEvent(Enum eventId, string eventName, Groups group) {
      if (InvokeSimMethod(MapClientEventToSimEventDelegate, eventId, eventName))
        return !DEBUG_NOTIFICATIONS || AddNotification(group, eventId);
      return false;
    }

    public bool TransmitClientEvent(Groups group, Enum eventId, uint data) {
      return InvokeSimMethod(TransmitClientEventDelegate, SimConnect.SIMCONNECT_OBJECT_ID_USER, eventId, data, group, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
    }

    public bool AddNotification(Groups group, Enum eventId) {
      return DEBUG_NOTIFICATIONS && InvokeSimMethod(AddClientEventToNotificationGroupDelegate, group, eventId, false);
    }

    public void SetNotificationGroupPriorities() {
      if (DEBUG_NOTIFICATIONS && _connected) {
        foreach (var g in Enum.GetValues<Groups>()) {
          if (g != Groups.None)
            InvokeSimMethod(SetNotificationGroupPriorityDelegate, g, NOTIFICATION_PRIORITY);
        }
      }
    }

    public void ClearAllDataDefinitions() {
      if (_connected) {
        foreach (var def in _addedDefinitions.Keys)
          ClearDataDefinition(def);
      }
    }

    public bool ClearDataDefinition(Definition def) {
      if (_addedDefinitions.TryGetValue(def, out var dataProvider)) {
        _addedDefinitions.Remove(def);
        if (dataProvider == DataProvider.SimConnect)
          return InvokeSimMethod(ClearDataDefinitionDelegate, def);
        return WasmInitialized && _wlib.removeDataRequest((uint)def) == HR.OK;
      }
      return false;
    }

    public bool RegisterToSimConnect(SimVarItem simVar) {
      if (_addedDefinitions.ContainsKey(simVar.Def)) {
        _logger.LogDebug($"SimVar already registered. {simVar}", simVar.ToDebugString());
        return true;
      }

      if (simVar.VariableType != 'A')
        return RegisterToWasm(simVar);

      if (!_registerDataDelegates.TryGetValue(simVar.StorageDataType, out var registerDataDelegate)) {
        _logger.LogError("Unable to register storage type for '{type}'", simVar.StorageDataType.ToString());
        return false;
      }

      string unitName = simVar.IsStringType ? null : simVar.Unit;
      if (!InvokeSimMethod(AddToDataDefinitionDelegate, simVar.Def, simVar.SimVarName, unitName, simVar.SimConnectDataType, simVar.DeltaEpsilon, SimConnect.SIMCONNECT_UNUSED))
        return false;

      _addedDefinitions.Add(simVar.Def, DataProvider.SimConnect);

      if (!InvokeSimMethod(registerDataDelegate, simVar.Def)) {
        ClearDataDefinition(simVar.Def);
        return false;
      }

      return simVar.NeedsScheduledRequest || RequestDataOnSimObject(simVar);
    }

    public bool RequestDataOnSimObjectType(SimVarItem simVar, SIMCONNECT_SIMOBJECT_TYPE objectType = SIMCONNECT_SIMOBJECT_TYPE.USER) {
      return InvokeSimMethod(RequestDataOnSimObjectTypeDelegate, simVar.Def, simVar.Def, 0U, objectType);
    }

    public bool RequestDataOnSimObject(SimVarItem simVar, uint objectId = (uint)SIMCONNECT_SIMOBJECT_TYPE.USER) {
      return InvokeSimMethod(
        RequestDataOnSimObjectDelegate,
        simVar.Def, simVar.Def, objectId, (SIMCONNECT_PERIOD)simVar.UpdatePeriod, SIMCONNECT_DATA_REQUEST_FLAG.CHANGED, 0U, simVar.UpdateInterval, 0U
      );
    }

    /// <summary>
    /// Set the value associated with a SimVar
    /// </summary>
    public bool SetDataOnSimObject(SimVarItem simVar, uint objectId = (uint)SIMCONNECT_SIMOBJECT_TYPE.USER) {
      return InvokeSimMethod(SetDataOnSimObjectDelegate, simVar.Def, objectId, SIMCONNECT_DATA_SET_FLAG.DEFAULT, simVar.Value);
    }

    /// <summary>
    /// Clear the AI control of a simulated object, typically an aircraft, in order for it to be controlled by a SimConnect client.
    /// </summary>
    /// <param name="def">The previously-registered data definition ID of the variable to release.</param>
    /// <returns>True on request success, false otherwise (this is the status of transmitting the command, not whether control was actually released).</returns>
    public bool ReleaseAIControl(Definition def, uint objectId = (uint)SIMCONNECT_SIMOBJECT_TYPE.USER) {
      return InvokeSimMethod(AIReleaseControlDelegate, objectId, def);
    }

    public bool SubscribeToSystemEvent(Enum eventId, string eventName) {
      return InvokeSimMethod(SubscribeToSystemEventDelegate, eventId, eventName);
    }

    // WASM
    public void UpdateWasmClientId(byte highByte)
    {
      _wasmClientId = (_wasmClientId & 0x00FFFFFFU) | ((uint)highByte << 24);
      _logger.LogDebug("Updated WASimClient ID to {id:X08}", _wasmClientId);
    }

    public bool ExecuteCalculatorCode(string code) {
      if (!WasmConnected || string.IsNullOrWhiteSpace(code))
        return false;
      _logger.LogDebug("Sending calculator string: '{code}'", code);
      return _wlib.executeCalculatorCode(code) == HR.OK;
    }

    public bool SetVariable(char varType, string varName, double value, string unit = "", bool createLocal = false) {
      if (!WasmConnected || string.IsNullOrWhiteSpace(varName))
        return false;
      _logger.LogDebug("Settings variable {varType} {varName} to {value}", varType, varName, value.ToString());
      if (createLocal && varType == 'L')
        return _wlib.setOrCreateLocalVariable(varName, value) == HR.OK;
      return _wlib.setVariable(new VariableRequest(varType, varName, unit), value) == HR.OK;
    }

    public bool RequestLookupList(Enum listType) {
      if (listType.GetType() != typeof(LookupItemType) || !WasmConnected)
        return false;
      _wlib.list((LookupItemType)listType);
      return true;
    }

    public bool RequestVariableValueUpdate(SimVarItem simVar) {
      if (!_addedDefinitions.TryGetValue(simVar.Def, out var dataProvider))
        return false;
      if (dataProvider == DataProvider.WASimClient)
        return _wlib.updateDataRequest((uint)simVar.Def) == HR.OK;
      return RequestDataOnSimObject(simVar);
    }

    #endregion Public Interface Methods

    #region SimConnect Event Handlers

    private void Simconnect_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data) {
      OnConnected(data);
    }

    private void Simconnect_OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data) {
      _logger.LogInformation("Received shutdown command from SimConnect, disconnecting.");
      Task.Run(Disconnect);  // async to avoid deadlock
    }

    private void Simconnect_OnRecvSimobjectDataBytype(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data) {
      if (data.dwData.Length > 0)
        OnDataUpdateEvent?.Invoke((Definition)data.dwDefineID, (Definition)data.dwRequestID, data.dwData[0]);
    }

    private void Simconnect_OnRecvSimObjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data) {
      if (data.dwData.Length > 0)
        OnDataUpdateEvent?.Invoke((Definition)data.dwDefineID, (Definition)data.dwRequestID, data.dwData[0]);
    }

    private void Simconnect_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data) {
      RequestTrackingData record = GetRequestRecord(data.dwSendID);
      record.eException = (SIMCONNECT_EXCEPTION)data.dwException;
      record.dwExceptionIndex = data.dwIndex;
      _logger.LogDebug("SimConnect Error: {e}; SendID: {sendID}; Index: {index};", record.eException, data.dwSendID, data.dwIndex);
      OnException?.Invoke(record);
    }

    private void Simconnect_OnRecvEvent(SimConnect sender, SIMCONNECT_RECV_EVENT data) {
      EventIds evId = (EventIds)data.uEventID;
      Groups gId = (Groups)data.uGroupID;
      string evName = evId.ToString();
      if (DEBUG_NOTIFICATIONS && !Enum.IsDefined(evId) && Enum.IsDefined(gId)) {
        evName = _reflectionService.GetSimEventNameById(data.uEventID);
      }
      _logger.LogDebug("Simconnect_OnRecvEvent Received: Group: {gId}; Event: {evName}; Data: {dwData}", gId, evName, data.dwData);
      OnEventReceived?.Invoke(evId, gId, data.dwData);
    }

    private void Simconnect_OnRecvFilename(SimConnect sender, SIMCONNECT_RECV_EVENT_FILENAME data) {
      EventIds evId = (EventIds)data.uEventID;
      Groups gId = (Groups)data.uGroupID;
      _logger.LogDebug("Simconnect_OnRecvFilename Received: Group: {gId}; Event: {evId}; Data: {fileName}", gId, evId, data.szFileName);
      OnEventReceived?.Invoke(evId, gId, data.szFileName);
    }

    #endregion SimConnect Event Handlers

    #region SimConnect Request Tracking

    private void SetupRequestTracking() {
      // Get direct access to the SimConnect handle, to use functions otherwise not supported.
      try {
        _hSimConnect = (IntPtr)_fiSimConnect.GetValue(_simConnect);
        _reqTrackIndex = 0;
      }
      catch (Exception e) {
        _hSimConnect = IntPtr.Zero;
        _logger.LogError(e, "Exception trying to get handle to SimConnect:");
      }
    }

    private void AddRequestRecord(MethodInfo info, params object[] args) {
      if (_simConnect == null || _hSimConnect == IntPtr.Zero)
        return;

      if (SimConnect_GetLastSentPacketID(_hSimConnect, out uint dwSendID) == S_OK) {
        _requestTracking[_reqTrackIndex] = new RequestTrackingData(dwSendID, info.Name, info.GetParameters(), args);
        _reqTrackIndex = (_reqTrackIndex + 1) % MAX_STORED_REQUEST_RECORDS;
      }
      else {
        _logger.LogWarning("SimConnect_GetLastSentPacketID returned E_FAIL");
      }
    }

    private RequestTrackingData GetRequestRecord(uint sendId) {
      /* Benchmark of 500 item array with worst-case scenario of scanning them all (while MSFS was running :).
      |         Method |     Mean |     Error |    StdDev |  Gen 0 | Allocated |
      |--------------- |---------:|----------:|----------:|-------:|----------:|
      |  FindWithLoop  | 1.618 us | 0.0273 us | 0.0255 us |      - |         - |
      | FirstOrDefault | 3.706 us | 0.0611 us | 0.0572 us | 0.0038 |      32 B |
      */
      // start at 10 records back from current index since it is more likely that the sendId error relates to a recent request than an old one
      int i = (MAX_STORED_REQUEST_RECORDS + _reqTrackIndex - 10) % MAX_STORED_REQUEST_RECORDS, e = i;
      for (bool first = true; i != e || first; i = (i + 1) % MAX_STORED_REQUEST_RECORDS, first = false) {
        if (_requestTracking[i]?.dwSendId == sendId)
          return _requestTracking[i];
      }
      return new RequestTrackingData(sendId);
      //return _requestTracking.FirstOrDefault(r => r.dwSendId == sendId) ?? new RequestTrackingData(sendId);
    }

    #endregion SimConnect Request Tracking

    #region IDisposable Support
    private bool disposedValue; // To detect redundant calls

    protected virtual void Dispose(bool disposing) {
      if (!disposedValue) {
        if (disposing) {
          // Dispose managed state (managed objects).
          try {

            StopSimConnect();
            _wlib?.Dispose();
            _scReady?.Dispose();
            _scQuit?.Dispose();
          }
          catch { }
        }

        disposedValue = true;
      }
    }

    // This code added to correctly implement the disposable pattern.
    public void Dispose() {
      // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
      Dispose(true);
      GC.SuppressFinalize(this);
    }
    #endregion IDisposable Support

  }

}
