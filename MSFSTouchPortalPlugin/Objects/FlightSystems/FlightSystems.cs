using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Enums;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.FlightSystems
{
  [SimVarDataRequestGroup]
  [SimNotificationGroup(Groups.FlightSystems)]
  [TouchPortalCategory("FlightSystems", "MSFS - Flight Systems")]
  internal static class FlightSystemsMapping {
    #region Ailerons

    [TouchPortalAction("Ailerons", "Ailerons", "MSFS", "Ailerons", "Ailerons - {0}", true)]
    [TouchPortalActionChoice(new [] { "Center", "Left", "Right" })]
    [TouchPortalActionMapping("CENTER_AILER_RUDDER", "Center")]
    [TouchPortalActionMapping("AILERONS_LEFT", "Left")]
    [TouchPortalActionMapping("AILERONS_RIGHT", "Right")]
    public static object Ailerons { get; }

    [TouchPortalAction("AileronsSet", "Ailerons Set", "MSFS", " Set Ailerons", "Ailerons set to {0} (-16383 - +16383)")]
    [TouchPortalActionText("0", -16383, 16383)]
    [TouchPortalActionMapping("AILERON_SET")]
    public static object AileronsSet { get; }

    #endregion

    #region Elevator

    [TouchPortalAction("Elevator", "Elevator", "MSFS", "Elevator", "Elevator - {0}", true)]
    [TouchPortalActionChoice(new[] { "Up", "Down" })]
    [TouchPortalActionMapping("ELEV_UP", "Up")]
    [TouchPortalActionMapping("ELEV_DOWN", "Down")]
    public static object Elevator { get; }

    [TouchPortalAction("ElevatorSet", "Elevator Set", "MSFS", " Set Elevator", "Elevator set to {0} (-16383 - +16383)")]
    [TouchPortalActionText("0", -16383, 16383)]
    [TouchPortalActionMapping("ELEVATOR_SET")]
    public static object ElevatorSet { get; }

    #endregion

    #region Brakes

    [TouchPortalAction("Brakes", "Brakes", "MSFS", "Brakes", "Brakes - {0}", true)]
    [TouchPortalActionChoice(new [] { "All", "Left", "Right" })]
    [TouchPortalActionMapping("BRAKES", "All")]
    [TouchPortalActionMapping("BRAKES_LEFT", "Left")]
    [TouchPortalActionMapping("BRAKES_RIGHT", "Right")]
    public static object Brakes { get; }

    [SimVarDataRequest]
    [TouchPortalAction("ParkingBreak", "Toggle Parking Brake", "MSFS", "Toggle Parking Brake", "Toggle Parking Brake")]
    [TouchPortalActionMapping("PARKING_BRAKES")]
    [TouchPortalState("ParkingBrakeIndicator", "text", "Parking Brake Indicator true/false", "")]
    public static readonly SimVarItem ParkingBrake = new SimVarItem { Def = Definition.ParkingBrakeIndicator, SimVarName = "BRAKE PARKING POSITION", Unit = Units.Bool, CanSet = false };

    #endregion

    #region Flaps

    [SimVarDataRequest]
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
    [TouchPortalState("FlapsHandlePercent", "text", "Flaps Handle Percentage", "")]
    public static readonly SimVarItem FlapsHandlePercent = new SimVarItem { Def = Definition.FlapsHandlePercent, SimVarName = "FLAPS HANDLE PERCENT", Unit = Units.percent, CanSet = false, StringFormat = "{0:0.0#}" };

    [TouchPortalAction("FlapsSet", "Flaps Set", "MSFS", " Set Flaps", "Flaps set to {0} (0 - +16383)")]
    [TouchPortalActionText("0", 0, 16383)]
    [TouchPortalActionMapping("FLAPS_SET")]
    public static object FlapsSet { get; }


    [TouchPortalAction("CowlFlapsAll", "Cowl Flaps All", "MSFS", "Cowl Flaps All", "Cowl Flaps All - {0}", true)]
    [TouchPortalActionChoice(new [] { "Increase", "Decrease" })]
    [TouchPortalActionMapping("INC_COWL_FLAPS", "Increase")]
    [TouchPortalActionMapping("DEC_COWL_FLAPS", "Decrease")]
    public static object CowlFlapsAll { get; }

    [TouchPortalAction("CowlFlaps1", "Cowl Flaps 1", "MSFS", "Cowl Flaps 1", "Cowl Flaps 1 - {0}", true)]
    [TouchPortalActionChoice(new [] { "Increase", "Decrease" })]
    [TouchPortalActionMapping("INC_COWL_FLAPS1", "Increase")]
    [TouchPortalActionMapping("DEC_COWL_FLAPS1", "Decrease")]
    [TouchPortalState("CowlFlaps1Percent", "text", "Cowl Flaps 1 Opened Percentage", "")]
    public static readonly SimVarItem CowlFlaps1 = new SimVarItem { Def = Definition.CowlFlaps1Percent, SimVarName = "RECIP ENG COWL FLAP POSITION:1", Unit = Units.percent, CanSet = true, StringFormat = "{0:0.0#}" };

    [TouchPortalAction("CowlFlaps2", "Cowl Flaps 2", "MSFS", "Cowl Flaps 2", "Cowl Flaps 2 - {0}", true)]
    [TouchPortalActionChoice(new [] { "Increase", "Decrease" })]
    [TouchPortalActionMapping("INC_COWL_FLAPS2", "Increase")]
    [TouchPortalActionMapping("DEC_COWL_FLAPS2", "Decrease")]
    [TouchPortalState("CowlFlaps2Percent", "text", "Cowl Flaps 2 Opened Percentage", "")]
    public static readonly SimVarItem CowlFlaps2 = new SimVarItem { Def = Definition.CowlFlaps2Percent, SimVarName = "RECIP ENG COWL FLAP POSITION:2", Unit = Units.percent, CanSet = true, StringFormat = "{0:0.0#}" };

    [TouchPortalAction("CowlFlaps3", "Cowl Flaps 3", "MSFS", "Cowl Flaps 3", "Cowl Flaps 3 - {0}", true)]
    [TouchPortalActionChoice(new [] { "Increase", "Decrease" })]
    [TouchPortalActionMapping("INC_COWL_FLAPS3", "Increase")]
    [TouchPortalActionMapping("DEC_COWL_FLAPS3", "Decrease")]
    [TouchPortalState("CowlFlaps3Percent", "text", "Cowl Flaps 3 Opened Percentage", "")]
    public static readonly SimVarItem CowlFlaps3 = new SimVarItem { Def = Definition.CowlFlaps3Percent, SimVarName = "RECIP ENG COWL FLAP POSITION:3", Unit = Units.percent, CanSet = true, StringFormat = "{0:0.0#}" };

    [TouchPortalAction("CowlFlaps4", "Cowl Flaps 4", "MSFS", "Cowl Flaps 4", "Cowl Flaps 4 - {0}", true)]
    [TouchPortalActionChoice(new [] { "Increase", "Decrease" })]
    [TouchPortalActionMapping("INC_COWL_FLAPS4", "Increase")]
    [TouchPortalActionMapping("DEC_COWL_FLAPS4", "Decrease")]
    [TouchPortalState("CowlFlaps4Percent", "text", "Cowl Flaps 4 Opened Percentage", "")]
    public static readonly SimVarItem CowlFlaps4 = new SimVarItem { Def = Definition.CowlFlaps4Percent, SimVarName = "RECIP ENG COWL FLAP POSITION:4", Unit = Units.percent, CanSet = true, StringFormat = "{0:0.0#}" };

    #endregion

    #region Gear

    [SimVarDataRequest]
    [TouchPortalAction("Gear", "Gear Manipulation", "MSFS", "Gear Manipulation", "Gear - {0}", true)]
    [TouchPortalActionChoice(new [] { "Toggle", "Up", "Down", "Pump" })]
    [TouchPortalActionMapping("GEAR_TOGGLE", "Toggle")]
    [TouchPortalActionMapping("GEAR_UP", "Up")]
    [TouchPortalActionMapping("GEAR_DOWN", "Down")]
    [TouchPortalActionMapping("GEAR_PUMP", "Pump")]
    [TouchPortalState("GearTotalExtended", "text", "Total percentage of gear extended", "")]
    public static readonly SimVarItem Gear =
      new SimVarItem { Def = Definition.GearTotalExtended, SimVarName = "GEAR TOTAL PCT EXTENDED", Unit = Units.percentage, CanSet = false };

    #endregion

    #region Rudder

    [TouchPortalAction("Rudder", "Rudder", "MSFS", "Rudder", "Rudder - {0}", true)]
    [TouchPortalActionChoice(new [] { "Center", "Left", "Right" })]
    [TouchPortalActionMapping("RUDDER_CENTER", "Center")]
    [TouchPortalActionMapping("RUDDER_LEFT", "Left")]
    [TouchPortalActionMapping("RUDDER_RIGHT", "Right")]
    public static object Rudder { get; }

    [TouchPortalAction("RudderSet", "Rudder Set", "MSFS", " Set Rudder", "Rudder set to {0} (-16383 - +16383)")]
    [TouchPortalActionText("0", -16383, 16383)]
    [TouchPortalActionMapping("RUDDER_SET")]
    public static object RudderSet { get; }

    #endregion

    #region Spoilers

    [TouchPortalAction("Spoilers", "Spoilers", "MSFS", "Spoilers", "Spoilers - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("SPOILERS_TOGGLE", "Toggle")]
    [TouchPortalActionMapping("SPOILERS_ON", "On")]
    [TouchPortalActionMapping("SPOILERS_OFF", "Off")]
    public static object Spoilers { get; }

    [TouchPortalAction("SpoilersSet", "Spoilers Set", "MSFS", " Set Spoilers", "Spoilers set to {0} (0- +16383)")]
    [TouchPortalActionText("0", 0, 16383)]
    [TouchPortalActionMapping("SPOILERS_SET")]
    public static object SpoilersSet { get; }

    [TouchPortalAction("SpoilersArm", "Spoilers Arm", "MSFS", "Spoilers Arm", "Spoilers Arm - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("SPOILERS_ARM_TOGGLE", "Toggle")]
    [TouchPortalActionMapping("SPOILERS_ARM_ON", "On")]
    [TouchPortalActionMapping("SPOILERS_ARM_OFF", "Off")]
    public static object SpoilersArm { get; }

    #endregion

    #region Trimming

    [SimVarDataRequest]
    [TouchPortalAction("AileronTrim", "Aileron Trim", "MSFS", "Aileron Trim", "Aileron Trim - {0}", true)]
    [TouchPortalActionChoice(new [] { "Left", "Right" }, "Left")]
    [TouchPortalActionMapping("AILERON_TRIM_LEFT", "Left")]
    [TouchPortalActionMapping("AILERON_TRIM_RIGHT", "Right")]
    [TouchPortalState("AileronTrim", "text", "Aileron Trim Angle", "")]
    public static readonly SimVarItem AileronTrim =
      new SimVarItem { Def = Definition.AileronTrim, SimVarName = "AILERON TRIM", Unit = Units.degrees, CanSet = false, StringFormat = "{0:0.0#}" };

    [TouchPortalAction("AileronTrimSet", "Aileron Trim Set", "MSFS", " Set Aileron Trim", "Aileron Trim set to {0}% (-100 - +100)")]
    [TouchPortalActionText("0", -100, 100)]
    [TouchPortalActionMapping("AILERON_TRIM_SET")]
    public static object AileronTrimSet { get; }


    [SimVarDataRequest]
    [TouchPortalAction("ElevatorTrim", "Elevator Trim", "MSFS", "Elevator Trim", "Elevator Trim - {0}", true)]
    [TouchPortalActionChoice(new [] { "Up", "Down" })]
    [TouchPortalActionMapping("ELEV_TRIM_DN", "Down")]
    [TouchPortalActionMapping("ELEV_TRIM_UP", "Up")]
    [TouchPortalState("ElevatorTrim", "text", "Elevator Trim Angle", "")]
    public static readonly SimVarItem ElevatorTrim =
      new SimVarItem { Def = Definition.ElevatorTrim, SimVarName = "ELEVATOR TRIM POSITION", Unit = Units.degrees, CanSet = true, StringFormat = "{0:0.0#}" };

    [TouchPortalAction("ElevatorTrimSet", "Elevator Trim Set", "MSFS", " Set Elevator Trim", "Elevator Trim set to {0} (-16383 - +16383)")]
    [TouchPortalActionText("0", -16383, 16383)]
    [TouchPortalActionMapping("ELEVATOR_TRIM_SET")]
    public static object ElevatorTrimSet { get; }


    [SimVarDataRequest]
    [TouchPortalAction("RudderTrim", "Rudder Trim", "MSFS", "Rudder Trim", "Rudder Trim - {0}", true)]
    [TouchPortalActionChoice(new [] { "Left", "Right" })]
    [TouchPortalActionMapping("RUDDER_TRIM_LEFT", "Left")]
    [TouchPortalActionMapping("RUDDER_TRIM_RIGHT", "Right")]
    [TouchPortalState("RudderTrim", "text", "Rudder Trim Angle", "")]
    public static readonly SimVarItem RudderTrim =
      new SimVarItem { Def = Definition.RudderTrim, SimVarName = "RUDDER TRIM", Unit = Units.degrees, CanSet = false, StringFormat = "{0:0.0#}" };

    [TouchPortalAction("RudderTrimSet", "Rudder Trim Set", "MSFS", " Set Rudder Trim", "Rudder Trim set to {0}% (-100 - +100)")]
    [TouchPortalActionText("0", -100, 100)]
    [TouchPortalActionMapping("RUDDER_TRIM_SET")]
    public static object RudderTrimSet { get; }


    [SimVarDataRequest] // XYZ
    [TouchPortalState("AileronTrimPct", "text", "Aileron Trim Percent", "0")]
    public static readonly SimVarItem AileronTrimPct =
      new SimVarItem { Def = Definition.AileronTrimPct, SimVarName = "AILERON TRIM PCT", Unit = Units.percentover100, CanSet = true, StringFormat = "{0:F1}" };

    [SimVarDataRequest]
    [TouchPortalState("ElevatorTrimPct", "text", "Elevator Trim Percent", "0")]
    public static readonly SimVarItem ElevatorTrimPct =
      new SimVarItem { Def = Definition.ElevatorTrimPct, SimVarName = "ELEVATOR TRIM PCT", Unit = Units.percentover100, CanSet = false, StringFormat = "{0:F1}" };

    [SimVarDataRequest]
    [TouchPortalState("RudderTrimPct", "text", "Rudder Trim Percent", "0")]
    public static readonly SimVarItem RudderTrimPct =
      new SimVarItem { Def = Definition.RudderTrimPct, SimVarName = "RUDDER TRIM PCT", Unit = Units.percentover100, CanSet = true, StringFormat = "{0:F1}" };
    #endregion
  }
}
