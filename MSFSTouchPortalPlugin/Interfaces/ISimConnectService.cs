using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MSFSTouchPortalPlugin.Interfaces {
  internal delegate void DataUpdateEventHandler(Definition def, Definition req, object data);
  internal delegate void ConnectEventHandler();
  internal delegate void DisconnectEventHandler();

  internal interface ISimConnectService {
    event DataUpdateEventHandler OnDataUpdateEvent;
    event ConnectEventHandler OnConnect;
    event DisconnectEventHandler OnDisconnect;

    bool IsConnected();
    bool AddNotification(Enum group, Enum eventId);
    bool Connect();
    void Disconnect();
    bool MapClientEventToSimEvent(Enum eventId, string eventName);
    bool RegisterToSimConnect(SimVarItem simVar);
    bool RequestDataOnSimObjectType(SimVarItem simVar);
    bool TransmitClientEvent(Groups group, Enum eventId, uint data);
    Task WaitForMessage(CancellationToken cancellationToken);
  }
}