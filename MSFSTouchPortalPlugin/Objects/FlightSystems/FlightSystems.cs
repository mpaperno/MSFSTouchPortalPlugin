using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Enums;

namespace MSFSTouchPortalPlugin.Objects.FlightSystems
{
  [SimNotificationGroup(Groups.FlightSystems)]
  [TouchPortalCategory("FlightSystems", "MSFS - Flight Systems")]
  internal static class FlightSystemsMapping {
    #region Ailerons

    [TouchPortalAction("Ailerons", "Ailerons", "MSFS", "Ailerons", "Ailerons - {0}", true)]
    [TouchPortalActionChoice(new [] { "Center", "Left", "Right" })]
    [TouchPortalActionMapping("CENTER_AILER_RUDDER", "Center")]
    [TouchPortalActionMapping("AILERONS_LEFT", "Left")]
    [TouchPortalActionMapping("AILERONS_RIGHT", "Right")]
    public static readonly object Ailerons;

    [TouchPortalAction("AileronsSet", "Ailerons Set", "MSFS", " Set Ailerons", "Ailerons set to {0} (-16384 to +16384)")]
    [TouchPortalActionText("0", -16384, 16384)]
    [TouchPortalActionMapping("AILERON_SET")]
    public static readonly object AileronsSet;

    #endregion

    #region Elevator

    [TouchPortalAction("Elevator", "Elevator", "MSFS", "Elevator", "Elevator - {0}", true)]
    [TouchPortalActionChoice(new[] { "Up", "Down" })]
    [TouchPortalActionMapping("ELEV_UP", "Up")]
    [TouchPortalActionMapping("ELEV_DOWN", "Down")]
    public static readonly object Elevator;

    [TouchPortalAction("ElevatorSet", "Elevator Set", "MSFS", " Set Elevator", "Elevator set to {0} (-16384 to +16384)")]
    [TouchPortalActionText("0", -16384, 16384)]
    [TouchPortalActionMapping("ELEVATOR_SET")]
    public static readonly object ElevatorSet;

    #endregion

    #region Brakes

    [TouchPortalAction("Brakes", "Brakes", "MSFS", "Brakes", "Brakes - {0}", true)]
    [TouchPortalActionChoice(new [] { "All", "Left", "Right" })]
    [TouchPortalActionMapping("BRAKES", "All")]
    [TouchPortalActionMapping("BRAKES_LEFT", "Left")]
    [TouchPortalActionMapping("BRAKES_RIGHT", "Right")]
    public static readonly object Brakes;

    [TouchPortalAction("ParkingBreak", "Toggle Parking Brake", "MSFS", "Toggle Parking Brake", "Toggle Parking Brake")]
    [TouchPortalActionMapping("PARKING_BRAKES")]
    public static readonly object ParkingBrake;

    #endregion

    #region Flaps

    [TouchPortalAction("Flaps", "Flaps", "MSFS", "Flaps", "Flaps - {0}")]
    [TouchPortalActionChoice(new [] { "Up", "Down", "Increase", "Decrease", "1", "2", "3", "4" })]
    [TouchPortalActionMapping("FLAPS_UP", "Up")]
    [TouchPortalActionMapping("FLAPS_DOWN", "Down")]
    [TouchPortalActionMapping("FLAPS_INCR", "Increase")]
    [TouchPortalActionMapping("FLAPS_DECR", "Decrease")]
    [TouchPortalActionMapping("FLAPS_1", "1")]
    [TouchPortalActionMapping("FLAPS_2", "2")]
    [TouchPortalActionMapping("FLAPS_3", "3")]
    [TouchPortalActionMapping("FLAPS_3", "4")]
    public static readonly object FlapsHandlePercent;

    [TouchPortalAction("FlapsSet", "Flaps Set", "MSFS", " Set Flaps", "Flaps set to {0} (0 to 16384)")]
    [TouchPortalActionText("0", 0, 16384)]
    [TouchPortalActionMapping("FLAPS_SET")]
    public static readonly object FlapsSet;


