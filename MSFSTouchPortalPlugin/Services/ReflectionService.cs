using Microsoft.Extensions.Logging;
using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Enums;
using MSFSTouchPortalPlugin.Interfaces;
using MSFSTouchPortalPlugin.Objects.Plugin;
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

    public ReflectionService(ILogger<ReflectionService> logger) {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // Mapping of the generated SimConnect client event ID Enum to the SimConnect event name and group id.
    // Primarily used to register events with SimConnect, but can also be useful for debug/logging.
    // Structure is: <Event ID> -> new { string EventName, Enums.Groups GroupId }
    private static readonly Dictionary<Enum, SimEventRecord> clientEventIdToNameMap = new();

    public ref readonly Dictionary<Enum, SimEventRecord> GetClientEventIdToNameMap() => ref clientEventIdToNameMap;
    public string GetSimEventNameById(Enum id) {
      return clientEventIdToNameMap.TryGetValue(id, out var entry) ? entry.EventName : "[unknown event]";
    }
    public string GetSimEventNameById(uint id) => GetSimEventNameById((SimEventClientId)id);
    public string GetSimEventNameById(int id) => GetSimEventNameById((SimEventClientId)id);

    public IEnumerable<TouchPortalCategoryAttribute> GetCategoryAttributes() {
      List<TouchPortalCategoryAttribute> ret = new();
      foreach (Groups catId in Enum.GetValues<Groups>()) {
        if (catId != Groups.None)
          ret.Add(new TouchPortalCategoryAttribute(catId) {
            Actions = GetActionAttributes(catId).ToArray()
          });
      }
      return ret;
    }

    public IEnumerable<TouchPortalActionAttribute> GetActionAttributes(Groups catId) {
      List<TouchPortalActionAttribute> ret = new();
      var container = _assemblyTypes.Where(t => t.IsClass && t.GetCustomAttribute<TouchPortalCategoryAttribute>()?.Id == catId);
      foreach (var c in container) {
        foreach (var m in c.GetMembers()) {
          if (m.GetCustomAttribute<TouchPortalActionAttribute>() is var actionAttrib && actionAttrib != null) {
            actionAttrib.Data = m.GetCustomAttributes<TouchPortalActionDataAttribute>(true).ToArray();
            actionAttrib.Mappings = m.GetCustomAttributes<TouchPortalActionMappingAttribute>(true).ToArray();
            ret.Add(actionAttrib);
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
          Id = Enum.Parse<Plugin>(actAttr.Id),
          ActionId = actAttr.Id,
          CategoryId = Groups.Plugin,
          DataAttributes = actAttr.Data.ToDictionary(d => d.Id, d => d)
        };
        // Put into returned collection
        if (!returnDict.TryAdd($"{TouchPortalBaseId}.{Groups.Plugin}.Action.{act.ActionId}", act)) {
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
            if (!act.TpActionToEventMap.TryAdd($"{string.Join(",", ma.Values)}", Enum.Parse<Plugin>(ma.ActionId)))
              _logger.LogWarning($"Duplicate action-to-event mapping found for Plugin action {act.ActionId} with choices '{string.Join(",", ma.Values)} for event '{ma.ActionId}'.");
          }
        }
      }
      return returnDict;
    }

    public Dictionary<string, ActionEventType> GetActionEvents() {
      int nextId = (int)SimEventClientId.Init + 1;
      var returnDict = GetInternalActionEvents();
      var catAttribs = GetCategoryAttributes();

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
          if (!returnDict.TryAdd($"{TouchPortalBaseId}.{catAttr.Id}.Action.{act.ActionId}", act)) {
            _logger.LogWarning($"Duplicate action ID found for action '{act.ActionId}' in category '{catAttr.Id}', skipping.'");
            continue;
          }
          // Check for mappings
          if (!actAttr.Mappings.Any()) {
            _logger.LogWarning($"No ActionMapping attributes found for action '{act.ActionId}' in category '{catAttr.Id}', skipping.'");
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
            Enum mapTarget = (SimEventClientId)nextId++;
            // Put into collections
            if (!act.TpActionToEventMap.TryAdd($"{string.Join(",", ma.Values)}", mapTarget))
              _logger.LogWarning($"Duplicate action-to-event mapping found for action {act.ActionId} with choices '{string.Join(",", ma.Values)} for event '{ma.ActionId}'.");
            // keep track of generated event IDs for Sim actions (for registering to SimConnect, and debug)
            clientEventIdToNameMap[mapTarget] = new SimEventRecord(catAttr.Id, ma.ActionId);
          }

        }  // actions loop
      }  // categories loop

      return returnDict;
    }

    public Dictionary<string, PluginSetting> GetSettings() {
      Dictionary<string, PluginSetting> returnDict = new();

      var setContainers = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass && t.GetCustomAttribute<TouchPortalSettingsContainerAttribute>() != null);
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
