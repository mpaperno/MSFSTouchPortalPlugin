using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Enums;
using MSFSTouchPortalPlugin.Types;

namespace MSFSTouchPortalPlugin.Objects.Plugin
{
  [TouchPortalCategory(Groups.Plugin)]
  internal static class PluginMapping
  {
    [TouchPortalAction(PluginActions.Connection, "Connection",
      "Toggle/On/Off SimConnect Connection",
      "SimConnect Connection - {0}")]
    [TouchPortalActionChoice(new[] { "Toggle", "On", "Off", "Reload States" }, Id = "Action")]
    [TouchPortalActionMapping(PluginActions.ToggleConnection, "Toggle")]
    [TouchPortalActionMapping(PluginActions.Connect, "On")]
    [TouchPortalActionMapping(PluginActions.Disconnect, "Off")]
    [TouchPortalActionMapping(PluginActions.ReloadStates, "Reload States")]
    public static readonly object Connection;

    [TouchPortalAction(PluginActions.ActionRepeatInterval, "Action Repeat Interval",
      "Held Action Repeat Rate (ms)",
      "Repeat Interval: {0} to/by: {1} ms",
      holdable: true)]
    [TouchPortalActionChoice(new[] { "Set", "Increment", "Decrement" }, Id = "Action")]
    [TouchPortalActionText("450", 50, int.MaxValue, Id = "Value")]
    [TouchPortalActionMapping(PluginActions.ActionRepeatIntervalSet, "Set")]
    [TouchPortalActionMapping(PluginActions.ActionRepeatIntervalInc, "Increment")]
    [TouchPortalActionMapping(PluginActions.ActionRepeatIntervalDec, "Decrement")]
    public static readonly object ActionRepeatInterval;

    [TouchPortalAction(PluginActions.SetCustomSimEvent, "Activate a Custom Simulator Event",
      "Trigger a Simulator Event by name. The value, if any, should evaluate to numeric. Using basic math operators and dynamic state values is possible.",
      "Activate Event {0} with value {1} (if any)",
      holdable: true)]
    [TouchPortalActionText("SIMULATOR_EVENT_NAME", Id = "EvtId", Label = "Event ID")]
    [TouchPortalActionText("", Id = "Value", Label = "Value")]
    public static readonly object SetCustomSimEvent;

    [TouchPortalAction(PluginActions.SetKnownSimEvent, "Activate a Simulator Event From List",
      "Trigger a Simulator Event. The value, if any, should evaluate to numeric. Using basic math operators and dynamic state values is possible.",
      "From Category {0} Activate Event {1} with value {2} (if any)",
      holdable: true)]
    [TouchPortalActionChoice("<plugin not connected>", "", Id = "SimCatName", Label = "Category")]
    [TouchPortalActionChoice("<select a category>", "", Id = "EvtId", Label = "Event ID")]
    [TouchPortalActionText("", Id = "Value", Label = "Value")]
    public static readonly object SetKnownSimEvent;

    [TouchPortalAction(PluginActions.SetSimVar, "Set Simulator Variable Value",
      "Sets a value on any loaded State which is marked as settable. If a numeric value is required, using basic math operators and dynamic state values is possible.",
      "Set Variable {0} to {1} (release AI: {2})",
      holdable: true)]
    [TouchPortalActionChoice("<plugin not connected>", "", Id = "VarName", Label = "Simulator Variable")]
    [TouchPortalActionText("0", Id = "Value", Label = "Value")]
    [TouchPortalActionSwitch(false, Id = "RelAI", Label = "Release AI")]
    public static readonly object SetSimVar;

    // The spacing and titles here are very carefully chosen to help align labels on the top row with entry fields on the bottom row, including the default lists/values.
    // Do not change without testing in TP!
    [TouchPortalAction(PluginActions.AddCustomSimVar, "Request a Custom Simulator Variable",
      "Request Simulator Variable Name:                            Index (if req'd):               Category:                                    " +
        "Units:                                                                               Format:                            Default Value:                 " +
        "Settable:           Update Period:                   Update Interval:               Delta Epsilon:     ",
      "{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}")]
    [TouchPortalActionText("SIMULATOR VARIABLE FULL NAME", Id = "VarName", Label = "Simulator Variable Name")]
    [TouchPortalActionNumeric(0, 0, 99, false, Id = "VarIndex", Label = "Variable Index")]
    [TouchPortalActionChoice("<connect plugin>", "", Id = "CatId", Label = "Category")]
    [TouchPortalActionChoice("<connect to plugin>                                 ", "", Id = "Unit", Label = "Unit Name")]
    [TouchPortalActionText("F2", Id = "Format", Label = "Formatting String")]
    [TouchPortalActionText("0", Id = "DfltVal", Label = "Default Value")]
    [TouchPortalActionSwitch(false, Id = "CanSet", Label = "Settable")]
    [TouchPortalActionChoice(new[] { /*"Never",*/ "Once", "VisualFrame", "SimFrame", "Second", "Millisecond", }, "SimFrame", Id = "UpdPer", Label = "Update Period")]
    [TouchPortalActionNumeric(0, 0, int.MaxValue, false, Id = "UpdInt", Label = "Interval")]
    [TouchPortalActionNumeric(0.0099999, 0.0, float.MaxValue, true, Id = "Epsilon", Label = "Delta Epsilon")]
    public static readonly object AddCustomSimVar;

