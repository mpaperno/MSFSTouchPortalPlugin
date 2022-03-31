using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Enums;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.InstrumentsSystems
{
  [SimNotificationGroup(Groups.Electrical)]
  [TouchPortalCategory("Electrical", "MSFS - Electrical")]
  internal static class ElectricalMapping {
    #region Avionics

    [TouchPortalAction("AvionicsMasterSwitch", "Avionics Master", "MSFS", "Toggle Avionics Master", "Toggle Avionics Master")]
    [TouchPortalActionMapping("TOGGLE_AVIONICS_MASTER")]
    public static readonly object TOGGLE_AVIONICS_MASTER;

    #endregion

    #region Alternator & Battery

    [TouchPortalAction("MasterAlternator", "Master Alternator", "MSFS", "Toggle Master Alternator", "Toggle Master Alternator")]
    [TouchPortalActionMapping("TOGGLE_MASTER_ALTERNATOR")]
    public static readonly object MASTER_ALTERNATOR;

    [TouchPortalAction("MasterBattery", "Master Battery", "MSFS", "Toggle Master Battery", "Toggle Master Battery")]
    [TouchPortalActionMapping("TOGGLE_MASTER_BATTERY")]
    public static readonly object MASTER_BATTERY;

    [TouchPortalAction("MasterBatteryAlternator", "Master Battery & Alternator", "MSFS", "Toggle Master Battery & Alternator", "Toggle Master Battery & Alternator")]
    [TouchPortalActionMapping("TOGGLE_MASTER_BATTERY_ALTERNATOR")]
    public static readonly object MASTER_BATTERY_ALTERNATOR;

    [TouchPortalAction("AlternatorIndex", "Alternator - Specific", "MSFS", "Toggle Specific Alternator", "Toggle Alternator - {0}")]
    [TouchPortalActionChoice(new[] { "1", "2", "3", "4" })]
    [TouchPortalActionMapping("TOGGLE_ALTERNATOR1", "1")]
    [TouchPortalActionMapping("TOGGLE_ALTERNATOR2", "2")]
    [TouchPortalActionMapping("TOGGLE_ALTERNATOR3", "3")]
    [TouchPortalActionMapping("TOGGLE_ALTERNATOR4", "4")]
    public static readonly object ALTERNATOR_INDEX;

    #endregion

    #region Lights

    [TouchPortalAction("StrobeLights", "Toggle/On/Off Strobe Lights", "MSFS", "Toggle/On/Off Strobe Lights", "Strobe Lights - {0}")]
    [TouchPortalActionChoice(new[] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("STROBES_TOGGLE", "Toggle")]
    [TouchPortalActionMapping("STROBES_ON", "On")]
    [TouchPortalActionMapping("STROBES_OFF", "Off")]
    public static readonly object STROBE_LIGHTS;

    [TouchPortalAction("PanelLights", "Toggle/On/Off Panel Lights", "MSFS", "Toggle/On/Off Panel Lights", "Panel Lights - {0}")]
    [TouchPortalActionChoice(new[] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("PANEL_LIGHTS_TOGGLE", "Toggle")]
    [TouchPortalActionMapping("PANEL_LIGHTS_ON", "On")]
    [TouchPortalActionMapping("PANEL_LIGHTS_OFF", "Off")]
    public static readonly object PANEL_LIGHTS;

    [TouchPortalAction("LandingLights", "Toggle/On/Off Landing Lights", "MSFS", "Toggle/On/Off Landing Lights", "Landing Lights - {0}")]
    [TouchPortalActionChoice(new[] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("LANDING_LIGHTS_TOGGLE", "Toggle")]
    [TouchPortalActionMapping("LANDING_LIGHTS_ON", "On")]
    [TouchPortalActionMapping("LANDING_LIGHTS_OFF", "Off")]
    public static readonly object LANDING_LIGHTS;

    [TouchPortalAction("ToggleLights", "Toggle All/Specific Lights", "MSFS", "Toggle All/Specific Lights", "Toggle Lights - {0}")]
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

    #endregion

  }
}
