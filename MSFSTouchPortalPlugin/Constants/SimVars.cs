using System;

namespace MSFSTouchPortalPlugin.Constants {
  public enum Definition {
    Init = 0,
    AileronTrimPct,
    AircraftTitle,
    AirSpeedIndicated,
    AirSpeedMach,
    AirSpeedTrue,
    AntiIceEng1,
    AntiIceEng2,
    AntiIceEng3,
    AntiIceEng4,
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
    AutoThrottleGA,
    AvionicsMasterSwitch,
    Com1ActiveFrequency,
    Com1StandbyFrequency,
    Com2ActiveFrequency,
    Com2StandbyFrequency,
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
    PlaneAltitude,
    PlaneAltitudeAGL,
    PlaneBankAngle,
    PlaneHeadingTrue,
    PlaneHeadingMagnetic,
    PlanePitchAngle,
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
  }

  /// <summary>
  /// The SimVarItem which defines all data variables for SimConnect
  /// </summary>
  public class SimVarItem {
    public bool PendingRequest { get; private set; }
    public Definition Def { get; set; }
    public string SimVarName { get; set; }
    public string Unit { get; set; }
    public bool CanSet { get; set; }
    public DateTime LastPending { get; set; } = DateTime.Now;
    public string Value { get; set; } = string.Empty;
    public string StringFormat { get; set; } = "{0}";
    public string TouchPortalStateId { get; set; } = "";

    /// <summary>
    /// Updates the object to either set pending update or no longer pending
    /// </summary>
    /// <param name="val">True/False</param>
    public void SetPending(bool val) {
      PendingRequest = val;

      if (val) {
        LastPending = DateTime.Now;
      }
    }

    /// <summary>
    /// If pending for more than 30 seconds, timeout
    /// </summary>
    public void PendingTimeout() {
      if (PendingRequest && DateTime.Now > LastPending.AddSeconds(30)) {
        SetPending(false);
      }
    }
  }
}
