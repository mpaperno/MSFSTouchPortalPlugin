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
using MSFSTouchPortalPlugin.Configuration;
using MSFSTouchPortalPlugin.Enums;
using MSFSTouchPortalPlugin.Interfaces;
using MSFSTouchPortalPlugin.Types;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

#if WASIM
using WASimCommander.CLI.Enums;
using WASimCommander.CLI.Structs;
using WASMLib = WASimCommander.CLI.Client.WASimClient;
// yeah so we got some name conflicts
using WasimLogLevel =  WASimCommander.CLI.Enums.LogLevel;
using ILogLevel = Microsoft.Extensions.Logging.LogLevel;
using WasmUptPeriod = WASimCommander.CLI.Enums.UpdatePeriod;
#endif
using SimConUpdPeriod = MSFSTouchPortalPlugin.Enums.UpdatePeriod;

namespace MSFSTouchPortalPlugin.Services
{
  /// <summary>
  /// Wrapper for SimConnect
  /// </summary>
  internal class SimConnectService : ISimConnectService
  {
    #region Public
    public event DataUpdateEventHandler OnDataUpdateEvent;
    public event RecvEventEventHandler OnEventReceived;
    public event ConnectEventHandler OnConnect;
    public event DisconnectEventHandler OnDisconnect;
    public event ExceptionEventHandler OnException;
    public event SimVarErrorEventHandler OnSimVarError;
#if WASIM
    public event LocalVarsListUpdatedHandler OnLVarsListUpdated;
#endif

    // hResult
    public const uint S_OK = 0;
    public const uint E_FAIL = 0x80004005;          // general SimConnect failure
    public const uint E_INVALIDARG = 0x80070057;    // returned by SimConnect if the config index is invalid
    public const uint E_TIMEOUT = 0x800708CA;       // we return this if SimConnect() was created OK but no OnOpen was received
    // SIMCONNECT_RECV_EVENT.dwData value for "View" event, from SimConnect.h
    public const uint VIEW_EVENT_DATA_COCKPIT_2D = 0x00000001;      // 2D Panels in cockpit view
    public const uint VIEW_EVENT_DATA_COCKPIT_3D = 0x00000002;      // Virtual (3D) panels in cockpit view
    public const uint VIEW_EVENT_DATA_ORTHOGONAL = 0x00000004;      // Orthogonal (Map) view

    public bool IsConnected => _connected;
    public bool WasmAvailable => WasmStatus != WasmModuleStatus.NotFound;
#if WASIM
    public WasmModuleStatus WasmStatus { get; private set; } = WasmModuleStatus.Unknown;
#else
    public WasmModuleStatus WasmStatus => WasmModuleStatus.NotFound;
#endif

    public SimConnectService(ILogger<SimConnectService> logger, SimVarCollection simVarCollection)
    {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _simVarCollection = simVarCollection ?? throw new ArgumentNullException(nameof(simVarCollection));

      _simVarCollection.OnSimVarAdded += RegisterDataRequest;
      _simVarCollection.OnSimVarRemoved += DeregisterDataRequest;
    }

    #endregion Public

    const int SIM_CONNECT_TIMEOUT_MS  = 5000;   // SimConnect "Open" event timeout after successfully creating SimConnect() instance (should really be almost instant if everything is OK)
    const int MSG_RCV_WAIT_TIME_MS    = 5000;   // SimConnect.ReceiveMessage() wait time
    // maximum stored requests for error tracking (adding is fast but search is 0(n)), should be large enough to handle flood of requests at initial connection
    const int MAX_STORED_REQUEST_RECORDS = 500;
    const uint WASM_CLIENT_ID = 0xFF700C49;   // ID to use for connecting with WASimClient; the high byte is replaced at Init() time.

    bool _connected;
    bool _connecting;
    bool _simConnectHasQuit = false;  // flag to prevent trying to invoke SimConnect in certain situations, eg. after a Quit or crash.
    bool WasmConnected => WasmStatus == WasmModuleStatus.Connected;
    bool WasmInitialized => WasmStatus > WasmModuleStatus.NotFound;
    int _reqTrackIndex = 0;  // current write slot index in _requestTracking array
    SimulatorInfo _simInfo;
    Task _messageWaitTask;
    SimConnect _simConnect = null;
#if WASIM
    WASMLib _wlib = null;
#endif

    readonly ILogger<SimConnectService> _logger;
    readonly SimVarCollection _simVarCollection;
    readonly EventWaitHandle _scReady = new EventWaitHandle(false, EventResetMode.AutoReset);
    readonly AutoResetEvent _scQuit = new(false);
    readonly RequestTrackingData[] _requestTracking = new RequestTrackingData[MAX_STORED_REQUEST_RECORDS];  // rolling buffer array for request tracking

