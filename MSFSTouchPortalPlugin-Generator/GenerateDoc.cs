using Microsoft.Extensions.Logging;
using MSFSTouchPortalPlugin.Configuration;
using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Enums;
using MSFSTouchPortalPlugin.Helpers;
using MSFSTouchPortalPlugin.Interfaces;
using MSFSTouchPortalPlugin.Types;
using MSFSTouchPortalPlugin_Generator.Configuration;
using MSFSTouchPortalPlugin_Generator.Interfaces;
using MSFSTouchPortalPlugin_Generator.Model;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Text;
using System.Collections.Generic;

namespace MSFSTouchPortalPlugin_Generator
{
  internal class GenerateDoc : IGenerateDoc {
    private readonly ILogger<GenerateDoc> _logger;
    private readonly GeneratorOptions _options;
    private readonly IReflectionService _reflectionSvc;
    private readonly PluginConfig _pluginConfig;

    public GenerateDoc(ILogger<GenerateDoc> logger, GeneratorOptions options, IReflectionService reflectionSvc, PluginConfig pluginConfig) {
      _logger = logger;
      _options = options;
      _reflectionSvc = reflectionSvc ?? throw new ArgumentNullException(nameof(reflectionSvc));
      _pluginConfig = pluginConfig ?? throw new ArgumentNullException(nameof(pluginConfig));
    }

    public void Generate() {
      // Create reflection model
      var model = CreateModel();
      if (model == null)
        return;

      // Create Markdown
      var result = CreateMarkdown(model);

      // Save
      var dest = Path.Combine(_options.OutputPath, "DOCUMENTATION.md");
      File.WriteAllText(dest, result);
      _logger.LogInformation($"Generated '{dest}'.");
    }

    private DocBase CreateModel() {

      // set plugin file location for version info
      VersionInfo.AssemblyLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), _options.PluginId + ".dll");

      // create the base model
      var model = new DocBase {
        Title = _options.PluginName + " Documentation",
        Overview = "This plugin will provide a two-way interface between Touch Portal and Flight Simulators which use SimConnect, such as Microsoft Flight Simulator 2020 and FS-X.",
        Version = VersionInfo.GetProductVersionString()
      };

      // read states config
      // always include the internal plugin states
      IEnumerable<SimVarItem> simVars = _pluginConfig.LoadPluginStates();
      if (_options.StateFiles.Any()) {
        PluginConfig.UserConfigFolder = _options.StateFilesPath;
        PluginConfig.UserStateFiles = string.Join(',', _options.StateFiles);
        model.Version += "<br/>" +
          $"Custom configuration version {_options.ConfigVersion}<br/>" +
          $"Custom State Definitions Source(s): {string.Join(", ", _options.StateFiles)}";
      }
      simVars = simVars.Concat(_pluginConfig.LoadSimVarStateConfigs());

