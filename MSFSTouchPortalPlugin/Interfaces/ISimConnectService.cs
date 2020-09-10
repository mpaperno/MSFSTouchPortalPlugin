using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Enums;
using System;
using System.Threading.Tasks;

namespace MSFSTouchPortalPlugin.Interfaces {
  internal delegate void DataUpdateEventHandler(Definition def, Request req, object data);
  internal delegate void ConnectEventHandler();

  internal interface ISimConnectService {
    event DataUpdateEventHandler OnDataUpdateEvent;
    event ConnectEventHandler OnConnect;

    bool IsConnected();
    bool AddNotification(Enum group, Enum eventId);
    void Connect();
    void Disconnect();
    bool MapClientEventToSimEvent(Enum eventId, string eventName);
    bool RegisterToSimConnect(SimVarItem simVar);
    bool RequestDataOnSimObjectType(SimVarItem simVar);
    bool TransmitClientEvent(Groups group, Enum eventId, uint data);
    Task WaitForMessage();
  }
}