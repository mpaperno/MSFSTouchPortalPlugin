using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Enums;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.Menu {
  [SimVarDataRequestGroup]
  [TouchPortalCategory("Menu", "MSFS - Menu")]
  internal class MenuMapping {
    //[TouchPortalAction("Pause", "Pause", "MSFS", "Toggle/On/Off Pause", "Pause - {0}")]
    //[TouchPortalActionChoice(new string[] { "Toggle", "On", "Off" }, "Toggle")]
    //public object PAUSE { get; }
  }

  [SimNotificationGroup(Groups.Menu)]
  [TouchPortalCategoryMapping("Menu")]
  internal enum Menu {
    // Placeholder to offset each enum for SimConnect
    Init = 3000,
  }
}
