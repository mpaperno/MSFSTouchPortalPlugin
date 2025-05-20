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

    [TouchPortalAction("Failures", "Failures", "Toggle Failure - {0}")]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("TOGGLE_ELECTRICAL_FAILURE", "Electrical")]
    [TouchPortalActionMapping("TOGGLE_VACUUM_FAILURE", "Vacuum")]
    [TouchPortalActionMapping("TOGGLE_PITOT_BLOCKAGE", "Pitot")]
    [TouchPortalActionMapping("TOGGLE_STATIC_PORT_BLOCKAGE", "Static Port")]
    [TouchPortalActionMapping("TOGGLE_HYDRAULIC_FAILURE", "Hydraulic")]
    [TouchPortalActionMapping("TOGGLE_TOTAL_BRAKE_FAILURE", "Total Brake")]
    [TouchPortalActionMapping("TOGGLE_LEFT_BRAKE_FAILURE", "Left Brake")]
    [TouchPortalActionMapping("TOGGLE_RIGHT_BRAKE_FAILURE", "Right Brake")]
    [TouchPortalActionMapping("TOGGLE_ENGINE1_FAILURE", "Engine 1")]
    [TouchPortalActionMapping("TOGGLE_ENGINE2_FAILURE", "Engine 2")]
    [TouchPortalActionMapping("TOGGLE_ENGINE3_FAILURE", "Engine 3")]
    [TouchPortalActionMapping("TOGGLE_ENGINE4_FAILURE", "Engine 4")]
    public static readonly object FAILURES;

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
    [TouchPortalConnectorMeta(DefaultMin = 0.0, DefaultMax = 3.0)]
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
#if !FSX
       EventIds.PauseFull,
       EventIds.PauseActive,
       EventIds.PauseSimulator,
       EventIds.PauseFullWithSound,
#endif
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

#if !FSX
    public static readonly TouchPortalEvent SimPauseEvent =
      new(
        "SimPauseEvent",
        "Simulator Pause State Change",
        "When the simulator's Pause State changes",
        [
          [ "NewState",     "New Pause State (numeric)" ],
          [ "NewStateStr",  "New Pause State (string)" ],
          [ "PrevState",    "Previous Pause State (numeric)" ],
          [ "PrevStateStr", "Previous Pause State (string)" ],
        ]
      ) {
      Description = "Describes the Pause State of the simulator.\n" +
        "The numeric status is a 4-bit Flag (bit-field) type which may be OR'd together when multiple pause states are active (eg. 'active' and 'sim' = `12` (`0xC`), or `1100` in binary) .\n" +
        "The string version contains text representations of the bit values, possibly joined with `|` character when there are multiple set flags (eg. `ACTIVE|SIM`).\n" +
        "Possible values, numeric and text:\n" +
         "* `0` (`OFF`): No Pause\n" +
         "* `1` (`FULL`): Full Pause with time (sim + traffic + etc...)  (SET_PAUSE 1 / Dev -> Options -> Pause)\n" +
         "* `2` (`FULL_WITH_SOUND`): FSX Legacy Pause (not used anymore)\n" +
         "* `4` (`ACTIVE`): Pause was activated using the \"Active Pause\" Button (position/attitude freeze)\n" +
         "* `8` (`SIM`): Pause the player sim but traffic, multi, etc., will still run (SET_PAUSE_ON / ESC menu pause)\n"
     };
#endif

  }
}
