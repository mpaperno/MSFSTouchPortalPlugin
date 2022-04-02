using MSFSTouchPortalPlugin.Enums;
using MSFSTouchPortalPlugin.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
//using SharpConfig;

namespace MSFSTouchPortalPlugin.Configuration
{

  internal class PluginConfig
  {

    /// <summary>
    /// RootName is used as the basis for the user folder name and TP State ID generation.
    /// </summary>
    public static string RootName { get; set; } = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

    public static string StatesConfigFile { get; set; } = "States.ini";
    public static string PluginStatesConfigFile { get; set; } = "PluginStates.ini";

    public static string AppRootFolder    { get; set; } = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
    public static string AppConfigFolder  { get; set; } = Path.Combine(AppRootFolder, "Configuration");
    public static string UserConfigFolder { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), RootName);

    private readonly ILogger<PluginConfig> _logger;

    public PluginConfig(ILogger<PluginConfig> logger) {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));

      // set SC writing options
      SharpConfig.Configuration.SpaceBetweenEquals = true;
      SharpConfig.Configuration.AlwaysQuoteStringValues = true;  // custom SharpConfig v3.2.9.2-mp feature
    }

    public IReadOnlyCollection<SimVarItem> LoadSimVarItems(bool isUserConfig = true, string filename = default) {
      List<SimVarItem> ret = new();
      if (filename == default)
        filename = StatesConfigFile;
      string filepath = Path.Combine(isUserConfig ? UserConfigFolder : AppConfigFolder, filename);
      if (!File.Exists(filepath)) {
        _logger.LogWarning($"Cannot load SimVar states, file '{filename}' not found at '{filepath}'");
        return ret;
      }
      SharpConfig.Configuration cfg;
      try {
        cfg = SharpConfig.Configuration.LoadFromFile(filepath, Encoding.UTF8);
      }
      catch (Exception e) {
        _logger.LogWarning(e, $"Configuration LoadFromFile error in '{filepath}':");
        return ret;
      }
      foreach (SharpConfig.Section item in cfg) {
        if (item.Name == SharpConfig.Section.DefaultSectionName)
          continue;
        SimVarItem simVar;
        try {
          simVar = item.ToObject<SimVarItem>();
        }
        catch (Exception e) {
          _logger.LogWarning(e, $"Deserialize exception for section '{item}':");
          continue;
        }
        if (simVar == null) {
          _logger.LogWarning($"Produced SimVar is null from section '{item}':");
          continue;
        }
        simVar.Id = item.Name;
        // check unique
        if (ret.FirstOrDefault(s => s.Id == simVar.Id) != null) {
          _logger.LogWarning($"Duplicate SimVar ID found for '{simVar.Id}', skipping.");
          continue;
        }
        simVar.TouchPortalStateId = $"{RootName}.{simVar.CategoryId}.State.{simVar.Id}";
        ret.Add(simVar);
      }

      return ret;
    }

    public IReadOnlyCollection<SimVarItem> LoadPluginStates()
      => LoadSimVarItems(false, PluginStatesConfigFile);

    public bool SaveSimVarItems(IReadOnlyCollection<SimVarItem> items, bool isUserConfig = true, string filename = default) {
      var cfg = new SharpConfig.Configuration();
      Groups lastCatId = default;

      foreach (SimVarItem item in items) {
        if (item == null)
          continue;
        try {
          var sect = cfg.Add(item.Id);
          if (item.CategoryId != lastCatId) {
            sect.PreComment = " Category: " + item.CategoryId.ToString() + "     " + new string('#', 30) + '\n';
            lastCatId = item.CategoryId;
          }

          sect.Add("CategoryId", item.CategoryId);
          sect.Add("Name", item.Name);
          sect.Add("SimVarName", item.SimVarName);
          sect.Add("Unit", item.Unit);
          if (!string.IsNullOrWhiteSpace(item.DefaultValue))
            sect.Add("DefaultValue", item.DefaultValue);
          if (!string.IsNullOrWhiteSpace(item.FormattingString))
            sect.Add("StringFormat", item.FormattingString);
          if (item.CanSet)
            sect.Add("CanSet", item.CanSet);
          if (item.UpdatePeriod != UpdatePeriod.Default)
            sect.Add("UpdateFreqency", item.UpdatePeriod);
          if (item.UpdateInterval != 0)
            sect.Add("UpdateInterval", item.UpdateInterval);
          if (item.DeltaEpsilon != 0.0f)
            sect.Add("DeltaEpsilon", item.DeltaEpsilon);
        }
        catch (Exception e) {
          _logger.LogWarning(e, $"Serialize exception for {item.ToDebugString()}:");
        }
      }

      if (filename == default)
        filename = StatesConfigFile;
      return SaveToFile(cfg, isUserConfig ? UserConfigFolder : AppConfigFolder, filename);
    }

    private bool SaveToFile(SharpConfig.Configuration cfg, string folder, string filename) {
      try {
        Directory.CreateDirectory(folder);
        cfg.SaveToFile(Path.Combine(folder, filename), Encoding.UTF8);
        return true;
      }
      catch (Exception e) {
        _logger.LogWarning(e, $"Error trying to write config file '{filename}' to folder '{folder}'");
      }
      return false;
    }

  }

  [Serializable]
  public class DuplicateIdException : Exception
  {
    public DuplicateIdException() : base() { }
    public DuplicateIdException(string message) : base(message) { }
    protected DuplicateIdException(SerializationInfo info, StreamingContext context)
        : base(info, context) {
    }
  }

}
