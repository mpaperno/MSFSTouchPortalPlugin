using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Enums;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.InstrumentsSystems {
  [SimVarDataRequestGroup]
  [TouchPortalCategory("Engine", "MSFS - Engine")]
  internal class EngineMapping {

    #region Ignition

    [TouchPortalAction("MasterIgnition", "Master Ignition Switch", "MSFS", "Toggle Master Ignition Switch", "Toggle Master Ignition Switch")]
    public object MASTER_IGNITION { get; }

    [TouchPortalAction("EngineAuto", "Engine Auto Start/Shutdown", "MSFS", "Start/Shutdown Engine", "Engine - {0}")]
    [TouchPortalActionChoice(new string[] { "Start", "Shutdown" }, "Start")]
    public object ENGINE_AUTO { get; }

    [SimVarDataRequest]
    [TouchPortalState("MasterIgnitionSwitch", "text", "Master Ignition Switch Status", "")]
    public static SimVarItem MasterIgnitionSwitch = new SimVarItem() { def = Definition.MasterIgnitionSwitch, req = Request.MasterIgnitionSwitch, SimVarName = "MASTER IGNITION SWITCH", Unit = Units.Bool, CanSet = false };

    #endregion

    #region Magneto

    [TouchPortalAction("AllMagenetos", "Toggle All Magnetos", "MSFS", "Toggle All Magnetos", "Toggle All Magnetos - {0}")]
    [TouchPortalActionChoice(new string[] { "Start", "Off", "Right", "Left", "Both", "Decrease", "Increase" }, "Start")]
    public object ALL_MAGNETOS { get; }

    [TouchPortalAction("Magneto_1", "Toggle Magneto 1", "MSFS", "Toggle Magneto 1", "Toggle Magneto 1 - {0}")]
    [TouchPortalActionChoice(new string[] { "Start", "Off", "Right", "Left", "Both", "Decrease", "Increase" }, "Start")]
    public object MAGNETO_1 { get; }

    [TouchPortalAction("Magneto_2", "Toggle Magneto 2", "MSFS", "Toggle Magneto 2", "Toggle Magneto 2 - {0}")]
    [TouchPortalActionChoice(new string[] { "Start", "Off", "Right", "Left", "Both", "Decrease", "Increase" }, "Start")]
    public object MAGNETO_2 { get; }

    [TouchPortalAction("Magneto_3", "Toggle Magneto 3", "MSFS", "Toggle Magneto 3", "Toggle Magneto 3 - {0}")]
    [TouchPortalActionChoice(new string[] { "Start", "Off", "Right", "Left", "Both", "Decrease", "Increase" }, "Start")]
    public object MAGNETO_3 { get; }

    [TouchPortalAction("Magneto_4", "Toggle Magneto 4", "MSFS", "Toggle Magneto 4", "Toggle Magneto 4 - {0}")]
    [TouchPortalActionChoice(new string[] { "Start", "Off", "Right", "Left", "Both", "Decrease", "Increase" }, "Start")]
    public object MAGNETO_4 { get; }

    #endregion

    #region Starters

    [TouchPortalAction("Starters", "Toggle Starters", "MSFS", "Toggle Starters", "Toggle Starter - {0}")]
    [TouchPortalActionChoice(new string[] { "All", "1", "2", "3", "4" }, "All")]
    public object STARTERS { get; }

    #endregion
  }

  [SimNotificationGroup(Groups.Engine)]
  [TouchPortalCategoryMapping("Engine")]
  internal enum Engine {
    // Placeholder to offset each enum for SimConnect
    Init = 4000,

    #region Ignition

    [SimActionEvent]
    [TouchPortalActionMapping("MasterIgnition")]
    TOGGLE_MASTER_IGNITION_SWITCH,

    [SimActionEvent]
    [TouchPortalActionMapping("EngineAuto", "Start")]
    ENGINE_AUTO_START,

    [SimActionEvent]
    [TouchPortalActionMapping("EngineAuto", "Shutdown")]
    ENGINE_AUTO_SHUTDOWN,

    #endregion

    #region Magneto

    [SimActionEvent]
    [TouchPortalActionMapping("AllMagenetos", "Off")]
    MAGNETO_OFF,
    [SimActionEvent]
    [TouchPortalActionMapping("AllMagenetos", "Right")]
    MAGNETO_RIGHT,
    [SimActionEvent]
    [TouchPortalActionMapping("AllMagenetos", "Left")]
    MAGNETO_LEFT,
    [SimActionEvent]
    [TouchPortalActionMapping("AllMagenetos", "Both")]
    MAGNETO_BOTH,
    [SimActionEvent]
    [TouchPortalActionMapping("AllMagenetos", "Start")]
    MAGNETO_START,
    [SimActionEvent]
    [TouchPortalActionMapping("AllMagenetos", "Decrease")]
    MAGNETO_DECR,
    [SimActionEvent]
    [TouchPortalActionMapping("AllMagenetos", "Increase")]
    MAGNETO_INCR,

    [SimActionEvent]
    [TouchPortalActionMapping("Magneto_1", "Off")]
    MAGNETO1_OFF,
    [SimActionEvent]
    [TouchPortalActionMapping("Magneto_1", "Right")]
    MAGNETO1_RIGHT,
    [SimActionEvent]
    [TouchPortalActionMapping("Magneto_1", "Left")]
    MAGNETO1_LEFT,
    [SimActionEvent]
    [TouchPortalActionMapping("Magneto_1", "Both")]
    MAGNETO1_BOTH,
    [SimActionEvent]
    [TouchPortalActionMapping("Magneto_1", "Start")]
    MAGNETO1_START,
    [SimActionEvent]
    [TouchPortalActionMapping("Magneto_1", "Decrease")]
    MAGNETO1_DECR,
    [SimActionEvent]
    [TouchPortalActionMapping("Magneto_1", "Increase")]
    MAGNETO1_INCR,

    [SimActionEvent]
    [TouchPortalActionMapping("Magneto_2", "Off")]
    MAGNETO2_OFF,
    [SimActionEvent]
    [TouchPortalActionMapping("Magneto_2", "Right")]
    MAGNETO2_RIGHT,
    [SimActionEvent]
    [TouchPortalActionMapping("Magneto_2", "Left")]
    MAGNETO2_LEFT,
    [SimActionEvent]
    [TouchPortalActionMapping("Magneto_2", "Both")]
    MAGNETO2_BOTH,
    [SimActionEvent]
    [TouchPortalActionMapping("Magneto_2", "Start")]
    MAGNETO2_START,
    [SimActionEvent]
    [TouchPortalActionMapping("Magneto_2", "Decrease")]
    MAGNETO2_DECR,
    [SimActionEvent]
    [TouchPortalActionMapping("Magneto_2", "Increase")]
    MAGNETO2_INCR,

    [SimActionEvent]
    [TouchPortalActionMapping("Magneto_3", "Off")]
    MAGNETO3_OFF,
    [SimActionEvent]
    [TouchPortalActionMapping("Magneto_3", "Right")]
    MAGNETO3_RIGHT,
    [SimActionEvent]
    [TouchPortalActionMapping("Magneto_3", "Left")]
    MAGNETO3_LEFT,
    [SimActionEvent]
    [TouchPortalActionMapping("Magneto_3", "Both")]
    MAGNETO3_BOTH,
    [SimActionEvent]
    [TouchPortalActionMapping("Magneto_3", "Start")]
    MAGNETO3_START,
    [SimActionEvent]
    [TouchPortalActionMapping("Magneto_3", "Decrease")]
    MAGNETO3_DECR,
    [SimActionEvent]
    [TouchPortalActionMapping("Magneto_3", "Increase")]
    MAGNETO3_INCR,

    [SimActionEvent]
    [TouchPortalActionMapping("Magneto_4", "Off")]
    MAGNETO4_OFF,
    [SimActionEvent]
    [TouchPortalActionMapping("Magneto_4", "Right")]
    MAGNETO4_RIGHT,
    [SimActionEvent]
    [TouchPortalActionMapping("Magneto_4", "Left")]
    MAGNETO4_LEFT,
    [SimActionEvent]
    [TouchPortalActionMapping("Magneto_4", "Both")]
    MAGNETO4_BOTH,
    [SimActionEvent]
    [TouchPortalActionMapping("Magneto_4", "Start")]
    MAGNETO4_START,
    [SimActionEvent]
    [TouchPortalActionMapping("Magneto_4", "Decrease")]
    MAGNETO4_DECR,
    [SimActionEvent]
    [TouchPortalActionMapping("Magneto_4", "Increase")]
    MAGNETO4_INCR,

    #endregion

    #region Starters

    [SimActionEvent]
    [TouchPortalActionMapping("Starters", "All")]
    TOGGLE_ALL_STARTERS,

    [SimActionEvent]
    [TouchPortalActionMapping("Starters", "1")]
    TOGGLE_STARTER1,

    [SimActionEvent]
    [TouchPortalActionMapping("Starters", "2")]
    TOGGLE_STARTER2,

    [SimActionEvent]
    [TouchPortalActionMapping("Starters", "3")]
    TOGGLE_STARTER3,

    [SimActionEvent]
    [TouchPortalActionMapping("Starters", "4")]
    TOGGLE_STARTER4,

    #endregion
  }
}
