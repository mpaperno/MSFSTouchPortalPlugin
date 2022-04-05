using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Enums;
using MSFSTouchPortalPlugin.Types;

namespace MSFSTouchPortalPlugin.Objects.Plugin
{
  [TouchPortalCategory(Groups.Plugin)]
  internal static class PluginMapping {
    [TouchPortalAction("Connection", "Connection", "MSFS", "Toggle/On/Off SimConnect Connection", "SimConnect Connection - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "On", "Off", "Reload States" })]
    [TouchPortalActionMapping("ToggleConnection", "Toggle")]
    [TouchPortalActionMapping("Connect", "On")]
    [TouchPortalActionMapping("Disconnect", "Off")]
    [TouchPortalActionMapping("ReloadStates", "Reload States")]
    public static readonly object Connection;


    [TouchPortalAction("SetSimVar", "Set Simulator Variable Value", "MSFS", "Sets a value on any loaded State which is marked as settable.", "Set Variable {0} to {1} (release AI: {2})")]
    [TouchPortalActionChoice(new[] { "<not connected>" }, "")]
    [TouchPortalActionText("0")]
    //[TouchPortalActionChoice(new[] { "No", "Yes" })]
    [TouchPortalActionSwitch(false)]
    [TouchPortalActionMapping("SetSimVar")]
    public static readonly object SetSimVar;
  }

  [TouchPortalCategory(Groups.Plugin)]
  [TouchPortalSettingsContainer]
  public static class Settings
  {
    public static readonly PluginSetting ConnectSimOnStartup = new PluginSetting("ConnectSimOnStartup", DataType.Switch) {
      Name = "Connect To Flight Sim on Startup (0/1)",
      Description = "Set to 1 to automatically attempt connection to flight simulator upon Touch Portal startup. Set to 0 to only connect manually via the provided Action.",
      Default = "0",
    };

    public static readonly PluginSetting UserStateFiles = new PluginSetting("UserStateFiles", DataType.Text) {
      Name = "Sim Variable State Config File(s) (blank = Default)",
      Description = "Here you can specify one or more custom configuration files which define SimConnect variables to request as Touch Portal States. " +
        "This plugin comes with an extensive default set of states, however since the possibilities between which variables are requested, which units they are displayed in," +
        "and how they are formatted are almost endless. This option provides a way to customize the output as desired.\n\n" +
        "Enter a file name here, with or w/out the suffix (`.ini` is assumed). Separate multiple files with commas (and optional space). " +
        "To include the default set of variables/states, use the name `Default` as one of the file names (in any position of the list).\n\n" +
        "Files are loaded in the order in which they appear in the list, and in case of conflicting state IDs, the last one found will be used.\n\n" +
        "The custom file(s) are expected to be in the folder specified in the \"User Config Files Path\" setting (see below).",
      Default = "Default",
      MaxLength = 255
    };

    public static readonly PluginSetting SimConnectConfigIndex = new PluginSetting("SimConnectConfigIndex", DataType.Number) {
      Name = "SimConnect.cfg Index (0 for MSFS, 1 for FSX, or custom)",
      Description =
        "A __SimConnect.cfg__ file can contain a number of connection configurations, identified in sections with the `[SimConnect.N]` title. " +
        "A default __SimConnect.cfg__ is included with this plugin (in the installation folder). " +
        "You may also use a custom configuration file stored in the \"User Config Files Path\" folder (see below). \n\n" +
        "The index number can be specified in this setting. This is useful for \n" +
        "  1. compatibility with FSX, and/or \n" +
        "  2. custom configurations over network connections (running Touch Portal on a different computer than the sim). \n\n" +
        "The default configuration index is zero, which (in the included default SimConnect.cfg) is suitable for MSFS (2020). Use the index 1 for compatibility with FSX (or perhaps other sims). \n\n" +
        "See here for more info: https://docs.flightsimulator.com/html/Programming_Tools/SimConnect/SimConnect_CFG_Definition.htm",
      Default = "0",
      MinValue = 0,
      MaxValue = 20
    };

    public static readonly PluginSetting UserConfigFilesPath = new PluginSetting("UserConfigFilesPath", DataType.Text) {
      Name = "User Config Files Path (blank for default)",
      Description = "The system path where custom user configuration files for sate definitions, SimConnect.cfg, etc, are stored. " +
        "Keep it blank for default, which is `C:\\Users\\<UserName>\\AppData\\Roaming\\MSFSTouchPortalPlugin`.\n\n" +
        "Note that using this plugin's installation folder for custom data storage is not recommended, since anything in there will likely get overwritten during a plugin update/re-install.",
      Default = "",
      MaxLength = 255
    };

    [TouchPortalAction("ActionRepeatInterval", "Action Repeat Interval", "MSFS", "Held Action Repeat Rate (ms)", "Repeat Interval: {0} to/by: {1} ms", true)]
    [TouchPortalActionChoice(new[] { "Set", "Increment", "Decrement" })]
    [TouchPortalActionText("450", 50, int.MaxValue)]
    [TouchPortalActionMapping("ActionRepeatIntervalSet", "Set")]
    [TouchPortalActionMapping("ActionRepeatIntervalInc", "Increment")]
    [TouchPortalActionMapping("ActionRepeatIntervalDec", "Decrement")]
    public static readonly PluginSetting ActionRepeatInterval = new PluginSetting("ActionRepeatInterval", DataType.Number) {
      Name = "Held Action Repeat Rate (ms)",
      Description = "Stores the held action repeat rate, which can be set via the 'MSFS - Plugin - Action Repeat Interval' action. This setting cannot be changed from the TP Plugin Settings page (even though it appears to be editable on there).",
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
    ReloadStates,

    ActionRepeatIntervalInc,
    ActionRepeatIntervalDec,
    ActionRepeatIntervalSet,

    SetSimVar,
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
