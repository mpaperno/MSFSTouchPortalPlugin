using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Enums;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.InstrumentsSystems {
  [SimVarDataRequestGroup]
  [TouchPortalCategory("Communication", "MSFS - Communication")]
  internal class CommunicationMapping {
  }

  [SimNotificationGroup(Groups.Communication)]
  [TouchPortalCategoryMapping("Communication")]
  internal enum Communication {
    // Placeholder to offset each enum for SimConnect
    Init = 10000,
  }
}
