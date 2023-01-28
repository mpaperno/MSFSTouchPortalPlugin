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

using Microsoft.Extensions.Logging;
using MSFSTouchPortalPlugin.Configuration.HubHop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MSFSTouchPortalPlugin.Configuration
{
  internal class HubHopPresetQuery
  {
    public HubHopType Type;
    public string Vendor;
    public string Aircraft;
    public string System;
    public string Label;
    public string OrderBy;

    public HubHopPresetQuery(HubHopType type, string vendor, string aircraft, string system, string label, string orderBy = null) {
      Type = type;
      Vendor = vendor;
      Aircraft = aircraft;
      System = system;
      Label = label;
      OrderBy = orderBy;
    }
    public HubHopPresetQuery() : this(HubHopType.Any) { }
    public HubHopPresetQuery(HubHopType type) : this(type, null) { }
    public HubHopPresetQuery(HubHopType type, string vendor) : this(type, vendor, null) { }
    public HubHopPresetQuery(HubHopType type, string vendor, string aircraft) : this(type, vendor, aircraft, null, null) { }
    public HubHopPresetQuery(HubHopType type, string vendor, string aircraft, string system) : this(type, vendor, aircraft, system, null) { }
  }

  internal delegate void HubHupDataUpdaEventHandler(bool updated);
  internal delegate void HubHupDataErrorEventHandler(LogLevel severity, string message);

  internal class HubHopPresetsCollection : IDisposable
  {
    #region Public

    public event HubHupDataUpdaEventHandler OnDataUpdateEvent;
    public event HubHupDataErrorEventHandler OnDataErrorEvent;
    /// <summary> Returns the UTC update date/time of latest database entry, or zero if the database is empty or not loaded. </summary>
    public DateTime LatestUpdateTime => DateTimeOffset.FromUnixTimeSeconds(_db?.ExecuteScalar<long>("SELECT MAX(CreatedDateTS) FROM HubHopPreset") ?? 0).UtcDateTime;

    /// <summary> Opens the specified database file for reading and writing HubHop Data. The database must have the HubHopPreset table. Errors are reported via `OnDataErrorEvent` event handler. </summary>
    /// <returns>True on success, false on failure.</returns>
    public bool OpenDataFile(string dbfile = default)
    {
      if (dbfile == default)
        dbfile = Common.PresetsDb;
      try {
        // must open in ReadWrite otherwise it doesn't close properly if an async connection was used.... :-|
        _db = new SQLiteConnection(dbfile, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache);
        Common.Logger?.LogDebug("HubHopPresetsCollection: opened database: {file}", dbfile);
        return true;
      }
      catch (Exception e) {
        OnDataErrorEvent?.Invoke(LogLevel.Critical, $"Unable to open database {dbfile}: {e.Message}");
        Common.Logger?.LogCritical(e, "HubHopPresetsCollection::OpenDataFile() - Cannot open database with Exception:");
      }
      return false;
    }

    /// <summary> Asynchronously checks for latest HubHop preset and updates the database if needed. Status is "returned" using `OnDataUpdateEvent` or `OnDataErrorEvent`  event handlers. </summary>
    /// <param name="downloadTimeoutSec">Timeout value for the update check and download process, in seconds.</param>
    public async Task UpdateIfNeededAsync(int downloadTimeoutSec = 60)
    {
      if (_db == null)
        return;
      try {
        bool updt = await HubHopDataUpdater.CheckForUpdates(LatestUpdateTime, downloadTimeoutSec).ConfigureAwait(false);
        if (!updt) {
          OnDataUpdateEvent?.Invoke(false);
          return;
        }
        if (await HubHopDataUpdater.DownloadPresets(Common.PresetsFile, downloadTimeoutSec).ConfigureAwait(false)) {
          if (await LoadAsync().ConfigureAwait(false)) {
            File.Delete(Common.PresetsFile);
            OnDataUpdateEvent?.Invoke(true);
          }
          else {
            OnDataErrorEvent?.Invoke(LogLevel.Error, "HubHop Updates database load failed, check log.");
          }
        }
        else {
          OnDataErrorEvent?.Invoke(LogLevel.Warning, "HubHop Updates download failed, check log.");
        }
      }
      catch (Exception e) {
        OnDataErrorEvent?.Invoke(LogLevel.Error, $"Update check failed with Exception: {e.Message}");
        Common.Logger?.LogError(e, "HubHopPresetsCollection::UpdateIfNeededAsync() - Update check Exception:");
      }
    }

    /// <summary> Asynchronously loads data from a JSON file of HubHop presets, updating or inserting entries as needed. Errors are reported via `OnDataErrorEvent` event handler. </summary>
    /// <returns>Returns false before async operation if JSON parsing fails, otherwise returns true after completion.</returns>
    public async Task<bool> LoadAsync(string jsonFile = default)
    {
      if (_db == null || !TryLoadJson(jsonFile == default ? Common.PresetsFile : jsonFile, out JToken presets))
        return false;
      var db = new SQLiteAsyncConnection(_db.DatabasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache);
      await
        db.RunInTransactionAsync((t) => { Load(presets, t); })
        .ContinueWith((t) => { db.CloseAsync(); });
      //Debug();
      //_db.Execute("VACUUM");
      return true;
    }

    /// <summary> Synchronously loads data from a JSON file of HubHop presets, updating or inserting entries as needed. Errors are reported via `OnDataErrorEvent` event handler. </summary>
    /// <returns>Returns false if JSON parsing fails, otherwise returns true after completion.</returns>
    public bool Load(string jsonFile = default)
    {
      if (_db != null && TryLoadJson(jsonFile == default ? Common.PresetsFile : jsonFile, out JToken presets)) {
        _db.RunInTransaction(() => { Load(presets, _db); });
        //_db.Execute("VACUUM");
        return true;
      }
      return false;
    }

    public List<string> Vendors(HubHopType type = HubHopType.Any) {
      return Vendors(new HubHopPresetQuery(type));
    }
    public List<string> Vendors(HubHopPresetQuery criteria) {
      return QueryStringScalars("Vendor", criteria);
    }

    public List<string> Aircraft(HubHopType type = HubHopType.Any, string vendor = null) {
      return Aircraft(new HubHopPresetQuery(type, vendor));
    }
    public List<string> Aircraft(HubHopPresetQuery criteria) {
      return QueryStringScalars("Aircraft", criteria);
    }

    public List<string> VendorAircraft(HubHopType type = HubHopType.Any) {
      return VendorAircraft(new HubHopPresetQuery(type));
    }
    public List<string> VendorAircraft(HubHopPresetQuery criteria) {
      criteria.OrderBy = "Vendor, Aircraft";
      return QueryStringScalars("Vendor || ' - ' || Aircraft AS VendorAircraft", criteria);
    }

    public List<string> Systems(HubHopType type = HubHopType.Any, string aircraft = null, string vendor = null) {
      return Systems(new HubHopPresetQuery(type, vendor, aircraft));
    }
    public List<string> Systems(HubHopPresetQuery criteria) {
      return QueryStringScalars("System", criteria);
    }

    public List<string> SystemsByVendorAircraft(string vendorAircraft, HubHopType type = HubHopType.Any) {
      var va = SplitVendorAircraft(vendorAircraft);
      return Systems(type, va.Item1, va.Item2);
    }

    public List<string> Names(HubHopType type = HubHopType.Any, string aircraft = null, string system = null, string vendor = null) {
      return Names(new HubHopPresetQuery(type, vendor, aircraft, system));
    }
    public List<string> Names(HubHopPresetQuery criteria) {
      return QueryStringScalars("Label", criteria);
    }

    public List<HubHopPreset> Presets(HubHopType type = HubHopType.Any, string aircraft = null, string system = null, string vendor = null) {
      return Presets(new HubHopPresetQuery(type, vendor, aircraft, system));
    }

    public List<HubHopPreset> Presets(HubHopPresetQuery criteria) {
      if (string.IsNullOrWhiteSpace(criteria.OrderBy))
        criteria.OrderBy = "Label";
      var qry = BuildQuery("*", criteria);
      return _db?.Query<HubHopPreset>(qry.Item1, qry.Item2);
    }

    public HubHopPreset Preset(HubHopPresetQuery criteria) {
      var p = Presets(criteria);
      return p?.Count > 0 ? p[0] : null;
    }

    public HubHopPreset Preset(string id) {
      return _db?.Get<HubHopPreset>(id);
    }

    public HubHopPreset PresetByVendorAircraft(string vendorAircraft, HubHopType type = HubHopType.Any, string system = null) {
      var va = SplitVendorAircraft(vendorAircraft);
      return Preset(new HubHopPresetQuery(type, va.Item1, va.Item2, system));
    }

    public HubHopPreset PresetByName(string name, HubHopType type = HubHopType.Any, string vendorAircraft = null, string system = null) {
      var va = SplitVendorAircraft(vendorAircraft);
      return Preset(new HubHopPresetQuery(type, va.Item1, va.Item2, system, name));
    }

    public static Tuple<string, string> SplitVendorAircraft(string vendorAircraft) {
      if (string.IsNullOrWhiteSpace(vendorAircraft))
        return Tuple.Create(string.Empty, string.Empty);
      var idx = vendorAircraft.IndexOf(" - ", StringComparison.InvariantCulture);
      if (idx < 0)
        return Tuple.Create(vendorAircraft, string.Empty);
      return Tuple.Create(vendorAircraft[0..idx], vendorAircraft[(idx + 3)..]);
    }

    public void Dispose() {
      ((IDisposable)_db)?.Dispose();  // same as Close()
      GC.SuppressFinalize(this);
    }

    #endregion Public

    #region Private

    SQLiteConnection _db = null;

    List<string> QueryStringScalars(string fieldName, HubHopPresetQuery qry) {
      var res = BuildQuery(fieldName, qry);
      return _db?.QueryScalars<string>(res.Item1, res.Item2);
    }

    const string _selectTemplate = "SELECT DISTINCT {0} FROM HubHopPreset {1} ORDER BY {2}";

    static Tuple<string, object[]> BuildQuery(string fieldName, HubHopPresetQuery qry) {
      List<string> cond = new(1);
      List<object> args = new(1);
      string where = string.Empty;
      if (qry.Type != HubHopType.Any) {
        cond.Add("(PresetType & ?) > 0");
        args.Add(qry.Type);
      }
      if (!string.IsNullOrEmpty(qry.Vendor)) {
        cond.Add("Vendor = ?");
        args.Add(qry.Vendor);
      }
      if (!string.IsNullOrEmpty(qry.Aircraft)) {
        cond.Add("Aircraft = ?");
        args.Add(qry.Aircraft);
      }
      if (!string.IsNullOrEmpty(qry.System)) {
        cond.Add("System = ?");
        args.Add(qry.System);
      }
      if (!string.IsNullOrEmpty(qry.Label)) {
        cond.Add("Label = ?");
        args.Add(qry.Label);
      }
      if (qry.OrderBy == null)
        qry.OrderBy = fieldName;
      if (cond.Count > 0)
        where = "WHERE " + string.Join(" AND ", cond);
      //Common.Logger?.LogTrace("\"{qry}\" ?= {{{args}}}", string.Format(_selectTemplate, fieldName, where, qry.OrderBy), string.Join(", ", args));
      return new Tuple<string, object[]>(string.Format(_selectTemplate, fieldName, where, qry.OrderBy), args.ToArray());
    }

    bool TryLoadJson(string filename, out JToken presets) {
      presets = new JArray();
      var serializer = JsonSerializer.Create(Common.SerializerSettings);
      try {
        using (StreamReader file = File.OpenText(filename)) {
          presets = (JToken)serializer.Deserialize(file, typeof(JToken));
        }
        if (presets.Type == JTokenType.Array)
          return true;
        else
          OnDataErrorEvent?.Invoke(LogLevel.Error, $"Unable to load {filename}, not a JSON array.");
      }
      catch (Exception e) {
        OnDataErrorEvent?.Invoke(LogLevel.Error, $"Exception on trying to load '{filename}': {e.Message}");
        Common.Logger?.LogError(e, "HubHopPresetsCollection::TryLoadJson() - Exception:");
      }
      return false;
    }

    static void Load(JToken presets, SQLiteConnection db) {
      var serializer = JsonSerializer.Create(Common.SerializerSettings);
      foreach (var p in presets) {
        try {
          var hhp = p.ToObject<HubHopPreset>(serializer);
          if (hhp != null && !string.IsNullOrWhiteSpace(hhp.Id) && hhp.Version > 0)
            InsertOrUpdate(hhp, db);
          else
            Common.Logger?.LogWarning("Error in JSON element {p}: Was null or had no ID or invalid Version.", p.ToString());
        }
        catch (Exception e) {
          Common.Logger?.LogError("Error deserializing element {p}: {message}", p.ToString(), e.Message);
        }
      }
    }

    static void InsertOrUpdate(HubHopPreset p, SQLiteConnection db) {
      bool updt;
      try {
        var current = db.Find<HubHopPreset>(p.Id);
        if (updt = current == null)
          db.Insert(p, typeof(HubHopPreset));
        else if (updt = current.Version != p.Version)
          db.Update(p, typeof(HubHopPreset));

        if (updt || current?.LastDbUpdate == 0) {
          if (!DateTime.TryParse(p.CreatedDate, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out var cdts))
            cdts = DateTime.UtcNow;
          db.Execute(
            "UPDATE HubHopPreset SET LastDbUpdate = ?, CreatedDateTS = ? WHERE Id = ?",
            new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(), new DateTimeOffset(cdts).ToUnixTimeSeconds(), p.Id
          );
        }
      }
      catch (Exception e) {
        Common.Logger?.LogError(e, "Database error with {p}: {message}", p.ToString(), e.Message);
      }
    }

#if false
    void Debug() {
      string qry = "SELECT * FROM HubHopPreset ORDER BY CreatedDate LIMIT 10";
      var results = _db.Query<HubHopPreset>(qry);

      foreach (var r in results) {
        Common.Logger?.LogDebug(JsonConvert.SerializeObject(r, Common.SerializerSettings));
      }

      Common.Logger?.LogDebug(string.Join(", ", Vendors(HubHopType.AllInputs)));
      Common.Logger?.LogDebug(string.Join(", ", Aircraft(HubHopType.Any)));
      Common.Logger?.LogDebug(string.Join(", ", Aircraft(HubHopType.Any, "Microsoft")));
      Common.Logger?.LogDebug(string.Join(", ", Systems(HubHopType.Any)));
      Common.Logger?.LogDebug(string.Join(", ", Systems(HubHopType.Output, "Bell 47 G")));
      Common.Logger?.LogDebug(string.Join(", ", Names(HubHopType.Output, "Bell 47 G", "Lights")));
      Common.Logger?.LogDebug(string.Join(", ", Presets(HubHopType.Output, "Bell 47 G", "Lights")));
      Common.Logger?.LogDebug(Preset("556e65e7-3af9-4c4c-b282-c7f4bb6a9453").ToString());
      Common.Logger?.LogDebug(string.Join(", ", VendorAircraft(HubHopType.Any)));
    }
#endif

    #endregion Private
  }
}
