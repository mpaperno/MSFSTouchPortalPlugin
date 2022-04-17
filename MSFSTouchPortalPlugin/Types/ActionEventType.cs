using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MSFSTouchPortalPlugin.Types
{
  internal class ActionEventType
  {
    public Enum Id;
    public Groups CategoryId;
    public string ActionId;
    public int ValueIndex = -1;
    public string KeyFormatStr = string.Empty;
    public double MinValue = double.NaN;  // for basic validation of a single value
    public double MaxValue = double.NaN;
    public DataType ValueType = DataType.None;  // assuming single value here also

    public IReadOnlyDictionary<string, TouchPortalActionDataAttribute> DataAttributes;  // list of all data type attributes

    // Mapping of TP actions to SimConnect or "Native" events. For SimConnect, the Enum is a generated number
    // of type SimEventClientId (but doesn't actually exist), and for internal plugin events its an actual
    // member of the MSFSTouchPortalPlugin.Objects.Plugin.PluginActions enum.
    readonly Dictionary<string, Enum> TpActionToEventMap = new();

    // this is how we generate unique SimConnect client Event IDs.
    private static EventIds _nextEventId = EventIds.DynamicEventInit;
    private static EventIds NextId() => ++_nextEventId;      // got a warning when trying to increment this directly from c'tor, but not via static member... ?

    public ActionEventType() { }

    /// <summary> c'tor for dynamically added actions with just one sim event, which returns the generated event ID of actual type SimEventClientId </summary>
    public ActionEventType(string actionId, Groups categoryId, bool hasValue, out Enum eventId) {
      Id = eventId = NextId();
      ActionId = actionId;
      CategoryId = categoryId;
      ValueIndex = hasValue ? 0 : -1;
      ValueType = DataType.Number;
    }

    public bool TryAddSimEventMapping(string actionId, out Enum eventId) {
      eventId = NextId();
      return TpActionToEventMap.TryAdd(actionId, eventId);
    }

    public bool TryAddPluginEventMapping(string actionName, PluginActions eventId) {
      return TpActionToEventMap.TryAdd(actionName, eventId);
    }

    // Get a unique event ID for this action, possibly based on data values
    // in the \c data array. Certain combination of values, eg. from choices,
    // may have their own unique events. Returns `false` if the lookup fails.
    public bool TryGetEventMapping(in string[] values, out Enum eventId) {
      if (!TpActionToEventMap.Any()) {
        eventId = Id;
        return true;
      }
      if (TpActionToEventMap.Count == 1) {
        eventId = TpActionToEventMap.Values.First();
        return true;
      }
      return TpActionToEventMap.TryGetValue(FormatLookupKey(values), out eventId);
    }

    public bool TryGetEventMapping(string value, out Enum eventId) =>
      TryGetEventMapping(new string[] { value }, out eventId);

    // Helper to format an array of action data values into a unique key
    // used for indexing the TpActionToEventMap dictionary.
    public string FormatLookupKey(in string[] values) {
      if (string.IsNullOrWhiteSpace(KeyFormatStr))
        return string.Empty;
      try { return string.Format(KeyFormatStr, values); }
      catch { return string.Empty; }
    }

  }
}
