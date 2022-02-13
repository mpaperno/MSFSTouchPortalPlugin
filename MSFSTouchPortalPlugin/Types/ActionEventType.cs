using MSFSTouchPortalPlugin.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using TouchPortalExtension.Enums;

namespace MSFSTouchPortalPlugin.Types
{
  internal class ActionEventType
  {
    public bool InternalEvent = false;
    public Groups SimConnectGroup;
    public string ActionId;
    public int ValueIndex = -1;
    public string KeyFormatStr = string.Empty;
    public double MinValue = double.NaN;  // for basic validation of a single value
    public double MaxValue = double.NaN;
    public DataType ValueType = DataType.None;  // assuming single value here also

    // Mapping of TP actions to SimConnect or "Native" events. For SimConnect, the Enum is a generated number
    // of type SimEventClientId (but doesn't actually exist), and for internal plugin events its an actual
    // member of the MSFSTouchPortalPlugin.Objects.Plugin.Plugin enum.
    public readonly Dictionary<string, Enum> TpActionToEventMap = new();

    // possible future use
    //public object ActionObject = null;  // the member to which all action attributes are assigned to
    //public IReadOnlyCollection<TouchPortalActionDataAttribute> DataAttributes;  // list of all data type attributes

    // Get a unique event ID for this action, possibly based on data values
    // in the \c data array. Certain combination of values, eg. from choices,
    // may have their own unique events. Returns `false` if the lookup fails.
    public bool TryGetEventMapping(in string[] values, out Enum eventEnum) {
      if (TpActionToEventMap.Count == 1) {
        eventEnum = TpActionToEventMap.First().Value;
        return true;
      }
      return TpActionToEventMap.TryGetValue(FormatLookupKey(values), out eventEnum);
    }

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
