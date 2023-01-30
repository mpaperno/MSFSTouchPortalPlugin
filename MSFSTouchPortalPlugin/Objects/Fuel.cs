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

namespace MSFSTouchPortalPlugin.Objects
{
  [TouchPortalCategory(Groups.Fuel)]
  internal static class FuelMapping
  {

    [TouchPortalAction("AddFuel", "Add Fuel", "Add Fuel (1 to 65535 or zero for 25% of capacity)", true,
      "Add {0} amount of Fuel",
      "Add Fuel\nin Value Range:"
    )]
    [TouchPortalActionText("0", 0, 65535)]
    [TouchPortalActionMapping("ADD_FUEL_QUANTITY")]
    public static readonly object ADD_FUEL;

    [TouchPortalAction("FuelRefillRepair", "Refuel & Repair", "Action: {0}")]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("REQUEST_FUEL_KEY", "Request Fuel (parked)")]
    [TouchPortalActionMapping("REPAIR_AND_REFUEL", "Repair & Refuel (unrealistic)")]
    public static readonly object FUEL_R_AND_R;

    [TouchPortalAction("FuelSelectors", "Fuel Selectors", "Fuel Selector {0} - {1}")]
    [TouchPortalActionChoice()]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("FUEL_SELECTOR_ALL", new[] { "1", "All" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_OFF", new[] { "1", "Off" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_LEFT", new[] { "1", "Left" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_RIGHT", new[] { "1", "Right" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_LEFT_MAIN", new[] { "1", "Left - Main" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_RIGHT_MAIN", new[] { "1", "Right - Main" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_LEFT_AUX", new[] { "1", "Left - Aux" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_RIGHT_AUX", new[] { "1", "Right - Aux" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_CENTER", new[] { "1", "Center" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_1_CROSSFEED", new[] { "1", "Crossfeed" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_1_ISOLATE",   new[] { "1", "Isolate" })]

    [TouchPortalActionMapping("FUEL_SELECTOR_2_ALL", new[] { "2", "All" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_2_OFF", new[] { "2", "Off" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_2_LEFT", new[] { "2", "Left" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_2_RIGHT", new[] { "2", "Right" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_2_LEFT_MAIN", new[] { "2", "Left - Main" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_2_RIGHT_MAIN", new[] { "2", "Right - Main" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_2_LEFT_AUX", new[] { "2", "Left - Aux" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_2_RIGHT_AUX", new[] { "2", "Right - Aux" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_2_CENTER", new[] { "2", "Center" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_2_CROSSFEED", new[] { "2", "Crossfeed" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_2_ISOLATE",   new[] { "2", "Isolate" })]

    [TouchPortalActionMapping("FUEL_SELECTOR_3_ALL", new[] { "3", "All" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_3_OFF", new[] { "3", "Off" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_3_LEFT", new[] { "3", "Left" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_3_RIGHT", new[] { "3", "Right" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_3_LEFT_MAIN", new[] { "3", "Left - Main" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_3_RIGHT_MAIN", new[] { "3", "Right - Main" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_3_LEFT_AUX", new[] { "3", "Left - Aux" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_3_RIGHT_AUX", new[] { "3", "Right - Aux" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_3_CENTER", new[] { "3", "Center" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_3_CROSSFEED", new[] { "3", "Crossfeed" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_3_ISOLATE",   new[] { "3", "Isolate" })]

    [TouchPortalActionMapping("FUEL_SELECTOR_4_ALL", new[] { "4", "All" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_4_OFF", new[] { "4", "Off" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_4_LEFT", new[] { "4", "Left" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_4_RIGHT", new[] { "4", "Right" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_4_RIGHT_MAIN", new[] { "4", "Right - Main" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_4_LEFT_MAIN", new[] { "4", "Left - Main" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_4_LEFT_AUX", new[] { "4", "Left - Aux" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_4_RIGHT_AUX", new[] { "4", "Right - Aux" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_4_CENTER", new[] { "4", "Center" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_4_CROSSFEED", new[] { "4", "Crossfeed" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_4_ISOLATE",   new[] { "4", "Isolate" })]
    public static readonly object FUEL_SELECTORS;

    [TouchPortalAction("FuelSystem", "Fuel System Component", "Set {0} on Fuel System @ Index {1}")]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("FUELSYSTEM_PUMP_SET",    "Pump Auto", 2 )]
    [TouchPortalActionMapping("FUELSYSTEM_PUMP_OFF",    "Pump Off"    )]
    [TouchPortalActionMapping("FUELSYSTEM_PUMP_ON",     "Pump On"     )]
    [TouchPortalActionMapping("FUELSYSTEM_PUMP_TOGGLE", "Pump Toggle" )]
    [TouchPortalActionMapping("FUELSYSTEM_TRIGGER_OFF",    "Trigger Event Off")]
    [TouchPortalActionMapping("FUELSYSTEM_TRIGGER_ON",     "Trigger Event On")]
    [TouchPortalActionMapping("FUELSYSTEM_TRIGGER_TOGGLE", "Trigger Event Toggle")]
    [TouchPortalActionMapping("FUELSYSTEM_VALVE_OFF",    "Valve Close")]
    [TouchPortalActionMapping("FUELSYSTEM_VALVE_ON",     "Valve Open")]
    [TouchPortalActionMapping("FUELSYSTEM_VALVE_TOGGLE", "Valve Toggle")]
    [TouchPortalActionText("1", 0, 99)]
    public static readonly object FUEL_SYSTEM;

    #region Engine-related

    [TouchPortalAction("CrossFeed", "Cross Feed Switch", "Cross Feed - {0}")]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("CROSS_FEED_TOGGLE", "Toggle")]
    [TouchPortalActionMapping("CROSS_FEED_OPEN", "Open")]
    [TouchPortalActionMapping("CROSS_FEED_OFF", "Off")]
    [TouchPortalActionMapping("CROSS_FEED_LEFT_TO_RIGHT", "Left To Right")]
    [TouchPortalActionMapping("CROSS_FEED_RIGHT_TO_LEFT", "Right To Left")]
    public static readonly object CROSS_FEED;

    [TouchPortalAction("FuelValve", "Fuel Valve", "Toggle Fuel Valve: {0}")]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("TOGGLE_FUEL_VALVE_ALL", "All")]
    [TouchPortalActionMapping("TOGGLE_FUEL_VALVE_ENG1", "1")]
    [TouchPortalActionMapping("TOGGLE_FUEL_VALVE_ENG2", "2")]
    [TouchPortalActionMapping("TOGGLE_FUEL_VALVE_ENG3", "3")]
    [TouchPortalActionMapping("TOGGLE_FUEL_VALVE_ENG4", "4")]
    public static readonly object FUEL_VALVE;

    [TouchPortalAction("ElectricFuelPumpSet", "Electric Fuel Pump Set", "Set Electric Fuel Pump {0} to {1} (0 = Off; 1 = On; 2 = Auto)")]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("ELECT_FUEL_PUMP1_SET", "1")]
    [TouchPortalActionMapping("ELECT_FUEL_PUMP2_SET", "2")]
    [TouchPortalActionMapping("ELECT_FUEL_PUMP3_SET", "3")]
    [TouchPortalActionMapping("ELECT_FUEL_PUMP4_SET", "4")]
    [TouchPortalActionText("1", 0, 2)]
    public static readonly object ELECTRIC_FUEL_PUMP_SET;

    [TouchPortalAction("ElectricFuelPump", "Electric Fuel Pump Toggle", "Toggle Electric Fuel Pump: {0}")]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("TOGGLE_ELECT_FUEL_PUMP", "All")]
    [TouchPortalActionMapping("TOGGLE_ELECT_FUEL_PUMP1", "1")]
    [TouchPortalActionMapping("TOGGLE_ELECT_FUEL_PUMP2", "2")]
    [TouchPortalActionMapping("TOGGLE_ELECT_FUEL_PUMP3", "3")]
    [TouchPortalActionMapping("TOGGLE_ELECT_FUEL_PUMP4", "4")]
    public static readonly object ELECTRIC_FUEL_PUMP_TOGGLE;

    #endregion

    [TouchPortalAction("FuelDump", "Fuel Dump / Tank Drop", "Action: {0}")]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("FUEL_DUMP_TOGGLE", "Toggle Fuel Dump")]
    [TouchPortalActionMapping("RELEASE_DROP_TANK_1", "Release Drop Tank 1")]
    [TouchPortalActionMapping("RELEASE_DROP_TANK_2", "Release Drop Tank 2")]
    [TouchPortalActionMapping("RELEASE_DROP_TANK_ALL", "Release All Drop Tanks")]
    public static readonly object FUEL_DUMP;


    #region DEPRECATED
    // Preserve backwards compatibility with hidden actions,

    [TouchPortalAction("FuelPump", "Fuel Pump Toggle", "Toggle Fuel Pump", Deprecated = true)]
    [TouchPortalActionMapping("FUEL_PUMP")]
    public static readonly object FUEL_PUMP;

    #endregion
  }


  /*
   FUEL_SELECTOR_SET', '''Sets selector 1 position (see code list below),
   FUEL_TANK_SELECTOR_OFF = 0
   FUEL_TANK_SELECTOR_ALL = 1
   FUEL_TANK_SELECTOR_LEFT = 2
   FUEL_TANK_SELECTOR_RIGHT = 3
   FUEL_TANK_SELECTOR_LEFT_AUX = 4
   FUEL_TANK_SELECTOR_RIGHT_AUX = 5
   FUEL_TANK_SELECTOR_CENTER = 6
   FUEL_TANK_SELECTOR_CENTER2 = 7
   FUEL_TANK_SELECTOR_CENTER3 = 8
   FUEL_TANK_SELECTOR_EXTERNAL1 = 9
   FUEL_TANK_SELECTOR_EXTERNAL2 = 10
   FUEL_TANK_SELECTOR_RIGHT_TIP = 11
   FUEL_TANK_SELECTOR_LEFT_TIP = 12
   FUEL_TANK_SELECTOR_CROSSFEED = 13
   FUEL_TANK_SELECTOR_CROSSFEED_L2R = 14
   FUEL_TANK_SELECTOR_CROSSFEED_R2L = 15
   FUEL_TANK_SELECTOR_BOTH = 16
   FUEL_TANK_SELECTOR_EXTERNAL_ALL = 17
   FUEL_TANK_SELECTOR_ISOLATE = 18''', "Shared Cockpit"),

   FUEL_SELECTOR_SET,
   FUEL_SELECTOR_2_SET,
   FUEL_SELECTOR_3_SET,
   FUEL_SELECTOR_4_SET,
  */

}
