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

namespace MSFSTouchPortalPlugin.Services
{
  /// <summary>
  /// Wrapper for SimConnect
  /// </summary>
  internal class SimConnectService : ISimConnectService {
    #region DLL Imports
    // get pointer to console window for SimConnect binding
    [DllImport("kernel32.dll")]
    static extern IntPtr GetConsoleWindow();

    // Set up tracking requests by their SendID for diagnostic purposes with Simconnect_OnRecvException()
    // https://docs.flightsimulator.com/html/Programming_Tools/SimConnect/API_Reference/Debug/SimConnect_GetLastSentPacketID.htm
    IntPtr _hSimConnect = IntPtr.Zero;  // native SimConnect handle pointer
    // Import methods from the actual SimConnect client which aren't available in the C# wrapper.
    [DllImport("SimConnect.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    static extern int /* HRESULT */ SimConnect_GetLastSentPacketID(IntPtr hSimConnect, out uint /* DWORD */ dwSendID);
#pragma warning disable S3011
    // Get a FieldInfo object on the SimConnect.hSimConnect private field variable so that we can query its value to get the handle to the actual SimConnect client.
    static readonly FieldInfo _fiSimConnect = typeof(SimConnect).GetField("hSimConnect", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
#pragma warning restore S3011
    #endregion DLL Imports

    public event DataUpdateEventHandler OnDataUpdateEvent;
    public event RecvEventEventHandler OnEventReceived;
    public event ConnectEventHandler OnConnect;
    public event DisconnectEventHandler OnDisconnect;
    public event ExceptionEventHandler OnException;

    // hResult
    public const uint S_OK = 0;
    public const uint E_FAIL = 0x80004005;
    public const uint E_INVALIDARG = 0x80070057;
    // SIMCONNECT_RECV_EVENT.dwData value for "View" event, from SimConnect.h
    public const uint VIEW_EVENT_DATA_COCKPIT_2D = 0x00000001;      // 2D Panels in cockpit view
    public const uint VIEW_EVENT_DATA_COCKPIT_3D = 0x00000002;      // Virtual (3D) panels in cockpit view
    public const uint VIEW_EVENT_DATA_ORTHOGONAL = 0x00000004;      // Orthogonal (Map) view

    // maximum stored requests for error tracking (adding is fast but search is 0(n)), should be large enough to handle flood of requests at initial connection
    const int MAX_STORED_REQUEST_RECORDS = 500;

    const uint NOTIFICATION_PRIORITY = 10000000;  // notification group priority for SimConnect.SetNotificationGroupPriority()
    const int WM_USER_SIMCONNECT = 0x0402;        // user event ID for SimConnect

    // enable AddNotification(), SetNotificationGroupPriorities(), and Simconnect_OnRecvEvent() for events we initiate; currently this serves no purpose except possible debug info.
    static readonly bool DEBUG_NOTIFICATIONS = false;

    readonly ILogger<SimConnectService> _logger;
    readonly IReflectionService _reflectionService;

    bool _connected;
    bool _connecting;
    int _reqTrackIndex = 0;  // current write slot index in _requestTracking array
    Task _messageWaitTask;
    SimConnect _simConnect;
    readonly EventWaitHandle _scReady = new EventWaitHandle(false, EventResetMode.AutoReset);
    readonly List<Definition> _addedDefinitions = new();  // keep track of added SimVar definitions to avoid redundant registrations
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

    public SimConnectService(ILogger<SimConnectService> logger, IReflectionService reflectionService) {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _reflectionService = reflectionService ?? throw new ArgumentNullException(nameof(reflectionService));
    }

    public bool IsConnected() => (_connected && _simConnect != null);

    public uint Connect(uint configIndex = 0) {
      if (_connecting || _simConnect != null)
        return _connected ? S_OK : E_FAIL;

      uint ret = E_FAIL;
      _connecting = true;
      _logger.LogInformation("Connecting to SimConnect...");

      try {
        _simConnect = new SimConnect("Touch Portal Plugin", GetConsoleWindow(), WM_USER_SIMCONNECT, _scReady, configIndex);

        _connected = true;

        // System Events
        _simConnect.OnRecvOpen += new SimConnect.RecvOpenEventHandler(Simconnect_OnRecvOpen);
        _simConnect.OnRecvQuit += new SimConnect.RecvQuitEventHandler(Simconnect_OnRecvQuit);
        _simConnect.OnRecvException += new SimConnect.RecvExceptionEventHandler(Simconnect_OnRecvException);

        // Sim mapped events
        _simConnect.OnRecvEvent         += Simconnect_OnRecvEvent;
        _simConnect.OnRecvEventFilename += Simconnect_OnRecvFilename;

        // Sim Data
        _simConnect.OnRecvSimobjectDataBytype += new SimConnect.RecvSimobjectDataBytypeEventHandler(Simconnect_OnRecvSimobjectDataBytype);
        _simConnect.OnRecvSimobjectData += new SimConnect.RecvSimobjectDataEventHandler(Simconnect_OnRecvSimObjectData);

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

        _messageWaitTask = Task.Run(ReceiveMessages);

        ret = S_OK;
      }
      catch (Exception e) {
        _connected = false;
        _logger.LogDebug("Connection to SimConnect failed: [{0:X}] {0}", e.HResult, e.Message);
        unchecked { ret = (uint)e.HResult; }
      }

      _connecting = false;
      return ret;
    }

    public void Disconnect() {
      if (!_connected)
        return;

      _connected = false;
      _scReady.Set();  // trigger message wait task to exit
      var sw = System.Diagnostics.Stopwatch.StartNew();
      while (_messageWaitTask.Status == TaskStatus.Running && sw.ElapsedMilliseconds <= 5000) {
        Thread.Sleep(2);
        _scReady.Set();
      }
      if (sw.ElapsedMilliseconds > 5000)
        _logger.LogWarning("Message wait task timed out while stopping.");
      try { _messageWaitTask.Dispose(); }
      catch { /* ignore in case it hung */ }

      // Dispose serves the same purpose as SimConnect_Close()
      try {
        _simConnect?.Dispose();
        _logger.LogDebug("SimConnect disposed");
      }
      catch (Exception e) {
        _logger.LogWarning(e, "Exception while trying to dispose SimConnect client.");
      }
      _simConnect = null;
      _messageWaitTask = null;
      _addedDefinitions.Clear();
      // Invoke Handler
      OnDisconnect?.Invoke();
    }

    // runs in separate task/thread
    private void ReceiveMessages() {
      _logger.LogDebug("ReceiveMessages task started.");
      try {
        while (_connected) {
          if (_scReady.WaitOne(5000) && _connected)
            _simConnect?.ReceiveMessage();
        }
      }
      catch (ObjectDisposedException) { /* ignore but exit */ }
      catch (Exception e) {
        _logger.LogError(e, "ReceiveMessages task exception, disconnecting.");
        Task.Run(Disconnect);  // async to avoid deadlock
      }
      _logger.LogDebug("ReceiveMessages task stopped.");
    }

    // Centralized SimConnect method handler
    private bool InvokeSimMethod(Delegate method, params object[] args) {
      if (method == null || !_connected)
        return false;
      try {
        _logger.LogTrace($"Invoking: {method.Method.Name}({string.Join(", ", args)})");
        method.DynamicInvoke(args);
        AddRequestRecord(method.Method, args);
        return true;
      }
      catch (COMException e) {
        _logger.LogWarning($"SimConnect returned an error: [{e.HResult:X}] {e.Message} <site: '{e.TargetSite}'; source: '{e.Source}'");
      }
      catch (Exception e) {
        _logger.LogError(e, $"Method invocation failed with system exception: [{e.HResult:X}] {e.Message}");
      }
      return false;
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
        foreach (var def in _addedDefinitions)
          ClearDataDefinition(def);
      }
    }

    public bool ClearDataDefinition(Definition def) {
      return _addedDefinitions.Remove(def) && InvokeSimMethod(ClearDataDefinitionDelegate, def);
    }

    public bool RegisterToSimConnect(SimVarItem simVar) {
      if (_addedDefinitions.Contains(simVar.Def)) {
        _logger.LogDebug($"SimVar already registered. {simVar.ToDebugString()}");
        return true;
      }
      if (!_registerDataDelegates.TryGetValue(simVar.StorageDataType, out var registerDataDelegate)) {
        _logger.LogError($"Unable to register storage type for '{simVar.StorageDataType}'");
        return false;
      }

      string unitName = simVar.IsStringType ? null : simVar.Unit;
      if (!InvokeSimMethod(AddToDataDefinitionDelegate, simVar.Def, simVar.SimVarName, unitName, simVar.SimConnectDataType, simVar.DeltaEpsilon, SimConnect.SIMCONNECT_UNUSED))
        return false;

      _addedDefinitions.Add(simVar.Def);

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

    #region SimConnect Event Handlers

    private void Simconnect_OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data) {
      _logger.LogInformation("Received shutdown command from SimConnect, disconnecting.");
      Disconnect();
    }

    private void Simconnect_OnRecvSimobjectDataBytype(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data) {
      if (data.dwData.Length > 0)
        OnDataUpdateEvent?.Invoke((Definition)data.dwDefineID, (Definition)data.dwRequestID, data.dwData[0]);
    }

    private void Simconnect_OnRecvSimObjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data) {
      if (data.dwData.Length > 0)
        OnDataUpdateEvent?.Invoke((Definition)data.dwDefineID, (Definition)data.dwRequestID, data.dwData[0]);
    }

    private void Simconnect_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data) {
      OnConnect?.Invoke(new SimulatorInfo(data));
    }

