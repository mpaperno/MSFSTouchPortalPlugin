using MSFSTouchPortalPlugin.Attributes;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.FlightSystems {
  [TouchPortalCategory("FlightSystems", "MSFS - Flight Systems")]
  internal class FlightSystemsMapping {
  }

  [SimNotificationGroup(SimConnectWrapper.Groups.FlightSystems)]
  [TouchPortalCategoryMapping("FlightSystems")]
  internal enum FlightSystems {
    // Placeholder to offset each enum for SimConnect
    Init = 7000,

    // TODO: Ailerons, Flags, Trims
  }
}
