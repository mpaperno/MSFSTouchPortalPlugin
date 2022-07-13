/*
This file is part of the MSFS Touch Portal Plugin project.
https://github.com/mpaperno/MSFSTouchPortalPlugin

COPYRIGHT:
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

using MSFSTouchPortalPlugin.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DisplayAttribute = System.ComponentModel.DataAnnotations.DisplayAttribute;

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

    public Dictionary<Enum, string> ChoiceMappings
    {
      get => _mappings;
      set {
        _mappings = value;
        _choices = value?.Values.ToArray();
      }
    }

    string[] _choices = null;
    Dictionary<Enum, string> _mappings = null;

    /// <summary>
    /// Create an event with no choices and optionally a specific DataType.
    /// If stateId is null then the id parameter is used.
    /// </summary>
    public TouchPortalEvent(string id, string name, string format, string stateId = null, DataType type = DataType.Choice) {
      Id = id;
      Name = name;
      Format = format;
      ValueStateId = stateId ?? id;
      DataType = type;
    }

    /// <summary>
    /// Create an event of Choice type with given array of choice text values.
    /// If stateId is null then the id parameter is used.
    /// </summary>
    public TouchPortalEvent(string id, string name, string format, string[] choices, string stateId = null)
      : this(id, name, format, stateId)
    {
      ValueChoices = choices;
    }

    /// <summary>
    /// Create an event of Choice type with given dict of Enum ids to TP choice name mappings.
    /// If stateId is null then the id parameter is used.
    /// </summary>
    public TouchPortalEvent(string id, string name, string format, Dictionary<Enum, string> mappings, string stateId = null)
      : this(id, name, format, stateId)
    {
      ChoiceMappings = mappings;
    }

    /// <summary>
    /// Create an event of Choice type with event mappings automatically generated based on DisplayAttribute.Name properties applied to eventIds enum values.
    /// If stateId is null then the id parameter is used.
    /// </summary>
    public TouchPortalEvent(string id, string name, string format, IEnumerable<Enum> eventIds, string stateId = null)
      : this(id, name, format, stateId)
    {
      SetMappingsFromEventIds(eventIds);
    }

    /// <summary>
    /// Returns a dictionary mapping event names (as they would appear in TP UI) to longer descriptions based on DisplayAttribute.Description properties applied to enum values,
    /// for doc/UI purposes. Returns an empty dict if no mappings, or no applied DisplayAttributes, were found.
    /// </summary>
    public IReadOnlyDictionary<string, string> GetEventDescriptions() {
      Dictionary<string, string> ret = new();
      if (_mappings == null || !_mappings.Any())
        return ret;
      var enumType = _mappings.Keys.First().GetType();
      foreach (var map in _mappings) {
        if (TryGetEnumMetaData(enumType, map.Key, out _, out var descript))
          ret.Add(map.Value, descript);
      }
      return ret;
    }

    void SetMappingsFromEventIds(IEnumerable<Enum> eventIds) {
      ChoiceMappings = new();
      if (!eventIds.Any())
        return;
      var enumType = eventIds.First().GetType();
      foreach (var id in eventIds) {
        if (TryGetEnumMetaData(enumType, id, out var name, out _))
          _mappings.Add(id, name);
      }
      _choices = _mappings.Values.ToArray();
    }

    static bool TryGetEnumMetaData(Type enumType, Enum member, out string name, out string descript) {
      if (enumType.GetMember(member.ToString()).First()?.GetCustomAttribute<DisplayAttribute>() is var attr && attr != null) {
        name = attr.Name;
        descript = attr.Description;
        return true;
      }
      name = descript = null;
      return false;
    }

  }
}
