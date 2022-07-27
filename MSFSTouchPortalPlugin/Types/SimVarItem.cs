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
using WASimCommander.CLI.Structs;
using Stopwatch = System.Diagnostics.Stopwatch;
using SIMCONNECT_DATATYPE = Microsoft.FlightSimulator.SimConnect.SIMCONNECT_DATATYPE;

namespace MSFSTouchPortalPlugin.Types
{
  /// <summary> Dynamically generated SimConnect definition IDs are "parented" to this enum type,
  /// meaning they become of this Type when they need to be cast to en Enum type (eg. for SimConnect C# API). </summary>
  public enum Definition
  {
    None = 0
  }

  /// <summary>
  /// The SimVarItem which defines all data variables for SimConnect
  /// </summary>
  public class SimVarItem : System.IComparable<SimVarItem>, System.IComparable
  {
    public const double DELTA_EPSILON_DEFAULT = 0.009;

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
    /// <summary> Corresponding SimConnect Variable name or calculator code (blank if used for internal purposes). </summary>
    public string SimVarName { get; set; }
    /// <summary> Default value string when no data from SimConnect. </summary>
    public string DefaultValue { get; set; }
    /// <summary> SimConnect settable Sim Var </summary>
    public bool CanSet { get; set; } = false;
    /// <summary> How often updates are sent by SimConnect if value changes (SIMCONNECT_PERIOD). Default is equivalent to SIMCONNECT_PERIOD_SIM_FRAME. </summary>
    public UpdatePeriod UpdatePeriod { get; set; } = UpdatePeriod.Default;
    /// <summary> The number of UpdatePeriod events that should elapse between data updates. Default is 0, which means the data is transmitted every Period.
    /// Note that when UpdatePeriod = Millisecond, there is an effective minimum of ~25ms. </summary>
    public uint UpdateInterval { get; set; } = 0;
    /// <summary> Only report change if it is greater than the value of this parameter (not greater than or equal to).
    /// Default is 0.009f limits changes to 2 decimal places which is suitable for most unit types (except perhaps MHz and "percent over 100"). </summary>
    public float DeltaEpsilon { get; set; } = (float)DELTA_EPSILON_DEFAULT;
    /// <summary> Could also be "choice" but we don't use that (yet?) </summary>
    public string TouchPortalValueType { get; set; } = "text";
    /// <summary> This could/should be populated by whatever is creating the SimVarItem instance </summary>
    public string TouchPortalStateId { get; set; }
    /// <summary> A string used to identify this var in TP selection lists. This could/should be populated by whatever is creating the SimVarItem instance </summary>
    public string TouchPortalSelector { get; set; }
    /// <summary> Tracks the origin of this item for later reference. </summary>
    public DataSourceType DataSource { get; set; }

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

    /// <summary>
    /// Returns the set calculation result type, for 'Q' type variables. Setting this value will also set the corresponding Unit type
    /// to either "number," (for Double result) "integer," or "String" and hence also the corresponding data type/size.
    /// </summary>
    public WASimCommander.CLI.Enums.CalcResultType CalcResultType
    {
      get => _calcResultType;
      set {
        if (value == _calcResultType)
          return;
        _calcResultType = value;
        if (value == WASimCommander.CLI.Enums.CalcResultType.Double)
          Unit = Units.number;
        else if (value == WASimCommander.CLI.Enums.CalcResultType.Integer)
          Unit = Units.integer;
        else
          Unit = Units.String;
      }
    }

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

