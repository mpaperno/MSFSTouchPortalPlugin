using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Enums;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.InstrumentsSystems
{
  [SimVarDataRequestGroup]
  [SimNotificationGroup(Groups.Engine)]
  [TouchPortalCategory("Engine", "MSFS - Engine")]
  internal static class EngineMapping {

    #region Ignition

    [SimVarDataRequest]
    [TouchPortalAction("MasterIgnition", "Master Ignition Switch", "MSFS", "Toggle Master Ignition Switch", "Toggle Master Ignition Switch")]
    [TouchPortalActionMapping("TOGGLE_MASTER_IGNITION_SWITCH")]
    [TouchPortalState("MasterIgnitionSwitch", "text", "Master Ignition Switch Status", "")]
    public static readonly SimVarItem MASTER_IGNITION = new SimVarItem { Def = Definition.MasterIgnitionSwitch, SimVarName = "MASTER IGNITION SWITCH", Unit = Units.Bool, CanSet = false };

    [TouchPortalAction("EngineAuto", "Engine Auto Start/Shutdown", "MSFS", "Start/Shutdown Engine", "Engine - {0}")]
    [TouchPortalActionChoice(new [] { "Start", "Shutdown" }, "Start")]
    [TouchPortalActionMapping("ENGINE_AUTO_START", "Start")]
    [TouchPortalActionMapping("ENGINE_AUTO_SHUTDOWN", "Shutdown")]
    public static object ENGINE_AUTO { get; }

    #endregion

    #region Magneto

    [TouchPortalAction("AllMagenetos", "Toggle All Magnetos", "MSFS", "Toggle All Magnetos", "Toggle All Magnetos - {0}")]
    [TouchPortalActionChoice(new [] { "Start", "Off", "Right", "Left", "Both", "Decrease", "Increase", "Select (for +/-)" })]
    [TouchPortalActionMapping("MAGNETO_OFF", "Off")]
    [TouchPortalActionMapping("MAGNETO_RIGHT", "Right")]
    [TouchPortalActionMapping("MAGNETO_LEFT", "Left")]
    [TouchPortalActionMapping("MAGNETO_BOTH", "Both")]
    [TouchPortalActionMapping("MAGNETO_START", "Start")]
    [TouchPortalActionMapping("MAGNETO_DECR", "Decrease")]
    [TouchPortalActionMapping("MAGNETO_INCR", "Increase")]
    [TouchPortalActionMapping("MAGNETO", "Select (for +/-)")]
    public static object ALL_MAGNETOS { get; }

    [TouchPortalAction("MagnetoSpecific", "Magnetos Specific", "MSFS", "Toggle Magneto Specific", "Toggle Magneto {0} - {1}")]
    [TouchPortalActionChoice(new [] { "1", "2", "3", "4" }, "1")]
    [TouchPortalActionChoice(new [] { "Start", "Off", "Right", "Left", "Both", "Decrease", "Increase" }, "Start")]
    [TouchPortalActionMapping("MAGNETO1_OFF", new[] { "1", "Off" })]
    [TouchPortalActionMapping("MAGNETO1_RIGHT", new[] { "1", "Right" })]
    [TouchPortalActionMapping("MAGNETO1_LEFT", new[] { "1", "Left" })]
    [TouchPortalActionMapping("MAGNETO1_BOTH", new[] { "1", "Both" })]
    [TouchPortalActionMapping("MAGNETO1_START", new[] { "1", "Start" })]
    [TouchPortalActionMapping("MAGNETO1_DECR", new[] { "1", "Decrease" })]
    [TouchPortalActionMapping("MAGNETO1_INCR", new[] { "1", "Increase" })]

    [TouchPortalActionMapping("MAGNETO2_OFF", new[] { "2", "Off" })]
    [TouchPortalActionMapping("MAGNETO2_RIGHT", new[] { "2", "Right" })]
    [TouchPortalActionMapping("MAGNETO2_LEFT", new[] { "2", "Left" })]
    [TouchPortalActionMapping("MAGNETO2_BOTH", new[] { "2", "Both" })]
    [TouchPortalActionMapping("MAGNETO2_START", new[] { "2", "Start" })]
    [TouchPortalActionMapping("MAGNETO2_DECR", new[] { "2", "Decrease" })]
    [TouchPortalActionMapping("MAGNETO2_INCR", new[] { "2", "Increase" })]

    [TouchPortalActionMapping("MAGNETO3_OFF", new[] { "3", "Off" })]
    [TouchPortalActionMapping("MAGNETO3_RIGHT", new[] { "3", "Right" })]
    [TouchPortalActionMapping("MAGNETO3_LEFT", new[] { "3", "Left" })]
    [TouchPortalActionMapping("MAGNETO3_BOTH", new[] { "3", "Both" })]
    [TouchPortalActionMapping("MAGNETO3_START", new[] { "3", "Start" })]
    [TouchPortalActionMapping("MAGNETO3_DECR", new[] { "3", "Decrease" })]
    [TouchPortalActionMapping("MAGNETO3_INCR", new[] { "3", "Increase" })]

    [TouchPortalActionMapping("MAGNETO4_OFF", new[] { "4", "Off" })]
    [TouchPortalActionMapping("MAGNETO4_RIGHT", new[] { "4", "Right" })]
    [TouchPortalActionMapping("MAGNETO4_LEFT", new[] { "4", "Left" })]
    [TouchPortalActionMapping("MAGNETO4_BOTH", new[] { "4", "Both" })]
    [TouchPortalActionMapping("MAGNETO4_START", new[] { "4", "Start" })]
    [TouchPortalActionMapping("MAGNETO4_DECR", new[] { "4", "Decrease" })]
    [TouchPortalActionMapping("MAGNETO4_INCR", new[] { "4", "Increase" })]
    public static object MAGNETO_SPECIFIC { get; }

    [TouchPortalAction("MagnetoSet", "Magnetos Set", "MSFS", "Set Magneto Switch", "Magneto Switch {0} to {1}")]
    [TouchPortalActionChoice(new[] { "All", "1", "2", "3", "4" })]
    [TouchPortalActionNumeric(1, 1, 5)]
    [TouchPortalActionMapping("MAGNETO_SET", "All")]
    [TouchPortalActionMapping("MAGNETO1_SET", "1")]
    [TouchPortalActionMapping("MAGNETO2_SET", "2")]
    [TouchPortalActionMapping("MAGNETO3_SET", "3")]
    [TouchPortalActionMapping("MAGNETO4_SET", "4")]
    public static object MAGNETO_SET { get; }

    #endregion

    #region Starters

    [TouchPortalAction("Starters", "Toggle Starters", "MSFS", "Toggle Starters", "Toggle Starter - {0}")]
    [TouchPortalActionChoice(new [] { "All", "1", "2", "3", "4" }, "All")]
    [TouchPortalActionMapping("TOGGLE_ALL_STARTERS", "All")]
    [TouchPortalActionMapping("TOGGLE_STARTER1", "1")]
    [TouchPortalActionMapping("TOGGLE_STARTER2", "2")]
    [TouchPortalActionMapping("TOGGLE_STARTER3", "3")]
    [TouchPortalActionMapping("TOGGLE_STARTER4", "4")]
    public static object STARTERS { get; }

    #endregion

    #region Throttle

    [TouchPortalAction("Throttle", "Throttle", "MSFS", "Sets all throttles", "All Throttles - {0}")]
    [TouchPortalActionChoice(new [] { "Full", "Increase", "Increase Small", "Decrease", "Decrease Small", "Cut", "10%", "20%", "30%", "40%", "50%", "60%", "70%", "80%", "90%" })]
    [TouchPortalActionMapping("THROTTLE_FULL", "Full")]
    [TouchPortalActionMapping("THROTTLE_INCR", "Increase")]
    [TouchPortalActionMapping("THROTTLE_INCR_SMALL", "Increase Small")]
    [TouchPortalActionMapping("THROTTLE_DECR", "Decrease")]
    [TouchPortalActionMapping("THROTTLE_DECR_SMALL", "Decrease Small")]
    [TouchPortalActionMapping("THROTTLE_CUT", "Cut")]
    [TouchPortalActionMapping("THROTTLE_10", "10%")]
    [TouchPortalActionMapping("THROTTLE_20", "20%")]
    [TouchPortalActionMapping("THROTTLE_30", "30%")]
    [TouchPortalActionMapping("THROTTLE_40", "40%")]
    [TouchPortalActionMapping("THROTTLE_50", "50%")]
    [TouchPortalActionMapping("THROTTLE_60", "60%")]
    [TouchPortalActionMapping("THROTTLE_70", "70%")]
    [TouchPortalActionMapping("THROTTLE_80", "80%")]
    [TouchPortalActionMapping("THROTTLE_90", "90%")]
    public static object THROTTLE { get; }

    [TouchPortalAction("ThrottleSpecific", "Throttle Specific", "MSFS", "Sets Throttle on specific engine", "Throttle {0} - {1}", true)]
    [TouchPortalActionChoice(new [] { "1", "2", "3", "4" }, "1")]
    [TouchPortalActionChoice(new [] { "Full", "Increase", "Increase Small", "Decrease", "Decrease Small", "Cut"})]
    [TouchPortalActionMapping("THROTTLE1_FULL",       new[] { "1", "Full" })]
    [TouchPortalActionMapping("THROTTLE1_INCR",       new[] { "1", "Increase" })]
    [TouchPortalActionMapping("THROTTLE1_INCR_SMALL", new[] { "1", "Increase Small" })]
    [TouchPortalActionMapping("THROTTLE1_DECR",       new[] { "1", "Descrease" })]
    [TouchPortalActionMapping("THROTTLE1_DECR_SMALL", new[] { "1", "Descrease Small" })]
    [TouchPortalActionMapping("THROTTLE1_CUT",        new[] { "1", "Cut" })]
    [TouchPortalActionMapping("THROTTLE2_FULL",       new[] { "2", "Full" })]
    [TouchPortalActionMapping("THROTTLE2_INCR",       new[] { "2", "Increase" })]
    [TouchPortalActionMapping("THROTTLE2_INCR_SALL",  new[] { "2", "Increase Small" })]
    [TouchPortalActionMapping("THROTTLE2_DECR",       new[] { "2", "Descrease" })]
    [TouchPortalActionMapping("THROTTLE2_DECR_SALL",  new[] { "2", "Descrease Small" })]
    [TouchPortalActionMapping("THROTTLE2_CUT",        new[] { "2", "Cut" })]
    [TouchPortalActionMapping("THROTTLE3_FULL",       new[] { "3", "Full" })]
    [TouchPortalActionMapping("THROTTLE3_INCR",       new[] { "3", "Increase" })]
    [TouchPortalActionMapping("THROTTLE3_INCR_SMALL", new[] { "3", "Increase Small" })]
    [TouchPortalActionMapping("THROTTLE3_DECR",       new[] { "3", "Descrease" })]
    [TouchPortalActionMapping("THROTTLE3_DECR_SMALL", new[] { "3", "Descrease Small" })]
    [TouchPortalActionMapping("THROTTLE3_CUT",        new[] { "3", "Cut" })]
    [TouchPortalActionMapping("THROTTLE4_FULL",       new[] { "4", "Full" })]
    [TouchPortalActionMapping("THROTTLE4_INCR",       new[] { "4", "Increase" })]
    [TouchPortalActionMapping("THROTTLE4_INCR_SMALL", new[] { "4", "Increase Small" })]
    [TouchPortalActionMapping("THROTTLE4_DECR",       new[] { "4", "Descrease" })]
    [TouchPortalActionMapping("THROTTLE4_DECR_SMALL", new[] { "4", "Descrease Small" })]
    [TouchPortalActionMapping("THROTTLE4_CUT",        new[] { "4", "Cut" })]
    public static object THROTTLE_SPECIFIC { get; }

    [TouchPortalAction("ThrottleSet", "Throttle Set", "MSFS", "Sets all or specific Throttle(s) to specific value", "Set Throttle {0} to {1}")]
    [TouchPortalActionChoice(new[] { "All", "1", "2", "3", "4" }, "All")]
    [TouchPortalActionText("0", 0, 100)]
    [TouchPortalActionMapping("THROTTLE_SET",  new[] { "All" })]
    [TouchPortalActionMapping("THROTTLE1_SET", new[] { "1" })]
    [TouchPortalActionMapping("THROTTLE2_SET", new[] { "2" })]
    [TouchPortalActionMapping("THROTTLE3_SET", new[] { "3" })]
    [TouchPortalActionMapping("THROTTLE4_SET", new[] { "4" })]
    public static object THROTTLE_SET { get; }


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

    [TouchPortalAction("Mixture", "Mixture", "MSFS", "Sets all mixtures", "All Mixtures - {0}", true)]
    [TouchPortalActionChoice(new [] { "Rich", "Increase", "Increase Small", "Decrease", "Decrease Small", "Lean", "Best"})]
    [TouchPortalActionMapping("MIXTURE_RICH", "Rich")]
    [TouchPortalActionMapping("MIXTURE_INCR", "Increase")]
    [TouchPortalActionMapping("MIXTURE_INCR_SMALL", "Increase Small")]
    [TouchPortalActionMapping("MIXTURE_DECR", "Decrease")]
    [TouchPortalActionMapping("MIXTURE_DECR_SMALL", "Decrease Small")]
    [TouchPortalActionMapping("MIXTURE_LEAN", "Lean")]
    [TouchPortalActionMapping("MIXTURE_SET_BEST", "Best")]
    public static object MIXTURE { get; }

    [TouchPortalAction("MixtureSpecific", "Mixture Specific", "MSFS", "Sets mixture on specific engine", "Mixture {0} - {1}", true)]
    [TouchPortalActionChoice(new [] { "1", "2", "3", "4" }, "1")]
    [TouchPortalActionChoice(new [] { "Rich", "Increase", "Increase Small", "Decrease", "Decrease Small", "Lean" })]
    [TouchPortalActionMapping("MIXTURE1_RICH", new[] { "1", "Rich" })]
    [TouchPortalActionMapping("MIXTURE1_INCR", new[] { "1", "Increase" })]
    [TouchPortalActionMapping("MIXTURE1_INCR_SMALL", new[] { "1", "Increase Small" })]
    [TouchPortalActionMapping("MIXTURE1_DECR", new[] { "1", "Decrease" })]
    [TouchPortalActionMapping("MIXTURE1_DECR_SMALL", new[] { "1", "Decrease Small" })]
    [TouchPortalActionMapping("MIXTURE1_LEAN", new[] { "1", "Lean" })]

    [TouchPortalActionMapping("MIXTURE2_RICH", new[] { "2", "Rich" })]
    [TouchPortalActionMapping("MIXTURE2_INCR", new[] { "2", "Increase" })]
    [TouchPortalActionMapping("MIXTURE2_INCR_SMALL", new[] { "2", "Increase Small" })]
    [TouchPortalActionMapping("MIXTURE2_DECR", new[] { "2", "Decrease" })]
    [TouchPortalActionMapping("MIXTURE2_DECR_SMALL", new[] { "2", "Decrease Small" })]
    [TouchPortalActionMapping("MIXTURE2_LEAN", new[] { "2", "Lean" })]

    [TouchPortalActionMapping("MIXTURE3_RICH", new[] { "3", "Rich" })]
    [TouchPortalActionMapping("MIXTURE3_INCR", new[] { "3", "Increase" })]
    [TouchPortalActionMapping("MIXTURE3_INCR_SMALL", new[] { "3", "Increase Small" })]
    [TouchPortalActionMapping("MIXTURE3_DECR", new[] { "3", "Decrease" })]
    [TouchPortalActionMapping("MIXTURE3_DECR_SMALL", new[] { "3", "Decrease Small" })]
    [TouchPortalActionMapping("MIXTURE3_LEAN", new[] { "3", "Lean" })]

    [TouchPortalActionMapping("MIXTURE4_RICH", new[] { "4", "Rich" })]
    [TouchPortalActionMapping("MIXTURE4_INCR", new[] { "4", "Increase" })]
    [TouchPortalActionMapping("MIXTURE4_INCR_SMALL", new[] { "4", "Increase Small" })]
    [TouchPortalActionMapping("MIXTURE4_DECR", new[] { "4", "Decrease" })]
    [TouchPortalActionMapping("MIXTURE4_DECR_SMALL", new[] { "4", "Decrease Small" })]
    [TouchPortalActionMapping("MIXTURE4_LEAN", new[] { "4", "Lean" })]
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

    [TouchPortalAction("PropellerPitch", "Propeller Pitch", "MSFS", "Adjusts propeller pitch levers/feather", "Pitch {0} - {1}", true)]
    [TouchPortalActionChoice(new[] {"All", "1", "2", "3", "4" })]
    [TouchPortalActionChoice(new[] { "Increment", "Increment Small", "Decrement", "Decrement Small", "Min (hi pitch)", "Max (lo pitch)", "Toggle Feather Switch" })]
    [TouchPortalActionMapping("PROP_PITCH_INCR", new[] { "All", "Increment" })]
    [TouchPortalActionMapping("PROP_PITCH_INCR_SMALL", new[] { "All", "Increment Small" })]
    [TouchPortalActionMapping("PROP_PITCH_DECR", new[] { "All", "Decrement" })]
    [TouchPortalActionMapping("PROP_PITCH_DECR_SMALL", new[] { "All", "Decrement Small" })]
    [TouchPortalActionMapping("PROP_PITCH_HI", new[] { "All", "Min (hi pitch)" })]
    [TouchPortalActionMapping("PROP_PITCH_LO", new[] { "All", "Max (lo pitch)" })]
    [TouchPortalActionMapping("TOGGLE_FEATHER_SWITCHES", new[] { "All", "Toggle Feather Switch" })]

    [TouchPortalActionMapping("PROP_PITCH1_INCR", new[] { "1", "Increment" })]
    [TouchPortalActionMapping("PROP_PITCH1_INCR_SMALL", new[] { "1", "Increment Small" })]
    [TouchPortalActionMapping("PROP_PITCH1_DECR", new[] { "1", "Decrement" })]
    [TouchPortalActionMapping("PROP_PITCH1_DECR_SMALL", new[] { "1", "Decrement Small" })]
    [TouchPortalActionMapping("PROP_PITCH1_HI", new[] { "1", "Min (hi pitch)" })]
    [TouchPortalActionMapping("PROP_PITCH1_LO", new[] { "1", "Max (lo pitch)" })]
    [TouchPortalActionMapping("TOGGLE_FEATHER_SWITCH_1", new[] { "1", "Toggle Feather Switch" })]

    [TouchPortalActionMapping("PROP_PITCH2_INCR", new[] { "2", "Increment" })]
    [TouchPortalActionMapping("PROP_PITCH2_INCR_SMALL", new[] { "2", "Increment Small" })]
    [TouchPortalActionMapping("PROP_PITCH2_DECR", new[] { "2", "Decrement" })]
    [TouchPortalActionMapping("PROP_PITCH2_DECR_SMALL", new[] { "2", "Decrement Small" })]
    [TouchPortalActionMapping("PROP_PITCH2_HI", new[] { "2", "Min (hi pitch)" })]
    [TouchPortalActionMapping("PROP_PITCH2_LO", new[] { "2", "Max (lo pitch)" })]
    [TouchPortalActionMapping("TOGGLE_FEATHER_SWITCH_2", new[] { "2", "Toggle Feather Switch" })]

    [TouchPortalActionMapping("PROP_PITCH3_INCR", new[] { "3", "Increment" })]
    [TouchPortalActionMapping("PROP_PITCH3_INCR_SMALL", new[] { "3", "Increment Small" })]
    [TouchPortalActionMapping("PROP_PITCH3_DECR", new[] { "3", "Decrement" })]
    [TouchPortalActionMapping("PROP_PITCH3_DECR_SMALL", new[] { "3", "Decrement Small" })]
    [TouchPortalActionMapping("PROP_PITCH3_HI", new[] { "3", "Min (hi pitch)" })]
    [TouchPortalActionMapping("PROP_PITCH3_LO", new[] { "3", "Max (lo pitch)" })]
    [TouchPortalActionMapping("TOGGLE_FEATHER_SWITCH_3", new[] { "3", "Toggle Feather Switch" })]

    [TouchPortalActionMapping("PROP_PITCH4_INCR", new[] { "4", "Increment" })]
    [TouchPortalActionMapping("PROP_PITCH4_INCR_SMALL", new[] { "4", "Increment Small" })]
    [TouchPortalActionMapping("PROP_PITCH4_DECR", new[] { "4", "Decrement" })]
    [TouchPortalActionMapping("PROP_PITCH4_DECR_SMALL", new[] { "4", "Decrement Small" })]
    [TouchPortalActionMapping("PROP_PITCH4_HI", new[] { "4", "Min (hi pitch)" })]
    [TouchPortalActionMapping("PROP_PITCH4_LO", new[] { "4", "Max (lo pitch)" })]
    [TouchPortalActionMapping("TOGGLE_FEATHER_SWITCH_4", new[] { "4", "Toggle Feather Switch" })]
    public static object PROPELLER_PITCH { get; }

    [TouchPortalAction("PropellerPitchSet", "Propeller Pitch Set", "MSFS", "Sets propeller pitch lever to value (0 to 16383)", "Swt Prop {0} Pitch to {1}")]
    [TouchPortalActionChoice(new[] { "All", "1", "2", "3", "4" })]
    [TouchPortalActionText("0", 0, 16383)]
    [TouchPortalActionMapping("PROP_PITCH_SET", "All")]
    [TouchPortalActionMapping("PROP_PITCH1_SET", "1")]
    [TouchPortalActionMapping("PROP_PITCH2_SET", "2")]
    [TouchPortalActionMapping("PROP_PITCH3_SET", "3")]
    [TouchPortalActionMapping("PROP_PITCH4_SET", "4")]
    public static object PROPELLER_PITCH_SET { get; }

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

    [SimVarDataRequest]
    [TouchPortalState("Propeller1FeatherSw", "text", "Propeller - Engine 1 - Feather Switch State (bool)", "")]
    public static readonly SimVarItem Propeller1FeatherSw = new SimVarItem { Def = Definition.Propeller1FeatherSw, SimVarName = "PROP FEATHER SWITCH:1", Unit = Units.Bool, CanSet = false };
    [SimVarDataRequest]
    [TouchPortalState("Propeller2FeatherSw", "text", "Propeller - Engine 2 - Feather Switch State (bool)", "")]
    public static readonly SimVarItem Propeller2FeatherSw = new SimVarItem { Def = Definition.Propeller2FeatherSw, SimVarName = "PROP FEATHER SWITCH:2", Unit = Units.Bool, CanSet = false };
    [SimVarDataRequest]
    [TouchPortalState("Propeller3FeatherSw", "text", "Propeller - Engine 3 - Feather Switch State (bool)", "")]
    public static readonly SimVarItem Propeller3FeatherSw = new SimVarItem { Def = Definition.Propeller3FeatherSw, SimVarName = "PROP FEATHER SWITCH:3", Unit = Units.Bool, CanSet = false };
    [SimVarDataRequest]
    [TouchPortalState("Propeller4FeatherSw", "text", "Propeller - Engine 4 - Feather Switch State (bool)", "")]
    public static readonly SimVarItem Propeller4FeatherSw = new SimVarItem { Def = Definition.Propeller4FeatherSw, SimVarName = "PROP FEATHER SWITCH:4", Unit = Units.Bool, CanSet = false };

    [SimVarDataRequest]
    [TouchPortalState("Propeller1Feathered", "text", "Propeller - Engine 1 - Feathered (bool)", "")]
    public static readonly SimVarItem Propeller1Feathered = new SimVarItem { Def = Definition.Propeller1Feathered, SimVarName = "PROP FEATHERED:1", Unit = Units.Bool, CanSet = false };
    [SimVarDataRequest]
    [TouchPortalState("Propeller2Feathered", "text", "Propeller - Engine 2 - Feathered (bool)", "")]
    public static readonly SimVarItem Propeller2Feathered = new SimVarItem { Def = Definition.Propeller2Feathered, SimVarName = "PROP FEATHERED:2", Unit = Units.Bool, CanSet = false };
    [SimVarDataRequest]
    [TouchPortalState("Propeller3Feathered", "text", "Propeller - Engine 3 - Feathered (bool)", "")]
    public static readonly SimVarItem Propeller3Feathered = new SimVarItem { Def = Definition.Propeller3Feathered, SimVarName = "PROP FEATHERED:3", Unit = Units.Bool, CanSet = false };
    [SimVarDataRequest]
    [TouchPortalState("Propeller4Feathered", "text", "Propeller - Engine 4 - Feathered (bool)", "")]
    public static readonly SimVarItem Propeller4Feathered = new SimVarItem { Def = Definition.Propeller4Feathered, SimVarName = "PROP FEATHERED:4", Unit = Units.Bool, CanSet = false };


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

}
