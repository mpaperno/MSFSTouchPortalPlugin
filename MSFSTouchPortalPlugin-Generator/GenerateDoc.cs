﻿/*
This file is part of the MSFS Touch Portal Plugin project.
https://github.com/mpaperno/MSFSTouchPortalPlugin

COPYRIGHT:
(c) 2020 Tim Lewis;
(c) Maxim Paperno; All Rights Reserved.

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

    static void SerializeActionData(DocActionBase action, TouchPortalActionDataAttribute[] attribData)
    {
      foreach (var attrib in attribData) {
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
    }

    private DocBase CreateModel() {

      // set plugin file location for version info
      var assembly = Assembly.GetExecutingAssembly();
      VersionInfo.AssemblyLocation = Path.Combine(Path.GetDirectoryName(assembly.Location), _options.PluginId + ".dll");

      // create the base model
      var model = new DocBase {
        Title = _options.PluginName + " Documentation",
        Overview = "This plugin provides a two-way interface between Touch Portal and Flight Simulators which use SimConnect, such as Microsoft Flight Simulator 2020 and FS-X.",
        Version = VersionInfo.GetProductVersionString(),
        DocsUrl = _options.DocumentationUrl
      };

      // read states config
      // always include the internal plugin states
      IEnumerable<SimVarItem> simVars = _pluginConfig.LoadPluginStates();
      if (_options.StateFiles.Any()) {
        _pluginConfig.UserConfigFolder = _options.StateFilesPath;
        _pluginConfig.UserStateFiles = string.Join(',', _options.StateFiles);
      }
      simVars = simVars.Concat(_pluginConfig.LoadSimVarStateConfigs());

      var categegoryAttribs = _reflectionSvc.GetCategoryAttributes();
      foreach (var catAttrib in categegoryAttribs) {

        var category = new DocCategory {
          CategoryId = catAttrib.Id,
          Id = $"{_options.PluginId}.{catAttrib.Id}",
          Name = catAttrib.Name
        };

        // workaround for backwards compat with mis-named actions in category InstrumentsSystems.Fuel
        string actionCatId = _options.PluginId + "." + Categories.ActionCategoryId(catAttrib.Id);

        // Add Actions
        foreach (var actionAttrib in catAttrib.Actions) {
          if (actionAttrib.Deprecated)
            continue;
          var action = new DocAction {
            Name = actionAttrib.Name,
            Description = actionAttrib.Description,
            Type = actionAttrib.Type,
            Format = actionAttrib.Format,
            HasHoldFunctionality = actionAttrib.HasHoldFunctionality,
          };
          category.Actions.Add(action);

          // Action Data
          if (actionAttrib.Data.Any())
            SerializeActionData(action, actionAttrib.Data);

          // Action data mappings, but Plugin category doesn't show them anyway
          if (Categories.InternalActionCategories.Contains(catAttrib.Id))
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

        // Connectors
        foreach (var connAttrib in catAttrib.Connectors) {
          if (connAttrib.Deprecated)
            continue;
          var action = new DocConnector {
            Name = connAttrib.Name,
            Description = connAttrib.Description,
            Format = connAttrib.Format,
          };

          // Connector Data
          if (connAttrib.Data.Any())
            SerializeActionData(action, connAttrib.Data);
          category.Connectors.Add(action);
        }  // connectors

        // Add States
        var categoryStates = simVars.Where(s => s.CategoryId == catAttrib.Id);
        foreach (SimVarItem state in categoryStates) {
          var newState = new DocState {
            Id = state.TouchPortalStateId.Split('.')[^1],
            Type = state.TouchPortalValueType,
            Description = state.Name,
            DefaultValue = state.DefaultValue ?? string.Empty,
            SimVarName = state.SimVarName,
            Unit = state.Unit,
            FormattingString = state.FormattingString,
          };
          category.States.Add(newState);
        }

        // Events
        var catEvents = _reflectionSvc.GetEvents(catAttrib.Id, fullStateId: false);
        foreach (var ev in catEvents) {
          var tpEv = new DocEvent {
            Id = ev.Id,
            Name = ev.Name,
            Format = ev.Format.Replace("$val", "$" + ev.ValueType),
            ValueType = ev.ValueType,
            ValueChoices = ev.ValueChoices,
            ValueStateId = ev.ValueStateId.Remove(0, ev.ValueStateId.IndexOf('.') + 1),
            ChoiceMappings = (Dictionary<string, string>)ev.GetEventDescriptions(),
          };
          category.Events.Add(tpEv);
        }

        if (!category.Actions.Any() && !category.Connectors.Any() && !category.States.Any() && !category.Events.Any())
          continue;

        // Sort the actions and states for SimConnect groups
        if (!Categories.InternalActionCategories.Contains(catAttrib.Id)) {
          category.Actions = category.Actions.OrderBy(c => c.Name).ToList();
          category.Connectors = category.Connectors.OrderBy(c => c.Name).ToList();
          category.States = category.States.OrderBy(c => c.Description).ToList();
          category.Events = category.Events.OrderBy(c => c.Name).ToList();
        }

        model.Categories.Add(category);
      }  // categories loop

      // Settings
      var settings = _reflectionSvc.GetSettings().Values;
      foreach (var s in settings) {
        var setting = new DocSetting {
          Name = s.Name,
          Description = s.Description,
          Type = s.ValueType == DataType.Switch ? "boolean (on/off)" : s.TouchPortalType,
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

    static void CreateActionDataMd(StringBuilder s, DocActionBase act)
    {
      s.Append($"<tr valign='top'><td>{act.Name}</td><td>{act.Description}</td><td>{act.Format}</td>");
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
    }

    private string CreateMarkdown(DocBase model) {
      var s = new StringBuilder(85 * 1024);

      s.Append($"# {model.Title}\n\n");
      s.Append($"{model.Overview}\n\n");
      if (!string.IsNullOrWhiteSpace(model.DocsUrl))
        s.Append("For further documentation, please see ").Append(model.DocsUrl).Append("\n\n");
      s.Append($"This documentation generated for plugin v{model.Version}\n\n");
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
        //s.Append("| Read-only ");
        s.Append("| Type | Default Value");
        if (f[0]) s.Append(" | Max. Length");
        if (f[1]) s.Append(" | Min. Value");
        if (f[2]) s.Append(" | Max. Value");
        s.Append(" |\n");
        //s.Append("| --- ");  // Read-only
        s.Append("| --- | ---");
        for (var i = 0; i < 3; ++i)
          if (f[i]) s.Append(" | ---");
        s.Append(" |\n");
        //s.Append($"| {setting.ReadOnly} ");
        s.Append($"| {setting.Type} | {setting.DefaultValue}");
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
        if (cat.Actions.Any()) {
          s.Append("#### Actions\n\n");
          s.Append("<table>\n");   // use HTML table for row valign attribute
          s.Append("<tr valign='bottom'>" +
            "<th>Name</th>" +
            "<th>Description</th>" +
            "<th>Format</th>" +
            "<th nowrap>Data<br/><div align=left><sub>index. &nbsp; [type] &nbsp; &nbsp; choices/default (in bold)</th>" +
            (!Categories.InternalActionCategories.Contains(cat.CategoryId) ? "<th>Sim Event(s)</th>" : "") +
            "<th>On<br/>Hold</sub></div></th>" +
            "</tr>\n");
          cat.Actions.ForEach(act => {
            // Loop action data
            CreateActionDataMd(s, act);
            // mappings (only for SimConnect events)
            if (!Categories.InternalActionCategories.Contains(cat.CategoryId)) {
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

        // Loop Connectors
        if (cat.Connectors.Any()) {
          s.Append("#### Connectors\n\n");
          s.Append("<table>\n");   // use HTML table for row valign attribute
          s.Append("<tr valign='bottom'>" +
            "<th>Name</th>" +
            "<th>Description</th>" +
            "<th>Format</th>" +
            "<th nowrap>Data<br/><div align=left><sub>index. &nbsp; [type] &nbsp; &nbsp; choices/default (in bold)</th>" +
            "</tr>\n");
          cat.Connectors.ForEach(act => {
            // Loop action data
            CreateActionDataMd(s, act);
          });
          s.Append("</table>\n\n\n");
        }

        if (cat.Events.Any()) {
          // Loop States
          s.Append("#### Events\n\n");
          s.Append($" **Base Id:** {cat.Id}.Event. &nbsp; &nbsp; **Base State Id** {cat.Id.Split('.')[0]}.\n\n");
          s.Append("<table>\n");   // use HTML table for row valign attribute
          s.Append("<tr valign='bottom'>" +
            "<th>Id</th>" +
            "<th>Name</th>" +
            "<th nowrap>Evaluated State Id</th>" +
            "<th>Format</th>" +
            "<th>Type</th>" +
            "<th>Choice(s)</th>" +
            "</tr>\n");
          cat.Events.ForEach(ev => {
            s.Append($"<tr valign='top'>" +
              $"<td>{ev.Id}</td><" +
              $"td>{ev.Name}</td>" +
              $"<td>{ev.ValueStateId}</td>" +
              $"<td>{ev.Format}</td>" +
              $"<td>{ev.ValueType}</td>");
            s.Append("<td>");
            if (ev.ChoiceMappings == null || !ev.ChoiceMappings.Any()) {
              s.AppendJoin(", ", ev.ValueChoices);
            }
            else {
              s.Append("<details><summary><sub>details</sub></summary>\n<ul>");
              s.AppendJoin('\n', ev.ChoiceMappings.Select(m => $"<li><b>{m.Key}</b> - {m.Value}</li>"));
              s.Append($"</ul></details>");
            }
            s.Append("</td>");
            s.Append("</tr>\n");
          });
          s.Append("</table>\n\n\n");
        }

        if (cat.States.Any()) {
          s.Append("#### States\n\n");
          s.Append($" **Base Id:** {cat.Id}.State.\n\n");
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