    [TouchPortalAction("CowlFlapsAll", "Cowl Flaps All", "MSFS", "Cowl Flaps All", "Cowl Flaps All - {0}", true)]
    [TouchPortalActionChoice(new [] { "Increase", "Decrease" })]
    [TouchPortalActionMapping("INC_COWL_FLAPS", "Increase")]
    [TouchPortalActionMapping("DEC_COWL_FLAPS", "Decrease")]
    public static readonly object CowlFlapsAll;

    [TouchPortalAction("CowlFlaps1", "Cowl Flaps 1", "MSFS", "Cowl Flaps 1", "Cowl Flaps 1 - {0}", true)]
    [TouchPortalActionChoice(new [] { "Increase", "Decrease" })]
    [TouchPortalActionMapping("INC_COWL_FLAPS1", "Increase")]
    [TouchPortalActionMapping("DEC_COWL_FLAPS1", "Decrease")]
    public static readonly object CowlFlaps1;

    [TouchPortalAction("CowlFlaps2", "Cowl Flaps 2", "MSFS", "Cowl Flaps 2", "Cowl Flaps 2 - {0}", true)]
    [TouchPortalActionChoice(new [] { "Increase", "Decrease" })]
    [TouchPortalActionMapping("INC_COWL_FLAPS2", "Increase")]
    [TouchPortalActionMapping("DEC_COWL_FLAPS2", "Decrease")]
    public static readonly object CowlFlaps2;

    [TouchPortalAction("CowlFlaps3", "Cowl Flaps 3", "MSFS", "Cowl Flaps 3", "Cowl Flaps 3 - {0}", true)]
    [TouchPortalActionChoice(new [] { "Increase", "Decrease" })]
    [TouchPortalActionMapping("INC_COWL_FLAPS3", "Increase")]
    [TouchPortalActionMapping("DEC_COWL_FLAPS3", "Decrease")]
    public static readonly object CowlFlaps3;

    [TouchPortalAction("CowlFlaps4", "Cowl Flaps 4", "MSFS", "Cowl Flaps 4", "Cowl Flaps 4 - {0}", true)]
    [TouchPortalActionChoice(new [] { "Increase", "Decrease" })]
    [TouchPortalActionMapping("INC_COWL_FLAPS4", "Increase")]
    [TouchPortalActionMapping("DEC_COWL_FLAPS4", "Decrease")]
    public static readonly object CowlFlaps4;

    #endregion

    #region Gear

    [TouchPortalAction("Gear", "Gear Manipulation", "MSFS", "Gear Manipulation", "Gear - {0}", true)]
    [TouchPortalActionChoice(new [] { "Toggle", "Up", "Down", "Pump" })]
    [TouchPortalActionMapping("GEAR_TOGGLE", "Toggle")]
    [TouchPortalActionMapping("GEAR_UP", "Up")]
    [TouchPortalActionMapping("GEAR_DOWN", "Down")]
    [TouchPortalActionMapping("GEAR_PUMP", "Pump")]
    public static readonly object Gear;

    #endregion

    #region Rudder

    [TouchPortalAction("Rudder", "Rudder", "MSFS", "Rudder", "Rudder - {0}", true)]
    [TouchPortalActionChoice(new [] { "Center", "Left", "Right" })]
    [TouchPortalActionMapping("RUDDER_CENTER", "Center")]
    [TouchPortalActionMapping("RUDDER_LEFT", "Left")]
    [TouchPortalActionMapping("RUDDER_RIGHT", "Right")]
    public static readonly object Rudder;

    [TouchPortalAction("RudderSet", "Rudder Set", "MSFS", " Set Rudder", "Rudder set to {0} (-16384 to +16384)")]
    [TouchPortalActionText("0", -16384, 16384)]
    [TouchPortalActionMapping("RUDDER_SET")]
    public static readonly object RudderSet;

