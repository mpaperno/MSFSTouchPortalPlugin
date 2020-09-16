using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Enums;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.InstrumentsSystems {
  [SimVarDataRequestGroup]
  [TouchPortalCategory("Engine", "MSFS - Engine")]
  internal class EngineMapping {

    #region Ignition

    [SimVarDataRequest]
    [TouchPortalAction("MasterIgnition", "Master Ignition Switch", "MSFS", "Toggle Master Ignition Switch", "Toggle Master Ignition Switch")]
    [TouchPortalState("MasterIgnitionSwitch", "text", "Master Ignition Switch Status", "")]
    public static SimVarItem MASTER_IGNITION = new SimVarItem() { def = Definition.MasterIgnitionSwitch, req = Request.MasterIgnitionSwitch, SimVarName = "MASTER IGNITION SWITCH", Unit = Units.Bool, CanSet = false };

    [TouchPortalAction("EngineAuto", "Engine Auto Start/Shutdown", "MSFS", "Start/Shutdown Engine", "Engine - {0}")]
    [TouchPortalActionChoice(new string[] { "Start", "Shutdown" }, "Start")]
    public object ENGINE_AUTO { get; }

    #endregion

    #region Magneto

    [TouchPortalAction("AllMagenetos", "Toggle All Magnetos", "MSFS", "Toggle All Magnetos", "Toggle All Magnetos - {0}")]
    [TouchPortalActionChoice(new string[] { "Start", "Off", "Right", "Left", "Both", "Decrease", "Increase" }, "Start")]
    public object ALL_MAGNETOS { get; }

    [TouchPortalAction("MagnetoSpecific", "Magnetos Specific", "MSFS", "Toggle Magneto Specific", "Toggle Magneto {0} - {1}")]
    [TouchPortalActionChoice(new string[] { "1", "2", "3", "4" }, "1")]
    [TouchPortalActionChoice(new string[] { "Start", "Off", "Right", "Left", "Both", "Decrease", "Increase" }, "Start")]
    public object MAGNETO_SPECIFIC { get; }

    #endregion

    #region Starters

    [TouchPortalAction("Starters", "Toggle Starters", "MSFS", "Toggle Starters", "Toggle Starter - {0}")]
    [TouchPortalActionChoice(new string[] { "All", "1", "2", "3", "4" }, "All")]
    public object STARTERS { get; }

    #endregion

    #region Throttle

    [TouchPortalAction("Throttle", "Throttle", "MSFS", "Sets all throttles", "All Throttles - {0}")]
    [TouchPortalActionChoice(new string[] { "Full", "Increase", "Increase Small", "Decrease", "Decrease Small", "Cut", "Set", "10%", "20%", "30%", "40%", "50%", "60%", "70%", "80%", "90%" }, "Full")]
    public object THROTTLE { get; }

    #endregion

    #region Mixture

    [TouchPortalAction("Mixture", "Mixture", "MSFS", "Sets all mixtures", "All Mixtures - {0}")]
    [TouchPortalActionChoice(new string[] { "Rich", "Increase", "Increase Small", "Decrease", "Decrease Small", "Lean", "Best", "Set" }, "Rich")]
    public object MIXTURE { get; }

    [TouchPortalAction("MixtureSpecific", "Mixture Specific", "MSFS", "Sets mixture on specific engine", "Mixture {0} - {1}")]
    [TouchPortalActionChoice(new string[] { "1", "2", "3", "4" }, "1")]
    [TouchPortalActionChoice(new string[] { "Rich", "Increase", "Increase Small", "Decrease", "Decrease Small", "Lean" }, "Rich")]
    public object MIXTURE_SPECIFIC { get; }

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
    [TouchPortalActionMapping("MagnetoSpecific", new string[] { "1", "Off" })]
    MAGNETO1_OFF,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new string[] { "1", "Right" })]
    MAGNETO1_RIGHT,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new string[] { "1", "Left" })]
    MAGNETO1_LEFT,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new string[] { "1", "Both" })]
    MAGNETO1_BOTH,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new string[] { "1", "Start" })]
    MAGNETO1_START,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new string[] { "1", "Decrease" })]
    MAGNETO1_DECR,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new string[] { "1", "Increase" })]
    MAGNETO1_INCR,

    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new string[] { "2", "Off" })]
    MAGNETO2_OFF,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new string[] { "2", "Right" })]
    MAGNETO2_RIGHT,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new string[] { "2", "Left" })]
    MAGNETO2_LEFT,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new string[] { "2", "Both" })]
    MAGNETO2_BOTH,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new string[] { "2", "Start" })]
    MAGNETO2_START,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new string[] { "2", "Decrease" })]
    MAGNETO2_DECR,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new string[] { "2", "Increase" })]
    MAGNETO2_INCR,

    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new string[] { "3", "Off" })]
    MAGNETO3_OFF,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new string[] { "3", "Right" })]
    MAGNETO3_RIGHT,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new string[] { "3", "Left" })]
    MAGNETO3_LEFT,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new string[] { "3", "Both" })]
    MAGNETO3_BOTH,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new string[] { "3", "Start" })]
    MAGNETO3_START,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new string[] { "3", "Decrease" })]
    MAGNETO3_DECR,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new string[] { "3", "Increase" })]
    MAGNETO3_INCR,

    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new string[] { "4", "Off" })]
    MAGNETO4_OFF,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new string[] { "4", "Right" })]
    MAGNETO4_RIGHT,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new string[] { "4", "Left" })]
    MAGNETO4_LEFT,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new string[] { "4", "Both" })]
    MAGNETO4_BOTH,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new string[] { "4", "Start" })]
    MAGNETO4_START,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new string[] { "4", "Decrease" })]
    MAGNETO4_DECR,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new string[] { "4", "Increase" })]
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

    #region Throttle

    [SimActionEvent]
    [TouchPortalActionMapping("Throttle", "Full")]
    THROTTLE_FULL,
    [SimActionEvent]
    [TouchPortalActionMapping("Throttle", "Increase")]
    THROTTLE_INCR,
    [SimActionEvent]
    [TouchPortalActionMapping("Throttle", "Increase Small")]
    THROTTLE_INCR_SMALL,
    [SimActionEvent]
    [TouchPortalActionMapping("Throttle", "Decrease")]
    THROTTLE_DECR,
    [SimActionEvent]
    [TouchPortalActionMapping("Throttle", "Decrease Small")]
    THROTTLE_DECR_SMALL,
    [SimActionEvent]
    [TouchPortalActionMapping("Throttle", "Cut")]
    THROTTLE_CUT,
    [SimActionEvent]
    [TouchPortalActionMapping("Throttle", "Set")]
    THROTTLE_SET,
    [SimActionEvent]
    [TouchPortalActionMapping("Throttle", "10%")]
    THROTTLE_10,
    [SimActionEvent]
    [TouchPortalActionMapping("Throttle", "20%")]
    THROTTLE_20,
    [SimActionEvent]
    [TouchPortalActionMapping("Throttle", "30%")]
    THROTTLE_30,
    [SimActionEvent]
    [TouchPortalActionMapping("Throttle", "40%")]
    THROTTLE_40,
    [SimActionEvent]
    [TouchPortalActionMapping("Throttle", "50%")]
    THROTTLE_50,
    [SimActionEvent]
    [TouchPortalActionMapping("Throttle", "60%")]
    THROTTLE_60,
    [SimActionEvent]
    [TouchPortalActionMapping("Throttle", "70%")]
    THROTTLE_70,
    [SimActionEvent]
    [TouchPortalActionMapping("Throttle", "80%")]
    THROTTLE_80,
    [SimActionEvent]
    [TouchPortalActionMapping("Throttle", "90%")]
    THROTTLE_90,

    #endregion

    #region Mixture

    [SimActionEvent]
    [TouchPortalActionMapping("Mixture", "Set")]
    MIXTURE_SET,
    [SimActionEvent]
    [TouchPortalActionMapping("Mixture", "Rich")]
    MIXTURE_RICH,
    [SimActionEvent]
    [TouchPortalActionMapping("Mixture", "Increase")]
    MIXTURE_INCR,
    [SimActionEvent]
    [TouchPortalActionMapping("Mixture", "Increase Small")]
    MIXTURE_INCR_SMALL,
    [SimActionEvent]
    [TouchPortalActionMapping("Mixture", "Decrease")]
    MIXTURE_DECR,
    [SimActionEvent]
    [TouchPortalActionMapping("Mixture", "Decrease Small")]
    MIXTURE_DECR_SMALL,
    [SimActionEvent]
    [TouchPortalActionMapping("Mixture", "Lean")]
    MIXTURE_LEAN,
    [SimActionEvent]
    [TouchPortalActionMapping("Mixture", "Best")]
    MIXTURE_SET_BEST,

    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new string[] { "1", "Rich" })]
    MIXTURE1_RICH,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new string[] { "1", "Increase" })]
    MIXTURE1_INCR,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new string[] { "1", "Increase Small" })]
    MIXTURE1_INCR_SMALL,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new string[] { "1", "Decrease" })]
    MIXTURE1_DECR,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new string[] { "1", "Decrease Small" })]
    MIXTURE1_DECR_SMALL,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new string[] { "1", "Lean" })]
    MIXTURE1_LEAN,

    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new string[] { "2", "Rich" })]
    MIXTURE2_RICH,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new string[] { "2", "Increase" })]
    MIXTURE2_INCR,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new string[] { "2", "Increase Small" })]
    MIXTURE2_INCR_SMALL,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new string[] { "2", "Decrease" })]
    MIXTURE2_DECR,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new string[] { "2", "Decrease Small" })]
    MIXTURE2_DECR_SMALL,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new string[] { "2", "Lean" })]
    MIXTURE2_LEAN,

    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new string[] { "3", "Rich" })]
    MIXTURE3_RICH,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new string[] { "3", "Increase" })]
    MIXTURE3_INCR,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new string[] { "3", "Increase Small" })]
    MIXTURE3_INCR_SMALL,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new string[] { "3", "Decrease" })]
    MIXTURE3_DECR,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new string[] { "3", "Decrease Small" })]
    MIXTURE3_DECR_SMALL,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new string[] { "3", "Lean" })]
    MIXTURE3_LEAN,

    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new string[] { "4", "Rich" })]
    MIXTURE4_RICH,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new string[] { "4", "Increase" })]
    MIXTURE4_INCR,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new string[] { "4", "Increase Small" })]
    MIXTURE4_INCR_SMALL,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new string[] { "4", "Decrease" })]
    MIXTURE4_DECR,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new string[] { "4", "Decrease Small" })]
    MIXTURE4_DECR_SMALL,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new string[] { "4", "Lean" })]
    MIXTURE4_LEAN,

    MIXTURE1_SET,
    MIXTURE2_SET,
    MIXTURE3_SET,
    MIXTURE4_SET,

    #endregion
  }
}
