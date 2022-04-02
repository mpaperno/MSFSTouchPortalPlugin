
// define to enable extra request tracking code (for UNKNOWN_ID and similar errors)
//#define DEBUG_REQUESTS

using Microsoft.Extensions.Logging;
using Microsoft.FlightSimulator.SimConnect;
using MSFSTouchPortalPlugin.Enums;
using MSFSTouchPortalPlugin.Interfaces;
using MSFSTouchPortalPlugin.Types;
using System;
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

    SimConnect _simConnect;
    private bool _connected;
    private bool _connecting;
    private Task _messageWaitTask;
    private readonly EventWaitHandle _scReady = new EventWaitHandle(false, EventResetMode.AutoReset);
    private readonly System.Collections.Generic.List<Definition> _addedDefinitions = new();

    public event DataUpdateEventHandler OnDataUpdateEvent;
    public event ConnectEventHandler OnConnect;
    public event DisconnectEventHandler OnDisconnect;

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

    public void ReceiveMessages() {
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

    public bool MapClientEventToSimEvent(Enum eventId, string eventName) {
      if (_connected) {
        _simConnect.MapClientEventToSimEvent(eventId, eventName);
        DbgAddSendRecord($"MapClientEventToSimEvent(eventId: {eventId}; eventName: {eventName})");
        return true;
      }

      return false;
    }

    public bool TransmitClientEvent(Groups group, Enum eventId, uint data) {
      if (_connected) {
        try {
          _simConnect.TransmitClientEvent(SimConnect.SIMCONNECT_OBJECT_ID_USER, eventId, data, group, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
          DbgAddSendRecord($"TransmitClientEvent(group: {group}; eventId: {eventId}; data: {data})");
          return true;
        }
        catch (Exception ex) {
          _logger.LogError(ex, $"TransmitClientEvent({group}, {eventId}, {data}) failed, disconnecting.");
          Disconnect();
        }
      }

      return false;
    }

    public bool AddNotification(Groups group, Enum eventId) {
      if (DEBUG_NOTIFICATIONS && _connected) {
        _simConnect.AddClientEventToNotificationGroup(group, eventId, false);
        DbgAddSendRecord($"AddNotification(group: {group}; eventId: {eventId}");
        return true;
      }

      return false;
    }

    public void SetNotificationGroupPriorities() {
      if (DEBUG_NOTIFICATIONS && _connected) {
        foreach (Enum g in Enum.GetValues(typeof(Groups))) {
          _simConnect.SetNotificationGroupPriority(g, NOTIFICATION_PRIORITY);
          DbgAddSendRecord($"SetNotificationGroupPriority(Group: {g})");
        }
      }
    }

    private void ClearDataDefinition(Definition def) {
      _simConnect.ClearDataDefinition(def);
      DbgAddSendRecord($"ClearDataDefinition({def})");
    }

    public bool RegisterToSimConnect(SimVarItem simVar) {
      if (_connected) {
        string unitName = simVar.IsStringType ? null : simVar.Unit;
        _simConnect.AddToDataDefinition(simVar.Def, simVar.SimVarName, unitName, simVar.SimConnectDataType, simVar.DeltaEpsilon, SimConnect.SIMCONNECT_UNUSED);
        DbgAddSendRecord($"AddToDataDefinition({simVar.ToDebugString()}, {unitName}, {simVar.SimConnectDataType})");

        switch (simVar.Value) {
          case double:
            _simConnect.RegisterDataDefineStruct<double>(simVar.Def);
            break;
          case uint:
            _simConnect.RegisterDataDefineStruct<uint>(simVar.Def);
            break;
          case long:
            _simConnect.RegisterDataDefineStruct<long>(simVar.Def);
            break;
          case StringVal:
            _simConnect.RegisterDataDefineStruct<StringVal>(simVar.Def);
            break;
          default:
            _logger.LogError($"Unable to register storage type for '{simVar.StorageDataType}'");
            ClearDataDefinition(simVar.Def);
            return false;
        }
        DbgAddSendRecord($"RegisterDataDefineStruct<{simVar.StorageDataType}>({simVar.ToDebugString()})");

        if (!simVar.NeedsScheduledRequest) {
          _simConnect.RequestDataOnSimObject(simVar.Def, simVar.Def, (uint)SIMCONNECT_SIMOBJECT_TYPE.USER, (SIMCONNECT_PERIOD)simVar.UpdatePeriod, SIMCONNECT_DATA_REQUEST_FLAG.CHANGED, 0, simVar.UpdateInterval, 0);
          DbgAddSendRecord($"RequestDataOnSimObject({simVar.ToDebugString()})");
        }

        _addedDefinitions.Add(simVar.Def);
        return true;
      }

      return false;
    }

    public void ClearAllDataDefinitions() {
      if (_connected) {
        foreach (var def in _addedDefinitions)
          ClearDataDefinition(def);
      }
      _addedDefinitions.Clear();
    }

    public bool RequestDataOnSimObjectType(SimVarItem simVar) {
      if (_connected) {
        try {
          _simConnect.RequestDataOnSimObjectType(simVar.Def, simVar.Def, 0, SIMCONNECT_SIMOBJECT_TYPE.USER);
          DbgAddSendRecord($"RequestDataOnSimObjectType({simVar.ToDebugString()})");
          return true;
        }
        catch (Exception ex) {
          _logger.LogError(ex, $"RequestDataOnSimObjectType({simVar.Def}) failed, disconnecting.");
          Disconnect();
        }
      }

      return false;
    }

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

    /// <summary>
    /// Events triggered by sending events to the Sim
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="data"></param>
    private void Simconnect_OnRecvEvent(SimConnect sender, SIMCONNECT_RECV_EVENT data) {
      string grpName = data.uGroupID.ToString();
      string eventId = data.uEventID.ToString();
      if (Enum.IsDefined(typeof(Groups), (int)data.uGroupID)) {
        grpName = ((Groups)data.uGroupID).ToString();
        eventId = _reflectionService.GetSimEventNameById(data.uEventID);
      }
      _logger.LogDebug($"Simconnect_OnRecvEvent Recieved: Group: {grpName}; Event: {eventId}");
    }

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
    #endregion

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