    // The spacing and titles here are very carefully chosen to help align labels on the top row with entry fields on the bottom row, including the default lists/values.
    // Do not change without testing in TP!
    [TouchPortalAction(PluginActions.AddKnownSimVar, "Request a Simulator Variable From List",
      "From Simulator Category            Request Simulator Variable                                                                        " +
        "Index (if req'd):                Plugin Category:                         Units:                                                                               " +
        "Format:                            Default Value:                 Update Period:                  Update Interval:              Delta Epsilon:      ",
      "{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}")]
    [TouchPortalActionChoice("<connect to plugin>", "", Id = "SimCatName", Label = "Category")]
    [TouchPortalActionChoice("<select a category>                                                            ", "", Id = "VarName", Label = "Simulator Variable Name")]
    [TouchPortalActionNumeric(0, 0, 99, false, Id = "VarIndex", Label = "Variable Index")]
    [TouchPortalActionChoice("<connect plugin>", "", Id = "CatId", Label = "Category")]
    [TouchPortalActionChoice("<connect to plugin>                                 ", "", Id = "Unit", Label = "Unit Name")]
    [TouchPortalActionText("F2", Id = "Format", Label = "Formatting String")]
    [TouchPortalActionText("0", Id = "DfltVal", Label = "Default Value")]
    //[TouchPortalActionSwitch(false, Id = "CanSet", Label = "Settable")]  // we should know if it's settable from the imported data
    [TouchPortalActionChoice(new[] { /*"Never",*/ "Once", "VisualFrame", "SimFrame", "Second", "Millisecond", }, "SimFrame", Id = "UpdPer", Label = "Update Period")]
    [TouchPortalActionNumeric(0, 0, int.MaxValue, false, Id = "UpdInt", Label = "Interval")]
    [TouchPortalActionNumeric(0.0099999, 0.0, float.MaxValue, true, Id = "Epsilon", Label = "Delta Epsilon")]
    public static readonly object AddKnownSimVar;

    [TouchPortalAction(PluginActions.RemoveSimVar, "Remove a Simulator Variable",
      "Remove an existing Simulator Variable. Note that static TP States cannot be removed.",
      "Remove Simulator Variable {0}")]
    [TouchPortalActionChoice("<plugin not connected>", "", Id = "VarName", Label = "Simulator Variable")]
    public static readonly object RemoveSimVar;

    [TouchPortalAction(PluginActions.LoadSimVars, "Load SimVar Definitions From File",
      "Load a set of simulator variable state definitions from a configuration file.",
      "Load from file {0} (name only for user config. folder, or full path with file name)")]
    [TouchPortalActionFile("CustomStates.ini", Id = "VarsFile", Label = "Load From File")]
    public static readonly object LoadSimVars;

    [TouchPortalAction(PluginActions.SaveSimVars, "Save SimVar Definitions To File",
      "Save the current simulator variable state definitions to a configuration file.",
      "Save {0} states to file {1} (name only for user config. folder, or full path with file name)")]
    [TouchPortalActionChoice(new[] { "Custom", "All" }, Id = "VarsSet", Label = "Variables Set")]
    [TouchPortalActionText("CustomStates.ini", Id = "VarsFile", Label = "Save to File")]
    //[TouchPortalActionFile("CustomStates.ini", Id = "VarsFile", Label = "Save to File")]  // doesn't allow freeform entry or selecting a non-existing file as of TP 3.0.10
    [TouchPortalActionMapping(PluginActions.SaveCustomSimVars, "Custom")]
    [TouchPortalActionMapping(PluginActions.SaveAllSimVars, "All")]
    public static readonly object SaveSimVars;
  }

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
}