    private void Simconnect_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data) {
      RequestTrackingData record = GetRequestRecord(data.dwSendID);
      record.eException = (SIMCONNECT_EXCEPTION)data.dwException;
      record.dwExceptionIndex = data.dwIndex;
      _logger.LogDebug($"SimConnect Error: {record.eException}; SendID: {data.dwSendID}; Index: {data.dwIndex};");
      OnException?.Invoke(record);
    }

    private void Simconnect_OnRecvEvent(SimConnect sender, SIMCONNECT_RECV_EVENT data) {
      EventIds evId = (EventIds)data.uEventID;
      Groups gId = (Groups)data.uGroupID;
      string evName = evId.ToString();
      if (DEBUG_NOTIFICATIONS && !Enum.IsDefined(evId) && Enum.IsDefined(gId)) {
        evName = _reflectionService.GetSimEventNameById(data.uEventID);
      }
      _logger.LogDebug($"Simconnect_OnRecvEvent Received: Group: {gId}; Event: {evName}; Data: {data.dwData}");
      OnEventReceived?.Invoke(evId, gId, data.dwData);
    }

    private void Simconnect_OnRecvFilename(SimConnect sender, SIMCONNECT_RECV_EVENT_FILENAME data) {
      EventIds evId = (EventIds)data.uEventID;
      Groups gId = (Groups)data.uGroupID;
      _logger.LogDebug($"Simconnect_OnRecvFilename Received: Group: {gId}; Event: {evId}; Data: {data.szFileName}");
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
        _logger.LogError(e, $"Exception trying to get handle to SimConnect: {e.Message}");
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
          Disconnect();
          _scReady?.Dispose();
          _messageWaitTask?.Dispose();
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
