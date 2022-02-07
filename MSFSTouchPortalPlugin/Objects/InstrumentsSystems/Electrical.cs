using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Enums;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.InstrumentsSystems {
  [SimVarDataRequestGroup]
  [TouchPortalCategory("Electrical", "MSFS - Electrical")]
  internal static class ElectricalMapping {
    #region Alternator & Battery

    [SimVarDataRequest]
    [TouchPortalAction("MasterAlternator", "Master Alternator", "MSFS", "Toggle Master Alternator", "Toggle Master Alternator")]
    [TouchPortalState("MasterAlternator", "text", "Master Alternator Status", "")]
    public static readonly SimVarItem MASTER_ALTERNATOR =
      new SimVarItem { Def = Definition.MasterAlternator, SimVarName = "GENERAL ENG MASTER ALTERNATOR:1", Unit = Units.Bool, CanSet = false };

    [SimVarDataRequest]
    [TouchPortalAction("MasterBattery", "Master Battery", "MSFS", "Toggle Master Battery", "Toggle Master Battery")]
    [TouchPortalState("MasterBattery", "text", "Master Battery Status", "")]
    public static readonly SimVarItem MASTER_BATTERY =
      new SimVarItem { Def = Definition.MasterBattery, SimVarName = "ELECTRICAL MASTER BATTERY", Unit = Units.Bool, CanSet = false };

    [TouchPortalAction("MasterBatteryAlternator", "Master Battery & Alternator", "MSFS", "Toggle Master Battery & Alternator", "Toggle Master Battery & Alternator")]
    public static object MASTER_BATTERY_ALTERNATOR { get; }

    [TouchPortalAction("AlternatorIndex", "Alternator - Specific", "MSFS", "Toggle Specific Alternator", "Toggle Altenator - {0}")]
    [TouchPortalActionChoice(new [] { "1", "2", "3", "4" }, "1")]
    public static object ALTERNATOR_INDEX { get; }

    #endregion

    #region Lights

    [TouchPortalAction("StrobeLights", "Toggle/On/Off/Set Strobe Lights", "MSFS", "Toggle/On/Off/Set Strobe Lights", "Strobe Lights - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off", "Set" }, "Toggle")]
    public static object STROBE_LIGHTS { get; }

    [TouchPortalAction("PanelLights", "Toggle/On/Off/Set Panel Lights", "MSFS", "Toggle/On/Off/Set Panel Lights", "Panel Lights - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off", "Set" }, "Toggle")]
    public static object PANEL_LIGHTS { get; }

    [TouchPortalAction("LandingLights", "Toggle/On/Off/Set Landing Lights", "MSFS", "Toggle/On/Off/Set Landing Lights", "Landing Lights - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off", "Set" }, "Toggle")]
    public static object LANDING_LIGHTS { get; }

    [TouchPortalAction("ToggleLights", "Toggle All/Specific Lights", "MSFS", "Toggle All/Specific Lights", "Toggle Lights - {0}")]
    [TouchPortalActionChoice(new [] { "All", "Beacon", "Taxi", "Logo", "Recognition", "Wing", "Nav", "Cabin" }, "All")]
    public static object ALL_LIGHTS { get; }

    [SimVarDataRequest]
    [TouchPortalState("LightBeaconOn", "text", "Light Beacon Switch Status", "")]
    public static readonly SimVarItem LightBeaconOn = new SimVarItem { Def = Definition.LightBeaconOn, SimVarName = "LIGHT BEACON", Unit = Units.Bool, CanSet = false };
    [SimVarDataRequest]
    [TouchPortalState("LightBrakeOn", "text", "Light Brake Switch or Light Status", "")]
    public static readonly SimVarItem LightBrakeOn = new SimVarItem { Def = Definition.LightBrakeOn, SimVarName = "LIGHT BRAKE ON", Unit = Units.Bool, CanSet = false };
    [SimVarDataRequest]
    [TouchPortalState("LightCabinOn", "text", "Light Cabin Switch Status", "")]
    public static readonly SimVarItem LightCabinOn = new SimVarItem { Def = Definition.LightCabinOn, SimVarName = "LIGHT CABIN", Unit = Units.Bool, CanSet = false };
    [SimVarDataRequest]
    [TouchPortalState("LightHeadOn", "text", "Light Head Switch or Light Status", "")]
    public static readonly SimVarItem LightHeadOn = new SimVarItem { Def = Definition.LightHeadOn, SimVarName = "LIGHT HEAD ON", Unit = Units.Bool, CanSet = false };
    [SimVarDataRequest]
    [TouchPortalState("LightLandingOn", "text", "Light Landing Switch Status", "")]
    public static readonly SimVarItem LightLandingOn = new SimVarItem { Def = Definition.LightLandingOn, SimVarName = "LIGHT LANDING", Unit = Units.Bool, CanSet = false };
    [SimVarDataRequest]
    [TouchPortalState("LightLogoOn", "text", "Light Logo Switch Status", "")]
    public static readonly SimVarItem LightLogoOn = new SimVarItem { Def = Definition.LightLogoOn, SimVarName = "LIGHT LOGO", Unit = Units.Bool, CanSet = false };
    [SimVarDataRequest]
    [TouchPortalState("LightNavOn", "text", "Light Nav Switch Status", "")]
    public static readonly SimVarItem LightNavOn = new SimVarItem { Def = Definition.LightNavOn, SimVarName = "LIGHT NAV", Unit = Units.Bool, CanSet = false };
    [SimVarDataRequest]
    [TouchPortalState("LightPanelOn", "text", "Light Panel Switch Status", "")]
    public static readonly SimVarItem LightPanelOn = new SimVarItem { Def = Definition.LightPanelOn, SimVarName = "LIGHT PANEL", Unit = Units.Bool, CanSet = false };
    [SimVarDataRequest]
    [TouchPortalState("LightRecognitionOn", "text", "Light Recognition Switch Status", "")]
    public static readonly SimVarItem LightRecognitionOn = new SimVarItem { Def = Definition.LightRecognitionOn, SimVarName = "LIGHT RECOGNITION", Unit = Units.Bool, CanSet = false };
    [SimVarDataRequest]
    [TouchPortalState("LightStrobeOn", "text", "Light Strobe Switch Status", "")]
    public static readonly SimVarItem LightStrobeOn = new SimVarItem { Def = Definition.LightStrobeOn, SimVarName = "LIGHT STROBE", Unit = Units.Bool, CanSet = false };
    [SimVarDataRequest]
    [TouchPortalState("LightTaxiOn", "text", "Light Taxi Switch Status", "")]
    public static readonly SimVarItem LightTaxiOn = new SimVarItem { Def = Definition.LightTaxiOn, SimVarName = "LIGHT TAXI", Unit = Units.Bool, CanSet = false };
    [SimVarDataRequest]
    [TouchPortalState("LightWingOn", "text", "Light Wing Switch Status", "")]
    public static readonly SimVarItem LightWingOn = new SimVarItem { Def = Definition.LightWingOn, SimVarName = "LIGHT WING", Unit = Units.Bool, CanSet = false };

    #endregion

  }

  [SimNotificationGroup(Groups.Electrical)]
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
