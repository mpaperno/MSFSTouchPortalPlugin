using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MSFSTouchPortalPlugin.Configuration;
using MSFSTouchPortalPlugin.Interfaces;
using MSFSTouchPortalPlugin.Services;
using MSFSTouchPortalPlugin_Generator.Configuration;
using MSFSTouchPortalPlugin_Generator.Interfaces;
using System.Threading.Tasks;

namespace MSFSTouchPortalPlugin_Generator {
  static class Program {
    static async Task Main(string[] args) {
      System.Environment.SetEnvironmentVariable("Logging:LogLevel:Microsoft", "Warning");
      await Host.CreateDefaultBuilder(args).ConfigureServices((context, services) => {
        services
        .AddLogging()
        .Configure<GeneratorOptions>((opt) => {
          opt.PluginName = "MSFSTouchPortalPlugin";
          opt.PluginFolder = "MSFS-TouchPortal-Plugin";
          if (args.Length >= 1)
            opt.TargetPath = args[0];
          else
            opt.TargetPath = ".\\";  // cwd
        })
        .AddHostedService<RunService>()
        .AddSingleton<IReflectionService, ReflectionService>()
        .AddSingleton(typeof(PluginConfig))
        .AddSingleton<IGenerateDoc, GenerateDoc>()
        .AddSingleton<IGenerateEntry, GenerateEntry>();
      }).RunConsoleAsync();
    }
  }
}
