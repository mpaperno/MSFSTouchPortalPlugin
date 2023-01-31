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
  [TouchPortalCategory(Groups.Electrical)]
  internal static class ElectricalMapping
  {
    #region Avionics

    [TouchPortalAction("AvionicsMasterSwitchAdj", "Avionics Master Switch", "Avionics {0}")]
    [TouchPortalActionMapping("TOGGLE_AVIONICS_MASTER", "Master Toggle")]
    [TouchPortalActionMapping("AVIONICS_MASTER_1_OFF", "Master 1 Off")]
    [TouchPortalActionMapping("AVIONICS_MASTER_1_ON",  "Master 1 On")]
    [TouchPortalActionMapping("AVIONICS_MASTER_2_OFF", "Master 2 Off")]
    [TouchPortalActionMapping("AVIONICS_MASTER_2_ON",  "Master 2 On")]
    public static readonly object AVIONICS_MASTER_ADJ;

    #endregion

    #region Alternator & Battery

    [TouchPortalAction("MasterBattery", "Master Battery", "Toggle Master Battery")]
    [TouchPortalActionMapping("TOGGLE_MASTER_BATTERY")]
    public static readonly object MASTER_BATTERY;

    [TouchPortalAction("MasterBatteryAlternator", "Master Battery & Alternator", "Toggle Master Battery & Alternator")]
    [TouchPortalActionMapping("TOGGLE_MASTER_BATTERY_ALTERNATOR")]
    public static readonly object MASTER_BATTERY_ALTERNATOR;

    [TouchPortalAction("AlternatorIndex", "Alternator Toggle", "Toggle Alternator - {0}")]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("TOGGLE_MASTER_ALTERNATOR", "Master", 0)]
    [TouchPortalActionMapping("TOGGLE_ALTERNATOR1", "1")]
    [TouchPortalActionMapping("TOGGLE_ALTERNATOR2", "2")]
    [TouchPortalActionMapping("TOGGLE_ALTERNATOR3", "3")]
    [TouchPortalActionMapping("TOGGLE_ALTERNATOR4", "4")]
    public static readonly object ALTERNATOR_INDEX;

    [TouchPortalAction("AlternatorSwitch", "Alternator Control", "{0} Alternator at Index {1}")]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("TOGGLE_MASTER_ALTERNATOR", "Toggle")]
    [TouchPortalActionMapping("ALTERNATOR_ON", "Turn On")]
    [TouchPortalActionMapping("ALTERNATOR_OFF", "Turn Off")]
    [TouchPortalActionText("0", 0, 99)]
    public static readonly object ALTERNATOR_SWITCH;

    #endregion

    #region Lights

    [TouchPortalAction("LandingLights", "Landing Lights Switch/Direction", "Landing Lights {0} at circuit index {1}")]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("LANDING_LIGHTS_TOGGLE", "Toggle")]
    [TouchPortalActionMapping("LANDING_LIGHTS_ON", "On")]
    [TouchPortalActionMapping("LANDING_LIGHTS_OFF", "Off")]
    [TouchPortalActionMapping("LANDING_LIGHT_LEFT", "Left")]
    [TouchPortalActionMapping("LANDING_LIGHT_RIGHT", "Right")]
    [TouchPortalActionMapping("LANDING_LIGHT_UP", "Up")]
    [TouchPortalActionMapping("LANDING_LIGHT_DOWN", "Down")]
    [TouchPortalActionMapping("LANDING_LIGHT_HOME", "Home")]
    [TouchPortalActionText("0", 0, 99)]
    public static readonly object LANDING_LIGHTS;

    [TouchPortalAction("LightSwitches", "Light Switch Control",
      "Light Switch Controls. NOTE: The 'All' and 'Circuit' types can only be Toggled.",
      "Switch {0} Light {1} at circuit index {2}", UserValueIndex = -1)]
    [TouchPortalActionChoice()]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("ALL_LIGHTS_TOGGLE", "All", "Toggle")]
    [TouchPortalActionMapping("TOGGLE_BEACON_LIGHTS", "Beacon", "Toggle")]
    [TouchPortalActionMapping("TOGGLE_CABIN_LIGHTS", "Cabin", "Toggle")]
    [TouchPortalActionMapping("GLARESHIELD_LIGHTS_TOGGLE", "Glareshield", "Toggle")]
    [TouchPortalActionMapping("LANDING_LIGHTS_TOGGLE", "Landing", "Toggle")]
    [TouchPortalActionMapping("TOGGLE_LOGO_LIGHTS", "Logo", "Toggle")]
    [TouchPortalActionMapping("TOGGLE_NAV_LIGHTS", "Nav", "Toggle")]
    [TouchPortalActionMapping("PANEL_LIGHTS_TOGGLE", "Panel", "Toggle")]
    [TouchPortalActionMapping("PEDESTRAL_LIGHTS_TOGGLE", "Pedestal", "Toggle")]
    [TouchPortalActionMapping("TOGGLE_RECOGNITION_LIGHTS", "Recognition", "Toggle")]
    [TouchPortalActionMapping("STROBES_TOGGLE", "Strobe", "Toggle")]
    [TouchPortalActionMapping("TOGGLE_TAXI_LIGHTS", "Taxi", "Toggle")]
    [TouchPortalActionMapping("TOGGLE_WING_LIGHTS", "Wing", "Toggle")]
    [TouchPortalActionMapping("ELECTRICAL_CIRCUIT_TOGGLE", "Circuit")]
    //[TouchPortalActionMapping("ALL_LIGHTS_TOGGLE",       "All",         "On")]
    [TouchPortalActionMapping("BEACON_LIGHTS_ON", "Beacon", "On")]
    [TouchPortalActionMapping("CABIN_LIGHTS_ON", "Cabin", "On")]
    [TouchPortalActionMapping("GLARESHIELD_LIGHTS_ON", "Glareshield", "On")]
    [TouchPortalActionMapping("LANDING_LIGHTS_ON", "Landing", "On")]
    [TouchPortalActionMapping("LOGO_LIGHTS_SET", "Logo", "On", 1)]
    [TouchPortalActionMapping("NAV_LIGHTS_ON", "Nav", "On")]
    [TouchPortalActionMapping("PANEL_LIGHTS_ON", "Panel", "On")]
    [TouchPortalActionMapping("PEDESTRAL_LIGHTS_ON", "Pedestal", "On")]
    [TouchPortalActionMapping("RECOGNITION_LIGHTS_SET", "Recognition", "On", 1)]
    [TouchPortalActionMapping("STROBES_ON", "Strobe", "On")]
    [TouchPortalActionMapping("TAXI_LIGHTS_ON", "Taxi", "On")]
    [TouchPortalActionMapping("WING_LIGHTS_ON", "Wing", "On")]
    //[TouchPortalActionMapping("ALL_LIGHTS_TOGGLE",       "All",         "Off")]
    [TouchPortalActionMapping("BEACON_LIGHTS_OFF", "Beacon", "Off")]
    [TouchPortalActionMapping("CABIN_LIGHTS_OFF", "Cabin", "Off")]
    [TouchPortalActionMapping("GLARESHIELD_LIGHTS_OFF", "Glareshield", "Off")]
    [TouchPortalActionMapping("LANDING_LIGHTS_OFF", "Landing", "Off")]
    [TouchPortalActionMapping("LOGO_LIGHTS_SET", "Logo", "Off", 0)]
    [TouchPortalActionMapping("NAV_LIGHTS_OFF", "Nav", "Off")]
    [TouchPortalActionMapping("PANEL_LIGHTS_OFF", "Panel", "Off")]
    [TouchPortalActionMapping("PEDESTRAL_LIGHTS_OFF", "Pedestal", "Off")]
    [TouchPortalActionMapping("RECOGNITION_LIGHTS_SET", "Recognition", "Off", 0)]
    [TouchPortalActionMapping("STROBES_OFF", "Strobe", "Off")]
    [TouchPortalActionMapping("TAXI_LIGHTS_OFF", "Taxi", "Off")]
    [TouchPortalActionMapping("WING_LIGHTS_OFF", "Wing", "Off")]
    [TouchPortalActionText("0", 0, 99)]
    public static readonly object LIGHT_SWITCHES;

    //[TouchPortalAction("LightSwitches", "Light Switch Set", "Set Light Switch States.", "Set {0} Light Switch to {1} (0/1) at circuit index {2}")]
    //[TouchPortalActionChoice()]
    //[TouchPortalActionMapping("BEACON_LIGHTS_SET", "Beacon")]
    //[TouchPortalActionMapping("CABIN_LIGHTS_SET", "Cabin")]
    //[TouchPortalActionMapping("GLARESHIELD_LIGHTS_SET", "Glareshield")]
    //[TouchPortalActionMapping("LANDING_LIGHTS_SET", "Landing")]
    //[TouchPortalActionMapping("LOGO_LIGHTS_SET", "Logo")]
    //[TouchPortalActionMapping("NAV_LIGHTS_SET", "Nav")]
    //[TouchPortalActionMapping("PANEL_LIGHTS_SET", "Panel")]
    //[TouchPortalActionMapping("PEDESTRAL_LIGHTS_SET", "Pedestal")]
    //[TouchPortalActionMapping("RECOGNITION_LIGHTS_SET", "Recognition")]
    //[TouchPortalActionMapping("STROBES_SET", "Strobe")]
    //[TouchPortalActionMapping("TAXI_LIGHTS_SET", "Taxi")]
    //[TouchPortalActionMapping("WING_LIGHTS_SET", "Wing")]
    //[TouchPortalActionText("0", 0, 1)]
    //[TouchPortalActionText("0", 0, 99)]
    //public static readonly object LIGHT_SWITCH_SET;

    [TouchPortalAction("ExternalPower", "External Power", "External Power Control.", "External Power {0} at index {1}", UserValueIndex = -1)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("TOGGLE_EXTERNAL_POWER", "Toggle")]
    [TouchPortalActionMapping("SET_EXTERNAL_POWER", "Off", 0)]
    [TouchPortalActionMapping("SET_EXTERNAL_POWER", "On", 1)]
    [TouchPortalActionText("0", 0, 99)]
    public static readonly object EXT_POWER;

    [TouchPortalAction("LightPorentiometerSet", "Light Dimming", true,
      "Set Light Potentiometer {0} to {1} (0 to 100)",
      "Set Light Potentiometer {0}in Value\nRange:"
    )]
    [TouchPortalActionChoice()]
    [TouchPortalActionText("0", 0, 100)]
    [TouchPortalActionMapping("LIGHT_POTENTIOMETER_SET", "0")]
    [TouchPortalActionMapping("LIGHT_POTENTIOMETER_1_SET", "1")]
    [TouchPortalActionMapping("LIGHT_POTENTIOMETER_2_SET", "2")]
    [TouchPortalActionMapping("LIGHT_POTENTIOMETER_3_SET", "3")]
    [TouchPortalActionMapping("LIGHT_POTENTIOMETER_4_SET", "4")]
    [TouchPortalActionMapping("LIGHT_POTENTIOMETER_5_SET", "5")]
    [TouchPortalActionMapping("LIGHT_POTENTIOMETER_6_SET", "6")]
    [TouchPortalActionMapping("LIGHT_POTENTIOMETER_7_SET", "7")]
    [TouchPortalActionMapping("LIGHT_POTENTIOMETER_8_SET", "8")]
    [TouchPortalActionMapping("LIGHT_POTENTIOMETER_9_SET", "9")]
    [TouchPortalActionMapping("LIGHT_POTENTIOMETER_10_SET", "10")]
    [TouchPortalActionMapping("LIGHT_POTENTIOMETER_11_SET", "11")]
    [TouchPortalActionMapping("LIGHT_POTENTIOMETER_12_SET", "12")]
    [TouchPortalActionMapping("LIGHT_POTENTIOMETER_13_SET", "13")]
    [TouchPortalActionMapping("LIGHT_POTENTIOMETER_14_SET", "14")]
    [TouchPortalActionMapping("LIGHT_POTENTIOMETER_15_SET", "15")]
    [TouchPortalActionMapping("LIGHT_POTENTIOMETER_16_SET", "16")]
    [TouchPortalActionMapping("LIGHT_POTENTIOMETER_17_SET", "17")]
    [TouchPortalActionMapping("LIGHT_POTENTIOMETER_18_SET", "18")]
    [TouchPortalActionMapping("LIGHT_POTENTIOMETER_19_SET", "19")]
    [TouchPortalActionMapping("LIGHT_POTENTIOMETER_20_SET", "20")]
    [TouchPortalActionMapping("LIGHT_POTENTIOMETER_21_SET", "21")]
    [TouchPortalActionMapping("LIGHT_POTENTIOMETER_22_SET", "22")]
    [TouchPortalActionMapping("LIGHT_POTENTIOMETER_23_SET", "23")]
    [TouchPortalActionMapping("LIGHT_POTENTIOMETER_24_SET", "24")]
    [TouchPortalActionMapping("LIGHT_POTENTIOMETER_25_SET", "25")]
    [TouchPortalActionMapping("LIGHT_POTENTIOMETER_26_SET", "26")]
    [TouchPortalActionMapping("LIGHT_POTENTIOMETER_27_SET", "27")]
    [TouchPortalActionMapping("LIGHT_POTENTIOMETER_28_SET", "28")]
    [TouchPortalActionMapping("LIGHT_POTENTIOMETER_29_SET", "29")]
    [TouchPortalActionMapping("LIGHT_POTENTIOMETER_30_SET", "30")]
    public static readonly object LIGHT_POTENTIOMETER_SET;

    #endregion

    #region DEPRECATED
    // Preserve backwards compatibility with hidden actions,

    [TouchPortalAction("AvionicsMasterSwitch", "Avionics Master Switch", "Toggle Avionics Master", Deprecated = true)]
    [TouchPortalActionMapping("TOGGLE_AVIONICS_MASTER")]
    public static readonly object TOGGLE_AVIONICS_MASTER;

    [TouchPortalAction("MasterAlternator", "Master Alternator", "Toggle Master Alternator", Deprecated = true)]
    [TouchPortalActionMapping("TOGGLE_MASTER_ALTERNATOR")]
    public static readonly object MASTER_ALTERNATOR;

    [TouchPortalAction("ToggleLights", "Toggle All/Specific Lights", "Toggle Lights - {0}", Deprecated = true)]
    [TouchPortalActionChoice(new[] { "All", "Beacon", "Taxi", "Logo", "Recognition", "Wing", "Nav", "Cabin" })]
    [TouchPortalActionMapping("ALL_LIGHTS_TOGGLE", "All")]
    [TouchPortalActionMapping("TOGGLE_BEACON_LIGHTS", "Beacon")]
    [TouchPortalActionMapping("TOGGLE_TAXI_LIGHTS", "Taxi")]
    [TouchPortalActionMapping("TOGGLE_LOGO_LIGHTS", "Logo")]
    [TouchPortalActionMapping("TOGGLE_RECOGNITION_LIGHTS", "Recognition")]
    [TouchPortalActionMapping("TOGGLE_WING_LIGHTS", "Wing")]
    [TouchPortalActionMapping("TOGGLE_NAV_LIGHTS", "Nav")]
    [TouchPortalActionMapping("TOGGLE_CABIN_LIGHTS", "Cabin")]
    public static readonly object ALL_LIGHTS;
    [TouchPortalAction("StrobeLights", "Toggle/On/Off Strobe Lights", "Strobe Lights - {0}", Deprecated = true)]
    [TouchPortalActionChoice(new[] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("STROBES_TOGGLE", "Toggle")]
    [TouchPortalActionMapping("STROBES_ON", "On")]
    [TouchPortalActionMapping("STROBES_OFF", "Off")]
    public static readonly object STROBE_LIGHTS;
    [TouchPortalAction("PanelLights", "Toggle/On/Off Panel Lights", "Panel Lights - {0}", Deprecated = true)]
    [TouchPortalActionChoice(new[] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("PANEL_LIGHTS_TOGGLE", "Toggle")]
    [TouchPortalActionMapping("PANEL_LIGHTS_ON", "On")]
    [TouchPortalActionMapping("PANEL_LIGHTS_OFF", "Off")]
    public static readonly object PANEL_LIGHTS;

    #endregion

  }
}
