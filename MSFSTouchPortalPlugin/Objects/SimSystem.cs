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

using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Enums;
using MSFSTouchPortalPlugin.Types;

namespace MSFSTouchPortalPlugin.Objects
{
  [TouchPortalCategory(Groups.SimSystem)]
  internal static class SimSystemMapping
  {
    [TouchPortalAction("SimulationRate", "Simulation Rate", "{0} Simulation Rate", true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("SIM_RATE_INCR", "Increase")]
    [TouchPortalActionMapping("SIM_RATE_DECR", "Decrease")]
    public static readonly object SimulationRate;

    [TouchPortalAction("SelectedParameter", "Change Selected Value (+/-)", "{0} Selected Value", true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("PLUS", "Increase")]
    [TouchPortalActionMapping("MINUS", "Decrease")]
    public static readonly object SELECTED_PARAMETER_CHANGE;


    // Event

    public static readonly TouchPortalEvent SimSystemEvent = new("SimSystemEvent", "Simulator System Event", "On Simulator Event $val",
      new System.Enum[] {
       EventIds.SimConnecting,
       EventIds.SimConnected,
       EventIds.SimDisconnected,
       EventIds.SimTimedOut,
       EventIds.SimError,
       EventIds.PluginError,
       EventIds.PluginInfo,
       EventIds.Paused,
       EventIds.Unpaused,
       EventIds.Pause,
       EventIds.SimStart,
       EventIds.SimStop,
       EventIds.Sim,
       EventIds.AircraftLoaded,
       EventIds.Crashed,
       EventIds.CrashReset,
       EventIds.FlightLoaded,
       EventIds.FlightSaved,
       EventIds.FlightPlanActivated,
       EventIds.FlightPlanDeactivated,
       EventIds.PositionChanged,
       EventIds.Sound,
       EventIds.ViewCockpit,
       EventIds.ViewExternal,
    });
  }
}
