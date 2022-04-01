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

  internal class ReflectionService : IReflectionService {
    private readonly string rootName = Assembly.GetExecutingAssembly().GetName().Name;
    private readonly ILogger<ReflectionService> _logger;

    public ReflectionService(ILogger<ReflectionService> logger) {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // Mapping of the generated SimConnect client event ID Enum to the SimConnect event name and group id.
    // Primarily used to register events with SimConnect, but can also be useful for debug/logging.
    // Structure is: <Event ID> -> new { string EventName, Enums.Groups GroupId }
    private static readonly Dictionary<Enum, dynamic> clientEventIdToNameMap = new();

    public ref readonly Dictionary<Enum, dynamic> GetClientEventIdToNameMap() => ref clientEventIdToNameMap;
    public string GetSimEventNameById(Enum id) {
      return clientEventIdToNameMap.TryGetValue(id, out var entry) ? entry.EventName : "[unknown event]";
    }
    public string GetSimEventNameById(uint id) => GetSimEventNameById((SimEventClientId)id);
    public string GetSimEventNameById(int id) => GetSimEventNameById((SimEventClientId)id);

    public Dictionary<string, ActionEventType> GetActionEvents() {
      var returnDict = new Dictionary<string, ActionEventType>();
      int nextId = (int)SimEventClientId.Init + 1;

      // Get all types which have actions mapped to events, sim or internal.
      var eventContainers = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass && (t.GetCustomAttribute<TouchPortalCategoryAttribute>() != null));
      foreach (var notifyType in eventContainers) {
        // Get the TP Category ID for this type
        Groups catId = notifyType.GetCustomAttribute<TouchPortalCategoryAttribute>()?.Id ?? default;
        if (catId == default)
          continue;
        var internalEvent = catId == Groups.Plugin;
        // Get all members which have an action mapping, which is some unique combination of value(s) mapped to a SimConnect event name or internal event enum
        List<MemberInfo> actionMembers = notifyType.GetMembers().Where(m => m.CustomAttributes.Any(att => att.AttributeType == typeof(TouchPortalActionMappingAttribute))).ToList();
        actionMembers.ForEach(e => {
          // Create the action data object to store in the return dict, using the meta data we've collected so far.
          ActionEventType act = new ActionEventType {
            InternalEvent = internalEvent,
            CategoryId = catId,
            ActionId = e.GetCustomAttribute<TouchPortalActionAttribute>().Id,
            //ActionObject = e
          };
          // Loop over all the data attributes to find the "choice" types for mapping and also the index of any free-form data value field
          var dataAttribs = e.GetCustomAttributes<TouchPortalActionDataAttribute>().ToList() ?? new();
          for (var i = 0; i < dataAttribs.Count; ++i) {
            var dataAttrib = dataAttribs[i];
            if (dataAttrib.ValueType == DataType.Choice) {
              // for Choice types, we combine them to create a unique lookup key which maps to a particular event.
              if (act.KeyFormatStr.Length > 1)
                act.KeyFormatStr += ",";
              act.KeyFormatStr += $"{{{i}}}";
            }
            else {
              // we only support one free-form value per mapping, which is all SimConnect supports, and internal events can handle the data as necessary already.
              act.ValueIndex = i;
              act.MinValue = dataAttrib.MinValue;
              act.MaxValue = dataAttrib.MaxValue;
              act.ValueType = dataAttrib.ValueType;
            }
          }
          // Now get all the action mappings to produce the final list of all possible action events
          var mappingAttribs = e.GetCustomAttributes<TouchPortalActionMappingAttribute>()?.ToList();
          mappingAttribs?.ForEach(ma => {
            Enum mapTarget = internalEvent ? Enum.Parse<Plugin>(ma.ActionId) : (SimEventClientId)nextId++;
            // Put into collections
            if (!act.TpActionToEventMap.TryAdd($"{string.Join(",", ma.Values)}", mapTarget))
              _logger.LogWarning($"Duplicate action-to-event mapping found for action {act.ActionId} with choices '{string.Join(",", ma.Values)} for event '{ma.ActionId}'!'");
            // keep track of generated event IDs for Sim actions (for registering to SimConnect, and debug)
            if (!internalEvent)
              clientEventIdToNameMap[mapTarget] = new { EventName = ma.ActionId, GroupId = catId };
          });
          // Put into returned collection
          if (!returnDict.TryAdd($"{rootName}.{catId}.Action.{act.ActionId}", act))
            _logger.LogWarning($"Duplicate action ID found for action '{act.ActionId}' in category '{catId}', skipping.'");
        });
      }

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
              setting.TouchPortalStateId = $"{rootName}.Plugin.State.{setting.TouchPortalStateId}";
            returnDict.TryAdd(setting.Name, setting);
          }
        }
      }

      return returnDict;
    }
  }

}
