using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Enums;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.Failures {
  [SimVarDataRequestGroup]
  [TouchPortalCategory("Failures", "MSFS - Failures")]
  internal static class FailuresMapping {

    [TouchPortalAction("Failures", "Failures", "MSFS", "Toggle Failures", "Toggle Failures - {0}")]
    [TouchPortalActionChoice(new [] { "Electrical", "Vacuum", "Pitot", "Static Port", "Hydraulic", "Total Brake", "Left Brake", "Right Brake", "Engine 1", "Engine 2", "Engine 3", "Engine 4" }, "Electrical")]
    public static object FAILURES { get; }
  }

  [SimNotificationGroup(Groups.Failures)]
  [TouchPortalCategoryMapping("Failures")]
  internal enum Failures {
    // Placeholder to offset each enum for SimConnect
    Init = 9000,

    [SimActionEvent]
    [TouchPortalActionMapping("Failures", "Vacuum")]
    TOGGLE_VACUUM_FAILURE,
    [SimActionEvent]
    [TouchPortalActionMapping("Failures", "Electrical")]
    TOGGLE_ELECTRICAL_FAILURE,
    [SimActionEvent]
    [TouchPortalActionMapping("Failures", "Pitot")]
    TOGGLE_PITOT_BLOCKAGE,
    [SimActionEvent]
    [TouchPortalActionMapping("Failures", "Static Port")]
    TOGGLE_STATIC_PORT_BLOCKAGE,
    [SimActionEvent]
    [TouchPortalActionMapping("Failures", "Hydraulic")]
    TOGGLE_HYDRAULIC_FAILURE,
    [SimActionEvent]
    [TouchPortalActionMapping("Failures", "Total Brake")]
    TOGGLE_TOTAL_BRAKE_FAILURE,
    [SimActionEvent]
    [TouchPortalActionMapping("Failures", "Left Brake")]
    TOGGLE_LEFT_BRAKE_FAILURE,
    [SimActionEvent]
    [TouchPortalActionMapping("Failures", "Right Brake")]
    TOGGLE_RIGHT_BRAKE_FAILURE,
    [SimActionEvent]
    [TouchPortalActionMapping("Failures", "Engine 1")]
    TOGGLE_ENGINE1_FAILURE,
    [SimActionEvent]
    [TouchPortalActionMapping("Failures", "Engine 2")]
    TOGGLE_ENGINE2_FAILURE,
    [SimActionEvent]
    [TouchPortalActionMapping("Failures", "Engine 3")]
    TOGGLE_ENGINE3_FAILURE,
    [SimActionEvent]
    [TouchPortalActionMapping("Failures", "Engine 4")]
    TOGGLE_ENGINE4_FAILURE,
  }
}
