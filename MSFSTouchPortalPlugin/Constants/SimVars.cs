using MSFSTouchPortalPlugin.Enums;
using Stopwatch = System.Diagnostics.Stopwatch;
using SIMCONNECT_DATATYPE = Microsoft.FlightSimulator.SimConnect.SIMCONNECT_DATATYPE;

namespace MSFSTouchPortalPlugin.Constants
{
  // TODO: Remove all values except Init, move to Enums folder/namespace
  public enum Definition
  {
    None = 0,
    AileronTrimPct,
    AircraftTitle,
    AirSpeedIndicated,
    AirSpeedMach,
    AirSpeedTrue,
    AntiIceEng1,
    AntiIceEng2,
    AntiIceEng3,
    AntiIceEng4,
    AntiIcePanelSwitch,
    AntiIcePropeller1Switch,
    AntiIcePropeller2Switch,
    AntiIcePropeller3Switch,
    AntiIcePropeller4Switch,
    AntiIceStructuralSwitch,
    AntiIceWindshieldSwitch,
    AtcAirline,
    AtcFlightNumber,
    AtcId,
    AtcModel,
    AtcType,
    AutoPilotAirSpeedHold,
    AutoPilotAirSpeedVar,
    AutoPilotAltitudeHold,
    AutoPilotAltitudeVar,
    AutoPilotApproachHold,
    AutoPilotAttitudeHold,
    AutoPilotAttitudeVar,
    AutoPilotAvailable,
    AutoPilotBackCourseHold,
    AutoPilotBanking,
    AutoPilotFlightDirector,
    AutoPilotFlightDirectorCurrentBank,
    AutoPilotFlightDirectorCurrentPitch,
    AutoPilotHeadingHold,
    AutoPilotHeadingVar,
    AutoPilotMach,
    AutoPilotMachVar,
    AutoPilotMaster,
    AutoPilotNav1Hold,
    AutoPilotNavSelected,
    AutoPilotPitchHold,
    AutoPilotVerticalSpeedHold,
    AutoPilotVerticalSpeedVar,
    AutoPilotWingLeveler,
    AutoPilotYawDampener,
    AutoThrottleArm,
    AutoThrottleGoAround,
    AvionicsMasterSwitch,
    Com1ActiveFrequency,
    Com1StandbyFrequency,
    Com2ActiveFrequency,
    Com2StandbyFrequency,
    CowlFlaps1Percent,
    CowlFlaps2Percent,
    CowlFlaps3Percent,
    CowlFlaps4Percent,
    ElevatorTrimPct,
    FlapsHandlePercent,
    FlapSpeedExceeeded,
    GroundAltitude,
    GroundVelocity,
    LightBeaconOn,
    LightBrakeOn,
    LightCabinOn,
    LightHeadOn,
    LightLandingOn,
    LightLogoOn,
    LightNavOn,
    LightPanelOn,
    LightRecognitionOn,
    LightStrobeOn,
    LightTaxiOn,
    LightWingOn,
    MasterAlternator,
    MasterBattery,
    MasterIgnitionSwitch,
    MixtureEngine1,
    MixtureEngine2,
    MixtureEngine3,
    MixtureEngine4,
    Nav1ActiveFrequency,
    Nav1StandbyFrequency,
    Nav2ActiveFrequency,
    Nav2StandbyFrequency,
    OverspeedWarning,
    ParkingBrakeIndicator,
    PitotHeat,
    PitotHeatSwitch1,
    PitotHeatSwitch2,
    PitotHeatSwitch3,
    PitotHeatSwitch4,
    PlaneAltitude,
    PlaneAltitudeAGL,
    PlaneBankAngle,
    PlaneHeadingTrue,
    PlaneHeadingMagnetic,
    PlanePitchAngle,
    Propeller1Feathered,
    Propeller2Feathered,
    Propeller3Feathered,
    Propeller4Feathered,
    Propeller1FeatherSw,
    Propeller2FeatherSw,
    Propeller3FeatherSw,
    Propeller4FeatherSw,
    PropellerEngine1,
    PropellerEngine2,
    PropellerEngine3,
    PropellerEngine4,
    RPMN1Engine1,
    RPMN1Engine2,
    RPMN1Engine3,
    RPMN1Engine4,
    RPMPropeller1,
    RPMPropeller2,
    RPMPropeller3,
    RPMPropeller4,
    RudderTrimPct,
    SimulationRate,
    SpoilersArmed,
    SpoilersAvailable,
    SpoilersHandlePosition,
    SpoilersLeftPosition,
    SpoilersRightPosition,
    StallWarning,
    ThrottleEngine1,
    ThrottleEngine2,
    ThrottleEngine3,
    ThrottleEngine4,
    VerticalSpeed,

