using MSFSTouchPortalPlugin.Attributes;

namespace MSFSTouchPortalPlugin {
  [TouchPortalCategory("AutoPilot", "MSFS - AutoPilot")]
  internal enum AutoPilot {
    // Auto Pilot
    [SimActionEvent]
    [TouchPortalAction("AutoPilotMaster", "AutoPilot - Toggle", "MSFS", "Toggles the autopilot on and off", "")]
    AP_MASTER,

    [SimActionEvent]
    [TouchPortalAction("AutoPilotOn", "AutoPilot - ON", "MSFS", "Turns the autopilot on", "")]
    AUTOPILOT_ON,

    [SimActionEvent]
    [TouchPortalAction("AutoPilotOff", "AutoPilot - Off", "MSFS", "Turns the autopilot off", "")]
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
    [TouchPortalAction("AutoPilotAttitudeToggle", "Attitude Hold - Toggle", "MSFS", "Toggles the attitude hold for auto pilot", "")]
    AP_ATT_HOLD,

    [SimActionEvent]
    [TouchPortalAction("AutoPilotAttitudeOn", "Attitude Hold - On", "MSFS", "Turns on the attitude hold for auto pilot", "")]
    AP_ATT_HOLD_ON,

    [SimActionEvent]
    [TouchPortalAction("AutoPilotAttitudeOff", "Attitude Hold - Off", "MSFS", "Turns off the attitude hold for auto pilot", "")]
    AP_ATT_HOLD_OFF,

    // TODO: ??
    AP_LOC_HOLD,
    AP_LOC_HOLD_ON,
    AP_LOC_HOLD_OFF,

    // Approach Mode
    [SimActionEvent]
    [TouchPortalAction("AutoPilotApproachToggle", "Approach mode - Toggle", "MSFS", "Toggles the approach mode for auto pilot", "")]
    AP_APR_HOLD,

    [SimActionEvent]
    [TouchPortalAction("AutoPilotApproachOn", "Approach mode - On", "MSFS", "Turns on the approach mode for auto pilot", "")]
    AP_APR_HOLD_ON,

    [SimActionEvent] // Broken?
    [TouchPortalAction("AutoPilotApproachOff", "Approach mode - Off", "MSFS", "Turns off the approach mode for auto pilot", "")]
    AP_APR_HOLD_OFF,

    // Heading
    [SimActionEvent]
    [TouchPortalAction("AutoPilotHeadingToggle", "Heading Hold - Toggle", "MSFS", "Toggles the heading hold for auto pilot", "")]
    AP_HDG_HOLD,

    [SimActionEvent]
    [TouchPortalAction("AutoPilotHeadingOn", "Heading Hold - On", "MSFS", "Turns on the heading hold for auto pilot", "")]
    AP_HDG_HOLD_ON,

    [SimActionEvent]
    [TouchPortalAction("AutoPilotHeadingOff", "Heading Hold - Off", "MSFS", "Turns off the heading hold for auto pilot", "")]
    AP_HDG_HOLD_OFF,

    // Altitude
    [SimActionEvent]
    [TouchPortalAction("AutoPilotAltitudeToggle", "Altitude Hold - Toggle", "MSFS", "Toggles the altitude hold for auto pilot", "")]
    AP_ALT_HOLD,

    [SimActionEvent]
    [TouchPortalAction("AutoPilotAltitudeOn", "Altitude Hold - On", "MSFS", "Turns on the altitude hold for auto pilot", "")]
    AP_ALT_HOLD_ON,

    [SimActionEvent]
    [TouchPortalAction("AutoPilotAltitudeOff", "Altitude Hold - Off", "MSFS", "Turns off the altitude hold for auto pilot", "")]
    AP_ALT_HOLD_OFF,

    // Back course mode
    [SimActionEvent]
    [TouchPortalAction("AutoPilotBackCourseToggle", "Back Course mode - Toggle", "MSFS", "Toggles the back course mode for auto pilot", "")]
    AP_BC_HOLD,

    [SimActionEvent]
    [TouchPortalAction("AutoPilotBackCourseOn", "Back Course mode - On", "MSFS", "Turns on the back course mode for auto pilot", "")]
    AP_BC_HOLD_ON,

    [SimActionEvent]
    [TouchPortalAction("AutoPilotBackCourseOff", "Back Course mode - Off", "MSFS", "Turns off the back course mode for auto pilot", "")]
    AP_BC_HOLD_OFF,

    // Nav1 mode
    [SimActionEvent]
    [TouchPortalAction("AutoPilotNav1Toggle", "Nav1 mode - Toggle", "MSFS", "Toggles the Nav1 mode for auto pilot", "")]
    AP_NAV1_HOLD,

    [SimActionEvent]
    [TouchPortalAction("AutoPilotNav1On", "Nav1 mode - On", "MSFS", "Turns on the Nav1 mode for auto pilot", "")]
    AP_NAV1_HOLD_ON,

    [SimActionEvent]
    [TouchPortalAction("AutoPilotNav1Off", "Nav1 mode - Off", "MSFS", "Turns off the Nav1 mode for auto pilot", "")]
    AP_NAV1_HOLD_OFF,

    // TODO: ??

    AP_WING_LEVELER,

    AP_WING_LEVELER_ON,

    AP_WING_LEVELER_OFF,

    AP_AIRSPEED_HOLD
  }
}
