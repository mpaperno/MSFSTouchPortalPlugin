/*
This file is part of the MSFS Touch Portal Plugin project.
https://github.com/mpaperno/MSFSTouchPortalPlugin

COPYRIGHT:
(c) 2020 Tim Lewis;
(c) Maxim Paperno; All Rights Reserved.

This file may be used under the terms of the GNU General Public License (GPL)
as published by the Free Software Foundation, either version 3 of the Licenses,
or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

A copy of the GNU GPL is included with this project
and is also available at <http://www.gnu.org/licenses/>.
*/

using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Enums;

namespace MSFSTouchPortalPlugin.Objects
{
  [TouchPortalCategory(Groups.AutoPilot)]
  internal static class AutoPilotMapping {

    #region Switches

    [TouchPortalAction("AutoPilotSwitches", "AP Switches", "Auto Pilot {0} Switch {1}")]
    [TouchPortalActionChoice()]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("AP_MASTER",            "Master",            "Toggle")]
    [TouchPortalActionMapping("AUTOPILOT_ON",         "Master",            "On")]
    [TouchPortalActionMapping("AUTOPILOT_OFF",        "Master",            "Off")]
    [TouchPortalActionMapping("AP_AIRSPEED_HOLD",     "Airspeed Hold",     "Toggle")]
    [TouchPortalActionMapping("AP_AIRSPEED_ON",       "Airspeed Hold",     "On")]
    [TouchPortalActionMapping("AP_AIRSPEED_OFF",      "Airspeed Hold",     "Off")]
    [TouchPortalActionMapping("AP_PANEL_SPEED_HOLD",   "Panel Airspeed Hold", "Toggle")]
    [TouchPortalActionMapping("AP_PANEL_SPEED_ON",     "Panel Airspeed Hold", "On")]
    [TouchPortalActionMapping("AP_PANEL_SPEED_OFF",    "Panel Airspeed Hold", "Off")]
    [TouchPortalActionMapping("AP_ALT_HOLD",          "Altitude Hold",     "Toggle")]
    [TouchPortalActionMapping("AP_ALT_HOLD_ON",       "Altitude Hold",     "On")]
    [TouchPortalActionMapping("AP_ALT_HOLD_OFF",      "Altitude Hold",     "Off")]
#if !FSX
    [TouchPortalActionMapping("PANEL_ALTITUDE_HOLD",  "Panel Altitude Hold", "Toggle")]
    [TouchPortalActionMapping("PANEL_ALTITUDE_ON",    "Panel Altitude Hold", "On")]
    [TouchPortalActionMapping("PANEL_ALTITUDE_OFF",   "Panel Altitude Hold", "Off")]
#endif
    [TouchPortalActionMapping("AP_APR_HOLD",          "Approach Mode",     "Toggle")]
    [TouchPortalActionMapping("AP_APR_HOLD_ON",       "Approach Mode",     "On")]
    [TouchPortalActionMapping("AP_APR_HOLD_OFF",      "Approach Mode",     "Off")]
    [TouchPortalActionMapping("AP_ATT_HOLD",          "Attitude Hold",     "Toggle")]
    [TouchPortalActionMapping("AP_ATT_HOLD_ON",       "Attitude Hold",     "On")]
    [TouchPortalActionMapping("AP_ATT_HOLD_OFF",      "Attitude Hold",     "Off")]
    [TouchPortalActionMapping("AP_BC_HOLD",           "Back Course Mode",  "Toggle")]
    [TouchPortalActionMapping("AP_BC_HOLD_ON",        "Back Course Mode",  "On")]
    [TouchPortalActionMapping("AP_BC_HOLD_OFF",       "Back Course Mode",  "Off")]
#if !FSX
    [TouchPortalActionMapping("AP_BANK_HOLD",         "Bank Mode",         "Toggle")]
    [TouchPortalActionMapping("AP_BANK_HOLD_ON",      "Bank Mode",         "On")]
    [TouchPortalActionMapping("AP_BANK_HOLD_OFF",     "Bank Mode",         "Off")]
    [TouchPortalActionMapping("AP_FLIGHT_LEVEL_CHANGE",     "Flight Level Change", "Toggle")]
    [TouchPortalActionMapping("AP_FLIGHT_LEVEL_CHANGE_ON",  "Flight Level Change", "On")]
    [TouchPortalActionMapping("AP_FLIGHT_LEVEL_CHANGE_OFF", "Flight Level Change", "Off")]
#endif
    [TouchPortalActionMapping("AP_HDG_HOLD",          "Heading Hold",      "Toggle")]
    [TouchPortalActionMapping("AP_HDG_HOLD_ON",       "Heading Hold",      "On")]
    [TouchPortalActionMapping("AP_HDG_HOLD_OFF",      "Heading Hold",      "Off")]
    [TouchPortalActionMapping("AP_PANEL_HEADING_HOLD", "Panel Heading Hold", "Toggle")]
    [TouchPortalActionMapping("AP_PANEL_HEADING_ON",   "Panel Heading Hold", "On")]
    [TouchPortalActionMapping("AP_PANEL_HEADING_OFF",  "Panel Heading Hold", "Off")]
    [TouchPortalActionMapping("AP_LOC_HOLD",          "Localizer",         "Toggle")]
    [TouchPortalActionMapping("AP_LOC_HOLD_ON",       "Localizer",         "On")]
    [TouchPortalActionMapping("AP_LOC_HOLD_OFF",      "Localizer",         "Off")]
    [TouchPortalActionMapping("AP_MACH_HOLD",         "Mach Hold",         "Toggle")]
    [TouchPortalActionMapping("AP_MACH_ON",           "Mach Hold",         "On")]
    [TouchPortalActionMapping("AP_MACH_OFF",          "Mach Hold",         "Off")]
    [TouchPortalActionMapping("AP_PANEL_MACH_HOLD",   "Panel Mach Hold",   "Toggle")]
    [TouchPortalActionMapping("AP_PANEL_MACH_ON",     "Panel Mach Hold",   "On")]
    [TouchPortalActionMapping("AP_PANEL_MACH_OFF",    "Panel Mach Hold",   "Off")]
    [TouchPortalActionMapping("AP_N1_HOLD",           "N1 Hold",         "Toggle")]
    [TouchPortalActionMapping("AP_NAV1_HOLD",         "Nav1 Hold",         "Toggle")]
    [TouchPortalActionMapping("AP_NAV1_HOLD_ON",      "Nav1 Hold",         "On")]
    [TouchPortalActionMapping("AP_NAV1_HOLD_OFF",     "Nav1 Hold",         "Off")]
    [TouchPortalActionMapping("AP_VS_HOLD",           "Vertical Speed",    "Toggle")]
    [TouchPortalActionMapping("AP_VS_ON",             "Vertical Speed",    "On")]
    [TouchPortalActionMapping("AP_VS_OFF",            "Vertical Speed",    "Off")]
    [TouchPortalActionMapping("AP_PANEL_VS_ON",       "Panel Vertical Speed", "On")]
    [TouchPortalActionMapping("AP_PANEL_VS_OFF",      "Panel Vertical Speed", "Off")]
    [TouchPortalActionMapping("AP_WING_LEVELER",      "Wing Leveler",      "Toggle")]
    [TouchPortalActionMapping("AP_WING_LEVELER_ON",   "Wing Leveler",      "On")]
    [TouchPortalActionMapping("AP_WING_LEVELER_OFF",  "Wing Leveler",      "Off")]
    [TouchPortalActionMapping("YAW_DAMPER_TOGGLE",    "Yaw Dampener",      "Toggle")]
    [TouchPortalActionMapping("YAW_DAMPER_ON",        "Yaw Dampener",      "On")]
    [TouchPortalActionMapping("YAW_DAMPER_OFF",       "Yaw Dampener",      "Off")]

