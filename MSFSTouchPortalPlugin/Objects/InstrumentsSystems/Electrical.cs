using MSFSTouchPortalPlugin.Attributes;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.InstrumentsSystems {
  [TouchPortalCategory("Electrical", "MFS - Electrical")]
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
  }
}
