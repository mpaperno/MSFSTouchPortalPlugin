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

using Microsoft.FlightSimulator.SimConnect;
using MSFSTouchPortalPlugin.Types;
using System.Collections.Concurrent;

namespace MSFSTouchPortalPlugin.Types
{
  /// <summary>
  /// Represents a `SIMCONNECT_INPUT_EVENT_DESCRIPTOR` type, with params.
  /// https://docs.flightsimulator.com/html/Programming_Tools/SimConnect/API_Reference/Structures_And_Enumerations/SIMCONNECT_INPUT_EVENT_DESCRIPTOR.htm
  /// </summary>
  internal class SimInputEvent
  {
    /// <summary> The name of the Input Event. </summary>
    public string Name { get; set; }
    /// <summary> The hash ID for the event. </summary>
    public ulong Hash { get; set; }
    /// <summary> SIMCONNECT_INPUT_EVENT_TYPE:  NONE, DOUBLE, STRING </summary>
    public SIMCONNECT_INPUT_EVENT_TYPE Type { get; set; }
    /// <summary> Parameter types. </summary>
    public string Params { get; set; } = string.Empty;
    /// <summary> Stored ID of related SimVarItem when used for data requests. </summary>
    public Definition SimVarDef { get; set; } = Definition.None;

    public SimInputEvent(SIMCONNECT_INPUT_EVENT_DESCRIPTOR ev)
    {
      this.Name = ev.Name;
      this.Hash = ev.Hash;
      this.Type = ev.eType;
    }

    public static implicit operator string(SimInputEvent v) => v.Name;
    public static implicit operator ulong(SimInputEvent v) => v.Hash;
  }


  /// <summary>
  /// Represents a thread-safe Dictionary-style collection of SimInputEvents which are indexed by name and Hash ID.
  /// </summary>
  internal class SimInputEventCollection : ConcurrentDictionary<string, SimInputEvent>
  {
    readonly ConcurrentDictionary<ulong, SimInputEvent> idxByHash;

    public SimInputEventCollection(int concurrencyLevel = 2, int initialCapacity = 200) :
      base(concurrencyLevel, initialCapacity)
    {
      idxByHash = new(concurrencyLevel, initialCapacity);
    }

    public new SimInputEvent this[string name]
    {
      get => ((ConcurrentDictionary<string, SimInputEvent>)this)[name];
      set {
        if (value == null) {
          Remove(name);
        }
        else {
          ((ConcurrentDictionary<string, SimInputEvent>)this)[name] = value;
          idxByHash[value.Hash] = value;
        }
      }
    }

    public SimInputEvent this[ulong hash]
    {
      get => idxByHash[hash];
      set {
        if (value == null)
          Remove(hash);
        else
          this[value.Name] = value;
      }
    }

    public SimInputEvent Get(string name) { try { return this[name]; } catch { return null; } }
    public SimInputEvent Get(ulong hash) { try { return this[hash]; } catch { return null; } }

    public bool TryGetValue(ulong hash, out SimInputEvent obj) { return (obj = Get(hash)) != null; }

    public new bool TryAdd(string _, SimInputEvent item) => TryAdd(item);
    public bool TryAdd(ulong _, SimInputEvent item) => TryAdd(item);

    public bool TryAdd(SimInputEvent item)
    {
      if (item != null && ((ConcurrentDictionary<string, SimInputEvent>)this).TryAdd((string)item, item)) {
        idxByHash[item] = item;
        return true;
      }
      return false;
    }

    public bool Remove(string name) { return TryRemove(name, out var _); }
    public bool Remove(ulong hash) { return TryRemove(hash, out var _); }
    public bool TryRemove(SimInputEvent item, out SimInputEvent value) { return TryRemove(item?.Name, out value); }

    public new bool TryRemove(string name, out SimInputEvent value)
    {
      if (((ConcurrentDictionary<string, SimInputEvent>)this).TryRemove(name, out value))
        idxByHash.TryRemove(value, out _);
      return value != null;
    }

    public bool TryRemove(ulong hash, out SimInputEvent value)
    {
      if (idxByHash.TryRemove(hash, out value))
        ((ConcurrentDictionary<string, SimInputEvent>)this).TryRemove(value.Name, out value);
      return value != null;
    }

    public new void Clear()
    {
      ((ConcurrentDictionary<string, SimInputEvent>)this).Clear();
      idxByHash.Clear();
    }

  }

}
