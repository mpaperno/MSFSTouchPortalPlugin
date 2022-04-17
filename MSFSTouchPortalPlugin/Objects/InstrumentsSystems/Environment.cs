using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Enums;

namespace MSFSTouchPortalPlugin.Objects.InstrumentsSystems
{
  [TouchPortalCategory(Groups.Environment)]
  internal static class EnvironmentMapping {
    #region Anti-Ice

    [TouchPortalAction("AntiIce", "Anti-Ice", "MSFS", "Toggle/On/Off Anti Ice", "Anti Ice - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("ANTI_ICE_TOGGLE", "Toggle")]
    [TouchPortalActionMapping("ANTI_ICE_ON", "On")]
    [TouchPortalActionMapping("ANTI_ICE_OFF", "Off")]
    public static readonly object AntiIce;

    [TouchPortalAction("AntiIceEng", "Anti-Ice Engine", "MSFS", "Toggle Anti Ice Engine", "Anti Ice Engine {0} Toggle")]
    [TouchPortalActionChoice(new[] { "1", "2", "3", "4" })]
    [TouchPortalActionMapping("ANTI_ICE_TOGGLE_ENG1", "1")]
    [TouchPortalActionMapping("ANTI_ICE_TOGGLE_ENG2", "2")]
    [TouchPortalActionMapping("ANTI_ICE_TOGGLE_ENG3", "3")]
    [TouchPortalActionMapping("ANTI_ICE_TOGGLE_ENG4", "4")]
    public static readonly object AntiIceEng;

    [TouchPortalAction("AntiIceEngSet", "Anti-Ice Engine Set", "MSFS", "Set On/Off Anti Ice Engine", "Anti Ice Engine {0} - {1}")]
    [TouchPortalActionChoice(new[] { "1", "2", "3", "4" })]
    [TouchPortalActionSwitch()]
    [TouchPortalActionMapping("ANTI_ICE_SET_ENG1", "1")]
    [TouchPortalActionMapping("ANTI_ICE_SET_ENG2", "2")]
    [TouchPortalActionMapping("ANTI_ICE_SET_ENG3", "3")]
    [TouchPortalActionMapping("ANTI_ICE_SET_ENG4", "4")]
    public static readonly object ANTI_ICE_ENGINE;

    [TouchPortalAction("StructuralDeIce", "Structural De-ice", "MSFS", "Toggle Structural DeIce", "Toggle Structural DeIce")]
    [TouchPortalActionMapping("TOGGLE_STRUCTURAL_DEICE")]
    public static readonly object STRUCTURAL_DEICE;

    [TouchPortalAction("PropellerDeIce", "Propeller De-ice", "MSFS", "Toggle Propeller DeIce", "Toggle Propeller DeIce")]
    [TouchPortalActionMapping("TOGGLE_PROPELLER_DEICE")]
    public static readonly object PROPELLER_DEICE;

    #endregion

    #region Pitot Heat

    [TouchPortalAction("PitotHeat", "Pitot Heat", "MSFS", "Toggle/On/Off Pitot Heat", "Pitot Heat - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("PITOT_HEAT_TOGGLE", "Toggle")]
    [TouchPortalActionMapping("PITOT_HEAT_ON", "On")]
    [TouchPortalActionMapping("PITOT_HEAT_OFF", "Off")]
    public static readonly object PITOT_HEAT;

    #endregion
  }
}