    /// <summary> Returns a SimConnect dwSizeOrType value. </summary>
    public uint DataSize
    {
      get {
        if (StorageDataType == typeof(double))
          return WASimCommander.CLI.ValueTypes.DATA_TYPE_DOUBLE;
        if (StorageDataType == typeof(long))
          return WASimCommander.CLI.ValueTypes.DATA_TYPE_INT64;
        if (StorageDataType == typeof(uint) || StorageDataType == typeof(int))
          return WASimCommander.CLI.ValueTypes.DATA_TYPE_INT32;
        if (StorageDataType == typeof(StringVal))
          return StringVal.MAX_SIZE;   // return actual byte size for strings
        // we don't use the types below but just in case we do later.
        if (StorageDataType == typeof(float))
          return WASimCommander.CLI.ValueTypes.DATA_TYPE_FLOAT;
        if (StorageDataType == typeof(byte))
          return WASimCommander.CLI.ValueTypes.DATA_TYPE_INT8;
        if (StorageDataType == typeof(short) || StorageDataType == typeof(char))
          return WASimCommander.CLI.ValueTypes.DATA_TYPE_INT16;
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
    /// <summary> Indicates that this state needs a scheduled update request (UpdatePeriod == Millisecond). </summary>
    public bool NeedsScheduledRequest => UpdatePeriod == UpdatePeriod.Millisecond;
    /// <summary> For serializing the "raw" formatting string w/out "{0}" parts </summary>
    public string FormattingString => _formatString;

    /// <summary>
    /// Indicates that the value has "expired" based on the UpdatePeriod and UpdateInterval since the last time the value was set.
    /// This always returns false if UpdatePeriod != UpdatePeriod.Millisecond. Also returns false if a request for this value is pending and hasn't yet timed out.
    /// </summary>
    public bool UpdateRequired => _valueExpires > 0 && !CheckPending() && Stopwatch.GetTimestamp() > _valueExpires;


    private object _value;         // the actual Value storage
    private string _unit;          // unit type storage
    private string _formatString;  // the "raw" formatting string w/out "{0}" part
    private long _lastUpdate = 0;  // value update timestamp in Stopwatch ticks
    private long _valueExpires;    // value expiry timestamp in Stopwatch ticks, if a timed UpdatePeriod type, zero otherwise
    private long _requestTimeout;  // for tracking last data request time to avoid race conditions, next pending timeout ticks count or zero if not pending
    private WASimCommander.CLI.Enums.CalcResultType _calcResultType;
    private const short REQ_TIMEOUT_SEC = 30;  // pending value timeout period in seconds

    private bool ValInit => _lastUpdate > 0;  // has value been set at least once

    // this is how we generate unique Def IDs for every instance of SimVarItem. Assigned in c'tor.
    private static Definition _nextDefinionId = Definition.None;
    private static Definition NextId() => ++_nextDefinionId;      // got a warning when trying to increment this directly from c'tor, but not via static member... ?

    public SimVarItem() {
      Def = NextId();
      Unit = "number";  // set a default unit
    }

    public bool ValueEquals(string value) => ValInit && IsStringType && value == Value.ToString();
    public bool ValueEquals(double value) => ValInit && IsRealType && System.Math.Abs((double)Value - value) <= DeltaEpsilon;
    public bool ValueEquals(long value)   => ValInit && IsIntegralType && System.Math.Abs((long)Value - value) <= (long)DeltaEpsilon;
    public bool ValueEquals(uint value)   => ValInit && IsBooleanType && System.Math.Abs((uint)Value - value) <= (uint)DeltaEpsilon;
    public bool ValueEquals(DataRequestRecord dr) => ValInit && Value switch {
      StringVal or string => ValueEquals((string)dr),
      uint => ValueEquals((uint)dr),
      double => ValueEquals((double)dr),
      long => ValueEquals((long)dr),
      _ => false,
    };

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
          DataRequestRecord dr => ValueEquals(dr),
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
          DataRequestRecord dr => SetValue(dr),
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

    // Object

    public override int GetHashCode() => Id.GetHashCode();
    public override string ToString() => SimVarName;

    public string ToDebugString() {
      return $"{GetType().Name}: {{{Id}; Def: {Def}; Type: {VariableType}; VarName: {SimVarName}; Unit: {Unit}; ResType: {CalcResultType}; Cat: {CategoryId}; Name: {Name}; Per: {UpdatePeriod}; Itvl: {UpdateInterval}; DE: {DeltaEpsilon:F6};}}";
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