    // SimConnect method delegates, for centralized interaction in InvokeSimMethod()
    Action<Enum>             ClearDataDefinitionDelegate;
    Action<Enum, string>     MapClientEventToSimEventDelegate;
    Action<Enum, string>     SubscribeToSystemEventDelegate;
    Action<Enum, Enum, uint, SIMCONNECT_SIMOBJECT_TYPE>   RequestDataOnSimObjectTypeDelegate;
#if FSX
    Action<uint, Enum, uint, Enum, SIMCONNECT_EVENT_FLAG> TransmitClientEventDelegate;
#else
    Action<uint, Enum, Enum, SIMCONNECT_EVENT_FLAG, uint, uint, uint, uint, uint> TransmitClientEventEx1Delegate;
#endif
    Action<Enum, uint, SIMCONNECT_DATA_SET_FLAG, object>  SetDataOnSimObjectDelegate;
    Action<Enum, string, string, SIMCONNECT_DATATYPE, float, uint> AddToDataDefinitionDelegate;
    Action<Enum, Enum, uint, SIMCONNECT_PERIOD, SIMCONNECT_DATA_REQUEST_FLAG, uint, uint, uint> RequestDataOnSimObjectDelegate;

    readonly Dictionary<Type, Action<Enum> > _registerDataDelegates = new();

    uint StartSimConnect(uint configIndex)
    {
      if (_connecting || _connected)
        return IsConnected? S_OK : E_FAIL;

      uint ret = E_FAIL;
      _connecting = true;
      _logger.LogInformation("Connecting to SimConnect...");

#if WASIM
      _wlib?.setNetworkConfigurationId((int)configIndex);
#endif

      try {
        _simConnect = new SimConnect(PluginConfig.PLUGIN_ID, IntPtr.Zero, 0, _scReady, configIndex);

        // Set up minimum handlers to receive connection notification
        _simConnect.OnRecvOpen += new SimConnect.RecvOpenEventHandler(Simconnect_OnRecvOpen);
        _simConnect.OnRecvException += new SimConnect.RecvExceptionEventHandler(Simconnect_OnRecvException);
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
        _logger.LogDebug("Connection to SimConnect failed: [{hr:X}] {message}", e.HResult, e.Message);
        StopSimConnect();
        unchecked { ret = (uint)e.HResult; }
      }

      _connecting = false;
      return ret;
    }

    void OnConnected(SIMCONNECT_RECV_OPEN data)
    {
      _connected = true;
      _simInfo = new SimulatorInfo(data);
      _logger.LogInformation((int)EventIds.PluginInfo, "Connected to {info}", _simInfo.ToString());

      // System Events
      _simConnect.OnRecvQuit      += Simconnect_OnRecvQuit;

      // Sim mapped events
      _simConnect.OnRecvEvent         += Simconnect_OnRecvEvent;
      _simConnect.OnRecvEventFilename += Simconnect_OnRecvFilename;

      // Sim Data
      _simConnect.OnRecvSimobjectData       += Simconnect_OnRecvSimObjectData;
      _simConnect.OnRecvSimobjectDataBytype += Simconnect_OnRecvSimobjectDataBytype;

      // Method delegates
      MapClientEventToSimEventDelegate          = _simConnect.MapClientEventToSimEvent;
#if FSX
      TransmitClientEventDelegate               = _simConnect.TransmitClientEvent;
#else
      TransmitClientEventEx1Delegate            = _simConnect.TransmitClientEvent_EX1;
#endif
      ClearDataDefinitionDelegate               = _simConnect.ClearDataDefinition;
      AddToDataDefinitionDelegate               = _simConnect.AddToDataDefinition;
      RequestDataOnSimObjectDelegate            = _simConnect.RequestDataOnSimObject;
      RequestDataOnSimObjectTypeDelegate        = _simConnect.RequestDataOnSimObjectType;
      SetDataOnSimObjectDelegate                = _simConnect.SetDataOnSimObject;
      SubscribeToSystemEventDelegate            = _simConnect.SubscribeToSystemEvent;

      _registerDataDelegates.Clear();
      _registerDataDelegates.Add(typeof(double),    _simConnect.RegisterDataDefineStruct<double>);
      _registerDataDelegates.Add(typeof(uint),      _simConnect.RegisterDataDefineStruct<uint>);
      _registerDataDelegates.Add(typeof(long),      _simConnect.RegisterDataDefineStruct<long>);
      _registerDataDelegates.Add(typeof(StringVal), _simConnect.RegisterDataDefineStruct<StringVal>);

#if FSX
      SetupRequestTracking();
#endif
#if WASIM
      ConnectWasm();
#endif
      RegisterRequests();
      SubscribeSystemEvents();
      OnConnect?.Invoke(_simInfo);
#if WASIM
      RequestLocalVariablesList();
#endif
    }

