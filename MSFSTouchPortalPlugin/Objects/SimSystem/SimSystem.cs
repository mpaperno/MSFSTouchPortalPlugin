using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Enums;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.SimSystem {
  [SimVarDataRequestGroup]
  [TouchPortalCategory("SimSystem", "MSFS - System")]
  internal class SimSystemMapping {
    //[TouchPortalAction("Pause", "Pause", "MSFS", "Toggle/On/Off Pause", "Pause - {0}")]
    //[TouchPortalActionChoice(new string[] { "Toggle", "On", "Off" }, "Toggle")]
    //public object PAUSE { get; }

    [SimVarDataRequest]
    [TouchPortalAction("SimulationRate", "Simulation Rate", "MSFS", "Simulation Rate", "Rate {0}")]
    [TouchPortalActionChoice(new string[] { "Increase", "Decrease" }, "Decrease")]
    [TouchPortalState("SimulationRate", "text", "The current simulation rate", "")]
    public static SimVarItem SimulationRate =
      new SimVarItem() { def = Definition.SimulationRate, req = Request.SimulationRate, SimVarName = "SIMULATION RATE", Unit = Units.number, CanSet = false };
  }

  [SimNotificationGroup(Groups.SimSystem)]
  [TouchPortalCategoryMapping("SimSystem")]
  internal enum SimSystem {
    // Placeholder to offset each enum for SimConnect
    Init = 3000,

    #region SimRate

    [SimActionEvent]
    [TouchPortalActionMapping("SimulationRate", "Increase")]
    SIM_RATE_INCR,
    [SimActionEvent]
    [TouchPortalActionMapping("SimulationRate", "Decrease")]
    SIM_RATE_DECR

    #endregion
  }
}
