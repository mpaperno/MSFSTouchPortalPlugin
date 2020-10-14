using MSFSTouchPortalPlugin.Attributes;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.Plugin {
  [TouchPortalCategory("Plugin", "MSFS - Plugin")]
  internal static class PluginMapping {
    [TouchPortalAction("Connection", "Connection", "MSFS", "Toggle/On/Off SimConnect Connection", "SimConnect Connection - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" }, "Toggle")]
    [TouchPortalState("Connected", "text", "The status of SimConnect", "false")]
    public static object Connection { get; }
  }

  [InternalEvent]
  [TouchPortalCategoryMapping("Plugin")]
  internal enum Plugin : short {
    // Starting point
    Init = 255,

    [TouchPortalActionMapping("Connection", "Toggle")]
    ToggleConnection,
    [TouchPortalActionMapping("Connection", "On")]
    Connect,
    [TouchPortalActionMapping("Connection", "Off")]
    Disconnect
  }
}
