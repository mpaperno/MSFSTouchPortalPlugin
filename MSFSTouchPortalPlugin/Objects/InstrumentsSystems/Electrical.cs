﻿using MSFSTouchPortalPlugin.Attributes;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.InstrumentsSystems {
  [TouchPortalCategory("Electrical", "MSFS - Electrical")]
  internal class ElectricalMapping {
    #region Alternator & Battery

    [TouchPortalAction("MasterAlternator", "Master Alternator", "MSFS", "Toggle Master Alternator", "Toggle Master Alternator")]
    public object MASTER_ALTERNATOR { get; }

    [TouchPortalAction("MasterBattery", "Master Battery", "MSFS", "Toggle Master Battery", "Toggle Master Battery")]
    public object MASTER_BATTERY { get; }

    [TouchPortalAction("MasterBatteryAlternator", "Master Battery & Alternator", "MSFS", "Toggle Master Battery & Alternator", "Toggle Master Battery & Alternator")]
    public object MASTER_BATTERY_ALTERNATOR { get; }

    [TouchPortalAction("AlternatorIndex", "Alternator - Specific", "MSFS", "Toggle Specific Alternator", "Toggle Altenator - {0}")]
    [TouchPortalActionChoice(new string[] { "1", "2", "3", "4" }, "1")]
    public object ALTERNATOR_INDEX { get; }

    #endregion

    #region Lights

    [TouchPortalAction("StrobeLights", "Toggle/On/Off/Set Strobe Lights", "MSFS", "Toggle/On/Off/Set Strobe Lights", "Strobe Lights - {0}")]
    [TouchPortalActionChoice(new string[] { "Toggle", "On", "Off", "Set" }, "Toggle")]
    public object STROBE_LIGHTS { get; }

    [TouchPortalAction("PanelLights", "Toggle/On/Off/Set Panel Lights", "MSFS", "Toggle/On/Off/Set Panel Lights", "Panel Lights - {0}")]
    [TouchPortalActionChoice(new string[] { "Toggle", "On", "Off", "Set" }, "Toggle")]
    public object PANEL_LIGHTS { get; }

    [TouchPortalAction("LandingLights", "Toggle/On/Off/Set Landing Lights", "MSFS", "Toggle/On/Off/Set Landing Lights", "Landing Lights - {0}")]
    [TouchPortalActionChoice(new string[] { "Toggle", "On", "Off", "Set" }, "Toggle")]
    public object LANDING_LIGHTS { get; }

    [TouchPortalAction("ToggleLights", "Toggle All/Specific Lights", "MSFS", "Toggle All/Specific Lights", "Toggle Lights - {0}")]
    [TouchPortalActionChoice(new string[] { "All", "Beacon", "Taxi", "Logo", "Recognition", "Wing", "Nav", "Cabin" }, "All")]
    public object ALL_LIGHTS { get; }

    #endregion

  }

  [SimNotificationGroup(SimConnectWrapper.Groups.Electrical)]
  [TouchPortalCategoryMapping("Electrical")]
  internal enum Electrical {
    // Placeholder to offset each enum for SimConnect
    Init = 6000,

    #region Alternator & Battery

    [SimActionEvent]
    [TouchPortalActionMapping("MasterAlternator")]
    TOGGLE_MASTER_ALTERNATOR,
    
    [SimActionEvent]
    [TouchPortalActionMapping("MasterBattery")]
    TOGGLE_MASTER_BATTERY,
    
    [SimActionEvent]
    [TouchPortalActionMapping("MasterBatteryAlternator")]
    TOGGLE_MASTER_BATTERY_ALTERNATOR,
    
    [SimActionEvent]
    [TouchPortalActionMapping("AlternatorIndex", "1")]
    TOGGLE_ALTERNATOR1,
    
    [SimActionEvent]
    [TouchPortalActionMapping("AlternatorIndex", "2")]
    TOGGLE_ALTERNATOR2,
    
    [SimActionEvent]
    [TouchPortalActionMapping("AlternatorIndex", "3")]
    TOGGLE_ALTERNATOR3,

    [SimActionEvent]
    [TouchPortalActionMapping("AlternatorIndex", "4")]
    TOGGLE_ALTERNATOR4,

    #endregion

    #region Lights

    [SimActionEvent]
    [TouchPortalActionMapping("StrobeLights", "Toggle")]
    STROBES_TOGGLE,
    [SimActionEvent]
    [TouchPortalActionMapping("StrobeLights", "On")]
    STROBES_ON,
    [SimActionEvent]
    [TouchPortalActionMapping("StrobeLights", "Off")]
    STROBES_OFF,
    [SimActionEvent]
    [TouchPortalActionMapping("StrobeLights", "Set")]
    STROBES_SET,

    [SimActionEvent]
    [TouchPortalActionMapping("PanelLights", "Toggle")]
    PANEL_LIGHTS_TOGGLE,
    [SimActionEvent]
    [TouchPortalActionMapping("PanelLights", "On")]
    PANEL_LIGHTS_ON,
    [SimActionEvent]
    [TouchPortalActionMapping("PanelLights", "Off")]
    PANEL_LIGHTS_OFF,
    [SimActionEvent]
    [TouchPortalActionMapping("PanelLights", "Set")]
    PANEL_LIGHTS_SET,

    [SimActionEvent]
    [TouchPortalActionMapping("LandingLights", "Toggle")]
    LANDING_LIGHTS_TOGGLE,
    [SimActionEvent]
    [TouchPortalActionMapping("LandingLights", "On")]
    LANDING_LIGHTS_ON,
    [SimActionEvent]
    [TouchPortalActionMapping("LandingLights", "Off")]
    LANDING_LIGHTS_OFF,
    [SimActionEvent]
    [TouchPortalActionMapping("LandingLights", "Set")]
    LANDING_LIGHTS_SET,

    [SimActionEvent]
    [TouchPortalActionMapping("ToggleLights", "All")]
    ALL_LIGHTS_TOGGLE,
    [SimActionEvent]
    [TouchPortalActionMapping("ToggleLights", "Beacon")]
    TOGGLE_BEACON_LIGHTS,
    [SimActionEvent]
    [TouchPortalActionMapping("ToggleLights", "Taxi")]
    TOGGLE_TAXI_LIGHTS,
    [SimActionEvent]
    [TouchPortalActionMapping("ToggleLights", "Logo")]
    TOGGLE_LOGO_LIGHTS,
    [SimActionEvent]
    [TouchPortalActionMapping("ToggleLights", "Recognition")]
    TOGGLE_RECOGNITION_LIGHTS,
    [SimActionEvent]
    [TouchPortalActionMapping("ToggleLights", "Wing")]
    TOGGLE_WING_LIGHTS,
    [SimActionEvent]
    [TouchPortalActionMapping("ToggleLights", "Nav")]
    TOGGLE_NAV_LIGHTS,
    [SimActionEvent]
    [TouchPortalActionMapping("ToggleLights", "Cabin")]
    TOGGLE_CABIN_LIGHTS,

    #endregion
  }
}