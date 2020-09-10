using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Enums;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.AutoPilot {

  [SimVarDataRequestGroup]
  [TouchPortalCategory("AutoPilot", "MSFS - AutoPilot")]
  internal class AutoPilotMapping {

    #region AutoPilot Master

    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotMaster", "AutoPilot", "MSFS", "Toggle/On/Off Auto Pilot", "Auto Pilot Master - {0}")]
    [TouchPortalActionChoice(new string[] { "Toggle", "On", "Off" }, "Toggle")]
    [TouchPortalState("AutoPilotMaster", "text", "AutoPilot Master Status", "")]
    public static SimVarItem AP_MASTER = new SimVarItem() { def = Definition.AutoPilotMaster, req = Request.AutoPilotMaster, SimVarName = "AUTOPILOT MASTER", Unit = Units.Bool, CanSet = false };

    [SimVarDataRequest]
    [TouchPortalState("AutoPilotAvailable", "text", "AutoPilot Availability", "")]
    public static SimVarItem AutoPilotAvailable =
      new SimVarItem() { def = Definition.AutoPilotAvailable, req = Request.AutoPilotAvailable, SimVarName = "AUTOPILOT AVAILABLE", Unit = Units.Bool, CanSet = false };

    #endregion

    #region Attitude

    [TouchPortalAction("AutoPilotAttitude", "Attitude Hold", "MSFS", "Toggle/On/Off the attitude hold for auto pilot", "Attitude Hold - {0}")]
    [TouchPortalActionChoice(new string[] { "Toggle", "On", "Off" }, "Toggle")]
    public object AP_ATTITUDE { get; }

    [SimVarDataRequest]
    [TouchPortalState("AutoPilotAttitudeHold", "text", "AutoPilot Attitude Status", "")]
    public static SimVarItem AutoPilotAttitudeHold =
      new SimVarItem() { def = Definition.AutoPilotAttitudeHold, req = Request.AutoPilotAttitudeHold, SimVarName = "AUTOPILOT ATTITUDE HOLD", Unit = Units.Bool, CanSet = false };

    #endregion

    #region Approach

    [TouchPortalAction("AutoPilotApproach", "Approach Mode", "MSFS", "Toggle/On/Off the approach mode for auto pilot", "Approach Mode - {0}")]
    [TouchPortalActionChoice(new string[] { "Toggle", "On", "Off" }, "Toggle")]
    public object AP_APPROACH { get; }

    [SimVarDataRequest]
    [TouchPortalState("AutoPilotApproachHold", "text", "AutoPilot Approach Status", "")]
    public static SimVarItem AutoPilotApproachHold =
      new SimVarItem() { def = Definition.AutoPilotApproachHold, req = Request.AutoPilotApproachHold, SimVarName = "AUTOPILOT APPROACH HOLD", Unit = Units.Bool, CanSet = false };

    #endregion

    #region Heading

    [TouchPortalAction("AutoPilotHeading", "Heading Hold", "MSFS", "Toggle/On/Off the heading hold for auto pilot", "Heading Hold - {0}")]
    [TouchPortalActionChoice(new string[] { "Toggle", "On", "Off" }, "Toggle")]
    public object AP_HEADING { get; }

    [SimVarDataRequest]
    [TouchPortalState("AutoPilotHeadingHold", "text", "AutoPilot Heading Status", "")]
    public static SimVarItem AutoPilotHeadingHold = 
      new SimVarItem() { def = Definition.AutoPilotHeadingHold, req = Request.AutoPilotHeadingHold, SimVarName = "AUTOPILOT HEADING LOCK", Unit = Units.Bool, CanSet = false };

    [SimVarDataRequest]
    [TouchPortalState("AutoPilotHeadingVar", "text", "AutoPilot Heading Direction", "")]
    public static SimVarItem AutoPilotHeadingVar = 
      new SimVarItem() { def = Definition.AutoPilotHeadingVar, req = Request.AutoPilotHeadingVar, SimVarName = "AUTOPILOT HEADING LOCK DIR", Unit = Units.degrees, CanSet = false, StringFormat = "{0:F0}" };

    [TouchPortalAction("AutoPilotHeadingVar", "Heading Hold Value", "MSFS", "Sets the heading hold value", "Heading Hold Value - {0}")]
    [TouchPortalActionChoice(new string[] { "Select", "Increase", "Decrease", "Set" }, "Select")]
    public object AP_HEADING_VAR { get; }

    #endregion

    #region Altitude

    [TouchPortalAction("AutoPilotAltitude", "Altitude Hold", "MSFS", "Toggle/On/Off the altitude hold for auto pilot", "Altitude Hold - {0}")]
    [TouchPortalActionChoice(new string[] { "Toggle", "On", "Off" }, "Toggle")]
    public object AP_ALTITUDE { get; }

    [SimVarDataRequest]
    [TouchPortalState("AutoPilotAltitudeHold", "text", "AutoPilot Altitude Status", "")]
    public static SimVarItem AutoPilotAltitudeHold =
      new SimVarItem() { def = Definition.AutoPilotAltitudeHold, req = Request.AutoPilotAltitudeHold, SimVarName = "AUTOPILOT ALTITUDE LOCK", Unit = Units.Bool, CanSet = false };

    [TouchPortalAction("AutoPilotAltitudeVar", "Altitude Hold Value", "MSFS", "Sets the altitude hold value", "Altitude Hold Value - {0}")]
    [TouchPortalActionChoice(new string[] { "Select", "Increase", "Decrease" }, "Select")]
    public object AP_ALTITUDE_VAR { get; }

    [SimVarDataRequest]
    [TouchPortalState("AutoPilotAltitudeVar", "text", "AutoPilot Altitude Value", "")]
    public static SimVarItem AutoPilotAltitudeVar = 
      new SimVarItem() { def = Definition.AutoPilotAltitudeVar, req = Request.AutoPilotAltitudeVar, SimVarName = "AUTOPILOT ALTITUDE LOCK VAR", Unit = Units.feet, CanSet = false };

    #endregion

    #region Back Course

    [TouchPortalAction("AutoPilotBackCourse", "Back Course Mode", "MSFS", "Toggle/On/Off the back course mode for auto pilot", "Back Course Mode - {0}")]
    [TouchPortalActionChoice(new string[] { "Toggle", "On", "Off" }, "Toggle")]
    public object AP_BACKCOURSE { get; }

    [SimVarDataRequest]
    [TouchPortalState("AutoPilotBackCourseHold", "text", "AutoPilot Back Course Status", "")]
    public static SimVarItem AutoPilotBackCourseHold = 
      new SimVarItem() { def = Definition.AutoPilotBackCourseHold, req = Request.AutoPilotBackCourseHold, SimVarName = "AUTOPILOT BACKCOURSE HOLD", Unit = Units.Bool, CanSet = false };

    #endregion

    #region Nav1

    [TouchPortalAction("AutoPilotNav1", "Nav1 Mode", "MSFS", "Toggle/On/Off the Nav1 mode for auto pilot", "Nav1 Mode - {0}")]
    [TouchPortalActionChoice(new string[] { "Toggle", "On", "Off" }, "Toggle")]
    public object AP_NAV1 { get; }

    [SimVarDataRequest]
    [TouchPortalState("AutoPilotNav1Hold", "text", "AutoPilot Nav1 Status", "")]
    public static SimVarItem AutoPilotNav1Hold = 
      new SimVarItem() { def = Definition.AutoPilotNav1Hold, req = Request.AutoPilotNav1Hold, SimVarName = "AUTOPILOT NAV1 LOCK", Unit = Units.Bool, CanSet = false };

    #endregion

    #region Vertical Speed

    [SimVarDataRequest]
    [TouchPortalState("AutoPilotVerticalSpeedHold", "text", "AutoPilot Vertical Speed Status", "")]
    public static SimVarItem AutoPilotVerticalSpeedHold = 
      new SimVarItem() { def = Definition.AutoPilotVerticalSpeedHold, req = Request.AutoPilotVerticalSpeedHold, SimVarName = "AUTOPILOT VERTICAL HOLD", Unit = Units.Bool, CanSet = false };


    [TouchPortalAction("AutoPilotVerticalSpeedVar", "Vertical Speed Value", "MSFS", "Sets the vertical speed value", "Vertical Speed Value - {0}")]
    [TouchPortalActionChoice(new string[] { "Select", "Increase", "Decrease", "Set" }, "Select")]
    public object AP_VERTICALSPEED_VAR { get; }

    [SimVarDataRequest]
    [TouchPortalState("AutoPilotVerticalSpeedVar", "text", "AutoPilot Vertical Speed Value", "")]
    public static SimVarItem AutoPilotVerticalSpeedVar = 
      new SimVarItem() { def = Definition.AutoPilotVerticalSpeedVar, req = Request.AutoPilotVerticalSpeedVar, SimVarName = "AUTOPILOT VERTICAL HOLD VAR", Unit = Units.feetminute, CanSet = false };

    #endregion

    #region Airspeed

    [TouchPortalAction("AutoPilotAirSpeed", "Airspeed Hold", "MSFS", "Toggle/On/Off/Set the airspeed hold for auto pilot", "Airspeed Hold - {0}")]
    [TouchPortalActionChoice(new string[] { "Toggle", "On", "Off", "Set" }, "Toggle")]
    public object AP_AIRSPEED { get; }

    [SimVarDataRequest]
    [TouchPortalState("AutoPilotAirSpeedHold", "text", "AutoPilot Air Speed Status", "")]
    public static SimVarItem AutoPilotAirSpeedHold = 
      new SimVarItem() { def = Definition.AutoPilotAirSpeedHold, req = Request.AutoPilotAirSpeedHold, SimVarName = "AUTOPILOT AIRSPEED HOLD", Unit = Units.Bool, CanSet = false };

    [TouchPortalAction("AutoPilotAirSpeedVar", "Airspeed Hold Value", "MSFS", "Sets the airspeed hold value", "Airspeed Hold Value - {0}")]
    [TouchPortalActionChoice(new string[] { "Select", "Increase", "Decrease", "Set" }, "Select")]
    public object AP_AIRSPEED_VAR { get; }

    [SimVarDataRequest]
    [TouchPortalState("AutoPilotAirSpeedVar", "text", "AutoPilot Air Speed Value", "")]
    public static SimVarItem AutoPilotAirSpeedVar = 
      new SimVarItem() { def = Definition.AutoPilotAirSpeedVar, req = Request.AutoPilotAirSpeedVar, SimVarName = "AUTOPILOT AIRSPEED HOLD VAR", Unit = Units.knots, CanSet = false };

    #endregion

    #region Mach

    [TouchPortalAction("AutoPilotMach", "Mach Hold", "MSFS", "Toggle/On/Off/Set the mach hold for auto pilot", "Mach Hold - {0}")]
    [TouchPortalActionChoice(new string[] { "Toggle", "On", "Off", "Set" }, "Toggle")]
    public object AP_MACH { get; }

    [TouchPortalAction("AutoPilotMachVar", "Mach Hold Value", "MSFS", "Sets the mach hold value", "Mach Hold Value - {0}")]
    [TouchPortalActionChoice(new string[] { "Select", "Increase", "Decrease" }, "Increase")]
    public object AP_MACH_VAR { get; }

    #endregion

    #region Flight Director

    [TouchPortalAction("AutoPilotFlightDirector", "Flight Director", "MSFS", "Toggle the Flight Director for auto pilot", "Flight Director - {0}")]
    [TouchPortalActionChoice(new string[] { "Toggle" }, "Toggle")]
    public object AP_FLIGHT_DIRECTOR { get; }

    #endregion

    #region Wing Leveler

    [TouchPortalAction("AutoPilotWingLeveler", "Wing Leveler", "MSFS", "Toggle/On/Off the Wing Leveler for auto pilot", "Wing Leveler - {0}")]
    [TouchPortalActionChoice(new string[] { "Toggle", "On", "Off" }, "Toggle")]
    public object AP_WING_LEVELER { get; }

    #endregion

    #region Localizer

    [TouchPortalAction("AutoPilotLocalizer", "Localizer", "MSFS", "Toggle/On/Off the localizer for auto pilot", "Localizer - {0}")]
    [TouchPortalActionChoice(new string[] { "Toggle", "On", "Off" }, "Toggle")]
    public object AP_LOCALIZER { get; }

    #endregion

    #region Yaw Dampener

    [TouchPortalAction("AutoPilotYawDampener", "Yaw Dampener", "MSFS", "Toggle/On/Off/Set the Yaw Dampener", "Yaw Dampener - {0}")]
    [TouchPortalActionChoice(new string[] { "Toggle", "On", "Off", "Set" }, "Toggle")]
    public object AP_YAWDAMPENER { get; }

    #endregion

    #region Flight Level Control

    [SimVarDataRequest]
    [TouchPortalState("AutoPilotPitchHold", "text", "The status of Auto Pilot Pitch Hold button", "")]
    public static SimVarItem AutoPilotPitchHold =
      new SimVarItem() { def = Definition.AutoPilotPitchHold, req = Request.AutoPilotPitchHold, SimVarName = "AUTOPILOT PITCH HOLD", Unit = Units.Bool, CanSet = false }; 

    #endregion
  }

  [SimNotificationGroup(Groups.AutoPilot)]
  [TouchPortalCategoryMapping("AutoPilot")]
  internal enum AutoPilot {
    // Placeholder to offset each enum for SimConnect
    Init = 1000,

    // TODO: ??
    AP_PANEL_HEADING_HOLD,
    AP_PANEL_ALTITUDE_HOLD,
    AP_PANEL_SPEED_HOLD,
    AP_PANEL_MACH_HOLD,
    AP_PANEL_SPEED_HOLD_TOGGLE,
    AP_PANEL_MACH_HOLD_TOGGLE,

    #region AutoPilot Master

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

    #endregion

    #region Attitude

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

    #endregion

    #region Approach

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

    #endregion

    #region Heading

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

    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotHeadingVar", "Set")]
    HEADING_BUG_SET,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotHeadingVar", "Increase")]
    HEADING_BUG_INC,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotHeadingVar", "Decrease")]
    HEADING_BUG_DEC,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotHeadingVar", "Select")]
    HEADING_BUG_SELECT,

    #endregion

    #region Altitude

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

    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotAltitudeVar", "Increase")]
    AP_ALT_VAR_INC,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotAltitudeVar", "Decrease")]
    AP_ALT_VAR_DEC,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotAltitudeVar", "Select")]
    ALTITUDE_BUG_SELECT,

    #endregion

    #region Back Course

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

    #endregion

    #region Nav1

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

    #endregion

    #region Vertical Speed

    // Vertical Speed
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotVerticalSpeedVar", "Increase")]
    AP_VS_VAR_INC,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotVerticalSpeedVar", "Decrease")]
    AP_VS_VAR_DEC,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotVerticalSpeedVar", "Set")]
    AP_VS_VAR_SET_ENGLISH,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotVerticalSpeedVar", "Select")]
    VSI_BUG_SELECT,

    #endregion

    #region Airspeed

    // Air Speed
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotAirSpeed", "Toggle")]
    AP_AIRSPEED_HOLD,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotAirSpeed", "On")]
    AP_AIRSPEED_ON,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotAirSpeed", "Off")]
    AP_AIRSPEED_OFF,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotAirSpeed", "Set")]
    AP_AIRSPEED_SET,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotAirSpeedVar", "Increase")]
    AP_SPD_VAR_INC,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotAirSpeedVar", "Decrease")]
    AP_SPD_VAR_DEC,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotAirSpeedVar", "Set")]
    AP_SPD_VAR_SET,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotAirSpeedVar", "Select")]
    AIRSPEED_BUG_SELECT,

    #endregion

    #region Mach

    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotMach", "Toggle")]
    AP_MACH_HOLD,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotMach", "On")]
    AP_MACH_ON,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotMach", "Off")]
    AP_MACH_OFF,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotMach", "Set")]
    AP_MACH_SET,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotMachVar", "Increase")]
    AP_MACH_VAR_INC,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotMachVar", "Decrease")]
    AP_MACH_VAR_DEC,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotMachVar", "Set")]
    AP_MACH_VAR_SET,

    #endregion

    #region Flight Director

    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotFlightDirector", "Toggle")]
    TOGGLE_FLIGHT_DIRECTOR,

    #endregion

    #region Wing Leveler

    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotWingLeveler", "Toggle")]
    AP_WING_LEVELER,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotWingLeveler", "On")]
    AP_WING_LEVELER_ON,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotWingLeveler", "Off")]
    AP_WING_LEVELER_OFF,

    #endregion

    #region Localizer

    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotLocalizer", "Toggle")]
    AP_LOC_HOLD,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotLocalizer", "On")]
    AP_LOC_HOLD_ON,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotLocalizer", "Off")]
    AP_LOC_HOLD_OFF,

    #endregion

    #region Yaw Dampener

    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotYawDampener", "Toggle")]
    YAW_DAMPER_TOGGLE,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotYawDampener", "On")]
    YAW_DAMPER_ON,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotYawDampener", "Off")]
    YAW_DAMPER_OFF,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotYawDampener", "Set")]
    YAW_DAMPER_SET,

    #endregion
  }
}
