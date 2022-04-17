using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Enums;
using MSFSTouchPortalPlugin.Types;

namespace MSFSTouchPortalPlugin.Objects.SimSystem
{
  [TouchPortalCategory(Groups.SimSystem)]
  internal static class SimSystemMapping
  {
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


    // Event

    public static readonly TouchPortalEvent SimSystemEvent = new("SimSystemEvent", "Simulator System Event", "On Simulator Event $val",
      new System.Enum[] {
       EventIds.SimConnecting,
       EventIds.SimConnected,
       EventIds.SimDisconnected,
       EventIds.SimTimedOut,
       EventIds.SimError,
       EventIds.PluginError,
       EventIds.PluginInfo,
       EventIds.Paused,
       EventIds.Unpaused,
       EventIds.Pause,
       EventIds.SimStart,
       EventIds.SimStop,
       EventIds.Sim,
       EventIds.AircraftLoaded,
       EventIds.Crashed,
       EventIds.CrashReset,
       EventIds.FlightLoaded,
       EventIds.FlightSaved,
       EventIds.FlightPlanActivated,
       EventIds.FlightPlanDeactivated,
       EventIds.PositionChanged,
       EventIds.Sound,
       EventIds.ViewCockpit,
       EventIds.ViewExternal,
    });
  }
}
