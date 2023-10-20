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

  public enum SimVarErrorType
  {
    None, VarType, Registration, SimVersion, SimConnectError
  }

  internal delegate void DataUpdateEventHandler(Definition def, Definition req, object data);
  internal delegate void RecvEventEventHandler(EventIds evenId, Groups categoryId, object data);
  internal delegate void ConnectEventHandler(SimulatorInfo info);
  internal delegate void DisconnectEventHandler();
  internal delegate void ExceptionEventHandler(RequestTrackingData data);
  internal delegate void LocalVarsListUpdatedHandler(System.Collections.Generic.IReadOnlyDictionary<int, string> list);
#nullable enable
  internal delegate void SimVarErrorEventHandler(Definition def, SimVarErrorType errType, object? data = null);
#nullable restore

  internal interface ISimConnectService : IDisposable {
    event DataUpdateEventHandler OnDataUpdateEvent;
    event RecvEventEventHandler OnEventReceived;
    event ConnectEventHandler OnConnect;
    event DisconnectEventHandler OnDisconnect;
    event ExceptionEventHandler OnException;
    event SimVarErrorEventHandler OnSimVarError;
#if WASIM
    event LocalVarsListUpdatedHandler OnLVarsListUpdated;
#endif

    bool IsConnected { get; }
    bool WasmAvailable { get; }
    WasmModuleStatus WasmStatus { get; }

    void Init();
    uint Connect(uint configIndex = 0);
    void Disconnect();
    bool TransmitClientEvent(EventMappingRecord eventRecord, uint[] data);
    bool CanRequestVariableType(char varType);
    bool CanSetVariableType(char varType);
    bool SetVariable(char varType, string varName, object value, string unit = "", bool createLocal = false);
    bool RequestVariableValueUpdate(SimVarItem simVar);
    bool RequestLocalVariablesList();
    void RetryRegisterVarRequests(char varType);
  }
}
