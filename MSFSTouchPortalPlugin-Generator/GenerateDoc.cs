/*
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

    public void Generate()
    {
      var result = CreateMarkdown();

      // Save
      var dest = Path.Combine(_options.OutputPath, _options.DocFilename);
      File.WriteAllText(dest, result);
      _logger.LogInformation($"Generated '{dest}'.");
    }

    void CreateActionDataMd(StringBuilder s, TouchPortalActionBaseAttribute act, bool withMappings)
    {
      var fieldStyle = "color:SelectedItemText; background-color:SelectedItem; padding: 0 5px;";

      s.Append($"<tr valign='top'><td>{act.Name}</td><td>{act.Description}</td>");
      s.Append("<td>\n");
      List<string> dataArray = new();
      foreach (var ad in act.Data) {
        if (ad.Id != null && (ad.Id.StartsWith("OnHold") /*|| ad.Id.StartsWith("Fb")*/))
          continue;

        var defaultVal = ad.GetDefaultValue()?.ToString();
        int prec = ad.AllowDecimals ? 2 : 0;

        StringBuilder sb = new(" ");
        if (ad.ValueType == DataType.Choice) {
          sb.Append($"<ul type='none' style='{fieldStyle} display:inline-table;'>");
          foreach (var choice in ad.ChoiceValues)
            sb.Append($"<li style='padding: 0 5px; font-weight: {(choice == defaultVal ? "bold" : "normal")}'>{choice}</li>");
          sb.Append("</ul> &nbsp;");
        }
        else if (ad.ValueType == DataType.Number || (ad.ValueType == DataType.Text && (!double.IsNaN(ad.MinValue) || !double.IsNaN(ad.MaxValue)))) {
          sb.Append($"<span style='{fieldStyle}'>[number");
          double min = double.NaN, max = double.NaN;
          if (!double.IsNaN(ad.MinValue) && ad.MinValue != int.MinValue && ad.MinValue != double.MinValue && ad.MinValue != float.MinValue)
            min = ad.MinValue;
          if (!double.IsNaN(ad.MaxValue) && ad.MaxValue != int.MaxValue && ad.MaxValue != uint.MaxValue && ad.MaxValue != double.MaxValue && ad.MaxValue != float.MaxValue)
            max = ad.MaxValue;

          if (!double.IsNaN(min) || !double.IsNaN(max)) {
            sb.Append(" (<ruby>");
            if (!double.IsNaN(min))
              sb.Append($"{min.ToString($"F{prec}")}<rp>(</rp><rt style='font-size:75%'>min</rt><rp>)</rp>");
            if (!double.IsNaN(min) && !double.IsNaN(max))
              sb.Append(" &nbsp;");
            if (!double.IsNaN(max))
              sb.Append($"{max.ToString($"F{prec}")}<rp>(</rp><rt style='font-size:75%'>max</rt><rp>)</rp>");
            sb.Append("</ruby>)");
          }
          sb.Append("]</span>");
        }
        else if (ad.ValueType == DataType.Switch) {
          sb.Append($"<span style='{fieldStyle}'>On/Off</span>");
        }
        else {
          sb.Append($"<span style='{fieldStyle}'>[text]</span>");
        }
        sb.Append(" ");
        dataArray.Add(sb.ToString());
      }

      try {
        if (act.GetType() == typeof(TouchPortalConnectorAttribute))
          act.Format = new Regex(@"\| Feedback.*$", RegexOptions.Singleline).Replace(act.Format, "");
        act.Format = new Regex(@"\([-\+\d\s]+[\s\w]*([-\+\d\s]+)?\)").Replace(act.Format, "");
        if (dataArray.Count > 0)
          s.Append(string.Format(act.Format, dataArray.ToArray()));
        else
          s.Append(act.Format);
        }
      catch (Exception e) {
        _logger.LogError("Failed to format '{Format}' for {ActionId} with data ({Count})\n{@Data}\n{Error}", act.Format, act.Id, dataArray.Count, dataArray, e);
      }
      s.Append("</td>\n");

      // mappings (only for SimConnect events)
      if (withMappings) {
        s.Append("<td>");

        //if (act.Mappings.Length > 2)  // collapsible only if more than 2 items
          //s.Append("<details><summary><sub>details</sub></summary>");
        s.Append("<dl>");
        foreach (var am in act.Mappings) {
          if (am.Values?.Length > 0)
            s.Append($"<dt>{string.Join(" + ", am.Values)}</dt>");
          s.Append($"<dd>{am.ActionId}</dd>");
        }
        s.Append($"</dl>");
        //if (act.Mappings.Length > 2)
          //s.Append($"</details>");
        s.Append("</td>\n");
      }
    }

    private string CreateMarkdown() {

      // set plugin file location for version info
      var assembly = Assembly.GetExecutingAssembly();
      VersionInfo.AssemblyLocation = Path.Combine(Path.GetDirectoryName(assembly.Location), _options.PluginId + ".dll");

      // read states config
      // always include the internal plugin states
      IEnumerable<SimVarItem> simVars = _pluginConfig.LoadPluginStates();
      if (_options.StateFiles.Any()) {
        _pluginConfig.UserConfigFolder = _options.StateFilesPath;
        _pluginConfig.UserStateFiles = string.Join(',', _options.StateFiles);
      }
      simVars = simVars.Concat(_pluginConfig.LoadSimVarStateConfigs());

      Dictionary<Groups, IEnumerable<TouchPortalActionAttribute>> catActsMap = new();
      Dictionary<Groups, IEnumerable<TouchPortalConnectorAttribute>> catConnMap = new();
      Dictionary<Groups, IEnumerable<MSFSTouchPortalPlugin.Types.TouchPortalEvent>> catEventsMap = new();
      Dictionary<Groups, IEnumerable<SimVarItem>> catStatesMap = new();

      var s = new StringBuilder(85 * 1024);

      s.Append($"# {_options.PluginName + " Documentation"}\n\n");
      s.Append(GeneratorConstants.INTRO_TEXT + "\n\n");
      if (!string.IsNullOrWhiteSpace(_options.DocumentationUrl))
        s.Append("For further documentation, please see ").Append(_options.DocumentationUrl).Append("\n\n");
      s.Append($"This documentation generated for plugin v{VersionInfo.GetProductVersionString()}\n\n");
      s.Append("---\n\n");

      // Get all categories
      var categegoryAttribs = _reflectionSvc.GetCategoryAttributes();

      // Table of Contents
      s.Append("## Table of Contents\n\n");
      s.Append("[Plugin Settings](#plugin-settings)\n\n");
      s.Append("[Actions and States by Category](#actions-and-states-by-category)\n\n");

      foreach (var cat in categegoryAttribs) {
        var actList = cat.Actions.Where(a => !a.Deprecated);
        var connList = cat.Connectors.Where(a => !a.Deprecated);
        var evList = _reflectionSvc.GetEvents(cat.Id).OrderBy(c => c.Name);
        var stateList = simVars.Where(s => s.CategoryId == cat.Id).OrderBy(c => c.Name);
        if (!actList.Any() && !connList.Any() && !evList.Any() && !stateList.Any())
          continue;

        s.Append($"* [{cat.Name}](#{cat.Name.Replace(" ", "-").Replace("&", "").ToLower()})\n\n");

        catActsMap.Add(cat.Id, actList);
        catConnMap.Add(cat.Id, connList);
        catEventsMap.Add(cat.Id, evList);
        catStatesMap.Add(cat.Id, stateList);
      }

      s.Append("---\n\n");

      // Show settings first
      s.Append("## Plugin Settings\n<details><summary><sub>Click to expand</sub></summary>\n\n");
      var settings = _reflectionSvc.GetSettings().Values;
      foreach (var setting in settings) {
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
        var settingType = setting.ValueType == DataType.Switch ? "boolean (on/off)" : setting.TouchPortalType;
        s.Append($"| {settingType} | {setting.Default}");
        if (f[0]) s.Append(" | ").Append(setting.MaxLength);
        if (f[1]) s.Append(" | ").Append(setting.MinValue);
        if (f[2]) s.Append(" | ").Append(setting.MaxValue);
        s.Append(" |\n\n");
        s.Append(setting.Description + "\n\n");
      }
      s.Append("</details>\n\n---\n\n");

      s.Append("## Actions and States by Category\n\n");

      // Loop Categories
      foreach (var cat in categegoryAttribs) {
        if (!catActsMap.ContainsKey(cat.Id))
          continue;
        var isInternal = Categories.InternalActionCategories.Contains(cat.Id);
        var catLink = cat.Id.ToString().ToLower();
        var hasActions = catActsMap.TryGetValue(cat.Id, out var catActions) && catActions.Any();
        var hasConnectors = catConnMap.TryGetValue(cat.Id, out var catConn) && catConn.Any();
        var hasStates = catStatesMap.TryGetValue(cat.Id, out var categoryStates) && categoryStates.Any();
        var hasEvents = catEventsMap.TryGetValue(cat.Id, out var catEvents) && catEvents.Any();

        s.Append($"### {cat.Name}\n<details><summary><sub>Click to expand</sub></summary>\n\n");
        List<string> links = new();
        if (hasActions)
          links.Add($"[Actions](#{catLink}-actions)");
        if (hasConnectors)
          links.Add($"[Connectors](#{catLink}-connectors)");
        if (hasStates)
          links.Add($"[States](#{catLink}-states)");
        if (hasEvents)
          links.Add($"[Events](#{catLink}-events)");
        s.Append(string.Join(" | ", links) + "\n\n");

        // Loop Actions
        if (hasActions) {
          s.Append($"<a id='{catLink}-actions'></a>\n\n");
          s.Append($"#### Actions\n");
          s.Append("<table>\n");   // use HTML table for row valign attribute
          s.Append(
            "<tr valign='bottom'>" +
            "<th>Name</th>" +
            "<th>Description</th>" +
            "<th>Format</th>" +
            (!isInternal ? "<th>Sim Event Mappings</th>" : "") +
            "<th>On<br/>Hold</sub></div></th>" +
            "</tr>\n"
          );
          if (!isInternal)
            catActions = catActions.OrderBy(c => c.Name);
          foreach (var act in catActions) {
            if (act.Deprecated)
              continue;

            // Loop action data
            CreateActionDataMd(s, act, !isInternal);

            // has hold
            s.Append($"<td align='center'>{(act.HasHoldFunctionality ? "&#9745;" : "")}</td></tr>\n");  // U+2611 Ballot Box with Check Emoji
          }
          s.Append("</table>\n\n\n");
        }

        // Loop Connectors
        if (hasConnectors) {
          s.Append($"<a id='{catLink}-connectors'></a>\n\n");
          s.Append($"#### Connectors\n");
          s.Append("<table>\n");   // use HTML table for row valign attribute
          s.Append(
            "<tr valign='bottom'>" +
            "<th>Name</th>" +
            "<th>Description</th>" +
            "<th>Format</th>" +
            (!isInternal ? "<th>Sim Event Mappings</th>" : "") +
            "</tr>\n"
          );
          if (!isInternal)
            catConn = catConn.OrderBy(c => c.Name);
          foreach (var act in catConn) {
            // Loop action data
            CreateActionDataMd(s, act, !isInternal);
          }
          s.Append("</table>\n\n\n");
        }

        if (hasStates) {
          s.Append($"<a id='{catLink}-states'></a>\n\n");
          s.Append($"#### States\n");
          s.Append($" **Base Id:** {_options.PluginId}.{cat.Id}.State.\n\n");
          s.Append("| Id | SimVar Name | Description | Unit | Format | DefaultValue |\n");
          s.Append("| --- | --- | --- | --- | --- | --- |\n");
          foreach (SimVarItem state in categoryStates) {
            s.Append($"| {state.TouchPortalStateId.Split('.')[^1]} | {state.SimVarName} | {state.Name} | {state.Unit} | {state.FormattingString} | {state.DefaultValue ?? ""} |\n");
          }
          s.Append("\n\n");
        }

        if (hasEvents) {
          // Loop Events
          s.Append($"<a id='{catLink}-events'></a>\n\n");
          s.Append($"#### Events\n\n");
          s.Append("<table>\n");   // use HTML table for row valign attribute
          s.Append("<tr valign='bottom'>" +
            "<th>Name</th>" +
            "<th>Id</th>" +
            "<th nowrap>Evaluated State Id</th>" +
            "<th>Format</th>" +
            "</tr>\n");
          foreach (var ev in catEvents) {
            string evVal =" ";

            if (ev.ValueChoices != null & ev.ValueChoices.Length > 0) {
              evVal += "&nbsp;<details style='display:inline;'><summary>[ select ]</summary>\n<ul>";
              var choiceMappings = (Dictionary<string, string>)ev.GetEventDescriptions();
              if (choiceMappings == null || !choiceMappings.Any())
                evVal += string.Join("\n", ev.ValueChoices.Select(v => $"<li>{v}</li>"));
              else
                evVal += string.Join("\n", choiceMappings.Select(m => $"<li><b>{m.Key}</b> - {m.Value}</li>"));
              evVal += "</ul></details>";
            }

            s.Append($"<tr valign='top'>" +
              $"<td>{ev.Name}</td>" +
              $"<td>{cat.Id}.Event.{ev.Id}</td>" +
              $"<td>{ev.ValueStateId}</td>" +
              $"<td>{ev.Format.Replace("$val", evVal)}</td>"
            );
            s.Append("</tr>\n");
          }
          s.Append("</table>\n\n\n");
        }

        // Loop Events

        s.Append("</details>\n\n---\n\n");

      }  // end categories loop

      return s.ToString();
    }
  }
}
