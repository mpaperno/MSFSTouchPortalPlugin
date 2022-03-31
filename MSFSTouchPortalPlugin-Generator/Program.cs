using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MSFSTouchPortalPlugin.Interfaces;
using MSFSTouchPortalPlugin.Services;
using MSFSTouchPortalPlugin_Generator.Configuration;
using MSFSTouchPortalPlugin_Generator.Interfaces;
using System.Threading.Tasks;

namespace MSFSTouchPortalPlugin_Generator {
  static class Program {
    static async Task Main(string[] args) {
      await Host.CreateDefaultBuilder(args).ConfigureServices((context, services) => {
        services
        .AddLogging()
        .Configure<GeneratorOptions>((opt) => {
          opt.PluginName = "MSFSTouchPortalPlugin";
          opt.PluginFolder = "MSFS-TouchPortal-Plugin";
          if (args.Length >= 1)
            opt.TargetPath = args[0];
          else
            opt.TargetPath = "..\\..\\..\\";  // assumes it is being run from the build output folder
        })
        .AddHostedService<RunService>()
        .AddSingleton<IPluginService, PluginService>() // Force load of assembly for generation
        .AddSingleton<IGenerateDoc, GenerateDoc>()
        .AddSingleton<IGenerateEntry, GenerateEntry>();
      }).RunConsoleAsync();
    }
  }
}