    #endregion

    #region Spoilers
    // https://docs.flightsimulator.com/html/Programming_Tools/SimVars/Aircraft_SimVars/Aircraft_Control_Variables.htm#spoilers

    [TouchPortalAction("Spoilers", "Spoilers", "MSFS", "Spoilers", "Spoilers - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("SPOILERS_TOGGLE", "Toggle")]
    [TouchPortalActionMapping("SPOILERS_ON", "On")]
    [TouchPortalActionMapping("SPOILERS_OFF", "Off")]
    public static readonly object SpoilersAvailable;

    [TouchPortalAction("SpoilersSet", "Spoilers Set", "MSFS", " Set Spoilers", "Set Spoilers handle position to {0} (0 to 16384)")]
    [TouchPortalActionText("0", 0, 16384)]
    [TouchPortalActionMapping("SPOILERS_SET")]
    public static readonly object SpoilersHandlePosition;

    [TouchPortalAction("SpoilersArm", "Spoilers Arm", "MSFS", "Spoilers Arm", "Spoilers Arm - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("SPOILERS_ARM_TOGGLE", "Toggle")]
    [TouchPortalActionMapping("SPOILERS_ARM_ON", "On")]
    [TouchPortalActionMapping("SPOILERS_ARM_OFF", "Off")]
    public static readonly object SpoilersArmed;

    #endregion

    #region Trimming

    [TouchPortalAction("AileronTrim", "Aileron Trim", "MSFS", "Aileron Trim", "Aileron Trim - {0}", true)]
    [TouchPortalActionChoice(new [] { "Left", "Right" }, "Left")]
    [TouchPortalActionMapping("AILERON_TRIM_LEFT", "Left")]
    [TouchPortalActionMapping("AILERON_TRIM_RIGHT", "Right")]
    public static readonly object AileronTrim;

    [TouchPortalAction("AileronTrimSet", "Aileron Trim Set", "MSFS", " Set Aileron Trim", "Aileron Trim set to {0}% (-100 - +100)")]
    [TouchPortalActionText("0", -100, 100)]
    [TouchPortalActionMapping("AILERON_TRIM_SET")]
    public static readonly object AileronTrimSet;


    [TouchPortalAction("ElevatorTrim", "Elevator Trim", "MSFS", "Elevator Trim", "Elevator Trim - {0}", true)]
    [TouchPortalActionChoice(new [] { "Up", "Down" })]
    [TouchPortalActionMapping("ELEV_TRIM_DN", "Down")]
    [TouchPortalActionMapping("ELEV_TRIM_UP", "Up")]
    public static readonly object ElevatorTrim;

    [TouchPortalAction("ElevatorTrimSet", "Elevator Trim Set", "MSFS", " Set Elevator Trim", "Elevator Trim set to {0} (-16384 to +16384)")]
    [TouchPortalActionText("0", -16384, 16384)]
    [TouchPortalActionMapping("ELEVATOR_TRIM_SET")]
    public static readonly object ElevatorTrimSet;


    [TouchPortalAction("RudderTrim", "Rudder Trim", "MSFS", "Rudder Trim", "Rudder Trim - {0}", true)]
    [TouchPortalActionChoice(new [] { "Left", "Right" })]
    [TouchPortalActionMapping("RUDDER_TRIM_LEFT", "Left")]
    [TouchPortalActionMapping("RUDDER_TRIM_RIGHT", "Right")]
    public static readonly object RudderTrim;

    [TouchPortalAction("RudderTrimSet", "Rudder Trim Set", "MSFS", " Set Rudder Trim", "Rudder Trim set to {0}% (-100 - +100)")]
    [TouchPortalActionText("0", -100, 100)]
    [TouchPortalActionMapping("RUDDER_TRIM_SET")]
    public static readonly object RudderTrimSet;

    #endregion
  }
}
