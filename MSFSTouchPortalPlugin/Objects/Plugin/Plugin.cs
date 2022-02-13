using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Types;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.Plugin {
  [TouchPortalCategory("Plugin", "MSFS - Plugin")]
  internal static class PluginMapping {
    [TouchPortalAction("Connection", "Connection", "MSFS", "Toggle/On/Off SimConnect Connection", "SimConnect Connection - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" }, "Toggle")]
    [TouchPortalState("Connected", "text", "The status of SimConnect", "false")]
    public static object Connection { get; }
  }

  [TouchPortalCategory("Plugin", "MSFS - Plugin")]
  [TouchPortalSettingsContainer]
  public static class Settings {
    [TouchPortalSetting(
      Name = "Connect To Flight Sim on Startup (0/1)",
      Description = "Set to 1 to automatically attempt connection to flight simulator upon Touch Portal startup. Set to 0 to only connect manually via the provided Action.",
      Type = "number", Default = "1", MinValue = 0, MaxValue = 1
    )]
    public static readonly PluginSetting ConnectSimOnStartup = new PluginSetting("ConnectSimOnStartup", 0, 1);

    [TouchPortalSetting(
      Name = "Held Action Repeat Rate (ms)",
      Description = "Stores the held action repeat rate, which can be set via the 'MSFS - Plugin - Action Repeat Interval' action.",
      Type = "number", Default = "450", MinValue = 50, MaxValue = int.MaxValue, ReadOnly = true
    )]
    [TouchPortalAction("ActionRepeatInterval", "Action Repeat Interval", "MSFS", "Held Action Repeat Rate (ms)", "Repeat Interval: {0} milliseconds", true)]
    [TouchPortalActionChoice(new[] { "Increment 50ms", "Decrement 50ms", "50", "100", "200", "250", "300", "350", "400", "450", "500", "550", "600", "700", "800", "900", "1000" }, "450")]  // TODO replace with freeform value
    [TouchPortalState("ActionRepeatInterval", "text", "The current Held Action Repeat Rate (ms)", "450")]
    public static readonly PluginSetting ActionRepeatInterval = new PluginSetting("ActionRepeatInterval", 50, int.MaxValue);
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
    Disconnect,

    [TouchPortalActionMapping("ActionRepeatInterval", "Increment 50ms")]
    ActionRepeatIntervalInc,
    [TouchPortalActionMapping("ActionRepeatInterval", "Decrement 50ms")]
    ActionRepeatIntervalDec,
    [TouchPortalActionMapping("ActionRepeatInterval", "50")]
    ActionRepeatInterval50,
    [TouchPortalActionMapping("ActionRepeatInterval", "100")]
    ActionRepeatInterval100,
    [TouchPortalActionMapping("ActionRepeatInterval", "200")]
    ActionRepeatInterval200,
    [TouchPortalActionMapping("ActionRepeatInterval", "250")]
    ActionRepeatInterval250,
    [TouchPortalActionMapping("ActionRepeatInterval", "300")]
    ActionRepeatInterval300,
    [TouchPortalActionMapping("ActionRepeatInterval", "350")]
    ActionRepeatInterval350,
    [TouchPortalActionMapping("ActionRepeatInterval", "400")]
    ActionRepeatInterval400,
    [TouchPortalActionMapping("ActionRepeatInterval", "450")]
    ActionRepeatInterval450,
    [TouchPortalActionMapping("ActionRepeatInterval", "500")]
    ActionRepeatInterval500,
    [TouchPortalActionMapping("ActionRepeatInterval", "550")]
    ActionRepeatInterval550,
    [TouchPortalActionMapping("ActionRepeatInterval", "600")]
    ActionRepeatInterval600,
    [TouchPortalActionMapping("ActionRepeatInterval", "700")]
    ActionRepeatInterval700,
    [TouchPortalActionMapping("ActionRepeatInterval", "800")]
    ActionRepeatInterval800,
    [TouchPortalActionMapping("ActionRepeatInterval", "900")]
    ActionRepeatInterval900,
    [TouchPortalActionMapping("ActionRepeatInterval", "1000")]
    ActionRepeatInterval1000,
  }
}
