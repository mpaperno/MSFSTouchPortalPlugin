using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Enums;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.SimSystem {
  [SimVarDataRequestGroup]
  [TouchPortalCategory("SimSystem", "MSFS - System")]
  internal static class SimSystemMapping {
    [SimVarDataRequest]
    [TouchPortalAction("SimulationRate", "Simulation Rate", "MSFS", "Simulation Rate", "Rate {0}")]
    [TouchPortalActionChoice(new [] { "Increase", "Decrease" }, "Decrease")]
    [TouchPortalState("SimulationRate", "text", "The current simulation rate", "")]
    public static readonly SimVarItem SimulationRate =
      new SimVarItem { Def = Definition.SimulationRate, SimVarName = "SIMULATION RATE", Unit = Units.number, CanSet = false };

    [SimVarDataRequest]
    [TouchPortalAction("SelectedParameter", "Change Selected Value (+/-)", "MSFS", "Selected Value", "Value {0}", true)]
    [TouchPortalActionChoice(new[] { "Increase", "Decrease" }, "Decrease")]
    public static object SELECTED_PARAMETER_CHANGE { get; }

    [SimVarDataRequest]
    [TouchPortalState("AtcType", "text", "Type of aircraft used by ATC", "")]
    public static readonly SimVarItem AtcType = new SimVarItem { Def = Definition.AtcType, SimVarName = "ATC TYPE", Unit = Units.String, CanSet = false };

    [SimVarDataRequest]
    [TouchPortalState("AtcModel", "text", "Model of aircraft used by ATC", "")]
    public static readonly SimVarItem AtcModel = new SimVarItem { Def = Definition.AtcModel, SimVarName = "ATC MODEL", Unit = Units.String, CanSet = false };

    [SimVarDataRequest]
    [TouchPortalState("AtcId", "text", "Aircraft Id used by ATC", "")]
    public static readonly SimVarItem AtcId = new SimVarItem { Def = Definition.AtcId, SimVarName = "ATC ID", Unit = Units.String, CanSet = true };

    [SimVarDataRequest]
    [TouchPortalState("AtcAirline", "text", "Airline used by ATC", "")]
    public static readonly SimVarItem AtcAirline = new SimVarItem { Def = Definition.AtcAirline, SimVarName = "ATC AIRLINE", Unit = Units.String, CanSet = true };

    [SimVarDataRequest]
    [TouchPortalState("AtcFlightNumber", "text", "Flight Number used by ATC", "")]
    public static readonly SimVarItem AtcFlightNumber = new SimVarItem { Def = Definition.AtcFlightNumber, SimVarName = "ATC FLIGHT NUMBER", Unit = Units.String, CanSet = true };

    [SimVarDataRequest]
    [TouchPortalState("AircraftTitle", "text", "Aircraft Title", "")]
    public static readonly SimVarItem AircraftTitle = new SimVarItem { Def = Definition.AircraftTitle, SimVarName = "TITLE", Unit = Units.String, CanSet = false };
  }

  [SimNotificationGroup(Groups.SimSystem)]
  [TouchPortalCategoryMapping("SimSystem")]
  internal enum SimSystem {
    // Placeholder to offset each enum for SimConnect
    Init = 3000,

    #region KeySelect

    [SimActionEvent]
    [TouchPortalActionMapping("SelectedParameter", "Increase")]
    PLUS,
    [SimActionEvent]
    [TouchPortalActionMapping("SelectedParameter", "Decrease")]
    MINUS,

    #endregion

    #region SimRate

    [SimActionEvent]
    [TouchPortalActionMapping("SimulationRate", "Increase")]
    SIM_RATE_INCR,
    [SimActionEvent]
    [TouchPortalActionMapping("SimulationRate", "Decrease")]
    SIM_RATE_DECR,

    #endregion
  }
}
