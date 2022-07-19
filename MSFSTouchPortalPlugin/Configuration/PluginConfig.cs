/*
This file is part of the MSFS Touch Portal Plugin project.
https://github.com/mpaperno/MSFSTouchPortalPlugin

COPYRIGHT: (c) Maxim Paperno; All Rights Reserved.

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

using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Enums;
using MSFSTouchPortalPlugin.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

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

    public static string StatesConfigFile { get; set; }       = "States.ini";
    public static string PluginStatesConfigFile { get; set; } = "PluginStates.ini";
    public static string SimVarsImportsFile { get; set; }     = "SimVariables.ini";
    public static string SimEventsImportsFile { get; set; }   = "SimEvents.ini";

    public static string AppRootFolder    { get; set; } = Path.GetDirectoryName(AppContext.BaseDirectory);
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

    public IEnumerable<string> DefaultStateIds { get; private set; }
    public IEnumerable<string> ImportedSimVarCategoryNames => _importedSimVars.Keys;
    public IEnumerable<string> ImportedSimEvenCategoryNames => _importedSimEvents.Keys;

    const string STR_DEFAULT = "default";
    static readonly string _defaultUserCfgFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), RootName);
    static readonly Regex _reSimVarIdFromName = new Regex(@"(?:\b|\W|_)(\w)");  // for creating a PascalCase ID from a SimVar name
    static string _currentUserCfgFolder = _defaultUserCfgFolder;
    static string[] _userStateFiles = Array.Empty<string>();
    IReadOnlyDictionary<string, IReadOnlyDictionary<string, SimVariable>> _importedSimVars;
    IReadOnlyDictionary<string, IReadOnlyDictionary<string, SimEvent>> _importedSimEvents;
    readonly ILogger<PluginConfig> _logger;

    public PluginConfig(ILogger<PluginConfig> logger) {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));

      // set SC writing options
      SharpConfig.Configuration.SpaceBetweenEquals = true;
      SharpConfig.Configuration.AlwaysQuoteStringValues = true;  // custom SharpConfig v3.2.9.2-mp feature
    }

    // Loads all imports
    public void Init() {
      DefaultStateIds = LoadSimVarItems(false).Select(s => s.Id);
      _importedSimVars = ImportSimVars();
      _importedSimEvents = ImportSimEvents();
      // Check for custom SimConnect.cfg and try copy it to application dir (may require elevated privileges)
      CopySimConnectConfig();
    }

    // SimVar/SimVariable helper methods

    public bool TryGetImportedSimVarNamesForCateogy(string simCategoryName, out IEnumerable<string> list) {
      if (_importedSimVars.TryGetValue(simCategoryName, out var dict)) {
        list = dict.Keys;
        return true;
      }
      list = Array.Empty<string>();
      return false;
    }

    public bool TryGetImportedSimVarsForCateogy(string simCategoryName, out IEnumerable<SimVariable> list) {
      if (_importedSimVars.TryGetValue(simCategoryName, out var dict)) {
        list = dict.Values;
        return true;
      }
      list = Array.Empty<SimVariable>();
      return false;
    }

    public bool TryGetImportedSimVarBySelector(string selector, out SimVariable simVar) {
      return TryGetImportedSimVarBySelector(selector, out simVar, out _, out _);
    }

    bool TryGetImportedSimVarBySelector(string varName, out SimVariable simVar, out string cleanName, out uint index) {
      simVar = null;
      if (!TryNormalizeVarName(varName, out cleanName, out index))
        return false;
      foreach (var cat in _importedSimVars.Values) {
        if (cat.TryGetValue(cleanName, out simVar))
          return true;
      }
      return false;
    }

    // "normalize" a SimVar name passed from touch portal
    static bool TryNormalizeVarName(string name, out string varName, out uint index) {
      index = 0;
      varName = name.Trim();
      if (string.IsNullOrEmpty(varName))
        return false;
      // strip leading "exists" indicator "* "
      if (varName[0] == '*')
        varName = varName[2..];
      // strip trailing single-char index indicator or digit, ":N" or ":i" or ":1" and such
      if (varName[^2] == ':') {
        if (uint.TryParse(varName[^1].ToString(), out uint res))
          index = res;
        varName = varName[..^2];
      }
      // trailing 2 digit index number
      else if (varName[^3] == ':') {
        if (uint.TryParse(varName[..^2], out uint res))
          index = res;
        varName = varName[..^3];
      }
      // otherwise check and strip ":index" suffix
      else if (varName[^6] == ':' && varName[^5..].ToLowerInvariant() == "index")
        varName = varName[..^6];
      return true;
    }

    // This is a helper for creating SimVars dynamically at runtime. It is here to centralize how some of the
    // information is populated/formatted to keep things consistent with SimVars read from config files.
    public SimVarItem CreateDynamicSimVarItem(char varType, string varName, Groups catId, string unit, uint index = 0) {
      SimVarItem simVar;
      string name = varName;
      uint parsedIndex = 0;
      if (varType == 'A' && TryGetImportedSimVarBySelector(varName, out SimVariable impSimVar, out name, out parsedIndex)) {
        simVar = new SimVarItem() {
          Id = impSimVar.Id,
          VariableType = varType,
          Name = impSimVar.Name,
          SimVarName = impSimVar.SimVarName,
          CanSet = impSimVar.CanSet
        };
      }
      else {
        if (varType != 'A')
          name = name.Trim();
        simVar = new SimVarItem() {
          // Create a reasonable string for a TP state ID
          Id = _reSimVarIdFromName.Replace(name.ToLower(), m => (m.Groups[1].ToString().ToUpper())),
          VariableType = varType,
          Name = name,  // for lack of anything better
          SimVarName = name,
        };
      }
      if (parsedIndex > 0 && index == 0)
        index = parsedIndex;
      if (index > 0) {
        string sIdx = Math.Clamp(index, 1, 99).ToString();
        simVar.Id += sIdx;
        simVar.SimVarName += ':' + sIdx;
        simVar.Name += ' ' + sIdx;
      }
      simVar.CategoryId = catId;
      simVar.Unit = unit ?? "number";
      simVar.DataSource = DataSourceType.Dynamic;
      SetSimVarItemTpMetaData(simVar);

      return simVar;
    }

    static void SetSimVarItemTpMetaData(SimVarItem simVar) {
      simVar.TouchPortalStateId = $"{RootName}.{simVar.CategoryId}.State.{simVar.Id}";
      simVar.TouchPortalSelector = $"{simVar.Name} ({simVar.Unit}) [{simVar.Id}]";
    }


    // Imported SimEvents methods

    public bool TryGetImportedSimEventNamesForCateogy(string simCategoryName, out IEnumerable<string> list) {
      if (_importedSimEvents.TryGetValue(simCategoryName, out var dict)) {
        list = dict.Keys;
        return true;
      }
      list = Array.Empty<string>();
      return false;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static")]
    public bool TryGetImportedSimEventIdFromSelector(string selector, out string eventId) {
      eventId = selector?.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).First() ?? string.Empty;
      return !string.IsNullOrWhiteSpace(eventId);
    }


    // Config File loaders

    // Check if user config folder contains a SimConnect.cfg file and tries to copy it into the current running folder.
    public bool CopySimConnectConfig() {
      string filename = "SimConnect.cfg";
      string srcFile = Path.Combine(UserConfigFolder, filename);
      bool ret = File.Exists(srcFile);
      if (!ret && !File.Exists(filename))  // check that it exists in current directory
        srcFile = Path.Combine(AppConfigFolder, filename);

      if (File.Exists(srcFile)) {
        try {
          File.Copy(srcFile, Path.Combine(Directory.GetCurrentDirectory(), filename), true);
          _logger.LogInformation($"Copied SimConnect.cfg file from '{srcFile}'.");
        }
        catch (Exception e) {
          _logger.LogError($"Error trying to copy SimConnect.cfg file from '{srcFile}': {e.Message}");
          ret = false;
        }
      }
      return ret;
    }

    // Loads an individual sim var states config file, either from user's config folder, the default app config location, or a full file path
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
        simVar.DataSource = isUserConfig ? DataSourceType.UserFile : DataSourceType.DefaultFile;
        SetSimVarItemTpMetaData(simVar);
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

    // Loads all sim var config as per current configuration of user/default state files
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

    // Loads the definitions of the internal plugin states
    public IReadOnlyCollection<SimVarItem> LoadPluginStates()
      => LoadSimVarItems(false, PluginStatesConfigFile);

    // Save collection to file
    public int SaveSimVarItems(IEnumerable<SimVarItem> items, bool isUserConfig = true, string filename = default) {
      if (!items.Any())
        return 0;
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
          sect.Add("VariableType", item.VariableType);
          sect.Add("SimVarName", item.SimVarName);
          if (item.VariableType == 'Q')
            sect.Add("CalcResultType", item.CalcResultType);
          else
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
      if (SaveToFile(cfg, filename))
        _logger.LogDebug($"Saved {count} SimVars to '{filename}'");
      else
        count = 0;
      return count;
    }


    // private importers, loaded data is stored internally

    IReadOnlyDictionary<string, IReadOnlyDictionary<string, SimVariable>> ImportSimVars() {
      Dictionary<string, IReadOnlyDictionary<string, SimVariable>> ret = new();
      var filename = Path.Combine(AppConfigFolder, SimVarsImportsFile);
      _logger.LogDebug($"Importing SimVars from file '{filename}'...");

      if (!LoadFromFile(filename, out var cfg))
        return ret;

      int count = 0;
      string currCatName = string.Empty;
      Dictionary<string, SimVariable> catDict = null;
      foreach (SharpConfig.Section section in cfg) {
        if (section.Name == SharpConfig.Section.DefaultSectionName)
          continue;

        // new category
        if (section.Name.StartsWith("category_")) {
          if (catDict != null) {
            ret[currCatName] = catDict.OrderBy(kv => kv.Key).ToDictionary(s => s.Key, s => s.Value);
          }
          if (section.TryGetSetting("Name", out var setting)) {
            currCatName = setting.StringValue;
            // the categories should be sequential, but JIC we check if we already have it
            catDict = ret.GetValueOrDefault(currCatName, new Dictionary<string, SimVariable>()).ToDictionary(s => s.Key, s => s.Value);
          }
          continue;
        }
        if (catDict == null)
          continue;

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
        simVar.TouchPortalSelectorName = $"{simVar.SimVarName}{(simVar.Indexed ? ":N" : "")}";
        // pad with 2 spaces per char because TP uses _very_ proportional fonts and we need the names to be of same width
        simVar.TouchPortalSelectorName += string.Concat(Enumerable.Repeat("  ", Math.Max(0, 45 - simVar.TouchPortalSelectorName.Length)));

        // check unique
        if (!catDict.TryAdd(simVar.SimVarName, simVar)) {
          catDict[simVar.SimVarName] = simVar;
          _logger.LogWarning($"Duplicate SimVar ID found for '{simVar.Id}' ('{simVar.SimVarName}'), overwriting.");
        }
        else {
          ++count;
        }
      }
      ret = ret.OrderBy(s => s.Key).ToDictionary(s => s.Key, s => s.Value);
      _logger.LogDebug($"Imported {count} SimVars in {ret.Count} categories from '{filename}'");
      return ret;
    }

    IReadOnlyDictionary<string, IReadOnlyDictionary<string, SimEvent>> ImportSimEvents() {
      Dictionary<string, IReadOnlyDictionary<string, SimEvent>> ret = new();
      var filename = Path.Combine(AppConfigFolder, SimEventsImportsFile);
      _logger.LogDebug($"Importing SimEvents from file '{filename}'...");

      if (!LoadFromFile(filename, out var cfg))
        return ret;

      int count = 0;
      string currCatName = string.Empty;
      Dictionary<string, SimEvent> catDict = null;
      foreach (SharpConfig.Section section in cfg) {
        if (section.Name == SharpConfig.Section.DefaultSectionName)
          continue;

        // new category
        if (section.Name.StartsWith("category_")) {
          if (catDict != null) {
            ret[currCatName] = catDict.OrderBy(kv => kv.Key).ToDictionary(s => s.Key, s => s.Value);
          }
          if (section.TryGetSetting("Name", out var setting)) {
            currCatName = setting.StringValue;
            // the categories should be sequential, but JIC we check if we already have it
            catDict = ret.GetValueOrDefault(currCatName, new Dictionary<string, SimEvent>()).ToDictionary(s => s.Key, s => s.Value);
          }
          continue;
        }
        if (catDict == null)
          continue;

        SimEvent simEvt;
        try {
          simEvt = section.ToObject<SimEvent>();
        }
        catch (Exception e) {
          _logger.LogError(e, $"Deserialize exception for section '{section}': {e.Message}:");
          continue;
        }
        if (simEvt == null) {
          _logger.LogError($"Produced SimEvent is null from section '{section}':");
          continue;
        }
        // INI section name is the ID
        simEvt.Id = section.Name;
        // set up a name to use in the TP UI selection list
        simEvt.TouchPortalSelectorName = $"{simEvt.Id} - {simEvt.Name}";

        // check unique
        if (!catDict.TryAdd(simEvt.TouchPortalSelectorName, simEvt)) {
          catDict[simEvt.TouchPortalSelectorName] = simEvt;
          _logger.LogWarning($"Duplicate SimEvent ID found for '{simEvt.Id}', overwriting.");
        }
        else {
          ++count;
        }
      }
      ret = ret.OrderBy(s => s.Key).ToDictionary(s => s.Key, s => s.Value);
      _logger.LogDebug($"Imported {count} SimEvents in {ret.Count} categories from '{filename}'");
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
