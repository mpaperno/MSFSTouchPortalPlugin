using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Enums;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.AutoPilot {

  [SimVarDataRequestGroup]
  [TouchPortalCategory("AutoPilot", "MSFS - AutoPilot")]
  internal static class AutoPilotMapping {

    #region AutoPilot Master

    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotMaster", "AutoPilot", "MSFS", "Toggle/On/Off Auto Pilot", "Auto Pilot Master - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" }, "Toggle")]
    [TouchPortalState("AutoPilotMaster", "text", "AutoPilot Master Status", "")]
    public static readonly SimVarItem AP_MASTER = new SimVarItem { Def = Definition.AutoPilotMaster, SimVarName = "AUTOPILOT MASTER", Unit = Units.Bool, CanSet = false };

    [SimVarDataRequest]
    [TouchPortalState("AutoPilotAvailable", "text", "AutoPilot Availability", "")]
    public static readonly SimVarItem AutoPilotAvailable =
      new SimVarItem { Def = Definition.AutoPilotAvailable, SimVarName = "AUTOPILOT AVAILABLE", Unit = Units.Bool, CanSet = false };

    [SimVarDataRequest]
    [TouchPortalState("AutoPilotPitchHold", "text", "The status of Auto Pilot Pitch Hold button", "")]
    public static readonly SimVarItem AutoPilotPitchHold =
      new SimVarItem { Def = Definition.AutoPilotPitchHold, SimVarName = "AUTOPILOT PITCH HOLD", Unit = Units.Bool, CanSet = false };

    #endregion

    #region Attitude

    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotAttitude", "Attitude Hold", "MSFS", "Toggle/On/Off the attitude hold for auto pilot", "Attitude Hold - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" }, "Toggle")]
    [TouchPortalState("AutoPilotAttitudeHold", "text", "AutoPilot Attitude Status", "")]
    public static readonly SimVarItem AP_ATTITUDE =
      new SimVarItem { Def = Definition.AutoPilotAttitudeHold, SimVarName = "AUTOPILOT ATTITUDE HOLD", Unit = Units.Bool, CanSet = false };

    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotAttitudeVar", "Attitude Hold Value", "MSFS", "Sets the attitude hold value", "Attitude Hold Value - {0}")]
    [TouchPortalActionChoice(new [] { "Select", "Increase", "Decrease" }, "Select")]
    [TouchPortalState("AutoPilotAttitudeVar", "text", "AutoPilot Pitch Reference Value", "")]
    public static readonly SimVarItem AP_ATTITUDE_PITCH =
      new SimVarItem { Def = Definition.AutoPilotAttitudeVar, SimVarName = "AUTOPILOT PITCH HOLD REF", Unit = Units.radians, CanSet = false };

    #endregion

    #region Approach

    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotApproach", "Approach Mode", "MSFS", "Toggle/On/Off the approach mode for auto pilot", "Approach Mode - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" }, "Toggle")]
    [TouchPortalState("AutoPilotApproachHold", "text", "AutoPilot Approach Status", "")]
    public static readonly SimVarItem AP_APPROACH =
      new SimVarItem { Def = Definition.AutoPilotApproachHold, SimVarName = "AUTOPILOT APPROACH HOLD", Unit = Units.Bool, CanSet = false };

    #endregion

    #region Bank

    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotBanking", "AP Max Bank Angle", "MSFS", "Increase/Decrease the max bank angle", "Max Bank Angle - {0}")]
    [TouchPortalActionChoice(new [] { "Increase", "Decrease" }, "Increase")]
    [TouchPortalState("AutoPilotBanking", "text", "AutoPilot Max Bank Angle", "")]
    public static readonly SimVarItem AP_MAX_BANK =
      new SimVarItem { Def = Definition.AutoPilotBanking, SimVarName = "AUTOPILOT MAX BANK", Unit = Units.radians, CanSet = false };

    #endregion

    #region Heading

    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotHeading", "Heading Hold", "MSFS", "Toggle/On/Off the heading hold for auto pilot", "Heading Hold - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" }, "Toggle")]
    [TouchPortalState("AutoPilotHeadingHold", "text", "AutoPilot Heading Status", "")]
    public static readonly SimVarItem AP_HEADING =
      new SimVarItem { Def = Definition.AutoPilotHeadingHold, SimVarName = "AUTOPILOT HEADING LOCK", Unit = Units.Bool, CanSet = false };


    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotHeadingVar", "Heading Hold Value", "MSFS", "Sets the heading hold value", "Heading Hold Value - {0}")]
    [TouchPortalActionChoice(new [] { "Select", "Increase", "Decrease", "Set" }, "Select")]
    [TouchPortalState("AutoPilotHeadingVar", "text", "AutoPilot Heading Direction", "")]
    public static readonly SimVarItem AP_HEADING_VAR =
      new SimVarItem { Def = Definition.AutoPilotHeadingVar, SimVarName = "AUTOPILOT HEADING LOCK DIR", Unit = Units.degrees, CanSet = false, StringFormat = "{0:F0}" };

    #endregion

    #region Altitude

    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotAltitude", "Altitude Hold", "MSFS", "Toggle/On/Off the altitude hold for auto pilot", "Altitude Hold - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" }, "Toggle")]
    [TouchPortalState("AutoPilotAltitudeHold", "text", "AutoPilot Altitude Status", "")]
    public static readonly SimVarItem AP_ALTITUDE =
      new SimVarItem { Def = Definition.AutoPilotAltitudeHold, SimVarName = "AUTOPILOT ALTITUDE LOCK", Unit = Units.Bool, CanSet = false };


    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotAltitudeVar", "Altitude Hold Value", "MSFS", "Sets the altitude hold value", "Altitude Hold Value - {0}")]
    [TouchPortalActionChoice(new [] { "Select", "Increase", "Decrease", "Set", "Set Metric" }, "Select")]
    [TouchPortalState("AutoPilotAltitudeVar", "text", "AutoPilot Altitude Value", "")]
    public static readonly SimVarItem AP_ALTITUDE_VAR =
      new SimVarItem { Def = Definition.AutoPilotAltitudeVar, SimVarName = "AUTOPILOT ALTITUDE LOCK VAR", Unit = Units.feet, CanSet = false };

    #endregion

    #region Back Course

    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotBackCourse", "Back Course Mode", "MSFS", "Toggle/On/Off the back course mode for auto pilot", "Back Course Mode - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" }, "Toggle")]
    [TouchPortalState("AutoPilotBackCourseHold", "text", "AutoPilot Back Course Status", "")]
    public static readonly SimVarItem AP_BACKCOURSE =
      new SimVarItem { Def = Definition.AutoPilotBackCourseHold, SimVarName = "AUTOPILOT BACKCOURSE HOLD", Unit = Units.Bool, CanSet = false };

    #endregion

    #region Nav1

    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotNav1", "Nav1 Mode", "MSFS", "Toggle/On/Off the Nav1 mode for auto pilot", "Nav1 Mode - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" }, "Toggle")]
    [TouchPortalState("AutoPilotNav1Hold", "text", "AutoPilot Nav1 Status", "")]
    public static readonly SimVarItem AP_NAV1 =
      new SimVarItem { Def = Definition.AutoPilotNav1Hold, SimVarName = "AUTOPILOT NAV1 LOCK", Unit = Units.Bool, CanSet = false };

    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotNavSelect", "Nav Mode - Set", "MSFS", "Sets the nav to 1 or 2 for Nav mode", "Nav Mode - {0} ")]
    [TouchPortalActionChoice(new [] { "1", "2" }, "1")]
    [TouchPortalState("AutoPilotNavSelected", "text", "AutoPilot Nav Selected Index", "")]
    public static readonly SimVarItem AP_NAV_SELECT_SET =
      new SimVarItem { Def = Definition.AutoPilotNavSelected, SimVarName = "AUTOPILOT NAV SELECTED", Unit = Units.number, CanSet = false };

    #endregion

    #region Vertical Speed


    // TODO action?
    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotVerticalSpeed", "Vertical Speed", "MSFS", "Toggle the Vertical Speed for auto pilot", "Vertical Speed - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle" }, "Toggle")]
    [TouchPortalState("AutoPilotVerticalSpeedHold", "text", "AutoPilot Vertical Speed Status", "")]
    public static readonly SimVarItem AutoPilotVerticalSpeedHold = 
      new SimVarItem { Def = Definition.AutoPilotVerticalSpeedHold, SimVarName = "AUTOPILOT VERTICAL HOLD", Unit = Units.Bool, CanSet = false };


    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotVerticalSpeedVar", "Vertical Speed Value", "MSFS", "Sets the vertical speed value", "Vertical Speed Value - {0}")]
    [TouchPortalActionChoice(new [] { "Select", "Increase", "Decrease", "Set", "Set Metric" }, "Select")]
    [TouchPortalState("AutoPilotVerticalSpeedVar", "text", "AutoPilot Vertical Speed Value", "")]
    public static readonly SimVarItem AP_VERTICALSPEED_VAR =
      new SimVarItem { Def = Definition.AutoPilotVerticalSpeedVar, SimVarName = "AUTOPILOT VERTICAL HOLD VAR", Unit = Units.feetminute, CanSet = false };

    #endregion

    #region Airspeed

    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotAirSpeed", "Airspeed Hold", "MSFS", "Toggle/On/Off/Set the airspeed hold for auto pilot", "Airspeed Hold - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off", "Set" }, "Toggle")]
    [TouchPortalState("AutoPilotAirSpeedHold", "text", "AutoPilot Air Speed Status", "")]
    public static readonly SimVarItem AP_AIRSPEED =
      new SimVarItem { Def = Definition.AutoPilotAirSpeedHold, SimVarName = "AUTOPILOT AIRSPEED HOLD", Unit = Units.Bool, CanSet = false };

    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotAirSpeedVar", "Airspeed Hold Value", "MSFS", "Sets the airspeed hold value", "Airspeed Hold Value - {0}")]
    [TouchPortalActionChoice(new [] { "Select", "Increase", "Decrease", "Set" }, "Select")]
    [TouchPortalState("AutoPilotAirSpeedVar", "text", "AutoPilot Air Speed Value", "")]
    public static readonly SimVarItem AP_AIRSPEED_VAR =
      new SimVarItem { Def = Definition.AutoPilotAirSpeedVar, SimVarName = "AUTOPILOT AIRSPEED HOLD VAR", Unit = Units.knots, CanSet = false };

    #endregion

    #region AutoThrottle

    [SimVarDataRequest]
    [TouchPortalAction("AutoThrottle", "Auto Throttle Mode", "MSFS", "Toggles the Arm/GoAround modes for auto throttle", "Toggle Auto Throttle - {0}")]
    [TouchPortalActionChoice(new [] { "Arm", "GoAround" }, "Arm")]
    [TouchPortalState("AutoThrottleArm", "text", "Auto Throttle Armed", "")]
    public static readonly SimVarItem AUTO_THROTTLE =
      new SimVarItem { Def = Definition.AutoThrottleArm, SimVarName = "AUTOPILOT THROTTLE ARM", Unit = Units.Bool, CanSet = false };

    [SimVarDataRequest]
    [TouchPortalState("AutoThrottleGoAround", "text", "Auto Throttle GoAround", "")]
    public static readonly SimVarItem AUTO_THROTTLE_GA =
      new SimVarItem { Def = Definition.AutoThrottleGA, SimVarName = "AUTOPILOT TAKEOFF POWER ACTIVE", Unit = Units.Bool, CanSet = false };


    #endregion

    #region AutoBrake

    [TouchPortalAction("AutoBrake", "Auto Brake", "MSFS", "Increase/Decrease the auto brake", "Auto Brake - {0}")]
    [TouchPortalActionChoice(new [] { "Increaes", "Decrease" }, "Increase")]
    public static object AUTO_BRAKE { get; }

    #endregion

    #region Mach

    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotMach", "Mach Hold", "MSFS", "Toggle/On/Off/Set the mach hold for auto pilot", "Mach Hold - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off", "Set" }, "Toggle")]
    [TouchPortalState("AutoPilotMach", "text", "AutoPilot Mach Hold", "")]
    public static readonly SimVarItem AP_MACH =
      new SimVarItem { Def = Definition.AutoPilotMach, SimVarName = "AUTOPILOT MACH HOLD", Unit = Units.Bool, CanSet = false };

    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotMachVar", "Mach Hold Value", "MSFS", "Sets the mach hold value", "Mach Hold Value - {0}")]
    [TouchPortalActionChoice(new [] { "Select", "Increase", "Decrease" }, "Increase")]
    [TouchPortalState("AutoPilotMachVar", "text", "AutoPilot Mach Value", "")]
    public static readonly SimVarItem AP_MACH_VAR =
      new SimVarItem { Def = Definition.AutoPilotMachVar, SimVarName = "AUTOPILOT MACH HOLD VAR", Unit = Units.number, CanSet = false };

    #endregion

    #region Flight Director

    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotFlightDirector", "Flight Director", "MSFS", "Toggle the Flight Director for auto pilot", "Flight Director - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle" }, "Toggle")]
    [TouchPortalState("AutoPilotFlightDirector", "text", "AutoPilot Flight Director Status", "")]
    public static readonly SimVarItem AP_FLIGHT_DIRECTOR =
      new SimVarItem { Def = Definition.AutoPilotFlightDirector, SimVarName = "AUTOPILOT FLIGHT DIRECTOR ACTIVE", Unit = Units.Bool, CanSet = false };

    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotFlightDirectorCurrentPitch", "Flight Director Pitch Sync", "MSFS", "Syncs the Flight Director with the current pitch", "Flight Director Pitch Sync")]
    [TouchPortalState("AutoPilotFlightDirectorCurrentPitch", "text", "Flight Director Current Pitch", "")]
    public static readonly SimVarItem SYNC_FLIGHT_DIRECTOR_PITCH =
      new SimVarItem { Def = Definition.AutoPilotFlightDirectorCurrentPitch, SimVarName = "AUTOPILOT FLIGHT DIRECTOR PITCH", Unit = Units.radians, CanSet = false };

    [TouchPortalState("AutoPilotFlightDirectorCurrentBank", "text", "Flight Director Current Bank", "")]
    public static readonly SimVarItem SYNC_FLIGHT_DIRECTOR_Bank =
  new SimVarItem { Def = Definition.AutoPilotFlightDirectorCurrentBank, SimVarName = "AUTOPILOT FLIGHT DIRECTOR BANK", Unit = Units.radians, CanSet = false };

    #endregion

    #region Wing Leveler

    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotWingLeveler", "Wing Leveler", "MSFS", "Toggle/On/Off the Wing Leveler for auto pilot", "Wing Leveler - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" }, "Toggle")]
    [TouchPortalState("AutoPilotWingLeveler", "text", "AutoPilot Wing Leveler", "")]
    public static readonly SimVarItem AP_WING_LEVELER =
      new SimVarItem { Def = Definition.AutoPilotWingLeveler, SimVarName = "AUTOPILOT WING LEVELER", Unit = Units.Bool, CanSet = false };

    #endregion

    #region Localizer

    // TODO: Localizer state?
    [TouchPortalAction("AutoPilotLocalizer", "Localizer", "MSFS", "Toggle/On/Off the localizer for auto pilot", "Localizer - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" }, "Toggle")]
    public static object AP_LOCALIZER { get; }

    #endregion

    #region Yaw Dampener

    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotYawDampener", "Yaw Dampener", "MSFS", "Toggle/On/Off/Set the Yaw Dampener", "Yaw Dampener - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off", "Set" }, "Toggle")]
    [TouchPortalState("AutoPilotYawDampener", "text", "Yaw Dampener Status", "")]
    public static readonly SimVarItem AP_YAWDAMPENER =
      new SimVarItem { Def = Definition.AutoPilotYawDampener, SimVarName = "AUTOPILOT YAW DAMPER", Unit = Units.Bool, CanSet = false };

    #endregion

    #region Flight Level Control

    #endregion
  }

  [SimNotificationGroup(Groups.AutoPilot)]
  [TouchPortalCategoryMapping("AutoPilot")]
  internal enum AutoPilot {
    // Placeholder to offset each enum for SimConnect
    Init = 1000,

    // TODO: ??
    AP_N1_HOLD,
    AP_N1_REF_INC,
    AP_N1_REF_DEC,
    AP_N1_REF_SET,
    FLY_BY_WIRE_ELAC_TOGGLE,
    FLY_BY_WIRE_FAC_TOGGLE,
    FLY_BY_WIRE_SEC_TOGGLE,

    AP_PANEL_SPEED_HOLD_TOGGLE, // With current speed
    AP_PANEL_MACH_HOLD_TOGGLE, // WIth Current Mach

    // Doesn't set value
    AP_PANEL_ALTITUDE_HOLD,
    AP_PANEL_ALTITUDE_ON,
    AP_PANEL_ALTITUDE_OFF,
    AP_PANEL_ALTITUDE_SET,
    AP_PANEL_HEADING_HOLD,
    AP_PANEL_HEADING_ON,
    AP_PANEL_HEADING_OFF,
    AP_PANEL_HEADING_SET,
    AP_PANEL_MACH_HOLD,
    AP_PANEL_MACH_ON,
    AP_PANEL_MACH_OFF,
    AP_PANEL_MACH_SET,
    AP_PANEL_SPEED_HOLD,
    AP_PANEL_SPEED_ON,
    AP_PANEL_SPEED_OFF,
    AP_PANEL_SPEED_SET,
    AP_PANEL_VS_OFF,
    AP_PANEL_VS_ON,
    AP_PANEL_VS_SET,

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

    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotAttitudeVar", "Increase")]
    AP_PITCH_REF_INC_UP,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotAttitudeVar", "Decrease")]
    AP_PITCH_REF_INC_DN,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotAttitudeVar", "Set")]
    AP_PITCH_REF_SELECT,

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

    #region Bank

    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotBanking", "Increase")]
    AP_MAX_BANK_INC,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotBanking", "Decrease")]
    AP_MAX_BANK_DEC,

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
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotAltitudeVar", "Set")]
    AP_ALT_VAR_SET_ENGLISH,
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotAltitudeVar", "Set Metric")]
    AP_ALT_VAR_SET_METRIC,

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
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotNavSelect")]
    AP_NAV_SELECT_SET,

    #endregion

    #region Vertical Speed

    // Vertical Speed
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotVerticalSpeed", "Toggle")]
    AP_PANEL_VS_HOLD,
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
    [TouchPortalActionMapping("AutoPilotVerticalSpeedVar", "Set Metric")]
    AP_VS_VAR_SET_METRIC,
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

    #region AutoThrottle

    [SimActionEvent]
    [TouchPortalActionMapping("AutoThrottle", "Arm")]
    AUTO_THROTTLE_ARM,

    [SimActionEvent]
    [TouchPortalActionMapping("AutoThrottle", "GoAround")]
    AUTO_THROTTLE_TO_GA,

    #endregion

    #region AutoBrake

    [SimActionEvent]
    [TouchPortalActionMapping("AutoBrake", "Increase")]
    INCREASE_AUTOBRAKE_CONTROL,

    [SimActionEvent]
    [TouchPortalActionMapping("AutoBrake", "Decrease")]
    DECREASE_AUTOBRAKE_CONTROL,

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
    [SimActionEvent]
    [TouchPortalActionMapping("AutoPilotFlightDirectorCurrentPitch")]
    SYNC_FLIGHT_DIRECTOR_PITCH,

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
