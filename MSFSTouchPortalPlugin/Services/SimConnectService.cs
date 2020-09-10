using Microsoft.FlightSimulator.SimConnect;
using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Enums;
using MSFSTouchPortalPlugin.Interfaces;
using MSFSTouchPortalPlugin.Objects.AutoPilot;
using MSFSTouchPortalPlugin.Objects.InstrumentsSystems;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace MSFSTouchPortalPlugin.Services {
  /// <summary>
  /// Wrapper for SimConnect
  /// </summary>
  internal class SimConnectService : ISimConnectService {
    [DllImport("kernel32.dll")]
    static extern IntPtr GetConsoleWindow();

    const uint NOTIFICATION_PRIORITY = 10000000;
    const int WM_USER_SIMCONNECT = 0x0402;
    SimConnect _simConnect = null;
    EventWaitHandle _scReady = new EventWaitHandle(false, EventResetMode.AutoReset);
    private bool _connected = false;
    
    public event DataUpdateEventHandler OnDataUpdateEvent;
    public event ConnectEventHandler OnConnect;

    public SimConnectService() { }

    public bool IsConnected() => _connected;

    public void Connect() {
      Console.WriteLine("Connect SimConnect");

      try {
        _simConnect = new SimConnect("Touch Portal Plugin", GetConsoleWindow(), WM_USER_SIMCONNECT, _scReady, 0);

        _connected = true;

        // System Events
        _simConnect.OnRecvOpen += new SimConnect.RecvOpenEventHandler(simconnect_OnRecvOpen);
        _simConnect.OnRecvQuit += new SimConnect.RecvQuitEventHandler(simconnect_OnRecvQuit);
        _simConnect.OnRecvException += new SimConnect.RecvExceptionEventHandler(simconnect_OnRecvException);

        // Sim mapped events
        _simConnect.OnRecvEvent += new SimConnect.RecvEventEventHandler(simconnect_OnRecvEvent);

        // simconnect.OnRecvAssignedObjectId += new SimConnect.RecvAssignedObjectIdEventHandler(simconnect_OnRecvAssignedObjectId);

        // Sim Data
        _simConnect.OnRecvSimobjectData += new SimConnect.RecvSimobjectDataEventHandler(simconnect_OnRecvSimObjectData);
        _simConnect.OnRecvSimobjectDataBytype += new SimConnect.RecvSimobjectDataBytypeEventHandler(simconnect_OnRecvSimobjectDataBytype);

        _simConnect.ClearNotificationGroup(Groups.System);
        _simConnect.SetNotificationGroupPriority(Groups.System, NOTIFICATION_PRIORITY);

        _simConnect.ClearNotificationGroup(Groups.AutoPilot);
        _simConnect.SetNotificationGroupPriority(Groups.AutoPilot, NOTIFICATION_PRIORITY);

        _simConnect.ClearNotificationGroup(Groups.Fuel);
        _simConnect.SetNotificationGroupPriority(Groups.Fuel, NOTIFICATION_PRIORITY);

        _simConnect.Text(SIMCONNECT_TEXT_TYPE.PRINT_BLACK, 5, Events.StartupMessage, "TouchPortal Connected");

        // Invoke Handler
        OnConnect();
      } catch (COMException ex) {
        Console.WriteLine("Connection to Sim failed: " + ex.Message);
      }
    }

    public void Disconnect() {
      Console.WriteLine("Disconnect SimConnect");

      if (_simConnect != null) {
        /// Dispose serves the same purpose as SimConnect_Close()
        _simConnect.Dispose();
        _simConnect = null;
      }

      _connected = false;
    }

    public Task WaitForMessage() {
      while (true) {
        _scReady.WaitOne();

        // TODO: Exception on quit
        _simConnect?.ReceiveMessage();
        //simconnect.RequestDataOnSimObjectType(Events.Test, Group.Test, 0, SIMCONNECT_SIMOBJECT_TYPE.AIRCRAFT);
      }
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
        _simConnect.TransmitClientEvent((uint)SimConnect.SIMCONNECT_OBJECT_ID_USER, eventId, data, group, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
        return true;
      }

      return false;
    }

    public bool AddNotification(Enum group, Enum eventId) {
      if (_connected) {
        _simConnect.AddClientEventToNotificationGroup(group, eventId, false);
        return true;
      }

      return false;
    }

    public bool RegisterToSimConnect(SimVarItem simVar) {
      if (_connected) {
        _simConnect.AddToDataDefinition(simVar.def, simVar.SimVarName, simVar.Unit, SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
        _simConnect.RegisterDataDefineStruct<double>(simVar.def);
        return true;
      }

      return false;
    }

    public bool RequestDataOnSimObjectType(SimVarItem simVar) {
      if (_connected) {
        _simConnect.RequestDataOnSimObjectType(simVar.req, simVar.def, 0, SIMCONNECT_SIMOBJECT_TYPE.USER);
      }

      return false;
    }

    private void simconnect_OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data) {
      Console.WriteLine("Quit");
    }

    private void simconnect_OnRecvSimobjectDataBytype(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data) {
      Console.WriteLine("ReceivedObjectDataByType");
      if (data.dwData.Length > 0) {
        OnDataUpdateEvent((Definition)data.dwDefineID, (Request)data.dwRequestID, data.dwData[0]);
      }

    }

    private void simconnect_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data) {
      Console.WriteLine("Opened");
    }

    private void simconnect_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data) {
      SIMCONNECT_EXCEPTION eException = (SIMCONNECT_EXCEPTION)data.dwException;
      Console.WriteLine("SimConnect_OnRecvException: " + eException.ToString());
    }

    //private void simconnect_OnRecvAssignedObjectId(SimConnect sender, SIMCONNECT_RECV_ASSIGNED_OBJECT_ID data) {
    //  Console.WriteLine("Recieved");
    //}

    /// <summary>
    /// Events triggered by sending events to the Sim
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="data"></param>
    private void simconnect_OnRecvEvent(SimConnect sender, SIMCONNECT_RECV_EVENT data) {
      Groups group = (Groups)data.uGroupID;
      dynamic eventId = null;

      switch (group) {
        case Groups.System:
          eventId = (Events)data.uEventID;
          break;
        case Groups.AutoPilot:
          eventId = (AutoPilot)data.uEventID;
          break;
        case Groups.Fuel:
          eventId = (Fuel)data.uEventID;
          break;
      }

      Console.WriteLine($"{DateTime.Now} Recieved: {group} - {eventId}");
    }

    private void simconnect_OnRecvSimObjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data) {
      Console.WriteLine("Recieved");
    }
  }
}
