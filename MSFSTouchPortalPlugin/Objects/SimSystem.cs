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

    [TouchPortalAction("SelectedParameter", "Adjust a Selected Value (+/-)", "{0} Selected Value", true,
      Description = "This action affects any value previously \"selected\" with some other action (such as AP settings, frequencies, etc)."
    )]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("PLUS", "Increase")]
    [TouchPortalActionMapping("MINUS", "Decrease")]
    public static readonly object SelectedParameterChange;

    [TouchPortalAction("PauseFullSet", "Pause - Full", "Set Full Pause to {0} (0/1)", false,
      Description = "A \"full\" pause stops the simulation completely, including any time passing in the simulated world. Same as \"Dev Mode Pause.\""
    )]
    [TouchPortalActionText("1", 0, 1, AllowDecimals = true)]
    [TouchPortalActionMapping("PAUSE_SET")]
    public static readonly object PauseFullSet;

    [TouchPortalAction("PauseSimSet", "Pause - Simulator", "{0} Simulator Pause", false,
      Description = "A \"simulator\" pause stops some aspects of the simulation, but not time. Same as the \"Menu (ESC) Pause\" but w/out the actual menu."
    )]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("PAUSE_ON", "Enable")]
    [TouchPortalActionMapping("PAUSE_OFF", "Disable")]
    public static readonly object PauseSimSet;

    [TouchPortalAction("SimulationRate", "Simulation Rate Adjust", "{0} Simulation Rate", true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("SIM_RATE_INCR", "Increase")]
    [TouchPortalActionMapping("SIM_RATE_DECR", "Decrease")]
    [TouchPortalActionMapping("SIM_RATE", "Select (for +/- adjustment)")]
    public static readonly object SimulationRate;

    [TouchPortalAction("SimulationRateSet", "Simulation Rate Set", true,
      "Set Simulation Rate to {0}",
      "Set Simulation Rate\nin Value Range:"
    )]
    [TouchPortalActionText("1", 0, 50000, AllowDecimals = true)]
    [TouchPortalActionMapping("SIM_RATE_SET")]
    public static readonly object SimulationRateSet;


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
       EventIds.PauseFull,
       EventIds.PauseActive,
       EventIds.PauseSimulator,
       EventIds.PauseFullWithSound,
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
