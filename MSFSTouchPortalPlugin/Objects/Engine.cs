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

using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Enums;

namespace MSFSTouchPortalPlugin.Objects
{
  [TouchPortalCategory(Groups.Engine)]
  internal static class EngineMapping
  {

    #region Ignition/Master

    [TouchPortalAction("MasterIgnition", "Master Ignition Switch", "Toggle Master Ignition Switch", "Toggle Master Ignition Switch")]
    [TouchPortalActionMapping("TOGGLE_MASTER_IGNITION_SWITCH")]
    public static readonly object MASTER_IGNITION;

    [TouchPortalAction("EngineAuto", "Engine Auto Start/Shutdown", "Start/Shutdown Engine", "Engine - {0}")]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("ENGINE_AUTO_START", "Start")]
    [TouchPortalActionMapping("ENGINE_AUTO_SHUTDOWN", "Shutdown")]
    public static readonly object ENGINE_AUTO;

    [TouchPortalAction("EngineMasterTgl", "Engine Master Toggle", "Toggle Engine Master Switch {0}")]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("ENGINE_MASTER_TOGGLE", "All")]
    [TouchPortalActionMapping("ENGINE_MASTER_1_TOGGLE", "1")]
    [TouchPortalActionMapping("ENGINE_MASTER_2_TOGGLE", "2")]
    [TouchPortalActionMapping("ENGINE_MASTER_3_TOGGLE", "3")]
    [TouchPortalActionMapping("ENGINE_MASTER_4_TOGGLE", "4")]
    public static readonly object ENGINE_MASTER_TGL;

    [TouchPortalAction("EngineMasterSet", "Engine Master Set", "Set Engine {0} Master Switch to {1} (0/1)")]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("ENGINE_MASTER_SET", "All")]
    [TouchPortalActionMapping("ENGINE_MASTER_1_SET", "1")]
    [TouchPortalActionMapping("ENGINE_MASTER_2_SET", "2")]
    [TouchPortalActionMapping("ENGINE_MASTER_3_SET", "3")]
    [TouchPortalActionMapping("ENGINE_MASTER_4_SET", "4")]
    [TouchPortalActionNumeric(1, 0, 1)]
    public static readonly object ENGINE_MASTER_SET;

    #endregion

    #region Primers

    [TouchPortalAction("Primers", "Primers", "Toggle Primer(s): {0}")]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("TOGGLE_PRIMER", "All")]
    [TouchPortalActionMapping("TOGGLE_PRIMER1", "1")]
    [TouchPortalActionMapping("TOGGLE_PRIMER2", "2")]
    [TouchPortalActionMapping("TOGGLE_PRIMER3", "3")]
    [TouchPortalActionMapping("TOGGLE_PRIMER4", "4")]
    public static readonly object PRIMERS;

    #endregion

    #region Magneto

    [TouchPortalAction("AllMagenetos", "Magnetos Switch - All", "All Magnetos - {0}")]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("MAGNETO_START", "Start")]
    [TouchPortalActionMapping("MAGNETO_OFF", "Off")]
    [TouchPortalActionMapping("MAGNETO_RIGHT", "Right")]
    [TouchPortalActionMapping("MAGNETO_LEFT", "Left")]
    [TouchPortalActionMapping("MAGNETO_BOTH", "Both")]
    [TouchPortalActionMapping("MAGNETO_DECR", "Decrease")]
    [TouchPortalActionMapping("MAGNETO_INCR", "Increase")]
    [TouchPortalActionMapping("MAGNETO", "Select (for +/-)")]
    public static readonly object ALL_MAGNETOS;

    [TouchPortalAction("MagnetoSpecific", "Magnetos Switch - Individual", "Magneto {0} - {1}")]
    [TouchPortalActionChoice()]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("MAGNETO1_START", new[] { "1", "Start" })]
    [TouchPortalActionMapping("MAGNETO1_OFF", new[] { "1", "Off" })]
    [TouchPortalActionMapping("MAGNETO1_RIGHT", new[] { "1", "Right" })]
    [TouchPortalActionMapping("MAGNETO1_LEFT", new[] { "1", "Left" })]
    [TouchPortalActionMapping("MAGNETO1_BOTH", new[] { "1", "Both" })]
    [TouchPortalActionMapping("MAGNETO1_DECR", new[] { "1", "Decrease" })]
    [TouchPortalActionMapping("MAGNETO1_INCR", new[] { "1", "Increase" })]

    [TouchPortalActionMapping("MAGNETO2_START", new[] { "2", "Start" })]
    [TouchPortalActionMapping("MAGNETO2_OFF", new[] { "2", "Off" })]
    [TouchPortalActionMapping("MAGNETO2_RIGHT", new[] { "2", "Right" })]
    [TouchPortalActionMapping("MAGNETO2_LEFT", new[] { "2", "Left" })]
    [TouchPortalActionMapping("MAGNETO2_BOTH", new[] { "2", "Both" })]
    [TouchPortalActionMapping("MAGNETO2_DECR", new[] { "2", "Decrease" })]
    [TouchPortalActionMapping("MAGNETO2_INCR", new[] { "2", "Increase" })]

    [TouchPortalActionMapping("MAGNETO3_START", new[] { "3", "Start" })]
    [TouchPortalActionMapping("MAGNETO3_OFF", new[] { "3", "Off" })]
    [TouchPortalActionMapping("MAGNETO3_RIGHT", new[] { "3", "Right" })]
    [TouchPortalActionMapping("MAGNETO3_LEFT", new[] { "3", "Left" })]
    [TouchPortalActionMapping("MAGNETO3_BOTH", new[] { "3", "Both" })]
    [TouchPortalActionMapping("MAGNETO3_DECR", new[] { "3", "Decrease" })]
    [TouchPortalActionMapping("MAGNETO3_INCR", new[] { "3", "Increase" })]

    [TouchPortalActionMapping("MAGNETO4_START", new[] { "4", "Start" })]
    [TouchPortalActionMapping("MAGNETO4_OFF", new[] { "4", "Off" })]
    [TouchPortalActionMapping("MAGNETO4_RIGHT", new[] { "4", "Right" })]
    [TouchPortalActionMapping("MAGNETO4_LEFT", new[] { "4", "Left" })]
    [TouchPortalActionMapping("MAGNETO4_BOTH", new[] { "4", "Both" })]
    [TouchPortalActionMapping("MAGNETO4_DECR", new[] { "4", "Decrease" })]
    [TouchPortalActionMapping("MAGNETO4_INCR", new[] { "4", "Increase" })]
    public static readonly object MAGNETO_SPECIFIC;

    [TouchPortalAction("MagnetoSet", "Magnetos Switch Set", false,
      "Set Magneto Switch {0} to position {1} (0-4)",
      "Set Magneto Switch{0}in Range:"
    )]
    [TouchPortalActionChoice()]
    [TouchPortalActionNumeric(0, 0, 4)]
    //[TouchPortalActionMapping("MAGNETO_SET", "All")]  // only value "1" works, same as MAGNETO_START
    //[TouchPortalActionMapping("MAGNETO_SET_ACTUAL", "All")]  // Prepar3D
    [TouchPortalActionMapping("MAGNETO1_SET", "1")]
    [TouchPortalActionMapping("MAGNETO2_SET", "2")]
    [TouchPortalActionMapping("MAGNETO3_SET", "3")]
    [TouchPortalActionMapping("MAGNETO4_SET", "4")]
    public static readonly object MAGNETO_SET;

    #endregion

    #region Starters

    [TouchPortalAction("Starters", "Starters Toggle", "Toggle Starter - {0}")]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("TOGGLE_ALL_STARTERS", "All")]
    [TouchPortalActionMapping("TOGGLE_STARTER1", "1")]
    [TouchPortalActionMapping("TOGGLE_STARTER2", "2")]
    [TouchPortalActionMapping("TOGGLE_STARTER3", "3")]
    [TouchPortalActionMapping("TOGGLE_STARTER4", "4")]
    [TouchPortalActionMapping("TOGGLE_MASTER_STARTER_SWITCH", "Master Switch")]
    public static readonly object STARTERS;

    [TouchPortalAction("StartersSet", "Starters Set", "Starter {0} {1} to {2} (0/1)", true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("STARTER_SET",  new[] { "All", "Set" })]
    [TouchPortalActionMapping("STARTER1_SET", new[] { "1",   "Set" })]
    [TouchPortalActionMapping("STARTER2_SET", new[] { "2",   "Set" })]
    [TouchPortalActionMapping("STARTER3_SET", new[] { "3",   "Set" })]
    [TouchPortalActionMapping("STARTER4_SET", new[] { "4",   "Set" })]
    [TouchPortalActionMapping("SET_STARTER_ALL_HELD",  new[] { "All", "Set Held" })]
    [TouchPortalActionMapping("SET_STARTER1__HELD",    new[] { "1",   "Set Held" })]
    [TouchPortalActionMapping("SET_STARTER2__HELD",    new[] { "2",   "Set Held" })]
    [TouchPortalActionMapping("SET_STARTER3__HELD",    new[] { "3",   "Set Held" })]
    [TouchPortalActionMapping("SET_STARTER4__HELD",    new[] { "4",   "Set Held" })]
    [TouchPortalActionNumeric(1, 0, 1)]
    public static readonly object STARTERS_SET;

    #endregion

    #region Throttle

    [TouchPortalAction("Throttle", "Throttle Adjust - All", "All Throttles - {0}", true)]
    [TouchPortalActionChoice()]
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
    public static readonly object THROTTLE;

    [TouchPortalAction("ThrottleSpecific", "Throttle Adjust - Individual", "Throttle {0} - {1}", true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("THROTTLE1_FULL", new[] { "1", "Full" })]
    [TouchPortalActionMapping("THROTTLE1_INCR", new[] { "1", "Increase" })]
    [TouchPortalActionMapping("THROTTLE1_INCR_SMALL", new[] { "1", "Increase Small" })]
    [TouchPortalActionMapping("THROTTLE1_DECR", new[] { "1", "Decrease" })]
    [TouchPortalActionMapping("THROTTLE1_DECR_SMALL", new[] { "1", "Decrease Small" })]
    [TouchPortalActionMapping("THROTTLE1_CUT", new[] { "1", "Cut" })]
    [TouchPortalActionMapping("THROTTLE2_FULL", new[] { "2", "Full" })]
    [TouchPortalActionMapping("THROTTLE2_INCR", new[] { "2", "Increase" })]
    [TouchPortalActionMapping("THROTTLE2_INCR_SMALL", new[] { "2", "Increase Small" })]
    [TouchPortalActionMapping("THROTTLE2_DECR", new[] { "2", "Decrease" })]
    [TouchPortalActionMapping("THROTTLE2_DECR_SMALL", new[] { "2", "Decrease Small" })]
    [TouchPortalActionMapping("THROTTLE2_CUT", new[] { "2", "Cut" })]
    [TouchPortalActionMapping("THROTTLE3_FULL", new[] { "3", "Full" })]
    [TouchPortalActionMapping("THROTTLE3_INCR", new[] { "3", "Increase" })]
    [TouchPortalActionMapping("THROTTLE3_INCR_SMALL", new[] { "3", "Increase Small" })]
    [TouchPortalActionMapping("THROTTLE3_DECR", new[] { "3", "Decrease" })]
    [TouchPortalActionMapping("THROTTLE3_DECR_SMALL", new[] { "3", "Decrease Small" })]
    [TouchPortalActionMapping("THROTTLE3_CUT", new[] { "3", "Cut" })]
    [TouchPortalActionMapping("THROTTLE4_FULL", new[] { "4", "Full" })]
    [TouchPortalActionMapping("THROTTLE4_INCR", new[] { "4", "Increase" })]
    [TouchPortalActionMapping("THROTTLE4_INCR_SMALL", new[] { "4", "Increase Small" })]
    [TouchPortalActionMapping("THROTTLE4_DECR", new[] { "4", "Decrease" })]
    [TouchPortalActionMapping("THROTTLE4_DECR_SMALL", new[] { "4", "Decrease Small" })]
    [TouchPortalActionMapping("THROTTLE4_CUT", new[] { "4", "Cut" })]
    public static readonly object THROTTLE_SPECIFIC;

    [TouchPortalAction("ThrottleSet", "Throttle Set", true,
      "Set Throttle {0} to {1} (-16384 to +16384)",
      "Set Throttle{0}in Value\nRange:"
    )]
    [TouchPortalActionChoice()]
    [TouchPortalActionText("0", -16384, 16384)]
    [TouchPortalActionMapping("THROTTLE_SET", new[] { "All" })]
    [TouchPortalActionMapping("THROTTLE1_SET", new[] { "1" })]
    [TouchPortalActionMapping("THROTTLE2_SET", new[] { "2" })]
    [TouchPortalActionMapping("THROTTLE3_SET", new[] { "3" })]
    [TouchPortalActionMapping("THROTTLE4_SET", new[] { "4" })]
    public static readonly object THROTTLE_SET;

    [TouchPortalAction("EngineAfterburners", "Afterburner Toggle", "Toggle Afterburner(s): {0}")]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("TOGGLE_AFTERBURNER", "All")]
    [TouchPortalActionMapping("TOGGLE_AFTERBURNER1", "1")]
    [TouchPortalActionMapping("TOGGLE_AFTERBURNER1", "2")]
    [TouchPortalActionMapping("TOGGLE_AFTERBURNER1", "3")]
    [TouchPortalActionMapping("TOGGLE_AFTERBURNER1", "4")]
    public static readonly object AFTERBURNERS;

    #endregion

    #region Mixture

    [TouchPortalAction("Mixture", "Mixture Adjust - All", "All Mixtures - {0}", true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("MIXTURE_RICH", "Rich")]
    [TouchPortalActionMapping("MIXTURE_INCR", "Increase")]
    [TouchPortalActionMapping("MIXTURE_INCR_SMALL", "Increase Small")]
    [TouchPortalActionMapping("MIXTURE_DECR", "Decrease")]
    [TouchPortalActionMapping("MIXTURE_DECR_SMALL", "Decrease Small")]
    [TouchPortalActionMapping("MIXTURE_LEAN", "Lean")]
    [TouchPortalActionMapping("MIXTURE_SET_BEST", "Best")]
    public static readonly object MIXTURE;

    [TouchPortalAction("MixtureSpecific", "Mixture Adjust - Individual", "Mixture {0} - {1}", true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionChoice()]
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
    public static readonly object MIXTURE_SPECIFIC;

    [TouchPortalAction("MixtureSet", "Mixture Set", true,
      "Set Mixture {0} to {1} (-16384 to +16384)",
      "Set Mixture{0}in Value\nRange:"
    )]
    [TouchPortalActionChoice()]
    [TouchPortalActionText("0", 0, 16384)]
    [TouchPortalActionMapping("MIXTURE_SET", new[] { "All" })]
    [TouchPortalActionMapping("MIXTURE1_SET", new[] { "1" })]
    [TouchPortalActionMapping("MIXTURE2_SET", new[] { "2" })]
    [TouchPortalActionMapping("MIXTURE3_SET", new[] { "3" })]
    [TouchPortalActionMapping("MIXTURE4_SET", new[] { "4" })]
    public static readonly object MIXTURE_SET;

    #endregion

    #region Condition Lever

    [TouchPortalAction("ConditionLever", "Condition Lever Adjust", "Condition Lever {0} {1}", true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("CONDITION_LEVER_INC", "All", "Increment")]
    [TouchPortalActionMapping("CONDITION_LEVER_1_INC", "1", "Increment")]
    [TouchPortalActionMapping("CONDITION_LEVER_2_INC", "2", "Increment")]
    [TouchPortalActionMapping("CONDITION_LEVER_3_INC", "3", "Increment")]
    [TouchPortalActionMapping("CONDITION_LEVER_4_INC", "4", "Increment")]
    [TouchPortalActionMapping("CONDITION_LEVER_DEC", "All", "Decrement")]
    [TouchPortalActionMapping("CONDITION_LEVER_1_DEC", "1", "Decrement")]
    [TouchPortalActionMapping("CONDITION_LEVER_2_DEC", "2", "Decrement")]
    [TouchPortalActionMapping("CONDITION_LEVER_3_DEC", "3", "Decrement")]
    [TouchPortalActionMapping("CONDITION_LEVER_4_DEC", "4", "Decrement")]
    [TouchPortalActionMapping("CONDITION_LEVER_HIGH_IDLE", "All", "Set to High Idle")]
    [TouchPortalActionMapping("CONDITION_LEVER_1_HIGH_IDLE", "1", "Set to High Idle")]
    [TouchPortalActionMapping("CONDITION_LEVER_2_HIGH_IDLE", "2", "Set to High Idle")]
    [TouchPortalActionMapping("CONDITION_LEVER_3_HIGH_IDLE", "3", "Set to High Idle")]
    [TouchPortalActionMapping("CONDITION_LEVER_4_HIGH_IDLE", "4", "Set to High Idle")]
    [TouchPortalActionMapping("CONDITION_LEVER_LOW_IDLE", "All", "Set to Low Idle")]
    [TouchPortalActionMapping("CONDITION_LEVER_1_LOW_IDLE", "1", "Set to Low Idle")]
    [TouchPortalActionMapping("CONDITION_LEVER_2_LOW_IDLE", "2", "Set to Low Idle")]
    [TouchPortalActionMapping("CONDITION_LEVER_3_LOW_IDLE", "3", "Set to Low Idle")]
    [TouchPortalActionMapping("CONDITION_LEVER_4_LOW_IDLE", "4", "Set to Low Idle")]
    [TouchPortalActionMapping("CONDITION_LEVER_CUT_OFF", "All", "Cutoff")]
    [TouchPortalActionMapping("CONDITION_LEVER_1_CUT_OFF", "1", "Cutoff")]
    [TouchPortalActionMapping("CONDITION_LEVER_2_CUT_OFF", "2", "Cutoff")]
    [TouchPortalActionMapping("CONDITION_LEVER_3_CUT_OFF", "3", "Cutoff")]
    [TouchPortalActionMapping("CONDITION_LEVER_4_CUT_OFF", "4", "Cutoff")]
    public static readonly object CONDITION_LEVER_ADJ;

    [TouchPortalAction("ConditionLeverSet", "Condition Lever Set", true,
      "Set Condition Lever {0} {1} to {2} (Position: 0 - 2, Axis: 0 - 100)",
      "Set Condition Lever {0} {1} in Value Range\n(Position: 0 - 2, Axis: 0 - 100):"
    )]
    [TouchPortalActionChoice()]
    [TouchPortalActionChoice()]
    [TouchPortalActionText("0", 0, 16384)]
    [TouchPortalActionMapping("CONDITION_LEVER_SET", "All", "Position" )]
    [TouchPortalActionMapping("CONDITION_LEVER_1_SET", "1", "Position" )]
    [TouchPortalActionMapping("CONDITION_LEVER_2_SET", "2", "Position" )]
    [TouchPortalActionMapping("CONDITION_LEVER_3_SET", "3", "Position" )]
    [TouchPortalActionMapping("CONDITION_LEVER_4_SET", "4", "Position")]
    [TouchPortalActionMapping("AXIS_CONDITION_LEVER_SET", "All", "Axis" )]
    [TouchPortalActionMapping("AXIS_CONDITION_LEVER_1_SET", "1", "Axis" )]
    [TouchPortalActionMapping("AXIS_CONDITION_LEVER_2_SET", "2", "Axis" )]
    [TouchPortalActionMapping("AXIS_CONDITION_LEVER_3_SET", "3", "Axis" )]
    [TouchPortalActionMapping("AXIS_CONDITION_LEVER_4_SET", "4", "Axis" )]
    public static readonly object CONDITION_LEVER_SET;

    #endregion

    #region Propeller

    [TouchPortalAction("PropellerPitch", "Propeller Pitch Adjust", "Pitch {0} - {1}", true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionChoice()]
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
    public static readonly object PROPELLER_PITCH;

    [TouchPortalAction("PropellerPitchSet", "Propeller Pitch Set", true,
      "Set Propeller {0} Pitch to {1} (0 to 16384)",
      "Set Propeller{0}Pitch in\nValue Range:"
    )]
    [TouchPortalActionChoice()]
    [TouchPortalActionText("0", 0, 16384)]
    [TouchPortalActionMapping("PROP_PITCH_SET", "All")]
    [TouchPortalActionMapping("PROP_PITCH1_SET", "1")]
    [TouchPortalActionMapping("PROP_PITCH2_SET", "2")]
    [TouchPortalActionMapping("PROP_PITCH3_SET", "3")]
    [TouchPortalActionMapping("PROP_PITCH4_SET", "4")]
    public static readonly object PROPELLER_PITCH_SET;

    #endregion

  }

}
