using Microsoft.Extensions.Logging;
using MSFSTouchPortalPlugin.Configuration.HubHop;
using MSFSTouchPortalPlugin.Enums;
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
    public string OrderyBy;

    public HubHopPresetQuery(HubHopType type, string vendor, string aircraft, string system, string label, string orderyBy = null) {
      Type = type;
      Vendor = vendor;
      Aircraft = aircraft;
      System = system;
      Label = label;
      OrderyBy = orderyBy;
    }
    public HubHopPresetQuery() : this(HubHopType.Any) { }
    public HubHopPresetQuery(HubHopType type) : this(type, null) { }
    public HubHopPresetQuery(HubHopType type, string vendor) : this(type, vendor, null) { }
    public HubHopPresetQuery(HubHopType type, string vendor, string aircraft) : this(type, vendor, aircraft, null, null) { }
    public HubHopPresetQuery(HubHopType type, string vendor, string aircraft, string system) : this(type, vendor, aircraft, system, null) { }
  }

  internal class HubHopPresetsCollection : IDisposable
  {
    #region Public

    public HubHopPresetsCollection(string dbfile = default) {
      // must open in ReadWrite otherwise it doesn't close properly if an async connection was used.... :-|
      try {
        _db = new SQLiteConnection(dbfile == default ? Common.PresetsDb : dbfile, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache);
      }
      catch (Exception e) {
        Common.Logger?.LogCritical(e, $"Unable to open database {(dbfile == default ? Common.PresetsDb : dbfile)}: {e.Message}");
      }
    }

    public DateTime LatestUpdateTime => DateTimeOffset.FromUnixTimeSeconds(_db.ExecuteScalar<long>("SELECT MAX(CreatedDateTS) FROM HubHopPreset")).UtcDateTime;

    public async Task UpdateIfNeededAsync() {
      if (_db == null)
        return;
      try {
        var last = LatestUpdateTime;
        bool updt = await HubHopDataUpdater.CheckForUpdates(last).ConfigureAwait(false);
        if (!updt) {
          Common.Logger?.LogInformation((int)EventIds.PluginInfo, "No HubHop Updates Detected; Latest entry date: " + LatestUpdateTime.ToString("u"));
          return;
        }
        if (await HubHopDataUpdater.DownloadPresets(Common.PresetsFile).ConfigureAwait(false)) {
          if (await LoadAsync().ConfigureAwait(false)) {
            File.Delete(Common.PresetsFile);
            Common.Logger?.LogInformation((int)EventIds.PluginInfo, "HubHop Data updated; Latest entry date: " + LatestUpdateTime.ToString("u"));
          }
          else {
            Common.Logger?.LogWarning("HubHop Updates database load failed.");
          }
        }
        else {
          Common.Logger?.LogWarning("HubHop Updates download failed.");
        }
      }
      catch (Exception e) {
        Common.Logger?.LogError(e, $"Update check failed with Exception: {e.Message}");
      }
    }

    public async Task<bool> LoadAsync(string jsonFile = default) {
      if (_db == null || !TryLoadJson(jsonFile == default ? Common.PresetsFile : jsonFile, out JToken presets))
        return false;
      var db = new SQLiteAsyncConnection(_db.DatabasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache);
      await
        db.RunInTransactionAsync((t) => { Load(presets, t); })
        .ContinueWith((t) => { db.CloseAsync(); });
      //Debug();
      return true;
    }

    public bool Load(string jsonFile = default) {
      if (_db != null && TryLoadJson(jsonFile == default ? Common.PresetsFile : jsonFile, out JToken presets)) {
        _db.RunInTransaction(() => { Load(presets, _db); });
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
      criteria.OrderyBy = "Vendor, Aircraft";
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
      if (string.IsNullOrWhiteSpace(criteria.OrderyBy))
        criteria.OrderyBy = "Label";
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

    readonly SQLiteConnection _db;

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
      if (qry.OrderyBy == null)
        qry.OrderyBy = fieldName;
      if (cond.Count > 0)
        where = "WHERE " + string.Join(" AND ", cond);
      Common.Logger?.LogTrace($"\"{string.Format(_selectTemplate, fieldName, where, qry.OrderyBy)}\"  ?= {{{string.Join(", ", args)}}}");
      return new Tuple<string, object[]>(string.Format(_selectTemplate, fieldName, where, qry.OrderyBy), args.ToArray());
    }

    static bool TryLoadJson(string filename, out JToken presets) {
      presets = new JArray();
      var serializer = JsonSerializer.Create(Common.SerializerSettings);
      try {
        using (StreamReader file = File.OpenText(filename)) {
          presets = (JToken)serializer.Deserialize(file, typeof(JToken));
        }
        if (presets.Type == JTokenType.Array)
          return true;
        else
          Common.Logger?.LogError($"Unable to load {filename}, not a JSON array.");
      }
      catch (Exception e) {
        Common.Logger?.LogError(e, $"Unable to load {filename}: {e.Message}");
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
            Common.Logger?.LogWarning($"Error in JSON element {p}: Was null or had no ID or invalid Version.");
        }
        catch (Exception e) {
          Common.Logger?.LogError(e, $"Error deserializing element {p}: {e.Message}");
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
        Common.Logger?.LogError(e, $"Database error with {p}: {e.Message}");
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

#if false
SQLITE_OPEN_READONLY         0x00000001  /* Ok for sqlite3_open_v2() */
SQLITE_OPEN_READWRITE        0x00000002  /* Ok for sqlite3_open_v2() */
SQLITE_OPEN_CREATE           0x00000004  /* Ok for sqlite3_open_v2() */
SQLITE_OPEN_DELETEONCLOSE    0x00000008  /* VFS only */
SQLITE_OPEN_EXCLUSIVE        0x00000010  /* VFS only */
SQLITE_OPEN_AUTOPROXY        0x00000020  /* VFS only */
SQLITE_OPEN_URI              0x00000040  /* Ok for sqlite3_open_v2() */
SQLITE_OPEN_MEMORY           0x00000080  /* Ok for sqlite3_open_v2() */
SQLITE_OPEN_MAIN_DB          0x00000100  /* VFS only */
SQLITE_OPEN_TEMP_DB          0x00000200  /* VFS only */
SQLITE_OPEN_TRANSIENT_DB     0x00000400  /* VFS only */
SQLITE_OPEN_MAIN_JOURNAL     0x00000800  /* VFS only */
SQLITE_OPEN_TEMP_JOURNAL     0x00001000  /* VFS only */
SQLITE_OPEN_SUBJOURNAL       0x00002000  /* VFS only */
SQLITE_OPEN_SUPER_JOURNAL    0x00004000  /* VFS only */
SQLITE_OPEN_NOMUTEX          0x00008000  /* Ok for sqlite3_open_v2() */
SQLITE_OPEN_FULLMUTEX        0x00010000  /* Ok for sqlite3_open_v2() */
SQLITE_OPEN_SHAREDCACHE      0x00020000  /* Ok for sqlite3_open_v2() */
SQLITE_OPEN_PRIVATECACHE     0x00040000  /* Ok for sqlite3_open_v2() */
SQLITE_OPEN_WAL              0x00080000  /* VFS only */
SQLITE_OPEN_NOFOLLOW         0x01000000  /* Ok for sqlite3_open_v2() */
SQLITE_OPEN_EXRESCODE        0x02000000  /* Extended result codes */
#endif
