using Microsoft.Extensions.Logging;
using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Configuration;
using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Enums;
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
      // Find assembly
      var a = Assembly.GetExecutingAssembly().GetReferencedAssemblies().FirstOrDefault(a => a.Name == _options.PluginId);

      if (a == null) {
        _logger.LogError($"Unable to load assembly '{_options.PluginId}' for reflection.'");
        return;
      }

      // Load assembly
      var assembly = Assembly.Load(a);

      string basePath = $"%TP_PLUGIN_FOLDER%{_options.PluginFolder}/";
      bool useCustomConfigs = _options.StateFiles.Any();

      // Get version and see if we need a custom version number
      VersionInfo.Assembly = assembly;
      uint vNum = VersionInfo.GetProductVersionNumber() >> 8;  // strip the patch level
      // add custom config version if using custom files
      if (useCustomConfigs) {
        uint cVer = _options.ConfigVersion << PluginConfig.ENTRY_FILE_CONF_VER_SHIFT;
        if (cVer > PluginConfig.ENTRY_FILE_VER_MASK_CUSTOM)
          cVer = PluginConfig.ENTRY_FILE_VER_MASK_CUSTOM;
        vNum |= cVer;
      }

      // read states config
      IEnumerable<SimVarItem> simVars = Array.Empty<SimVarItem>();
      if (useCustomConfigs) {
        if (!string.IsNullOrWhiteSpace(_options.StateFilesPath))
          PluginConfig.UserConfigFolder = _options.StateFilesPath;
        simVars = _pluginConfig.LoadCustomSimVars(_options.StateFiles);
        _logger.LogInformation($"Generating entry.tp v{vNum:X} to '{_options.OutputPath}' with Custom states from file(s): {string.Join(", ", _options.StateFiles)}");
      }
      else {
        simVars = _pluginConfig.LoadSimVarItems(false);
        _logger.LogInformation($"Generating entry.tp v{vNum:X} to '{_options.OutputPath}' with Default states.");
      }
      // always include the internal plugin states
      simVars = simVars.Concat(_pluginConfig.LoadPluginStates());


      var q = assembly.GetTypes().ToList();

      // Setup Base Model
      var model = new Base {
        Sdk = 3,
        Version = vNum,
        Name = _options.PluginName,
        Id = _options.PluginId
      };
      if (!_options.Debug)
        model.Plugin_start_cmd = $"{basePath}dist/{_options.PluginId}.exe";


      // Get all classes with the TouchPortalCategory
      var categoryClasses = q.Where(t => t.CustomAttributes.Any(att => att.AttributeType == typeof(TouchPortalCategoryAttribute))).OrderBy(o => o.Name);

      // For each category, add to model
      foreach (var cat in categoryClasses) {
        var att = (TouchPortalCategoryAttribute)Attribute.GetCustomAttribute(cat, typeof(TouchPortalCategoryAttribute));
        Groups catId = att.Id;
        string catIdStr = $"{_options.PluginId}.{catId}";
        var category = model.Categories.FirstOrDefault(c => c.Id == catIdStr);
        if (category == null) {
          category = new TouchPortalCategory {
            Id = catIdStr,
            Name = Categories.FullCategoryName(catId),
            Imagepath = basePath + Categories.CategoryImage(catId)
          };
          model.Categories.Add(category);
        }

        // workaround for backwards compat with mis-named actions in category InstrumentsSystems.Fuel
        string actionCatId = _options.PluginId + "." + Categories.ActionCategoryId(catId);

        // Add actions
        var actions = cat.GetMembers().Where(t => t.CustomAttributes.Any(att => att.AttributeType == typeof(TouchPortalActionAttribute))).ToList();
        actions.ForEach(act => {
          var actionAttribute = (TouchPortalActionAttribute)Attribute.GetCustomAttribute(act, typeof(TouchPortalActionAttribute));
          var action = new TouchPortalAction {
            Id = $"{actionCatId}.Action.{actionAttribute.Id}",
            Name = actionAttribute.Name,
            Prefix = actionAttribute.Prefix,
            Type = actionAttribute.Type,
            Description = actionAttribute.Description,
            TryInline = true,
            Format = actionAttribute.Format,
            HasHoldFunctionality = actionAttribute.HasHoldFunctionality,
          };

          // Action Data
          var dataAttributes = act.GetCustomAttributes<TouchPortalActionDataAttribute>();
          if (dataAttributes.Any()) {
            for (int i = 0, e = dataAttributes.Count(); i < e; ++i) {
              var attrib = dataAttributes.ElementAt(i);
              var data = new TouchPortalActionData {
                Id = $"{action.Id}.Data.{i}",
                Type = attrib.Type,
                Label = attrib.Label ?? "Action",
                DefaultValue = attrib.GetDefaultValue(),
                ValueChoices = attrib.ChoiceValues,
                MinValue = attrib.MinValue,
                MaxValue = attrib.MaxValue,
                AllowDecimals = attrib.AllowDecimals,
              };

              action.Data.Add(data);
            }
            action.Format = string.Format(action.Format, action.Data.Select(d => $"{{${d.Id}$}}").ToArray());
          }

          // validate unique ID
          if (category.Actions.FirstOrDefault(a => a.Id == action.Id) == null)
            category.Actions.Add(action);
          else
            _logger.LogWarning($"Duplicate action ID found: '{action.Id}', skipping.'");
        });

        // Sort the actions
        category.Actions = category.Actions.OrderBy(c => c.Name).ToList();

        // States
        if (category.States.Any())
          continue;  // skip if already added for this category

        var categoryStates = simVars.Where(s => s.CategoryId == catId);
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

        // Sort the states
        category.States = category.States.OrderBy(c => c.Description).ToList();
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
