using Microsoft.Extensions.DependencyInjection;
using MSFSTouchPortalPlugin.Configuration;
using MSFSTouchPortalPlugin.Interfaces;
using MSFSTouchPortalPlugin.Services;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TouchPortalApi;

[assembly: InternalsVisibleTo("MSFSTouchPortalPlugin-Generator")]

namespace MSFSTouchPortalPlugin {
  class Program {
    static void Main(string[] args) {
      // Setup DI with configured options
      var serviceProvider = new ServiceCollection()
        .ConfigureTouchPointApi((opts) => {
          opts.PluginId = "MSFSTouchPortalPlugin";
          opts.ServerIp = "127.0.0.1";
          opts.ServerPort = 12136;
        })
        .AddSingleton<ISimConnectService, SimConnectService>()
        .AddSingleton<IPluginService, PluginService>()
        .AddSingleton<IReflectionService, ReflectionService>()
        .Configure<MSFSTouchPortalPluginOptions>((opt) => {
        })
        .BuildServiceProvider();

      try {
        // Startup Plugin Service
        var pluginService = serviceProvider.GetRequiredService<IPluginService>();
        
        // Connect to MSFS
        pluginService.TryConnect();

        Console.ReadLine();
      } catch (COMException ex) {

      }
    }
  }
}
