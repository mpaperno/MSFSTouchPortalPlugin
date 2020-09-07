using MSFSTouchPortalPlugin.Attributes;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.InstrumentsSystems {
  [TouchPortalCategory("Engine", "MFS - Engine")]
  internal class EngineMapping {

    #region Ignition

    [TouchPortalAction("MasterIgnition", "Master Ignition Switch", "MSFS", "Toggle Master Ignition Switch", "Toggle Master Ignition Switch")]
    public object MASTER_IGNITION { get; }

    #endregion
  }

  [SimNotificationGroup(SimConnectWrapper.Groups.Engine)]
  [TouchPortalCategoryMapping("Engine")]
  internal enum Engine {
    // Placeholder to offset each enum for SimConnect
    Init = 4000,

    #region Ignition

    [SimActionEvent]
    [TouchPortalActionMapping("MasterIgnition")]
    TOGGLE_MASTER_IGNITION_SWITCH

    #endregion
  }
}
