using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MSFSTouchPortalPlugin.Configuration;
using MSFSTouchPortalPlugin.Enums;
using MSFSTouchPortalPlugin.Helpers;
using MSFSTouchPortalPlugin.Types;
using MSFSTouchPortalPlugin_Generator.Configuration;
using MSFSTouchPortalPlugin_Generator.Interfaces;
using MSFSTouchPortalPlugin_Generator.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin_Generator
{
  internal class GenerateEntry : IGenerateEntry {
    private readonly ILogger<GenerateEntry> _logger;
    private readonly IOptions<GeneratorOptions> _options;

    public GenerateEntry(ILogger<GenerateEntry> logger, IOptions<GeneratorOptions> options) {
      _logger = logger;
      _options = options;

      JsonConvert.DefaultSettings = () => new JsonSerializerSettings {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
      };
    }

    public void Generate() {
      // Find assembly
      var a = Assembly.GetExecutingAssembly().GetReferencedAssemblies().FirstOrDefault(a => a.Name == _options.Value.PluginName);

      if (a == null) {
        throw new FileNotFoundException("Unable to load assembly for reflection.");
      }

      var assembly = Assembly.Load(a);
      string basePath = $"%TP_PLUGIN_FOLDER%{_options.Value.PluginFolder}";

      // read default states config
      // TODO: Allow configuration of which state config file(s) to read.
      var pc = PluginConfig.Instance;
      var configStates = pc.LoadSimVarItems(false);
      if (pc.HaveErrors) {
        foreach (var e in pc.ErrorsList)
          _logger.LogError(e, "Configuration reader error:");
      }

      // Load assembly
      _ = MSFSTouchPortalPlugin.Objects.Plugin.Plugin.Init;
      var q = assembly.GetTypes().ToList();

      // Setup Base Model
      VersionInfo.Assembly = assembly;
      var model = new Base {
        Sdk = 3,
        Version = VersionInfo.GetProductVersionNumber(),
        Name = _options.Value.PluginName,
        Id = _options.Value.PluginName,
        Plugin_start_cmd = $"{basePath}/dist/{_options.Value.PluginName}.exe"
      };

      // Get all classes with the TouchPortalCategory
      var categoryClasses = q.Where(t => t.CustomAttributes.Any(att => att.AttributeType == typeof(TouchPortalCategoryAttribute))).OrderBy(o => o.Name);

      // For each category, add to model
      foreach (var cat in categoryClasses) {
        var att = (TouchPortalCategoryAttribute)Attribute.GetCustomAttribute(cat, typeof(TouchPortalCategoryAttribute));
        // FIXME: the Split() is necessary due to legacy mis-named category InstrumentsSystems.Fuel
        if (!Enum.TryParse(att.Id.Split('.').Last(), false, out Groups catId)) {
          _logger.LogWarning($"Could not parse category ID: '{att.Id}', skipping.'");
          continue;
        }

        var category = model.Categories.FirstOrDefault(c => c.Name == att.Name);
        if (category == null) {
          category = new TouchPortalCategory {
            // FIXME: For now use attribute Id (att.Id) instead of actual parsed Groups enum (catId) for backwards compat with mis-named actions in category InstrumentsSystems.Fuel
            Id = $"{_options.Value.PluginName}.{att.Id}",
            Name = att.Name,
            // Imagepath = att.ImagePath
            Imagepath = basePath + "/airplane_takeoff24.png"
          };
          model.Categories.Add(category);
        }

        // Add actions
        var actions = cat.GetMembers().Where(t => t.CustomAttributes.Any(att => att.AttributeType == typeof(TouchPortalActionAttribute))).ToList();
        actions.ForEach(act => {
          var actionAttribute = (TouchPortalActionAttribute)Attribute.GetCustomAttribute(act, typeof(TouchPortalActionAttribute));
          var action = new TouchPortalAction {
            Id = $"{category.Id}.Action.{actionAttribute.Id}",
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

        System.Collections.Generic.IEnumerable<SimVarItem> categoryStates;
        // Plugin (non SimConnect) states are stored in a separate config file.
        if (catId == Groups.Plugin)
          categoryStates = pc.LoadPluginStates();
        else
          categoryStates = configStates.Where(s => s.CategoryId == catId);

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
      var setContainers = q.Where(t => t.CustomAttributes.Any(att => att.AttributeType == typeof(TouchPortalSettingsContainerAttribute))).OrderBy(o => o.Name).ToList();
      setContainers.ForEach(setCtr => {
        var settingsList = setCtr.GetMembers().Where(t => t.CustomAttributes.Any(att => att.AttributeType == typeof(TouchPortalSettingAttribute))).ToList();
        settingsList.ForEach(setType => {
          var att = (TouchPortalSettingAttribute)Attribute.GetCustomAttribute(setType, typeof(TouchPortalSettingAttribute));
          var setting = new TouchPortalSetting {
            Name = att.Name,
            Type = att.Type,
            DefaultValue = att.Default,
            IsPassword = att.IsPassword,
            ReadOnly = att.ReadOnly
          };
          if (att.MaxLength > 0)
            setting.MaxLength = att.MaxLength;
          if (!double.IsNaN(att.MinValue))
            setting.MinValue = att.MinValue;
          if (!double.IsNaN(att.MaxValue))
            setting.MaxValue = att.MaxValue;

          // validate unique Name
          if (model.Settings.FirstOrDefault(s => s.Name == setting.Name) == null)
            model.Settings.Add(setting);
          else
            _logger.LogWarning($"Duplicate Setting Name found: '{setting.Name}', skipping.'");
        });
      });

      var context = new ValidationContext(model, null, null);
      var errors = new Collection<ValidationResult>();
      var isValid = Validator.TryValidateObject(model, context, errors, true);

      if (!isValid) {
        throw new AggregateException(
          errors.Select((e) => new ValidationException(e.ErrorMessage)));
      }

      var result = JsonConvert.SerializeObject(model, Formatting.Indented);
      File.WriteAllText(Path.Combine(_options.Value.TargetPath, "entry.tp"), result);
      _logger.LogInformation("entry.tp generated.");
    }
  }
}
