using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Enums;

namespace MSFSTouchPortalPlugin.Objects.SimSystem
{
  [SimNotificationGroup(Groups.SimSystem)]
  [TouchPortalCategory("SimSystem", "MSFS - System")]
  internal static class SimSystemMapping {
    [TouchPortalAction("SimulationRate", "Simulation Rate", "MSFS", "Simulation Rate", "Rate {0}", true)]
    [TouchPortalActionChoice(new [] { "Increase", "Decrease" }, "Decrease")]
    [TouchPortalActionMapping("SIM_RATE_INCR", "Increase")]
    [TouchPortalActionMapping("SIM_RATE_DECR", "Decrease")]
    public static readonly object SimulationRate;

    [TouchPortalAction("SelectedParameter", "Change Selected Value (+/-)", "MSFS", "Selected Value", "Value {0}", true)]
    [TouchPortalActionChoice(new[] { "Increase", "Decrease" }, "Decrease")]
    [TouchPortalActionMapping("PLUS", "Increase")]
    [TouchPortalActionMapping("MINUS", "Decrease")]
    public static readonly object SELECTED_PARAMETER_CHANGE;

  }
}
