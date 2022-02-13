using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Enums;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.InstrumentsSystems
{
  [SimVarDataRequestGroup]
  [SimNotificationGroup(Groups.Electrical)]
  [TouchPortalCategory("Electrical", "MSFS - Electrical")]
  internal static class ElectricalMapping {
    #region Avionics

    [SimVarDataRequest]
    [TouchPortalAction("AvionicsMasterSwitch", "Avionics Master", "MSFS", "Toggle Avionics Master", "Toggle Avionics Master")]
    [TouchPortalActionMapping("TOGGLE_AVIONICS_MASTER")]
    [TouchPortalState("AvionicsMasterSwitch", "text", "Avionics Master Switch", "")]
    public static readonly SimVarItem TOGGLE_AVIONICS_MASTER = new SimVarItem { Def = Definition.AvionicsMasterSwitch, SimVarName = "AVIONICS MASTER SWITCH", Unit = Units.Bool, CanSet = false };

    #endregion

    #region Alternator & Battery

    [SimVarDataRequest]
    [TouchPortalAction("MasterAlternator", "Master Alternator", "MSFS", "Toggle Master Alternator", "Toggle Master Alternator")]
    [TouchPortalActionMapping("TOGGLE_MASTER_ALTERNATOR")]
    [TouchPortalState("MasterAlternator", "text", "Master Alternator Status", "")]
    public static readonly SimVarItem MASTER_ALTERNATOR =
      new SimVarItem { Def = Definition.MasterAlternator, SimVarName = "GENERAL ENG MASTER ALTERNATOR:1", Unit = Units.Bool, CanSet = false };

    [SimVarDataRequest]
    [TouchPortalAction("MasterBattery", "Master Battery", "MSFS", "Toggle Master Battery", "Toggle Master Battery")]
    [TouchPortalActionMapping("TOGGLE_MASTER_BATTERY")]
    [TouchPortalState("MasterBattery", "text", "Master Battery Status", "")]
    public static readonly SimVarItem MASTER_BATTERY =
      new SimVarItem { Def = Definition.MasterBattery, SimVarName = "ELECTRICAL MASTER BATTERY", Unit = Units.Bool, CanSet = false };

    [TouchPortalAction("MasterBatteryAlternator", "Master Battery & Alternator", "MSFS", "Toggle Master Battery & Alternator", "Toggle Master Battery & Alternator")]
    [TouchPortalActionMapping("TOGGLE_MASTER_BATTERY_ALTERNATOR")]
    public static object MASTER_BATTERY_ALTERNATOR { get; }

    [TouchPortalAction("AlternatorIndex", "Alternator - Specific", "MSFS", "Toggle Specific Alternator", "Toggle Altenator - {0}")]
    [TouchPortalActionChoice(new[] { "1", "2", "3", "4" })]
    [TouchPortalActionMapping("TOGGLE_ALTERNATOR1", "1")]
    [TouchPortalActionMapping("TOGGLE_ALTERNATOR2", "2")]
    [TouchPortalActionMapping("TOGGLE_ALTERNATOR3", "3")]
    [TouchPortalActionMapping("TOGGLE_ALTERNATOR4", "4")]
    public static object ALTERNATOR_INDEX { get; }

    #endregion

    #region Lights

    [TouchPortalAction("StrobeLights", "Toggle/On/Off Strobe Lights", "MSFS", "Toggle/On/Off Strobe Lights", "Strobe Lights - {0}")]
    [TouchPortalActionChoice(new[] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("STROBES_TOGGLE", "Toggle")]
    [TouchPortalActionMapping("STROBES_ON", "On")]
    [TouchPortalActionMapping("STROBES_OFF", "Off")]
    public static object STROBE_LIGHTS { get; }

    [TouchPortalAction("PanelLights", "Toggle/On/Off Panel Lights", "MSFS", "Toggle/On/Off Panel Lights", "Panel Lights - {0}")]
    [TouchPortalActionChoice(new[] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("PANEL_LIGHTS_TOGGLE", "Toggle")]
    [TouchPortalActionMapping("PANEL_LIGHTS_ON", "On")]
    [TouchPortalActionMapping("PANEL_LIGHTS_OFF", "Off")]
    public static object PANEL_LIGHTS { get; }

    [TouchPortalAction("LandingLights", "Toggle/On/Off Landing Lights", "MSFS", "Toggle/On/Off Landing Lights", "Landing Lights - {0}")]
    [TouchPortalActionChoice(new[] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("LANDING_LIGHTS_TOGGLE", "Toggle")]
    [TouchPortalActionMapping("LANDING_LIGHTS_ON", "On")]
    [TouchPortalActionMapping("LANDING_LIGHTS_OFF", "Off")]
    public static object LANDING_LIGHTS { get; }

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
}
