using MSFSTouchPortalPlugin.Attributes;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.InstrumentsSystems {
  [TouchPortalCategory("Engine", "MSFS - Engine")]
  internal class EngineMapping {

    #region Ignition

    [TouchPortalAction("MasterIgnition", "Master Ignition Switch", "MSFS", "Toggle Master Ignition Switch", "Toggle Master Ignition Switch")]
    public object MASTER_IGNITION { get; }

    [TouchPortalAction("EngineAuto", "Engine Auto Start/Shutdown", "MSFS", "Start/Shutdown Engine", "Engine - {0}")]
    [TouchPortalActionChoice(new string[] { "Start", "Shutdown" }, "Start")]
    public object ENGINE_AUTO { get; }

    #endregion
  }

  [SimNotificationGroup(SimConnectWrapper.Groups.Engine)]
  [TouchPortalCategoryMapping("Engine")]
  internal enum Engine {
    // Placeholder to offset each enum for SimConnect
    Init = 4000,

    // TODO: Magnetos

    #region Ignition

    [SimActionEvent]
    [TouchPortalActionMapping("MasterIgnition")]
    TOGGLE_MASTER_IGNITION_SWITCH,

    [SimActionEvent]
    [TouchPortalActionMapping("EngineAuto", "Start")]
    ENGINE_AUTO_START,

    [SimActionEvent]
    [TouchPortalActionMapping("EngineAuto", "Shutdown")]
    ENGINE_AUTO_SHUTDOWN

    #endregion
  }
}
