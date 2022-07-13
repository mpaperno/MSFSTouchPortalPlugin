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
using CommandLine.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MSFSTouchPortalPlugin.Configuration;
using MSFSTouchPortalPlugin.Interfaces;
using MSFSTouchPortalPlugin.Services;
using MSFSTouchPortalPlugin_Generator.Configuration;
using MSFSTouchPortalPlugin_Generator.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MSFSTouchPortalPlugin_Generator {
  static class Program {

    static async Task Main(string[] args) {
      System.Environment.SetEnvironmentVariable("Logging__LogLevel__Microsoft", "Warning");  // silence internal .net info messages

      // parse CLI options
      GeneratorOptions opts = new();
      try {
        var parser = new Parser(with => { with.HelpWriter = null; with.CaseSensitive = false; });  // Parser.Default
        var parseResult = parser.ParseArguments(() => opts, args);
        parseResult.WithNotParsed(errs => DisplayHelp(parseResult, errs));
        parser.Dispose();
      }
      catch (Exception e) {
        OnError($"Command line parsing failed with error: {e.Message}");
      }

      // validate options
      try { opts.OutputPath = Path.GetFullPath(opts.OutputPath); }
      catch (Exception e) { OnError($"TargetPath option error, not found or not accessible: {e.Message}"); }
      if (!string.IsNullOrWhiteSpace(opts.StateFilesPath)) {
        try { opts.StateFilesPath = Path.GetFullPath(opts.StateFilesPath); }
        catch (Exception e) { OnError($"StateFilesPath option error, not found or not accessible: {e.Message}"); }
      }

      await Host.CreateDefaultBuilder(args).ConfigureServices((context, services) => {
        services
        .AddLogging()
        .AddHostedService<RunService>()
        .AddSingleton(opts)
        .AddSingleton<IReflectionService, ReflectionService>()
        .AddSingleton(typeof(PluginConfig))
        .AddSingleton<IGenerateDoc, GenerateDoc>()
        .AddSingleton<IGenerateEntry, GenerateEntry>();
      }).RunConsoleAsync();
    }

    static void OnError(string message, int exitCode = -1) {
      Console.WriteLine(message);
      Environment.Exit(exitCode);
    }

    static void DisplayHelp<T>(ParserResult<T> result, System.Collections.Generic.IEnumerable<Error> errs) {
      HelpText helpText = null;
      if (errs.IsVersion()) {
        OnError(HelpText.AutoBuild(result), 0);
      }

      helpText = HelpText.AutoBuild(result, h =>
      {
        h.MaximumDisplayWidth = 160;
        //h.AdditionalNewLineAfterOption = false;
        var tgt = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TouchPortal", "plugins", "MSFS-TouchPortal-Plugin") + '\n';
        if (!errs.IsVersion()) {
          h.AddPreOptionsText(
            "\nUsage:\n" +
            $"  {AppDomain.CurrentDomain.FriendlyName} [-s <file(s)>] [-c <config version>] [<other options>] [-o <Output Path>]\n\n" +
            "Example: Generate with Default states plus a custom states file and put output in your plugin's installation folder.\n" +
            $"  {AppDomain.CurrentDomain.FriendlyName} -s Default,CustomStates -o {tgt}\n\n ");
          h.AddPreOptionsLine("\nOptions:");
        }

        return HelpText.DefaultParsingErrorsHandler(result, h);
      }, e => e);
      OnError(helpText);
    }

  }
}
