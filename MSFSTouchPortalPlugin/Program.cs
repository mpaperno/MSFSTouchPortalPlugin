using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MSFSTouchPortalPlugin.Configuration;
using MSFSTouchPortalPlugin.Interfaces;
using MSFSTouchPortalPlugin.Services;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using TouchPortalApi;

[assembly: InternalsVisibleTo("MSFSTouchPortalPlugin-Generator")]

namespace MSFSTouchPortalPlugin {
  public static class Program {
    private static async Task Main(string[] args) {
      // Logger
      var logFactory = new LoggerFactory();
      var logger = logFactory.CreateLogger("Program");

      // Ensure only one running instance
      const string mutextName = "MSFSTouchPortalPlugin";
      _ = new Mutex(true, mutextName, out var createdNew);

      if (!createdNew) {
        logger.LogError($"{mutextName} is already running. Exiting application.");
        return;
      }

      try {
        await Host.CreateDefaultBuilder(args).ConfigureServices((context, services) => {
          services.ConfigureTouchPointApi((opts) => {
            opts.PluginId = "MSFSTouchPortalPlugin";
            opts.ServerIp = "127.0.0.1";
            opts.ServerPort = 12136;
          })
          .AddLogging()
          .Configure<MsfsTouchPortalPlugin>((opt) => { })
          .AddHostedService<PluginService>()
          .AddSingleton<ISimConnectService, SimConnectService>()
          .AddSingleton<IReflectionService, ReflectionService>();
        }).RunConsoleAsync();
      } catch (COMException ex) {
        logger.LogError($"COMException: {ex.Message}");
      }
    }
  }
}
