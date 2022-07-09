using Microsoft.FlightSimulator.SimConnect;
using MSFSTouchPortalPlugin.Enums;
using MSFSTouchPortalPlugin.Types;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace MSFSTouchPortalPlugin.Interfaces
{
  public enum WasmModuleStatus {
    Unknown, NotFound, Found, Connected
  }

  internal delegate void DataUpdateEventHandler(Definition def, Definition req, object data);
  internal delegate void RecvEventEventHandler(EventIds evenId, Groups categoryId, object data);
  internal delegate void ConnectEventHandler(SimulatorInfo info);
  internal delegate void DisconnectEventHandler();
  internal delegate void ExceptionEventHandler(RequestTrackingData data);
  internal delegate void LocalVarsListUpdatedHandler(System.Collections.Generic.IReadOnlyDictionary<int, string> list);

  internal interface ISimConnectService : IDisposable {
    event DataUpdateEventHandler OnDataUpdateEvent;
    event RecvEventEventHandler OnEventReceived;
    event ConnectEventHandler OnConnect;
    event DisconnectEventHandler OnDisconnect;
    event ExceptionEventHandler OnException;
    event LocalVarsListUpdatedHandler OnLVarsListUpdated;

    bool IsConnected { get; }
    WasmModuleStatus WasmStatus { get; }

    bool AddNotification(Groups group, Enum eventId);
    uint Connect(uint configIndex = 0);
    void Disconnect();
    bool MapClientEventToSimEvent(Enum eventId, string eventName, Groups group);
    bool TransmitClientEvent(Groups group, Enum eventId, uint data);
    void SetNotificationGroupPriorities();
    void ClearAllDataDefinitions();
    bool RegisterToSimConnect(SimVarItem simVar);
    bool RequestDataOnSimObject(SimVarItem simVar, uint objectId = (uint)SIMCONNECT_SIMOBJECT_TYPE.USER);
    bool RequestDataOnSimObjectType(SimVarItem simVar, SIMCONNECT_SIMOBJECT_TYPE objectType = SIMCONNECT_SIMOBJECT_TYPE.USER);
    bool SetDataOnSimObject(SimVarItem simVar, uint objectId = (uint)SIMCONNECT_SIMOBJECT_TYPE.USER);
    bool ReleaseAIControl(Definition def, uint objectId = (uint)SIMCONNECT_SIMOBJECT_TYPE.USER);
    bool ClearDataDefinition(Definition def);
    bool SubscribeToSystemEvent(Enum eventId, string eventName);
    bool ExecuteCalculatorCode(string code);
    bool SetVariable(char varType, string varName, double value, string unit = "");
    bool RequestLookupList(Enum listType);
  }
}
