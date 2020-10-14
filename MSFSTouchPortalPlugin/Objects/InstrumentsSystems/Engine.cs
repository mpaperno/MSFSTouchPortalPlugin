using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Enums;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.InstrumentsSystems {
  [SimVarDataRequestGroup]
  [TouchPortalCategory("Engine", "MSFS - Engine")]
  internal static class EngineMapping {

    #region Ignition

    [SimVarDataRequest]
    [TouchPortalAction("MasterIgnition", "Master Ignition Switch", "MSFS", "Toggle Master Ignition Switch", "Toggle Master Ignition Switch")]
    [TouchPortalState("MasterIgnitionSwitch", "text", "Master Ignition Switch Status", "")]
    public static readonly SimVarItem MASTER_IGNITION = new SimVarItem { Def = Definition.MasterIgnitionSwitch, SimVarName = "MASTER IGNITION SWITCH", Unit = Units.Bool, CanSet = false };

    [TouchPortalAction("EngineAuto", "Engine Auto Start/Shutdown", "MSFS", "Start/Shutdown Engine", "Engine - {0}")]
    [TouchPortalActionChoice(new [] { "Start", "Shutdown" }, "Start")]
    public static object ENGINE_AUTO { get; }

    #endregion

    #region Magneto

    [TouchPortalAction("AllMagenetos", "Toggle All Magnetos", "MSFS", "Toggle All Magnetos", "Toggle All Magnetos - {0}")]
    [TouchPortalActionChoice(new [] { "Start", "Off", "Right", "Left", "Both", "Decrease", "Increase" }, "Start")]
    public static object ALL_MAGNETOS { get; }

    [TouchPortalAction("MagnetoSpecific", "Magnetos Specific", "MSFS", "Toggle Magneto Specific", "Toggle Magneto {0} - {1}")]
    [TouchPortalActionChoice(new [] { "1", "2", "3", "4" }, "1")]
    [TouchPortalActionChoice(new [] { "Start", "Off", "Right", "Left", "Both", "Decrease", "Increase" }, "Start")]
    public static object MAGNETO_SPECIFIC { get; }

    #endregion

    #region Starters

    [TouchPortalAction("Starters", "Toggle Starters", "MSFS", "Toggle Starters", "Toggle Starter - {0}")]
    [TouchPortalActionChoice(new [] { "All", "1", "2", "3", "4" }, "All")]
    public static object STARTERS { get; }

    #endregion

    #region Throttle

    [TouchPortalAction("Throttle", "Throttle", "MSFS", "Sets all throttles", "All Throttles - {0}")]
    [TouchPortalActionChoice(new [] { "Full", "Increase", "Increase Small", "Decrease", "Decrease Small", "Cut", "Set", "10%", "20%", "30%", "40%", "50%", "60%", "70%", "80%", "90%" }, "Full")]
    public static object THROTTLE { get; }

    [TouchPortalAction("ThrottleSpecific", "Throttle Specific", "MSFS", "Sets Throttle on specific engine", "Throttle {0} - {1}")]
    [TouchPortalActionChoice(new [] { "1", "2", "3", "4" }, "1")]
    [TouchPortalActionChoice(new [] { "Full", "Increase", "Increase Small", "Decrease", "Decrease Small", "Cut" }, "Full")]
    public static object THROTTLE_SPECIFIC { get; }


    [SimVarDataRequest]
    [TouchPortalState("ThrottleEngine1", "text", "Throttle - Engine 1 - Percentage", "")]
    public static readonly SimVarItem ThrottleEngine1 = new SimVarItem { Def = Definition.ThrottleEngine1, SimVarName = "GENERAL ENG THROTTLE LEVER POSITION:1", Unit = Units.percent, CanSet = true, StringFormat = "{0:0.#}" };

    [SimVarDataRequest]
    [TouchPortalState("ThrottleEngine2", "text", "Throttle - Engine 2 - Percentage", "")]
    public static readonly SimVarItem ThrottleEngine2 = new SimVarItem { Def = Definition.ThrottleEngine2, SimVarName = "GENERAL ENG THROTTLE LEVER POSITION:2", Unit = Units.percent, CanSet = true, StringFormat = "{0:0.#}" };

    [SimVarDataRequest]
    [TouchPortalState("ThrottleEngine3", "text", "Throttle - Engine 3 - Percentage", "")]
    public static readonly SimVarItem ThrottleEngine3 = new SimVarItem { Def = Definition.ThrottleEngine3, SimVarName = "GENERAL ENG THROTTLE LEVER POSITION:3", Unit = Units.percent, CanSet = true, StringFormat = "{0:0.#}" };

    [SimVarDataRequest]
    [TouchPortalState("ThrottleEngine4", "text", "Throttle - Engine 4 - Percentage", "")]
    public static readonly SimVarItem ThrottleEngine4 = new SimVarItem { Def = Definition.ThrottleEngine4, SimVarName = "GENERAL ENG THROTTLE LEVER POSITION:4", Unit = Units.percent, CanSet = true, StringFormat = "{0:0.#}" };

    #endregion

    #region Mixture

    [TouchPortalAction("Mixture", "Mixture", "MSFS", "Sets all mixtures", "All Mixtures - {0}")]
    [TouchPortalActionChoice(new [] { "Rich", "Increase", "Increase Small", "Decrease", "Decrease Small", "Lean", "Best", "Set" }, "Rich")]
    public static object MIXTURE { get; }

    [TouchPortalAction("MixtureSpecific", "Mixture Specific", "MSFS", "Sets mixture on specific engine", "Mixture {0} - {1}")]
    [TouchPortalActionChoice(new [] { "1", "2", "3", "4" }, "1")]
    [TouchPortalActionChoice(new [] { "Rich", "Increase", "Increase Small", "Decrease", "Decrease Small", "Lean" }, "Rich")]
    public static object MIXTURE_SPECIFIC { get; }

    [SimVarDataRequest]
    [TouchPortalState("MixtureEngine1", "text", "Mixture - Engine 1 - Percentage", "")]
    public static readonly SimVarItem MixtureEngine1 = new SimVarItem { Def = Definition.MixtureEngine1, SimVarName = "GENERAL ENG MIXTURE LEVER POSITION:1", Unit = Units.percent, CanSet = true, StringFormat = "{0:0.0#}" };

    [SimVarDataRequest]
    [TouchPortalState("MixtureEngine2", "text", "Mixture - Engine 2 - Percentage", "")]
    public static readonly SimVarItem MixtureEngine2 = new SimVarItem { Def = Definition.MixtureEngine2, SimVarName = "GENERAL ENG MIXTURE LEVER POSITION:2", Unit = Units.percent, CanSet = true, StringFormat = "{0:0.0#}" };

    [SimVarDataRequest]
    [TouchPortalState("MixtureEngine3", "text", "Mixture - Engine 3 - Percentage", "")]
    public static readonly SimVarItem MixtureEngine3 = new SimVarItem { Def = Definition.MixtureEngine3, SimVarName = "GENERAL ENG MIXTURE LEVER POSITION:3", Unit = Units.percent, CanSet = true, StringFormat = "{0:0.0#}" };

    [SimVarDataRequest]
    [TouchPortalState("MixtureEngine4", "text", "Mixture - Engine 4 - Percentage", "")]
    public static readonly SimVarItem MixtureEngine4 = new SimVarItem { Def = Definition.MixtureEngine4, SimVarName = "GENERAL ENG MIXTURE LEVER POSITION:4", Unit = Units.percent, CanSet = true, StringFormat = "{0:0.0#}" };

    #endregion

    #region Propeller

    [SimVarDataRequest]
    [TouchPortalState("PropellerEngine1", "text", "Propeller - Engine 1 - Percentage", "")]
    public static readonly SimVarItem PropellerEngine1 = new SimVarItem { Def = Definition.PropellerEngine1, SimVarName = "GENERAL ENG PROPELLER LEVER POSITION:1", Unit = Units.percent, CanSet = true, StringFormat = "{0:0.0#}" };

    [SimVarDataRequest]
    [TouchPortalState("PropellerEngine2", "text", "Propeller - Engine 2 - Percentage", "")]
    public static readonly SimVarItem PropellerEngine2 = new SimVarItem { Def = Definition.PropellerEngine2, SimVarName = "GENERAL ENG PROPELLER LEVER POSITION:2", Unit = Units.percent, CanSet = true, StringFormat = "{0:0.0#}" };

    [SimVarDataRequest]
    [TouchPortalState("PropellerEngine3", "text", "Propeller - Engine 3 - Percentage", "")]
    public static readonly SimVarItem PropellerEngine3 = new SimVarItem { Def = Definition.PropellerEngine3, SimVarName = "GENERAL ENG PROPELLER LEVER POSITION:3", Unit = Units.percent, CanSet = true, StringFormat = "{0:0.0#}" };

    [SimVarDataRequest]
    [TouchPortalState("PropellerEngine4", "text", "Propeller - Engine 4 - Percentage", "")]
    public static readonly SimVarItem PropellerEngine4 = new SimVarItem { Def = Definition.PropellerEngine4, SimVarName = "GENERAL ENG PROPELLER LEVER POSITION:4", Unit = Units.percent, CanSet = true, StringFormat = "{0:0.0#}" };

    #endregion

    #region RPM

    [SimVarDataRequest]
    [TouchPortalState("RPMN1Engine1", "text", "RPM - Engine 1", "")]
    public static readonly SimVarItem RPMN1Engine1 = new SimVarItem { Def = Definition.RPMN1Engine1, SimVarName = "ENG N1 RPM:1", Unit = Units.percent, CanSet = true, StringFormat = "{0:0.0#}" };

    [SimVarDataRequest]
    [TouchPortalState("RPMN1Engine2", "text", "RPM - Engine 2", "")]
    public static readonly SimVarItem RPMN1Engine2 = new SimVarItem { Def = Definition.RPMN1Engine2, SimVarName = "ENG N1 RPM:2", Unit = Units.percent, CanSet = true, StringFormat = "{0:0.0#}" };

    [SimVarDataRequest]
    [TouchPortalState("RPMN1Engine3", "text", "RPM - Engine 3", "")]
    public static readonly SimVarItem RPMN1Engine3 = new SimVarItem { Def = Definition.RPMN1Engine3, SimVarName = "ENG N1 RPM:3", Unit = Units.percent, CanSet = true, StringFormat = "{0:0.0#}" };

    [SimVarDataRequest]
    [TouchPortalState("RPMN1Engine4", "text", "RPM - Engine 4", "")]
    public static readonly SimVarItem RPMN1Engine4 = new SimVarItem { Def = Definition.RPMN1Engine4, SimVarName = "ENG N1 RPM:4", Unit = Units.percent, CanSet = true, StringFormat = "{0:0.0#}" };

    [SimVarDataRequest]
    [TouchPortalState("RPMPropeller1", "text", "RPM - Propeller 1", "")]
    public static readonly SimVarItem RPMPropeller1 = new SimVarItem { Def = Definition.RPMPropeller1, SimVarName = "PROP RPM:1", Unit = Units.rpm, CanSet = true, StringFormat = "{0:0.0#}" };

    [SimVarDataRequest]
    [TouchPortalState("RPMPropeller2", "text", "RPM - Propeller 2", "")]
    public static readonly SimVarItem RPMPropeller2 = new SimVarItem { Def = Definition.RPMPropeller2, SimVarName = "PROP RPM:2", Unit = Units.rpm, CanSet = true, StringFormat = "{0:0.0#}" };

    [SimVarDataRequest]
    [TouchPortalState("RPMPropeller3", "text", "RPM - Propeller 3", "")]
    public static readonly SimVarItem RPMPropeller3 = new SimVarItem { Def = Definition.RPMPropeller3, SimVarName = "PROP RPM:3", Unit = Units.rpm, CanSet = true, StringFormat = "{0:0.0#}" };

    [SimVarDataRequest]
    [TouchPortalState("RPMPropeller4", "text", "RPM - Propeller 4", "")]
    public static readonly SimVarItem RPMPropeller4 = new SimVarItem { Def = Definition.RPMPropeller4, SimVarName = "PROP RPM:4", Unit = Units.rpm, CanSet = true, StringFormat = "{0:0.0#}" };

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
    [TouchPortalActionMapping("MagnetoSpecific", new [] { "1", "Off" })]
    MAGNETO1_OFF,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new [] { "1", "Right" })]
    MAGNETO1_RIGHT,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new [] { "1", "Left" })]
    MAGNETO1_LEFT,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new [] { "1", "Both" })]
    MAGNETO1_BOTH,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new [] { "1", "Start" })]
    MAGNETO1_START,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new [] { "1", "Decrease" })]
    MAGNETO1_DECR,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new [] { "1", "Increase" })]
    MAGNETO1_INCR,

    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new [] { "2", "Off" })]
    MAGNETO2_OFF,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new [] { "2", "Right" })]
    MAGNETO2_RIGHT,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new [] { "2", "Left" })]
    MAGNETO2_LEFT,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new [] { "2", "Both" })]
    MAGNETO2_BOTH,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new [] { "2", "Start" })]
    MAGNETO2_START,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new [] { "2", "Decrease" })]
    MAGNETO2_DECR,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new [] { "2", "Increase" })]
    MAGNETO2_INCR,

    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new [] { "3", "Off" })]
    MAGNETO3_OFF,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new [] { "3", "Right" })]
    MAGNETO3_RIGHT,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new [] { "3", "Left" })]
    MAGNETO3_LEFT,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new [] { "3", "Both" })]
    MAGNETO3_BOTH,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new [] { "3", "Start" })]
    MAGNETO3_START,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new [] { "3", "Decrease" })]
    MAGNETO3_DECR,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new [] { "3", "Increase" })]
    MAGNETO3_INCR,

    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new [] { "4", "Off" })]
    MAGNETO4_OFF,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new [] { "4", "Right" })]
    MAGNETO4_RIGHT,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new [] { "4", "Left" })]
    MAGNETO4_LEFT,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new [] { "4", "Both" })]
    MAGNETO4_BOTH,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new [] { "4", "Start" })]
    MAGNETO4_START,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new [] { "4", "Decrease" })]
    MAGNETO4_DECR,
    [SimActionEvent]
    [TouchPortalActionMapping("MagnetoSpecific", new [] { "4", "Increase" })]
    MAGNETO4_INCR,

    MAGNETO_SET,
    MAGNETO1_SET,
    MAGNETO2_SET,
    MAGNETO3_SET,
    MAGNETO4_SET,

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

    [SimActionEvent]
    [TouchPortalActionMapping("ThrottleSpecific", new [] { "1", "Full" })]
    THROTTLE1_FULL,
    [SimActionEvent]
    [TouchPortalActionMapping("ThrottleSpecific", new [] { "1", "Increase" })]
    THROTTLE1_INCR,
    [SimActionEvent]
    [TouchPortalActionMapping("ThrottleSpecific", new [] { "1", "Increase Small" })]
    THROTTLE1_INCR_SMALL,
    [SimActionEvent]
    [TouchPortalActionMapping("ThrottleSpecific", new [] { "1", "Descrease" })]
    THROTTLE1_DECR,
    [SimActionEvent]
    [TouchPortalActionMapping("ThrottleSpecific", new [] { "1", "Descrease Small" })]
    THROTTLE1_DECR_SMALL,
    [SimActionEvent]
    [TouchPortalActionMapping("ThrottleSpecific", new [] { "1", "Cut" })]
    THROTTLE1_CUT,

    [SimActionEvent]
    [TouchPortalActionMapping("ThrottleSpecific", new [] { "2", "Full" })]
    THROTTLE2_FULL,
    [SimActionEvent]
    [TouchPortalActionMapping("ThrottleSpecific", new [] { "2", "Increase" })]
    THROTTLE2_INCR,
    [SimActionEvent]
    [TouchPortalActionMapping("ThrottleSpecific", new [] { "2", "Increase Small" })]
    THROTTLE2_INCR_SMALL,
    [SimActionEvent]
    [TouchPortalActionMapping("ThrottleSpecific", new [] { "2", "Descrease" })]
    THROTTLE2_DECR,
    [SimActionEvent]
    [TouchPortalActionMapping("ThrottleSpecific", new [] { "2", "Descrease Small" })]
    THROTTLE2_DECR_SMALL,
    [SimActionEvent]
    [TouchPortalActionMapping("ThrottleSpecific", new [] { "2", "Cut" })]
    THROTTLE2_CUT,

    [SimActionEvent]
    [TouchPortalActionMapping("ThrottleSpecific", new [] { "3", "Full" })]
    THROTTLE3_FULL,
    [SimActionEvent]
    [TouchPortalActionMapping("ThrottleSpecific", new [] { "3", "Increase" })]
    THROTTLE3_INCR,
    [SimActionEvent]
    [TouchPortalActionMapping("ThrottleSpecific", new [] { "3", "Increase Small" })]
    THROTTLE3_INCR_SMALL,
    [SimActionEvent]
    [TouchPortalActionMapping("ThrottleSpecific", new [] { "3", "Descrease" })]
    THROTTLE3_DECR,
    [SimActionEvent]
    [TouchPortalActionMapping("ThrottleSpecific", new [] { "3", "Descrease Small" })]
    THROTTLE3_DECR_SMALL,
    [SimActionEvent]
    [TouchPortalActionMapping("ThrottleSpecific", new [] { "3", "Cut" })]
    THROTTLE3_CUT,

    [SimActionEvent]
    [TouchPortalActionMapping("ThrottleSpecific", new [] { "4", "Full" })]
    THROTTLE4_FULL,
    [SimActionEvent]
    [TouchPortalActionMapping("ThrottleSpecific", new [] { "4", "Increase" })]
    THROTTLE4_INCR,
    [SimActionEvent]
    [TouchPortalActionMapping("ThrottleSpecific", new [] { "4", "Increase Small" })]
    THROTTLE4_INCR_SMALL,
    [SimActionEvent]
    [TouchPortalActionMapping("ThrottleSpecific", new [] { "4", "Descrease" })]
    THROTTLE4_DECR,
    [SimActionEvent]
    [TouchPortalActionMapping("ThrottleSpecific", new [] { "4", "Descrease Small" })]
    THROTTLE4_DECR_SMALL,
    [SimActionEvent]
    [TouchPortalActionMapping("ThrottleSpecific", new [] { "4", "Cut" })]
    THROTTLE4_CUT,

    THROTTLE1_SET,
    THROTTLE2_SET,
    THROTTLE3_SET,
    THROTTLE4_SET,

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
    [TouchPortalActionMapping("MixtureSpecific", new [] { "1", "Rich" })]
    MIXTURE1_RICH,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new [] { "1", "Increase" })]
    MIXTURE1_INCR,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new [] { "1", "Increase Small" })]
    MIXTURE1_INCR_SMALL,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new [] { "1", "Decrease" })]
    MIXTURE1_DECR,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new [] { "1", "Decrease Small" })]
    MIXTURE1_DECR_SMALL,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new [] { "1", "Lean" })]
    MIXTURE1_LEAN,

    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new [] { "2", "Rich" })]
    MIXTURE2_RICH,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new [] { "2", "Increase" })]
    MIXTURE2_INCR,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new [] { "2", "Increase Small" })]
    MIXTURE2_INCR_SMALL,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new [] { "2", "Decrease" })]
    MIXTURE2_DECR,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new [] { "2", "Decrease Small" })]
    MIXTURE2_DECR_SMALL,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new [] { "2", "Lean" })]
    MIXTURE2_LEAN,

    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new [] { "3", "Rich" })]
    MIXTURE3_RICH,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new [] { "3", "Increase" })]
    MIXTURE3_INCR,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new [] { "3", "Increase Small" })]
    MIXTURE3_INCR_SMALL,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new [] { "3", "Decrease" })]
    MIXTURE3_DECR,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new [] { "3", "Decrease Small" })]
    MIXTURE3_DECR_SMALL,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new [] { "3", "Lean" })]
    MIXTURE3_LEAN,

    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new [] { "4", "Rich" })]
    MIXTURE4_RICH,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new [] { "4", "Increase" })]
    MIXTURE4_INCR,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new [] { "4", "Increase Small" })]
    MIXTURE4_INCR_SMALL,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new [] { "4", "Decrease" })]
    MIXTURE4_DECR,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new [] { "4", "Decrease Small" })]
    MIXTURE4_DECR_SMALL,
    [SimActionEvent]
    [TouchPortalActionMapping("MixtureSpecific", new [] { "4", "Lean" })]
    MIXTURE4_LEAN,

    MIXTURE1_SET,
    MIXTURE2_SET,
    MIXTURE3_SET,
    MIXTURE4_SET,

    #endregion
  }
}