    void StopSimConnect()
    {
      if (_messageWaitTask != null) {
        _scQuit.Set();  // trigger message wait task to exit
        var sw = System.Diagnostics.Stopwatch.StartNew();
        while (_messageWaitTask.Status == TaskStatus.Running && sw.ElapsedMilliseconds <= MSG_RCV_WAIT_TIME_MS)
          Thread.Sleep(2);
        if (_messageWaitTask.Status == TaskStatus.Running)
          _logger.LogWarning((int)EventIds.Ignore, "Message wait task timed out while stopping.");
        try { _messageWaitTask.Dispose(); }
        catch { /* ignore in case it hung */ }
      }
#if WASIM
      if (WasmConnected) {
        _wlib?.disconnectSimulator();
        WasmStatus = WasmModuleStatus.Found;
      }
#endif

      if (_simConnect != null) {
        DeregisterRequests(true);
        // Dispose serves the same purpose as SimConnect_Close()
        try {
          _simConnect.Dispose();
          _logger.LogDebug("SimConnect disposed");
        }
        catch (Exception e) {
          _logger.LogWarning((int)EventIds.Ignore, e, "Exception while trying to dispose SimConnect client.");
        }
      }
      _simConnect = null;
      _messageWaitTask = null;
      _simConnectHasQuit = false;
      _connected = false;
    }

    // runs in separate task/thread
    void ReceiveMessages()
    {
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
        _logger.LogError((int)EventIds.Ignore, e, "ReceiveMessages task exception {HResult:X}, disconnecting.", e.HResult);
        _simConnectHasQuit = true;
        Task.Run(Disconnect);  // async to avoid deadlock
        // COMException (0xC000014B) = broken pipe (sim crashed/network loss on a Pipe type connection)
      }
      _logger.LogDebug("ReceiveMessages task stopped.");
    }

    // Centralized SimConnect method handler
    bool InvokeSimMethod(Delegate method, params object[] args)
    {
      if (method == null || !_connected || _simConnectHasQuit)
        return false;
      try {
        _logger.LogTrace("Invoking: {methodName}({args})", method.Method.Name, string.Join(", ", args));
        lock(_simConnect) {
          method.DynamicInvoke(args);
          AddRequestRecord(method.Method, args);
        }
        return true;
      }
      catch (COMException e) {
        _logger.LogWarning((int)EventIds.Ignore, "SimConnect returned an error: [{hResult:X}] {message} <site: '{targetSite}'; source: '{source}'", e.HResult, e.Message, e.TargetSite, e.Source);
      }
      catch (Exception e) {
        _logger.LogError((int)EventIds.Ignore, e, "Method invocation failed with system exception: [{hResult:X}] {message}", e.HResult, e.Message);
      }
      return false;
    }

    void RegisterRequests(char[] types = null)
    {
      if (!_connected)
        return;
      foreach (var simVar in _simVarCollection) {
        if (simVar.RegistrationStatus != SimVarRegistrationStatus.Registered && (types == null || Array.IndexOf(types, simVar.VariableType) > -1))
          RegisterDataRequest(simVar);
      }
    }

    void DeregisterRequests(bool simConnectOnly = true, char[] types = null)
    {
      foreach (var simVar in _simVarCollection) {
        if ((!simConnectOnly || simVar.DataProvider == SimVarDataProvider.SimConnect) && (types == null || Array.IndexOf(types, simVar.VariableType) > -1)) {
          if (_connected)
            DeregisterDataRequest(simVar);
          else
            simVar.RegistrationStatus = SimVarRegistrationStatus.Unregistered;
        }
      }
    }

    void RegisterDataRequest(SimVarItem simVar)
    {
      if (!_connected) {
        simVar.RegistrationStatus = SimVarRegistrationStatus.Unregistered;
        return;
      }
      if (!string.IsNullOrEmpty(simVar.SimVersion) && !_simInfo.AppVersionString.StartsWith(simVar.SimVersion, StringComparison.Ordinal)) {
        OnSimVarError?.Invoke(simVar.Def, SimVarErrorType.SimVersion, _simInfo.AppVersionString);
        return;
      }

      bool scSupported = SimVarItem.SimConnectSupported(simVar.VariableType);
#if WASIM
      if (!scSupported && !WasmInitialized) {
        OnSimVarError?.Invoke(simVar.Def, SimVarErrorType.VarType, $"can not request '{simVar.VariableType}' type variable without WASM integration");
        return;
      }
      if (!scSupported || (simVar.NeedsScheduledRequest && WasmInitialized)) {
        simVar.DataProvider = SimVarDataProvider.WASimClient;
        simVar.RegistrationStatus = RegisterToWasm(simVar);
      }
      else {
        simVar.DataProvider = SimVarDataProvider.SimConnect;
        simVar.RegistrationStatus = RegisterToSimConnect(simVar);
      }
#else
      if (!scSupported) {
        OnSimVarError?.Invoke(simVar.Def, SimVarErrorType.VarType, "invalid type for this Simulator version");
        return;
      }
      simVar.DataProvider = SimVarDataProvider.SimConnect;
      simVar.RegistrationStatus = RegisterToSimConnect(simVar);
#endif
      if (simVar.RegistrationStatus == SimVarRegistrationStatus.Error)
        OnSimVarError?.Invoke(simVar.Def, SimVarErrorType.Registration);
      _logger.LogTrace("Registered? request for {var}", simVar.ToDebugString());
    }

