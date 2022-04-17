using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Enums;

namespace MSFSTouchPortalPlugin.Objects.AutoPilot
{
  [TouchPortalCategory(Groups.AutoPilot)]
  internal static class AutoPilotMapping {

    #region AutoPilot Master

    [TouchPortalAction("AutoPilotMaster", "AutoPilot", "MSFS", "Toggle/On/Off Auto Pilot", "Auto Pilot Master - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("AP_MASTER", "Toggle")]
    [TouchPortalActionMapping("AUTOPILOT_ON", "On")]
    [TouchPortalActionMapping("AUTOPILOT_OFF", "Off")]
    public static readonly object AP_MASTER;

    #endregion

    #region Attitude

    [TouchPortalAction("AutoPilotAttitude", "Attitude Hold", "MSFS", "Toggle/On/Off the attitude hold for auto pilot", "Attitude Hold - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("AP_ATT_HOLD", "Toggle")]
    [TouchPortalActionMapping("AP_ATT_HOLD_ON", "On")]
    [TouchPortalActionMapping("AP_ATT_HOLD_OFF", "Off")]
    public static readonly object AP_ATTITUDE;

    [TouchPortalAction("AutoPilotAttitudeVar", "Attitude Hold Pitch Value", "MSFS", "Sets the attitude hold pitch value", "Attitude Hold Pitch Value - {0}", true)]
    [TouchPortalActionChoice(new [] { "Select", "Increase", "Decrease" })]
    [TouchPortalActionMapping("AP_PITCH_REF_INC_UP", "Increase")]
    [TouchPortalActionMapping("AP_PITCH_REF_INC_DN", "Decrease")]
    [TouchPortalActionMapping("AP_PITCH_REF_SELECT", "Select")]
    public static readonly object AP_ATTITUDE_PITCH;

    [TouchPortalAction("AutoPilotAttitudeSet", "Attitude Hold Pitch Value Set", "MSFS", "Sets the airspeed value", "Set Attitude Hold Pitch Value to {0} (-16384 to +16384)", true)]
    [TouchPortalActionText("0", -16384, 16384)]
    [TouchPortalActionMapping("AP_PITCH_REF_SET")]
    public static readonly object AP_ATTITUDE_PITCH_SET;

    #endregion

    #region Approach

    [TouchPortalAction("AutoPilotApproach", "Approach Mode", "MSFS", "Toggle/On/Off the approach mode for auto pilot", "Approach Mode - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("AP_APR_HOLD", "Toggle")]
    [TouchPortalActionMapping("AP_APR_HOLD_ON", "On")]
    [TouchPortalActionMapping("AP_APR_HOLD_OFF", "Off")]
    public static readonly object AP_APPROACH;

    #endregion

    #region Bank

    [TouchPortalAction("AutoPilotBanking", "AP Max Bank Angle", "MSFS", "Increase/Decrease the max bank angle", "Max Bank Angle - {0}", true)]
    [TouchPortalActionChoice(new [] { "Increase", "Decrease" })]
    [TouchPortalActionMapping("AP_MAX_BANK_INC", "Increase")]
    [TouchPortalActionMapping("AP_MAX_BANK_DEC", "Decrease")]
    public static readonly object AP_MAX_BANK;

    #endregion

    #region Heading

    [TouchPortalAction("AutoPilotHeading", "Heading Hold", "MSFS", "Toggle/On/Off the heading hold for auto pilot", "Heading Hold - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("AP_HDG_HOLD",     "Toggle")]
    [TouchPortalActionMapping("AP_HDG_HOLD_ON",  "On")]
    [TouchPortalActionMapping("AP_HDG_HOLD_OFF", "Off")]
    public static readonly object AP_HEADING;

    [TouchPortalAction("AutoPilotHeadingVar", "Heading Hold Value Adj/Sel", "MSFS", "Adjusts the heading hold value", "Heading Hold Value - {0}", true)]
    [TouchPortalActionChoice(new [] { "Select", "Increase", "Decrease" })]
    [TouchPortalActionMapping("HEADING_BUG_INC",    "Increase")]
    [TouchPortalActionMapping("HEADING_BUG_DEC",    "Decrease")]
    [TouchPortalActionMapping("HEADING_BUG_SELECT", "Select")]
    public static readonly object AP_HEADING_VAR;

    [TouchPortalAction("AutoPilotHeadingSet", "Heading Hold Value Set", "MSFS", "Sets the heading hold value", "Heading Hold Value - {0}", true)]
    [TouchPortalActionText("1", 0, 359)]
    [TouchPortalActionMapping("HEADING_BUG_SET")]
    public static readonly object AP_HEADING_SET;

    #endregion

    #region Altitude

    [TouchPortalAction("AutoPilotAltitude", "Altitude Hold", "MSFS", "Toggle/On/Off the altitude hold for auto pilot", "Altitude Hold - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("AP_ALT_HOLD", "Toggle")]
    [TouchPortalActionMapping("AP_ALT_HOLD_ON", "On")]
    [TouchPortalActionMapping("AP_ALT_HOLD_OFF", "Off")]
    public static readonly object AP_ALTITUDE;


    [TouchPortalAction("AutoPilotAltitudeVar", "Altitude Hold Value Adj/Sel", "MSFS", "Adjusts or selects the altitude hold value", "Altitude Hold Value - {0}", true)]
    [TouchPortalActionChoice(new [] { "Select", "Increase", "Decrease" })]
    [TouchPortalActionMapping("AP_ALT_VAR_INC", "Increase")]
    [TouchPortalActionMapping("AP_ALT_VAR_DEC", "Decrease")]
    [TouchPortalActionMapping("ALTITUDE_BUG_SELECT", "Select")]
    public static readonly object AP_ALTITUDE_VAR;

    [TouchPortalAction("AutoPilotAltitudeSet", "Altitude Hold Value Set", "MSFS", "Sets the altitude hold value", "Altitude Hold Value - {0} to {1}", true)]
    [TouchPortalActionChoice(new[] { "Set English", "Set Metric" })]
    [TouchPortalActionText("0")]
    [TouchPortalActionMapping("AP_ALT_VAR_SET_ENGLISH", "Set English")]
    [TouchPortalActionMapping("AP_ALT_VAR_SET_METRIC", "Set Metric")]
    public static readonly object AP_ALTITUDE_SET;

    #endregion

    #region Back Course

    [TouchPortalAction("AutoPilotBackCourse", "Back Course Mode", "MSFS", "Toggle/On/Off the back course mode for auto pilot", "Back Course Mode - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("AP_BC_HOLD", "Toggle")]
    [TouchPortalActionMapping("AP_BC_HOLD_ON", "On")]
    [TouchPortalActionMapping("AP_BC_HOLD_OFF", "Off")]
    public static readonly object AP_BACKCOURSE;

    #endregion

    #region Nav1

    [TouchPortalAction("AutoPilotNav1", "Nav1 Mode", "MSFS", "Toggle/On/Off the Nav1 mode for auto pilot", "Nav1 Mode - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("AP_NAV1_HOLD", "Toggle")]
    [TouchPortalActionMapping("AP_NAV1_HOLD_ON", "On")]
    [TouchPortalActionMapping("AP_NAV1_HOLD_OFF", "Off")]
    public static readonly object AP_NAV1;

    [TouchPortalAction("AutoPilotNavSelect", "Nav Mode - Set", "MSFS", "Sets the nav to 1 or 2 for Nav mode", "Nav Mode - {0} ")]
    [TouchPortalActionNumeric(1, 1, 2)]
    [TouchPortalActionMapping("AP_NAV_SELECT_SET")]
    public static readonly object AP_NAV_SELECT_SET;

    #endregion

    #region Vertical Speed

    [TouchPortalAction("AutoPilotVerticalSpeed", "Vertical Speed", "MSFS", "Toggle the Vertical Speed for auto pilot", "Vertical Speed - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("AP_VS_HOLD", "Toggle")]
    [TouchPortalActionMapping("AP_VS_ON", "On")]
    [TouchPortalActionMapping("AP_VS_OFF", "Off")]
    public static readonly object AutoPilotVerticalSpeedHold;

    [TouchPortalAction("AutoPilotVerticalSpeedVar", "Vertical Speed Value", "MSFS", "Adjusts or selects the vertical speed value", "Vertical Speed Value - {0}", true)]
    [TouchPortalActionChoice(new [] { "Select", "Increase", "Decrease", "Set Current" })]
    [TouchPortalActionMapping("VSI_BUG_SELECT", "Select")]
    [TouchPortalActionMapping("AP_VS_VAR_INC", "Increase")]
    [TouchPortalActionMapping("AP_VS_VAR_DEC", "Decrease")]
    [TouchPortalActionMapping("AP_VS_VAR_SET_CURRENT", "Set Current")]
    public static readonly object AP_VERTICALSPEED_VAR;

    [TouchPortalAction("AutoPilotVerticalSpeedSet", "Vertical Speed Value Set", "MSFS", "Sets the vertical speed value", "Vertical Speed Hold Value - {0} to {1}", true)]
    [TouchPortalActionChoice(new[] { "Set English", "Set Metric" })]
    [TouchPortalActionText("1", -5000, 5000)]
    [TouchPortalActionMapping("AP_VS_VAR_SET_ENGLISH", "Set English")]
    [TouchPortalActionMapping("AP_VS_VAR_SET_METRIC", "Set Metric")]
    public static readonly object AP_VERTICALSPEED_SET;

    #endregion

    #region Airspeed

    [TouchPortalAction("AutoPilotAirSpeed", "Airspeed Hold", "MSFS", "Toggle/On/Off the airspeed hold for auto pilot", "Airspeed Hold - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("AP_AIRSPEED_HOLD", "Toggle")]
    [TouchPortalActionMapping("AP_AIRSPEED_ON", "On")]
    [TouchPortalActionMapping("AP_AIRSPEED_OFF", "Off")]
    public static readonly object AP_AIRSPEED;

    [TouchPortalAction("AutoPilotAirSpeedVar", "Airspeed Hold Value", "MSFS", "Adjusts the airspeed hold value", "Airspeed Hold Value - {0}", true)]
    [TouchPortalActionChoice(new [] { "Select", "Increase", "Decrease" })]
    [TouchPortalActionMapping("AIRSPEED_BUG_SELECT", "Select")]
    [TouchPortalActionMapping("AP_SPD_VAR_INC", "Increase")]
    [TouchPortalActionMapping("AP_SPD_VAR_DEC", "Decrease")]
    public static readonly object AP_AIRSPEED_VAR;

    [TouchPortalAction("AutoPilotAirSpeedSet", "Airspeed Value Set", "MSFS", "Sets the airspeed value", "Set Airspeed Hold Value to {0}", true)]
    [TouchPortalActionText("0", 0, 5000)]
    [TouchPortalActionMapping("AP_SPD_VAR_SET")]
    public static readonly object AP_AIRSPEED_SET;

    #endregion

    #region AutoThrottle

    [TouchPortalAction("AutoThrottle", "Auto Throttle Mode", "MSFS", "Toggles the Arm/GoAround modes for auto throttle", "Toggle Auto Throttle - {0}")]
    [TouchPortalActionChoice(new [] { "Arm", "GoAround" })]
    [TouchPortalActionMapping("AUTO_THROTTLE_ARM", "Arm")]
    [TouchPortalActionMapping("AUTO_THROTTLE_TO_GA", "GoAround")]
    public static readonly object AUTO_THROTTLE;

    #endregion

    #region AutoBrake

    [TouchPortalAction("AutoBrake", "Auto Brake", "MSFS", "Increase/Decrease the auto brake", "Auto Brake - {0}", true)]
    [TouchPortalActionChoice(new [] { "Increase", "Decrease" })]
    [TouchPortalActionMapping("INCREASE_AUTOBRAKE_CONTROL", "Increase")]
    [TouchPortalActionMapping("DECREASE_AUTOBRAKE_CONTROL", "Decrease")]
    public static readonly object AUTO_BRAKE;

    #endregion

    #region Mach

    [TouchPortalAction("AutoPilotMach", "Mach Hold", "MSFS", "Toggle/On/Off the mach hold for auto pilot", "Mach Hold - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off"})]
    [TouchPortalActionMapping("AP_MACH_HOLD", "Toggle")]
    [TouchPortalActionMapping("AP_MACH_ON", "On")]
    [TouchPortalActionMapping("AP_MACH_OFF", "Off")]
    public static readonly object AP_MACH;

    [TouchPortalAction("AutoPilotMachVar", "Mach Hold Value", "MSFS", "Sets the mach hold value", "Mach Hold Value - {0}")]
    [TouchPortalActionChoice(new [] { "Select", "Increase", "Decrease" }, "Increase")]
    [TouchPortalActionMapping("AP_MACH_VAR_INC", "Increase")]
    [TouchPortalActionMapping("AP_MACH_VAR_DEC", "Decrease")]
    public static readonly object AP_MACH_VAR;

    [TouchPortalAction("AutoPilotMachSet", "Mach Hold Value Set", "MSFS", "Sets the mach hold value", "Set Mach Hold Value to {0}", true)]
    [TouchPortalActionText("0", 0, 20)]
    [TouchPortalActionMapping("AP_MACH_VAR_SET")]
    public static readonly object AP_MACH_SET;

    #endregion

    #region Flight Director

    [TouchPortalAction("AutoPilotFlightDirector", "Flight Director", "MSFS", "Toggle the Flight Director for auto pilot", "Toggle Flight Director On/Off")]
    [TouchPortalActionMapping("TOGGLE_FLIGHT_DIRECTOR")]
    public static readonly object AP_FLIGHT_DIRECTOR;

    [TouchPortalAction("AutoPilotFlightDirectorCurrentPitch", "Flight Director Pitch Sync", "MSFS", "Syncs the Flight Director with the current pitch", "Flight Director Pitch Sync")]
    [TouchPortalActionMapping("SYNC_FLIGHT_DIRECTOR_PITCH")]
    public static readonly object SYNC_FLIGHT_DIRECTOR_PITCH;

    #endregion

    #region Wing Leveler

    [TouchPortalAction("AutoPilotWingLeveler", "Wing Leveler", "MSFS", "Toggle/On/Off the Wing Leveler for auto pilot", "Wing Leveler - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("AP_WING_LEVELER", "Toggle")]
    [TouchPortalActionMapping("AP_WING_LEVELER_ON", "On")]
    [TouchPortalActionMapping("AP_WING_LEVELER_OFF", "Off")]
    public static readonly object AP_WING_LEVELER;

    #endregion

    #region Localizer

    // TODO: Localizer state?
    [TouchPortalAction("AutoPilotLocalizer", "Localizer", "MSFS", "Toggle/On/Off the localizer for auto pilot", "Localizer - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("AP_LOC_HOLD", "Toggle")]
    [TouchPortalActionMapping("AP_LOC_HOLD_ON", "On")]
    [TouchPortalActionMapping("AP_LOC_HOLD_OFF", "Off")]
    public static readonly object AP_LOCALIZER;

    #endregion

    #region Yaw Dampener

    [TouchPortalAction("AutoPilotYawDampener", "Yaw Dampener", "MSFS", "Toggle/On/Off the Yaw Dampener", "Yaw Dampener - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("YAW_DAMPER_TOGGLE", "Toggle")]
    [TouchPortalActionMapping("YAW_DAMPER_ON", "On")]
    [TouchPortalActionMapping("YAW_DAMPER_OFF", "Off")]
    public static readonly object AP_YAWDAMPENER;

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