      var categegoryAttribs = _reflectionSvc.GetCategoryAttributes();
      foreach (var catAttrib in categegoryAttribs) {

        var category = new DocCategory {
          CategoryId = catAttrib.Id,
          Id = $"{_options.PluginId}.{catAttrib.Id}",
          Name = catAttrib.Name
        };
        model.Categories.Add(category);

        // workaround for backwards compat with mis-named actions in category InstrumentsSystems.Fuel
        string actionCatId = _options.PluginId + "." + Categories.ActionCategoryId(catAttrib.Id);

        // Add Actions
        foreach (var actionAttrib in catAttrib.Actions) {
          var action = new DocAction {
            Name = actionAttrib.Name,
            Description = actionAttrib.Description,
            Type = actionAttrib.Type,
            Format = actionAttrib.Format,
            HasHoldFunctionality = actionAttrib.HasHoldFunctionality,
          };
          category.Actions.Add(action);

          // Action data
          foreach (var attrib in actionAttrib.Data) {
            var data = new DocActionData {
              Type = attrib.Type,
              DefaultValue = attrib.GetDefaultValue()?.ToString(),
              Values = attrib.ChoiceValues != null ? string.Join(", ", attrib.ChoiceValues) : "",
              MinValue = attrib.MinValue,
              MaxValue = attrib.MaxValue,
              AllowDecimals = attrib.AllowDecimals,
            };
            action.Data.Add(data);
          }

          // Action data mappings, but Plugin category doesn't show them anyway
          if (catAttrib.Id == Groups.Plugin)
            continue;

          // Warn about missing mappings
          if (!actionAttrib.Mappings.Any()) {
            _logger.LogWarning($"No event mappings found for action ID '{actionAttrib.Id}' in category '{category.Name}'");
            continue;
          }
          foreach (var attrib in actionAttrib.Mappings) {
            var map = new DocActionMapping {
              ActionId = attrib.ActionId,
              Values = attrib.Values,
            };
            action.Mappings.Add(map);
          }
        }  // actions

        // Add States
        var categoryStates = simVars.Where(s => s.CategoryId == catAttrib.Id);
        foreach (SimVarItem state in categoryStates) {
          var newState = new DocState {
            Id = state.TouchPortalStateId.Split('.').Last(),
            Type = state.TouchPortalValueType,
            Description = state.Name,
            DefaultValue = state.DefaultValue ?? string.Empty,
            SimVarName = state.SimVarName,
            Unit = state.Unit,
            FormattingString = state.FormattingString,
            CanSet = state.CanSet,
          };
          category.States.Add(newState);
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
        var setting = new DocSetting {
          Name = s.Name,
          Description = s.Description,
          Type = s.TouchPortalType,
          DefaultValue = s.Default,
          MaxLength = s.MaxLength,
          MinValue = s.MinValue,
          MaxValue = s.MaxValue,
          IsPassword = s.IsPassword,
          ReadOnly = s.ReadOnly
        };
        model.Settings.Add(setting);
      }

      return model;
    }