    void DeregisterDataRequest(SimVarItem simVar)
    {
      if (simVar.RegistrationStatus != SimVarRegistrationStatus.Registered)
        return;
      if (simVar.DataProvider == SimVarDataProvider.SimConnect) {
        // We need to first suspend updates for this variable before removing it, otherwise it seems SimConnect will sometimes crash
        if (simVar.UpdatePeriod != SimConUpdPeriod.Never && simVar.UpdatePeriod != SimConUpdPeriod.Millisecond) {
          var oldPeriod = simVar.UpdatePeriod;
          simVar.UpdatePeriod = SimConUpdPeriod.Never;
          RequestDataOnSimObject(simVar);
          // Now set it back to original value (in case it is not actually being deleted).
          simVar.UpdatePeriod = oldPeriod;
        }
        // Now we can remove it.
        InvokeSimMethod(ClearDataDefinitionDelegate, simVar.Def);
      }
#if WASIM
      else {
        _wlib?.removeDataRequest((uint)simVar.Def);
      }
#endif
      simVar.RegistrationStatus = SimVarRegistrationStatus.Unregistered;
      _logger.LogTrace("Removed data request for {var}", simVar.ToDebugString());
    }

    // SimConnect
    //

    SimVarRegistrationStatus RegisterToSimConnect(SimVarItem simVar)
    {
      if (!_registerDataDelegates.TryGetValue(simVar.StorageDataType, out var registerDataDelegate)) {
        _logger.LogError("Unable to register data storage for type '{type}'.", simVar.StorageDataType);
        return SimVarRegistrationStatus.Error;
      }

      string unitName = simVar.IsStringType ? null : simVar.Unit;
      string varName = simVar.VariableType != 'A' ? simVar.VariableType + ":" + simVar.SimVarName : simVar.SimVarName;
      if (!InvokeSimMethod(AddToDataDefinitionDelegate, simVar.Def, varName, unitName, simVar.SimConnectDataType, simVar.DeltaEpsilon, SimConnect.SIMCONNECT_UNUSED))
        return SimVarRegistrationStatus.Error;

      if (!InvokeSimMethod(registerDataDelegate, simVar.Def))
        return SimVarRegistrationStatus.Error;

      if (simVar.UpdatePeriod == SimConUpdPeriod.Never || simVar.UpdatePeriod == SimConUpdPeriod.Millisecond || RequestDataOnSimObject(simVar))
        return SimVarRegistrationStatus.Registered;
      return SimVarRegistrationStatus.Error;
    }

    void SubscribeSystemEvents()
    {
      for (EventIds eventId = EventIds.SimEventNone + 1; eventId < EventIds.SimEventLast; ++eventId)
        SubscribeToSystemEvent(eventId, eventId.ToString());
    }

    bool MapClientEventToSimEvent(Enum eventId, string eventName)
    {
      return InvokeSimMethod(MapClientEventToSimEventDelegate, eventId, eventName);
    }

