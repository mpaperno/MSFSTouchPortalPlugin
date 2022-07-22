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
using MSFSTouchPortalPlugin.Enums;
using MSFSTouchPortalPlugin.Interfaces;
using MSFSTouchPortalPlugin.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MSFSTouchPortalPlugin.Services
{

  internal class ReflectionService : IReflectionService
  {

    public static Assembly ExecutingAssembly { get; set; } = Assembly.GetExecutingAssembly();
    public static string TouchPortalBaseId { get; set; } = ExecutingAssembly.GetName().Name;

    private readonly Type[] _assemblyTypes = ExecutingAssembly.GetTypes();
    private readonly ILogger<ReflectionService> _logger;
    // Mapping of the generated SimConnect client event ID Enum to the SimConnect event name and group id.
    // Primarily used to register events with SimConnect, but can also be useful for debug/logging.
    // Structure is: <Event ID> -> new { string EventName, Enums.Groups GroupId }
    private static readonly Dictionary<Enum, SimEventRecord> clientEventIdToNameMap = new();

    public ReflectionService(ILogger<ReflectionService> logger)
    {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public ref readonly Dictionary<Enum, SimEventRecord> GetClientEventIdToNameMap() => ref clientEventIdToNameMap;
    public string GetSimEventNameById(Enum id) => clientEventIdToNameMap.TryGetValue(id, out var entry) ? entry.EventName : "[unknown event]";
    public string GetSimEventNameById(uint id) => GetSimEventNameById((EventIds)id);
    public string GetSimEventNameById(int id) => GetSimEventNameById((EventIds)id);
    public void AddSimEventNameMapping(Enum id, SimEventRecord record) => clientEventIdToNameMap[id] = record;

    public IEnumerable<TouchPortalCategoryAttribute> GetCategoryAttributes() {
      List<TouchPortalCategoryAttribute> ret = new();
      foreach (Groups catId in Enum.GetValues<Groups>()) {
        if (catId != Groups.None)
          ret.Add(new TouchPortalCategoryAttribute(catId) {
            Actions = GetActionAttributes(catId).ToArray(),
            Connectors = GetConnectorAttributes(catId).ToArray()
          });
      }
      return ret;
    }

    public IEnumerable<TouchPortalActionAttribute> GetActionAttributes(Groups catId) {
      return GetActionBaseAttributes<TouchPortalActionAttribute>(catId);
    }

    public IEnumerable<TouchPortalConnectorAttribute> GetConnectorAttributes(Groups catId)
    {
      List<TouchPortalConnectorAttribute> attribs = new(); //GetActionBaseAttributes<TouchPortalConnectorAttribute>(catId).ToList();
      // Generate connectors from compatible actions or explicit connector attributes.
      var actionAttribs = GetActionBaseAttributes<TouchPortalActionBaseAttribute>(catId).Where(m => m.GetType() == typeof(TouchPortalConnectorAttribute) || m.ConnectorFormat != null);
      foreach (var actAttr in actionAttribs) {
        TouchPortalConnectorAttribute conn = null;
        List<TouchPortalActionDataAttribute> connData;
        string format = string.Empty;
        TouchPortalConnectorMetaAttribute metaAttrib = actAttr.ParentObject.GetCustomAttribute<TouchPortalConnectorMetaAttribute>();
        if (actAttr.GetType() == typeof(TouchPortalConnectorAttribute)) {
          conn = actAttr as TouchPortalConnectorAttribute;
          connData = conn.Data.ToList();
          format = conn.Format;
        }
        else {
          TouchPortalActionDataAttribute valueAttrib = actAttr.Data.FirstOrDefault(m => (m.GetType() == typeof(TouchPortalActionTextAttribute) || m.GetType() == typeof(TouchPortalActionNumericAttribute)) && !double.IsNaN(m.MinValue));
          if (valueAttrib == null)
            continue;
          connData = actAttr.Data.Where(m => m != valueAttrib).ToList();
          format = actAttr.ConnectorFormat;
          if (metaAttrib == null)
            metaAttrib = new TouchPortalConnectorMetaAttribute(valueAttrib.MinValue, valueAttrib.MaxValue, valueAttrib.MinValue, valueAttrib.MaxValue, valueAttrib.AllowDecimals);
        }
        if (metaAttrib != null) {
          int fmtN = metaAttrib.RangeStartIndex < 0 ? connData.Count : metaAttrib.RangeStartIndex;
          format += $"{{{fmtN++}}}-{{{fmtN++}}}";
          connData.Add(new TouchPortalActionNumericAttribute(metaAttrib.DefaultMin, metaAttrib.MinValue, metaAttrib.MaxValue, metaAttrib.AllowDecimals) { Id = "RangeMin", Label = "Value Range Minimum" });
          connData.Add(new TouchPortalActionNumericAttribute(metaAttrib.DefaultMax, metaAttrib.MinValue, metaAttrib.MaxValue, metaAttrib.AllowDecimals) { Id = "RangeMax", Label = "Value Range Maximum" });
          if (metaAttrib.UseFeedback) {
            format += $"| Feedback From\n| State (opt):{{{fmtN++}}}{{{fmtN++}}}\nRange:{{{fmtN++}}}-{{{fmtN}}}";
            connData.Add(new TouchPortalActionChoiceAttribute("[connect plugin]", "") { Id = "FbCatId", Label = "Feedback Category" });
            connData.Add(new TouchPortalActionChoiceAttribute("[select a category]", "") { Id = "FbVarName", Label = "Feedback Variable" });
            connData.Add(new TouchPortalActionTextAttribute("", float.MinValue, float.MaxValue) { Id = "FbRangeMin", Label = "Feedback Range Minimum" });
            connData.Add(new TouchPortalActionTextAttribute("", float.MinValue, float.MaxValue) { Id = "FbRangeMax", Label = "Feedback Range Maximum" });
          }
        }
        if (conn == null) {
          conn = new (actAttr.Id, actAttr.Name, actAttr.Description, format) {
            Data = connData.ToArray(),
            Mappings = actAttr.Mappings
          };
        }
        else if (metaAttrib != null) {
          conn.Format = format;
          conn.Data = connData.ToArray();
        }
        attribs.Add(conn);
      }
      return attribs;
    }

    IEnumerable<T> GetActionBaseAttributes<T>(Groups catId) where T : TouchPortalActionBaseAttribute
    {
      List<T> ret = new();
      var container = _assemblyTypes.Where(t => t.IsClass && t.GetCustomAttribute<TouchPortalCategoryAttribute>()?.Id == catId);
      foreach (var c in container) {
        foreach (var m in c.GetMembers()) {
          if (m.GetCustomAttribute<T>() is var actionAttrib && actionAttrib != null) {
            actionAttrib.Data = m.GetCustomAttributes<TouchPortalActionDataAttribute>(true).ToArray();
            actionAttrib.Mappings = m.GetCustomAttributes<TouchPortalActionMappingAttribute>(true).ToArray();
            actionAttrib.ParentObject = m;
            var emptyChoices = actionAttrib.Data.Where(m => m is TouchPortalActionChoiceAttribute && m.ChoiceValues.Length == 0).ToArray();
            if (emptyChoices.Any() && actionAttrib.Mappings.Any()) {
              var choiceCount = emptyChoices.Count();
              //List<string>[] choices = new List<string>[choiceCount];
              foreach (var map in actionAttrib.Mappings) {
                for (int i = 0; i < choiceCount; ++i) {
                  var choiceEl = emptyChoices[i] as TouchPortalActionChoiceAttribute;
                  if (map.Values.Length > i && map.Values[i] is var choice && !choiceEl.ChoiceValues.Contains(choice)) {
                    choiceEl.ChoiceValues = choiceEl.ChoiceValues.Append(choice).ToArray();
                    if (string.IsNullOrEmpty(choiceEl.DefaultValue))
                      choiceEl.DefaultValue = choice;
                  }
                }
              }
            }

            ret.Add(actionAttrib);
          }
        }
      }
      return ret;
    }

    public IEnumerable<TouchPortalEvent> GetEvents(Groups catId, bool fullStateId = false) {
      List<TouchPortalEvent> ret = new();
      var container = _assemblyTypes.Where(t => t.IsClass && t.GetCustomAttribute<TouchPortalCategoryAttribute>()?.Id == catId);
      foreach (Type c in container) {
        var fields = c.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
        foreach (FieldInfo field in fields) {
          if (field.FieldType == typeof(TouchPortalEvent) && ((TouchPortalEvent)field.GetValue(null) is var ev && ev != null)) {
            if (ev.ValueStateId?.IndexOf('.') < 0)
              ev.ValueStateId = catId.ToString() + ".State." + ev.ValueStateId;  // qualify with category name, but not plugin name (which is assumed)
            if (fullStateId)
              ev.ValueStateId = TouchPortalBaseId + '.' + ev.ValueStateId;  // ok actually add the plugin name also, this is for the generators
            ret.Add(ev);
          }
        }
      }
      return ret;
    }

    private Dictionary<string, ActionEventType> GetInternalActionEvents() {
      var returnDict = new Dictionary<string, ActionEventType>();
      var catAttribs = GetActionAttributes(Groups.Plugin);
      // Loop over all actions which have an action mapping, which is some unique combination of value(s) mapped to a SimConnect event name or internal event enum
      foreach (var actAttr in catAttribs) {
        // Create the action data object to store in the return dict, using the meta data we've collected so far.
        ActionEventType act = new ActionEventType {
          Id = actAttr.EnumId,
          ActionId = actAttr.Id,
          CategoryId = Groups.Plugin,
          DataAttributes = actAttr.Data.ToDictionary(d => d.Id, d => d)
        };
        // Put into returned collection
        if (!returnDict.TryAdd($"{Groups.Plugin}.{act.ActionId}", act)) {
          _logger.LogWarning($"Duplicate action ID found for Plugin action '{act.ActionId}', skipping.'");
          continue;
        }
        // for Choice types, we combine them to create a unique lookup key which maps to a particular event.
        if (actAttr.Mappings.Any()) {
          List<string> fmtStrList = new();
          for (int i=0, e = actAttr.Data.Count(d => d.ValueType == DataType.Choice); i < e; ++i)
            fmtStrList.Add($"{{{i}}}");
          act.KeyFormatStr = string.Join(",", fmtStrList);
          foreach (var ma in actAttr.Mappings) {
            if (!act.TryAddPluginEventMapping($"{string.Join(",", ma.Values)}", (PluginActions)ma.EnumId))
              _logger.LogWarning($"Duplicate action-to-event mapping found for Plugin action {act.ActionId} with choices '{string.Join(",", ma.Values)} for event '{ma.ActionId}'.");
          }
        }
      }
      _logger.LogDebug($"Loaded {returnDict.Count} Internal Actions");
      return returnDict;
    }

    public Dictionary<string, ActionEventType> GetActionEvents() {
      var returnDict = GetInternalActionEvents();
      var catAttribs = GetCategoryAttributes();
      var intActsCnt = returnDict.Count;

      foreach (var catAttr in catAttribs) {
        if (catAttr.Id == Groups.Plugin)
          continue;
        // Loop over all actions which have an action mapping, which is some unique combination of value(s) mapped to a SimConnect event name or internal event enum
        foreach (var actAttr in catAttr.Actions) {
          // check that there are any mappings at all
          if (!actAttr.Mappings.Any()) {
            _logger.LogWarning($"No event mappings found for action ID '{actAttr.Id}' in category '{catAttr.Name}'.");
            continue;
          }
          // Create the action data object to store in the return dict, using the meta data we've collected so far.
          ActionEventType act = new ActionEventType {
            CategoryId = catAttr.Id,
            ActionId = actAttr.Id
          };
          // Put into returned collection
          if (!returnDict.TryAdd($"{catAttr.Id}.{act.ActionId}", act)) {
            _logger.LogWarning($"Duplicate action ID found for action '{act.ActionId}' in category '{catAttr.Id}', skipping.'");
            continue;
          }
          // Loop over all the data attributes to find the "choice" types for mapping and also the index of any free-form data value field
          int i = 0;
          List<string> fmtStrList = new();
          foreach (var dataAttrib in actAttr.Data) {
            if (dataAttrib.ValueType == DataType.Choice) {
              // for Choice types, we combine them to create a unique lookup key which maps to a particular event.
              fmtStrList.Add($"{{{i++}}}");
            }
            else {
              // we only support one free-form value per mapping, which is all SimConnect supports, and internal events can handle the data as necessary already.
              act.ValueIndex = i++;
              act.MinValue = dataAttrib.MinValue;
              act.MaxValue = dataAttrib.MaxValue;
              act.ValueType = dataAttrib.ValueType;
            }
          }
          act.KeyFormatStr = string.Join(",", fmtStrList);
          // Now get all the action mappings to produce the final list of all possible action events
          foreach (var ma in actAttr.Mappings) {
            // Put into collections
            if (act.TryAddSimEventMapping($"{string.Join(",", ma.Values)}", ma.EventValue, out Enum mapTarget))
              // keep track of generated event IDs for Sim actions (for registering to SimConnect, and debug)
              clientEventIdToNameMap[mapTarget] = new SimEventRecord(catAttr.Id, ma.ActionId);
            else
              _logger.LogWarning($"Duplicate action-to-event mapping found for action {act.ActionId} with choices '{string.Join(",", ma.Values)} for event '{ma.ActionId}'.");
          }

        }  // actions loop
      }  // categories loop

      _logger.LogDebug($"Loaded {returnDict.Count - intActsCnt} SimConnect Actions");
      return returnDict;
    }

    public Dictionary<string, PluginSetting> GetSettings() {
      Dictionary<string, PluginSetting> returnDict = new();

      var setContainers = _assemblyTypes.Where(t => t.IsClass && t.GetCustomAttribute<TouchPortalSettingsContainerAttribute>() != null);
      foreach (Type setCtr in setContainers) {
        var settingFields = setCtr.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
        foreach (FieldInfo field in settingFields) {
          if (field.FieldType == typeof(PluginSetting) && ((PluginSetting)field.GetValue(null) is var setting && setting != null)) {
            if (!string.IsNullOrWhiteSpace(setting.TouchPortalStateId))
              setting.TouchPortalStateId = $"{TouchPortalBaseId}.Plugin.State.{setting.TouchPortalStateId}";
            returnDict.TryAdd(setting.Name, setting);
          }
        }
      }

      return returnDict;
    }


  }

}
