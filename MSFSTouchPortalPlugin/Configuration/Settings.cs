/*
This file is part of the MSFS Touch Portal Plugin project.
https://github.com/mpaperno/MSFSTouchPortalPlugin

COPYRIGHT:
(c) Maxim Paperno; All Rights Reserved.

This file may be used under the terms of the GNU General Public License (GPL)
as published by the Free Software Foundation, either version 3 of the Licenses,
or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

A copy of the GNU GPL is included with this project
and is also available at <http://www.gnu.org/licenses/>.
*/

using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Enums;
using MSFSTouchPortalPlugin.Types;

namespace MSFSTouchPortalPlugin.Configuration
{
  [TouchPortalSettingsContainer]
  public static class Settings
  {
    public static readonly PluginSetting ConnectSimOnStartup = new PluginSetting("ConnectSimOnStartup", DataType.Switch) {
      Name = "Connect To Flight Sim on Startup (0/1)",
      Description = "Set to 1 to automatically attempt connection to flight simulator upon Touch Portal startup. Set to 0 to only connect manually via the Action \"MSFS - Plugin -> Connect & Update\".",
      Default = "0",
    };

    public static readonly PluginSetting UserStateFiles = new PluginSetting("UserStateFiles", DataType.Text) {
      Name = "Sim Variable State Config File(s) (blank = Default)",
      Description = "Here you can specify one or more custom configuration files which define SimConnect variables to request as Touch Portal States. " +
        "This plugin comes with an extensive default set of states, however since the possibilities between which variables are requested, which units they are displayed in," +
        "and how they are formatted are almost endless. This option provides a way to customize the output as desired.\n\n" +
        "Enter a file name here, with or w/out the suffix (\".ini\" is assumed). Separate multiple files with commas (and optional space). " +
        "To include the default set of variables/states, use the name `Default` as one of the file names (in any position of the list).\n\n" +
        "Files are loaded in the order in which they appear in the list, and in case of conflicting state IDs, the last one found will be used.\n\n" +
        "The custom file(s) are expected to be in the folder specified in the \"User Config Files Path\" setting (see below).\n\n" +
        "See https://github.com/mpaperno/MSFSTouchPortalPlugin/wiki/Using-Custom-States-and-Simulator-Variables for more details.",
      DocsUrl = "https://github.com/mpaperno/MSFSTouchPortalPlugin/wiki/Using-Custom-States-and-Simulator-Variables",
      Default = "Default",
      MaxLength = 255
    };

    public static readonly PluginSetting SimConnectConfigIndex = new PluginSetting("SimConnectConfigIndex", DataType.Number) {
      Name = "SimConnect.cfg Index (0 for MSFS, 1 for FSX, or custom)",
      Description =
        "A \"SimConnect.cfg\" file can contain a number of connection configurations, identified in sections with the `[SimConnect.N]` title. " +
        "A default \"SimConnect.cfg\" is included with this plugin (in the installation folder). " +
        "You may also use a custom configuration file stored in the \"User Config Files Path\" folder (see below). \n\n" +
        "The index number can be specified in this setting. This is useful for: \n" +
        "* Compatibility with FSX.\n" +
        "* Custom configurations over network connections (running Touch Portal on a different computer than the sim). \n\n" +
        "The default configuration index is zero, which (in the included default SimConnect.cfg) is suitable for MSFS (2020). Use the index 1 for compatibility with FSX (or perhaps other sims).\n\n" +
        "See here for more info about the file format: https://docs.flightsimulator.com/html/Programming_Tools/SimConnect/SimConnect_CFG_Definition.htm  \n\n" +
        "For more information on using Touch Portal remotely see https://github.com/mpaperno/MSFSTouchPortalPlugin/wiki/Multiple-Touch-Portal-Device-Setup",
      DocsUrl = "https://github.com/mpaperno/MSFSTouchPortalPlugin/wiki/Multiple-Touch-Portal-Device-Setup",
#if !FSX
      Default = "0",
#else
      Default = "1",
#endif
      MinValue = 0,
      MaxValue = 20
    };

