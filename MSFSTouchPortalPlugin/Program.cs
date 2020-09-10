using Microsoft.Extensions.DependencyInjection;
using MSFSTouchPortalPlugin.Configuration;
using MSFSTouchPortalPlugin.Interfaces;
using MSFSTouchPortalPlugin.Objects.Plugin;
using MSFSTouchPortalPlugin.Services;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
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
        var reflectionService = serviceProvider.GetRequiredService<IReflectionService>();

        var internalEvents = reflectionService.GetInternalEvents();
        var actionEvents = reflectionService.GetActionEvents();
        var states = reflectionService.GetStates();

        // Connect to MSFS
        pluginService.TryConnect();

        // Setup Events/Actions
        pluginService.SetupEventLists(internalEvents, actionEvents, states);

        // Run services
        Task.WaitAll(pluginService.RunPluginServices());


        Console.ReadLine();
      } catch (COMException ex) {

      }
    }
  }
}