    bool TransmitClientEvent(Enum eventId, uint[] data)
    {
      EventIds evId = (EventIds)eventId;
      if (evId <= EventIds.InternalEventsLast || data.Length < 5)
        return false;
#if FSX
      return InvokeSimMethod(TransmitClientEventDelegate, SimConnect.SIMCONNECT_OBJECT_ID_USER, eventId, data[0], (Groups)SimConnect.SIMCONNECT_GROUP_PRIORITY_HIGHEST, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
#elif WASIM
      if (WasmConnected && evId < EventIds.DynamicEventInit)
        return _wlib.sendKeyEvent((uint)evId, data[0], data[1], data[2], data[3], data[4]) == HR.OK;
#endif
      return InvokeSimMethod(TransmitClientEventEx1Delegate, SimConnect.SIMCONNECT_OBJECT_ID_USER, eventId, (Groups)SimConnect.SIMCONNECT_GROUP_PRIORITY_HIGHEST, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY, data[0], data[1], data[2], data[3], data[4]);
    }

    bool RequestDataOnSimObjectType(SimVarItem simVar, SIMCONNECT_SIMOBJECT_TYPE objectType = SIMCONNECT_SIMOBJECT_TYPE.USER)
    {
      return InvokeSimMethod(RequestDataOnSimObjectTypeDelegate, simVar.Def, simVar.Def, 0U, objectType);
    }

    bool RequestDataOnSimObject(SimVarItem simVar, uint objectId = (uint)SIMCONNECT_SIMOBJECT_TYPE.USER)
    {
      return InvokeSimMethod(
        RequestDataOnSimObjectDelegate,
        simVar.Def, simVar.Def, objectId, (SIMCONNECT_PERIOD)simVar.UpdatePeriod, SIMCONNECT_DATA_REQUEST_FLAG.CHANGED, 0U, simVar.UpdateInterval, 0U
      );
    }

    bool SetDataOnSimObject(SimVarItem simVar, uint objectId = (uint)SIMCONNECT_SIMOBJECT_TYPE.USER) {
      return InvokeSimMethod(SetDataOnSimObjectDelegate, simVar.Def, objectId, SIMCONNECT_DATA_SET_FLAG.DEFAULT, simVar.Value);
    }

    //  WASM
    //

#if WASIM
    void SetupWasm()
    {
      _wlib?.Dispose();
      var clientId = (WASM_CLIENT_ID & 0x00FFFFFFU) | ((uint)Settings.WasimClientIdHighByte.ByteValue << 24);
      _logger.LogDebug("Creating WASimClient with ID '{id:X08}'.", clientId);
      _wlib = new WASMLib(clientId, PluginConfig.AppRootFolder);
      _wlib.OnClientEvent += WASMLib_OnClientEvent;
      _wlib.OnLogRecordReceived += WASMLib_OnLogEntryReceived;
      _wlib.OnDataReceived += WASMLib_OnValueChanged;
      _wlib.OnListResults += WASMLib_OnListResults;
      _wlib.setLogLevel(GetWasimLogLevel(_logger), LogFacility.Remote, LogSource.Client);
      _wlib.setLogLevel(WasimLogLevel.None, LogFacility.Console, LogSource.Client);
      _wlib.setLogLevel(WasimLogLevel.Warning, LogFacility.Remote, LogSource.Server);
    }

    void ConnectWasm()
    {
      if (_wlib == null) {
        _logger.LogWarning((int)EventIds.PluginError, "WASM Client not initialized, integration disabled.");
        return;
      }

      HR hr;
      int count = 0;
      // Retry simulator connection several times in case of initial failure. Unlikely, but observed once during testing.
      do {
        hr = _wlib.connectSimulator(2000U);
      }
      while (hr == HR.TIMEOUT && ++count < 11);

      // Now attempt WASM server connection.
      if (hr == HR.OK)
        hr = _wlib.connectServer();
      else
        _logger.LogError("WASM Client could not connect to SimConnect for unknown reason. Result: {hr}", hr.ToString());

      if (hr != HR.OK) {
        WasmStatus = WasmModuleStatus.NotFound;
        _logger.LogWarning((int)EventIds.PluginError, "WASM Server not found or couldn't connect, integration disabled.");
        return;
      }
      WasmStatus = WasmModuleStatus.Connected;
      _logger.LogInformation("Connected to WASimConnect Server v{serverVer:X08}", _wlib.serverVersion());
    }

    SimVarRegistrationStatus RegisterToWasm(SimVarItem simVar)
    {
      if (!WasmInitialized) {
        OnSimVarError?.Invoke(simVar.Def, SimVarErrorType.VarType, $"Can not request '{simVar.VariableType}' type variable without WASM integration.");
        return SimVarRegistrationStatus.Error;
      }

      byte simVarIdx = 0;
      var simVarName = simVar.SimVarName.AsSpan();
      if (simVar.VariableType == 'L') {
        // Check if L var exists yet
        HR hr = _wlib.lookup(LookupItemType.LocalVariable, simVar.SimVarName, out int varId);
        _logger.LogDebug("Got ID lookup for '{varName}' with ID {id} and HResult {hr}", simVar.SimVarName, varId, hr);
        if (hr != HR.OK || varId < 0) {
          _logger.LogWarning((int)EventIds.SimError, "Local variable '{varName}' not found at simulator. Retry later.", simVar.SimVarName);
          return SimVarRegistrationStatus.Error;
        }
      }
      else if (simVar.VariableType == 'A') {
        // Check for and extract possible index number in variable name.
        var simVarIdxSpan = simVarName[^4..];
        if (simVarIdxSpan.IndexOf(':') is var colIdx && colIdx > -1) {
          if (!byte.TryParse(simVarIdxSpan[(colIdx + 1)..], out simVarIdx)) {
            simVarIdx = 0;
            _logger.LogWarning("Could not parse SimVar index from name {simVarName}", simVar.SimVarName);
          }
          simVarName = simVarName[..^(4 - colIdx)];
        }
      }
      // Convert the update period enum and also adjust the interval if needed (eg. seconds to ms).
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
        nameOrCode = new(simVarName.ToString()),
        unitName = new(simVar.Unit),
        simVarIndex = simVarIdx,
      };
      if (_wlib.saveDataRequest(dr) == HR.OK)
        return SimVarRegistrationStatus.Registered;
      return SimVarRegistrationStatus.Error;
    }

