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
using MSFSTouchPortalPlugin.Types;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using static SQLitePCL.raw;

namespace MSFSTouchPortalPlugin.Configuration
{
  public enum DocImportType
  {
    None, KeyEvent, SimVar, Unit
  }

  internal class DocImportQuery
  {
    public DocImportType Type;
    public string System = null;
    public string Category = null;
    public string Measure = null;
    public string Name = null;
    public string Alias = null;
    public bool? Settable = null;
    public bool Deprecated = true;
    public string OrderBy = null;
    public int Limit = -1;

    public DocImportQuery(DocImportType type, string system, string category, string name, bool? settable = null, string orderBy = null)
    {
      Type = type;
      System = system;
      Category = category;
      Name = name;
      Settable = settable;
      OrderBy = orderBy;
    }
    public DocImportQuery() : this(DocImportType.None) { }
    public DocImportQuery(DocImportType type) : this(type, null) { }
    public DocImportQuery(DocImportType type, string system) : this(type, system, null) { }
    public DocImportQuery(DocImportType type, string system, string category) : this(type, system, category, null, null) { }
    public DocImportQuery(DocImportType type, string system, string category, string name) : this(type, system, category, name, null) { }
    public DocImportQuery(string system, string category, string orderBy = null) : this(DocImportType.KeyEvent, system, category, null, null, orderBy) { }
    public DocImportQuery(string system, string category, bool? settable, string orderBy = null) : this(DocImportType.SimVar, system, category, null, settable, orderBy) { }
    public DocImportQuery(string measure) : this(DocImportType.Unit) { Measure = measure; }
  }

  internal delegate void DocImportsDataErrorEventHandler(LogLevel severity, string message);

  internal class DocImportsCollection : IDisposable
  {
    #region Public

    public event DocImportsDataErrorEventHandler OnDataErrorEvent;
#if FSX
    public static string SimulatorVersion = "MSFS_10";
#else
    public static string SimulatorVersion = "MSFS_12";
#endif
    public static string ImportsDbName { get; set; } = "MSFS_SDK_Doc_Import.sqlite3";
    public static string DataFolder { get => PluginConfig.AppConfigFolder; }
    public static string ImportsDb { get => Path.Combine(DataFolder, ImportsDbName); }
    public static int SelectorsMaxLineLen { get; set; } = 75;  // maximum characters to show in event/SimVar description lines

    public static ILogger Logger { get; set; } = null;

    /// <summary> Sets current simulator major version, used in queries to limit results to supported sim version. </summary>
    /// <returns>true if the version was changed, false if unchanged.</returns>
    public static bool SetNewSimulatorVersion(uint versionMajor) {
      var simVersionTag = $"MSFS_{versionMajor}";
      if (simVersionTag == SimulatorVersion)
        return false;
      SimulatorVersion = simVersionTag;
      return true;
    }

    /// <summary> Opens the specified database file for reading . The database must have the required tables. Errors are reported via `OnDataErrorEvent` event handler. </summary>
    /// <returns>True on success, false on failure.</returns>
    public bool OpenDataFile(string dbfile = default)
    {
      if (dbfile == default)
        dbfile = ImportsDb;
      try {
        if (_db != null) {
          _db.Dispose();
          _db = null;
        }
        _db = new SQLiteConnection(dbfile, SQLiteOpenFlags.ReadOnly);
        sqlite3_create_function(_db.Handle, "REGEXP", 2, SQLITE_DETERMINISTIC, null, sql_RegexIsMatch);
        sqlite3_create_function(_db.Handle, "ReReplace", 3, SQLITE_DETERMINISTIC, null, sql_RegexReplace);
        sqlite3_create_function(_db.Handle, "EventNameForSelector", 4, SQLITE_DETERMINISTIC, null, sql_EventNameForSelector);
        sqlite3_create_function(_db.Handle, "SimVarNameForSelector", 5, SQLITE_DETERMINISTIC, null, sql_SimVarNameForSelector);
        Logger?.LogDebug("DocImportsCollection: opened database: {file}", dbfile);
        return true;
      }
      catch (Exception e) {
        OnDataErrorEvent?.Invoke(LogLevel.Critical, $"Unable to open database {dbfile}: {e.Message}");
        Logger?.LogCritical(e, "DocImportsCollection::OpenDataFile({dbFile}) - Cannot open database with Exception:", dbfile);
      }
      return false;
    }

