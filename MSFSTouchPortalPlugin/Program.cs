using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MSFSTouchPortalPlugin.Configuration;
using MSFSTouchPortalPlugin.Interfaces;
using MSFSTouchPortalPlugin.Services;
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
    private static async Task Main(string[] args) {
      // Logger
      var logFactory = new LoggerFactory();
      var logger = logFactory.CreateLogger("Program");

      //Build configuration:
      var configurationRoot = new ConfigurationBuilder()
          .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
          .AddJsonFile("appsettings.json")
          .Build();

      // Ensure only one running instance
      const string mutextName = "MSFSTouchPortalPlugin";
      _ = new Mutex(true, mutextName, out var createdNew);

      if (!createdNew) {
        logger.LogError($"{mutextName} is already running. Exiting application.");
        return;
      }

      try {
        await Host.CreateDefaultBuilder(args).ConfigureServices((context, services) => {
          services.AddLogging(configure => {
            configure.AddSimpleConsole(options => options.TimestampFormat = "[yyyy.MM.dd HH:mm:ss] ");
            configure.AddConfiguration(configurationRoot.GetSection("Logging"));
          })
          .Configure<MsfsTouchPortalPlugin>((opt) => { })
          .AddHostedService<PluginService>()
          .AddSingleton<ISimConnectService, SimConnectService>()
          .AddSingleton<IReflectionService, ReflectionService>()
          .AddTouchPortalSdk(configurationRoot);
        }).RunConsoleAsync();
      } catch (COMException ex) {
        logger.LogError($"COMException: {ex.Message}");
      }
    }
  }
}