    bool ExecuteCalculatorCode(string code)
    {
      if (!WasmConnected || string.IsNullOrWhiteSpace(code))
        return false;
      _logger.LogDebug("Sending calculator string: '{code}'", code);
      return _wlib.executeCalculatorCode(code) == HR.OK;
    }

    void WASMLib_OnClientEvent(ClientEvent ev)
    {
      _logger.LogDebug("WASimClient Event: {ev}", ev);
    }

    void WASMLib_OnListResults(ListResult lr)
    {
      _logger.LogDebug("Got WASM list results of type: {type}, {count} item(s).", lr.listType, lr.list.Count);
      OnLVarsListUpdated?.Invoke(lr.list);
    }

    void WASMLib_OnValueChanged(DataRequestRecord dr)
    {
      _logger.LogTrace("Got WASM data for Request: {dr}", dr.ToString());
      OnDataUpdateEvent?.Invoke((Definition)dr.requestId, (Definition)dr.requestId, dr);
    }

    void WASMLib_OnLogEntryReceived(LogRecord log, LogSource src)
    {
      _logger.Log(WasmLogLevelToLoggerLevel(log.level), (int)EventIds.Ignore, "[WASM] {src} - {log}", src, log.message);
    }

#else
    void SetupWasm() {}
#endif  // WASIM

    // --------------------------------------
    #region Public Interface Methods
    // --------------------------------------

    public void Init() {
      SetupWasm();
    }

    public uint Connect(uint configIndex = 0) {
      return StartSimConnect(configIndex);
    }

    public void Disconnect() {
      StopSimConnect();
      // Invoke Handler
      OnDisconnect?.Invoke();
    }

    public bool TransmitClientEvent(EventMappingRecord eventRecord, uint[] data)
    {
      if (eventRecord == null)
        return false;

      if ((EventIds)eventRecord.EventId == EventIds.None) {
        if (string.IsNullOrEmpty(eventRecord.EventName))
          return false;
#if WASIM
        if (WasmConnected && !eventRecord.EventName.StartsWith('#') && !eventRecord.EventName.Contains('.')) {
          if (_wlib.lookup(LookupItemType.KeyEventId, eventRecord.EventName, out int keyEvtId) == HR.OK)
            eventRecord.EventId = (EventIds)keyEvtId;
          else
            _logger.LogWarning((int)EventIds.SimError, "Could not find Key Event ID for event name {evName}, will try with SimConnect.", eventRecord.EventName);
          // fall back to SimConnect if lookup fails
        }
        if ((EventIds)eventRecord.EventId == EventIds.None) {
          eventRecord.EventId = ActionEventType.NextId();
          if (!MapClientEventToSimEvent(eventRecord.EventId, eventRecord.EventName))
            return false;
        }
#else
        eventRecord.EventId = ActionEventType.NextId();
        if (!MapClientEventToSimEvent(eventRecord.EventId, eventRecord.EventName))
          return false;
#endif
      }
      return TransmitClientEvent(eventRecord.EventId, data);
    }

    public bool CanRequestVariableType(char varType) => (WasmAvailable || SimVarItem.SimConnectSupported(varType));
    public bool CanSetVariableType(char varType) => (WasmConnected || SimVarItem.SimConnectSupported(varType));

