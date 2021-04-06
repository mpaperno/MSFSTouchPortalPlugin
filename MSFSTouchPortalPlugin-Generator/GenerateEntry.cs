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
        throw new ArgumentNullException("Unable to load assembly for reflection.");
      }

      var assembly = Assembly.Load(a);

      var fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
      var version = fvi.FileVersion;

      // Setup Base Model
      var model = new Base {
        Sdk = 2,
        Version = int.Parse(version.Replace(".", "")),
        Name = _options.Value.PluginName,
        Id = _options.Value.PluginName
      };

      // Add Configuration
      // Add Plug Start Comand
      model.Plugin_start_cmd = Path.Combine("%TP_PLUGIN_FOLDER%", "MSFS-TouchPortal-Plugin\\dist", "MSFSTouchPortalPlugin.exe");
      // Load asembly
      _ = MSFSTouchPortalPlugin.Objects.AutoPilot.AutoPilot.AP_AIRSPEED_HOLD;

      var q = assembly.GetTypes().ToList();

      // Get all classes with the TouchPortalCategory
      var s = q.Where(t => t.CustomAttributes.Any(att => att.AttributeType == typeof(TouchPortalCategoryAttribute))).OrderBy(o => o.Name).ToList();

      // For each category, add to model
      s.ForEach(cat => {
        var att = (TouchPortalCategoryAttribute)Attribute.GetCustomAttribute(cat, typeof(TouchPortalCategoryAttribute));
        var category = new TouchPortalCategory {
          Id = $"{_options.Value.PluginName}.{att.Id}",
          Name = att.Name,
          // Imagepath = att.ImagePath
          Imagepath = Path.Combine("%TP_PLUGIN_FOLDER%", "MSFS-TouchPortal-Plugin", "airplane_takeoff24.png")
        };

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
          };

          // Has Choices
          var choiceAttributes = act.GetCustomAttributes<TouchPortalActionChoiceAttribute>()?.ToList();

          if (choiceAttributes?.Count > 0) {
            for (int i = 0; i < choiceAttributes.Count; i++) {
              var data = new TouchPortalActionData {
                Id = $"{action.Id}.Data.{i}",
                Type = "choice",
                Label = "Action",
                DefaultValue = choiceAttributes[i].DefaultValue,
                ValueChoices = choiceAttributes[i].ChoiceValues
              };

              action.Data.Add(data);
            }
            action.Format = string.Format(action.Format, action.Data.Select(d => $"{{${d.Id}$}}").ToArray());
          }

          category.Actions.Add(action);
        });

        // Ordering
        category.Actions = category.Actions.OrderBy(c => c.Name).ToList();

        // TODO: Non-Choice Types

        // States
        var states = cat.GetMembers().Where(t => t.CustomAttributes.Any(att => att.AttributeType == typeof(TouchPortalStateAttribute))).ToList();
        states.ForEach(state => {
          var stateAttribute = state.GetCustomAttribute<TouchPortalStateAttribute>();

          if (stateAttribute != null) {
            var newState = new TouchPortalState {
              Id = $"{category.Id}.State.{stateAttribute.Id}",
              Type = stateAttribute.Type,
              Description = $"{category.Name} - {stateAttribute.Description}",
              DefaultValue = stateAttribute.Default
            };

            category.States.Add(newState);
          }
        });

        // Ordering
        category.States = category.States.OrderBy(c => c.Description).ToList();

        // TODO: Add events

        model.Categories.Add(category);
      });

      // Ordering
      model.Categories = model.Categories.OrderBy(c => c.Name).ToList();

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