    #region Landing Gear
    GearTotalExtended,
    #endregion

    #region Trimming
    AileronTrim,
    ElevatorTrim,
    RudderTrim,
    #endregion

    Init = 1000,
  }

  /// <summary>
  /// The SimVarItem which defines all data variables for SimConnect
  /// </summary>
  public class SimVarItem  // TODO: Move to Types folder/namespace
  {
    /// <summary> Unique ID string, used to generate TouchPortal state ID (and possibly other uses). </summary>
    public string Id { get; set; }
    /// <summary> Category for sorting/organizing, also used in TouchPortal state ID. </summary>
    public Groups CategoryId { get; set; } = default;
    /// <summary> Descriptive name for this data (for TouchPortal or other UI). </summary>
    public string Name { get; set; }
    /// <summary> Corresponding SimConnect SimVar name (blank if used for internal purposes). </summary>
    public string SimVarName { get; set; }
    /// <summary> Default value string when no data from SimConnect. </summary>
    public string DefaultValue { get; set; }
    /// <summary> SimConnect settable value (future use) </summary>
    public bool CanSet { get; set; } = false;
    /// <summary> How often updates are sent by SimConnect if value changes (SIMCONNECT_PERIOD). Default is equivalent to SIMCONNECT_PERIOD_SIM_FRAME. </summary>
    public UpdateFreq UpdateFreqency { get; set; } = UpdateFreq.Default;
    /// <summary> Update frequency in ms when UpdateFrequency is set to UpdateFreq.Milliseconds</summary>
    public uint UpdateInterval { get; set; }
    /// <summary> Only report change if it is greater than the value of this parameter (not greater than or equal to). Default is any change. </summary>
    public float DeltaEpsilon { get; set; } = 0.0f;
    /// <summary> Could also be "choice" but we don't use that (yet?) </summary>
    public string TouchPortalValueType { get; set; } = "text";
    /// <summary> This could/should be populated by whatever is creating the SimVarItem instance </summary>
    public string TouchPortalStateId { get; set; }

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
        IsBooleanType = !IsStringType && Units.IsBooleantype(_unit);
        IsIntegralType = !IsStringType && !IsBooleanType && Units.IsIntegraltype(_unit);
        IsRealType = !IsStringType && !IsBooleanType && !IsIntegralType;
        SimConnectDataType = IsStringType ? SIMCONNECT_DATATYPE.STRING64 : IsIntegralType ? SIMCONNECT_DATATYPE.INT64 : IsBooleanType ? SIMCONNECT_DATATYPE.INT32 : SIMCONNECT_DATATYPE.FLOAT64;
        StorageDataType = IsStringType ? typeof(string) : IsIntegralType ? typeof(long) : IsBooleanType ? typeof(uint) : typeof(double);
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
        if (value.StartsWith('{'))
          value = value.Trim('{', '}');
        if (value.StartsWith("0:"))
          value = value.Remove(0, 2);
        _formatString = value;
      }
    }

    /// <summary> The current value as an object. May be null; </summary>
    public object Value
    {
      get => _value;
      set {
        _value = value;
        _valInit = true;
      }
    }

    /// <summary>
    /// Returns the current value as a formatted string according to the value type and StringFormat property.
    /// If no value has been explicitly set, returns the DefaultValue.
    /// </summary>
    public string FormattedValue
    {
      get {
        if (!_valInit)
          return DefaultValue;
        return Value switch {
          double v => string.Format(StringFormat, v),
          uint v => string.Format(StringFormat, v),
          long v => string.Format(StringFormat, v),
          string v => string.Format(StringFormat, v),
          _ => string.Empty,
        };
      }
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
          _value = value == typeof(string) ? string.Empty : System.Activator.CreateInstance(value);
          _valInit = false;
        }
      }
    }

    /// <summary> Returns true if this value is of a real (double) type, false otherwise </summary>
    public bool IsRealType { get; private set; }
    /// <summary> Returns true if this value is of a string type, false if numeric. </summary>
    public bool IsStringType { get; private set; }
    /// <summary> Returns true if this value is of a integer type, false if string or real. </summary>
    public bool IsIntegralType { get; private set; }
    /// <summary> Returns true if this value is of a boolean type, false otherwise </summary>
    public bool IsBooleanType { get; private set; }

    /// <summary> Unique Definition ID for SimConnect </summary>
    public Definition Def { get; set; }  // TODO: make read-only
    /// <summary> The SimConnect data type for registering this var. </summary>
    public SIMCONNECT_DATATYPE SimConnectDataType { get; private set; }
    /// <summary> Indicates if a SimConnect request for this variable is already pending. </summary>
    public bool PendingRequest { get; private set; }
    /// <summary> For serializing the raw value </summary>
    public string FormattingString => _formatString;

    private object _value;
    private bool _valInit;
    private string _unit;
    private string _formatString;
    private long _timeoutTicks;

    private static Definition _nextDefinionId = Definition.Init;
    private static Definition NextId() => ++_nextDefinionId;

    public SimVarItem() {
      Def = NextId();
    }

    public bool ValueEquals(string value) => _valInit && IsStringType && value == (string)Value;
    public bool ValueEquals(double value) => _valInit && IsRealType && System.Math.Abs((double)Value - ConvertValueIfNeeded(value)) <= DeltaEpsilon;
    public bool ValueEquals(long value) => _valInit && IsIntegralType && System.Math.Abs((long)Value - value) <= (long)DeltaEpsilon;
    public bool ValueEquals(uint value) => _valInit && IsBooleanType && System.Math.Abs((uint)Value - value) <= (uint)DeltaEpsilon;

    public bool ValueEquals(object value) {
      if (!_valInit)
        return false;
      try {
        return value switch {
          double v => ValueEquals(v),
          uint v => ValueEquals(v),
          long v => ValueEquals(v),
          _ => ValueEquals(value.ToString()),
        };
      }
      catch {
        return false;
      }
    }

    public bool SetValue(string value) {
      if (IsStringType)
        Value = value;
      return IsStringType;
    }

    public bool SetValue(double value) {
      if (!IsStringType)
        Value = ConvertValueIfNeeded(value);
      return !IsStringType;
    }

    public bool SetValue(long value) {
      if (IsIntegralType)
        Value = value;
      return IsIntegralType;
    }

    public bool SetValue(uint value) {
      if (IsBooleanType)
        Value = value;
      return IsBooleanType;
    }

    public bool SetValue(object value) {
      try {
        return value switch {
          double v => SetValue(v),
          uint v => SetValue(v),
          long v => SetValue(v),
          _ => SetValue(value.ToString()),
        };
      }
      catch {
        return false;
      }
    }

    public double ConvertValueIfNeeded(double value) {
      // Convert to Degrees
      if (Unit == Units.radians)
        return value * (180.0 / System.Math.PI);
      // Convert to actual percentage (percentover100 range is 0 to 1)
      if (Unit == Units.percentover100)
        return value * 100.0;
      // no conversion
      return value;
    }

    /// <summary>
    /// Updates the object to either set pending update or no longer pending
    /// </summary>
    /// <param name="val">True/False</param>
    public void SetPending(bool val) {
      PendingRequest = val;

      if (val) {
        _timeoutTicks = Stopwatch.GetTimestamp() + 30 * Stopwatch.Frequency;
      }
    }

    /// <summary>
    /// If pending for more than 30 seconds, timeout
    /// </summary>
    /// <returns>true if a timeout occurred, false otherwise.</returns>
    public bool PendingTimeout() {
      if (PendingRequest && Stopwatch.GetTimestamp() > _timeoutTicks) {
        SetPending(false);
        return true;
      }
      return false;
    }

    public string ToDebugString() {
      return $"{GetType()}: {{Def: {Def}; SimVarName: {SimVarName}; Unit: {Unit}; Cat: {CategoryId}; Name: {Name}}}";
    }

  }
}
