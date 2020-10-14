using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Enums;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.InstrumentsSystems {
  [SimVarDataRequestGroup]
  [TouchPortalCategory("Environment", "MSFS - Environment")]
  internal static class EnvironmentMapping {
    #region Anti-Ice

    [TouchPortalAction("AntiIce", "Anti-Ice", "MSFS", "Toggle/On/Off Anti Ice", "Anti Ice - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off", "Set" }, "Toggle")]
    public static object ANTI_ICE { get; }

    [TouchPortalAction("AntiIceEng", "Anti-Ice Engine", "MSFS", "Toggle/On/Off Anti Ice Engine", "Anti Ice Engine {0} - {1}")]
    [TouchPortalActionChoice(new[] { "1", "2", "3", "4" }, "1")]
    [TouchPortalActionChoice(new [] { "Toggle","Set" }, "Toggle")]
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
    public static object STRUCTURAL_DEICE { get; }

    [TouchPortalAction("PropellerDeIce", "Propeller De-ice", "MSFS", "Toggle Propeller DeIce", "Toggle Propeller DeIce")]
    public static object PROPELLER_DEICE { get; }

    #endregion

    #region Pitot Heat

    [SimVarDataRequest]
    [TouchPortalAction("PitotHeat", "Pitot Heat", "MSFS", "Toggle/On/Off Pitot Heat", "Pitot Heat - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off", "Set" }, "Toggle")]
    [TouchPortalState("PitotHeat", "text", "Pitot Heat Status", "")]
    public static readonly SimVarItem PITOT_HEAT = new SimVarItem { Def = Definition.PitotHeat, SimVarName = "PITOT HEAT", Unit = Units.Bool, CanSet = false };

    #endregion
  }

  [SimNotificationGroup(Groups.Environment)]
  [TouchPortalCategoryMapping("Environment")]
  internal enum Environment {
    // Placeholder to offset each enum for SimConnect
    Init = 5000,

    #region Anti-Ice

    // Anti-Ice
    [SimActionEvent]
    [TouchPortalActionMapping("AntiIce", "Toggle")]
    ANTI_ICE_TOGGLE,
    [SimActionEvent]
    [TouchPortalActionMapping("AntiIce", "On")]
    ANTI_ICE_ON,
    [SimActionEvent]
    [TouchPortalActionMapping("AntiIce", "Off")]
    ANTI_ICE_OFF,
    [SimActionEvent]
    [TouchPortalActionMapping("AntiIce", "Set")]
    ANTI_ICE_SET,

    // Anti-Ice Eng 1
    [SimActionEvent]
    [TouchPortalActionMapping("AntiIceEng", new[] { "1", "Toggle" })]
    ANTI_ICE_TOGGLE_ENG1,
    [SimActionEvent]
    [TouchPortalActionMapping("AntiIceEng", new[] { "1", "Set" })]
    ANTI_ICE_SET_ENG1,

    // Anti-Ice Eng 2
    [SimActionEvent]
    [TouchPortalActionMapping("AntiIceEng", new[] { "2", "Toggle" })]
    ANTI_ICE_TOGGLE_ENG2,
    [SimActionEvent]
    [TouchPortalActionMapping("AntiIceEng", new[] { "2", "Set" })]
    ANTI_ICE_SET_ENG2,

    // Anti-Ice Eng 3
    [SimActionEvent]
    [TouchPortalActionMapping("AntiIceEng", new[] { "3", "Toggle" })]
    ANTI_ICE_TOGGLE_ENG3,
    [SimActionEvent]
    [TouchPortalActionMapping("AntiIceEng", new[] { "3", "Set" })]
    ANTI_ICE_SET_ENG3,

    // Anti-Ice Eng 4
    [SimActionEvent]
    [TouchPortalActionMapping("AntiIceEng", new[] { "4", "Toggle" })]
    ANTI_ICE_TOGGLE_ENG4,
    [SimActionEvent]
    [TouchPortalActionMapping("AntiIceEng", new[] { "4", "Set" })]
    ANTI_ICE_SET_ENG4,

    [SimActionEvent]
    [TouchPortalActionMapping("StructuralDeIce")]
    TOGGLE_STRUCTURAL_DEICE,

    [SimActionEvent]
    [TouchPortalActionMapping("PropellerDeIce")]
    TOGGLE_PROPELLER_DEICE,

    #endregion

    #region Pitot Heat
    // Pitot Heat
    [SimActionEvent]
    [TouchPortalActionMapping("PitotHeat", "Toggle")]
    PITOT_HEAT_TOGGLE,
    [SimActionEvent]
    [TouchPortalActionMapping("PitotHeat", "On")]
    PITOT_HEAT_ON,
    [SimActionEvent]
    [TouchPortalActionMapping("PitotHeat", "Off")]
    PITOT_HEAT_OFF,
    [SimActionEvent]
    [TouchPortalActionMapping("PitotHeat", "Set")]
    PITOT_HEAT_SET,
    #endregion
  }
}
