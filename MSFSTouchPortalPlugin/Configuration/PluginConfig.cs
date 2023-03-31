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
    public List<string> LoadedStateConfigFiles = new();

    const string STR_DEFAULT = "default";
    static readonly string _defaultUserCfgFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), RootName);
    static readonly Regex _reSimVarIdFromName = new Regex(@"(?:\b|\W|_)(\w)");  // for creating a PascalCase ID from a SimVar name
    string _currentUserCfgFolder = _defaultUserCfgFolder;
    string[] _userStateFiles = Array.Empty<string>();
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
      // Check for custom SimConnect.cfg and try copy it to application dir (may require elevated privileges)
      CopySimConnectConfig();
    }

    // SimVar/SimVariable helper methods

    // "normalize" a SimVar name passed from touch portal
    public static bool TryNormalizeVarName(string name, out string varName, out uint index) {
      index = 0;
      // Some values may have multiple lines
      varName = name.Split('\n', 2, StringSplitOptions.TrimEntries)[0].Trim();
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
      //else if (varName[^6] == ':' && varName[^5..].ToLowerInvariant() == "index")
      //  varName = varName[..^6];
      return true;
    }

    // This is a helper for creating SimVars dynamically at runtime. It is here to centralize how some of the
    // information is populated/formatted to keep things consistent with SimVars read from config files.
    public static SimVarItem CreateDynamicSimVarItem(char varType, string varName, Groups catId, string unit, uint index, SimVarCollection current = null)
    {
      string varId;
      if (!TryNormalizeVarName(varName, out var name, out uint parsedIndex))
        return null;
      varName = name;
      varId = _reSimVarIdFromName.Replace(name.ToLower(), m => (m.Groups[1].ToString().ToUpper()));
      if (parsedIndex > 0 && index == 0)
        index = parsedIndex;
      if (index > 0) {
        string sIdx = Math.Clamp(index, 1, 99).ToString();
        varId += sIdx;
        varName += ':' + sIdx;
        name += ' ' + sIdx;
      }

      // check if this variable already exists, avoid duplicates and keep any default naming
      if (current != null && current.TryGetBySimName(varName, out SimVarItem existingVar)) {
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
        DefinitionSource = SimVarDefinitionSource.Dynamic
      };
      if (catId != Groups.None)
        SetSimVarItemTpMetaData(simVar);
      return simVar;
    }

    static void SetSimVarItemTpMetaData(SimVarItem simVar)
    {
      simVar.TouchPortalStateId = $"{RootName}.{simVar.CategoryId}.State.{simVar.Id}";
      simVar.TouchPortalSelector = $"{simVar.Name} ({simVar.Unit}) [{simVar.Id}]";
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
    public IReadOnlyCollection<SimVarItem> LoadSimVarItems(bool isUserConfig = true, string filename = default)
    {
      List<SimVarItem> ret = new();
      filename = GetFullFilePath(filename, isUserConfig);

      _logger.LogDebug("Loading SimVars from file '{filename}'...", filename);
      if (!LoadFromFile(filename, out SharpConfig.Configuration cfg))
        return ret;

      foreach (SharpConfig.Section item in cfg) {
        if (item.Name == SharpConfig.Section.DefaultSectionName)
          continue;
        SimVarItem simVar;
        try {
          simVar = item.ToObject<SimVarItem>();
        }
        catch (Exception e) {
          _logger.LogError(e, "Exception while importing Variable Request '{item}': {message}:", item, e.Message);
          continue;
        }
        if (simVar == null) {
          _logger.LogError("Produced SimVar is null from section '{item}':", item);
          continue;
        }
        simVar.Id = item.Name;
        simVar.DefinitionSource = isUserConfig ? SimVarDefinitionSource.UserFile : SimVarDefinitionSource.DefaultFile;

        if (!simVar.Validate(out var validationError)) {
          _logger.LogError("Validation error while importing Variable Request '{itemName}': {error}.", item.Name, validationError);
          continue;
        }
        else if (!string.IsNullOrEmpty(validationError)) {
          _logger.LogWarning("Validation warning while importing Variable Request '{itemName}': {error}.", item.Name, validationError);
        }

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

      string bareName = Path.GetFileName(filename);
      if (!LoadedStateConfigFiles.Contains(bareName))
        LoadedStateConfigFiles.Add(bareName);
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
