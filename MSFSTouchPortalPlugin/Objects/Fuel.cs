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

    [TouchPortalActionMapping("FUEL_SELECTOR_2_ALL", new[] { "2", "All" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_2_OFF", new[] { "2", "Off" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_2_LEFT", new[] { "2", "Left" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_2_RIGHT", new[] { "2", "Right" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_2_LEFT_MAIN", new[] { "2", "Left - Main" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_2_RIGHT_MAIN", new[] { "2", "Right - Main" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_2_LEFT_AUX", new[] { "2", "Left - Aux" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_2_RIGHT_AUX", new[] { "2", "Right - Aux" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_2_CENTER", new[] { "2", "Center" })]

    [TouchPortalActionMapping("FUEL_SELECTOR_3_ALL", new[] { "3", "All" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_3_OFF", new[] { "3", "Off" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_3_LEFT", new[] { "3", "Left" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_3_RIGHT", new[] { "3", "Right" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_3_LEFT_MAIN", new[] { "3", "Left - Main" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_3_RIGHT_MAIN", new[] { "3", "Right - Main" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_3_LEFT_AUX", new[] { "3", "Left - Aux" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_3_RIGHT_AUX", new[] { "3", "Right - Aux" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_3_CENTER", new[] { "3", "Center" })]

    [TouchPortalActionMapping("FUEL_SELECTOR_4_ALL", new[] { "4", "All" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_4_OFF", new[] { "4", "Off" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_4_LEFT", new[] { "4", "Left" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_4_RIGHT", new[] { "4", "Right" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_4_RIGHT_MAIN", new[] { "4", "Right - Main" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_4_LEFT_MAIN", new[] { "4", "Left - Main" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_4_LEFT_AUX", new[] { "4", "Left - Aux" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_4_RIGHT_AUX", new[] { "4", "Right - Aux" })]
    [TouchPortalActionMapping("FUEL_SELECTOR_4_CENTER", new[] { "4", "Center" })]
    public static readonly object FUEL_SELECTORS;

    [TouchPortalAction("Primers", "Primers", "Toggle Primer(s): {0}")]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("TOGGLE_PRIMER", "All")]
    [TouchPortalActionMapping("TOGGLE_PRIMER1", "1")]
    [TouchPortalActionMapping("TOGGLE_PRIMER2", "2")]
    [TouchPortalActionMapping("TOGGLE_PRIMER3", "3")]
    [TouchPortalActionMapping("TOGGLE_PRIMER4", "4")]
    public static readonly object PRIMERS;

    [TouchPortalAction("FuelDump", "Fuel Dump - Toggle", "Toggle Fuel Dump")]
    [TouchPortalActionMapping("FUEL_DUMP_TOGGLE")]
    public static readonly object FUEL_DUMP;

    [TouchPortalAction("CrossFeed", "Cross Feed Switch", "Cross Feed - {0}")]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("CROSS_FEED_TOGGLE", "Toggle")]
    [TouchPortalActionMapping("CROSS_FEED_OPEN", "Open")]
    [TouchPortalActionMapping("CROSS_FEED_OFF", "Off")]
    public static readonly object CROSS_FEED;

    [TouchPortalAction("FuelValve", "Fuel Valve", "Toggle Fuel Valve: {0}")]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("TOGGLE_FUEL_VALVE_ALL", "All")]
    [TouchPortalActionMapping("TOGGLE_FUEL_VALVE_ENG1", "1")]
    [TouchPortalActionMapping("TOGGLE_FUEL_VALVE_ENG2", "2")]
    [TouchPortalActionMapping("TOGGLE_FUEL_VALVE_ENG3", "3")]
    [TouchPortalActionMapping("TOGGLE_FUEL_VALVE_ENG4", "4")]
    public static readonly object FUEL_VALVE;

    #region Fuel Pump

    [TouchPortalAction("FuelPump", "Fuel Pump", "Toggle Fuel Pump")]
    [TouchPortalActionMapping("FUEL_PUMP")]
    public static readonly object FUEL_PUMP;

    [TouchPortalAction("ElectricFuelPump", "Electric Fuel Pump", "Toggle Electric Fuel Pump: {0}")]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("TOGGLE_ELECT_FUEL_PUMP", "All")]
    [TouchPortalActionMapping("TOGGLE_ELECT_FUEL_PUMP1", "1")]
    [TouchPortalActionMapping("TOGGLE_ELECT_FUEL_PUMP2", "2")]
    [TouchPortalActionMapping("TOGGLE_ELECT_FUEL_PUMP3", "3")]
    [TouchPortalActionMapping("TOGGLE_ELECT_FUEL_PUMP4", "4")]
    public static readonly object ELECTRIC_FUEL_PUMP;

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
