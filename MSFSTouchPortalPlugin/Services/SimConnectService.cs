
// define to enable extra request tracking code (for UNKNOWN_ID and similar errors)
//#define DEBUG_REQUESTS

using Microsoft.Extensions.Logging;
using Microsoft.FlightSimulator.SimConnect;
using MSFSTouchPortalPlugin.Enums;
using MSFSTouchPortalPlugin.Interfaces;
using MSFSTouchPortalPlugin.Types;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace MSFSTouchPortalPlugin.Services
{
  /// <summary>
  /// Wrapper for SimConnect
  /// </summary>
  internal class SimConnectService : ISimConnectService, IDisposable {
    [DllImport("kernel32.dll")]
    static extern IntPtr GetConsoleWindow();

    private readonly ILogger<SimConnectService> _logger;
    private readonly IReflectionService _reflectionService;

    const uint NOTIFICATION_PRIORITY = 10000000;
    const int WM_USER_SIMCONNECT = 0x0402;
    /// enable AddNotification(), SetNotificationGroupPriorities(), and Simconnect_OnRecvEvent(); currently they serve no purpose except possible debug info.
    private static readonly bool DEBUG_NOTIFICATIONS = false;

    private SimConnect _simConnect;
    private bool _connected;
    private bool _connecting;
    private Task _messageWaitTask;
    private readonly EventWaitHandle _scReady = new EventWaitHandle(false, EventResetMode.AutoReset);
    private readonly List<Definition> _addedDefinitions = new();

    public event DataUpdateEventHandler OnDataUpdateEvent;
    public event ConnectEventHandler OnConnect;
    public event DisconnectEventHandler OnDisconnect;

    // SimConnect method delegates, for centralized interaction in InvokeSimMethod()
    private Action<Enum>             ClearDataDefinitionDelegate;
    private Action<Enum, uint>       SetNotificationGroupPriorityDelegate;
    private Action<uint, Enum>       AIReleaseControlDelegate;
    private Action<Enum, string>     MapClientEventToSimEventDelegate;
    private Action<Enum, Enum, bool> AddClientEventToNotificationGroupDelegate;
    private Action<Enum, Enum, uint, SIMCONNECT_SIMOBJECT_TYPE>   RequestDataOnSimObjectTypeDelegate;
    private Action<uint, Enum, uint, Enum, SIMCONNECT_EVENT_FLAG> TransmitClientEventDelegate;
    private Action<Enum, uint, SIMCONNECT_DATA_SET_FLAG, object>  SetDataOnSimObjectDelegate;
    private Action<Enum, string, string, SIMCONNECT_DATATYPE, float, uint> AddToDataDefinitionDelegate;
    private Action<Enum, Enum, uint, SIMCONNECT_PERIOD, SIMCONNECT_DATA_REQUEST_FLAG, uint, uint, uint> RequestDataOnSimObjectDelegate;
    private readonly Dictionary<Type, Action<Enum> > _registerDataDelegates = new();

    public SimConnectService(ILogger<SimConnectService> logger, IReflectionService reflectionService) {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _reflectionService = reflectionService ?? throw new ArgumentNullException(nameof(reflectionService));
    }

    public bool IsConnected() => (_connected && _simConnect != null);

    public bool Connect(uint configIndex = 0) {
      if (_connecting || _simConnect != null)
        return _connected;

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
        if (DEBUG_NOTIFICATIONS)
          _simConnect.OnRecvEvent += new SimConnect.RecvEventEventHandler(Simconnect_OnRecvEvent);

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

        _registerDataDelegates.Clear();
        _registerDataDelegates.Add(typeof(double),    _simConnect.RegisterDataDefineStruct<double>);
        _registerDataDelegates.Add(typeof(uint),      _simConnect.RegisterDataDefineStruct<uint>);
        _registerDataDelegates.Add(typeof(long),      _simConnect.RegisterDataDefineStruct<long>);
        _registerDataDelegates.Add(typeof(StringVal), _simConnect.RegisterDataDefineStruct<StringVal>);

#if DEBUG_REQUESTS
        DbgSetupRequestTracking();
#endif
        //_simConnect.Text(SIMCONNECT_TEXT_TYPE.PRINT_BLACK, 5, Events.StartupMessage, "TouchPortal Connected");  // not currently supported in MSFS SDK

        _messageWaitTask = Task.Run(ReceiveMessages);

      } catch (COMException ex) {
        _connected = false;
        _logger.LogInformation("Connection to Sim failed: {exception}", ex.Message);
        return false;
      }

      _connecting = false;
      // Invoke Handler
      OnConnect?.Invoke();
      return true;
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
        _logger.LogInformation("SimConnect Disconnected");
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
        _logger.LogTrace($"Invoking: {method.Method.Name}({(args != null ? string.Join(", ", args) : "null")})");
        method.DynamicInvoke(args);
        DbgAddSendRecord($"{method.Method.Name}({(args != null ? string.Join(", ", args) : "null")})");
        return true;
      }
      catch (COMException e) {
        _logger.LogWarning($"SimConnect returned an error: [{e.HResult}] {e.Message} <site: '{e.TargetSite}'; source: '{e.Source}'");
      }
      catch (Exception e) {
        _logger.LogError(e, $"Method invocation failed with system exception: [{e.HResult}] {e.Message}");
      }
      return false;
    }

    public bool MapClientEventToSimEvent(Enum eventId, string eventName) {
      return InvokeSimMethod(MapClientEventToSimEventDelegate, eventId, eventName);
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
      _addedDefinitions.Clear();
    }

    public bool ClearDataDefinition(Definition def) {
      return InvokeSimMethod(ClearDataDefinitionDelegate, def);
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

      if (!InvokeSimMethod(registerDataDelegate, simVar.Def)) {
        ClearDataDefinition(simVar.Def);
        return false;
      }

      _addedDefinitions.Add(simVar.Def);
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
      _logger.LogInformation("SimConnect Connected");
    }

    private void Simconnect_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data) {
      SIMCONNECT_EXCEPTION eException = (SIMCONNECT_EXCEPTION)data.dwException;
      string request = DbgGetSendRecord(data.dwSendID);
      _logger.LogWarning($"SimConnect_OnRecvException: {eException}; SendID: {data.dwSendID}; Index: {data.dwIndex}; Request: {request}");
    }

    private void Simconnect_OnRecvEvent(SimConnect sender, SIMCONNECT_RECV_EVENT data) {
      string grpName = data.uGroupID.ToString();
      string eventId = data.uEventID.ToString();
      if (Enum.IsDefined(typeof(Groups), (int)data.uGroupID)) {
        grpName = ((Groups)data.uGroupID).ToString();
        eventId = _reflectionService.GetSimEventNameById(data.uEventID);
      }
      _logger.LogDebug($"Simconnect_OnRecvEvent Received: Group: {grpName}; Event: {eventId}");
    }

    #endregion SimConnect Event Handlers

    #region IDisposable Support
    private bool disposedValue; // To detect redundant calls

    protected virtual void Dispose(bool disposing) {
      if (!disposedValue) {
        if (disposing) {
          // Dispose managed state (managed objects).
          if (_connected)
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

    #region Request Debugging

#if DEBUG_REQUESTS

    // Extra SimConnect functions via native pointer
    IntPtr hSimConnect;
    [DllImport("SimConnect.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    private static extern int /* HRESULT */ SimConnect_GetLastSentPacketID(IntPtr hSimConnect, out uint /* DWORD */ dwSendID);
    // for tracking requests by their SendID
    private readonly System.Collections.Generic.Dictionary<uint, string> dbgSendRecordsDict = new();

    private void DbgSetupRequestTracking() {
      // Get direct access to the SimConnect handle, to use functions otherwise not supported.
#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
      System.Reflection.FieldInfo fiSimConnect = typeof(SimConnect).GetField("hSimConnect", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
#pragma warning restore S3011
      hSimConnect = (IntPtr)fiSimConnect.GetValue(_simConnect);
    }

    private void DbgAddSendRecord(string record) {
      if (_simConnect == null || !_connected)
        return;
      if (dbgSendRecordsDict.Count > 5000) {
        // we'd like to remove the oldest, first, record but the order isn't really guaranteed. We could get a list of keys and sort it... but this should suffice for debugs.
        var enmr = dbgSendRecordsDict.Keys.GetEnumerator();
        enmr.MoveNext();
        dbgSendRecordsDict.Remove(enmr.Current);
      }
      if (SimConnect_GetLastSentPacketID(hSimConnect, out uint dwSendID) == 0)
        _ = dbgSendRecordsDict.TryAdd(dwSendID, record);
    }

    private string DbgGetSendRecord(uint sendId) {
      if (dbgSendRecordsDict.TryGetValue(sendId, out string record))
        return record;
      return $"Record not found for SendID {sendId}";
    }

#else
    [System.Diagnostics.Conditional("DEBUG_REQUESTS")]  // prevents any parameters being passed to this method from being evaluated
    private static void DbgAddSendRecord(string record) { _ = record; /* no-op when request tracking disabled */ }
    private static string DbgGetSendRecord(uint sendId) { _ = sendId; return "Request tracking disabled."; }
#endif  // DEBUG_REQUESTS

    #endregion
  }
}