    public List<string> Systems(DocImportType type)
    {
      return Systems(new DocImportQuery(type));
    }
    public List<string> Systems(DocImportQuery criteria)
    {
      return QueryStringScalars("System", criteria);
    }

    public List<string> Categories(DocImportType type, string system = null)
    {
      return Categories(new DocImportQuery(type, system));
    }
    public List<string> Categories(DocImportQuery criteria)
    {
      return QueryStringScalars("Category", criteria);
    }

    public List<string> Names(DocImportType type, string system = null, string category = null)
    {
      return Names(new DocImportQuery(type, system, category));
    }
    public List<string> Names(DocImportQuery criteria)
    {
      return QueryStringScalars("Name", criteria);
    }

    // Event Specific

    public List<string> EventSystemCategories()
    {
      DocImportQuery criteria = new DocImportQuery(DocImportType.KeyEvent, null, null, null, null, "System, Category");
      return QueryStringScalars("System || ' - ' || Category AS SystemCategory", criteria);
    }

    public List<string> EventNamesForSelector(string system = null, string category = null)
    {
      DocImportQuery criteria = new DocImportQuery(DocImportType.KeyEvent, system, category, null, null, "Name");
      return QueryStringScalars($"EventNameForSelector(Name, Description, Params, {SimulatorVersion}) AS EventSelectorName", criteria);
    }

    public List<string> EventNamesForSelector(string systemCategory)
    {
      var syscat = systemCategory.Split('-', 2, StringSplitOptions.TrimEntries);
      return EventNamesForSelector(syscat[0], syscat[^1]);
    }

    public List<SimEvent> KeyEvents(string system = null, string category = null)
    {
      return KeyEvents(new DocImportQuery(DocImportType.KeyEvent, system, category));
    }
    public List<SimEvent> KeyEvents(DocImportQuery criteria)
    {
      if (string.IsNullOrWhiteSpace(criteria.OrderBy))
        criteria.OrderBy = "Name";
      return Query<SimEvent>(criteria);
    }

    public SimEvent KeyEvent(DocImportQuery criteria)
    {
      var p = KeyEvents(criteria);
      return p?.Count > 0 ? p[0] : null;
    }
    public SimEvent KeyEvent(string id)
    {
      return Get<SimEvent>(id);
    }

    // SimVar Specific

    public List<string> SimVarSystemCategories(bool? settable = null)
    {
      DocImportQuery criteria = new DocImportQuery(DocImportType.SimVar, null, null, null, settable, "System, Category");
      return QueryStringScalars("System || ' - ' || Category AS SystemCategory", criteria);
    }

    public List<string> SimVarNamesForSelector(string system = null, string category = null, bool? settable = null, IEnumerable<string> currentNames = null)
    {
      DocImportQuery criteria = new DocImportQuery(system, category, settable, "Name");
      string current = currentNames != null ? string.Join(',', currentNames) : string.Empty;
      //Logger?.LogDebug("UnitForSimVar({simVarName}): {C} {result}", simVarName, p.Count, (p.Count > 0 ? p[0] : null));
      return QueryStringScalars($"SimVarNameForSelector(Name, Description, Indexed, {SimulatorVersion}, '{current}') AS SimVarSelectorName", criteria);
    }

    public List<string> SimVarNamesForSelector(string systemCategory, bool? settable = null, IEnumerable<string> currentNames = null)
    {
      var syscat = systemCategory.Split('-', 2, StringSplitOptions.TrimEntries);
      return SimVarNamesForSelector(syscat[0], syscat[^1], settable, currentNames);
    }

    static string GetCleanSimVarNameFromSelector(string selectorName)
    {
      if (PluginConfig.TryNormalizeVarName(selectorName, out var cleanName, out var _))
        return cleanName;
      return null;
    }

