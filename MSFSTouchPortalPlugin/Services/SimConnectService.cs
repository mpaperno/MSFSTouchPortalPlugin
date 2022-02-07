
// define to enable extra request tracking code (for UNKNOWN_ID and similar errors)
//#define DEBUG_REQUESTS

using Microsoft.Extensions.Logging;
using Microsoft.FlightSimulator.SimConnect;
using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Enums;
using MSFSTouchPortalPlugin.Interfaces;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
#if DEBUG_REQUESTS
using System.Collections.Generic;
#endif

namespace MSFSTouchPortalPlugin.Services {
  /// <summary>
  /// Wrapper for SimConnect
  /// </summary>
  internal class SimConnectService : ISimConnectService, IDisposable {
    [DllImport("kernel32.dll")]
    static extern IntPtr GetConsoleWindow();

    private readonly ILogger<SimConnectService> _logger;

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

    public SimConnectService(ILogger<SimConnectService> logger) {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

    public bool RegisterToSimConnect(SimVarItem simVar) {
      if (_connected) {
        bool isStrType = simVar.Unit == Units.String;
        string unitName = isStrType ? null : simVar.Unit;
        SIMCONNECT_DATATYPE scType = isStrType ? SIMCONNECT_DATATYPE.STRING64 : SIMCONNECT_DATATYPE.FLOAT64;
        _simConnect.AddToDataDefinition(simVar.Def, simVar.SimVarName, unitName, scType, 0.0f, SimConnect.SIMCONNECT_UNUSED);
        DbgAddSendRecord($"AddToDataDefinition(simVar.Def: {simVar.Def}; simVar.SimVarName: {simVar.SimVarName}; simVar.Unit: {simVar.Unit};");

        if (isStrType)
          _simConnect.RegisterDataDefineStruct<StringVal64>(simVar.Def);
        else
          _simConnect.RegisterDataDefineStruct<double>(simVar.Def);
        DbgAddSendRecord($"RegisterDataDefineStruct<{(isStrType ? "String64" : "double")}>(simVar.Def: {simVar.Def}");

        return true;
      }

      return false;
    }

    public bool RequestDataOnSimObjectType(SimVarItem simVar) {
      if (_connected) {
        try {
          _simConnect.RequestDataOnSimObjectType(simVar.Def, simVar.Def, 0, SIMCONNECT_SIMOBJECT_TYPE.USER);
          DbgAddSendRecord($"RequestDataOnSimObjectType(simVar.Def: {simVar.Def})");
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
        Groups group = (Groups)data.uGroupID;
        grpName = group.ToString();
        Type toEnum = Assembly.GetExecutingAssembly().GetTypes().First(
          t => t.IsEnum && t.GetCustomAttribute<SimNotificationGroupAttribute>() != null && t.GetCustomAttribute<SimNotificationGroupAttribute>().Group == group
        );
        if (toEnum != null)
          eventId = Enum.ToObject(toEnum, data.uEventID).ToString();
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
    private readonly Dictionary<uint, string> dbgSendRecordsDict = new();

    private void DbgSetupRequestTracking() {
      // Get direct access to the SimConnect handle, to use functions otherwise not supported.
      FieldInfo fiSimConnect = typeof(SimConnect).GetField("hSimConnect", BindingFlags.NonPublic | BindingFlags.Instance);
      hSimConnect = (IntPtr)fiSimConnect.GetValue(_simConnect);
    }

    private void DbgAddSendRecord(string record) {
      if (_simConnect == null || !_connected)
        return;
      if (dbgSendRecordsDict.Count > 5000)
        dbgSendRecordsDict.Remove(dbgSendRecordsDict.Keys.First());
      SimConnect_GetLastSentPacketID(hSimConnect, out uint dwSendID);
      _ = dbgSendRecordsDict.TryAdd(dwSendID, record);
    }

    private string DbgGetSendRecord(uint sendId) {
      if (dbgSendRecordsDict.TryGetValue(sendId, out string record))
        return record;
      return $"Recrod not found for SendID {sendId}";
    }

#else
#pragma warning disable S1172 // Unused method parameters should be removed
    [System.Diagnostics.Conditional("DEBUG_REQUESTS")]  // prevents any parameters being passed to this method from being evaluated
    private static void DbgAddSendRecord(string _) { /* no-op when request tracking disabled */ }
    private static string DbgGetSendRecord(uint _) => "Request tracking disabled.";
#pragma warning restore S1172
#endif  // DEBUG_REQUESTS

    #endregion
  }

  struct StringVal64 : IEquatable<StringVal64> {
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
    public string Value;

    public bool Equals(StringVal64 other) => other.Value == Value;
    public override bool Equals(object obj) => (obj is StringVal64 && Equals((StringVal64)obj));
    public override string ToString() => Value;
    public override int GetHashCode() => Value.GetHashCode();
  }
}
