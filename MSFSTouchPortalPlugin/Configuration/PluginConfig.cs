using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Enums;
using MSFSTouchPortalPlugin.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;

namespace MSFSTouchPortalPlugin.Configuration
{

  internal class PluginConfig
  {
    // these constants are for entry.tp version number en/decoding
    public const uint ENTRY_FILE_VER_MASK_NOSTATES = 0x80000000;  // leading bit indicates an entry file with no static SimVar states at all (create all states dynamically)
    public const uint ENTRY_FILE_VER_MASK_CUSTOM   = 0x7F000000;  // any of the other 7 bits indicates an entry file generated from custom config files (do not create any dynamic states)
    public const ushort ENTRY_FILE_CONF_VER_SHIFT  = 24;          // bit position of custom config version number, used as the last 7 bits of file version

    /// <summary>
    /// RootName is used as the basis for the user folder name and TP State ID generation.
    /// </summary>
    public static string RootName { get; set; } = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

    public static string StatesConfigFile { get; set; } = "States.ini";
    public static string PluginStatesConfigFile { get; set; } = "PluginStates.ini";
    public static string SimVarsImportsFile { get; set; } = "SimVars.ini";
    public static string SimEventsImportsFile { get; set; } = "SimEvents.ini";

    public static string AppRootFolder    { get; set; } = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
    public static string AppConfigFolder  { get; set; } = Path.Combine(AppRootFolder, "Configuration");

    public static string UserConfigFolder {
      get => _currentUserCfgFolder;
      set {
        if (string.IsNullOrWhiteSpace(value) || (value.Trim() is var val && val.ToLower() == STR_DEFAULT))
          _currentUserCfgFolder = _defaultUserCfgFolder;
        else
          _currentUserCfgFolder = val;
      }
    }

    public static string UserStateFiles
    {
      get => string.Join(',', _userStateFiles);
      set {
        if (string.IsNullOrWhiteSpace(value) || value.Trim().ToLower().Replace(".ini", "") == STR_DEFAULT) {
          _userStateFiles = Array.Empty<string>();
          return;
        }
        _userStateFiles = value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        for (int i=0, e=_userStateFiles.Length; i < e; ++i) {
          if (_userStateFiles[i].ToLower().Replace(".ini", "") == STR_DEFAULT)
            _userStateFiles[i] = STR_DEFAULT;  // "normalize" the default string for simpler comparison later.
        }
      }
    }

    public static bool HaveUserStateFiles => _userStateFiles.Any();
    public static IReadOnlyCollection<string> UserStateFilesArray => _userStateFiles;