    public string UnitForSimVar(string simVarName, bool normalize = true)
    {
      var p = QueryStringScalars(@"LOWER(RTRIM(ReReplace(Units, '(?s)^([\w ]+).*', '$1'))) AS Units", new DocImportQuery(DocImportType.SimVar, null, null, simVarName, null, "Units"));
      //Logger?.LogDebug("UnitForSimVar({simVarName}): {C} {result}", simVarName, p.Count, (p.Count > 0 ? GetNormalizedUnitName(p[0]) : null));
      if (p.Count == 0)
        return null;
      if (normalize)
        return GetNormalizedUnitName(p[0]);
      return p[0];
    }

    public string UnitForSimVarBySelectorName(string varName)
    {
      string cleanName = GetCleanSimVarNameFromSelector(varName);
      return cleanName != null ? UnitForSimVar(cleanName) : null;
    }

    public List<SimVariable> SimVariables(string system = null, string category = null)
    {
      return SimVariables(new DocImportQuery(DocImportType.KeyEvent, system, category));
    }
    public List<SimVariable> SimVariables(DocImportQuery criteria)
    {
      if (string.IsNullOrWhiteSpace(criteria.OrderBy))
        criteria.OrderBy = "Name";
      return Query<SimVariable>(criteria);
    }

    public bool TryGetSimVariable(string varName, out SimVariable simVar)
    {
      simVar = SimVariable(varName);
      return !string.IsNullOrEmpty(simVar.Name);
    }

    //public bool TryGetSimVariableBySelector(string varName, out SimVariable simVar)
    //{
    //  simVar = null;
    //  string cleanName = GetCleanSimVarNameFromSelector(varName);
    //  if (cleanName == null)
    //    return false;
    //  simVar = Get<SimVariable>(cleanName);
    //  return !string.IsNullOrEmpty(simVar.Id);
    //}

    public SimVariable SimVariable(DocImportQuery criteria)
    {
      criteria.Limit = 1;
      var p = SimVariables(criteria);
      return p?.Count > 0 ? p[0] : null;
    }
    public SimVariable SimVariable(string id)
    {
      return Get<SimVariable>(id);
    }

    // Units

    public List<string> UnitNames() => Names(DocImportType.Unit);
    public List<string> UnitShortNames()
    {
      return QueryStringScalars("ShortName", new DocImportQuery(DocImportType.Unit));
    }

    public string GetNormalizedUnitName(string unitName)
    {
      var nameQry = QueryStringScalars("Name", new DocImportQuery(DocImportType.Unit) { Alias = unitName, Limit = 1 });
      //Logger?.LogDebug("GetNormalizedUnitName({unitName}): {C} {result}", unitName, nameQry.Count, (nameQry.Count > 0 ? nameQry[0] : null));
      return nameQry.Count > 0 ? nameQry[0] : null;
    }

    public List<string> GetCompatibleUnits(string unitName)
    {
      var measure = QueryStringScalars("Measure", new DocImportQuery(DocImportType.Unit) { Name = unitName, Limit = 1 });
      if (measure.Count > 0 && !measure[0].StartsWith("Misc"))
        return QueryStringScalars("Name", new DocImportQuery(measure[0]) { OrderBy = $"(Name = '{unitName}') DESC, Name" });
      return new List<string>() { unitName };
    }

    public SimVarUnit SimVarUnit(string name)
    {
      return Get<SimVarUnit>(name);
    }

    // Disposable

    public void Dispose()
    {
      ((IDisposable)_db)?.Dispose();  // same as Close()
      GC.SuppressFinalize(this);
    }

    #endregion Public

    #region Private

    SQLiteConnection _db = null;

    static readonly Regex RxFindFirstSentence = new Regex(@"\.(?:\W|$)", RegexOptions.Multiline | RegexOptions.Compiled);
    static readonly Regex RxTabifyLines = new Regex(@"(?:\r?\n\s*)+", RegexOptions.Compiled);
    static readonly Regex RxShortenLines = new Regex("^(.{0," + SelectorsMaxLineLen + "}).*$", RegexOptions.Multiline | RegexOptions.Compiled);

