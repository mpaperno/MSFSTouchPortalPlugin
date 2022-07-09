using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;

namespace MSFSTouchPortalPlugin.Configuration.HubHop
{
  internal static class Common
  {
    public static string PresetsFileName { get; set; } = "hubhop-presets.json";
    public static string PresetsDbName { get; set; } = "hubhop_presets.sqlite3";

    public static string PresetsFolder { get => Path.Combine(PluginConfig.AppConfigFolder, "HubHop"); }
    public static string PresetsDb { get => Path.Combine(PresetsFolder, PresetsDbName); }
    public static string PresetsFile { get => Path.Combine(PresetsFolder, PresetsFileName); }

    public static ILogger Logger { get; set; } = null;

    public static JsonSerializerSettings SerializerSettings => new JsonSerializerSettings() {
      NullValueHandling = NullValueHandling.Ignore,
      Formatting = Formatting.Indented,
      ContractResolver = new CamelCasePropertyNamesContractResolver(),
      Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() }
    };

  }
}
