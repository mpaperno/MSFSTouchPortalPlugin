using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MSFSTouchPortalPlugin_Generator.Configuration;
using MSFSTouchPortalPlugin_Generator.Interfaces;
using MSFSTouchPortalPlugin_Generator.Model;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin_Generator {
  internal class GenerateDoc : IGenerateDoc {
    private readonly ILogger<GenerateDoc> _logger;
    private readonly IOptions<GeneratorOptions> _options;

    public GenerateDoc(ILogger<GenerateDoc> logger, IOptions<GeneratorOptions> options) {
      _logger = logger;
      _options = options;
    }

    public void Generate() {
      // Create reflection model
      var model = CreateModel();

      // Create Markdown
      var result = CreateMarkdown(model);

      File.WriteAllText(Path.Combine(_options.Value.TargetPath, "DOCUMENTATION.md"), result);
      _logger.LogInformation("DOCUMENTATION.md generated.");
    }

    private DocBase CreateModel() {
      // Load asembly
      _ = MSFSTouchPortalPlugin.Objects.AutoPilot.AutoPilot.AP_AIRSPEED_HOLD;

      // Find assembly
      var a = Assembly.GetExecutingAssembly().GetReferencedAssemblies().FirstOrDefault(a => a.Name == _options.Value.PluginName);

      if (a == null) {
        throw new ArgumentNullException("Unable to load assembly for reflection.");
      }

      var model = new DocBase {
        Title = "MSFS 2020 TouchPortal Plugin",
        Overview = "This plugin will provide a two way interface between Touch Portal and Microsoft Flight Simulator 2020 through SimConnect."
      };

      var assembly = Assembly.Load(a);
      var assemblyList = assembly.GetTypes().ToList();

      // Get all classes with the TouchPortalCategory
      var classList = assemblyList.Where(t => t.CustomAttributes.Any(att => att.AttributeType == typeof(TouchPortalCategoryAttribute))).OrderBy(o => o.Name).ToList();

      // Loop through categories
      classList.ForEach(cat => {
        var catAttr = (TouchPortalCategoryAttribute)Attribute.GetCustomAttribute(cat, typeof(TouchPortalCategoryAttribute));
        var newCat = new DocCategory {
          Name = catAttr.Name
        };

        // Loop through Actions
        var actions = cat.GetMembers().Where(t => t.CustomAttributes.Any(att => att.AttributeType == typeof(TouchPortalActionAttribute))).ToList();
        actions.ForEach(act => {
          var actionAttribute = (TouchPortalActionAttribute)Attribute.GetCustomAttribute(act, typeof(TouchPortalActionAttribute));
          var newAct = new DocAction {
            Name = actionAttribute.Name,
            Description = actionAttribute.Description,
            Type = actionAttribute.Type,
            Format = actionAttribute.Format
          };

          // Loop through Action Data
          var choiceAttributes = act.GetCustomAttributes<TouchPortalActionChoiceAttribute>()?.ToList();

          if (choiceAttributes?.Count > 0) {
            for (int i = 0; i < choiceAttributes.Count; i++) {
              var data = new DocActionData {
                Type = "choice",
                DefaultValue = choiceAttributes[i].DefaultValue,
                Values = string.Join(",", choiceAttributes[i].ChoiceValues)
              };
              newAct.Data.Add(data);
            }
          }

          newCat.Actions.Add(newAct);
        });

        newCat.Actions = newCat.Actions.OrderBy(c => c.Name).ToList();

        // Loop through States
        var states = cat.GetFields().Where(m => m.CustomAttributes.Any(att => att.AttributeType == typeof(TouchPortalState))).ToList();
        states.ForEach(state => {
          var stateAttribute = state.GetCustomAttribute<TouchPortalStateAttribute>();

          if (stateAttribute != null) {
            var newState = new DocState {
              Id = $"{_options.Value.PluginName}.{catAttr.Id}.State.{stateAttribute.Id}",
              Type = stateAttribute.Type,
              Description = stateAttribute.Description,
              DefaultValue = stateAttribute.Default
            };

            newCat.States.Add(newState);
          }
        });

        newCat.States = newCat.States.OrderBy(c => c.Description).ToList();

        // Loop through Events
        // TODO: Need events

        model.Categories.Add(newCat);
      });

      model.Categories = model.Categories.OrderBy(c => c.Name).ToList();

      return model;
    }

    private string CreateMarkdown(DocBase model) {
      var s = new StringBuilder();

      s.Append($"# {model.Title}\n\n");
      s.Append($"{model.Overview}\n\n");
      s.Append("---\n\n");

      // Table of Contents
      s.Append("## Table of Contents\n\n");
      model.Categories.ForEach(cat => {
        s.Append($"[{cat.Name}](#{cat.Name.Replace(" ", "").ToLower()})\n\n");
      });
      s.Append("---\n\n");

      // Loop Categories
      model.Categories.ForEach(cat => {
        s.Append($"## {cat.Name}\n\n");

        // Loop Actions
        if (cat.Actions.Count > 0) {
          s.Append("### Actions\n\n");
          s.Append("| Name | Description | Type | Format | Data (Default in bold) |\n");
          s.Append("| --- | --- | --- | --- | --- |\n");
          cat.Actions.ForEach(act => {
            // TODO: Only supports showing a single line of data
            s.Append($"| {act.Name} | {act.Description} | {act.Type} | {act.Format} | {(act.Data.Count > 0 ? act.Data[0].Values.Replace(act.Data[0].DefaultValue, $"**{act.Data[0].DefaultValue}**") :  "")} |\n");
          });
          s.Append("\n\n");
        }

        if (cat.States.Count > 0) {
          // Loop States
          s.Append("### States\n\n");
          s.Append("| Id | Type | Description | DefaultValue |\n");
          s.Append("| --- | --- | --- | --- |\n");
          cat.States.ForEach(state => {
            s.Append($"| {state.Id} | {state.Type} | {state.Description} | {state.DefaultValue} |\n");
          });
          s.Append("\n\n");
        }

        // TODO: Loop Events

        s.Append("---\n\n");
      });

      return s.ToString();
    }
  }
}