    private static string FormatDescriptionForSelector(string desc)
    {
      var match = RxFindFirstSentence.Match(desc);
      if (match.Success)
        desc = desc[0..(Math.Min(match.Index + 1, SelectorsMaxLineLen))] + (match.Index > SelectorsMaxLineLen ? "..." : "");
      else if (desc.Length > SelectorsMaxLineLen)
        desc = desc[0..SelectorsMaxLineLen] + "...";
      else
        desc += '.';
      return "\n " + desc.Replace('\n', ' ');
    }


    void OpenDbIfNeeded()
    {
      if (_db == null)
        OpenDataFile();
    }

    List<string> QueryStringScalars(string fieldName, object[] args)
    {
      OpenDbIfNeeded();
      try {
        return _db?.QueryScalars<string>(fieldName, args) ?? new List<string>();
      }
      catch (Exception e) {
        OnDataErrorEvent?.Invoke(LogLevel.Error, $"Database exception in DocImportsCollection::QueryStringScalars({fieldName}): {e.Message}");
        Logger?.LogError(e, "DocImportsCollection::QueryStringScalars({fieldName}) - Database Exception:", fieldName);
        return new List<string>();
      }
    }

    List<string> QueryStringScalars(string fieldName, DocImportQuery qry)
    {
      var res = BuildQuery(fieldName, qry);
      return QueryStringScalars(res.Item1, res.Item2);
    }

    List<T> Query<T>(DocImportQuery criteria) where T : new()
    {
      OpenDbIfNeeded();
      var qry = BuildQuery("*", criteria);
      try {
        return _db?.Query<T>(qry.Item1, qry.Item2) ?? new List<T>();
      }
      catch (Exception e) {
        OnDataErrorEvent?.Invoke(LogLevel.Error, $"Database exception in DocImportsCollection::Query({criteria.Type}): {e.Message}");
        Logger?.LogError(e, "DocImportsCollection::Query({type}) - Database Exception:", criteria.Type);
        return new List<T>();
      }
    }

    T Get<T>(string id) where T : new()
    {
      OpenDbIfNeeded();
      try {
        return _db != null ? _db.Get<T>(id) : default;
      }
      catch (Exception e) {
        OnDataErrorEvent?.Invoke(LogLevel.Error, $"Database exception in DocImportsCollection::Get('{id}'): {e.Message}");
        Logger?.LogError(e, "DocImportsCollection::Get('{id}') - Database Exception:", id);
        return default;
      }
    }

    const string _selectTemplate = "SELECT DISTINCT {0} FROM {1} {2} ORDER BY {3} {4}";

    static Tuple<string, object[]> BuildQuery(string fieldName, DocImportQuery qry)
    {
      List<string> cond = new();
      List<object> args = new();
      string where = string.Empty;
      string limit = string.Empty;
      string table;
      switch (qry.Type) {
        case DocImportType.KeyEvent:
          table = "KeyEvents";
          break;
        case DocImportType.SimVar:
          table = "SimVars";
          if (qry.Settable != null) {
            cond.Add("Settable = ?");
            args.Add(qry.Settable);
          }
          break;
        case DocImportType.Unit:
          table = "SimVarUnits";
          if (!string.IsNullOrEmpty(qry.Measure)) {
            cond.Add("Measure = ?");
            args.Add(qry.Measure);
          }
          if (!string.IsNullOrEmpty(qry.Alias)) {
            cond.Add($"Aliases REGEXP '(?:^|,){qry.Alias}(?:,|$)'");
          }
          break;
        default:
          return new Tuple<string, object[]>("", Array.Empty<object>());
      }
      if (!string.IsNullOrEmpty(qry.Name)) {
        cond.Add("Name = ?");
        args.Add(qry.Name);
      }
      if (qry.Type != DocImportType.Unit) {
        if (qry.Deprecated)
          cond.Add($"{SimulatorVersion} > 0");
        else
          cond.Add($"{SimulatorVersion} = 1");

        if (!string.IsNullOrEmpty(qry.System)) {
          cond.Add("System LIKE ?");
          args.Add(qry.System + '%');
        }
        if (!string.IsNullOrEmpty(qry.Category)) {
          cond.Add("Category = ?");
          args.Add(qry.Category);
        }
      }

      qry.OrderBy ??= fieldName;
      if (cond.Count > 0)
        where = "WHERE " + string.Join(" AND ", cond);
      if (qry.Limit > -1)
        limit = "LIMIT " + qry.Limit.ToString();
      //Logger?.LogDebug("\"{qry}\" ?= {{{args}}}", string.Format(_selectTemplate, fieldName, table, where, qry.OrderBy, limit), string.Join(", ", args));
      return new Tuple<string, object[]>(string.Format(_selectTemplate, fieldName, table, where, qry.OrderBy, limit), args.ToArray());
    }