    private static string CreateMarkdown(DocBase model) {
      var s = new StringBuilder();

      s.Append($"# {model.Title}\n\n");
      s.Append($"{model.Overview}\n\n");
      s.Append($"Docuemntation generated for plugin version {model.Version}\n\n");
      s.Append("---\n\n");

      // Table of Contents
      s.Append("## Table of Contents\n\n");
      s.Append("[Plugin Settings](#plugin-settings)\n\n");
      s.Append("[Actions and States by Category](#actions-and-states-by-category)\n\n");
      model.Categories.ForEach(cat => {
        s.Append($"* [{cat.Name}](#{cat.Name.Replace(" ", "-").ToLower()})\n\n");
      });
      s.Append("---\n\n");

      // Show settings first
      s.Append("## Plugin Settings\n<details><summary><sub>Click to expand</sub></summary>\n\n");
      model.Settings.ForEach(setting => {
        bool[] f = new[] { setting.MaxLength > 0, !double.IsNaN(setting.MinValue), !double.IsNaN(setting.MaxValue) };
        s.Append($"### {setting.Name}\n\n");
        s.Append("| Read-only | Type | Default Value");
        if (f[0]) s.Append(" | Max. Length");
        if (f[1]) s.Append(" | Min. Value");
        if (f[2]) s.Append(" | Max. Value");
        s.Append(" |\n");
        s.Append("| --- | --- | ---");
        for (var i = 0; i < 3; ++i)
          if (f[i]) s.Append(" | ---");
        s.Append(" |\n");
        s.Append($"| {setting.ReadOnly} | {setting.Type} | {setting.DefaultValue}");
        if (f[0]) s.Append(" | ").Append(setting.MaxLength);
        if (f[1]) s.Append(" | ").Append(setting.MinValue);
        if (f[2]) s.Append(" | ").Append(setting.MaxValue);
        s.Append(" |\n\n");
        s.Append(setting.Description + "\n\n");
      });
      s.Append("</details>\n\n---\n\n");

      s.Append("## Actions and States by Category\n\n");

      // Loop Categories
      model.Categories.ForEach(cat => {
        s.Append($"### {cat.Name}\n<details><summary><sub>Click to expand</sub></summary>\n\n");

        // Loop Actions
        if (cat.Actions.Count > 0) {
          s.Append("#### Actions\n\n");
          s.Append("<table>\n");   // use HTML table for row valign attribute
          s.Append("<tr valign='bottom'>" +
            "<th>Name</th>" +
            "<th>Description</th>" +
            "<th>Format</th>" +
            "<th nowrap>Data<br/><div align=left><sub>index. &nbsp; [type] &nbsp; &nbsp; choices/default (in bold)</th>" +
            (cat.CategoryId != Groups.Plugin ? "<th>Sim Event(s)</th>" : "") +
            "<th>On<br/>Hold</sub></div></th>" +
            "</tr>\n");
          cat.Actions.ForEach(act => {
            s.Append($"<tr valign='top'><td>{act.Name}</td><td>{act.Description}</td><td>{act.Format}</td>");
            // Loop action data
            // I first tried by making a nested table to list the data, but it looked like ass on GitHub due to their CSS which forces a table width and (as of Feb '22). -MP
            s.Append("<td><ol start=0>\n");
            act.Data.ForEach(ad => {
              s.Append($"<li>[{ad.Type}] &nbsp; ");
              if (ad.Type == "choice")
                s.Append(new Regex(Regex.Escape(ad.DefaultValue)).Replace(ad.Values, $"<b>{ad.DefaultValue}</b>", 1));  // only replace 1st occurrence of default string
              else if (!string.IsNullOrWhiteSpace(ad.DefaultValue))
                s.Append($"<b>{ad.DefaultValue}</b>");
              else
                s.Append("&lt;empty&gt;");
              int prec = ad.AllowDecimals ? 2 : 0;
              if (!double.IsNaN(ad.MinValue))
                s.Append($" &nbsp; <sub>&lt;min: {ad.MinValue.ToString($"F{prec}")}&gt;</sub>");
              if (!double.IsNaN(ad.MaxValue))
                s.Append($" <sub>&lt;max: {ad.MaxValue.ToString($"F{prec}")}&gt;</sub>");  // seriously... printf("%.*f", prec, val) anyone?
              s.Append("</li>\n");
            });
            s.Append("</ol></td>\n");
            // mappings (only for SimConnect events)
            if (cat.CategoryId != Groups.Plugin) {
              s.Append("<td>");
              if (act.Mappings.Count > 2)  // collapsible only if more than 2 items
                s.Append("<details><summary><sub>details</sub></summary>");
              s.Append("<dl>");
              act.Mappings.ForEach(am => {
                if (am.Values?.Length > 0)
                  s.Append($"<dt>{string.Join("+", am.Values)}</dt>");
                s.Append($"<dd>{am.ActionId}</dd>");
              });
              s.Append($"</dl>");
              if (act.Mappings.Count > 2)
                s.Append($"</details>");
              s.Append("</td>\n");
            }
            // has hold
            s.Append($"<td align='center'>{(act.HasHoldFunctionality ? "&#9745;" : "")}</td></tr>\n");  // U+2611 Ballot Box with Check Emoji
          });
          s.Append("</table>\n\n\n");
        }

        if (cat.States.Count > 0) {
          // Loop States
          s.Append("#### States\n\n");
          s.Append($" **Base Id:** {cat.Id}.State.\n\n");
          s.Append("| Id | SimVar Name | Description | Unit | Format | DefaultValue | Settable |\n");
          s.Append("| --- | --- | --- | --- | --- | --- | --- |\n");
          cat.States.ForEach(state => {
            s.Append($"| {state.Id} | {state.SimVarName} | {state.Description} | {state.Unit} | {state.FormattingString} | {state.DefaultValue}");
            s.Append($"| {(state.CanSet ? "&#9745;" : "")} |\n");  // U+2611 Ballot Box with Check Emoji
          });
          s.Append("\n\n");
        }

        // Loop Events

        s.Append("</details>\n\n---\n\n");
      });

      return s.ToString();
    }
  }
}
