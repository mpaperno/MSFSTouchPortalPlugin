using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Enums;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.FlightSystems {
  [SimVarDataRequestGroup]
  [TouchPortalCategory("FlightSystems", "MSFS - Flight Systems")]
  internal class FlightSystemsMapping {
    #region Ailerons

    [TouchPortalAction("Ailerons", "Ailerons", "MSFS", "Ailerons", "Ailerons - {0}")]
    [TouchPortalActionChoice(new string[] { "Center", "Left", "Right", "Set" }, "Center")]
    public object AILERONS { get; }

    #endregion

    #region Brakes

    [TouchPortalAction("Brakes", "Brakes", "MSFS", "Brakes", "Brakes - {0}")]
    [TouchPortalActionChoice(new string[] { "All", "Left", "Right" }, "All")]
    public object BRAKES { get; }

    [TouchPortalAction("ParkingBreak", "Toggle Parking Brake", "MSFS", "Toggle Parking Brake", "Toggle Parking Brake")]
    public object PARKING_BRAKE { get; }

    #endregion

    #region Flaps

    [TouchPortalAction("Flaps", "Flaps", "MSFS", "Flaps", "Flaps - {0}")]
    [TouchPortalActionChoice(new string[] { "Up", "Down", "Increase", "Decrease", "1", "2", "3", "Set" }, "Up")]
    public object FLAPS { get; }

    [TouchPortalAction("CowlFlapsAll", "Cowl Flaps All", "MSFS", "Cowl Flaps All", "Cowl Flaps All - {0}")]
    [TouchPortalActionChoice(new string[] { "Increase", "Decrease" }, "Increase")]
    public object COWL_FLAPS_All { get; }

    [TouchPortalAction("CowlFlaps1", "Cowl Flaps 1", "MSFS", "Cowl Flaps 1", "Cowl Flaps 1 - {0}")]
    [TouchPortalActionChoice(new string[] { "Increase", "Decrease" }, "Increase")]
    public object COWL_FLAPS_1 { get; }

    [TouchPortalAction("CowlFlaps2", "Cowl Flaps 2", "MSFS", "Cowl Flaps 2", "Cowl Flaps 2 - {0}")]
    [TouchPortalActionChoice(new string[] { "Increase", "Decrease" }, "Increase")]
    public object COWL_FLAPS_2 { get; }

    [TouchPortalAction("CowlFlaps3", "Cowl Flaps 3", "MSFS", "Cowl Flaps 3", "Cowl Flaps 3 - {0}")]
    [TouchPortalActionChoice(new string[] { "Increase", "Decrease" }, "Increase")]
    public object COWL_FLAPS_3 { get; }

    [TouchPortalAction("CowlFlaps4", "Cowl Flaps 4", "MSFS", "Cowl Flaps 4", "Cowl Flaps 4 - {0}")]
    [TouchPortalActionChoice(new string[] { "Increase", "Decrease" }, "Increase")]
    public object COWL_FLAPS_4 { get; }

    #endregion

    #region Gear

    [SimVarDataRequest]
    [TouchPortalAction("Gear", "Gear Manipulation", "MSFS", "Gear Manipulation", "Gear - {0}")]
    [TouchPortalActionChoice(new string[] { "Toggle", "Up", "Down", "Set", "Pump" }, "Toggle")]
    [TouchPortalState("GearTotalExtended", "text", "Total percentage of gear extended", "")]
    public static SimVarItem GEAR =
      new SimVarItem() { def = Definition.GearTotalExtended, req = Request.GearTotalExtended, SimVarName = "GEAR TOTAL PCT EXTENDED", Unit = Units.percentage, CanSet = false };

    #endregion

    #region Rudder

    [TouchPortalAction("Rudder", "Rudder", "MSFS", "Rudder", "Rudder - {0}")]
    [TouchPortalActionChoice(new string[] { "Center", "Left", "Right", "Set" }, "Center")]
    public object RUDDER { get; }

    #endregion

    #region Spoilers

    [TouchPortalAction("Spoilers", "Spoilers", "MSFS", "Spoilers", "Spoilers - {0}")]
    [TouchPortalActionChoice(new string[] { "Toggle", "On", "Off", "Set" }, "Toggle")]
    public object SPOILERS { get; }

    [TouchPortalAction("SpoilersArm", "Spoilers Arm", "MSFS", "Spoilers Arm", "Spoilers Arm - {0}")]
    [TouchPortalActionChoice(new string[] { "Toggle", "On", "Off", "Set" }, "Toggle")]
    public object SPOILERS_ARM { get; }

    #endregion

    #region Trimming

    [SimVarDataRequest]
    [TouchPortalAction("AileronTrim", "Aileron Trim", "MSFS", "Aileron Trim", "Aileron Trim - {0}")]
    [TouchPortalActionChoice(new string[] { "Left", "Right" }, "Left")]
    [TouchPortalState("AileronTrim", "text", "Aileron Trim Angle", "")]
    public static SimVarItem AILERON_TRIM =
      new SimVarItem() { def = Definition.AileronTrim, req = Request.AileronTrim, SimVarName = "AILERON TRIM", Unit = Units.radians, CanSet = false };

    [SimVarDataRequest]
    [TouchPortalAction("ElevatorTrim", "Elevator Trim", "MSFS", "Elevator Trim", "Elevator Trim - {0}")]
    [TouchPortalActionChoice(new string[] { "Up", "Down" }, "Up")]
    [TouchPortalState("ElevatorTrim", "text", "Elevator Trim Angle", "")]
    public static SimVarItem ELEVATOR_TRIM =
      new SimVarItem() { def = Definition.ElevatorTrim, req = Request.ElevatorTrim, SimVarName = "", Unit = Units.radians, CanSet = false };
    
    [SimVarDataRequest]
    [TouchPortalAction("RudderTrim", "Rudder Trim", "MSFS", "Rudder Trim", "Rudder Trim - {0}")]
    [TouchPortalActionChoice(new string[] { "Left", "Right" }, "Left")]
    [TouchPortalState("RudderTrim", "text", "Rudder Trim Angle", "")]
    public static SimVarItem RUDDER_TRIM =
            new SimVarItem() { def = Definition.RudderTrim, req = Request.RudderTrim, SimVarName = "RUDDER TRIM", Unit = Units.radians, CanSet = false };


    [SimVarDataRequest] // XYZ
    public static SimVarItem AileronTrimPct = new SimVarItem() { def = Definition.AileronTrimPct, req = Request.AileronTrimPct, SimVarName = "", Unit = Units.number, CanSet = true };
    [SimVarDataRequest]
    public static SimVarItem RudderTrimPct = new SimVarItem() { def = Definition.RudderTrimPct, req = Request.RudderTrimPct, SimVarName = "RUDDER TRIM PCT", Unit = Units.percentover100, CanSet = true };

    #endregion
  }

  [SimNotificationGroup(Groups.FlightSystems)]
  [TouchPortalCategoryMapping("FlightSystems")]
  internal enum FlightSystems {
    // Placeholder to offset each enum for SimConnect
    Init = 7000,

    #region Ailerons

    [SimActionEvent]
    [TouchPortalActionMapping("Ailerons", "Set")]
    AILERON_SET,
    [SimActionEvent]
    [TouchPortalActionMapping("Ailerons", "Left")]
    AILERONS_LEFT,
    [SimActionEvent]
    [TouchPortalActionMapping("Ailerons", "Center")]
    CENTER_AILER_RUDDER,
    [SimActionEvent]
    [TouchPortalActionMapping("Ailerons", "Right")]
    AILERONS_RIGHT,

    #endregion

    #region Brakes

    [SimActionEvent]
    [TouchPortalActionMapping("Brakes", "All")]
    BRAKES,
    [SimActionEvent]
    [TouchPortalActionMapping("Brakes", "Left")]
    BRAKES_LEFT,
    [SimActionEvent]
    [TouchPortalActionMapping("Brakes", "Right")]
    BRAKES_RIGHT,

    [SimActionEvent]
    [TouchPortalActionMapping("ParkingBreak")]
    PARKING_BRAKES,

    #endregion

    #region Flaps

    [SimActionEvent]
    [TouchPortalActionMapping("Flaps", "Up")]
    FLAPS_UP,
    [SimActionEvent]
    [TouchPortalActionMapping("Flaps", "Down")]
    FLAPS_DOWN,
    [SimActionEvent]
    [TouchPortalActionMapping("Flaps", "1")]
    FLAPS_1,
    [SimActionEvent]
    [TouchPortalActionMapping("Flaps", "2")]
    FLAPS_2,
    [SimActionEvent]
    [TouchPortalActionMapping("Flaps", "3")]
    FLAPS_3,
    [SimActionEvent]
    [TouchPortalActionMapping("Flaps", "Increase")]
    FLAPS_INCR,
    [SimActionEvent]
    [TouchPortalActionMapping("Flaps", "Decrease")]
    FLAPS_DECR,
    [SimActionEvent]
    [TouchPortalActionMapping("Flaps", "Set")]
    FLAPS_SET,

    [SimActionEvent]
    [TouchPortalActionMapping("CowlFlapsAll", "Increase")]
    INC_COWL_FLAPS,
    [SimActionEvent]
    [TouchPortalActionMapping("CowlFlapsAll", "Decrease")]
    DEC_COWL_FLAPS,
    [SimActionEvent]
    [TouchPortalActionMapping("CowlFlaps1", "Increase")]
    INC_COWL_FLAPS1,
    [SimActionEvent]
    [TouchPortalActionMapping("CowlFlaps1", "Decrease")]
    DEC_COWL_FLAPS1,
    [SimActionEvent]
    [TouchPortalActionMapping("CowlFlaps2", "Increase")]
    INC_COWL_FLAPS2,
    [SimActionEvent]
    [TouchPortalActionMapping("CowlFlaps2", "Decrease")]
    DEC_COWL_FLAPS2,
    [SimActionEvent]
    [TouchPortalActionMapping("CowlFlaps3", "Increase")]
    INC_COWL_FLAPS3,
    [SimActionEvent]
    [TouchPortalActionMapping("CowlFlaps3", "Decrease")]
    DEC_COWL_FLAPS3,
    [SimActionEvent]
    [TouchPortalActionMapping("CowlFlaps4", "Increase")]
    INC_COWL_FLAPS4,
    [SimActionEvent]
    [TouchPortalActionMapping("CowlFlaps4", "Decrease")]
    DEC_COWL_FLAPS4,

    #endregion

    #region Gear

    [SimActionEvent]
    [TouchPortalActionMapping("Gear", "Toggle")]
    GEAR_TOGGLE,
    [SimActionEvent]
    [TouchPortalActionMapping("Gear", "Set")]
    GEAR_SET,
    [SimActionEvent]
    [TouchPortalActionMapping("Gear", "Pump")]
    GEAR_PUMP,
    [SimActionEvent]
    [TouchPortalActionMapping("Gear", "Up")]
    GEAR_UP,
    [SimActionEvent]
    [TouchPortalActionMapping("Gear", "Down")]
    GEAR_DOWN,

    #endregion

    #region Rudder

    [SimActionEvent]
    [TouchPortalActionMapping("Rudder", "Set")]
    RUDDER_SET,
    [SimActionEvent]
    [TouchPortalActionMapping("Rudder", "Left")]
    RUDDER_LEFT,
    [SimActionEvent]
    [TouchPortalActionMapping("Rudder", "Center")]
    RUDDER_CENTER,
    [SimActionEvent]
    [TouchPortalActionMapping("Rudder", "Right")]
    RUDDER_RIGHT,

    #endregion

    #region Spoilers

    [SimActionEvent]
    [TouchPortalActionMapping("Spoilers", "Toggle")]
    SPOILERS_TOGGLE,
    [SimActionEvent]
    [TouchPortalActionMapping("Spoilers", "On")]
    SPOILERS_ON,
    [SimActionEvent]
    [TouchPortalActionMapping("Spoilers", "Off")]
    SPOILERS_OFF,
    [SimActionEvent]
    [TouchPortalActionMapping("Spoilers", "Set")]
    SPOILERS_SET,

    [SimActionEvent]
    [TouchPortalActionMapping("SpoilersArm", "Toggle")]
    SPOILERS_ARM_TOGGLE,
    [SimActionEvent]
    [TouchPortalActionMapping("SpoilersArm", "On")]
    SPOILERS_ARM_ON,
    [SimActionEvent]
    [TouchPortalActionMapping("SpoilersArm", "Off")]
    SPOILERS_ARM_OFF,
    [SimActionEvent]
    [TouchPortalActionMapping("SpoilersArm", "Set")]
    SPOILERS_ARM_SET,

    #endregion

    #region Trimming

    [SimActionEvent]
    [TouchPortalActionMapping("AileronTrim", "Left")]
    AILERON_TRIM_LEFT,
    [SimActionEvent]
    [TouchPortalActionMapping("AileronTrim", "Right")]
    AILERON_TRIM_RIGHT,

    [SimActionEvent]
    [TouchPortalActionMapping("ElevatorTrim", "Down")]
    ELEV_TRIM_DN,
    [SimActionEvent]
    [TouchPortalActionMapping("ElevatorTrim", "Up")]
    ELEV_TRIM_UP,

    [SimActionEvent]
    [TouchPortalActionMapping("RudderTrim", "Left")]
    RUDDER_TRIM_LEFT,
    [SimActionEvent]
    [TouchPortalActionMapping("RudderTrim", "Right")]
    RUDDER_TRIM_RIGHT,

    #endregion
  }
}
