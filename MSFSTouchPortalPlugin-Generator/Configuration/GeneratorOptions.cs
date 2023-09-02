/*
This file is part of the MSFS Touch Portal Plugin project.
https://github.com/mpaperno/MSFSTouchPortalPlugin

COPYRIGHT:
(c) 2020 Tim Lewis;
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

using CommandLine;
using MSFSTouchPortalPlugin.Configuration;

namespace MSFSTouchPortalPlugin_Generator.Configuration
{
  internal class GeneratorOptions
  {
    [Option('o', "OutputPath", Default = ".\\", MetaValue = "<Path>", Required = false,
      HelpText = "Output directory path for generated files (default is current working directory).")]
    public string OutputPath { get; set; }

    [Option('s', "StateFiles", Default = null, MetaValue = "<LIST>", Separator = ',', Required = false,
      HelpText = "\nOne or more custom configuration files which define Simulator Variables/Touch Portal States for documentation. \n" +
      "Separate multiple files with commas (eg. \"Engines,Fuel\"). The file extension is optional, \".ini\" is assumed. \n" +
      "The file names may optionally include folder paths, but the default location is according to the StateFilesPath option (see below). \n" +
      "To include the default set of variables/states, use the name \"Default\" as one of the names (in any position of the list).")]
    public System.Collections.Generic.IEnumerable<string> StateFiles { get; set; }

    [Option('p', "StateFilesPath", Default = "", MetaValue = "<DIR>", Required = false,
      HelpText = "\nWhere to find the custom state configuration file(s) specified in 'StateFiles' parameter.\n" +
      "Default is the current user's 'AppData/Roaming/MSFSTouchPortalPlugin' directory.\n" +
      "Note that the Default states file (if used) is always read from the 'Configuration' subfolder of this utility.")]
    public string StateFilesPath { get; set; }

    [Option("ColorLight",
#if FSX
      Default = "996c00",
#else
      Default = "0090ff",
#endif
      MetaValue = "<HEX_COLOR>", Required = false,
      HelpText = "\nColor for the background of action labels and descriptions. Hex color string without the leading '#' sign.")]
    public string ColorLight { get; set; }

    [Option("ColorDark",
      Default = "212121",
      MetaValue = "<HEX_COLOR>", Required = false,
      HelpText = "\nColor for the background of editable fields and selection lists. Hex color string without the leading '#' sign.")]
    public string ColorDark { get; set; }

    [Option('i', "PluginId",
      Default = PluginConfig.PLUGIN_ID,
      MetaValue = "<STRING>", Required = false,
      HelpText = "\nThe Plugin ID string which will be used for all the entry definitions.")]
    public string PluginId { get; set; }

    [Option('n', "PluginName",
      Default = PluginConfig.PLUGIN_NAME_PREFIX + " Touch Portal Plugin",
      MetaValue = "<STRING>", Required = false,
      HelpText = "\nThe Plugin Name for TP entry.tp base name attribute.")]
    public string PluginName { get; set; }

    [Option('f', "PluginFolder",
      Default = PluginConfig.PLUGIN_NAME_PREFIX + "-TouchPortal-Plugin",
      MetaValue = "<STRING>", Required = false,
      HelpText = "\nName of the plugin's folder once installed to TP.")]
    public string PluginFolder { get; set; }

    [Option('u', "DocsUrl",
      Default = "https://github.com/mpaperno/MSFSTouchPortalPlugin/wiki",
      MetaValue = "<URL>", Required = false,
      HelpText = "\nURL to full project documentation, if any.")]
    public string DocumentationUrl { get; set; }

    [Option('d', "debug", Default = false, Required = false,
      HelpText = "Enables generating a 'debug' version of entry.tp. Currently that means w/out a startup command.")]
    public bool Debug { get; set; }
  }
}