    public bool SetVariable(char varType, string varName, object value, string unit = "", bool createLocal = false)
    {
      if (!IsConnected)
        return false;

      if (!CanSetVariableType(varType)) {
        _logger.LogError("Cannot set '{varType}' type variable '{varName}' without active WASM module connection.", varType, varName);
        return false;
      }


#if WASIM
      // If WASM is available, take the shorter and better route.
      // One exception is for L vars which have a custom Unit specified... WASM can't handle that.
      if (WasmConnected && !(varType == 'L' && (string.IsNullOrWhiteSpace(unit) || unit == "number"))) {
        if (varType == 'L') {
          _logger.LogDebug("Setting L variable {varName} to {val}", varName, (double)value);
          if (createLocal)
            return _wlib.setOrCreateLocalVariable(varName, (double)value) == HR.OK;
          return _wlib.setLocalVariable(varName, (double)value) == HR.OK;
        }

        string calcCode;
        if (varType == 'Q')
          calcCode = value.ToString();
        else if (unit == "string")
          calcCode = $"'{value}' (>{varType}:{varName})";
        else if (!string.IsNullOrEmpty(unit))
          calcCode = $"{value} (>{varType}:{varName}, {unit})";
        else
          calcCode = $"{value} (>{varType}:{varName})";
        return ExecuteCalculatorCode(calcCode);
      }
#endif

      if (!_simVarCollection.TryGetBySimName(varName, out SimVarItem simVar)) {
        simVar = PluginConfig.CreateDynamicSimVarItem(varType, varName, Groups.None, unit, 0);
        if (simVar == null) {
          _logger.LogError("Could not create a valid variable from name: {varName}", varName);
          return false;
        }
        simVar.DefinitionSource = SimVarDefinitionSource.Temporary;
        simVar.UpdatePeriod = SimConUpdPeriod.Never;
        _simVarCollection.Add(simVar);
        _logger.LogDebug("Added temporary SimVar {simVarId} for {simVarName}", simVar.Id, simVar.SimVarName);
      }
      else if (simVar.RegistrationStatus == SimVarRegistrationStatus.Error) {
        _logger.LogWarning("Variable '{simVarName}' from name '{varName}' is invalid due to previous registration error.", simVar.SimVarName, varName);
        return false;
      }
      if (!simVar.SetValue(value)) {
        _logger.LogError("Could not set value '{value}' for SimVar: '{varId}' From Name: '{varName}'", value.ToString(), simVar.Id, varName);
        return false;
      }

      _logger.LogDebug("Sending {simVar} with value {value}", simVar.ToString(), simVar.Value);
      return SetDataOnSimObject(simVar);
    }

    public bool RequestLocalVariablesList() {
#if WASIM
      return WasmConnected && _wlib.list(LookupItemType.LocalVariable) == HR.OK;
#else
      return false;
#endif
    }

    public bool RequestVariableValueUpdate(SimVarItem simVar) {
#if WASIM
      if (simVar.DataProvider == SimVarDataProvider.WASimClient)
        return _wlib.updateDataRequest((uint)simVar.Def) == HR.OK;
#endif
      if (simVar.DataProvider == SimVarDataProvider.SimConnect)
        return RequestDataOnSimObjectType(simVar);
      return false;
    }

    public void RetryRegisterVarRequests(char varType) {
      RegisterRequests(new[] { varType });
    }

    #endregion Public Interface Methods

    #region SimConnect Event Handlers

    private void Simconnect_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data) {
      Task.Run(delegate { OnConnected(data); });
    }

    private void Simconnect_OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data) {
      _logger.LogInformation("Received shutdown command from SimConnect, disconnecting.");
      _simConnectHasQuit = true;
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
      if (data.dwException == 0)  // SIMCONNECT_EXCEPTION.NONE
        return;
      RequestTrackingData record = GetRequestRecord(data.dwSendID, (SIMCONNECT_EXCEPTION)data.dwException, data.dwIndex);
      _logger.LogWarning((int)EventIds.SimError, "SimConnect Request Error: {error}", record.ToString());
      OnException?.Invoke(record);

      if (record.aArguments.Length > 0 && record.aArguments[0] is Definition defId) {
        OnSimVarError?.Invoke(defId, SimVarErrorType.SimConnectError, record);
      }
    }

    private void Simconnect_OnRecvEvent(SimConnect sender, SIMCONNECT_RECV_EVENT data) {
      EventIds evId = (EventIds)data.uEventID;
      Groups gId = (Groups)data.uGroupID;
      _logger.LogDebug("Simconnect_OnRecvEvent Received: Group: {gId}; Event: {evName}; Data: {dwData}", gId, evId, data.dwData);
      OnEventReceived?.Invoke(evId, gId, data.dwData);
    }

    string _lastLoadedAircraft = "";

    private void Simconnect_OnRecvFilename(SimConnect sender, SIMCONNECT_RECV_EVENT_FILENAME data) {
      EventIds evId = (EventIds)data.uEventID;
      Groups gId = (Groups)data.uGroupID;
      _logger.LogDebug("Simconnect_OnRecvFilename Received: Group: {gId}; Event: {evId}; Data: {fileName}", gId, evId, data.szFileName);
      OnEventReceived?.Invoke(evId, gId, data.szFileName);
#if !FSX
      if (evId == EventIds.AircraftLoaded && data.szFileName != _lastLoadedAircraft) {
          _lastLoadedAircraft = data.szFileName;
          if (WasmAvailable)
            Task.Run(RequestLocalVariablesList);
      }
#endif
    }

    #endregion SimConnect Event Handlers

    #region SimConnect Request Tracking

