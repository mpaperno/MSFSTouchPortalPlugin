using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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

namespace MSFSTouchPortalPlugin_Generator {
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

      var fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
      var version = fvi.FileVersion;

      // Setup Base Model
      var model = new Base {
        Sdk = 3,
        Version = int.Parse(version.Replace(".", "")),
        Name = _options.Value.PluginName,
        Id = _options.Value.PluginName
      };

      // Add Configuration
      // Add Plug Start Comand
      model.Plugin_start_cmd = Path.Combine("%TP_PLUGIN_FOLDER%", "MSFS-TouchPortal-Plugin\\dist", "MSFSTouchPortalPlugin.exe");
      // Load asembly
      _ = MSFSTouchPortalPlugin.Objects.Plugin.Plugin.Init;

      var q = assembly.GetTypes().ToList();

      // Get all classes with the TouchPortalCategory
      var s = q.Where(t => t.CustomAttributes.Any(att => att.AttributeType == typeof(TouchPortalCategoryAttribute))).OrderBy(o => o.Name).ToList();

      // For each category, add to model
      s.ForEach(cat => {
        var att = (TouchPortalCategoryAttribute)Attribute.GetCustomAttribute(cat, typeof(TouchPortalCategoryAttribute));
        bool newCatCreated = false;
        var category = model.Categories.FirstOrDefault(c => c.Name == att.Name);
        if (category == null) {
          category = new TouchPortalCategory {
            Id = $"{_options.Value.PluginName}.{att.Id}",
            Name = att.Name,
            // Imagepath = att.ImagePath
            Imagepath = Path.Combine("%TP_PLUGIN_FOLDER%", "MSFS-TouchPortal-Plugin", "airplane_takeoff24.png")
          };
          newCatCreated = true;
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

        // Ordering
        category.Actions = category.Actions.OrderBy(c => c.Name).ToList();

        // States
        var states = cat.GetMembers().Where(t => t.CustomAttributes.Any(att => att.AttributeType == typeof(TouchPortalStateAttribute))).ToList();
        states.ForEach(state => {
          var stateAttribute = state.GetCustomAttribute<TouchPortalStateAttribute>();
          var newState = new TouchPortalState {
            Id = $"{category.Id}.State.{stateAttribute.Id}",
            Type = stateAttribute.Type,
            Description = $"{category.Name} - {stateAttribute.Description}",
            DefaultValue = stateAttribute.Default
          };

          // validate unique ID
          if (category.States.FirstOrDefault(s => s.Id == newState.Id) == null)
            category.States.Add(newState);
          else
            _logger.LogWarning($"Duplicate state ID found: '{newState.Id}', skipping.'");
        });

        // Ordering
        category.States = category.States.OrderBy(c => c.Description).ToList();

        // Add events

        if (newCatCreated)
          model.Categories.Add(category);
      });

      // Ordering
      model.Categories = model.Categories.OrderBy(c => c.Name).ToList();

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
