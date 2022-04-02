using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Enums;
using MSFSTouchPortalPlugin.Types;

namespace MSFSTouchPortalPlugin.Objects.Plugin
{
  [TouchPortalCategory(Groups.Plugin)]
  internal static class PluginMapping {
    [TouchPortalAction("Connection", "Connection", "MSFS", "Toggle/On/Off SimConnect Connection", "SimConnect Connection - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off" })]
    [TouchPortalActionMapping("ToggleConnection", "Toggle")]
    [TouchPortalActionMapping("Connect", "On")]
    [TouchPortalActionMapping("Disconnect", "Off")]
    public static readonly object Connection;
  }

  [TouchPortalCategory(Groups.Plugin)]
  [TouchPortalSettingsContainer]
  public static class Settings
  {
    public static readonly PluginSetting ConnectSimOnStartup = new PluginSetting("ConnectSimOnStartup", "1", DataType.Switch) {
      Name = "Connect To Flight Sim on Startup (0/1)",
      Description = "Set to 1 to automatically attempt connection to flight simulator upon Touch Portal startup. Set to 0 to only connect manually via the provided Action.",
    };

    public static readonly PluginSetting SimConnectConfigIndex = new PluginSetting("SimConnectConfigIndex", DataType.Number) {
      Name = "SimConnect.cfg Index (0 for MSFS, 1 for FSX, or custom)",
      Description = @"A default SimConnect.cfg is included with this plugin (in the installation folder). " +
      "You may also use a custom configuration file stored in your 'C:\\Users\\<UserName>\\AppData\\Roaming\\MSFSTouchPortalPlugin' folder. \n\n" +
      "The SimConnect.cfg file can contain a number of configurations, identified in sections with the [SimConnect.N] title. " +
      "The index number can be specified in this setting. This is useful for \n" +
      "  1. compatibility with FSX, and/or \n" +
      "  2. custom configurations over network connections (running Touch Portal on a different computer than the sim). \n" +
      "The default configuration index is zero, which (in the included default SimConnect.cfg) is suitable for MSFS (2020). Use the index 1 for compatibility with FSX (or perhaps other sims). \n\n" +
      "See here for more info: https://docs.flightsimulator.com/html/Programming_Tools/SimConnect/SimConnect_CFG_Definition.htm",
      Default = "0",
      MinValue = 0,
      MaxValue = 20
    };

    [TouchPortalAction("ActionRepeatInterval", "Action Repeat Interval", "MSFS", "Held Action Repeat Rate (ms)", "Repeat Interval: {0} to/by: {1} ms", true)]
    [TouchPortalActionChoice(new[] { "Set", "Increment", "Decrement" })]
    [TouchPortalActionText("450", 50, int.MaxValue)]
    [TouchPortalActionMapping("ActionRepeatIntervalSet", "Set")]
    [TouchPortalActionMapping("ActionRepeatIntervalInc", "Increment")]
    [TouchPortalActionMapping("ActionRepeatIntervalDec", "Decrement")]
    public static readonly PluginSetting ActionRepeatInterval = new PluginSetting("ActionRepeatInterval", DataType.Number) {
      Name = "Held Action Repeat Rate (ms)",
      Description = "Stores the held action repeat rate, which can be set via the 'MSFS - Plugin - Action Repeat Interval' action.",
      Default = "450",
      MinValue = 50,
      MaxValue = int.MaxValue,
      ReadOnly = true,
      TouchPortalStateId = "ActionRepeatInterval"
    };

  }

  // IDs for handling internal events
  internal enum Plugin : short {
    // Starting point
    Init = 255,

    ToggleConnection,
    Connect,
    Disconnect,

    ActionRepeatIntervalInc,
    ActionRepeatIntervalDec,
    ActionRepeatIntervalSet,
  }

  // Dynamically generated SimConnect client event IDs are "parented" to this enum type,
  // meaning they become of this Type when they need to be cast to en Enum type (eg. for SimConnect C# API).
  // This is done by the ReflectionService when generating the list of events for SimConnect.
  // They really could be cast any Enum type at all, so this is mostly for semantics.
  internal enum SimEventClientId
  {
    // Starting point
    Init = 1000,
  }
}
