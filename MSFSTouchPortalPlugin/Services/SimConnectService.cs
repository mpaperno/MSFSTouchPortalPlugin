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
        if (_scReady.WaitOne(TimeSpan.FromSeconds(5))) {
          _simConnect?.ReceiveMessage();
        }
      }

      return Task.CompletedTask;
    }

    public bool MapClientEventToSimEvent(Enum eventId, string eventName) {
      if (_connected) {
        _simConnect.MapClientEventToSimEvent(eventId, eventName);
        return true;
      }

      return false;
    }

    public bool TransmitClientEvent(Groups group, Enum eventId, uint data) {
      if (_connected) {
        try {
          _simConnect.TransmitClientEvent(SimConnect.SIMCONNECT_OBJECT_ID_USER, eventId, data, group, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
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
        return true;
      }

      return false;
    }

    public void SetNotificationGroupPriorities() {
      if (DEBUG_NOTIFICATIONS && _connected) {
        foreach (Enum g in Enum.GetValues(typeof(Groups)))
          _simConnect.SetNotificationGroupPriority(g, NOTIFICATION_PRIORITY);
      }
    }

    public bool RegisterToSimConnect(SimVarItem simVar) {
      if (_connected) {
        if (simVar.Unit == Units.String) {
          _simConnect.AddToDataDefinition(simVar.Def, simVar.SimVarName, null, SIMCONNECT_DATATYPE.STRING64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
          _simConnect.RegisterDataDefineStruct<StringVal64>(simVar.Def);
        } else {
          _simConnect.AddToDataDefinition(simVar.Def, simVar.SimVarName, simVar.Unit, SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
          _simConnect.RegisterDataDefineStruct<double>(simVar.Def);
        }

        return true;
      }

      return false;
    }

    public bool RequestDataOnSimObjectType(SimVarItem simVar) {
      if (_connected) {
        try {
          _simConnect.RequestDataOnSimObjectType(simVar.Def, simVar.Def, 0, SIMCONNECT_SIMOBJECT_TYPE.USER);
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
      _logger.LogInformation("SimConnect_OnRecvException: {exception}", eException.ToString());
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
