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

using Microsoft.Extensions.Logging;
using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Enums;
using MSFSTouchPortalPlugin.Helpers;
using MSFSTouchPortalPlugin.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MSFSTouchPortalPlugin.Configuration
{
  internal class PluginConfig
  {
    /// <summary> RootName is used as the basis for the user folder name and TP State ID generation. </summary>
    public static string RootName { get; set; } = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

    public static string SettingsConfigFile { get; set; } = "plugin_settings.ini";
    public static string StatesConfigFile { get; set; }       = "States.ini";
    public static string PluginStatesConfigFile { get; set; } = "PluginStates.ini";
    public static string SimVarsImportsFile { get; set; }     = "SimVariables.ini";
    public static string SimEventsImportsFile { get; set; }   = "SimEvents.ini";

    public static string AppRootFolder    { get; set; } = Path.GetDirectoryName(AppContext.BaseDirectory);
    public static string AppConfigFolder  { get; set; } = Path.Combine(AppRootFolder, "Configuration");

    public string UserConfigFolder {
      get => _currentUserCfgFolder;
      set {
        string newFolder;
        if (string.IsNullOrWhiteSpace(value) || (value.Trim() is var val && val.ToLower() == STR_DEFAULT))
          newFolder = _defaultUserCfgFolder;
        else
          newFolder = val;
        if (newFolder != _currentUserCfgFolder) {
          CopySettingsConfigFile(_currentUserCfgFolder, newFolder);
          _currentUserCfgFolder = newFolder;
        }
      }
    }

    public string UserStateFiles
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

    public bool HaveUserStateFiles => _userStateFiles.Any();
    public IReadOnlyCollection<string> UserStateFilesArray => _userStateFiles;
    public IEnumerable<string> ImportedSimVarCategoryNames => _importedSimVars.Keys;
    public IEnumerable<string> ImportedSimEvenCategoryNames => _importedSimEvents.Keys;

    const string STR_DEFAULT = "default";
    static readonly string _defaultUserCfgFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), RootName);
    static readonly Regex _reSimVarIdFromName = new Regex(@"(?:\b|\W|_)(\w)");  // for creating a PascalCase ID from a SimVar name
    string _currentUserCfgFolder = _defaultUserCfgFolder;
    string[] _userStateFiles = Array.Empty<string>();
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
      ReadOrCreateSettingsFile();
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
    public SimVarItem CreateDynamicSimVarItem(char varType, string varName, Groups catId, string unit, uint index, SimVarCollection current)
    {
      string name = varName;
      string varId;
      bool canSet = false;
      uint parsedIndex = 0;
      if (varType == 'A' && TryGetImportedSimVarBySelector(varName, out SimVariable impSimVar, out name, out parsedIndex)) {
        varId = impSimVar.Id;
        name = impSimVar.Name;
        varName = impSimVar.SimVarName;
        canSet = impSimVar.CanSet;
      }
      else {
        if (varType != 'A')
          name = name.Trim();
        varName = name;
        // Create a reasonable string for a TP state ID
        varId = _reSimVarIdFromName.Replace(name.ToLower(), m => (m.Groups[1].ToString().ToUpper()));
      }
      if (parsedIndex > 0 && index == 0)
        index = parsedIndex;
      if (index > 0) {
        string sIdx = Math.Clamp(index, 1, 99).ToString();
        varId += sIdx;
        varName += ':' + sIdx;
        name += ' ' + sIdx;
      }

      // check if this variable already exists, avoid duplicates and keep any default naming
      if (current.TryGetBySimName(varName, out SimVarItem existingVar)) {
        varId = existingVar.Id;
        if (varType != 'Q')
          name = existingVar.Name;
      }

      SimVarItem simVar = new SimVarItem() {
        Id = varId,
        VariableType = varType,
        CategoryId = catId,
        Name = name,
        SimVarName = varName,
        Unit = unit ?? "number",
        CanSet = canSet,
        DefinitionSource = SimVarDefinitionSource.Dynamic
      };
      SetSimVarItemTpMetaData(simVar);
      return simVar;
    }

    static void SetSimVarItemTpMetaData(SimVarItem simVar)
    {
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

    void ReadOrCreateSettingsFile()
    {
      string cfgFile = Path.Combine(UserConfigFolder, SettingsConfigFile);
      SharpConfig.Section plugin = null;
      if (LoadFromFile(cfgFile, out SharpConfig.Configuration cfg)) {
        if (cfg.Contains("Plugin")) {
          plugin = cfg["Plugin"];
          if (plugin.TryGetSetting(Settings.PluginSettingsVersion.SettingID, out var lastVersion))
            Settings.PluginSettingsVersion.Value = lastVersion.UIntValue;
          if (plugin.TryGetSetting(Settings.WasimClientIdHighByte.SettingID, out var hiByte))
            Settings.WasimClientIdHighByte.Value = hiByte.ByteValue;
          if (plugin.TryGetSetting(Settings.ActionRepeatInterval.SettingID, out var interval))
            Settings.ActionRepeatInterval.Value = interval.UIntValue;
        }
        _logger.LogDebug("Loaded settings from {cfgFile} with version {cfgVersion:X08}", cfgFile, Settings.PluginSettingsVersion.UIntValue);
      }
      bool needUpdate = cfg == null || plugin == null;
      if (Settings.WasimClientIdHighByte.UIntValue == 0) {
        var rand = new Random();
        Settings.WasimClientIdHighByte.Value = (byte)rand.Next(1, 0xFF);
        needUpdate = true;
        _logger.LogDebug("Generated new WASimClient ID high byte {clientId:X}", Settings.WasimClientIdHighByte.ByteValue);
      }
      if (needUpdate)
        SaveSettings();
    }

    public bool SaveSettings()
    {
      SharpConfig.Configuration cfg = new();
      SharpConfig.Section plugin = cfg["Plugin"];
      plugin[Settings.PluginSettingsVersion.SettingID].SetValue($"0x{VersionInfo.GetProductVersionNumber():X08}");
      plugin[Settings.WasimClientIdHighByte.SettingID].ByteValue = Settings.WasimClientIdHighByte.ByteValue;
      plugin[Settings.ActionRepeatInterval.SettingID].UIntValue = Settings.ActionRepeatInterval.UIntValue;
      return SaveToFile(cfg, Path.Combine(UserConfigFolder, SettingsConfigFile));
    }

    void CopySettingsConfigFile(string oldPath, string newPath)
    {
      oldPath = Path.Combine(oldPath, SettingsConfigFile);
      if (File.Exists(oldPath)) {
        try {
          Directory.CreateDirectory(newPath);
          File.Copy(oldPath, Path.Combine(newPath, SettingsConfigFile), true);
        }
        catch (Exception e) {
          _logger.LogError(e, "Error trying copy settings file from '{oldPath}' to '{newPath}': {message}", oldPath, newPath, e.Message);
        }
      }
    }

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
          _logger.LogInformation("Copied SimConnect.cfg file from {srcFile} folder.", ret ? "user settings" : "plugin installation");
        }
        catch (Exception e) {
          _logger.LogError("Error trying to copy SimConnect.cfg file from '{srcFile}': {message}", srcFile, e.Message);
          ret = false;
        }
      }
      return ret;
    }

    // Loads an individual sim var states config file, either from user's config folder, the default app config location, or a full file path
    public IReadOnlyCollection<SimVarItem> LoadSimVarItems(bool isUserConfig = true, string filename = default) {
      List<SimVarItem> ret = new();
      filename = GetFullFilePath(filename, isUserConfig);

      _logger.LogDebug("Loading SimVars from file '{filename}'...", filename);
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
          _logger.LogError(e, "Deserialize exception for section '{item}': {message}:", item, e.Message);
          continue;
        }
        if (simVar == null) {
          _logger.LogError("Produced SimVar is null from section '{item}':", item);
          continue;
        }
        simVar.Id = item.Name;
        simVar.DefinitionSource = isUserConfig ? SimVarDefinitionSource.UserFile : SimVarDefinitionSource.DefaultFile;
        SetSimVarItemTpMetaData(simVar);
        // check unique
        if (ret.FindIndex(s => s.Id == simVar.Id) is int idx && idx > -1) {
          _logger.LogWarning("Duplicate SimVar ID found for '{simVarId}', overwriting.", simVar.Id);
          ret[idx] = simVar;
        }
        else {
          ret.Add(simVar);
        }
      }

      _logger.LogDebug("Loaded {count} SimVars from '{filename}'", ret.Count, filename);
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
    public int SaveSimVarItems(IEnumerable<SimVarItem> items, bool isUserConfig = true, string filename = default)
    {
      if (!items.Any())
        return 0;
      var cfg = new SharpConfig.Configuration();
      Groups lastCatId = default;
      int count = 0;
      items = items.OrderBy(s => s.CategoryId);
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
            sect.Add("UpdatePeriod", item.UpdatePeriod);
          if (item.UpdateInterval != 0)
            sect.Add("UpdateInterval", item.UpdateInterval);
          if (item.DeltaEpsilon != (float)SimVarItem.DELTA_EPSILON_DEFAULT)
            sect.Add("DeltaEpsilon", item.DeltaEpsilon);

          ++count;
        }
        catch (Exception e) {
          _logger.LogError(e, "Serialize exception for {item}: {message}:", item.ToDebugString(), e.Message);
        }
      }

      filename = GetFullFilePath(filename, isUserConfig);
      if (SaveToFile(cfg, filename))
        _logger.LogDebug("Saved {count} SimVars to '{filename}'", count, filename);
      else
        count = 0;
      return count;
    }


    // private importers, loaded data is stored internally

    IReadOnlyDictionary<string, IReadOnlyDictionary<string, SimVariable>> ImportSimVars() {
      Dictionary<string, IReadOnlyDictionary<string, SimVariable>> ret = new();
      var filename = Path.Combine(AppConfigFolder, SimVarsImportsFile);
      _logger.LogDebug("Importing SimVars from file '{filename}'...", filename);

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
          _logger.LogError(e, "Deserialize exception for section '{section}': {message}:", section, e.Message);
          continue;
        }
        if (simVar == null) {
          _logger.LogError("Produced SimVar is null from section '{section}':", section);
          continue;
        }
        string normUnit = Units.NormalizedUnit(simVar.Unit);
        if (normUnit != null)
          simVar.Unit = normUnit;
        else
          _logger.LogWarning("Could not find Unit '{simVarUnit}' for '{simVarId}'", simVar.Unit, simVar.Id);

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
          _logger.LogWarning("Duplicate SimVar ID found for '{simVarId}' ('{simVarName}'), overwriting.", simVar.Id, simVar.SimVarName);
        }
        else {
          ++count;
        }
      }
      ret = ret.OrderBy(s => s.Key).ToDictionary(s => s.Key, s => s.Value);
      _logger.LogDebug("Imported {count} SimVars in {catCount} categories from '{filename}'", count, ret.Count, filename);
      return ret;
    }

    IReadOnlyDictionary<string, IReadOnlyDictionary<string, SimEvent>> ImportSimEvents() {
      Dictionary<string, IReadOnlyDictionary<string, SimEvent>> ret = new();
      var filename = Path.Combine(AppConfigFolder, SimEventsImportsFile);
      _logger.LogDebug("Importing SimEvents from file '{filename}'...", filename);

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
          _logger.LogError(e, "Deserialize exception for section '{section}': {message}:", section, e.Message);
          continue;
        }
        if (simEvt == null) {
          _logger.LogError("Produced SimEvent is null from section '{section}':", section);
          continue;
        }
        // INI section name is the ID
        simEvt.Id = section.Name;
        // set up a name to use in the TP UI selection list
        simEvt.TouchPortalSelectorName = $"{simEvt.Id} - {simEvt.Name}";

        // check unique
        if (!catDict.TryAdd(simEvt.TouchPortalSelectorName, simEvt)) {
          catDict[simEvt.TouchPortalSelectorName] = simEvt;
          _logger.LogWarning("Duplicate SimEvent ID found for '{simEvtId}', overwriting.", simEvt.Id);
        }
        else {
          ++count;
        }
      }
      ret = ret.OrderBy(s => s.Key).ToDictionary(s => s.Key, s => s.Value);
      _logger.LogDebug("Imported {count} SimEvents in {catCount} categories from '{filename}'", count, ret.Count, filename);
      return ret;
    }


    // Private common utility methods

    string GetFullFilePath(string filename, bool isUserConfig) {
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
        _logger.LogError("Config file '{filename}' was not found.", filename);
        return false;
      }
      try {
        cfg = SharpConfig.Configuration.LoadFromFile(filename, Encoding.UTF8);
      }
      catch (Exception e) {
        _logger.LogError(e, "Configuration error in '{filename}': {message}", filename, e.Message);
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
        _logger.LogError(e, "Error trying to write config file '{filename}': {message}", filename, e.Message);
      }
      return false;
    }

  }

}
