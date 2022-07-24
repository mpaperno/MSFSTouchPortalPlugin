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
  [TouchPortalCategory(Groups.FlightSystems)]
  internal static class FlightSystemsMapping
  {
    #region Ailerons

    [TouchPortalAction("Ailerons", "Ailerons Adjust", "Adjust Ailerons {0}", true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("CENTER_AILER_RUDDER", "Center")]
    [TouchPortalActionMapping("AILERONS_LEFT", "Left")]
    [TouchPortalActionMapping("AILERONS_RIGHT", "Right")]
    public static readonly object Ailerons;

    [TouchPortalAction("AileronsSet", "Ailerons Set", " Set Ailerons", true,
      "Set Ailerons to {0} (-16384 to +16384)",
      "Set Ailerons\nin Value Range:"
    )]
    [TouchPortalActionText("0", -16384, 16384)]
    [TouchPortalActionMapping("AILERON_SET")]
    public static readonly object AileronsSet;

    #endregion

    #region Elevator

    [TouchPortalAction("Elevator", "Elevator Adjust", "Adjust Elevator {0}", true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("ELEV_UP", "Up")]
    [TouchPortalActionMapping("ELEV_DOWN", "Down")]
    public static readonly object Elevator;

    [TouchPortalAction("ElevatorSet", "Elevator Set", "Set Elevator", true,
      "Set Elevator to {0} (-16384 to +16384)",
      "Set Elevator\nin Value Range:"
    )]
    [TouchPortalActionText("0", -16384, 16384)]
    [TouchPortalActionMapping("ELEVATOR_SET")]
    public static readonly object ElevatorSet;

    #endregion

    #region Brakes

    [TouchPortalAction("Brakes", "Brakes Activate", "Activate {0} Brakes", true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("BRAKES", "All")]
    [TouchPortalActionMapping("BRAKES_LEFT", "Left")]
    [TouchPortalActionMapping("BRAKES_RIGHT", "Right")]
    public static readonly object Brakes;

    [TouchPortalAction("BrakesSet", "Brake Axis Set", true,
      "Set Brake Axis {0} to {1} (0 to 16384)",
      "Set Brake Axis{0}in Value\n Range:"
    )]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("AXIS_LEFT_BRAKE_SET", "Left")]
    [TouchPortalActionMapping("AXIS_RIGHT_BRAKE_SET", "Right")]
    [TouchPortalActionText("0", 0, 16384)]
    public static readonly object BrakesSet;

    [TouchPortalAction("ParkingBreak", "Parking Brake Toggle", "Toggle the Parking Brake On/Off")]
    [TouchPortalActionMapping("PARKING_BRAKES")]
    public static readonly object ParkingBrake;

    #endregion

    #region Flaps

    [TouchPortalAction("Flaps", "Flaps Adjust", "Flaps {0}")]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("FLAPS_UP", "Up")]
    [TouchPortalActionMapping("FLAPS_DOWN", "Down")]
    [TouchPortalActionMapping("FLAPS_INCR", "Increase")]
    [TouchPortalActionMapping("FLAPS_DECR", "Decrease")]
    [TouchPortalActionMapping("FLAPS_1", "1")]
    [TouchPortalActionMapping("FLAPS_2", "2")]
    [TouchPortalActionMapping("FLAPS_3", "3")]
    [TouchPortalActionMapping("FLAPS_3", "4")]
    public static readonly object FlapsHandlePercent;

    [TouchPortalAction("FlapsSet", "Flaps Handle Set", "Set Flaps Handle Position", true,
      "Set Flaps Handle to {0} (0 to 16384)",
      "Set Flaps Handle\nin Value Range:"
    )]
    [TouchPortalActionText("0", 0, 16384)]
    [TouchPortalActionMapping("FLAPS_SET")]
    public static readonly object FlapsSet;

    [TouchPortalAction("CowlFlapsAdjust", "Cowl Flap Levers Adjust", "Cowl Flaps Lever {0} {1}", true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("INC_COWL_FLAPS", "All", "Increase")]
    [TouchPortalActionMapping("DEC_COWL_FLAPS", "All", "Decrease")]
    [TouchPortalActionMapping("INC_COWL_FLAPS1", "1", "Increase")]
    [TouchPortalActionMapping("DEC_COWL_FLAPS1", "1", "Decrease")]
    [TouchPortalActionMapping("INC_COWL_FLAPS2", "2", "Increase")]
    [TouchPortalActionMapping("DEC_COWL_FLAPS2", "2", "Decrease")]
    [TouchPortalActionMapping("INC_COWL_FLAPS3", "3", "Increase")]
    [TouchPortalActionMapping("DEC_COWL_FLAPS3", "3", "Decrease")]
    [TouchPortalActionMapping("INC_COWL_FLAPS4", "4", "Increase")]
    [TouchPortalActionMapping("DEC_COWL_FLAPS4", "4", "Decrease")]
    public static readonly object CowlFlapsAdjust;

    [TouchPortalAction("CowlFlapsSet", "Cowl Flaps Lever Set", true,
      "Set Cowl {0} Flaps Lever to {0} (0 to 16384)",
      "Set Cowl{0}Flaps Lever\nin Value Range:"
    )]
    [TouchPortalActionChoice()]
    [TouchPortalActionText("0", 0, 16384)]
    [TouchPortalActionMapping("COWLFLAP1_SET", "1")]
    [TouchPortalActionMapping("COWLFLAP2_SET", "2")]
    [TouchPortalActionMapping("COWLFLAP3_SET", "3")]
    [TouchPortalActionMapping("COWLFLAP4_SET", "4")]
    public static readonly object CowlFlapsSet;

    // Deprecated
    [TouchPortalAction("CowlFlapsAll", "Cowl Flap Levers - Adjust All", "{0} All Cowl Flaps Levers", true, Deprecated = true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("INC_COWL_FLAPS", "Increase")]
    [TouchPortalActionMapping("DEC_COWL_FLAPS", "Decrease")]
    public static readonly object CowlFlapsAll;
    [TouchPortalAction("CowlFlaps1", "Cowl Flaps 1 Lever Adjust", "{0} Cowl Flaps 1 Lever", true, Deprecated = true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("INC_COWL_FLAPS1", "Increase")]
    [TouchPortalActionMapping("DEC_COWL_FLAPS1", "Decrease")]
    public static readonly object CowlFlaps1;
    [TouchPortalAction("CowlFlaps2", "Cowl Flaps 2 Lever Adjust", "{0} Cowl Flaps 2 Lever", true, Deprecated = true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("INC_COWL_FLAPS2", "Increase")]
    [TouchPortalActionMapping("DEC_COWL_FLAPS2", "Decrease")]
    public static readonly object CowlFlaps2;
    [TouchPortalAction("CowlFlaps3", "Cowl Flaps 3 Lever Adjust", "{0} Cowl Flaps 3 Lever", true, Deprecated = true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("INC_COWL_FLAPS3", "Increase")]
    [TouchPortalActionMapping("DEC_COWL_FLAPS3", "Decrease")]
    public static readonly object CowlFlaps3;
    [TouchPortalAction("CowlFlaps4", "Cowl Flaps 4 Lever Adjust", "{0} Cowl Flaps 4 Lever", true, Deprecated = true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("INC_COWL_FLAPS4", "Increase")]
    [TouchPortalActionMapping("DEC_COWL_FLAPS4", "Decrease")]
    public static readonly object CowlFlaps4;

    #endregion

    #region Gear

    [TouchPortalAction("Gear", "Gear Manipulation", "Gear {0}", true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("GEAR_TOGGLE", "Toggle")]
    [TouchPortalActionMapping("GEAR_UP", "Up")]
    [TouchPortalActionMapping("GEAR_DOWN", "Down")]
    [TouchPortalActionMapping("GEAR_PUMP", "Pump")]
    public static readonly object Gear;

    #endregion

    #region Rudder

    [TouchPortalAction("Rudder", "Rudder Adjust", "Adjust Rudder {0}", true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("RUDDER_CENTER", "Center")]
    [TouchPortalActionMapping("RUDDER_LEFT", "Left")]
    [TouchPortalActionMapping("RUDDER_RIGHT", "Right")]
    public static readonly object Rudder;

    [TouchPortalAction("RudderSet", "Rudder Set", " Set Rudder", true,
      "Set Rudder to {0} (-16384 to +16384)",
      "Set Rudder\nin Value Range:"
    )]
    [TouchPortalActionText("0", -16384, 16384)]
    [TouchPortalActionMapping("RUDDER_SET")]
    public static readonly object RudderSet;

    #endregion

    #region Spoilers
    // https://docs.flightsimulator.com/html/Programming_Tools/SimVars/Aircraft_SimVars/Aircraft_Control_Variables.htm#spoilers

    [TouchPortalAction("Spoilers", "Spoilers Action", "Spoilers {0}")]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("SPOILERS_TOGGLE", "Toggle")]
    [TouchPortalActionMapping("SPOILERS_ON", "On")]
    [TouchPortalActionMapping("SPOILERS_OFF", "Off")]
    public static readonly object SpoilersAvailable;

    [TouchPortalAction("SpoilersSet", "Spoilers Set", "", true,
      "Set Spoilers handle position to {0} (0 to 16384)",
      "Set Spoilers Handle\nPosition in Value Range:"
    )]
    [TouchPortalActionText("0", 0, 16384)]
    [TouchPortalActionMapping("SPOILERS_SET")]
    public static readonly object SpoilersHandlePosition;

    [TouchPortalAction("SpoilersArm", "Spoilers Arm", "Spoilers Arm {0}")]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("SPOILERS_ARM_TOGGLE", "Toggle")]
    [TouchPortalActionMapping("SPOILERS_ARM_ON", "On")]
    [TouchPortalActionMapping("SPOILERS_ARM_OFF", "Off")]
    public static readonly object SpoilersArmed;

    #endregion

    #region Trimming

    [TouchPortalAction("AileronTrim", "Aileron Trim Adjust", "Adjust Aileron Trim {0}", true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("AILERON_TRIM_LEFT", "Left")]
    [TouchPortalActionMapping("AILERON_TRIM_RIGHT", "Right")]
    public static readonly object AileronTrim;

    [TouchPortalAction("AileronTrimSet", "Aileron Trim Set", "", true,
      "Set Aileron Trim to {0}% (-100 - +100)",
      "Set Aileron Trim\nin Value Range (%):"
    )]
    [TouchPortalActionText("0", -100, 100)]
    [TouchPortalActionMapping("AILERON_TRIM_SET")]
    public static readonly object AileronTrimSet;


    [TouchPortalAction("ElevatorTrim", "Elevator Trim Adjust", "Adjust Elevator Trim {0}", true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("ELEV_TRIM_UP", "Up")]
    [TouchPortalActionMapping("ELEV_TRIM_DN", "Down")]
    public static readonly object ElevatorTrim;

    [TouchPortalAction("ElevatorTrimSet", "Elevator Trim Set", true,
      "Set Elevator Trim to {0} (-16384 to +16384)",
      "Set Elevator Trim\nin Value Range:"
    )]
    [TouchPortalActionText("0", -16384, 16384)]
    [TouchPortalActionMapping("ELEVATOR_TRIM_SET")]
    public static readonly object ElevatorTrimSet;


    [TouchPortalAction("RudderTrim", "Rudder Trim Adjust", "Adjust Rudder Trim {0}", true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("RUDDER_TRIM_LEFT", "Left")]
    [TouchPortalActionMapping("RUDDER_TRIM_RIGHT", "Right")]
    public static readonly object RudderTrim;

    [TouchPortalAction("RudderTrimSet", "Rudder Trim Set", "", true,
      "Set Rudder Trim to {0}% (-100 - +100)",
      "Set Rudder Trim\nin Value Range (%):"
    )]
    [TouchPortalActionText("0", -100, 100)]
    [TouchPortalActionMapping("RUDDER_TRIM_SET")]
    public static readonly object RudderTrimSet;

    #endregion
  }
}
