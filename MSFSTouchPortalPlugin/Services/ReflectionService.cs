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
using MSFSTouchPortalPlugin.Constants;
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

    public ReflectionService(ILogger<ReflectionService> logger)
    {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

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
      var attribs = GetActionBaseAttributes<TouchPortalActionAttribute>(catId);
      foreach (var attr in attribs) {
        if (!attr.HasHoldFunctionality)
          continue;
        int fmtN = attr.Data.Count(),
          indxN = fmtN;
        var data = attr.Data.ToList();
        attr.OnHoldFormat = attr.Format + $" | Fire On {{{fmtN++}}} Repeat {{{fmtN++}}} \nRate {{{fmtN++}}} \nDelay {{{fmtN++}}} \n(ms, empty to use defaults)  ";
        data.Add(new TouchPortalActionChoiceAttribute(new [] { "Press", "Release", "Press & Release", "Repeat Only" }) { Id = "OnHoldAction", Label = "On Hold Action", UsedInMapping = false });
        data.Add(new TouchPortalActionSwitchAttribute(true) { Id = "OnHoldRepeat", Label = "Repeat While Held", SkipForValIndex = true });
        data.Add(new TouchPortalActionTextAttribute("", 0, float.MaxValue) { Id = "OnHoldRate", Label = "Held Action Repeat Rate", SkipForValIndex = true });
        data.Add(new TouchPortalActionTextAttribute("", 0, float.MaxValue) { Id = "OnHoldDelay", Label = "Held Action Repeat Delay", SkipForValIndex = true });
        attr.Data = data.ToArray();
      }
      return attribs;
    }

    public IEnumerable<TouchPortalConnectorAttribute> GetConnectorAttributes(Groups catId)
    {
      List<TouchPortalConnectorAttribute> attribs = new();
      // Generate connectors from compatible actions or explicit connector attributes.
      var actionAttribs = GetActionBaseAttributes<TouchPortalActionBaseAttribute>(catId, true);
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
          metaAttrib ??= new TouchPortalConnectorMetaAttribute(valueAttrib.MinValue, valueAttrib.MaxValue, valueAttrib.MinValue, valueAttrib.MaxValue, valueAttrib.AllowDecimals);
        }
        if (metaAttrib != null) {
          int fmtN = metaAttrib.RangeStartIndex < 0 ? connData.Count : metaAttrib.RangeStartIndex,
            idxN = fmtN;
          if (metaAttrib.InsertValueRange) {
            format += $"{{{fmtN++}}}-{{{fmtN++}}}";
            connData.Insert(idxN++, new TouchPortalActionTextAttribute(metaAttrib.DefaultMin.ToString(), metaAttrib.MinValue, metaAttrib.MaxValue, metaAttrib.AllowDecimals) { Id = "RangeMin", Label = "Value Range Minimum" });
            connData.Insert(idxN++, new TouchPortalActionTextAttribute(metaAttrib.DefaultMax.ToString(), metaAttrib.MinValue, metaAttrib.MaxValue, metaAttrib.AllowDecimals) { Id = "RangeMax", Label = "Value Range Maximum" });
          }
          if (metaAttrib.UseFeedback) {
            format += $"| Feedback From\n| State (opt):{{{fmtN++}}}{{{fmtN++}}}\nRange:{{{fmtN++}}}-{{{fmtN}}}";
            connData.Insert(idxN++, new TouchPortalActionChoiceAttribute("[connect plugin]", "") { Id = "FbCatId", Label = "Feedback Category" });
            connData.Insert(idxN++, new TouchPortalActionChoiceAttribute("[select a category]", "") { Id = "FbVarName", Label = "Feedback Variable" });
            connData.Insert(idxN++, new TouchPortalActionTextAttribute("", float.MinValue, float.MaxValue) { Id = "FbRangeMin", Label = "Feedback Range Minimum" });
            connData.Insert(idxN++, new TouchPortalActionTextAttribute("", float.MinValue, float.MaxValue) { Id = "FbRangeMax", Label = "Feedback Range Maximum" });
          }
        }
        if (conn == null) {
          conn = new (actAttr.Id, actAttr.Name, actAttr.Description, format) {
            Data = connData.ToArray(),
            Mappings = actAttr.Mappings,
            Deprecated = actAttr.Deprecated,
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

    IEnumerable<T> GetActionBaseAttributes<T>(Groups catId, bool connOnly = false) where T : TouchPortalActionBaseAttribute
    {
      List<T> ret = new();
      var container = _assemblyTypes.Where(t => t.IsClass && t.GetCustomAttribute<TouchPortalCategoryAttribute>()?.Id == catId);
      foreach (var c in container) {
        foreach (var m in c.GetMembers()) {
          if (m.GetCustomAttribute<T>() is var actionAttrib && actionAttrib != null) {
            if (connOnly && !(actionAttrib.GetType() == typeof(TouchPortalConnectorAttribute) || actionAttrib.ConnectorFormat != null))
                continue;
            actionAttrib.Data = m.GetCustomAttributes<TouchPortalActionDataAttribute>(true).ToArray();
            actionAttrib.Mappings = m.GetCustomAttributes<TouchPortalActionMappingAttribute>(true).ToArray();
            actionAttrib.ParentObject = m;
            var emptyChoices = actionAttrib.Data.Where(m => m is TouchPortalActionChoiceAttribute && m.ChoiceValues.Length == 0).ToArray();
            if (emptyChoices.Any() && actionAttrib.Mappings.Any()) {
              var choiceCount = emptyChoices.Length;
              //List<string>[] choices = new List<string>[choiceCount];
              foreach (var map in actionAttrib.Mappings) {
                if (map.Deprecated)
                  continue;
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

    private Dictionary<string, ActionEventType> GetInternalActionEvents()
    {
      var returnDict = new Dictionary<string, ActionEventType>();
      var catAttribs = GetActionAttributes(Groups.Plugin).Concat(GetActionAttributes(Groups.StatesEditor));
      // Loop over all actions which have an action mapping, which is some unique combination of value(s) mapped to a SimConnect event name or internal event enum
      foreach (var actAttr in catAttribs) {
        // Create the action data object to store in the return dict, using the meta data we've collected so far.
        ActionEventType act = new ActionEventType((PluginActions)actAttr.EnumId, actionId: actAttr.Id);
        // Put into returned collection
        if (!returnDict.TryAdd($"{Groups.Plugin}.{act.ActionId}", act)) {
          _logger.LogWarning("Duplicate action ID found for Plugin action '{actId}', skipping.'", act.ActionId);
          continue;
        }
        // for Choice types, we combine them to create a unique lookup key which maps to a particular event.
        if (actAttr.Mappings.Any()) {
          List<string> fmtStrList = new();
          for (int i=0, e = actAttr.Data.Count(d => d.ValueType == DataType.Choice && d.UsedInMapping); i < e; ++i)
            fmtStrList.Add($"{{{i}}}");
          act.KeyFormatStr = string.Join(",", fmtStrList);
          foreach (var ma in actAttr.Mappings) {
            if (!act.TryAddPluginEventMapping(string.Join(",", ma.Values), (PluginActions)ma.EnumId, ma.ActionId))
              _logger.LogWarning("Duplicate action-to-event mapping found for Plugin action {actId} with choices '{choices} for event '{mapId}'.", act.ActionId, string.Join(",", ma.Values), ma.ActionId);
          }
        }
      }
      _logger.LogDebug("Loaded {count} Internal Actions", returnDict.Count);
      return returnDict;
    }

    public Dictionary<string, ActionEventType> GetActionEvents()
    {
      var returnDict = GetInternalActionEvents();
      var catAttribs = GetCategoryAttributes();
      var intActsCnt = returnDict.Count;

      foreach (var catAttr in catAttribs) {
        if (Categories.InternalActionCategories.Contains(catAttr.Id))
          continue;
        // Loop over all actions which have an action mapping, which is some unique combination of value(s) mapped to a SimConnect event name or internal event enum
        foreach (var actAttr in catAttr.Actions) {
          // check that there are any mappings at all
          if (!actAttr.Mappings.Any()) {
            _logger.LogWarning("No event mappings found for action ID '{actId}' in category '{catName}'.", actAttr.Id, catAttr.Name);
            continue;
          }
          // Create the action data object to store in the return dict, using the meta data we've collected so far.
          ActionEventType act = new ActionEventType(actAttr.Id, catAttr.Id);
          // Put into returned collection
          if (!returnDict.TryAdd($"{catAttr.Id}.{act.ActionId}", act)) {
            _logger.LogWarning("Duplicate action ID found for action '{actId}' in category '{catName}', skipping.", act.ActionId, catAttr.Name);
            continue;
          }
          act.OutputValueIndex = actAttr.UserValueIndex;
          // Loop over all the data attributes to find the "choice" types for mapping and also the index of any free-form data value field
          int i = 0;
          List<string> fmtStrList = new();
          foreach (var dataAttrib in actAttr.Data) {
            if (dataAttrib.ValueType == DataType.Choice) {
              // for Choice types, we combine them to create a unique lookup key which maps to a particular event.
              if (dataAttrib.UsedInMapping)
                fmtStrList.Add($"{{{i}}}");
            }
            else if (!dataAttrib.SkipForValIndex) {
              // we only support one free-form value per mapping, which is all SimConnect supports, and internal events can handle the data as necessary already.
              act.ValueIndex = i;
              act.MinValue = dataAttrib.MinValue;
              act.MaxValue = dataAttrib.MaxValue;
              act.ValueType = dataAttrib.ValueType;
            }
            ++i;
          }
          act.KeyFormatStr = string.Join(",", fmtStrList);
          // Now get all the action mappings to produce the final list of all possible action events
          foreach (var ma in actAttr.Mappings) {
            // Put into collections
            if (!act.TryAddSimEventMapping(string.Join(",", ma.Values), ma.ActionId, ma.EventValues))
              _logger.LogWarning("Duplicate action-to-event mapping found for action {actId} with choices '{choices} for event '{mapId}'.", act.ActionId, string.Join(",", ma.Values), ma.ActionId);
          }

        }  // actions loop
      }  // categories loop

      _logger.LogDebug("Loaded {count} SimConnect Actions", returnDict.Count - intActsCnt);
      return returnDict;
    }

    public Dictionary<string, PluginSetting> GetSettings() {
      Dictionary<string, PluginSetting> returnDict = new();

      var setContainers = _assemblyTypes.Where(t => t.IsClass && t.GetCustomAttribute<TouchPortalSettingsContainerAttribute>() != null);
      foreach (Type setCtr in setContainers) {
        var settingFields = setCtr.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
        foreach (FieldInfo field in settingFields) {
          if (field.FieldType == typeof(PluginSetting) && (PluginSetting)field.GetValue(null) is var setting && setting != null && !string.IsNullOrEmpty(setting.Name)) {
            if (!returnDict.TryAdd(setting.Name, setting))
              _logger.LogError("Duplicate Setting name detected: {name}", setting.Name);
          }
        }
      }

      return returnDict;
    }


  }

}
