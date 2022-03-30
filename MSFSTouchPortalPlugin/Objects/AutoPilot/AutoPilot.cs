using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Enums;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.AutoPilot
{
  [SimVarDataRequestGroup]
  [SimNotificationGroup(Groups.AutoPilot)]
  [TouchPortalCategory("AutoPilot", "MSFS - AutoPilot")]
  internal static class AutoPilotMapping {

    #region AutoPilot Master

    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotMaster", "AutoPilot", "MSFS", "Toggle/On/Off Auto Pilot", "Auto Pilot Master - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("AP_MASTER", "Toggle")]
    [TouchPortalActionMapping("AUTOPILOT_ON", "On")]
    [TouchPortalActionMapping("AUTOPILOT_OFF", "Off")]
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
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("AP_ATT_HOLD", "Toggle")]
    [TouchPortalActionMapping("AP_ATT_HOLD_ON", "On")]
    [TouchPortalActionMapping("AP_ATT_HOLD_OFF", "Off")]
    [TouchPortalState("AutoPilotAttitudeHold", "text", "AutoPilot Attitude Status", "")]
    public static readonly SimVarItem AP_ATTITUDE =
      new SimVarItem { Def = Definition.AutoPilotAttitudeHold, SimVarName = "AUTOPILOT ATTITUDE HOLD", Unit = Units.Bool, CanSet = false };

    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotAttitudeVar", "Attitude Hold Pitch Value", "MSFS", "Sets the attitude hold pitch value", "Attitude Hold Pitch Value - {0}", true)]
    [TouchPortalActionChoice(new [] { "Select", "Increase", "Decrease" })]
    [TouchPortalActionMapping("AP_PITCH_REF_INC_UP", "Increase")]
    [TouchPortalActionMapping("AP_PITCH_REF_INC_DN", "Decrease")]
    [TouchPortalActionMapping("AP_PITCH_REF_SELECT", "Select")]
    [TouchPortalState("AutoPilotAttitudeVar", "text", "AutoPilot Pitch Reference Value", "")]
    public static readonly SimVarItem AP_ATTITUDE_PITCH =
      new SimVarItem { Def = Definition.AutoPilotAttitudeVar, SimVarName = "AUTOPILOT PITCH HOLD REF", Unit = Units.radians, CanSet = false };

    [TouchPortalAction("AutoPilotAttitudeSet", "Attitude Hold Pitch Value Set", "MSFS", "Sets the airspeed value", "Set Attitude Hold Pitch Value to {0} (-16383 - +16383)")]
    [TouchPortalActionText("0", -16383, 16383)]
    [TouchPortalActionMapping("AP_PITCH_REF_SET")]
    public static object AP_ATTITUDE_PITCH_SET { get; }

    #endregion

    #region Approach

    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotApproach", "Approach Mode", "MSFS", "Toggle/On/Off the approach mode for auto pilot", "Approach Mode - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("AP_APR_HOLD", "Toggle")]
    [TouchPortalActionMapping("AP_APR_HOLD_ON", "On")]
    [TouchPortalActionMapping("AP_APR_HOLD_OFF", "Off")]
    [TouchPortalState("AutoPilotApproachHold", "text", "AutoPilot Approach Status", "")]
    public static readonly SimVarItem AP_APPROACH =
      new SimVarItem { Def = Definition.AutoPilotApproachHold, SimVarName = "AUTOPILOT APPROACH HOLD", Unit = Units.Bool, CanSet = false };

    #endregion

    #region Bank

    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotBanking", "AP Max Bank Angle", "MSFS", "Increase/Decrease the max bank angle", "Max Bank Angle - {0}", true)]
    [TouchPortalActionChoice(new [] { "Increase", "Decrease" })]
    [TouchPortalActionMapping("AP_MAX_BANK_INC", "Increase")]
    [TouchPortalActionMapping("AP_MAX_BANK_DEC", "Decrease")]
    [TouchPortalState("AutoPilotBanking", "text", "AutoPilot Max Bank Angle", "")]
    public static readonly SimVarItem AP_MAX_BANK =
      new SimVarItem { Def = Definition.AutoPilotBanking, SimVarName = "AUTOPILOT MAX BANK", Unit = Units.radians, CanSet = false };

    #endregion

    #region Heading

    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotHeading", "Heading Hold", "MSFS", "Toggle/On/Off the heading hold for auto pilot", "Heading Hold - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("AP_HDG_HOLD",     "Toggle")]
    [TouchPortalActionMapping("AP_HDG_HOLD_ON",  "On")]
    [TouchPortalActionMapping("AP_HDG_HOLD_OFF", "Off")]
    [TouchPortalState("AutoPilotHeadingHold", "text", "AutoPilot Heading Status", "")]
    public static readonly SimVarItem AP_HEADING =
      new SimVarItem { Def = Definition.AutoPilotHeadingHold, SimVarName = "AUTOPILOT HEADING LOCK", Unit = Units.Bool, CanSet = false };

    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotHeadingVar", "Heading Hold Value Adj/Sel", "MSFS", "Adjusts the heading hold value", "Heading Hold Value - {0}", true)]
    [TouchPortalActionChoice(new [] { "Select", "Increase", "Decrease" })]
    [TouchPortalActionMapping("HEADING_BUG_INC",    "Increase")]
    [TouchPortalActionMapping("HEADING_BUG_DEC",    "Decrease")]
    [TouchPortalActionMapping("HEADING_BUG_SELECT", "Select")]
    [TouchPortalState("AutoPilotHeadingVar", "text", "AutoPilot Heading Direction", "")]
    public static readonly SimVarItem AP_HEADING_VAR =
      new SimVarItem { Def = Definition.AutoPilotHeadingVar, SimVarName = "AUTOPILOT HEADING LOCK DIR", Unit = Units.degrees, CanSet = false, StringFormat = "{0:F0}" };

    [TouchPortalAction("AutoPilotHeadingSet", "Heading Hold Value Set", "MSFS", "Sets the heading hold value", "Heading Hold Value - {0}")]
    [TouchPortalActionText("1", 0, 359)]
    [TouchPortalActionMapping("HEADING_BUG_SET")]
    public static object AP_HEADING_SET { get; }

    #endregion

    #region Altitude

    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotAltitude", "Altitude Hold", "MSFS", "Toggle/On/Off the altitude hold for auto pilot", "Altitude Hold - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("AP_ALT_HOLD", "Toggle")]
    [TouchPortalActionMapping("AP_ALT_HOLD_ON", "On")]
    [TouchPortalActionMapping("AP_ALT_HOLD_OFF", "Off")]
    [TouchPortalState("AutoPilotAltitudeHold", "text", "AutoPilot Altitude Status", "")]
    public static readonly SimVarItem AP_ALTITUDE =
      new SimVarItem { Def = Definition.AutoPilotAltitudeHold, SimVarName = "AUTOPILOT ALTITUDE LOCK", Unit = Units.Bool, CanSet = false };


    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotAltitudeVar", "Altitude Hold Value Adj/Sel", "MSFS", "Adjusts or selects the altitude hold value", "Altitude Hold Value - {0}", true)]
    [TouchPortalActionChoice(new [] { "Select", "Increase", "Decrease" })]
    [TouchPortalActionMapping("AP_ALT_VAR_INC", "Increase")]
    [TouchPortalActionMapping("AP_ALT_VAR_DEC", "Decrease")]
    [TouchPortalActionMapping("ALTITUDE_BUG_SELECT", "Select")]
    [TouchPortalState("AutoPilotAltitudeVar", "text", "AutoPilot Altitude Value", "")]
    public static readonly SimVarItem AP_ALTITUDE_VAR =
      new SimVarItem { Def = Definition.AutoPilotAltitudeVar, SimVarName = "AUTOPILOT ALTITUDE LOCK VAR", Unit = Units.feet, CanSet = false };

    [TouchPortalAction("AutoPilotAltitudeSet", "Altitude Hold Value Set", "MSFS", "Sets the altitude hold value", "Altitude Hold Value - {0} to {1}")]
    [TouchPortalActionChoice(new[] { "Set English", "Set Metric" })]
    [TouchPortalActionText("0")]
    [TouchPortalActionMapping("AP_ALT_VAR_SET_ENGLISH", "Set English")]
    [TouchPortalActionMapping("AP_ALT_VAR_SET_METRIC", "Set Metric")]
    public static object AP_ALTITUDE_SET { get; }

    #endregion

    #region Back Course

    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotBackCourse", "Back Course Mode", "MSFS", "Toggle/On/Off the back course mode for auto pilot", "Back Course Mode - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("AP_BC_HOLD", "Toggle")]
    [TouchPortalActionMapping("AP_BC_HOLD_ON", "On")]
    [TouchPortalActionMapping("AP_BC_HOLD_OFF", "Off")]
    [TouchPortalState("AutoPilotBackCourseHold", "text", "AutoPilot Back Course Status", "")]
    public static readonly SimVarItem AP_BACKCOURSE =
      new SimVarItem { Def = Definition.AutoPilotBackCourseHold, SimVarName = "AUTOPILOT BACKCOURSE HOLD", Unit = Units.Bool, CanSet = false };

    #endregion

    #region Nav1

    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotNav1", "Nav1 Mode", "MSFS", "Toggle/On/Off the Nav1 mode for auto pilot", "Nav1 Mode - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("AP_NAV1_HOLD", "Toggle")]
    [TouchPortalActionMapping("AP_NAV1_HOLD_ON", "On")]
    [TouchPortalActionMapping("AP_NAV1_HOLD_OFF", "Off")]
    [TouchPortalState("AutoPilotNav1Hold", "text", "AutoPilot Nav1 Status", "")]
    public static readonly SimVarItem AP_NAV1 =
      new SimVarItem { Def = Definition.AutoPilotNav1Hold, SimVarName = "AUTOPILOT NAV1 LOCK", Unit = Units.Bool, CanSet = false };

    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotNavSelect", "Nav Mode - Set", "MSFS", "Sets the nav to 1 or 2 for Nav mode", "Nav Mode - {0} ")]
    [TouchPortalActionNumeric(1, 1, 2)]
    [TouchPortalActionMapping("AP_NAV_SELECT_SET")]
    [TouchPortalState("AutoPilotNavSelected", "text", "AutoPilot Nav Selected Index", "")]
    public static readonly SimVarItem AP_NAV_SELECT_SET =
      new SimVarItem { Def = Definition.AutoPilotNavSelected, SimVarName = "AUTOPILOT NAV SELECTED", Unit = Units.number, CanSet = false };

    #endregion

    #region Vertical Speed

    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotVerticalSpeed", "Vertical Speed", "MSFS", "Toggle the Vertical Speed for auto pilot", "Vertical Speed - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("AP_VS_HOLD", "Toggle")]
    [TouchPortalActionMapping("AP_VS_ON", "On")]
    [TouchPortalActionMapping("AP_VS_OFF", "Off")]
    [TouchPortalState("AutoPilotVerticalSpeedHold", "text", "AutoPilot Vertical Speed Status", "")]
    public static readonly SimVarItem AutoPilotVerticalSpeedHold =
      new SimVarItem { Def = Definition.AutoPilotVerticalSpeedHold, SimVarName = "AUTOPILOT VERTICAL HOLD", Unit = Units.Bool, CanSet = false };

    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotVerticalSpeedVar", "Vertical Speed Value", "MSFS", "Adjusts or selects the vertical speed value", "Vertical Speed Value - {0}", true)]
    [TouchPortalActionChoice(new [] { "Select", "Increase", "Decrease", "Set Current" })]
    [TouchPortalActionMapping("VSI_BUG_SELECT", "Select")]
    [TouchPortalActionMapping("AP_VS_VAR_INC", "Increase")]
    [TouchPortalActionMapping("AP_VS_VAR_DEC", "Decrease")]
    [TouchPortalActionMapping("AP_VS_VAR_SET_CURRENT", "Set Current")]
    [TouchPortalState("AutoPilotVerticalSpeedVar", "text", "AutoPilot Vertical Speed Value", "")]
    public static readonly SimVarItem AP_VERTICALSPEED_VAR =
      new SimVarItem { Def = Definition.AutoPilotVerticalSpeedVar, SimVarName = "AUTOPILOT VERTICAL HOLD VAR", Unit = Units.feetminute, CanSet = false };

    [TouchPortalAction("AutoPilotVerticalSpeedSet", "Vertical Speed Value Set", "MSFS", "Sets the vertical speed value", "Vertical Speed Hold Value - {0} to {1}")]
    [TouchPortalActionChoice(new[] { "Set English", "Set Metric" })]
    [TouchPortalActionText("1", -5000, 5000)]
    [TouchPortalActionMapping("AP_VS_VAR_SET_ENGLISH", "Set English")]
    [TouchPortalActionMapping("AP_VS_VAR_SET_METRIC", "Set Metric")]
    public static object AP_VERTICALSPEED_SET { get; }

    #endregion

    #region Airspeed

    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotAirSpeed", "Airspeed Hold", "MSFS", "Toggle/On/Off the airspeed hold for auto pilot", "Airspeed Hold - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("AP_AIRSPEED_HOLD", "Toggle")]
    [TouchPortalActionMapping("AP_AIRSPEED_ON", "On")]
    [TouchPortalActionMapping("AP_AIRSPEED_OFF", "Off")]
    [TouchPortalState("AutoPilotAirSpeedHold", "text", "AutoPilot Air Speed Status", "")]
    public static readonly SimVarItem AP_AIRSPEED =
      new SimVarItem { Def = Definition.AutoPilotAirSpeedHold, SimVarName = "AUTOPILOT AIRSPEED HOLD", Unit = Units.Bool, CanSet = false };

    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotAirSpeedVar", "Airspeed Hold Value", "MSFS", "Adjusts the airspeed hold value", "Airspeed Hold Value - {0}", true)]
    [TouchPortalActionChoice(new [] { "Select", "Increase", "Decrease" })]
    [TouchPortalActionMapping("AIRSPEED_BUG_SELECT", "Select")]
    [TouchPortalActionMapping("AP_SPD_VAR_INC", "Increase")]
    [TouchPortalActionMapping("AP_SPD_VAR_DEC", "Decrease")]
    [TouchPortalState("AutoPilotAirSpeedVar", "text", "AutoPilot Air Speed Value", "")]
    public static readonly SimVarItem AP_AIRSPEED_VAR =
      new SimVarItem { Def = Definition.AutoPilotAirSpeedVar, SimVarName = "AUTOPILOT AIRSPEED HOLD VAR", Unit = Units.knots, CanSet = false };

    [TouchPortalAction("AutoPilotAirSpeedSet", "Airspeed Value Set", "MSFS", "Sets the airspeed value", "Set Airspeed Hold Value to {0}")]
    [TouchPortalActionText("0", 0, 5000)]
    [TouchPortalActionMapping("AP_SPD_VAR_SET")]
    public static object AP_AIRSPEED_SET { get; }

    #endregion

    #region AutoThrottle

    [SimVarDataRequest]
    [TouchPortalAction("AutoThrottle", "Auto Throttle Mode", "MSFS", "Toggles the Arm/GoAround modes for auto throttle", "Toggle Auto Throttle - {0}")]
    [TouchPortalActionChoice(new [] { "Arm", "GoAround" })]
    [TouchPortalActionMapping("AUTO_THROTTLE_ARM", "Arm")]
    [TouchPortalActionMapping("AUTO_THROTTLE_TO_GA", "GoAround")]
    [TouchPortalState("AutoThrottleArm", "text", "Auto Throttle Armed", "")]
    public static readonly SimVarItem AUTO_THROTTLE =
      new SimVarItem { Def = Definition.AutoThrottleArm, SimVarName = "AUTOPILOT THROTTLE ARM", Unit = Units.Bool, CanSet = false };

    [SimVarDataRequest]
    [TouchPortalState("AutoThrottleGoAround", "text", "Auto Throttle GoAround", "")]
    public static readonly SimVarItem AUTO_THROTTLE_GA =
      new SimVarItem { Def = Definition.AutoThrottleGA, SimVarName = "AUTOPILOT TAKEOFF POWER ACTIVE", Unit = Units.Bool, CanSet = false };


    #endregion

    #region AutoBrake

    [TouchPortalAction("AutoBrake", "Auto Brake", "MSFS", "Increase/Decrease the auto brake", "Auto Brake - {0}", true)]
    [TouchPortalActionChoice(new [] { "Increase", "Decrease" })]
    [TouchPortalActionMapping("INCREASE_AUTOBRAKE_CONTROL", "Increase")]
    [TouchPortalActionMapping("DECREASE_AUTOBRAKE_CONTROL", "Decrease")]
    public static object AUTO_BRAKE { get; }

    #endregion

    #region Mach

    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotMach", "Mach Hold", "MSFS", "Toggle/On/Off the mach hold for auto pilot", "Mach Hold - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off"})]
    [TouchPortalActionMapping("AP_MACH_HOLD", "Toggle")]
    [TouchPortalActionMapping("AP_MACH_ON", "On")]
    [TouchPortalActionMapping("AP_MACH_OFF", "Off")]
    [TouchPortalState("AutoPilotMach", "text", "AutoPilot Mach Hold", "")]
    public static readonly SimVarItem AP_MACH =
      new SimVarItem { Def = Definition.AutoPilotMach, SimVarName = "AUTOPILOT MACH HOLD", Unit = Units.Bool, CanSet = false };

    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotMachVar", "Mach Hold Value", "MSFS", "Sets the mach hold value", "Mach Hold Value - {0}")]
    [TouchPortalActionChoice(new [] { "Select", "Increase", "Decrease" }, "Increase")]
    [TouchPortalActionMapping("AP_MACH_VAR_INC", "Increase")]
    [TouchPortalActionMapping("AP_MACH_VAR_DEC", "Decrease")]
    [TouchPortalState("AutoPilotMachVar", "text", "AutoPilot Mach Value", "")]
    public static readonly SimVarItem AP_MACH_VAR =
      new SimVarItem { Def = Definition.AutoPilotMachVar, SimVarName = "AUTOPILOT MACH HOLD VAR", Unit = Units.number, CanSet = false };

    [TouchPortalAction("AutoPilotMachSet", "Mach Hold Value Set", "MSFS", "Sets the mach hold value", "Set Mach Hold Value to {0}")]
    [TouchPortalActionText("0", 0, 20)]
    [TouchPortalActionMapping("AP_MACH_VAR_SET")]
    public static object AP_MACH_SET { get; }

    #endregion

    #region Flight Director

    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotFlightDirector", "Flight Director", "MSFS", "Toggle the Flight Director for auto pilot", "Toggle Flight Director On/Off")]
    [TouchPortalActionMapping("TOGGLE_FLIGHT_DIRECTOR")]
    [TouchPortalState("AutoPilotFlightDirector", "text", "AutoPilot Flight Director Status", "")]
    public static readonly SimVarItem AP_FLIGHT_DIRECTOR =
      new SimVarItem { Def = Definition.AutoPilotFlightDirector, SimVarName = "AUTOPILOT FLIGHT DIRECTOR ACTIVE", Unit = Units.Bool, CanSet = false };

    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotFlightDirectorCurrentPitch", "Flight Director Pitch Sync", "MSFS", "Syncs the Flight Director with the current pitch", "Flight Director Pitch Sync")]
    [TouchPortalActionMapping("SYNC_FLIGHT_DIRECTOR_PITCH")]
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
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("AP_WING_LEVELER", "Toggle")]
    [TouchPortalActionMapping("AP_WING_LEVELER_ON", "On")]
    [TouchPortalActionMapping("AP_WING_LEVELER_OFF", "Off")]
    [TouchPortalState("AutoPilotWingLeveler", "text", "AutoPilot Wing Leveler", "")]
    public static readonly SimVarItem AP_WING_LEVELER =
      new SimVarItem { Def = Definition.AutoPilotWingLeveler, SimVarName = "AUTOPILOT WING LEVELER", Unit = Units.Bool, CanSet = false };

    #endregion

    #region Localizer

    // TODO: Localizer state?
    [TouchPortalAction("AutoPilotLocalizer", "Localizer", "MSFS", "Toggle/On/Off the localizer for auto pilot", "Localizer - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("AP_LOC_HOLD", "Toggle")]
    [TouchPortalActionMapping("AP_LOC_HOLD_ON", "On")]
    [TouchPortalActionMapping("AP_LOC_HOLD_OFF", "Off")]
    public static object AP_LOCALIZER { get; }

    #endregion

    #region Yaw Dampener

    [SimVarDataRequest]
    [TouchPortalAction("AutoPilotYawDampener", "Yaw Dampener", "MSFS", "Toggle/On/Off the Yaw Dampener", "Yaw Dampener - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("YAW_DAMPER_TOGGLE", "Toggle")]
    [TouchPortalActionMapping("YAW_DAMPER_ON", "On")]
    [TouchPortalActionMapping("YAW_DAMPER_OFF", "Off")]
    [TouchPortalState("AutoPilotYawDampener", "text", "Yaw Dampener Status", "")]
    public static readonly SimVarItem AP_YAWDAMPENER =
      new SimVarItem { Def = Definition.AutoPilotYawDampener, SimVarName = "AUTOPILOT YAW DAMPER", Unit = Units.Bool, CanSet = false };

    #endregion

    #region Flight Level Control

    #endregion


    #region Todo ??

    //AP_N1_HOLD,
    //AP_N1_REF_INC,
    //AP_N1_REF_DEC,
    //AP_N1_REF_SET,
    //FLY_BY_WIRE_ELAC_TOGGLE,
    //FLY_BY_WIRE_FAC_TOGGLE,
    //FLY_BY_WIRE_SEC_TOGGLE,

    //AP_PANEL_SPEED_HOLD_TOGGLE, // With current speed
    //AP_PANEL_MACH_HOLD_TOGGLE, // WIth Current Mach

    //// Doesn't set value
    //AP_PANEL_ALTITUDE_HOLD,
    //AP_PANEL_ALTITUDE_ON,
    //AP_PANEL_ALTITUDE_OFF,
    //AP_PANEL_ALTITUDE_SET,
    //AP_PANEL_HEADING_HOLD,
    //AP_PANEL_HEADING_ON,
    //AP_PANEL_HEADING_OFF,
    //AP_PANEL_HEADING_SET,
    //AP_PANEL_MACH_HOLD,
    //AP_PANEL_MACH_ON,
    //AP_PANEL_MACH_OFF,
    //AP_PANEL_MACH_SET,
    //AP_PANEL_SPEED_HOLD,
    //AP_PANEL_SPEED_ON,
    //AP_PANEL_SPEED_OFF,
    //AP_PANEL_SPEED_SET,
    //AP_PANEL_VS_OFF,
    //AP_PANEL_VS_ON,
    //AP_PANEL_VS_SET,

    #endregion

  }
}
