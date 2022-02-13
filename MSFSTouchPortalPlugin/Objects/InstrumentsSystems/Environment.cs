using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Enums;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.InstrumentsSystems
{
  [SimVarDataRequestGroup]
  [SimNotificationGroup(Groups.Environment)]
  [TouchPortalCategory("Environment", "MSFS - Environment")]
  internal static class EnvironmentMapping {
    #region Anti-Ice

    [TouchPortalAction("AntiIce", "Anti-Ice", "MSFS", "Toggle/On/Off Anti Ice", "Anti Ice - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("ANTI_ICE_TOGGLE", "Toggle")]
    [TouchPortalActionMapping("ANTI_ICE_ON", "On")]
    [TouchPortalActionMapping("ANTI_ICE_OFF", "Off")]
    public static object AntiIce { get; }

    [TouchPortalAction("AntiIceEng", "Anti-Ice Engine", "MSFS", "Toggle Anti Ice Engine", "Anti Ice Engine {0} Toggle")]
    [TouchPortalActionChoice(new[] { "1", "2", "3", "4" })]
    [TouchPortalActionMapping("ANTI_ICE_TOGGLE_ENG1", "1")]
    [TouchPortalActionMapping("ANTI_ICE_TOGGLE_ENG2", "2")]
    [TouchPortalActionMapping("ANTI_ICE_TOGGLE_ENG3", "3")]
    [TouchPortalActionMapping("ANTI_ICE_TOGGLE_ENG4", "4")]
    public static object AntiIceEng { get; }

    [TouchPortalAction("AntiIceEngSet", "Anti-Ice Engine Set", "MSFS", "Set On/Off Anti Ice Engine", "Anti Ice Engine {0} - {1}")]
    [TouchPortalActionChoice(new[] { "1", "2", "3", "4" })]
    [TouchPortalActionSwitch()]
    [TouchPortalActionMapping("ANTI_ICE_SET_ENG1", "1")]
    [TouchPortalActionMapping("ANTI_ICE_SET_ENG2", "2")]
    [TouchPortalActionMapping("ANTI_ICE_SET_ENG3", "3")]
    [TouchPortalActionMapping("ANTI_ICE_SET_ENG4", "4")]
    public static object ANTI_ICE_ENGINE { get; }

    [SimVarDataRequest]
    [TouchPortalState("AntiIceEng1", "text", "Anti-Ice Engine 1", "")]
    public static readonly SimVarItem AntiIceEng1 = new SimVarItem { Def = Definition.AntiIceEng1, SimVarName = "GENERAL ENG ANTI ICE POSITION:1", Unit = Units.Bool, CanSet = false };

    [SimVarDataRequest]
    [TouchPortalState("AntiIceEng2", "text", "Anti-Ice Engine 2", "")]
    public static readonly SimVarItem AntiIceEng2 = new SimVarItem { Def = Definition.AntiIceEng2, SimVarName = "GENERAL ENG ANTI ICE POSITION:2", Unit = Units.Bool, CanSet = false };

    [SimVarDataRequest]
    [TouchPortalState("AntiIceEng3", "text", "Anti-Ice Engine 3", "")]
    public static readonly SimVarItem AntiIceEng3 = new SimVarItem { Def = Definition.AntiIceEng3, SimVarName = "GENERAL ENG ANTI ICE POSITION:3", Unit = Units.Bool, CanSet = false };

    [SimVarDataRequest]
    [TouchPortalState("AntiIceEng4", "text", "Anti-Ice Engine 4", "")]
    public static readonly SimVarItem AntiIceEng4 = new SimVarItem { Def = Definition.AntiIceEng4, SimVarName = "GENERAL ENG ANTI ICE POSITION:4", Unit = Units.Bool, CanSet = false };

    [TouchPortalAction("StructuralDeIce", "Structural De-ice", "MSFS", "Toggle Structural DeIce", "Toggle Structural DeIce")]
    [TouchPortalActionMapping("TOGGLE_STRUCTURAL_DEICE")]
    public static object STRUCTURAL_DEICE { get; }

    [TouchPortalAction("PropellerDeIce", "Propeller De-ice", "MSFS", "Toggle Propeller DeIce", "Toggle Propeller DeIce")]
    [TouchPortalActionMapping("TOGGLE_PROPELLER_DEICE")]
    public static object PROPELLER_DEICE { get; }

    [SimVarDataRequest]
    [TouchPortalState("AntiIcePanelSwitch", "text", "Panel Anti-Ice Switch", "")]
    public static readonly SimVarItem AntiIcePanelSwitch = new SimVarItem { Def = Definition.AntiIcePanelSwitch, SimVarName = "PANEL ANTI ICE SWITCH", Unit = Units.Bool, CanSet = false };

    [SimVarDataRequest]
    [TouchPortalState("AntiIceStructuralSwitch", "text", "Structural Deice Switch", "")]
    public static readonly SimVarItem AntiIceStructuralSwitch = new SimVarItem { Def = Definition.AntiIceStructuralSwitch, SimVarName = "STRUCTURAL DEICE SWITCH", Unit = Units.Bool, CanSet = false };

    [SimVarDataRequest]
    [TouchPortalState("AntiIceWindshieldSwitch", "text", "Windshield Deice Switch", "")]
    public static readonly SimVarItem AntiIceWindshieldSwitch = new SimVarItem { Def = Definition.AntiIceWindshieldSwitch, SimVarName = "WINDSHIELD DEICE SWITCH", Unit = Units.Bool, CanSet = false };

    [SimVarDataRequest]
    [TouchPortalState("AntiIcePropeller1Switch", "text", "Propeller 1 Deice Switch", "")]
    public static readonly SimVarItem AntiIcePropeller1Switch = new SimVarItem { Def = Definition.AntiIcePropeller1Switch, SimVarName = "PROP DEICE SWITCH:1", Unit = Units.Bool, CanSet = false };
    [SimVarDataRequest]
    [TouchPortalState("AntiIcePropeller2Switch", "text", "Propeller 2 Deice Switch", "")]
    public static readonly SimVarItem AntiIcePropeller2Switch = new SimVarItem { Def = Definition.AntiIcePropeller2Switch, SimVarName = "PROP DEICE SWITCH:2", Unit = Units.Bool, CanSet = false };
    [SimVarDataRequest]
    [TouchPortalState("AntiIcePropeller3Switch", "text", "Propeller 3 Deice Switch", "")]
    public static readonly SimVarItem AntiIcePropeller3Switch = new SimVarItem { Def = Definition.AntiIcePropeller3Switch, SimVarName = "PROP DEICE SWITCH:3", Unit = Units.Bool, CanSet = false };
    [SimVarDataRequest]
    [TouchPortalState("AntiIcePropeller4Switch", "text", "Propeller 4 Deice Switch", "")]
    public static readonly SimVarItem AntiIcePropeller4Switch = new SimVarItem { Def = Definition.AntiIcePropeller4Switch, SimVarName = "PROP DEICE SWITCH:4", Unit = Units.Bool, CanSet = false };

    #endregion

    #region Pitot Heat

    [SimVarDataRequest]
    [TouchPortalAction("PitotHeat", "Pitot Heat", "MSFS", "Toggle/On/Off Pitot Heat", "Pitot Heat - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("PITOT_HEAT_TOGGLE", "Toggle")]
    [TouchPortalActionMapping("PITOT_HEAT_ON", "On")]
    [TouchPortalActionMapping("PITOT_HEAT_OFF", "Off")]
    [TouchPortalState("PitotHeat", "text", "Pitot Heat Status", "")]
    public static readonly SimVarItem PITOT_HEAT = new SimVarItem { Def = Definition.PitotHeat, SimVarName = "PITOT HEAT", Unit = Units.Bool, CanSet = false };

    [SimVarDataRequest]
    [TouchPortalState("PitotHeatSwitch1", "text", "Pitot Heat Switch 1 State (0=Off; 1=On; 2=Auto)", "")]
    public static readonly SimVarItem PitotHeatSwitch1 = new SimVarItem { Def = Definition.PitotHeatSwitch1, SimVarName = "PITOT HEAT SWITCH:1", Unit = Units.Enum, CanSet = false };

    [SimVarDataRequest]
    [TouchPortalState("PitotHeatSwitch2", "text", "Pitot Heat Switch 2 State (0=Off; 1=On; 2=Auto)", "")]
    public static readonly SimVarItem PitotHeatSwitch2 = new SimVarItem { Def = Definition.PitotHeatSwitch2, SimVarName = "PITOT HEAT SWITCH:2", Unit = Units.Enum, CanSet = false };

    [SimVarDataRequest]
    [TouchPortalState("PitotHeatSwitch3", "text", "Pitot Heat Switch 3 State (0=Off; 1=On; 2=Auto)", "")]
    public static readonly SimVarItem PitotHeatSwitch3 = new SimVarItem { Def = Definition.PitotHeatSwitch3, SimVarName = "PITOT HEAT SWITCH:3", Unit = Units.Enum, CanSet = false };

    [SimVarDataRequest]
    [TouchPortalState("PitotHeatSwitch4", "text", "Pitot Heat Switch 4 State (0=Off; 1=On; 2=Auto)", "")]
    public static readonly SimVarItem PitotHeatSwitch4 = new SimVarItem { Def = Definition.PitotHeatSwitch4, SimVarName = "PITOT HEAT SWITCH:4", Unit = Units.Enum, CanSet = false };

    #endregion
  }
}
