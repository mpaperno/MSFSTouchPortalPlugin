using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Configuration;
using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Enums;
using MSFSTouchPortalPlugin.Helpers;
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

namespace MSFSTouchPortalPlugin_Generator
{
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

      // Save
      var dest = Path.Combine(_options.Value.TargetPath, "DOCUMENTATION.md");
      File.WriteAllText(dest, result);
      _logger.LogInformation($"Generated '{dest}'.");
    }

    private DocBase CreateModel() {
      // Find assembly
      var a = Assembly.GetExecutingAssembly().GetReferencedAssemblies().FirstOrDefault(a => a.Name == _options.Value.PluginName);

      if (a == null) {
        throw new FileNotFoundException("Unable to load assembly for reflection.");
      }

      var assembly = Assembly.Load(a);
      var assemblyList = assembly.GetTypes().ToList();

      VersionInfo.Assembly = assembly;

      var model = new DocBase {
        Title = "MSFS 2020 Touch Portal Plugin",
        Overview = "This plugin will provide a two-way interface between Touch Portal and Microsoft Flight Simulator 2020 through SimConnect.",
        Version = VersionInfo.GetProductVersionString()
      };

      // read default states config
      // TODO: Allow configuration of which state config file(s) to read.
      var pc = PluginConfig.Instance;
      var configStates = pc.LoadSimVarItems(false);
      if (pc.HaveErrors) {
        foreach (var e in pc.ErrorsList)
          _logger.LogError(e, "Configuration reader error:");
      }

      // Get all classes with the TouchPortalCategory
      var classList = assemblyList.Where(t => t.CustomAttributes.Any(att => att.AttributeType == typeof(TouchPortalCategoryAttribute))).OrderBy(o => o.Name);

      // Loop through categories
      foreach (var cat in classList) {
        var catAttr = (TouchPortalCategoryAttribute)Attribute.GetCustomAttribute(cat, typeof(TouchPortalCategoryAttribute));
        // FIXME: the Split() is necessary due to legacy mis-named category InstrumentsSystems.Fuel
        if (!Enum.TryParse(catAttr.Id.Split('.').Last(), false, out Groups catId)) {
          _logger.LogWarning($"Could not parse category ID: '{catAttr.Id}', skipping.'");
          continue;
        }
        var newCat = model.Categories.FirstOrDefault(c => c.Id == catId);
        if (newCat == null) {
          newCat = new DocCategory {
            Id = catId,
            Name = Categories.FullCategoryName(catId),
          };
          model.Categories.Add(newCat);
        }

        // Loop through Actions
        var actions = cat.GetMembers().Where(t => t.CustomAttributes.Any(att => att.AttributeType == typeof(TouchPortalActionAttribute))).ToList();
        actions.ForEach(act => {
          var actionAttribute = act.GetCustomAttribute<TouchPortalActionAttribute>();
          var newAct = new DocAction {
            Name = actionAttribute.Name,
            Description = actionAttribute.Description,
            Type = actionAttribute.Type,
            Format = actionAttribute.Format,
            HasHoldFunctionality = actionAttribute.HasHoldFunctionality
          };

          // Loop through Action Data
          var dataAttributes = act.GetCustomAttributes<TouchPortalActionDataAttribute>();
          foreach (var attrib in dataAttributes) {
            var data = new DocActionData {
              Type = attrib.Type,
              DefaultValue = attrib.GetDefaultValue()?.ToString(),
              Values = attrib.ChoiceValues != null ? string.Join(", ", attrib.ChoiceValues) : "",
              MinValue = attrib.MinValue,
              MaxValue = attrib.MaxValue,
              AllowDecimals = attrib.AllowDecimals
            };
            newAct.Data.Add(data);
          }

          // Loop through Action mappings
          var mapAttribs = act.GetCustomAttributes<MSFSTouchPortalPlugin.Attributes.TouchPortalActionMappingAttribute>();
          foreach (var attrib in mapAttribs) {
            var map = new DocActionMapping {
              ActionId = attrib.ActionId,
              Values = attrib.Values,
            };
            newAct.Mappings.Add(map);
          }
          // Warn about missing mappings
          if (!mapAttribs.Any())
            _logger.LogWarning($"No event mappings found for action ID '{actionAttribute.Id}' in category '{cat.Name}'");

          newCat.Actions.Add(newAct);
        });

        newCat.Actions = newCat.Actions.OrderBy(c => c.Name).ToList();

        // Loop through States
        if (newCat.States.Any())
          continue;  // skip if already added for this category

        System.Collections.Generic.IEnumerable<SimVarItem> categoryStates;
        // Plugin (non SimConnect) states are stored in a separate config file.
        if (catId == Groups.Plugin)
          categoryStates = pc.LoadPluginStates();
        else
          categoryStates = configStates.Where(s => s.CategoryId == catId);

        foreach (SimVarItem state in categoryStates) {
          var newState = new DocState {
            Id = state.TouchPortalStateId,
            Type = state.TouchPortalValueType,
            Description = state.Name,
            DefaultValue = state.DefaultValue ?? string.Empty,
            SimVarName = state.SimVarName,
            Unit = state.Unit,
            FormattingString = state.FormattingString
          };
          newCat.States.Add(newState);
        }

        // Sort the states
        newCat.States = newCat.States.OrderBy(c => c.Id).ToList();
      }  // categories loop

      // Settings
      var setContainers = assemblyList.Where(t => t.CustomAttributes.Any(att => att.AttributeType == typeof(TouchPortalSettingsContainerAttribute))).OrderBy(o => o.Name).ToList();
      setContainers.ForEach(setCtr => {
        var settingsList = setCtr.GetMembers().Where(t => t.CustomAttributes.Any(att => att.AttributeType == typeof(TouchPortalSettingAttribute))).ToList();
        settingsList.ForEach(setType => {
          var att = (TouchPortalSettingAttribute)Attribute.GetCustomAttribute(setType, typeof(TouchPortalSettingAttribute));
          var setting = new DocSetting {
            Name = att.Name,
            Description = att.Description,
            Type = att.Type,
            DefaultValue = att.Default,
            MaxLength = att.MaxLength,
            MinValue = att.MinValue,
            MaxValue = att.MaxValue,
            IsPassword = att.IsPassword,
            ReadOnly = att.ReadOnly
          };

          model.Settings.Add(setting);
        });
      });
      model.Settings = model.Settings.OrderBy(c => c.Name).ToList();

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
      model.Categories.ForEach(cat => {
        s.Append($"[{cat.Name}](#{cat.Name.Replace(" ", "-").ToLower()})\n\n");
      });
      s.Append("---\n\n");

      // Show settings first
      s.Append("## Plugin Settings\n<details><summary><sub>Click to expand</sub></summary>\n\n");
      model.Settings.ForEach(setting => {
        s.Append($"### {setting.Name}\n\n");
        s.Append("| Read-only | Type | Default Value | Max. Length | Min. Value | Max. Value |\n");
        s.Append("| --- | --- | --- | --- | --- | --- |\n");
        s.Append($"| {setting.ReadOnly} | {setting.Type} | {setting.DefaultValue} ");
        s.Append($"| {(setting.MaxLength > 0 ? setting.MaxLength : "N/A")} ");
        s.Append($"| {(double.IsNaN(setting.MinValue) ? "N/A" : setting.MinValue)} ");
        s.Append($"| {(double.IsNaN(setting.MaxValue) ? "N/A" : setting.MaxValue)} ");
        s.Append("|\n\n");
        s.Append(setting.Description + "\n\n");
      });
      s.Append("</details>\n\n---\n\n");

      // Loop Categories
      model.Categories.ForEach(cat => {
        s.Append($"## {cat.Name}\n<details><summary><sub>Click to expand</sub></summary>\n\n");

        // Loop Actions
        if (cat.Actions.Count > 0) {
          s.Append("### Actions\n\n");
          s.Append("<table>\n");   // use HTML table for row valign attribute
          s.Append("<tr valign='bottom'><th>Name</th><th>Description</th><th>Format</th>" +
            "<th nowrap>Data<br/><div align=left><sub>index. &nbsp; [type] &nbsp; &nbsp; choices/default (in bold)</th>" +
            "<th>Sim Event(s)</th><th>On<br/>Hold</sub></div></th></tr>\n");
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
            s.Append($"</ol></td>\n<td>");
            if (act.Mappings.Count > 2)  // collapsible only if more than 2 items
              s.Append($"<details><summary><sub>details</sub></summary>");
            s.Append("<dl>");
            act.Mappings.ForEach(am => {
              if (am.Values?.Length > 0)
                s.Append($"<dt>{string.Join("+", am.Values)}</dt>");
              s.Append($"<dd>{am.ActionId}</dd>");
            });
            s.Append($"</dl>");
            if (act.Mappings.Count > 2)
              s.Append($"</details>");
            s.Append($"</td>\n<td align='center'>{(act.HasHoldFunctionality ? "&#9745;" : "")}</td></tr>\n");  // U+2611 Ballot Box with Check Emoji
          });
          s.Append("</table>\n\n\n");
        }

        if (cat.States.Count > 0) {
          // Loop States
          s.Append("### States\n\n");
          s.Append("| Id | SimVar Name | Description | Unit | Format | DefaultValue |\n");
          s.Append("| --- | --- | --- | --- | --- | --- |\n");
          cat.States.ForEach(state => {
            s.Append($"| {state.Id} | {state.SimVarName} | {state.Description} | {state.Unit} | {state.FormattingString} | {state.DefaultValue} |\n");
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
