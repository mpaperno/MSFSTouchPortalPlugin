using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Enums;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.InstrumentsSystems {
  [SimVarDataRequestGroup]
  [TouchPortalCategory("Environment", "MSFS - Environment")]
  internal class EnvironmentMapping {
    #region Anti-Ice

    [TouchPortalAction("AntiIce", "Anti-Ice", "MSFS", "Toggle/On/Off Anti Ice", "Anti Ice - {0}")]
    [TouchPortalActionChoice(new string[] { "Toggle", "On", "Off", "Set" }, "Toggle")]
    public object ANTI_ICE { get; }

    [TouchPortalAction("AntiIceEng1", "Anti-Ice Engine 1", "MSFS", "Toggle/On/Off Anti Ice Engine 1", "Anti Ice Engine 1 - {0}")]
    [TouchPortalActionChoice(new string[] { "Toggle","Set" }, "Toggle")]
    public object ANTI_ICE_ENG1 { get; }

    [TouchPortalAction("AntiIceEng2", "Anti-Ice Engine 2", "MSFS", "Toggle/On/Off Anti Ice Engine 2", "Anti Ice Engine 2 - {0}")]
    [TouchPortalActionChoice(new string[] { "Toggle","Set" }, "Toggle")]
    public object ANTI_ICE_ENG2 { get; }

    [TouchPortalAction("AntiIceEng3", "Anti-Ice Engine 3", "MSFS", "Toggle/On/Off Anti Ice Engine 3", "Anti Ice Engine 3 - {0}")]
    [TouchPortalActionChoice(new string[] { "Toggle", "Set" }, "Toggle")]
    public object ANTI_ICE_ENG3 { get; }

    [TouchPortalAction("AntiIceEng4", "Anti-Ice Engine 4", "MSFS", "Toggle/On/Off Anti Ice Engine 4", "Anti Ice Engine 4 - {0}")]
    [TouchPortalActionChoice(new string[] { "Toggle", "Set" }, "Toggle")]
    public object ANTI_ICE_ENG4 { get; }

    [TouchPortalAction("StructuralDeIce", "Structural De-ice", "MSFS", "Toggle Structural DeIce", "Toggle Structural DeIce")]
    public object STRUCTURAL_DEICE { get; }

    [TouchPortalAction("PropellerDeIce", "Propeller De-ice", "MSFS", "Toggle Propeller DeIce", "Toggle Propeller DeIce")]
    public object PROPELLER_DEICE { get; }

    #endregion

    #region Pitot Heat

    [SimVarDataRequest]
    [TouchPortalAction("PitotHeat", "Pitot Heat", "MSFS", "Toggle/On/Off Pitot Heat", "Pitot Heat - {0}")]
    [TouchPortalActionChoice(new string[] { "Toggle", "On", "Off", "Set" }, "Toggle")]
    [TouchPortalState("PitotHeat", "text", "Pitot Heat Status", "")]
    public static SimVarItem PITOT_HEAT =
      new SimVarItem() { def = Definition.PitotHeat, req = Request.PitotHeat, SimVarName = "", Unit = Units.Bool, CanSet = false };

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
    [TouchPortalActionMapping("AntiIceEng1", "Toggle")]
    ANTI_ICE_TOGGLE_ENG1,
    [SimActionEvent]
    [TouchPortalActionMapping("AntiIceEng1", "Set")]
    ANTI_ICE_SET_ENG1,

    // Anti-Ice Eng 2
    [SimActionEvent]
    [TouchPortalActionMapping("AntiIceEng2", "Toggle")]
    ANTI_ICE_TOGGLE_ENG2,
    [SimActionEvent]
    [TouchPortalActionMapping("AntiIceEng2", "Set")]
    ANTI_ICE_SET_ENG2,

    // Anti-Ice Eng 3
    [SimActionEvent]
    [TouchPortalActionMapping("AntiIceEng3", "Toggle")]
    ANTI_ICE_TOGGLE_ENG3,
    [SimActionEvent]
    [TouchPortalActionMapping("AntiIceEng3", "Set")]
    ANTI_ICE_SET_ENG3,

    // Anti-Ice Eng 4
    [SimActionEvent]
    [TouchPortalActionMapping("AntiIceEng4", "Toggle")]
    ANTI_ICE_TOGGLE_ENG4,
    [SimActionEvent]
    [TouchPortalActionMapping("AntiIceEng4", "Set")]
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
