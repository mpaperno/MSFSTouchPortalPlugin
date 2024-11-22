/*
This file is part of the MSFS Touch Portal Plugin project.
https://github.com/mpaperno/MSFSTouchPortalPlugin

COPYRIGHT: (c) Maxim Paperno; All Rights Reserved.

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

namespace MSFSTouchPortalPlugin.Enums
{
  // IDs for handling internal events
  public enum PluginActions : short
  {
    None = 0,

    // Action IDs
    Connection,
    ActionRepeatInterval,

    SetCustomSimEvent,
    SetKnownSimEvent,
    SetHubHopEvent,
    SetSimulatorVar,
    SetLocalVar,
    SetInputEvent,
    SetVariable,
    ExecCalcCode,
    SetConnectorValue,
    SetSimVar,       // deprecated

    AddSimulatorVar,
    AddLocalVar,
    AddInputEventVar,
    AddNamedVariable,
    AddCalculatedValue,
    UpdateVarValue,
    RemoveSimVar,
    ClearSimVars,
    SaveSimVars,
    LoadSimVars,
    AddKnownSimVar,   // deprecated
    AddCustomSimVar,  // deprecated

    // Action choice mapping IDs
    ToggleConnection,
    Connect,
    Disconnect,
    ReloadStates,
    ResendStates,
    UpdateConnectorValues,
    UpdateHubHopPresets,
    UpdateLocalVarsList,
    UpdateInputEventsList,
    ReRegisterInputEventVars,

    ActionRepeatIntervalInc,
    ActionRepeatIntervalDec,
    ActionRepeatIntervalSet,

    SaveCustomSimVars,
    SaveAllSimVars,
    ClearCustomSimVars,
    ClearAllSimVars,
    ClearSimVarsFromFile,
    ClearSimVarsOfType,
  }
}