    public static readonly PluginSetting UserConfigFilesPath = new PluginSetting("UserConfigFilesPath", DataType.Text) {
      Name = "User Config Files Path (blank for default)",
      Description = "The system path where plugin settings are stored, including custom user State configuration files for sate definitions & the \"SimConnect.cfg\" configuration file.\n " +
        "Keep it blank for default, which is \"C:\\Users\\<UserName>\\AppData\\Roaming\\" + PluginConfig.PLUGIN_ID + "\".\n\n" +
        "Note that using this plugin's installation folder for custom data storage is not recommended, since anything in there will likely get overwritten during a plugin update/re-install.",
      Default = "",
      MaxLength = 255
    };

    public static readonly PluginSetting UseInvariantCulture = new PluginSetting("UseInvariantCulture", DataType.Switch) {
      Name = "Ignore Local Number Format Rules (0/1)",
      Description = "Touch Portal cannot perform math or numeric comparison operations on decimal numbers formatted with comma decimal separator, even in locations where this is the norm. " +
        "Set this setting to 1 (one) to always format numbers in \"neutral\" format with period decimal separators.\n\n" +
        "**NOTE** that this affects **input** number formatting as well (the plugin will expect all numbers with period decimal separators regardless of your location).",
      Default = "1",
    };

#if !FSX
    public static readonly PluginSetting UpdateHubHopOnStartup = new PluginSetting("UpdateHubHopOnStartup", DataType.Switch) {
      Name = "Update HubHop Data on Startup (0/1)",
      Description = "Set to 1 (one) to automatically load latest HubHop data when plugin starts. Set to 0 (zero) to disable. Updates can always be triggered manually via the Action \"MSFS - Plugin -> Connect & Update\".\n" +
                    "**Updates require a working Internet connection!**",
      Default = "0",
    };

    public static readonly PluginSetting HubHopUpdateTimeout = new PluginSetting("HubHopUpdateTimeout", DataType.Number) {
      Name = "HubHop Data Update Timeout (seconds)",
      Description = "Maximum number of seconds to wait for a HubHop data update check or download via the Internet.",
      Default = "60",
      MinValue = 0,
      MaxValue = 600
    };
#endif
#if WASIM
    public static readonly PluginSetting SortLVarsAlpha = new PluginSetting("SortLVarsAlpha", DataType.Switch) {
      Name = "Sort Local Variables Alphabetically (0/1)",
      Description =
        "Set to 1 (one) to have all Local ('L') simulator variables sorted in alphabetical order within selection lists. " +
        "Setting to 0 (zero) will keep them in the original order they're loaded in on the simulator (by ascending ID).\n\n" +
        "This setting only applies when using WASimModule integration since there is no other way to get a list.",
      Default = "1",
    };
#endif

    public static readonly PluginSetting ActionRepeatDelay = new PluginSetting("ActionRepeatDelay", DataType.Number) {
      Name = "Default Held Action Repeat Delay (milliseconds)",
      Description =
        "Default delay period before a held action starts repeating.\n" +
        "* A value of 0 will make the delay be the same as the current repeat interval\n\t(this is the default value for this setting).\n" +
        "* If set to > 0, the smallest effective value is " + PluginConfig.ACTION_REPEAT_RATE_MIN_MS.ToString() + "ms, same as the repeat interval.\n" +
        "\tThis is how often the plugin checks for events to fire.\n\n" +
        "Typically the delay would be same or longer than the repeat interval.\n" +
        "For example with a very short interval it may be possible to get unintentional repeating if a button is pressed for a little too long. " +
        "Introducing a longer delay time before the repeat starts would help the issue.\n\n" +
        "A delay can also be specified per action when it is used in the \"On Hold\" button setup area, which would override this setting.",
      Default = "0",
      TpMinValue = 0,
      MinValue = PluginConfig.ACTION_REPEAT_RATE_MIN_MS,
      MaxValue = uint.MaxValue
    };

    // Internally tracked settings, not used via Touch Portal UI.

    // Version of the plugin with which the settings were last saved. Used for change tracking/notifications/etc.
    public static readonly PluginSetting PluginSettingsVersion = new("PluginSettingsVersion", 0, 0xFFFFFFFF, "0");
    // Random part of WASimClient ID, set once per plugin installation and saved in settings config file.
    public static readonly PluginSetting WasimClientIdHighByte = new("WasimClientIdHighByte", 0, 0xFF, "0");
    // Held action repeat interval; settable by user.
    public static readonly PluginSetting ActionRepeatInterval = new("ActionRepeatInterval", PluginConfig.ACTION_REPEAT_RATE_MIN_MS, uint.MaxValue, "450");
  }
}
