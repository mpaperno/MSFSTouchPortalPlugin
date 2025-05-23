﻿/*
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

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MSFSTouchPortalPlugin.Enums;

namespace MSFSTouchPortalPlugin.Types
{

  internal delegate void SimVarAddedHandler(SimVarItem simVar);
  internal delegate void SimVarRemovedHandler(SimVarItem simVar);

  /// <summary>
  /// Manages the main collection of SimVarItems for TP States and provides indexed lookup by several properties (Definition, Id, SimVarName)
  /// as well as maintaining some reference lists of settable and polled-update SimVars.
  /// It provides an enumerable collection of SimVarItems as well as Dictionary-style behavior.
  /// </summary>
  [DebuggerDisplay("Count = {Count}")]
  internal /*sealed*/ class SimVarCollection : ICollection<SimVarItem>, ICollection, IDictionary<Definition, SimVarItem>, IReadOnlyDictionary<Definition, SimVarItem>
  {
    internal event SimVarAddedHandler OnSimVarAdded;
    internal event SimVarRemovedHandler OnSimVarRemoved;

    public bool IsEmpty => _idxByDef.IsEmpty;
    public int Count => _idxByDef.Count;
    public IEnumerable<Definition> Keys => ((IReadOnlyDictionary <Definition, SimVarItem>)_idxByDef).Keys;
    public IEnumerable<SimVarItem> Values => ((IReadOnlyDictionary <Definition, SimVarItem>)_idxByDef).Values;
    ICollection<Definition> IDictionary<Definition, SimVarItem>.Keys => ((IDictionary<Definition, SimVarItem>)_idxByDef).Keys;
    ICollection<SimVarItem> IDictionary<Definition, SimVarItem>.Values => ((IDictionary<Definition, SimVarItem>)_idxByDef).Values;
    public bool IsReadOnly => false;
    object ICollection.SyncRoot => _idxByDef;
    bool ICollection.IsSynchronized => false;

    public IReadOnlyList<SimVarItem> PolledUpdateVars {
      get {
        int cnt = _polledUpdateVars.Count;
        if (cnt == 0)
          return Array.Empty<SimVarItem>();
        SimVarItem[] list = new SimVarItem[cnt];
        lock (_polledUpdateVars)
          _polledUpdateVars.CopyTo(list);
        return list;
      }
    }

    // List of vars which were not read from default config file.
    public IEnumerable<SimVarItem> CustomVariables =>
      (from SimVarItem s in _idxByDef.Values where (s.DefinitionSource == SimVarDefinitionSource.UserFile || s.DefinitionSource == SimVarDefinitionSource.Dynamic) select s);
    // List of all vars except temporary/internal use ones.
    public IEnumerable<SimVarItem> RequestedVariables =>
      (from SimVarItem s in _idxByDef.Values where s.DefinitionSource != SimVarDefinitionSource.Temporary select s);
    // List of all SimVar (A) types that have a category
    public IEnumerable<string> SimVarNames =>
      (from SimVarItem s in _idxByDef.Values where (s.VariableType == 'A' && s.CategoryId != Groups.None) select s.SimVarName);
    // List of all variables loaded from a particular file.
    public IEnumerable<SimVarItem> VariablesFromFile(string file) =>
      (from SimVarItem s in _idxByDef.Values where (s.DefinitionSourceFile == file) select s);
    // List of all variables by type, excluding temporary vars..
    public IEnumerable<SimVarItem> VariablesOfType(char type) =>
      (from SimVarItem s in _idxByDef.Values where (s.VariableType == type && s.DefinitionSource != SimVarDefinitionSource.Temporary) select s);

    const int MAX_CONCURRENCY = 6;
    const int INITIAL_CAPACITY = 500;

    // Primary storage by enum value; concurrent since may be used from both the main thread and when data arrives asynchronously from SimConnect; indexed by Definition for quickest lookup when data arrives.
    readonly ConcurrentDictionary<Definition, SimVarItem> _idxByDef = new(MAX_CONCURRENCY, INITIAL_CAPACITY);
    readonly Dictionary<string, SimVarItem> _idxById = new(INITIAL_CAPACITY);       // index by string id
    readonly Dictionary<string, SimVarItem> _idxBySimName = new(INITIAL_CAPACITY);  // index by SimVarName
    readonly Dictionary<Groups, List<Definition>> _idxByCategory = new(INITIAL_CAPACITY);  // index by Category Id
    readonly List<SimVarItem> _polledUpdateVars = new();                            // list of all vars needing request polling, protected by lock

    SimVarItem IDictionary<Definition, SimVarItem>.this[Definition def]
    {
      get {
        try { return _idxByDef[def]; }
        catch { return null; }
      }
      set {
        Remove(def);
        Add(value);
      }
    }

    SimVarItem IReadOnlyDictionary<Definition, SimVarItem>.this[Definition def] {
      get {
        try   { return ((IReadOnlyDictionary<Definition, SimVarItem>)_idxByDef)[def]; }
        catch { return null; }
      }
    }

    public SimVarItem this[string id]
    {
      get {
        try { return _idxById[id]; }
        catch { return null; }
      }
      set {
        Remove(id);
        Add(value);
      }
    }

    public SimVarItem Get(Definition def) {
      return ((IDictionary<Definition, SimVarItem>)this)[def];
    }

    public SimVarItem Get(string id) {
      return this[id];
    }

    public bool TryGet(Definition def, [MaybeNullWhen(false)] out SimVarItem obj) {
      return _idxByDef.TryGetValue(def, out obj);
    }

    public bool TryGet(string id, [MaybeNullWhen(false)] out SimVarItem obj) {
      return _idxById.TryGetValue(id, out obj);
    }

    // IDictionary compat
    public bool TryGetValue(Definition key, [MaybeNullWhen(false)] out SimVarItem item) {
      return TryGet(key, out item);
    }

    public SimVarItem GetBySimName(string simVarName) {
      try   { return _idxBySimName[simVarName]; }
      catch { return null; }
    }

    public bool TryGetBySimName(string simVarName, [MaybeNullWhen(false)] out SimVarItem item) {
      item = GetBySimName(simVarName);
      return item != null;
    }

    public bool ContainsKey(Definition key) {
      return ((IReadOnlyDictionary<Definition, SimVarItem>)_idxByDef).ContainsKey(key);
    }

    public bool Contains(SimVarItem item) => item != null && ContainsKey(item.Def);
    public bool Contains(Definition key) => ContainsKey(key);
    public bool Contains(string id) => _idxById.ContainsKey(id);
    public bool Contains(KeyValuePair<Definition, SimVarItem> pair) => ContainsKey(pair.Key);

    public void Add(SimVarItem item) {
      if (item == null)
        return;
      _idxByDef[item.Def] = item;
      _idxById[item.Id] = item;
      _idxBySimName[item.SimVarName] = item;
      if (!_idxByCategory.ContainsKey(item.CategoryId))
        _idxByCategory.Add(item.CategoryId, new());
      _idxByCategory[item.CategoryId].Add(item.Def);
      if (item.NeedsScheduledRequest) {
        lock (_polledUpdateVars)
          _polledUpdateVars.Add(item);
      }
      OnSimVarAdded?.Invoke(item);
    }

    // IDictionary
    public void Add(Definition key, SimVarItem value) => Add(value);
    // IDictionary
    public void Add(KeyValuePair<Definition, SimVarItem> pair) => Add(pair.Value);

    public bool TryAdd(SimVarItem item) {
      if (Contains(item))
        return false;
      Add(item);
      return true;
    }

    public bool Remove(SimVarItem item) {
      if (item == null)
        return false;
      bool ret = _idxByDef.Remove(item.Def, out _);
      _idxById.Remove(item.Id, out _);
      _idxBySimName.Remove(item.SimVarName, out _);
      if (_idxByCategory.ContainsKey(item.CategoryId))
        _idxByCategory[item.CategoryId].Remove(item.Def);
      lock (_polledUpdateVars)
        _polledUpdateVars.Remove(item);
      OnSimVarRemoved?.Invoke(item);
      return ret;
    }

    public bool Remove(Definition def) => Remove(Get(def));
    public bool Remove(string id) => Remove(Get(id));
    bool ICollection<KeyValuePair<Definition, SimVarItem>>.Remove(KeyValuePair<Definition, SimVarItem> pair) => Remove(pair.Value);

    public void Clear() {
      _idxByDef.Clear();
      _idxById.Clear();
      _idxBySimName.Clear();
      _idxByCategory.Clear();
      lock (_polledUpdateVars)
        _polledUpdateVars.Clear();
    }

    public void RemoveFromPolled(SimVarItem item) {
      lock (_polledUpdateVars)
        _polledUpdateVars.Remove(item);
    }

    // Utility methods

    public IOrderedEnumerable<string> GetSimVarSelectorList(Groups categoryId)
    {
      if (!_idxByCategory.ContainsKey(categoryId))
        return new List<string>().OrderBy(n => n);
      List <Definition> cat = _idxByCategory[categoryId];
      List<string> list = new(cat.Count);
      foreach (Definition v in cat)
        if (Get(v) is var sv && sv != null)
          list.Add(sv.TouchPortalSelector);
      return list.OrderBy(n => n);
    }

    // Interface implementations

    // IEnumerable<SimVarItem>
    public IEnumerator<SimVarItem> GetEnumerator() => _idxByDef.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _idxByDef.Values.GetEnumerator();
    // IDictionary
    IEnumerator<KeyValuePair<Definition, SimVarItem>> IEnumerable<KeyValuePair<Definition, SimVarItem>>.GetEnumerator() => _idxByDef.GetEnumerator();

    public override bool Equals(object obj) => _idxByDef.Equals(obj);
    public override int GetHashCode() => _idxByDef.GetHashCode();
    public override string ToString() => _idxByDef.ToString();
    public void CopyTo(KeyValuePair<Definition, SimVarItem>[] array, int arrayIndex) => ((ICollection)_idxByDef).CopyTo(array, arrayIndex);
    public void CopyTo(SimVarItem[] array, int arrayIndex) => _idxByDef.Values.CopyTo(array, arrayIndex);
    public void CopyTo(Array array, int index) => ((ICollection)_idxByDef.Values).CopyTo(array, index);


#if false
    // Updating variable requests is currently more hassle than it seems to be worth, and is easy to get SimConnect to crash the whole sim.
    // Maybe revisit this some day.
    public bool AddOrUpdate(SimVarItem item)
    {
      if (item == null)
        return false;
      if (TryGet(item.Id, out var oldVar)) {
        Update(item, oldVar);
        return true;
      }
      Add(item);
      return false;
    }

    public void Update(SimVarItem newItem, SimVarItem oldItem)
    {
      if (newItem == null || oldItem == null)
        return;
      newItem.Def = oldItem.Def;  // keep the old definition ID
      newItem.DataProvider = oldItem.DataProvider;  // this lets SimConnectService know that the request is an update to an existing one
      newItem.RegistrationStatus = oldItem.RegistrationStatus;  // also keep the previous registration status to know if old version needs removal from provider.
      // check if category has changed
      if (newItem.CategoryId != oldItem.CategoryId && _idxByCategory.ContainsKey(oldItem.CategoryId))
        _idxByCategory[oldItem.CategoryId].Remove(oldItem.Def);
      // check if settable flag has changed
      // removed from polled vars list if no longer required
      if (oldItem.NeedsScheduledRequest && !newItem.NeedsScheduledRequest) {
        lock (_polledUpdateVars)
          _polledUpdateVars.Remove(oldItem);
      }
      Add(newItem);
    }
#endif

  }

}
