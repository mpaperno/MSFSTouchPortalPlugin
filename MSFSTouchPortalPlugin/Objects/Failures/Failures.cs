using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Enums;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.Failures 
{
  [SimVarDataRequestGroup]
  [SimNotificationGroup(Groups.Failures)]
  [TouchPortalCategory("Failures", "MSFS - Failures")]
  internal static class FailuresMapping {

    [TouchPortalAction("Failures", "Failures", "MSFS", "Toggle Failures", "Toggle Failures - {0}")]
    [TouchPortalActionChoice(new [] { "Electrical", "Vacuum", "Pitot", "Static Port", "Hydraulic", "Total Brake", "Left Brake", "Right Brake", "Engine 1", "Engine 2", "Engine 3", "Engine 4" })]
    [TouchPortalActionMapping("TOGGLE_VACUUM_FAILURE", "Vacuum")]
    [TouchPortalActionMapping("TOGGLE_ELECTRICAL_FAILURE", "Electrical")]
    [TouchPortalActionMapping("TOGGLE_PITOT_BLOCKAGE", "Pitot")]
    [TouchPortalActionMapping("TOGGLE_STATIC_PORT_BLOCKAGE", "Static Port")]
    [TouchPortalActionMapping("TOGGLE_HYDRAULIC_FAILURE", "Hydraulic")]
    [TouchPortalActionMapping("TOGGLE_TOTAL_BRAKE_FAILURE", "Total Brake")]
    [TouchPortalActionMapping("TOGGLE_LEFT_BRAKE_FAILURE", "Left Brake")]
    [TouchPortalActionMapping("TOGGLE_RIGHT_BRAKE_FAILURE", "Right Brake")]
    [TouchPortalActionMapping("TOGGLE_ENGINE1_FAILURE", "Engine 1")]
    [TouchPortalActionMapping("TOGGLE_ENGINE2_FAILURE", "Engine 2")]
    [TouchPortalActionMapping("TOGGLE_ENGINE3_FAILURE", "Engine 3")]
    [TouchPortalActionMapping("TOGGLE_ENGINE4_FAILURE", "Engine 4")]
    public static object FAILURES { get; }
  }
}
