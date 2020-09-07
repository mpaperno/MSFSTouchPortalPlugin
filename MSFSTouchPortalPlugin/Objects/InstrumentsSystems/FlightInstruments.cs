using MSFSTouchPortalPlugin.Attributes;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.InstrumentsSystems {
  [TouchPortalCategory("FlightInstruments", "MSFS - Flight Instruments")]
  internal class FlightInstrumentsMapping {
  }

  [SimNotificationGroup(SimConnectWrapper.Groups.FlightInstruments)]
  [TouchPortalCategoryMapping("FlightInstruments")]
  internal enum FlightInstruments {
    // Placeholder to offset each enum for SimConnect
    Init = 7000,
  }
}
