using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Enums;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.InstrumentsSystems {
  [SimVarDataRequestGroup]
  [TouchPortalCategory("FlightInstruments", "MSFS - Flight Instruments")]
  internal static class FlightInstrumentsMapping {

    #region Velocity

    [SimVarDataRequest]
    [TouchPortalState("GroundVelocity", "text", "Ground Speed in Knots", "")]
    public static readonly SimVarItem GroundVelocity = new SimVarItem { Def = Definition.GroundVelocity, SimVarName = "GROUND VELOCITY", Unit = Units.knots, CanSet = false, StringFormat = "{0:0.0#}" };

    [SimVarDataRequest]
    [TouchPortalState("AirSpeedTrue", "text", "Air speed true in Knots", "")]
    public static readonly SimVarItem AirSpeedTrue = new SimVarItem { Def = Definition.AirSpeedTrue, SimVarName = "AIRSPEED TRUE", Unit = Units.knots, CanSet = true, StringFormat = "{0:0.0#}" };

    [SimVarDataRequest]
    [TouchPortalState("AirSpeedIndicated", "text", "Air speed indicated in Knots", "")]
    public static readonly SimVarItem AirSpeedIndicated = new SimVarItem { Def = Definition.AirSpeedIndicated, SimVarName = "AIRSPEED INDICATED", Unit = Units.knots, CanSet = true, StringFormat = "{0:0.0#}" };

    [SimVarDataRequest]
    [TouchPortalState("AirSpeedMach", "text", "Air speed indicated in mach", "")]
    public static readonly SimVarItem AirSpeedMach = new SimVarItem { Def = Definition.AirSpeedMach, SimVarName = "AIRSPEED MACH", Unit = Units.mach, CanSet = true, StringFormat = "{0:0.0#}" };
    #endregion

    #region Altitude

    [SimVarDataRequest]
    [TouchPortalState("PlaneAltitude", "text", "Plane Altitude in Feet", "")]
    public static readonly SimVarItem PlaneAltitude = new SimVarItem { Def = Definition.PlaneAltitude, SimVarName = "PLANE ALTITUDE", Unit = Units.feet, CanSet = false, StringFormat = "{0:0.#}" };

    [SimVarDataRequest]
    [TouchPortalState("PlaneAltitudeAGL", "text", "Plane Altitude AGL in Feet", "")]
    public static readonly SimVarItem PlaneAltitudeAGL = new SimVarItem { Def = Definition.PlaneAltitudeAGL, SimVarName = "PLANE ALT ABOVE GROUND", Unit = Units.feet, CanSet = false, StringFormat = "{0:0.#}" };

    [SimVarDataRequest]
    [TouchPortalState("GroundAltitude", "text", "Ground level in Feet", "")]
    public static readonly SimVarItem GroundAltitude = new SimVarItem { Def = Definition.GroundAltitude, SimVarName = "GROUND ALTITUDE", Unit = Units.feet, CanSet = false, StringFormat = "{0:0.#}" };

    #endregion

    #region Heading

    [SimVarDataRequest]
    [TouchPortalState("PlaneHeadingTrue", "text", "Plane Heading (True North) in Degrees", "")]
    public static readonly SimVarItem PlaneHeadingTrue = new SimVarItem { Def = Definition.PlaneHeadingTrue, SimVarName = "PLANE HEADING DEGREES TRUE", Unit = Units.radians, CanSet = false, StringFormat = "{0:0}" };

    [SimVarDataRequest]
    [TouchPortalState("PlaneHeadingMagnetic", "text", "Plane Heading (Magnetic North) in Degrees", "")]
    public static readonly SimVarItem PlaneHeadingMagnetic = new SimVarItem { Def = Definition.PlaneHeadingMagnetic, SimVarName = "PLANE HEADING DEGREES MAGNETIC", Unit = Units.radians, CanSet = false, StringFormat = "{0:0}" };

    #endregion

    #region Bank and Pitch

    [SimVarDataRequest]
    [TouchPortalState("PlaneBankAngle", "text", "Plane Bank Angle in Degrees", "")]
    public static readonly SimVarItem PlaneBankAngle = new SimVarItem { Def = Definition.PlaneBankAngle, SimVarName = "PLANE BANK DEGREES", Unit = Units.radians, CanSet = false, StringFormat = "{0:0}" };

    [SimVarDataRequest]
    [TouchPortalState("PlanePitchAngle", "text", "Plane Pitch Angle in Degrees", "")]
    public static readonly SimVarItem PlanePitchAngle = new SimVarItem { Def = Definition.PlanePitchAngle, SimVarName = "PLANE PITCH DEGREES", Unit = Units.radians, CanSet = false, StringFormat = "{0:0}" };

    [SimVarDataRequest]
    [TouchPortalState("VerticalSpeed", "text", "Vertical Speed in feet per minute", "")]
    public static readonly SimVarItem VerticalSpeed = new SimVarItem { Def = Definition.VerticalSpeed, SimVarName = "VERTICAL SPEED", Unit = Units.feetminute, CanSet = true, StringFormat = "{0:0.0#}" };

      #endregion

      #region Warnings

    [SimVarDataRequest]
    [TouchPortalState("StallWarning", "text", "Stall Warning true/false", "")]
    public static readonly SimVarItem StallWarning = new SimVarItem { Def = Definition.StallWarning, SimVarName = "STALL WARNING", Unit = Units.Bool, CanSet = false };

    [SimVarDataRequest]
    [TouchPortalState("OverspeedWarning", "text", "Overspeed Warning true/false", "")]
    public static readonly SimVarItem OverspeedWarning = new SimVarItem { Def = Definition.OverspeedWarning, SimVarName = "OVERSPEED WARNING", Unit = Units.Bool, CanSet = false };

    [SimVarDataRequest]
    [TouchPortalState("FlapSpeedExceeeded", "text", "Flap Speed Exceeded Warning true/false", "")]
    public static readonly SimVarItem FlapSpeedExceeeded = new SimVarItem { Def = Definition.FlapSpeedExceeeded, SimVarName = "FLAP SPEED EXCEEDED", Unit = Units.Bool, CanSet = false };

    #endregion

  }

  [SimNotificationGroup(Groups.FlightInstruments)]
  [TouchPortalCategoryMapping("FlightInstruments")]
  internal enum FlightInstruments {
    // Placeholder to offset each enum for SimConnect
    Init = 7000,
  }
}
