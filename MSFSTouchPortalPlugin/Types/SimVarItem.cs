/*
This file is part of the MSFS Touch Portal Plugin project.
https://github.com/mpaperno/MSFSTouchPortalPlugin

COPYRIGHT:
(c) 2020 Tim Lewis;
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

using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Enums;

using Stopwatch = System.Diagnostics.Stopwatch;
using Regex = System.Text.RegularExpressions.Regex;
using RegexOptions = System.Text.RegularExpressions.RegexOptions;
using SIMCONNECT_DATATYPE = Microsoft.FlightSimulator.SimConnect.SIMCONNECT_DATATYPE;
#if WASIM
using DataRequestRecord = WASimCommander.CLI.Structs.DataRequestRecord;
using CalcResultType = WASimCommander.CLI.Enums.CalcResultType;
#endif

namespace MSFSTouchPortalPlugin.Types
{
  /// <summary> Dynamically generated SimConnect definition IDs are "parented" to this enum type,
  /// meaning they become of this Type when they need to be cast to en Enum type (eg. for SimConnect C# API). </summary>
  public enum Definition
  {
    None = 0
  }

  public enum SimVarDataProvider : byte
  {
    None, SimConnect, WASimClient
  };

  public enum SimVarRegistrationStatus : byte
  {
    Unregistered, Registered, Error, TemporaryError
  };

  /// <summary>
  /// The SimVarItem which defines all data variables for SimConnect
  /// </summary>
  public class SimVarItem : System.IComparable<SimVarItem>, System.IComparable
  {
    public const double DELTA_EPSILON_DEFAULT = 0.0;
#if WASIM
    public static readonly string[] ReadableVariableTypes = { "A: SimVar", "B: Input Event", "C: GPS", "E: Env.", "L: Local", "M: Mouse", "R: Rsrc.", "T: Token", "Z: Custom" };
    public static readonly string[] SettableVariableTypes = { "A: SimVar", "B: Input Event", "C: GPS", "H: HTML Event", "K: Key Event", "L: Local", "Z: Custom" };
#elif FSX
    public static readonly string[] ReadableVariableTypes = { "A: SimVar" };
    public static readonly string[] SettableVariableTypes = ReadableVariableTypes;
#else
    public static readonly string[] ReadableVariableTypes = { "A: SimVar", "B: Input Event", "L: Local" };
    public static readonly string[] SettableVariableTypes = ReadableVariableTypes;
#endif

    /// <summary> Unique ID string, used to generate TouchPortal state ID (and possibly other uses). </summary>
    public string Id { get; set; }
    /// <summary> Category for sorting/organizing, also used in TouchPortal state ID. </summary>
    public Groups CategoryId { get; set; } = default;
    /// <summary>
    /// The variable type designator, as per MSFS RPN docs (mostly): https://docs.flightsimulator.com/html/Additional_Information/Reverse_Polish_Notation.htm
    /// One of: 'A' (default, SimVar), 'B', 'C', 'E', 'L', 'M', 'P', 'R', 'Z', plus 'T' for Token and 'Q' for calculator code.
    /// </summary>
    public char VariableType { get; set; } = 'A';
    /// <summary> Descriptive name for this data (for TouchPortal or other UI). </summary>
    public string Name { get; set; }
    /// <summary> Corresponding SimConnect Variable name or calculator code. </summary>
    public string SimVarName { get; set; }
    /// <summary> Default value string when no data from SimConnect. </summary>
    public string DefaultValue { get; set; }
    /// <summary> How often updates are sent by SimConnect if value changes (SIMCONNECT_PERIOD). Default is equivalent to SIMCONNECT_PERIOD_SIM_FRAME. </summary>
    public UpdatePeriod UpdatePeriod { get; set; } = UpdatePeriod.Default;
    /// <summary> The number of UpdatePeriod events that should elapse between data updates. Default is 0, which means the data is transmitted every Period.
    /// Note that when UpdatePeriod = Millisecond, there is an effective minimum of ~25ms. </summary>
    public uint UpdateInterval { get; set; } = 0;
    /// <summary> Only report change if it is greater than the value of this parameter (not greater than or equal to).
    /// Eg. value of 0.009f limits changes to 2 decimal places which is suitable for most decimal unit types (except perhaps MHz and "percent over 100"). </summary>
    public float DeltaEpsilon { get; set; } = (float)DELTA_EPSILON_DEFAULT;
    /// <summary> Simulator version match required for this variable. Eg. "11" or "11.0.23112" </summary>
    public string SimVersion { get; set; }
    /// <summary> Could also be "choice" but we don't use that (yet?) </summary>
    public string TouchPortalValueType { get; set; } = "text";
    /// <summary> This could/should be populated by whatever is creating the SimVarItem instance </summary>
    public string TouchPortalStateId { get; set; }
    /// <summary> A string used to identify this var in TP selection lists. This could/should be populated by whatever is creating the SimVarItem instance </summary>
    public string TouchPortalSelector { get; set; }
    /// <summary> Tracks the origin of this item for later reference. </summary>
    public SimVarDefinitionSource DefinitionSource { get; set; }
    /// <summary> Tracks which file this item was loaded from, if any, for later reference. </summary>
    public string DefinitionSourceFile { get; set; }
    /// <summary> Tracks the source of the value data for this variable request. </summary>
    public SimVarDataProvider DataProvider { get; set; } = SimVarDataProvider.None;
    /// <summary> Status of this request registration with the data provider. </summary>
    public SimVarRegistrationStatus RegistrationStatus { get; set; } = SimVarRegistrationStatus.Unregistered;
    public bool IsRegistered => RegistrationStatus == SimVarRegistrationStatus.Registered;

    /// <summary>
    /// SimConnect unit name. Changing this property will clear any current value!
    /// Setting this property also sets the SimConnectDataType, StorageDataType, and all the Is*Type properties.
    /// </summary>
    public string Unit
    {
      get => _unit;
      set {
        if (_unit == value)
          return;
        _unit = value;
        // set up type information based on the new Unit.
        IsStringType = Units.IsStringType(_unit);
        IsBooleanType = !IsStringType && Units.IsBooleanType(_unit);
        IsIntegralType = !IsStringType && !IsBooleanType && Units.IsIntegralType(_unit);
        IsRealType = !IsStringType && !IsBooleanType && !IsIntegralType;
        SimConnectDataType = IsStringType ? SIMCONNECT_DATATYPE.STRING256 : IsIntegralType ? SIMCONNECT_DATATYPE.INT64 : IsBooleanType ? SIMCONNECT_DATATYPE.INT32 : SIMCONNECT_DATATYPE.FLOAT64;
        StorageDataType = IsStringType ? typeof(StringVal) : IsIntegralType ? typeof(long) : IsBooleanType ? typeof(uint) : typeof(double);
      }
    }

#if WASIM
    /// <summary>
    /// Returns the set calculation result type, for 'Q' type variables. Setting this value will also set the corresponding Unit type
    /// to either "number," (for Double result) "integer," or "String" and hence also the corresponding data type/size.
    /// </summary>
    public CalcResultType CalcResultType
    {
      get => _calcResultType;
      set {
        if (value == _calcResultType)
          return;
        _calcResultType = value;
        if (value == CalcResultType.Double)
          Unit = "number";
        else if (value == CalcResultType.Integer)
          Unit = "integer";
        else
          Unit = "string";
      }
    }
    private CalcResultType _calcResultType;
#else
    public uint CalcResultType = 0;
#endif

    /// <summary>
    /// This returns a full formatting string, as in "{0}" or "{0:FormattingString}" as needed.
    /// It can be set with either a full string (with "{}" brackets and/or "0:" part(s)) or just as the actual formatting part (what goes after the "0:" part).
    /// To get the "raw" formatting string, w/out any "{}" or "0:" parts, use the FormattingString property (which may be blank/null). </summary>
    public string StringFormat
    {
      get => string.IsNullOrWhiteSpace(_formatString) ? "{0}" : "{0:" + _formatString + "}";
      set {
        value = value.Trim('{', '}');
        if (value.StartsWith("0:"))
          value = value.Remove(0, 2);
        _formatString = value;
      }
    }

    /// <summary> The current value as an object. Read-only, and may be null. Use SetValue() methods to set value. </summary>
    public object Value
    {
      get => _value;
      private set {
        _value = value;
        _lastUpdate = Stopwatch.GetTimestamp();
        SetPending(false);
        _valueExpires = UpdatePeriod switch {
          UpdatePeriod.Millisecond => _lastUpdate + UpdateInterval * (Stopwatch.Frequency / 1000L),
          //UpdatePeriod.Second      => _lastUpdate + UpdateInterval * Stopwatch.Frequency,
          _ => 0,  // never?  or always?
        };
      }
    }

    /// <summary>
    /// Returns the current value as a formatted string according to the value type and StringFormat property.
    /// If no value has been explicitly set, returns the DefaultValue.
    /// </summary>
    public string FormattedValue
    {
      get {
        if (!ValInit)
          return DefaultValue;
        return Value switch {
          double v => string.Format(StringFormat, v),
          uint v => string.Format(StringFormat, v),
          long v => string.Format(StringFormat, v),
          StringVal v => string.Format(StringFormat, v.ToString()),
          _ => string.Empty,
        };
      }
    }

    public static implicit operator double(SimVarItem simVar)
    {
      if (!simVar.ValInit)
        return 0;
      return simVar.Value switch {
        double v => v,
        uint v => v,
        long v => v,
        _ => 0,
      };
    }

    /// <summary>
    /// The actual system Type used for Value property. This is determined automatically when setting the Unit type.
    /// The return type could be null if Value type is null. Changing this property will clear any current value!
    /// </summary>
    public System.Type StorageDataType
    {
      get => Value?.GetType();
      private set {
        if (Value == null || Value.GetType() != value) {
          _value = value == typeof(StringVal) ? new StringVal() : System.Activator.CreateInstance(value);
          _lastUpdate = 0;
        }
      }
    }

    /// <summary> Returns a SimConnect dwSizeOrType value.  SIMCONNECT_CLIENTDATATYPE_* constants (not present in managed version).
    /// https://docs.flightsimulator.com/html/Programming_Tools/SimConnect/API_Reference/Events_And_Data/SimConnect_AddToClientDataDefinition.htm#parameters
    /// </summary>
    public uint DataSize
    {
      get {
        if (StorageDataType == typeof(double))
          return unchecked((uint)-6);   // SIMCONNECT_CLIENTDATATYPE_FLOAT64
        if (StorageDataType == typeof(long))
          return unchecked((uint)-4);   // SIMCONNECT_CLIENTDATATYPE_INT64
        if (StorageDataType == typeof(uint) || StorageDataType == typeof(int))
          return unchecked((uint)-3);   // SIMCONNECT_CLIENTDATATYPE_INT32
        if (StorageDataType == typeof(StringVal))
          return StringVal.MAX_SIZE;   // return actual byte size for strings
        // we don't use the types below but just in case we do later.
        if (StorageDataType == typeof(float))
          return unchecked((uint)-5);   // SIMCONNECT_CLIENTDATATYPE_FLOAT32
        if (StorageDataType == typeof(byte))
          return unchecked((uint)-1);   // SIMCONNECT_CLIENTDATATYPE_INT8
        if (StorageDataType == typeof(short) || StorageDataType == typeof(char))
          return unchecked((uint)-2);   // SIMCONNECT_CLIENTDATATYPE_INT16
        return 0;
      }
    }

    /// <summary> Returns true if this value is of a real (double) type, false otherwise </summary>
    public bool IsRealType { get; private set; }
    /// <summary> Returns true if this value is of a string type, false if numeric or bool. </summary>
    public bool IsStringType { get; private set; }
    /// <summary> Returns true if this value is of a integer type, false if string, real or bool. </summary>
    public bool IsIntegralType { get; private set; }
    /// <summary> Returns true if this value is of a boolean type, false otherwise </summary>
    public bool IsBooleanType { get; private set; }

    /// <summary> Unique Definition ID for SimConnect </summary>
    public Definition Def { get; private set; }
    /// <summary> The SimConnect data type for registering this var. </summary>
    public SIMCONNECT_DATATYPE SimConnectDataType { get; private set; }
    /// <summary> The SimConnect "hash" ID for Input Event ('B') type variables. </summary>
    public ulong InputEventHash { get; set; } = 0;
    /// <summary> Indicates that this state needs a scheduled update request (UpdatePeriod == Millisecond). </summary>
    public bool NeedsScheduledRequest => DataProvider != SimVarDataProvider.WASimClient && UpdatePeriod == UpdatePeriod.Millisecond;
    /// <summary> For serializing the "raw" formatting string w/out "{0}" parts </summary>
    public string FormattingString => _formatString;

    /// <summary>
    /// Indicates that the value has "expired" based on the UpdatePeriod and UpdateInterval since the last time the value was set.
    /// This always returns false if UpdatePeriod != UpdatePeriod.Millisecond. Also returns false if a request for this value is pending and hasn't yet timed out.
    /// </summary>
    public bool UpdateRequired => IsRegistered && NeedsScheduledRequest && !CheckPending() && Stopwatch.GetTimestamp() > _valueExpires;

    // Private properties  ---------------------

    private object _value;         // the actual Value storage
    private string _unit;          // unit type storage
    private string _formatString;  // the "raw" formatting string w/out "{0}" part
    private long _lastUpdate = 0;  // value update timestamp in Stopwatch ticks
    private long _valueExpires;    // value expiry timestamp in Stopwatch ticks, if a timed UpdatePeriod type, zero otherwise
    private long _requestTimeout;  // for tracking last data request time to avoid race conditions, next pending timeout ticks count or zero if not pending
    private const short REQ_TIMEOUT_SEC = 30;  // pending value timeout period in seconds

    private bool ValInit => _lastUpdate > 0;  // has value been set at least once

    // this is how we generate unique Def IDs for every instance of SimVarItem. Assigned in c'tor.
    private static Definition _nextDefinionId = Definition.None;
    private static Definition NextId() => ++_nextDefinionId;      // got a warning when trying to increment this directly from c'tor, but not via static member... ?

    // Constructor  --------------------

    public SimVarItem() {
      Def = NextId();
      Unit = "number";  // set a default unit
    }

    // Methods  ------------------------

    /// <summary> Convenience static method to check if a variable type requires a Unit specifier or not. </summary>
    /// <param name="varType">The type of variable ('A', 'L', etc).</param>
    /// <returns>`true` if the type requires a unit specifier, `false` otherwise.</returns>
    public static bool RequiresUnitType(char varType) => (varType == 'A' || varType == 'C' || varType == 'E');
    /// <summary> Convenience static method to check if a variable type is supported by SimConnect or requires WASM integration. </summary>
    /// <param name="varType">The type of variable ('A', 'L', etc).</param>
    /// <returns>`true` if the type is supported by SimConnect, `false` otherwise.</returns>
#if FSX
    public static bool SimConnectSupported(char varType) => varType == 'A';
#else
    public static bool SimConnectSupported(char varType) => (varType == 'A' || varType == 'B' || varType == 'L');
#endif

    public bool ValueEquals(string value) => ValInit && IsStringType && value == Value.ToString();
    public bool ValueEquals(double value) => ValInit && IsRealType && System.Math.Abs((double)Value - value) <= DeltaEpsilon;
    public bool ValueEquals(long value)   => ValInit && IsIntegralType && System.Math.Abs((long)Value - value) <= (long)DeltaEpsilon;
    public bool ValueEquals(uint value)   => ValInit && IsBooleanType && System.Math.Abs((uint)Value - value) <= (uint)DeltaEpsilon;
#if WASIM
    public bool ValueEquals(DataRequestRecord dr) => ValInit && Value switch {
      StringVal or string => ValueEquals((string)dr),
      uint => ValueEquals((uint)dr),
      double => ValueEquals((double)dr),
      long => ValueEquals((long)dr),
      _ => false,
    };
#endif

    /// <summary>
    /// Compare this instance's value to the given object's value. For numeric types, it takes the DeltaEpsilon property into account.
    /// Uses strict type matching for double, long, uint, and falls back to string compare for all other types.
    /// </summary>
    public bool ValueEquals(object value) {
      try {
        return value switch {
          double v => ValueEquals(v),
          uint v => ValueEquals(v),
          long v => ValueEquals(v),
#if WASIM
          DataRequestRecord dr => ValueEquals(dr),
#endif
          _ => ValueEquals(value.ToString()),
        };
      }
      catch {
        return false;
      }
    }

    internal bool SetValue(StringVal value) {
      if (IsStringType)
        Value = value;
      return IsStringType;
    }

    internal bool SetValue(double value) {
      if (IsStringType)
        return false;
      if (IsRealType)
        Value = value;
      else if (IsBooleanType)
        Value = (long)value != 0;
      else
        Value = (long)value;
      return true;
    }

    internal bool SetValue(long value) {
      if (IsIntegralType)
        Value = value;
      return IsIntegralType;
    }

    internal bool SetValue(uint value) {
      if (IsBooleanType)
        Value = value;
      return IsBooleanType;
    }

    internal bool SetValue(string value) {
      return Value switch {
        StringVal or string => SetValue(new StringVal(value)),
        uint           => SetValue((uint)new BooleanString(value)),
        double or long => double.TryParse(value, out var dVal) && SetValue(dVal),
        _              => false,
      };
    }

#if WASIM
    internal bool SetValue(DataRequestRecord dr) {
      return Value switch {
        StringVal or string => SetValue((string)dr),
        uint => SetValue((uint)dr),
        double => SetValue((double)dr),
        long => SetValue((long)dr),
        _ => false,
      };
      //return SetValue(System.Convert.ChangeType(dr, StorageDataType));
    }
#endif

    /// <summary>
    /// Prefer using this method, or one of the type-specific SetValue() overloads to
    /// to set the Value property, vs. direct access. Returns false if the given object's
    /// value type doesn't match this type.
    /// </summary>
    internal bool SetValue(object value) {
      try {
        return value switch {
          double v => SetValue(v),
          uint v => SetValue(v),
          long v => SetValue(v),
          StringVal v => SetValue(v),
#if WASIM
          DataRequestRecord dr => SetValue(dr),
#endif
          _ => SetValue(value.ToString())
        };
      }
      catch {
        return false;
      }
    }

    /// <summary>
    /// Updates the object to either set pending update or no longer pending
    /// </summary>
    /// <param name="val">True/False</param>
    public void SetPending(bool val) {
      _requestTimeout = val ? Stopwatch.GetTimestamp() + REQ_TIMEOUT_SEC * Stopwatch.Frequency : 0;
    }

    private bool CheckPending() {
      if (_requestTimeout == 0)
        return false;
      if (Stopwatch.GetTimestamp() > _requestTimeout) {
        SetPending(false);
        return false;
      }
      return true;
    }

    /// <summary>
    /// Checks that all properties have reasonable values. This assumes the variable will be _requested_, so it must be a readable type.
    /// </summary>
    /// <param name="resultMsg">Any generated error or warning text is returned here.
    /// If the returned value is not empty but the method returned `true` then the message is considered a warning, not error.</param>
    /// <returns>`true` if validation passed w/out errors, `false` otherwise.</returns>
    public bool Validate(out string resultMsg)
    {
      static bool returnError(string err, ref string outerr) {
        outerr = err;
        return false;
      }

      resultMsg = string.Empty;

      // Ensure required data is not missing entirely
      if (string.IsNullOrEmpty(Id))
        return returnError("ID is empty", ref resultMsg);
      if (string.IsNullOrEmpty(Name))
        return returnError("Name is empty", ref resultMsg);
      if (string.IsNullOrEmpty(SimVarName))
        return returnError("Variable Name is empty", ref resultMsg);
      if (RequiresUnitType(VariableType) && string.IsNullOrEmpty(Unit))
        return returnError("Unit type is empty and required", ref resultMsg);

      // Make sure a category is assigned, except for temporary items
      if (CategoryId == Groups.None && DefinitionSource != SimVarDefinitionSource.Temporary)
        return returnError("CategoryId is required", ref resultMsg);

      // Check variable name minimum length, indexed character checks follow
      if (SimVarName.Length < 4)
        return returnError("Variable Name is too short to be valid", ref resultMsg);
      // Make sure variable name doesn't have leading "N:" type specifier
      if (SimVarName[1] == ':')
        return returnError($"Variable Name appears to contain variable type prefix ('{SimVarName[0..2]}')", ref resultMsg);

      // Check type-specific attributes and also that the type can be requested in the first place.
      switch (VariableType) {
        case 'A':
          // SimVar names may have spaces but never underscores; may be followed by ":I" index value.
          if (!Regex.IsMatch(SimVarName, @"^[a-zA-Z][a-zA-Z0-9 ]+(?::\d{1,3})?$", RegexOptions.Compiled | RegexOptions.CultureInvariant))
            return returnError($"SimVar Name '{SimVarName}' contains invalid character(s)", ref resultMsg);
          break;

        case 'C':
          // "C" type vars require a module name prefix; add a default if one is missing
          if (SimVarName.IndexOf(':', 2) < 0)
            SimVarName = "fs9gps:" + SimVarName;
          // They may have underscores in the name, but not spaces.
          if (!Regex.IsMatch(SimVarName, @"^\w{2,}:[a-zA-Z][a-zA-Z0-9_]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant))
            return returnError($"Callback Variable Name '{SimVarName}' contains invalid character(s)", ref resultMsg);
          break;

        case 'E':
        case 'P':
          // Environment vars may have spaces in the names, but no underscores.
          if (!Regex.IsMatch(SimVarName, @"^[a-zA-Z][a-zA-Z0-9 ]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant))
            return returnError($"Environment Variable Name '{SimVarName}' contains invalid character(s)", ref resultMsg);
          break;

        case 'B':
        case 'L':
        case 'T':
          // Local, Token, and Input Event variable types may only have alphanumerics, underscores, and colons; no spaces.
          if (!Regex.IsMatch(SimVarName, @"^[a-zA-Z][a-zA-Z0-9_:]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant))
            return returnError($"Variable Name '{SimVarName}' contains invalid character(s)", ref resultMsg);
          break;

        case 'M':
          // Mouse vars are only PascalCase text.
          if (!Regex.IsMatch(SimVarName, @"^[A-Z][a-zA-Z]*$", RegexOptions.Compiled | RegexOptions.CultureInvariant))
            return returnError($"Mouse Variable Name '{SimVarName}' contains invalid character(s)", ref resultMsg);
          break;
#if WASIM
        case 'Q':
          if (CalcResultType == WASimCommander.CLI.Enums.CalcResultType.None)
            return returnError("Calculation result type (CalcResultType) is required for 'Q' type request", ref resultMsg);
          break;
#endif
        case 'R':
          // Resource type vars are: "0:HELPID_EXTR_LOW_VOLT" or "1:@TT_Package.AUDIOPANEL_KNOB_COM_VOLUME_ACTION"
          if (!Regex.IsMatch(SimVarName, @"^\d:[a-zA-Z@][a-zA-Z0-9_\.]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant))
            return returnError($"Resource Variable Name '{SimVarName}' has invalid format", ref resultMsg);
          break;

        default:
          return returnError($"Variable type '{VariableType}' cannot be requested", ref resultMsg);
      }

      // Reset pointless DeltaEpsolin values
      if (!IsRealType && DeltaEpsilon != (float)DELTA_EPSILON_DEFAULT) {
        if (IsStringType || IsBooleanType || DeltaEpsilon < 1.0f) {
          resultMsg = $"Invalid DeltaEpsilon value {DeltaEpsilon} for unit type {Unit}; resetting to default";
          DeltaEpsilon = (float)DELTA_EPSILON_DEFAULT;
        }
      }

      return true;
    }


    // Object

    public override int GetHashCode() => Id.GetHashCode();
    public override string ToString() => SimVarName;

    public string ToDebugString() {
      return $"{GetType().Name}: {{{Id}; Def: {Def}; Type: {VariableType}; VarName: {SimVarName}; Unit: {Unit}; " +
        $"ResType: {CalcResultType}; Cat: {CategoryId}; Name: {Name}; Per: {UpdatePeriod}; Itvl: {UpdateInterval}; DE: {DeltaEpsilon:F6}; " +
        $"Stat: {RegistrationStatus}; Prov: {DataProvider}; Src: {DefinitionSource};}} = '{FormattedValue}'";
    }

    // IComparable

    public override bool Equals(object obj) => ReferenceEquals(this, obj) || (obj is SimVarItem item && item.Id == Id);
    public static bool operator ==(SimVarItem left, SimVarItem right) => left is null ? right is null : left.Equals(right);
    public static bool operator !=(SimVarItem left, SimVarItem right) => !(left == right);
    public static bool operator <(SimVarItem left, SimVarItem right)  => left is null ? right is not null : left.CompareTo(right) < 0;
    public static bool operator <=(SimVarItem left, SimVarItem right) => left is null || left.CompareTo(right) <= 0;
    public static bool operator >(SimVarItem left, SimVarItem right)  => left is not null && left.CompareTo(right) > 0;
    public static bool operator >=(SimVarItem left, SimVarItem right) => left is null ? right is null : left.CompareTo(right) >= 0;

    public int CompareTo(object obj) => obj is SimVarItem item ? CompareTo(item) : -1;
    public int CompareTo(SimVarItem other) => other is null ? -1 : Id.CompareTo(other.Id);

  }
}
