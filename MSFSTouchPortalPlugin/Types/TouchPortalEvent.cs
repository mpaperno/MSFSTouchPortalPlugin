using MSFSTouchPortalPlugin.Enums;
using System.Collections.Generic;
using System.Linq;

namespace MSFSTouchPortalPlugin.Types
{
  public class TouchPortalEvent
  {
    // Touch Portal properties
    public string Id { get; set; }
    public string Name { get; set; }
    public string Format { get; set; }
    public string Type { get; set; } = "communicate";
    public string ValueType => DataType.ToString().ToLower();
    public string ValueStateId { get; set; }
    public string[] ValueChoices {
      get => _choices;
      set {
        _choices = value;
        _mappings = null;
      }
    }

    // Plugin-specific properties, etc.

    public DataType DataType { get; set; } = DataType.Choice;

    public Dictionary<EventIds, string> ChoiceMappings
    {
      get => _mappings;
      set {
        _mappings = value;
        _choices = value?.Values.ToArray();
      }
    }

    string[] _choices = null;
    Dictionary<EventIds, string> _mappings = null;

    public TouchPortalEvent(string id, string name, string format, string stateId = null) {
      Id = id;
      Name = name;
      Format = format;
      ValueStateId = stateId ?? id;
    }

    public TouchPortalEvent(string id, string name, string format, string[] choices, string stateId = null) : this(id, name, format, stateId) {
      ValueChoices = choices;
    }

    public TouchPortalEvent(string id, string name, string format, Dictionary<EventIds, string> mappings, string stateId = null) : this(id, name, format, stateId) {
      ChoiceMappings = mappings;
    }
  }

}