#if FSX
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

    private void SetupRequestTracking()
    {
      // Get direct access to the SimConnect handle, to use functions otherwise not supported.
      try {
        _hSimConnect = (IntPtr)_fiSimConnect.GetValue(_simConnect);
        _reqTrackIndex = 0;
      }
      catch (Exception e) {
        _hSimConnect = IntPtr.Zero;
        _logger.LogError((int)EventIds.Ignore, e, "Exception trying to get handle to SimConnect:");
      }
    }
#endif // FSX

    private void AddRequestRecord(MethodInfo info, params object[] args) {
      if (_simConnect == null)
        return;
      try {
#if FSX
        if (_hSimConnect == IntPtr.Zero)
          return;
        if (SimConnect_GetLastSentPacketID(_hSimConnect, out uint dwSendID) != S_OK) {
          _logger.LogWarning((int)EventIds.Ignore, "SimConnect_GetLastSentPacketID returned E_FAIL");
          return;
        }
#else
        uint dwSendID = _simConnect.GetLastSentPacketID();
#endif
        if (dwSendID > 0) {
          _requestTracking[_reqTrackIndex] = new RequestTrackingData(dwSendID, info.Name, info.GetParameters(), args);
          _reqTrackIndex = (_reqTrackIndex + 1) % MAX_STORED_REQUEST_RECORDS;
        }
      }
      catch (Exception e) {
        _logger.LogWarning((int)EventIds.Ignore, "SimConnect.GetLastSentPacketID returned [{result:X}] {message}", e.HResult, e.Message);
#if FSX
        _hSimConnect = IntPtr.Zero;
#endif
      }
    }

    private RequestTrackingData GetRequestRecord(uint sendId, SIMCONNECT_EXCEPTION err, uint errIdx) {
      /* Benchmark of 500 item array with worst-case scenario of scanning them all (while MSFS was running :).
      |         Method |     Mean |     Error |    StdDev |  Gen 0 | Allocated |
      |--------------- |---------:|----------:|----------:|-------:|----------:|
      |  FindWithLoop  | 1.618 us | 0.0273 us | 0.0255 us |      - |         - |
      | FirstOrDefault | 3.706 us | 0.0611 us | 0.0572 us | 0.0038 |      32 B |
      */
      // start at 10 records back from current index since it is more likely that the sendId error relates to a recent request than an old one
      int i = (MAX_STORED_REQUEST_RECORDS + _reqTrackIndex - 10) % MAX_STORED_REQUEST_RECORDS, e = i;
      for (bool first = true; i != e || first; i = (i + 1) % MAX_STORED_REQUEST_RECORDS, first = false) {
        if ((_requestTracking[i] is var rtd) && rtd?.dwSendId == sendId) {
          rtd.eException = err;
          rtd.dwExceptionIndex = errIdx;
          return rtd;
        }
      }
      return new RequestTrackingData(sendId, err, errIdx);
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
#if WASIM
            _wlib?.Dispose();
#endif
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

#if WASIM
    #region WASMCommander Utilities

    // convert WASimCommander LogLevel to ILogger LogLevel
    static ILogLevel WasmLogLevelToLoggerLevel(WasimLogLevel level)
    {
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
    static WasimLogLevel GetWasimLogLevel(ILogger logger)
    {
      // 'cuz.. yea... having an ILogger.CurrentLevel property is too much to ask for I guess...
      if (logger.IsEnabled(ILogLevel.Trace))
        return WasimLogLevel.Trace;
      if (logger.IsEnabled(ILogLevel.Debug))
        return WasimLogLevel.Debug;
      if (logger.IsEnabled(ILogLevel.Information))
        return WasimLogLevel.Info;
      if (logger.IsEnabled(ILogLevel.Warning))
        return WasimLogLevel.Warning;
      if (logger.IsEnabled(ILogLevel.Error))
        return WasimLogLevel.Error;
      if (logger.IsEnabled(ILogLevel.Critical))
        return WasimLogLevel.Critical;
      return WasimLogLevel.None;
    }

    static WasmUptPeriod PluginPeriodToWasmPeriod(SimConUpdPeriod p, ref uint interval)
    {
      if (p == SimConUpdPeriod.Second)
        interval *= 1000;
      return p switch {
        SimConUpdPeriod.Never => WasmUptPeriod.Never,
        SimConUpdPeriod.Once => WasmUptPeriod.Once,
        SimConUpdPeriod.Millisecond or SimConUpdPeriod.Second => WasmUptPeriod.Millisecond,
        _ => WasmUptPeriod.Tick,
      };
    }

    #endregion
#endif  // WASIM

  }

}