    // SQLite custom functions

#pragma warning disable IDE1006 // Naming Styles

    private static void sql_RegexIsMatch(SQLitePCL.sqlite3_context ctx, object _, SQLitePCL.sqlite3_value[] args)
    {
      bool isMatched = Regex.IsMatch(sqlite3_value_text(args[1]).utf8_to_string(), sqlite3_value_text(args[0]).utf8_to_string(), RegexOptions.Compiled);
      //Logger.LogDebug("RegexIsMatch: '{i}' '{r}' {m}", sqlite3_value_text(args[1]).utf8_to_string(), sqlite3_value_text(args[0]).utf8_to_string(), isMatched);
      sqlite3_result_int(ctx, isMatched ? 1 : 0);
    }

    private static void sql_RegexReplace(SQLitePCL.sqlite3_context ctx, object _, SQLitePCL.sqlite3_value[] args)
    {
      var newStr = Regex.Replace(sqlite3_value_text(args[0]).utf8_to_string(), sqlite3_value_text(args[1]).utf8_to_string(), sqlite3_value_text(args[2]).utf8_to_string(), RegexOptions.Compiled);
      sqlite3_result_text(ctx, newStr.Trim());
    }

    // EventNameForSelector(Name, Description, Params, {SimulatorVersion})
    private static void sql_EventNameForSelector(SQLitePCL.sqlite3_context ctx, object _, SQLitePCL.sqlite3_value[] args)
    {
      string name = sqlite3_value_text(args[0]).utf8_to_string();
      string desc = sqlite3_value_text(args[1]).utf8_to_string();
      string para = sqlite3_value_text(args[2]).utf8_to_string();
      int simVer = sqlite3_value_int(args[3]);
      if (!string.IsNullOrEmpty(desc))
        name += FormatDescriptionForSelector(desc);
      if (!string.IsNullOrEmpty(para) && !para.Equals("N/A", StringComparison.InvariantCultureIgnoreCase))
        name += "\n\t" + RxShortenLines.Replace(RxTabifyLines.Replace(para, "\n\t"), "$1");
      if (simVer == 2)
        name += '\n' + "(DEPRECATED)";
      sqlite3_result_text(ctx, name);
    }

    // SimVarNameForSelector(Name, Description, Indexed, {SimulatorVersion}, '{current}')
    private static void sql_SimVarNameForSelector(SQLitePCL.sqlite3_context ctx, object _, SQLitePCL.sqlite3_value[] args)
    {
      string name = sqlite3_value_text(args[0]).utf8_to_string();
      string desc = sqlite3_value_text(args[1]).utf8_to_string();
      bool indx = sqlite3_value_int(args[2]) > 0;
      int simVer = sqlite3_value_int(args[3]);
      string existing = sqlite3_value_text(args[4]).utf8_to_string();
      if (!string.IsNullOrEmpty(existing) && Array.Exists(existing.Split(','), n => n.Split(':').First().Equals(name)))
        name = "* " + name;
      if (indx)
        name += ":N";
      if (!string.IsNullOrEmpty(desc))
        name += FormatDescriptionForSelector(desc);
      if (simVer == 2)
        name += '\n' + "(DEPRECATED)";
      //Logger.LogDebug("SQL: {name}\n{existing}", name, existing.Split(','));
      sqlite3_result_text(ctx, name);
    }

#pragma warning restore IDE1006 // Naming Styles

    #endregion Private
  }
}