    const string STR_DEFAULT = "default";
    private static readonly string _defaultUserCfgFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), RootName);
    private static string _currentUserCfgFolder = _defaultUserCfgFolder;
    private static string[] _userStateFiles = Array.Empty<string>();
    private readonly ILogger<PluginConfig> _logger;


    public PluginConfig(ILogger<PluginConfig> logger) {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));

      // set SC writing options
      SharpConfig.Configuration.SpaceBetweenEquals = true;
      SharpConfig.Configuration.AlwaysQuoteStringValues = true;  // custom SharpConfig v3.2.9.2-mp feature
    }

    public bool CopySimConnectConfig() {
      string filename = "SimConnect.cfg";
      string srcFile = Path.Combine(UserConfigFolder, filename);
      if (File.Exists(srcFile)) {
        try {
          File.Copy(srcFile, Path.Combine(AppRootFolder, filename), true);
          return true;
        }
        catch (Exception e) {
          _logger.LogError(e, $"Error trying to copy SimConnect.cfg file from '{srcFile}': {e.Message}");
          return false;
        }
      }
      return false;
    }

    public IReadOnlyCollection<SimVarItem> LoadSimVarItems(bool isUserConfig = true, string filename = default) {
      List<SimVarItem> ret = new();
      filename = GetFullFilePath(filename, isUserConfig);

      _logger.LogDebug($"Loading SimVars from file '{filename}'...");
      if (!LoadFromFile(filename, out var cfg))
        return ret;

      foreach (SharpConfig.Section item in cfg) {
        if (item.Name == SharpConfig.Section.DefaultSectionName)
          continue;
        SimVarItem simVar;
        try {
          simVar = item.ToObject<SimVarItem>();
        }
        catch (Exception e) {
          _logger.LogError(e, $"Deserialize exception for section '{item}': {e.Message}:");
          continue;
        }
        if (simVar == null) {
          _logger.LogError($"Produced SimVar is null from section '{item}':");
          continue;
        }
        simVar.Id = item.Name;
        simVar.TouchPortalStateId = $"{RootName}.{simVar.CategoryId}.State.{simVar.Id}";
        // check unique
        if (ret.FindIndex(s => s.Id == simVar.Id) is int idx && idx > -1) {
          _logger.LogWarning($"Duplicate SimVar ID found for '{simVar.Id}', overwriting.");
          ret[idx] = simVar;
        }
        else {
          ret.Add(simVar);
        }
      }

      _logger.LogDebug($"Loaded {ret.Count} SimVars from '{filename}'");
      return ret;
    }

    public IReadOnlyCollection<SimVarItem> LoadSimVarStateConfigs() {
      if (!HaveUserStateFiles)
        return LoadSimVarItems(false);

      SimVarItem[] ret = Array.Empty<SimVarItem>();
      foreach (var file in UserStateFilesArray) {
        bool isUserCfg = file != STR_DEFAULT;
        ret = ret.Concat(LoadSimVarItems(isUserCfg, isUserCfg ? file : default)).ToArray();
      }
      return ret;
    }

    public IReadOnlyCollection<SimVarItem> LoadPluginStates()
      => LoadSimVarItems(false, PluginStatesConfigFile);

    public bool SaveSimVarItems(IEnumerable<SimVarItem> items, bool isUserConfig = true, string filename = default) {
      var cfg = new SharpConfig.Configuration();
      Groups lastCatId = default;
      int count = 0;
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

          ++count;
        }
        catch (Exception e) {
          _logger.LogError(e, $"Serialize exception for {item.ToDebugString()}: {e.Message}:");
        }
      }

      filename = GetFullFilePath(filename, isUserConfig);
      if (SaveToFile(cfg, filename) is bool ok)
        _logger.LogDebug($"Saved {count} SimVars to '{filename}'");
      return ok;
    }

    public IReadOnlyCollection<SimVariable> ImportSimVars() {
      List<SimVariable> ret = new();
      var filename = Path.Combine(AppConfigFolder, SimVarsImportsFile);
      _logger.LogDebug($"Importing SimVars from file '{filename}'...");

      if (!LoadFromFile(filename, out var cfg))
        return ret;

      foreach (SharpConfig.Section section in cfg) {
        if (section.Name == SharpConfig.Section.DefaultSectionName || section.Name.StartsWith("category_"))
          continue;  // do something with category later?

        SimVariable simVar;
        try {
          simVar = section.ToObject<SimVariable>();
        }
        catch (Exception e) {
          _logger.LogError(e, $"Deserialize exception for section '{section}': {e.Message}:");
          continue;
        }
        if (simVar == null) {
          _logger.LogError($"Produced SimVar is null from section '{section}':");
          continue;
        }
        string normUnit = Units.NormalizedUnit(simVar.Unit);
        if (normUnit != null)
          simVar.Unit = normUnit;
        else
          _logger.LogWarning($"Could not find Unit '{simVar.Unit}' for '{simVar.Id}'");

        // set a default name. Some of the Descriptions would be suitable names but it's tricky to filter that.
        if (string.IsNullOrWhiteSpace(simVar.Name))
          simVar.Name = simVar.SimVarName;
        // set up a name to use in the TP UI selection list
        simVar.TouchPortalSelectorName = $"{simVar.CategoryId} - {simVar.SimVarName}{(simVar.Indexed ? ":N" : "")}";

        // check unique
        if (ret.FindIndex(s => s.Id == simVar.Id) is int idx && idx > -1) {
          _logger.LogWarning($"Duplicate SimVar ID found for '{simVar.Id}', overwriting.");
          ret[idx] = simVar;
        }
        else {
          ret.Add(simVar);
        }
      }
      ret = ret.OrderBy(s => s.TouchPortalSelectorName).ToList();
      _logger.LogDebug($"Imported {ret.Count} SimVars from '{filename}'");
      return ret;
    }


    // Private common utility methods

    static string GetFullFilePath(string filename, bool isUserConfig) {
      if (filename == default)
        filename = StatesConfigFile;
      else if (string.IsNullOrWhiteSpace(Path.GetExtension(filename)))
        filename += ".ini";
      if (filename.IndexOfAny(new char[] { '\\', '/' }) < 0)
        filename = Path.Combine(isUserConfig ? UserConfigFolder : AppConfigFolder, filename);
      return filename;
    }

    bool LoadFromFile(string filename, out SharpConfig.Configuration cfg) {
      cfg = null;
      if (!File.Exists(filename)) {
        _logger.LogError($"Cannot import SimVars, file not found at '{filename}'");
        return false;
      }
      try {
        cfg = SharpConfig.Configuration.LoadFromFile(filename, Encoding.UTF8);
      }
      catch (Exception e) {
        _logger.LogError(e, $"Configuration error in '{filename}': {e.Message}");
        return false;
      }
      return true;
    }

    bool SaveToFile(SharpConfig.Configuration cfg, string filename) {
      try {
        Directory.CreateDirectory(Path.GetDirectoryName(filename));
        cfg.SaveToFile(filename, Encoding.UTF8);
        return true;
      }
      catch (Exception e) {
        _logger.LogError(e, $"Error trying to write config file '{filename}': {e.Message}");
      }
      return false;
    }

  }

}
