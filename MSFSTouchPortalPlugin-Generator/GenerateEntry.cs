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

    bool SerializeActionData(TouchPortalActionBase action, TouchPortalActionDataAttribute[] attribData)
    {
      int i = 0;
      foreach (var attrib in attribData) {
        string dataId = (string.IsNullOrWhiteSpace(attrib.Id) ? i.ToString() : attrib.Id);
        var data = new TouchPortalActionData {
          Id = $"{action.Id}.Data.{dataId}",
          Type = attrib.Type,
          Label = attrib.Label ?? "Action",
          DefaultValue = attrib.GetDefaultValue(),
          ValueChoices = attrib.ChoiceValues,
          MinValue = attrib.MinValue,
          MaxValue = attrib.MaxValue,
          AllowDecimals = attrib.AllowDecimals,
        };
        ++i;
        action.Data.Add(data);
      }
      try {
        action.Format = string.Format(action.Format, action.Data.Select(d => $"{{${d.Id}$}}").ToArray());
      }
      catch {
        _logger.LogError("Failed to format {0} for {1} with data {2}", action.Format, action.Id, string.Join(',', action.Data.Select(d => $"{{${d.Id}$}}")));
        return false;
      }
      return true;
    }

    public void Generate()
    {
      string basePath = $"%TP_PLUGIN_FOLDER%{_options.PluginFolder}/";
      // Get version number
      VersionInfo.AssemblyLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), _options.PluginId + ".dll");
      uint vNum = VersionInfo.GetProductVersionNumber();
      // read the internal plugin states config
      IEnumerable<SimVarItem> simVars = _pluginConfig.LoadPluginStates();

      _logger.LogInformation($"Generating entry.tp v{vNum:X} to '{_options.OutputPath}'.");

      // Setup Base Model
      var model = new Base {
        Sdk = 6,
        Version = vNum,
        Name = _options.PluginName,
        Id = _options.PluginId
      };
      if (!_options.Debug)
        model.Plugin_start_cmd = $"\"{basePath}dist/{_options.PluginId}.exe\"";
      model.Configuration.ColorDark = "#" + _options.ColorDark.Trim('#');
      model.Configuration.ColorLight = "#" + _options.ColorLight.Trim('#');

      var categegoryAttribs = _reflectionSvc.GetCategoryAttributes();
      foreach (var catAttrib in categegoryAttribs) {
        var category = new TouchPortalCategory {
          Id = $"{_options.PluginId}.{catAttrib.Id}",
          Name = Categories.FullCategoryName(catAttrib.Id),
          Imagepath = basePath + catAttrib.Imagepath
        };
        model.Categories.Add(category);

        // workaround for backwards compat with mis-named actions in category InstrumentsSystems.Fuel
        string actionCatId = _options.PluginId + "." + Categories.ActionCategoryId(catAttrib.Id);

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
            TryInline = true,
            Format = actionAttrib.Format,
            HasHoldFunctionality = actionAttrib.HasHoldFunctionality,
          };

          // Action Data
          if (actionAttrib.Data.Any() && !SerializeActionData(action, actionAttrib.Data))
            continue;

          // validate unique ID
          if (category.Actions.FirstOrDefault(a => a.Id == action.Id) == null)
            category.Actions.Add(action);
          else
            _logger.LogWarning($"Duplicate action ID found: '{action.Id}', skipping.'");

        }  // actions

        // Connectors
        foreach (var connAttrib in catAttrib.Connectors) {
          var action = new TouchPortalConnector {
            Id = $"{actionCatId}.Conn.{connAttrib.Id}",
            Name = connAttrib.Name,
            Format = connAttrib.Format
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

        // States
        var categoryStates = simVars.Where(s => s.CategoryId == catAttrib.Id);
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

        // Events
        var catEvents = _reflectionSvc.GetEvents(catAttrib.Id, fullStateId: true);
        foreach (var ev in catEvents) {
          var tpEv = new Model.TouchPortalEvent {
            Id = category.Id + ".Event." + ev.Id,   // these come unqualified
            Name = ev.Name,
            Format = ev.Format,
            Type = ev.Type,
            ValueType = ev.ValueType,
            ValueChoices = ev.ValueChoices,
            ValueStateId =ev.ValueStateId,
          };
          // validate unique ID
          if (category.Events.FirstOrDefault(s => s.Id == tpEv.Id) == null)
            category.Events.Add(tpEv);
          else
            _logger.LogWarning($"Duplicate Event ID found: '{ev.Id}', skipping.'");
        }

        // Sort the actions and states for SimConnect groups
        if (catAttrib.Id != MSFSTouchPortalPlugin.Enums.Groups.Plugin) {
          category.Actions = category.Actions.OrderBy(c => c.Name).ToList();
          category.Events = category.Events.OrderBy(c => c.Name).ToList();
          category.States = category.States.OrderBy(c => c.Description).ToList();
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
    }
  }
}
