using MSFSTouchPortalPlugin.Attributes;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.AutoPilot {

  [TouchPortalCategory("AutoPilot", "MSFS - AutoPilot")]
  internal class AutoPilotMapping {
    [TouchPortalAction("AutoPilotMaster", "AutoPilot", "MSFS", "Toggle/On/Off Auto Pilot", "Auto Pilot Master - {0}")]
    [TouchPortalActionChoice(new string[] {"Toggle", "On", "Off"}, "Toggle")]
    public object AP_MASTER { get; }

    [TouchPortalAction("AutoPilotAttitude", "Attitude Hold", "MSFS", "Toggle/On/Off the attitude hold for auto pilot", "Attitude Hold - {0}")]
    [TouchPortalActionChoice(new string[] { "Toggle", "On", "Off" }, "Toggle")]
    public object AP_ATTITUDE { get; }

    [TouchPortalAction("AutoPilotApproach", "Approach Mode", "MSFS", "Toggle/On/Off the approach mode for auto pilot", "Approach Mode - {0}")]
    [TouchPortalActionChoice(new string[] { "Toggle", "On", "Off" }, "Toggle")]
    public object AP_APPROACH { get; }

    [TouchPortalAction("AutoPilotHeading", "Heading Hold", "MSFS", "Toggle/On/Off the heading hold for auto pilot", "Heading Hold - {0}")]
    [TouchPortalActionChoice(new string[] { "Toggle", "On", "Off" }, "Toggle")]
    public object AP_HEADING { get; }

    [TouchPortalAction("AutoPilotAltitude", "Altitude Hold", "MSFS", "Toggle/On/Off the altitude hold for auto pilot", "Altitude Hold - {0}")]
    [TouchPortalActionChoice(new string[] { "Toggle", "On", "Off" }, "Toggle")]
    public object AP_ALTITUDE { get; }

    [TouchPortalAction("AutoPilotBackCourse", "Back Course Mode", "MSFS", "Toggle/On/Off the back course mode for auto pilot", "Back Course Mode - {0}")]
    [TouchPortalActionChoice(new string[] { "Toggle", "On", "Off" }, "Toggle")]
    public object AP_BACKCOURSE { get; }

    [TouchPortalAction("AutoPilotNav1", "Nav1 Mode", "MSFS", "Toggle/On/Off the Nav1 mode for auto pilot", "Nav1 Mode - {0}")]
    [TouchPortalActionChoice(new string[] { "Toggle", "On", "Off" }, "Toggle")]
    public object AP_NAV1 { get; }


  }

  [SimNotificationGroup(SimConnectWrapper.Groups.AutoPilot)]
  [TouchPortalCategoryMapping("AutoPilot")]
  internal enum AutoPilot {
    // Placeholder to offset each enum for SimConnect
    Init = 1000,

    // TODO: Heading/Alt Settings

    // Auto Pilot
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotMaster", "Toggle")]
    AP_MASTER,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotMaster", "On")]
    AUTOPILOT_ON,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotMaster", "Off")]
    AUTOPILOT_OFF,

    // TODO: ??
    AP_PANEL_HEADING_HOLD,
    AP_PANEL_ALTITUDE_HOLD,
    AP_PANEL_SPEED_HOLD,
    AP_PANEL_MACH_HOLD,
    AP_MACH_HOLD,
    AP_PANEL_SPEED_HOLD_TOGGLE,
    AP_PANEL_MACH_HOLD_TOGGLE,

    // Attitude
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotAttitude", "Toggle")]
    AP_ATT_HOLD,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotAttitude", "On")]
    AP_ATT_HOLD_ON,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotAttitude", "Off")]
    AP_ATT_HOLD_OFF,

    // TODO: ??
    AP_LOC_HOLD,
    AP_LOC_HOLD_ON,
    AP_LOC_HOLD_OFF,

    // Approach Mode
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotApproach", "Toggle")]
    AP_APR_HOLD,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotApproach", "On")]
    AP_APR_HOLD_ON,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotApproach", "Off")]
    AP_APR_HOLD_OFF,

    // Heading
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotHeading", "Toggle")]
    AP_HDG_HOLD,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotHeading", "On")]
    AP_HDG_HOLD_ON,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotHeading", "Off")]
    AP_HDG_HOLD_OFF,

    // Altitude
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotAltitude", "Toggle")]
    AP_ALT_HOLD,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotAltitude", "On")]
    AP_ALT_HOLD_ON,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotAltitude", "Off")]
    AP_ALT_HOLD_OFF,

    // Back course mode
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotBackCourse", "Toggle")]
    AP_BC_HOLD,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotBackCourse", "On")]
    AP_BC_HOLD_ON,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotBackCourse", "Off")]
    AP_BC_HOLD_OFF,

    // Nav1 mode
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotNav1", "Toggle")]
    AP_NAV1_HOLD,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotNav1", "On")]
    AP_NAV1_HOLD_ON,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotNav1", "Off")]
    AP_NAV1_HOLD_OFF,

    // TODO: ??
    AP_WING_LEVELER,
    AP_WING_LEVELER_ON,
    AP_WING_LEVELER_OFF,
    AP_AIRSPEED_HOLD
  }
}
