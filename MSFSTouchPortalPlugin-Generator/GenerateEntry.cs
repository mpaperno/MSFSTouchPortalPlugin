using Microsoft.Extensions.Logging;
using MSFSTouchPortalPlugin.Configuration;
using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Helpers;
using MSFSTouchPortalPlugin.Interfaces;
using MSFSTouchPortalPlugin.Types;
using MSFSTouchPortalPlugin_Generator.Configuration;
using MSFSTouchPortalPlugin_Generator.Interfaces;
using MSFSTouchPortalPlugin_Generator.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MSFSTouchPortalPlugin_Generator
{
  internal class GenerateEntry : IGenerateEntry {
    private readonly ILogger<GenerateEntry> _logger;
    private readonly GeneratorOptions _options;
    private readonly IReflectionService _reflectionSvc;
    private readonly PluginConfig _pluginConfig;

    public GenerateEntry(ILogger<GenerateEntry> logger, GeneratorOptions options, IReflectionService reflectionSvc, PluginConfig pluginConfig) {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _options = options ?? throw new ArgumentNullException(nameof(options));
      _reflectionSvc = reflectionSvc ?? throw new ArgumentNullException(nameof(reflectionSvc));
      _pluginConfig = pluginConfig ?? throw new ArgumentNullException(nameof(pluginConfig));

      JsonConvert.DefaultSettings = () => new JsonSerializerSettings {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
      };
    }

    public void Generate() {

      string basePath = $"%TP_PLUGIN_FOLDER%{_options.PluginFolder}/";
      bool useCustomConfigs = _options.StateFiles.Any();

      // Get version and see if we need a custom version number
      VersionInfo.AssemblyLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), _options.PluginId + ".dll");
      uint vNum = VersionInfo.GetProductVersionNumber() >> 8;  // strip the patch level
      // add custom config version if using custom files
      if (useCustomConfigs) {
        uint cVer = _options.ConfigVersion << PluginConfig.ENTRY_FILE_CONF_VER_SHIFT;
        if (cVer > PluginConfig.ENTRY_FILE_VER_MASK_CUSTOM)
          cVer = PluginConfig.ENTRY_FILE_VER_MASK_CUSTOM;
        vNum |= cVer;
      }

      // read states config
      // always include the internal plugin states
      IEnumerable<SimVarItem> simVars = _pluginConfig.LoadPluginStates();
      if (useCustomConfigs) {
        PluginConfig.UserConfigFolder = _options.StateFilesPath;
        PluginConfig.UserStateFiles = string.Join(',', _options.StateFiles);
        _logger.LogInformation($"Generating entry.tp v{vNum:X} to '{_options.OutputPath}' with Custom states from file(s): {PluginConfig.UserStateFiles}");
      }
      else {
        _logger.LogInformation($"Generating entry.tp v{vNum:X} to '{_options.OutputPath}' with Default states.");
      }
      simVars = simVars.Concat(_pluginConfig.LoadSimVarStateConfigs());

      // Setup Base Model
      var model = new Base {
        Sdk = 3,
        Version = vNum,
        Name = _options.PluginName,
        Id = _options.PluginId
      };
      if (!_options.Debug)
        model.Plugin_start_cmd = $"{basePath}dist/{_options.PluginId}.exe";

      var categegoryAttribs = _reflectionSvc.GetCategoryAttributes();
      foreach (var catAttrib in categegoryAttribs) {
        var category = new TouchPortalCategory {
          Id = $"{_options.PluginId}.{catAttrib.Id}",
          Name = Categories.FullCategoryName(catAttrib.Id),
          Imagepath = basePath + catAttrib.Imagepath
        };
        model.Categories.Add(category);

        // workaround for backwards compat with mis-named actions in category InstrumentsSystems.Fuel
        string actionCatId = _options.PluginId + "." + Categories.ActionCategoryId(catAttrib.Id);

        // Actions
        foreach (var actionAttrib in catAttrib.Actions) {
          var action = new TouchPortalAction {
            Id = $"{actionCatId}.Action.{actionAttrib.Id}",
            Name = actionAttrib.Name,
            Prefix = actionAttrib.Prefix,
            Type = actionAttrib.Type,
            Description = actionAttrib.Description,
            TryInline = true,
            Format = actionAttrib.Format,
            HasHoldFunctionality = actionAttrib.HasHoldFunctionality,
          };

          // Action Data
          if (actionAttrib.Data.Any()) {
            int i = 0;
            foreach (var attrib in actionAttrib.Data) {
              string dataId = (string.IsNullOrWhiteSpace(attrib.Id) ? i.ToString() : attrib.Id);
              var data = new TouchPortalActionData {
                Id = $"{action.Id}.Data.{dataId}",
                Type = attrib.Type,
                Label = attrib.Label ?? "Action",
                DefaultValue = attrib.GetDefaultValue(),
                ValueChoices = attrib.ChoiceValues,
                MinValue = attrib.MinValue,
                MaxValue = attrib.MaxValue,
                AllowDecimals = attrib.AllowDecimals,
              };
              ++i;
              action.Data.Add(data);
            }
            action.Format = string.Format(action.Format, action.Data.Select(d => $"{{${d.Id}$}}").ToArray());
          }  // action data

          // validate unique ID
          if (category.Actions.FirstOrDefault(a => a.Id == action.Id) == null)
            category.Actions.Add(action);
          else
            _logger.LogWarning($"Duplicate action ID found: '{action.Id}', skipping.'");

        }  // actions

        // States
        var categoryStates = simVars.Where(s => s.CategoryId == catAttrib.Id);
        foreach (SimVarItem state in categoryStates) {
          var newState = new TouchPortalState {
            Id = state.TouchPortalStateId,
            Type = state.TouchPortalValueType,
            Description = $"{category.Name} - {state.Name}",
            DefaultValue = state.DefaultValue ?? string.Empty,
          };
          // validate unique ID
          if (category.States.FirstOrDefault(s => s.Id == newState.Id) == null)
            category.States.Add(newState);
          else
            _logger.LogWarning($"Duplicate state ID found: '{newState.Id}', skipping.'");
        }

        // Sort the actions and states for SimConnect groups
        if (catAttrib.Id != MSFSTouchPortalPlugin.Enums.Groups.Plugin) {
          category.Actions = category.Actions.OrderBy(c => c.Name).ToList();
          category.States = category.States.OrderBy(c => c.Description).ToList();
        }
      }  // categories loop

      // Settings
      var settings = _reflectionSvc.GetSettings().Values;
      foreach (var s in settings) {
        var setting = new TouchPortalSetting {
          Name = s.Name,
          Type = s.TouchPortalType,
          DefaultValue = s.Default,
          IsPassword = s.IsPassword,
          ReadOnly = s.ReadOnly
        };
        if (s.MaxLength > 0)
          setting.MaxLength = s.MaxLength;
        if (!double.IsNaN(s.MinValue))
          setting.MinValue = s.MinValue;
        if (!double.IsNaN(s.MaxValue))
          setting.MaxValue = s.MaxValue;

        // validate unique Name
        if (model.Settings.FirstOrDefault(s => s.Name == setting.Name) == null)
          model.Settings.Add(setting);
        else
          _logger.LogWarning($"Duplicate Setting Name found: '{setting.Name}', skipping.'");
      }

      var context = new ValidationContext(model, null, null);
      var errors = new Collection<ValidationResult>();
      var isValid = Validator.TryValidateObject(model, context, errors, true);

      if (!isValid) {
        throw new AggregateException(
          errors.Select((e) => new ValidationException(e.ErrorMessage)));
      }

      var result = JsonConvert.SerializeObject(model, Formatting.Indented);
      var dest = Path.Combine(_options.OutputPath, "entry.tp");
      File.WriteAllText(dest, result);
      _logger.LogInformation($"Generated '{dest}'.");

      if (useCustomConfigs)
        return;

      // Generate the entry.tp version with no states; use version as runtime indicator for plugin
      model.Version |= PluginConfig.ENTRY_FILE_VER_MASK_NOSTATES;
      foreach (var cat in model.Categories)
        if (!cat.Id.EndsWith(".Plugin"))
          cat.States.Clear();
      result = JsonConvert.SerializeObject(model, Formatting.Indented);
      dest = Path.Combine(_options.OutputPath, "entry_no-states.tp");
      File.WriteAllText(dest, result);
      _logger.LogInformation($"Generated '{dest}'.");
    }
  }
}
