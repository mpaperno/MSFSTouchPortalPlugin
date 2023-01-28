/*
This file is part of the MSFS Touch Portal Plugin project.
https://github.com/mpaperno/MSFSTouchPortalPlugin

COPYRIGHT:
(c) 2020 Tim Lewis;
(c) Maxim Paperno; All Rights Reserved.

This file may be used under the terms of the GNU General Public License (GPL)
as published by the Free Software Foundation, either version 3 of the Licenses,
or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

A copy of the GNU GPL is included with this project
and is also available at <http://www.gnu.org/licenses/>.
*/

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
    bool WasmAvailable { get; }
    WasmModuleStatus WasmStatus { get; }

    uint Connect(uint configIndex = 0);
    void Disconnect();
    bool TransmitClientEvent(Enum eventId, uint data, uint d2 = 0, uint d3 = 0, uint d4 = 0, uint d5 = 0);
    bool TransmitClientEvent(EventMappingRecord eventRecord, uint data, uint d2 = 0, uint d3 = 0, uint d4 = 0, uint d5 = 0);
    bool TransmitClientEvent(EventMappingRecord eventRecord, uint[] data);
    bool RequestDataOnSimObject(SimVarItem simVar, uint objectId = (uint)SIMCONNECT_SIMOBJECT_TYPE.USER);
    bool RequestDataOnSimObjectType(SimVarItem simVar, SIMCONNECT_SIMOBJECT_TYPE objectType = SIMCONNECT_SIMOBJECT_TYPE.USER);
    bool SetDataOnSimObject(SimVarItem simVar, uint objectId = (uint)SIMCONNECT_SIMOBJECT_TYPE.USER);
    bool ReleaseAIControl(Definition def, uint objectId = (uint)SIMCONNECT_SIMOBJECT_TYPE.USER);
    bool SubscribeToSystemEvent(Enum eventId, string eventName);
    void UpdateWasmClientId(byte highByte);
    bool ExecuteCalculatorCode(string code);
    bool SetVariable(char varType, string varName, double value, string unit = "", bool createLocal = false);
    bool RequestLookupList(Enum listType);
    bool RequestVariableValueUpdate(SimVarItem simVar);
    void RetryRegisterLocalVars();
  }
}