    public static readonly object AP_SWITCHES;

    #endregion

    #region Attitude

    [TouchPortalAction("AutoPilotAttitudeVar", "Attitude Hold Pitch Value Adj/Sel", "Attitude Hold Pitch Value {0}", true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("AP_PITCH_REF_SELECT", "Select")]
    [TouchPortalActionMapping("AP_PITCH_REF_INC_UP", "Increase")]
    [TouchPortalActionMapping("AP_PITCH_REF_INC_DN", "Decrease")]
    public static readonly object AP_ATTITUDE_PITCH;

#if !FSX
    [TouchPortalAction("AutoPilotAttitudeSet", "Attitude Hold Pitch Value Set", true,
      "Set Attitude Hold Pitch Value to {0} (-16384 to +16384)",
      "Set Attitude Pitch Hold\nin Value Range:"
    )]
    [TouchPortalActionText("0", -16384, 16384)]
    [TouchPortalActionMapping("AP_PITCH_REF_SET")]
    public static readonly object AP_ATTITUDE_PITCH_SET;
#endif

    #endregion

    #region Bank

    [TouchPortalAction("AutoPilotBanking", "Max. Bank Angle Adjust", "Max Bank Angle - {0}", true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("AP_MAX_BANK_INC", "Increase")]
    [TouchPortalActionMapping("AP_MAX_BANK_DEC", "Decrease")]
    public static readonly object AP_MAX_BANK;

#if !FSX
    [TouchPortalAction("AutoPilotMaxBankSet", "Max. Bank Set", true,
      "Set Max Bank {0} Value to {1}",
      "Set Max Bank {0}in Value\nRange:"
    )]
    [TouchPortalActionChoice()]
    [TouchPortalActionText("0", 0, 180, AllowDecimals = true)]
    [TouchPortalActionMapping("AP_MAX_BANK_SET", "Limit Preset")]
    [TouchPortalActionMapping("AP_MAX_BANK_ANGLE_SET", "Angle")]
    [TouchPortalActionMapping("AP_MAX_BANK_VELOCITY_SET", "Velocity")]
    public static readonly object AP_MAX_BANK_SET;
#endif

    #endregion

    #region Heading

    [TouchPortalAction("AutoPilotHeadingVar", "Heading Hold Value Adj/Sel", "Heading Hold Value - {0}", true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("HEADING_BUG_SELECT", "Select")]
    [TouchPortalActionMapping("HEADING_BUG_INC",    "Increase")]
    [TouchPortalActionMapping("HEADING_BUG_DEC",    "Decrease")]
    public static readonly object AP_HEADING_VAR;

    [TouchPortalAction("AutoPilotHeadingSet", "Heading Hold Value Set", true,
      "Set Heading Hold Value - {0}",
      "Set Heading Hold\nin Value Range:"
    )]
    [TouchPortalActionText("1", 0, 359, AllowDecimals = true)]
    [TouchPortalActionMapping("HEADING_BUG_SET")]
    public static readonly object AP_HEADING_SET;

    #endregion

    #region Altitude

    [TouchPortalAction("AutoPilotAltitudeVar", "Altitude Hold Value Adj/Sel", "Altitude Hold Value - {0}", true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("ALTITUDE_BUG_SELECT", "Select")]
    [TouchPortalActionMapping("AP_ALT_VAR_INC", "Increase")]
    [TouchPortalActionMapping("AP_ALT_VAR_DEC", "Decrease")]
    public static readonly object AP_ALTITUDE_VAR;

    [TouchPortalAction("AutoPilotAltitudeSet", "Altitude Hold Value Set", "Altitude Hold Value {0} to {1}", true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionText("0", -1000, 150000, AllowDecimals = true)]
    [TouchPortalActionMapping("AP_ALT_VAR_SET_ENGLISH", "Set English")]
    [TouchPortalActionMapping("AP_ALT_VAR_SET_METRIC", "Set Metric")]
    public static readonly object AP_ALTITUDE_SET;

    #endregion

    #region Nav

    [TouchPortalAction("AutoPilotNavModeSelect", "Nav Mode Select", "Select Nav Mode {0}")]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("AP_NAV_SELECT_SET", "1", 1)]
    [TouchPortalActionMapping("AP_NAV_SELECT_SET", "2", 2)]
    public static readonly object AP_NAV_SELECT_SELECT;

    #endregion

    #region Vertical Speed

    [TouchPortalAction("AutoPilotVerticalSpeedVar", "Vertical Speed Value Adj/Sel/Hold", "Vertical Speed Value - {0}", true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("VSI_BUG_SELECT", "Select")]
    [TouchPortalActionMapping("AP_VS_VAR_INC", "Increase")]
    [TouchPortalActionMapping("AP_VS_VAR_DEC", "Decrease")]
#if !FSX
    [TouchPortalActionMapping("AP_VS_VAR_SET_CURRENT", "Hold Current")]
#endif
    public static readonly object AP_VERTICALSPEED_VAR;

    [TouchPortalAction("AutoPilotVerticalSpeedSet", "Vertical Speed Value Set", true,
      "Vertical Speed Hold {0} to Value {1}",
      "Vertical Speed Hold {0}in Value\nRange:"
    )]
    [TouchPortalActionChoice()]
    [TouchPortalActionText("1", -5000, 5000, AllowDecimals = true)]
    [TouchPortalActionMapping("AP_VS_VAR_SET_ENGLISH", "Set English")]
    [TouchPortalActionMapping("AP_VS_VAR_SET_METRIC", "Set Metric")]
    public static readonly object AP_VERTICALSPEED_SET;

    #endregion

    #region Airspeed

    [TouchPortalAction("AutoPilotAirSpeedVar", "Airspeed Hold Value  Adj/Sel/Hold", "Airspeed Hold Value - {0}", true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("AIRSPEED_BUG_SELECT", "Select")]
    [TouchPortalActionMapping("AP_SPD_VAR_INC", "Increase")]
    [TouchPortalActionMapping("AP_SPD_VAR_DEC", "Decrease")]
#if !FSX
    [TouchPortalActionMapping("AP_PANEL_SPEED_HOLD_TOGGLE", "Hold Current")]  // KEY_AUTOPILOT_AIRSPEED_HOLD_CURRENT
#endif
    public static readonly object AP_AIRSPEED_VAR;

    [TouchPortalAction("AutoPilotAirSpeedSet", "Airspeed Hold Value Set", true,
      "Set Airspeed Hold Value to {0} kts",
      "Set Airspeed Hold\nin Value Range (kts):"
    )]
    [TouchPortalActionText("0", 0, 50000, AllowDecimals = true)]
    [TouchPortalActionMapping("AP_SPD_VAR_SET")]
    public static readonly object AP_AIRSPEED_SET;

    #endregion

#if !FSX
    #region AutoThrottle

    [TouchPortalAction("AutoThrottle", "Auto Throttle", "Toggle Auto Throttle - {0}")]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("AUTO_THROTTLE_ARM", "Arm")]
    [TouchPortalActionMapping("AUTO_THROTTLE_TO_GA", "GoAround")]
    [TouchPortalActionMapping("AUTO_THROTTLE_DISCONNECT", "Disconnect")]
    public static readonly object AUTO_THROTTLE;

    #endregion

    #region AutoBrake

    [TouchPortalAction("AutoBrake", "Auto Brake", "Auto Brake - {0}", true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("INC_AUTOBRAKE_CONTROL", "Increase")]
    [TouchPortalActionMapping("INC_AUTOBRAKE_CONTROL", "Decrease")]
    [TouchPortalActionMapping("AUTOBRAKE_DISARM", "Disarm")]
    [TouchPortalActionMapping("AUTOBRAKE_LO_SET", "Set Low")]
    [TouchPortalActionMapping("AUTOBRAKE_MED_SET", "Set Medium")]
    [TouchPortalActionMapping("AUTOBRAKE_HI_SET", "Set Maximum")]
    public static readonly object AUTO_BRAKE;

    [TouchPortalAction("AutoBrakeSet", "Auto Brake Set", true,
      "Set Auto Brake Value to {0} (1-4)",
      "Set Auto Brake\nin Value Range:"
    )]
    [TouchPortalActionText("0", 0, 4, AllowDecimals = false)]
    [TouchPortalActionMapping("SET_AUTOBRAKE_CONTROL")]
    public static readonly object AUTO_BRAKE_SET;

    #endregion
#endif

    #region Mach

    [TouchPortalAction("AutoPilotMachVar", "Mach Hold Value Adjust/Hold", "Mach Hold Value - {0}", true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("AP_MACH_VAR_INC", "Increase")]
    [TouchPortalActionMapping("AP_MACH_VAR_DEC", "Decrease")]
#if !FSX
    [TouchPortalActionMapping("AP_PANEL_MACH_HOLD_TOGGLE", "Hold Current")]  // KEY_AUTOPILOT_MACH_HOLD_CURRENT
#endif
    public static readonly object AP_MACH_VAR;

    [TouchPortalAction("AutoPilotMachSet", "Mach Hold Value Set", true,
      "Set Mach Hold Value to {0}",
      "Set Mach Hold\nin Value Range:"
    )]
    [TouchPortalActionText("0", 0, 20, AllowDecimals = true)]
    [TouchPortalActionMapping("AP_MACH_VAR_SET")]
    public static readonly object AP_MACH_SET;

    #endregion

    #region N1

    [TouchPortalAction("AutoPilotN1Ref", "N1 Reference Value Adjust/Hold", "N1 Reference Value - {0}", true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("AP_N1_REF_INC", "Increase")]
    [TouchPortalActionMapping("AP_N1_REF_DEC", "Decrease")]
    [TouchPortalActionMapping("AP_N1_HOLD", "Hold Current")]
    public static readonly object N1_REF_ADJ;

    [TouchPortalAction("AutoPilotN1RefSet", "N1 Reference Value Set", true,
      "Set N1 Reference Value to {0}",
      "Set N1 Reference\nin Value Range:"
    )]
    [TouchPortalActionText("0", 0, 150, AllowDecimals = true)]
    [TouchPortalActionMapping("AP_N1_REF_SET")]
    public static readonly object N1_REF_SET;

    #endregion

    #region Flight Director

    [TouchPortalAction("AutoPilotFlightDirectorSwitches", "Flight Director Switches", "Toggle Flight Director {0} Switch")]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("TOGGLE_FLIGHT_DIRECTOR",     "Master")]
    [TouchPortalActionMapping("SYNC_FLIGHT_DIRECTOR_PITCH", "Pitch Sync")]
    public static readonly object AP_FLIGHT_DIRECTOR_SW;

    #endregion

    #region DEPRECATED

    [TouchPortalAction("AutoPilotNavSelect", "Nav Mode Set", "Sets the nav to 1 or 2 for Nav mode", "Nav Mode - {0} ", Deprecated = true)]
    [TouchPortalActionNumeric(1, 1, 2)]
    [TouchPortalActionMapping("AP_NAV_SELECT_SET")]
    public static readonly object AP_NAV_SELECT_SET;
    [TouchPortalAction("AutoPilotFlightDirector", "Flight Director Switch", "Toggle Flight Director On/Off", Deprecated = true)]
    [TouchPortalActionMapping("TOGGLE_FLIGHT_DIRECTOR")]
    public static readonly object AP_FLIGHT_DIRECTOR;
    [TouchPortalAction("AutoPilotFlightDirectorCurrentPitch", "Flight Director Pitch Sync Switch", "Sync Flight Director Pitch", Deprecated = true)]
    [TouchPortalActionMapping("SYNC_FLIGHT_DIRECTOR_PITCH")]
    public static readonly object SYNC_FLIGHT_DIRECTOR_PITCH;
    [TouchPortalAction("AutoPilotMaster", "AutoPilot Switch", "Auto Pilot Master - {0}", Deprecated = true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("AP_MASTER", "Toggle")]
    [TouchPortalActionMapping("AUTOPILOT_ON", "On")]
    [TouchPortalActionMapping("AUTOPILOT_OFF", "Off")]
    public static readonly object AP_MASTER;
    [TouchPortalAction("AutoPilotAttitude", "Attitude Hold Switch", "Attitude Hold - {0}", Deprecated = true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("AP_ATT_HOLD", "Toggle")]
    [TouchPortalActionMapping("AP_ATT_HOLD_ON", "On")]
    [TouchPortalActionMapping("AP_ATT_HOLD_OFF", "Off")]
    public static readonly object AP_ATTITUDE;
    [TouchPortalAction("AutoPilotApproach", "Approach Mode Switch", "Approach Mode - {0}", Deprecated = true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("AP_APR_HOLD", "Toggle")]
    [TouchPortalActionMapping("AP_APR_HOLD_ON", "On")]
    [TouchPortalActionMapping("AP_APR_HOLD_OFF", "Off")]
    public static readonly object AP_APPROACH;
    [TouchPortalAction("AutoPilotHeading", "Heading Hold Switch", "Heading Hold - {0}", Deprecated = true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("AP_HDG_HOLD", "Toggle")]
    [TouchPortalActionMapping("AP_HDG_HOLD_ON", "On")]
    [TouchPortalActionMapping("AP_HDG_HOLD_OFF", "Off")]
    public static readonly object AP_HEADING;
    [TouchPortalAction("AutoPilotBackCourse", "Back Course Mode Switch", "Back Course Mode - {0}", Deprecated = true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("AP_BC_HOLD", "Toggle")]
    [TouchPortalActionMapping("AP_BC_HOLD_ON", "On")]
    [TouchPortalActionMapping("AP_BC_HOLD_OFF", "Off")]
    public static readonly object AP_BACKCOURSE;
    [TouchPortalAction("AutoPilotNav1", "Nav1 Mode Switch", "Nav1 Mode - {0}", Deprecated = true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("AP_NAV1_HOLD", "Toggle")]
    [TouchPortalActionMapping("AP_NAV1_HOLD_ON", "On")]
    [TouchPortalActionMapping("AP_NAV1_HOLD_OFF", "Off")]
    public static readonly object AP_NAV1;
    [TouchPortalAction("AutoPilotAltitude", "Altitude Hold Switch", "Altitude Hold - {0}", Deprecated = true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("AP_ALT_HOLD", "Toggle")]
    [TouchPortalActionMapping("AP_ALT_HOLD_ON", "On")]
    [TouchPortalActionMapping("AP_ALT_HOLD_OFF", "Off")]
    public static readonly object AP_ALTITUDE;
    [TouchPortalAction("AutoPilotVerticalSpeed", "Vertical Speed Switch", "Vertical Speed - {0}", Deprecated = true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("AP_VS_HOLD", "Toggle")]
    [TouchPortalActionMapping("AP_VS_ON", "On")]
    [TouchPortalActionMapping("AP_VS_OFF", "Off")]
    public static readonly object AutoPilotVerticalSpeedHold;
    [TouchPortalAction("AutoPilotAirSpeed", "Airspeed Hold Switch", "Airspeed Hold - {0}", Deprecated = true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("AP_AIRSPEED_HOLD", "Toggle")]
    [TouchPortalActionMapping("AP_AIRSPEED_ON", "On")]
    [TouchPortalActionMapping("AP_AIRSPEED_OFF", "Off")]
    public static readonly object AP_AIRSPEED;
    [TouchPortalAction("AutoPilotMach", "Mach Hold Switch", "Mach Hold - {0}", Deprecated = true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("AP_MACH_HOLD", "Toggle")]
    [TouchPortalActionMapping("AP_MACH_ON", "On")]
    [TouchPortalActionMapping("AP_MACH_OFF", "Off")]
    public static readonly object AP_MACH;
    [TouchPortalAction("AutoPilotWingLeveler", "Wing Leveler Switch", "Wing Leveler - {0}", Deprecated = true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("AP_WING_LEVELER", "Toggle")]
    [TouchPortalActionMapping("AP_WING_LEVELER_ON", "On")]
    [TouchPortalActionMapping("AP_WING_LEVELER_OFF", "Off")]
    public static readonly object AP_WING_LEVELER;
    // TODO: Localizer state?
    [TouchPortalAction("AutoPilotLocalizer", "Localizer Switch", "Localizer - {0}", Deprecated = true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("AP_LOC_HOLD", "Toggle")]
    [TouchPortalActionMapping("AP_LOC_HOLD_ON", "On")]
    [TouchPortalActionMapping("AP_LOC_HOLD_OFF", "Off")]
    public static readonly object AP_LOCALIZER;
    [TouchPortalAction("AutoPilotYawDampener", "Yaw Dampener Switch", "Yaw Dampener - {0}", Deprecated = true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("YAW_DAMPER_TOGGLE", "Toggle")]
    [TouchPortalActionMapping("YAW_DAMPER_ON", "On")]
    [TouchPortalActionMapping("YAW_DAMPER_OFF", "Off")]
    public static readonly object AP_YAWDAMPENER;

    #endregion


    #region Todo ??

    //AP_N1_HOLD,
    //AP_N1_REF_INC,
    //AP_N1_REF_DEC,
    //AP_N1_REF_SET,
    //FLY_BY_WIRE_ELAC_TOGGLE,
    //FLY_BY_WIRE_FAC_TOGGLE,
    //FLY_BY_WIRE_SEC_TOGGLE,

    //// Doesn't set value
    //AP_PANEL_ALTITUDE_SET,
    //AP_PANEL_HEADING_SET,
    //AP_PANEL_MACH_SET,
    //AP_PANEL_SPEED_SET,
    //AP_PANEL_VS_SET,

    #endregion

  }
}
