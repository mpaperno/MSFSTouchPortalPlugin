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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MSFSTouchPortalPlugin.Configuration;
using MSFSTouchPortalPlugin.Interfaces;
using MSFSTouchPortalPlugin.Services;
using MSFSTouchPortalPlugin.Types;
using Serilog;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using TouchPortalSDK.Configuration;

[assembly: InternalsVisibleTo("MSFSTouchPortalPlugin-Generator")]

namespace MSFSTouchPortalPlugin {
  public static class Program {
    private static async Task Main(string[] args)
    {
      //Build configuration:
      var configurationRoot = new ConfigurationBuilder()
          .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
#if DEBUG
          // optional settings override for debug builds (eg. for logging levels)
          .AddJsonFile("appsettings.debug.json", optional: true, reloadOnChange: true)
#endif
          .AddEnvironmentVariables()  // inject env. vars
          .Build();

      // Ensure only one running instance
      const string mutextName = "MSFSTouchPortalPlugin";
      _ = new Mutex(true, mutextName, out var createdNew);

      if (!createdNew) {
        Console.WriteLine("{0} is already running. Exiting application.", mutextName);
        return;
      }

      try {
        await Host.CreateDefaultBuilder(args)
          .ConfigureLogging((hostContext, loggingBuilder) => {
            loggingBuilder
              .ClearProviders()
              .AddSerilog(logger: new LoggerConfiguration().ReadFrom.Configuration(configurationRoot).CreateLogger(), dispose: true)
              // PluginLogger is a feedback mechanism to get log entries delivered to the plugin's handler, which may then pass them on to TP as a State.
              .AddProvider(PluginLoggerProvider.Instance)
              // Do not change these filters w/out a good reason!
              .AddFilter<PluginLoggerProvider>("", LogLevel.None)
              .AddFilter<PluginLoggerProvider>("MSFSTouchPortalPlugin.Services.PluginService", LogLevel.Information)
              .AddFilter<PluginLoggerProvider>("MSFSTouchPortalPlugin.Services.SimConnectService", LogLevel.Warning);
          })
          .ConfigureServices((context, services) => {
            services
              .Configure<MsfsTouchPortalPlugin>((opt) => { })
              .AddHostedService<PluginService>()
              .AddSingleton<ISimConnectService, SimConnectService>()
              .AddSingleton<IReflectionService, ReflectionService>()
              .AddSingleton(typeof(PluginConfig))
              .AddSingleton(typeof(SimVarCollection))
              .AddTouchPortalSdk(configurationRoot);
          })
          .RunConsoleAsync();
      } catch (COMException ex) {
        Console.WriteLine("COMException: {0}", ex.Message);
      }
    }
  }
}
