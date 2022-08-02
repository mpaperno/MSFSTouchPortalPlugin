/*
This file is part of the MSFS Touch Portal Plugin project.
https://github.com/mpaperno/MSFSTouchPortalPlugin

COPYRIGHT: (c) Maxim Paperno; All Rights Reserved.

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

using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MSFSTouchPortalPlugin.Types
{
  internal class EventMappingRecord
  {
    // The event name is stored for mapping or lookup upon initial invocation of the event, and for logging reference.
    public string EventName;
    // For SimConnect, the EventId Enum is a generated number of type EventIds.
    // For WASimCommander events this is the actual KEY_ID of the event in the simulator engine.
    // For internal plugin events its an actual member of the MSFSTouchPortalPlugin.Objects.Plugin.PluginActions enum.
    public Enum EventId = EventIds.None;
    // Value(s) may be zero or more static values which accompany the event
    public uint[] Values = Array.Empty<uint>();

    public EventMappingRecord(string eventName, Enum eventId = null, uint[] values = null)
    {
      EventName = eventName;
      if (eventId != null)
        EventId = eventId;
      if (values != null)
        Values = values;
    }
    public EventMappingRecord(string eventName, Enum eventId, uint value) : this(eventName, eventId, new uint[] { value }) { }
    public EventMappingRecord(string eventName, uint value) : this(eventName, null, value) { }
  }

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

    // Mapping of TP actions to SimConnect or "Native" events.
    readonly Dictionary<string, EventMappingRecord> TpActionToEventMap = new();

    // this is how we generate unique SimConnect client Event IDs.
    private static EventIds _nextEventId = EventIds.DynamicEventInit;
    public static EventIds NextId() => ++_nextEventId;      // got a warning when trying to increment this directly from c'tor, but not via static member... ?

    /// <summary> c'tor used by ReflectionService when constructing an action from Object attribute data; creates no mapping. </summary>
    public ActionEventType(string actionId, Groups categoryId)
    {
      ActionId = actionId;
      CategoryId = categoryId;
    }

    /// <summary> c'tor for internal plugin events with explicit PluginActions Id type, creates a default mapping. </summary>
    public ActionEventType(PluginActions eventId, string actionId)
    {
      Id = eventId;
      ActionId = actionId;
      CategoryId = Groups.Plugin;
      TpActionToEventMap.TryAdd(actionId, new EventMappingRecord(ActionId, Id));
    }

    /// <summary> c'tor for dynamically added actions with just one sim event; creates a default mapping. </summary>
    public ActionEventType(string actionId, Groups categoryId, bool hasValue) {
      ActionId = actionId;
      CategoryId = categoryId;
      ValueIndex = hasValue ? 0 : -1;
      ValueType = DataType.Number;
      TpActionToEventMap.TryAdd(actionId, new EventMappingRecord(ActionId));
    }

    public bool TryAddSimEventMapping(string actionKey, uint eventValue, string actionId) {
      return TpActionToEventMap.TryAdd(actionKey, new EventMappingRecord(actionId, eventValue));
    }

    public bool TryAddPluginEventMapping(string actionKey, PluginActions eventId, string actionId) {
      return TpActionToEventMap.TryAdd(actionKey, new EventMappingRecord(actionId, eventId));
    }

    // Get a unique event ID for this action, possibly based on data values
    // in the \c data array. Certain combination of values, eg. from choices,
    // may have their own unique events. Returns `false` if the lookup fails.
    public bool TryGetEventMapping(IEnumerable<string> values, out EventMappingRecord eventRecord)
    {
      int count = TpActionToEventMap.Count;
      if (count == 0) {
        eventRecord = null;
        return false;
      }
      if (count == 1) {
        eventRecord = TpActionToEventMap.Values.First();
        return true;
      }
      return TpActionToEventMap.TryGetValue(FormatLookupKey(values), out eventRecord);
    }

    public bool TryGetEventMapping(string value, out EventMappingRecord eventRecord) =>
      TryGetEventMapping(new string[] { value }, out eventRecord);

    public EventMappingRecord GetEventMapping()
    {
      return TpActionToEventMap.Any() ? TpActionToEventMap.Values.First() : null;
    }

    // Helper to format an array of action data values into a unique key
    // used for indexing the TpActionToEventMap dictionary.
    public string FormatLookupKey(IEnumerable<string> values) {
      if (string.IsNullOrWhiteSpace(KeyFormatStr))
        return string.Empty;
      try { return string.Format(KeyFormatStr, values.ToArray()); }
      catch { return string.Empty; }
    }

  }
}
