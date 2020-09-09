using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Constants;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.InstrumentsSystems {
  [SimVarDataRequestGroup]
  [TouchPortalCategory("FlightInstruments", "MSFS - Flight Instruments")]
  internal class FlightInstrumentsMapping {

    #region Velocity

    [SimVarDataRequest]
    [TouchPortalState("GroundVelocity", "text", "Ground Speed in Knots", "")]
    public static SimVarItem GroundVelocity = new SimVarItem() { def = Definition.GroundVelocity, req = Request.GroundVelocity, SimVarName = "GROUND VELOCITY", Unit = Units.knots, CanSet = false, StringFormat = "{0:0.#} KTS" };

    #endregion

    #region Altitude

    [SimVarDataRequest]
    [TouchPortalState("PlaneAltitude", "text", "Plane Altitude in Feet", "")]
    public static SimVarItem PlaneAltitude = new SimVarItem() { def = Definition.PlaneAltitude, req = Request.PlaneAltitude, SimVarName = "PLANE ALTITUDE", Unit = Units.feet, CanSet = false, StringFormat = "{0:0.#} FT" };

    [SimVarDataRequest]
    [TouchPortalState("PlaneAltitudeAGL", "text", "Plane Altitude AGL in Feet", "")]
    public static SimVarItem PlaneAltitudeAGL = new SimVarItem() { def = Definition.PlaneAltitudeAGL, req = Request.PlaneAltitudeAGL, SimVarName = "PLANE ALT ABOVE GROUND", Unit = Units.feet, CanSet = false, StringFormat = "{0:0.#} FT" };

    [SimVarDataRequest]
    [TouchPortalState("GroundAltitude", "text", "Ground level in Feet", "")]
    public static SimVarItem GroundAltitude = new SimVarItem() { def = Definition.GroundAltitude, req = Request.GroundAltitude, SimVarName = "GROUND ALTITUDE", Unit = Units.feet, CanSet = false, StringFormat = "{0:0.#} FT" };

    #endregion

    #region Heading

    [SimVarDataRequest]
    [TouchPortalState("PlaneHeadingTrue", "text", "Plane Heading (True North) in Degrees", "")]
    public static SimVarItem PlaneHeadingTrue = new SimVarItem() { def = Definition.PlaneHeadingTrue, req = Request.PlaneHeadingTrue, SimVarName = "PLANE HEADING DEGREES TRUE", Unit = Units.radians, CanSet = false, StringFormat = "{0:0} Deg." };

    [SimVarDataRequest]
    [TouchPortalState("PlaneHeadingMagnetic", "text", "Plane Heading (Magnetic North) in Degrees", "")]
    public static SimVarItem PlaneHeadingMagnetic = new SimVarItem() { def = Definition.PlaneHeadingMagnetic, req = Request.PlaneHeadingMagnetic, SimVarName = "PLANE HEADING DEGREES MAGNETIC", Unit = Units.radians, CanSet = false, StringFormat = "{0:0} Deg." };

    #endregion

    #region Bank and Pitch

    [SimVarDataRequest]
    [TouchPortalState("PlaneBankAngle", "text", "Plane Bank Angle in Degrees", "")]
    public static SimVarItem PlaneBankAngle = new SimVarItem() { def = Definition.PlaneBankAngle, req = Request.PlaneBankAngle, SimVarName = "PLANE BANK DEGREES", Unit = Units.radians, CanSet = false, StringFormat = "{0:0} Deg." };

    [SimVarDataRequest]
    [TouchPortalState("PlanePitchAngle", "text", "Plane Pitch Angle in Degrees", "")]
    public static SimVarItem PlanePitchAngle = new SimVarItem() { def = Definition.PlanePitchAngle, req = Request.PlanePitchAngle, SimVarName = "PLANE PITCH DEGREES", Unit = Units.radians, CanSet = false, StringFormat = "{0:0} Deg." };

    #endregion

  }

  [SimNotificationGroup(SimConnectWrapper.Groups.FlightInstruments)]
  [TouchPortalCategoryMapping("FlightInstruments")]
  internal enum FlightInstruments {
    // Placeholder to offset each enum for SimConnect
    Init = 7000,
  }
}
