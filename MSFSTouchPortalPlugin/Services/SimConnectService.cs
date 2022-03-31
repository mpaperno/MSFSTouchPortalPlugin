
// define to enable extra request tracking code (for UNKNOWN_ID and similar errors)
//#define DEBUG_REQUESTS

using Microsoft.Extensions.Logging;
using Microsoft.FlightSimulator.SimConnect;
using MSFSTouchPortalPlugin.Constants;
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
    readonly EventWaitHandle _scReady = new EventWaitHandle(false, EventResetMode.AutoReset);
    private bool _connected;

    public event DataUpdateEventHandler OnDataUpdateEvent;
    public event ConnectEventHandler OnConnect;
    public event DisconnectEventHandler OnDisconnect;

    public SimConnectService(ILogger<SimConnectService> logger, IReflectionService reflectionService) {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _reflectionService = reflectionService ?? throw new ArgumentNullException(nameof(reflectionService));
    }

    public bool IsConnected() => (_connected && _simConnect != null);

    public bool Connect() {
      _logger.LogInformation("Connect SimConnect");

      try {
        _simConnect = new SimConnect("Touch Portal Plugin", GetConsoleWindow(), WM_USER_SIMCONNECT, _scReady, 0);

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
        //_simConnect.OnRecvSimobjectData += new SimConnect.RecvSimobjectDataEventHandler(Simconnect_OnRecvSimObjectData);  // unused for now

#if DEBUG_REQUESTS
        DbgSetupRequestTracking();
#endif
        _simConnect.Text(SIMCONNECT_TEXT_TYPE.PRINT_BLACK, 5, Events.StartupMessage, "TouchPortal Connected");

        // Invoke Handler
        OnConnect?.Invoke();

        return true;
      } catch (COMException ex) {
        _logger.LogInformation("Connection to Sim failed: {exception}", ex.Message);
      }

      return false;
    }

    public void Disconnect() {
      _logger.LogInformation("Disconnect SimConnect");

      if (_simConnect != null) {
        /// Dispose serves the same purpose as SimConnect_Close()
        _simConnect.Dispose();
        _simConnect = null;
      }

      _connected = false;

      // Invoke Handler
      OnDisconnect?.Invoke();
    }

    public Task WaitForMessage(CancellationToken cancellationToken) {
      while (_connected && !cancellationToken.IsCancellationRequested) {
        if (_scReady.WaitOne(5000)) {
          _simConnect?.ReceiveMessage();
        }
      }

      return Task.CompletedTask;
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

        return true;
      }

      return false;
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
      _logger.LogInformation("Quit");
      Disconnect();
    }

    private void Simconnect_OnRecvSimobjectDataBytype(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data) {
      if (data.dwData.Length > 0) {
        OnDataUpdateEvent?.Invoke((Definition)data.dwDefineID, (Definition)data.dwRequestID, data.dwData[0]);
      }

    }

    private void Simconnect_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data) {
      _logger.LogInformation("Opened");
    }

    private void Simconnect_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data) {
      SIMCONNECT_EXCEPTION eException = (SIMCONNECT_EXCEPTION)data.dwException;
      string request = DbgGetSendRecord(data.dwSendID);
      _logger.LogInformation($"SimConnect_OnRecvException: {eException}; SendID: {data.dwSendID}; Index: {data.dwIndex}; Request: {request}");
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
      _logger.LogInformation($"Simconnect_OnRecvEvent Recieved: Group: {grpName}; Event: {eventId}");
    }

    //private void Simconnect_OnRecvSimObjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data) {
    //  // Empty method for now, not implemented
    //}

    #region IDisposable Support
    private bool disposedValue; // To detect redundant calls

    protected virtual void Dispose(bool disposing) {
      if (!disposedValue) {
        if (disposing) {
          // Dispose managed state (managed objects).
          _scReady.Dispose();
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
