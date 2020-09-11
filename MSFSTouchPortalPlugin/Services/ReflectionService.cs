using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Services {
  internal class ReflectionService : IReflectionService {
    private readonly string rootName = Assembly.GetExecutingAssembly().GetName().Name;

    public Dictionary<string, Enum> GetInternalEvents() {
      var returnDict = new Dictionary<string, Enum>();

      // Map internal events
      var internalEventList = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsEnum && t.GetCustomAttribute<InternalEventAttribute>() != null).ToList();
      internalEventList.ForEach(internalEvent => {
        string catName = internalEvent.GetCustomAttribute<TouchPortalCategoryMappingAttribute>().CategoryId;

        var list = internalEvent.GetFields().Where(f => f.GetCustomAttribute<TouchPortalActionMappingAttribute>() != null).ToList();

        list.ForEach(ie => {
          string actionName = ie.GetCustomAttribute<TouchPortalActionMappingAttribute>().ActionId;
          string actionValue = ie.GetCustomAttribute<TouchPortalActionMappingAttribute>().Value;

          if (Enum.TryParse(ie.ReflectedType, ie.Name, out dynamic result)) {
            // Put into collection
            returnDict.TryAdd($"{rootName}.{catName}.Action.{actionName}:{actionValue}", result);
          }
        });
      });

      return returnDict;
    }

    public Dictionary<string, Enum> GetActionEvents() {
      var returnDict = new Dictionary<string, Enum>();

      // Map all Sim Events to the Sim
      var enumList = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsEnum && t.GetCustomAttribute<SimNotificationGroupAttribute>() != null).ToList();
      enumList.ForEach(enumValue => {
        // Get the notification group to register
        var group = enumValue.GetCustomAttribute<SimNotificationGroupAttribute>().Group;
        var catName = enumValue.GetCustomAttribute<TouchPortalCategoryMappingAttribute>().CategoryId;

        // Configure SimConnect action mappings
        var events = enumValue.GetMembers().Where(m => m.CustomAttributes.Any(att => att.AttributeType == typeof(SimActionEventAttribute))).ToList();

        events.ForEach(e => {
          // Map the touch portal action to the Sim Event
          if (Enum.TryParse(e.ReflectedType, e.Name, out dynamic result)) {
            // Register to Touch Portal
            string actionName = e.GetCustomAttribute<TouchPortalActionMappingAttribute>().ActionId;
            string actionValue = e.GetCustomAttribute<TouchPortalActionMappingAttribute>().Value;

            // Put into collection
            returnDict.TryAdd($"{rootName}.{catName}.Action.{actionName}:{actionValue}", result);
          }
        });
      });

      return returnDict;
    }

    public Dictionary<Definition, SimVarItem> GetStates() {
      var returnDict = new Dictionary<Definition, SimVarItem>();

      var stateFieldList = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass && t.GetCustomAttribute<SimVarDataRequestGroupAttribute>() != null).ToList();
      stateFieldList.ForEach(stateFieldClass => {
        string catName = stateFieldClass.GetCustomAttribute<TouchPortalCategoryAttribute>().Name;

        // Get all States and register to SimConnect
        var states = stateFieldClass.GetFields().Where(m => m.CustomAttributes.Any(att => att.AttributeType == typeof(SimVarDataRequestAttribute))).ToList();
        states.ForEach(s => {
          // Evaluate and setup the Touch Portal State ID
          string catId = stateFieldClass.GetCustomAttribute<TouchPortalCategoryAttribute>().Id;
          var item = (SimVarItem)s.GetValue(null);
          item.TouchPortalStateId = $"{rootName}.{catId}.State.{item.def.ToString()}";

          returnDict.TryAdd(item.def, item);
        });
      });

      return returnDict;
    }
  }
}
