using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Constants;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.InstrumentsSystems {
  [SimVarDataRequestGroup]
  [TouchPortalCategory("FlightInstruments", "MSFS - Flight Instruments")]
  internal class FlightInstrumentsMapping {

    [SimVarDataRequest]
    [TouchPortalState("GroundVelocity", "text", "Ground Speed in Knots", "")]
    public static SimVarItem GroundVelocity = new SimVarItem() { def = Definition.GroundVelocity, req = Request.GroundVelocity, SimVarName = "GROUND VELOCITY", Unit = Units.knots, CanSet = false };
  }

  [SimNotificationGroup(SimConnectWrapper.Groups.FlightInstruments)]
  [TouchPortalCategoryMapping("FlightInstruments")]
  internal enum FlightInstruments {
    // Placeholder to offset each enum for SimConnect
    Init = 7000,
  }
}
