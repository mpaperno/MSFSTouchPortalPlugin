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
using MSFSTouchPortalPlugin_Generator.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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

    bool SerializeActionData(TouchPortalActionBase action, TouchPortalActionDataAttribute[] attribData)
    {
      action.Data ??= new();
      int i = 0;
      foreach (var attrib in attribData) {
        string dataId = (string.IsNullOrWhiteSpace(attrib.Id) ? i.ToString() : attrib.Id);
        var data = new TouchPortalActionData {
          Id = $"{action.Id}.Data.{dataId}",
          Type = attrib.Type,
          DefaultValue = attrib.GetDefaultValue(),
          ValueChoices = attrib.ChoiceValues,
          MinValue = attrib.MinValue,
          MaxValue = attrib.MaxValue,
          AllowDecimals = attrib.AllowDecimals,
          Label = (bool)_options.TPv3 ? attrib.Label : null,  // deprecated in TPv4 so force to null if excluding v3 support
          FieldLabel = attrib.Label,         // may be used later for formatting lines, not for TP
          FieldSuffix = attrib.LabelSuffix,  //  "
        };
        ++i;
        action.Data.Add(data);
      }
      var tmpArry = action.Data.Select(d => $"{{${d.Id}$}}").ToArray();
      try {
        action.Format = string.Format(action.Format, tmpArry);
        if ((bool)_options.TPv4 && action.GetType() == typeof(TouchPortalAction)) {
          // For actions we also want to format the "on hold" line, if there is one.
          var holdAct = (action as TouchPortalAction);
          if (holdAct.FormatOnHold != null)
            holdAct.FormatOnHold = string.Format(holdAct.FormatOnHold, tmpArry);
        }
      }
      catch (Exception e) {
        _logger.LogError("Failed to format '{Format}' for {ActionId} with data\n{@Data}\n{Error}", action.Format, action.Id, string.Join(',', tmpArry), e);
        return false;
      }
      return true;
    }

    public void Generate()
    {
      string basePath = $"%TP_PLUGIN_FOLDER%{_options.PluginFolder}/";
      string baseIconPath = basePath + "icons/";
      // Get version number
      VersionInfo.AssemblyLocation = Path.Combine(Path.GetDirectoryName(AppContext.BaseDirectory), _options.PluginId + ".dll");
      uint vNum = VersionInfo.GetProductVersionNumber();
      // TP version(s) to generate for
      bool tpv3 = (bool)_options.TPv3;
      bool tpv4 = (bool)_options.TPv4;

      _logger.LogInformation($"\nGenerating {PluginConfig.PLUGIN_NAME_PREFIX} edition entry.tp v{vNum:X} [for TPv3 ({tpv3}) | TPv4 ({tpv4})] to '{_options.OutputPath}'");

      // read the internal plugin states config
      IEnumerable<SimVarItem> simVars = _pluginConfig.LoadPluginStates();

      // Setup Base Model
      var model = new Base {
        Version = vNum,
        Name = _options.PluginName,
        Id = _options.PluginId,
        SettingsDescription =
          GeneratorConstants.INTRO_TEXT +
          " For full documentation, please see " + _options.DocumentationUrl +
          "\nPlugin Version: " + VersionInfo.GetProductVersionString(),
      };
      if (tpv3)
        model.Sdk = 6;
      if (tpv4)
        model.Api = 10;
      if (!_options.Debug)
        model.Plugin_start_cmd = $"\"{basePath}dist/{_options.PluginId}.exe\"";
      model.Configuration.ColorDark = "#" + _options.ColorDark.Trim('#');
      model.Configuration.ColorLight = "#" + _options.ColorLight.Trim('#');
      model.Configuration.ParentCategory = "games";

      // Get all categories
      var categegoryAttribs = _reflectionSvc.GetCategoryAttributes();
      TouchPortalCategory category = null;

      if (tpv4) {
        // Add the main plugin category
        category = new TouchPortalCategory {
          Id = _options.PluginId + ".Main",
          Name = PluginConfig.PLUGIN_NAME_PREFIX,
          Imagepath = baseIconPath + Categories.CategoryImage(Groups.None)
        };
        model.Categories.Add(category);

        // Add sub-categories to main one
        category.SubCategories = [];
        foreach (var catAttrib in categegoryAttribs) {
          var subCat = new TouchPortalSubCategory {
            Id = $"{_options.PluginId}.{catAttrib.Id}",
            Name = Categories.CategoryName(catAttrib.Id),
            Imagepath = baseIconPath + catAttrib.Imagepath
          };
          category.SubCategories.Add(subCat);
        }
      }

      // Loop over categories adding actions/connectors/events/states for each.
      // In TPv4 mode we add everything to the main category, specifying the sub-category ID for actions & connectors.
      // In TPv3-only mode we'll create new top-level categories in the base model.
      foreach (var catAttrib in categegoryAttribs) {

        var fullCatName = Categories.FullCategoryName(catAttrib.Id);
        string subCategoryId = null;
        if (tpv4) {
          // For TPv4 we'll assign a sub category ID to each action/connector/event.
          subCategoryId = $"{_options.PluginId}.{catAttrib.Id}";
        }
        else {
          // create separate top-level categories for TPv3 and add all the contents to that instead of the main one like for TPv4.
          category = new TouchPortalCategory {
            Id = $"{_options.PluginId}.{catAttrib.Id}",
            Name = fullCatName,
            Imagepath = baseIconPath + catAttrib.Imagepath
          };
          model.Categories.Add(category);
        }

        // workaround for backwards compat with mis-named actions in category InstrumentsSystems.Fuel
        string actionCatId = _options.PluginId + "." + Categories.ActionCategoryId(catAttrib.Id);

        // Sort the actions and connectors in each non-internal group (those are manually arranged)
        if (!Categories.InternalActionCategories.Contains(catAttrib.Id)) {
          catAttrib.Actions = catAttrib.Actions.OrderBy(c => c.Name).ToArray();
          catAttrib.Connectors = catAttrib.Connectors.OrderBy(c => c.Name).ToArray();
        }

        // Actions
        foreach (var actionAttrib in catAttrib.Actions) {
          if (actionAttrib.Deprecated)
            continue;
          var action = new TouchPortalAction {
            Id = $"{actionCatId}.Action.{actionAttrib.Id}",
            Name = actionAttrib.Name,
            Prefix = actionAttrib.Prefix,
            Type = actionAttrib.Type,
            Description = actionAttrib.Description,
            Format = actionAttrib.Format,
            FormatOnHold = tpv4 ? actionAttrib.OnHoldFormat : null,
            HasHoldFunctionality = actionAttrib.HasHoldFunctionality,
            SubCategoryId = subCategoryId,
          };

          // Action Data
          if (actionAttrib.Data.Any() && !SerializeActionData(action, actionAttrib.Data))
            continue;

          if (tpv4) {
            // action.lines object for TP api v7+
            action.Lines = new TouchPortalLinesCollection();
            // common description line for action and onhold lines
            List <TouchPortalLineObject> lineData = [];
            if (!string.IsNullOrEmpty(action.Description))
              lineData.Add(new TouchPortalLineObject(action.Description));

            var linesObj = new TouchPortalLinesObject([.. lineData]);
            action.Lines.Action = [ linesObj ];
            // Split up actions with many fields into multiple lines. Currently these are just the custom states editor actions.
            if (actionAttrib.LayoutAsForm) {
              foreach (var data in action.Data) {
                linesObj.Data.Add(new TouchPortalLineObject($"{data.FieldLabel} {{${data.Id}$}} {data.FieldSuffix ?? ""}"));
              }
              if (actionAttrib.FormLabelWidth > 0) {
                if (linesObj.Suggestions == null)
                  linesObj.Suggestions = new();
                linesObj.Suggestions.FirstLineItemLabelWidth = actionAttrib.FormLabelWidth;
              }
            }
            else {
              linesObj.Data.Add(new TouchPortalLineObject(action.Format));
            }
            //_logger.LogInformation("{isMatch} \n'{format}'\n{@lines}", Regex.IsMatch(action.Format, @"(.+\{\$[\w\.]+\$\}){6,}", RegexOptions.Singleline), action.Format, linesObj.Data);

            if (tpv4 && actionAttrib.HasHoldFunctionality && action.FormatOnHold != null) {
              // The presence of `lines.onhold` is supposed to enable "HasHoldFunctionality" in TP v4, and allow for multi-line formatting.
              // However as of TP v4.3b4 beta this doesn't work. But in case it does start working in a future version, we'll have this in place.
              // Split the "On Hold" format text on pipe char and add as two separate lines. The 2nd line will be the specific "on hold" data fields.
              List<TouchPortalLineObject> holdLines = [ ..lineData ];
              action.FormatOnHold.Split('|').ToList().ForEach(l => { holdLines.Add(new TouchPortalLineObject(l.Trim())); });
              action.Lines.Onhold = [ new TouchPortalLinesObject(holdLines) ];
            }

            if (!tpv3) {
              // remove some deprecated properties
              action.Description = null;
              action.Format = null;
              action.Prefix = null;
              // keep these for BC with initial TP v4 releases
              //action.FormatOnHold = null;
              //action.HasHoldFunctionality = false;
            }
          }

          // validate unique ID
          if (category.Actions.FirstOrDefault(a => a.Id == action.Id) == null)
            category.Actions.Add(action);
          else
            _logger.LogWarning($"Duplicate action ID found: '{action.Id}', skipping.'");

        }  // actions

        // Connectors
        foreach (var connAttrib in catAttrib.Connectors) {
          if (connAttrib.Deprecated)
            continue;
          var action = new TouchPortalConnector {
            Id = $"{actionCatId}.Conn.{connAttrib.Id}",
            Name = connAttrib.Name,
            Description = connAttrib.Description,
            Format = connAttrib.Format,
            SubCategoryId = subCategoryId,
          };

          // Connector Data
          if (connAttrib.Data.Any() && !SerializeActionData(action, connAttrib.Data))
            continue;

          // validate unique ID
          if (category.Connectors.FirstOrDefault(a => a.Id == action.Id) == null)
            category.Connectors.Add(action);
          else
            _logger.LogWarning($"Duplicate connector ID found: '{action.Id}', skipping.'");

        }  // connectors

        // Events
        var catEvents = _reflectionSvc.GetEvents(catAttrib.Id).OrderBy(c => c.Name);
        foreach (var ev in catEvents) {
          var tpEv = new Model.TouchPortalEvent {
            Id = $"{_options.PluginId}.{catAttrib.Id}.Event.{ev.Id}",   // these come unqualified
            Name = ev.Name,
            Format = ev.Format,
            Type = ev.Type,
            ValueType = ev.ValueType,
            ValueChoices = ev.ValueChoices,
            ValueStateId = $"{_options.PluginId}.{ev.ValueStateId}",
            SubCategoryId = subCategoryId,
          };
          // validate unique ID
          if (category.Events.FirstOrDefault(s => s.Id == tpEv.Id) == null)
            category.Events.Add(tpEv);
          else
            _logger.LogWarning($"Duplicate Event ID found: '{ev.Id}', skipping.'");
        }

        // States
        var categoryStates = simVars.Where(s => s.CategoryId == catAttrib.Id).OrderBy(s => s.Name);
        // If building for TPv4 then we want to add states to new categories, not in the main one.
        // This way they get sorted correctly along with the dynamic states.
        // If building for v3 then we already are in the individual categories.
        var stateCategory = category;
        if (tpv4 && categoryStates.Count() > 0) {
          stateCategory = new TouchPortalCategory {
            Id = $"{_options.PluginId}.{catAttrib.Id}",
            Name = fullCatName,
            Imagepath = baseIconPath + catAttrib.Imagepath
          };
          model.Categories.Add(stateCategory);
        }
        foreach (SimVarItem state in categoryStates) {
          var newState = new TouchPortalState {
            Id = state.TouchPortalStateId,
            Type = state.TouchPortalValueType,
            Description = $"{fullCatName} - {state.Name}",
            DefaultValue = state.DefaultValue ?? string.Empty,
            ValueChoices = state.TouchPortalValueChoices,
            //ParentGroup = fullCatName,
          };
          // validate unique ID
          if (stateCategory.States.FirstOrDefault(s => s.Id == newState.Id) == null)
            stateCategory.States.Add(newState);
          else
            _logger.LogWarning($"Duplicate state ID found: '{newState.Id}', skipping.'");
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
          ReadOnly = s.ReadOnly,
          Tooltip = new TouchPortalTooltip {
            Title = Regex.Replace(s.Name, @" \(.+\)$", ""),
            Body = s.Description,
            DocUrl = (string.IsNullOrEmpty(s.DocsUrl) ? _options.DocumentationUrl : s.DocsUrl )
          }
        };
        if (s.MaxLength > 0)
          setting.MaxLength = s.MaxLength;
        else if (!double.IsNaN(s.TpMinValue))
          setting.MinValue = s.TpMinValue;
        else if (!double.IsNaN(s.MinValue))
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
      _logger.LogInformation($"Generated '{dest}'.\n");
    }
  }
}
