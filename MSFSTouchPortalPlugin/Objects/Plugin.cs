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
using System.Reflection.Emit;
using SimVarItem = MSFSTouchPortalPlugin.Types.SimVarItem;

namespace MSFSTouchPortalPlugin.Objects
{
  [TouchPortalCategory(Groups.Plugin)]
  internal static class PluginMapping
  {
    [TouchPortalAction(PluginActions.Connection, "Connect & Update",
      "Control connection to the Simulator, or perform various data update tasks.",
      "Plugin Action: {0}")]
    [TouchPortalActionChoice(Id = "Action")]
    [TouchPortalActionMapping(PluginActions.ToggleConnection, "Toggle Simulator Connection")]
    [TouchPortalActionMapping(PluginActions.Connect, "Connect to Simulator")]
    [TouchPortalActionMapping(PluginActions.Disconnect, "Disconnect from Simulator")]
    [TouchPortalActionMapping(PluginActions.ReloadStates, "Reload State Files")]
    [TouchPortalActionMapping(PluginActions.ResendStates, "Re-Send All State Values")]
    [TouchPortalActionMapping(PluginActions.UpdateConnectorValues, "Re-Send All Connector Feedback Values")]
    [TouchPortalActionMapping(PluginActions.UpdateLocalVarsList, "Update Airplane Local Vars List")]
    [TouchPortalActionMapping(PluginActions.ReRegisterLocalVars, "Re-Submit Airplane Local Var Requests")]
    [TouchPortalActionMapping(PluginActions.UpdateHubHopPresets, "Update HubHop Data")]
    // deprecated mappings
    [TouchPortalActionMapping(PluginActions.ToggleConnection, "Toggle", Deprecated = true)]
    [TouchPortalActionMapping(PluginActions.Connect, "On", Deprecated = true)]
    [TouchPortalActionMapping(PluginActions.Disconnect, "Off", Deprecated = true)]
    [TouchPortalActionMapping(PluginActions.UpdateLocalVarsList, "Update Local Var. List", Deprecated = true)]
    [TouchPortalActionMapping(PluginActions.ReRegisterLocalVars, "Re-Submit Local Var. Requests", Deprecated = true)]
    public static readonly object Connection;

    [TouchPortalAction(PluginActions.ActionRepeatInterval, "Action Repeat Interval",
      "Held Action Repeat Rate (ms)",
      "Repeat Interval: {0} to/by: {1} ms",
      "Set Repeat Interval in Range (ms):",
      holdable: true)]
    [TouchPortalActionChoice(Id = "Action")]
    [TouchPortalActionText("450", 50, int.MaxValue, Id = "Value")]
    [TouchPortalActionMapping(PluginActions.ActionRepeatIntervalSet, "Set")]
    [TouchPortalActionMapping(PluginActions.ActionRepeatIntervalInc, "Increment")]
    [TouchPortalActionMapping(PluginActions.ActionRepeatIntervalDec, "Decrement")]
    // hidden data fields for connector
    [TouchPortalActionText("Plugin", Id = "FbCatId")]
    [TouchPortalActionText("[ActionRepeatInterval]", Id = "FbVarName")]
    [TouchPortalConnectorMeta(1000, 50, 50, int.MaxValue, false, false, RangeStartIndex = 3)]
    public static readonly object ActionRepeatInterval;

    // Trigger Events

    [TouchPortalAction(PluginActions.SetCustomSimEvent, "Activate a Named Simulator Event",
      "Trigger any Simulator Event by name with optional parameter value(s). See MSFS SDK Documentation 'Event IDs' for reference.\n" +
        "The value(s), if any, should evaluate to numeric. Using basic math operators and dynamic state values is possible.",
      "Activate Event {0} with value(s): {1} {2} {3} {4} {5}",
      //"Set Event:{0}Value\nRange:",
      holdable: true)]
    [TouchPortalActionText("SIMULATOR_EVENT_NAME", Id = "EvtId", Label = "Event ID")]
    [TouchPortalActionText("", int.MinValue, uint.MaxValue, Id = "Value", Label = "Value 1")]
    [TouchPortalActionText("", int.MinValue, uint.MaxValue, Id = "Value2", Label = "Value 2")]
    [TouchPortalActionText("", int.MinValue, uint.MaxValue, Id = "Value3", Label = "Value 3")]
    [TouchPortalActionText("", int.MinValue, uint.MaxValue, Id = "Value4", Label = "Value 4")]
    [TouchPortalActionText("", int.MinValue, uint.MaxValue, Id = "Value5", Label = "Value 5")]
    //[TouchPortalConnectorMeta(RangeStartIndex = 1)]
    public static readonly object SetCustomSimEvent;

    [TouchPortalConnector(PluginActions.SetCustomSimEvent, "Activate a Named Simulator Event",
      "Trigger any Simulator Event by name setting one parameter value with a slider, with optional extra parameter value(s). See MSFS SDK Documentation 'Event IDs' for reference.\n" +
        "The extra value(s), if any, should evaluate to numeric. They are typically used to select a sub-system which will be affected (eg. one of several engines).",
      "Set Event:{0} in Value\nRange {1}-{2} | With Other\n| Values (opt) {3} {4} Connector\nValue Index {5}"
    )]
    [TouchPortalActionText("SIMULATOR_EVENT_NAME", Id = "EvtId", Label = "Event ID")]
    [TouchPortalActionText("-16384", int.MinValue, uint.MaxValue, Id = "RangeMin", Label = "Value Range Minimum")]
    [TouchPortalActionText("16384", int.MinValue, uint.MaxValue, Id = "RangeMax", Label = "Value Range Maximum")]
    [TouchPortalActionText("", int.MinValue, uint.MaxValue, Id = "Value", Label = "Value 1")]
    [TouchPortalActionText("", int.MinValue, uint.MaxValue, Id = "Value2", Label = "Value 2")]
    //[TouchPortalActionText("", int.MinValue, uint.MaxValue, Id = "Value3", Label = "Value 3")]
    [TouchPortalActionChoice(new[] { "0-first", "1-mid", "2-last", /*"3-last"*/ }, Id = "ConnValIdx", Label = "Conn. Value Index")]
    [TouchPortalConnectorMeta(InsertValueRange = false)]
    public static readonly object SetCustomSimEventConn;


    [TouchPortalAction(PluginActions.SetKnownSimEvent, "Activate a Selected Simulator Event",
      "Trigger a selected Simulator Event, with optional parameter value(s). See MSFS SDK Documentation 'Event IDs' for reference.\n" +
        "The value(s), if any, should evaluate to numeric. Using basic math operators and dynamic state values is possible.",
      "From Category {0} Activate Event {1} with value(s) {2} {3} {4}",
      //"From\nCategory:{0}Activate\nEvent:{1}Value\nRange:",
      holdable: true)]
    [TouchPortalActionChoice("[plugin not connected]", "", Id = "SimCatName", Label = "Category")]
    [TouchPortalActionChoice("[select a category]", "", Id = "EvtId", Label = "Event ID")]
    [TouchPortalActionText("", int.MinValue, uint.MaxValue, Id = "Value", Label = "Value 1")]
    [TouchPortalActionText("", int.MinValue, uint.MaxValue, Id = "Value2", Label = "Value 2")]
    [TouchPortalActionText("", int.MinValue, uint.MaxValue, Id = "Value3", Label = "Value 3")]
    //[TouchPortalConnectorMeta(RangeStartIndex = 2)]
    public static readonly object SetKnownSimEvent;

    [TouchPortalConnector(PluginActions.SetKnownSimEvent, "Activate a Selected Simulator Event",
      "Trigger a selected Simulator Event, setting one parameter value with a slider, with optional extra parameter value(s). See MSFS SDK Documentation 'Event IDs' for reference.\n" +
        "The extra value(s), if any, should evaluate to numeric. They are typically used to select a sub-system which will be affected (eg. one of several engines).",
      "From Category {0} Activate Event {1} in Value\nRange {2}-{3} | With Other\n| Values (opt) {4} {5} Connector\nValue Index {6}"
    )]
    [TouchPortalActionChoice("[plugin not connected]", "", Id = "SimCatName", Label = "Category")]
    [TouchPortalActionChoice("[select a category]", "", Id = "EvtId", Label = "Event ID")]
    [TouchPortalActionText("-16384", int.MinValue, uint.MaxValue, Id = "RangeMin", Label = "Value Range Minimum")]
    [TouchPortalActionText("16384", int.MinValue, uint.MaxValue, Id = "RangeMax", Label = "Value Range Maximum")]
    [TouchPortalActionText("", int.MinValue, uint.MaxValue, Id = "Value", Label = "Value 1")]
    [TouchPortalActionText("", int.MinValue, uint.MaxValue, Id = "Value2", Label = "Value 2")]
    //[TouchPortalActionText("", int.MinValue, uint.MaxValue, Id = "Value3", Label = "Value 3")]
    [TouchPortalActionChoice(new[] { "0-first", "1-mid", "2-last", /*"3-last"*/ }, Id = "ConnValIdx", Label = "Conn. Value Index")]
    [TouchPortalConnectorMeta(InsertValueRange = false)]
    public static readonly object SetKnownSimEventConn;


    [TouchPortalAction(PluginActions.SetHubHopEvent, "Activate an Input Event From HubHop",
      "\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t** Requires WASimModule or MobiFlight. **\n" +
        "Trigger a Simulator Event from loaded HubHop data.\t\t\t\t\t" +
        "\"Potentiometer\" type events are only supported with WASimModule (using the provided value, which should evaluate to numeric).",
      format: "Aircraft/Device: {0} System: {1} Event Name: {2} with value {3} (if any)",
      connectorFormat: "Aircraft\nDevice:{0}System:{1}Event\nName:{2}Value\nRange:",
      holdable: true)]
    [TouchPortalActionChoice("[plugin not connected]", "", Id = "VendorAircraft", Label = "Aircraft")]
    [TouchPortalActionChoice("[select an aircraft]", "", Id = "System", Label = "System")]
    [TouchPortalActionChoice("[select a system]", "", Id = "EvtId", Label = "Event Name")]
    [TouchPortalActionText("", int.MinValue, uint.MaxValue, Id = "Value", Label = "Value")]
    [TouchPortalConnectorMeta()]
    public static readonly object SetHubHopEvent;

    // Set Variables

    [TouchPortalAction(PluginActions.SetSimVar, "Set Simulator Variable (SimVar)",
      "Sets the value of a Simulator Variable selected from a list of Sim Vars which are marked as settable.",
      "From Category:{0}Set Variable:{1}To:{2} (Release AI:{3})",
      "From Category:{0}Set Variable:{1}Value\nRange:",
      holdable: true)]
    [TouchPortalActionChoice("[connect to plugin]", "", Id = "CatId", Label = "Category")]
    [TouchPortalActionChoice("[select a category or type]", "", Id = "VarName", Label = "Variable Name")]
    [TouchPortalActionText("0", float.MinValue, float.MaxValue, Id = "Value", Label = "Value")]
    [TouchPortalActionSwitch(false, Id = "RelAI", Label = "Release AI")]
    [TouchPortalConnectorMeta(decimals: true, feedback: false, RangeStartIndex = 3)]
    public static readonly object SetSimVar;


    [TouchPortalAction(PluginActions.SetLocalVar, "Set Airplane Local Variable",
      "Sets a value on a Local variable from currently loaded aircraft.\t\t\t\t\t** Requires WASimModule **",
      "Set Variable:{0}To:{1}in Units (opt){2}",
        "Set Variable:{0}Units:\n(opt){1}Value\nRange",
      holdable: true)]
    [TouchPortalActionChoice("[connect to plugin]", "", Id = "VarName", Label = "Variable Name")]
    [TouchPortalActionText("0", float.MinValue, float.MaxValue, Id = "Value", Label = "Value")]
    [TouchPortalActionChoice("[connect to plugin]", "", Id = "Unit", Label = "Unit Name")]
    [TouchPortalConnectorMeta()]
    public static readonly object SetLocalVar;


    [TouchPortalAction(PluginActions.SetVariable, "Set Named Variable Value",
      "Set a Named Variable\n" +
        "Sets a value on any named variable, by type of variable. Local (L) variables can also be created. SimVar types require a Unit specifier.\t\t\t\t\t** Requires WASimModule **",
      "Set\nVariable Type:{0}Named:{1} to Value:{2} in Units\n(opt){3} (Create 'L' var: {4}) (release AI: {5})",
      holdable: true)]
    [TouchPortalActionChoice(new[] { "A: SimVar", "C: GPS", "H: HTML Event", "K: Key Event", "L: Local", "Z: Custom SimVar" }, Id = "VarType", Label = "Variable Type")]
    [TouchPortalActionText("", Id = "VarName", Label = "Variable Name")]
    [TouchPortalActionText("0", float.MinValue, float.MaxValue, Id = "Value", Label = "Value")]
    [TouchPortalActionChoice("[connect to plugin]", "", Id = "Unit", Label = "Unit Name")]
    [TouchPortalActionChoice(new[] { "N/A", "No", "Yes" }, Id = "Create", Label = "Create Local Var")]
    [TouchPortalActionSwitch(false, Id = "RelAI", Label = "Release AI")]
    public static readonly object SetVariable;

    [TouchPortalConnector(PluginActions.SetVariable, "Set Named Variable Value",
      "Sets a value on any named variable of various types. SimVar types require a Unit specifier.\t\t\t\t\t** Requires WASimModule **",
      "Variable\nType:{0}Name:{1}Units\n(opt):{2}Value\nRange:"
    )]
    [TouchPortalActionChoice(new[] { "A: SimVar", "C: GPS", "H: HTML Event", "K: Key Event", "L: Local", "Z: Custom SimVar" }, Id = "VarType", Label = "Variable Type")]
    [TouchPortalActionText("", Id = "VarName", Label = "Variable Name")]
    [TouchPortalActionChoice("[connect to plugin]", "", Id = "Unit", Label = "Unit Name")]
    [TouchPortalConnectorMeta()]
    public static readonly object SetVariableConn;

    // Exec Calc Code

    [TouchPortalAction(PluginActions.ExecCalcCode, "Execute Calculator Code",
      "Execute Calculator Code\n" +
      "Runs any entered string of RPN code through the 'execute_calculator_code' Gauge API function. You may use TP state value macros to insert dynamic data.",
      "Execute this code: {0} (must be valid RPN format)                             ** Requires WASimModule **",
      holdable: true)]
    [TouchPortalActionText("1 (>H:AS1000_PFD_SOFTKEYS_1)", Id = "Code", Label = "Code")]
    public static readonly object ExecCalcCode;

    [TouchPortalConnector(PluginActions.ExecCalcCode, "Execute Calculator Code",
      "Runs any entered string of RPN code through the 'execute_calculator_code' Gauge API function. Use an '@' placeholder for the connector value.\t\t\t** Requires WASimModule **",
      "Calculator Code:\n(@ as placeholder(s) for value){0}Value\nRange:"
    )]
    [TouchPortalActionText("@ 1 (>K:2:PANEL_LIGHTS_POWER_SETTING_SET)", Id = "Code", Label = "Code")]
    [TouchPortalConnectorMeta()]
    public static readonly object ExecCalcCodeConn;

    // Visual Feedback

    [TouchPortalConnector(PluginActions.SetConnectorValue, "Visual Feedback Connector",
      "This connector provides only visual feedback by setting the position of a slider based on an existing State/variable value.",
      "Set Connector Value Based on\nFeedback From Category:{0}Variable:{1}Value Range:{2}-{3}"
    )]
    [TouchPortalActionChoice("[connect plugin]", "", Id = "FbCatId", Label = "Feedback Category")]
    [TouchPortalActionChoice("[select a category]", "", Id = "FbVarName", Label = "Feedback Variable")]
    [TouchPortalActionText("-16384", float.MinValue, float.MaxValue, true, Id = "FbRangeMin", Label = "Feedback Range Minimum")]
    [TouchPortalActionText("16384", float.MinValue, float.MaxValue, true, Id = "FbRangeMax", Label = "Feedback Range Maximum")]
    public static readonly object SetConnectorValue;

  }

  // Actions for editing variable requests has own category in TP UI, though actually these act as if they were in `Groups.Plugin`.
  [TouchPortalCategory(Groups.StatesEditor)]
  internal static class StatesEditorMapping
  {

    // The spacing and titles in the actions below are very carefully chosen to help align labels on the top row with entry fields on the bottom row,
    // including the default lists/values. Do not change without testing in TP!

    [TouchPortalAction(PluginActions.AddCustomSimVar, "Request a Custom Simulator Variable",
      "Request Simulator Variable Name:                            Index (if req'd):               " +
        "Units:                                                                " +
        "Category:                                    " +
        "Format:                            Default Value:                 " +
        "Settable:           Update Period:                   Update Interval:               Delta Epsilon:     ",
      "{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}")]
    [TouchPortalActionText("SIMULATOR VARIABLE FULL NAME", Id = "VarName", Label = "Simulator Variable Name")]
    [TouchPortalActionNumeric(0, 0, 99, false, Id = "VarIndex", Label = "Variable Index")]
    [TouchPortalActionChoice("[connect to plugin]                                 ", "", Id = "Unit", Label = "Unit Name")]
    [TouchPortalActionChoice("[connect plugin]", "", Id = "CatId", Label = "Category")]
    [TouchPortalActionText("F2", Id = "Format", Label = "Formatting String")]
    [TouchPortalActionText("0", Id = "DfltVal", Label = "Default Value")]
    [TouchPortalActionSwitch(false, Id = "CanSet", Label = "Settable")]
    [TouchPortalActionChoice(new[] { /*"Never",*/ "Once", /*"VisualFrame",*/ "SimFrame", "Second", "Millisecond", }, "SimFrame", Id = "UpdPer", Label = "Update Period")]
    [TouchPortalActionNumeric(0, 0, int.MaxValue, false, Id = "UpdInt", Label = "Interval")]
    [TouchPortalActionNumeric(SimVarItem.DELTA_EPSILON_DEFAULT, 0.0, float.MaxValue, true, Id = "Epsilon", Label = "Delta Epsilon")]
    public static readonly object AddCustomSimVar;

    [TouchPortalAction(PluginActions.AddKnownSimVar, "Request a Variable From List",
      "Request a Variable from Simulator\t\t\t\t\t** Local Variables support requires WASimModule **\n" +
        "Category or Local Aircraft          " +
        "Request Variable                                                                                               " +
        "Index (if req'd):                " +
        "Units:                                                                 " +
        "Plugin Category:                         " +
        "Format:                            Default Value:                 Update Period:                  Update Interval:              Delta Epsilon:      ",
      "{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}")]
    [TouchPortalActionChoice("[connect to plugin]", "", Id = "SimCatName", Label = "Category or Type")]
    [TouchPortalActionChoice("[select a category]                                                            ", "", Id = "VarName", Label = "Simulator Variable Name")]
    [TouchPortalActionNumeric(0, 0, 99, false, Id = "VarIndex", Label = "Variable Index")]
    [TouchPortalActionChoice("[connect to plugin]                                 ", "number", Id = "Unit", Label = "Unit Name")]
    [TouchPortalActionChoice("[connect plugin]", "", Id = "CatId", Label = "Category")]
    [TouchPortalActionText("F2", Id = "Format", Label = "Formatting String")]
    [TouchPortalActionText("0", Id = "DfltVal", Label = "Default Value")]
    //[TouchPortalActionSwitch(false, Id = "CanSet", Label = "Settable")]  // we should know if it's settable from the imported data
    [TouchPortalActionChoice(new[] { /*"Never",*/ "Once", /*"VisualFrame",*/ "SimFrame", "Second", "Millisecond", }, "SimFrame", Id = "UpdPer", Label = "Update Period")]
    [TouchPortalActionNumeric(0, 0, int.MaxValue, false, Id = "UpdInt", Label = "Interval")]
    [TouchPortalActionNumeric(SimVarItem.DELTA_EPSILON_DEFAULT, 0.0, float.MaxValue, true, Id = "Epsilon", Label = "Delta Epsilon")]
    public static readonly object AddKnownSimVar;

    [TouchPortalAction(PluginActions.AddNamedVariable, "Request a Named Variable",
      "Request a Named Variable                       For indexed SimVars, include it in the name after a : (colon).\t\t\t\t\t** Requires WASimModule **\n" +
        "Variable Type:                Name:                                                   " +
        "Units (optional):                                               " +
        "Plugin Category:                         " +
        "Format:                            Default Value:                 Update Period:                Update Interval:              Delta Epsilon:     ",
      "{0}{1}{2}{3}{4}{5}{6}{7}{8}")]
    [TouchPortalActionChoice(new[] { "A: SimVar", "B: Input", "C: GPS", "E: Env.", "L: Local", "M: Mouse", "R: Rsrc.", "T: Token", "Z: Custom" }, Id = "VarType", Label = "Variable Type")]
    [TouchPortalActionText("FULL VARIABLE NAME", Id = "VarName", Label = "Variable Name")]
    //[TouchPortalActionNumeric(0, 0, 99, false, Id = "VarIndex", Label = "Variable Index")]
    [TouchPortalActionChoice("[connect to plugin]                                 ", "number", Id = "Unit", Label = "Unit Name")]
    [TouchPortalActionChoice("[connect plugin]", "", Id = "CatId", Label = "Category")]
    [TouchPortalActionText("F2", Id = "Format", Label = "Formatting String")]
    [TouchPortalActionText("0", Id = "DfltVal", Label = "Default Value")]
    //[TouchPortalActionSwitch(false, Id = "CanSet", Label = "Settable")]
    [TouchPortalActionChoice(new[] { /*"Never",*/ "Once", "SimFrame", "Second", "Millisecond", }, "SimFrame", Id = "UpdPer", Label = "Update Period")]
    [TouchPortalActionNumeric(0, 0, int.MaxValue, false, Id = "UpdInt", Label = "Interval")]
    [TouchPortalActionNumeric(SimVarItem.DELTA_EPSILON_DEFAULT, 0.0, float.MaxValue, true, Id = "Epsilon", Label = "Delta Epsilon")]
    public static readonly object AddNamedVariable;

    [TouchPortalAction(PluginActions.AddCalculatedValue, "Request a Calculated Value",
      "Request a Calculated Value\t\t\t\t\t** Requires WASimModule **\n" +
        "Calculator Code:                                                                                                         " +
        "Result Type:                  " +
        "Plugin Category:                         " +
        "State Name:                                                " +
        "Format:                            Default Value:                 Update Period:                Update Interval:              Delta Epsilon:     ",
      "{0}{1}{2}{3}{4}{5}{6}{7}{8}")]
    [TouchPortalActionText("(A:TRAILING EDGE FLAPS LEFT ANGLE, degrees) 30 - abs 0.1 <", Id = "CalcCode", Label = "Calculator Code")]
    [TouchPortalActionChoice(new[] { "Double", "Integer", "String", "Formatted" }, Id = "ResType", Label = "Result Type")]
    [TouchPortalActionChoice("[connect plugin]", "", Id = "CatId", Label = "Category")]
    [TouchPortalActionText("A name for the States list", Id = "VarName", Label = "State Name")]
    [TouchPortalActionText("F2", Id = "Format", Label = "Formatting String")]
    [TouchPortalActionText("0", Id = "DfltVal", Label = "Default Value")]
    [TouchPortalActionChoice(new[] { /*"Never",*/ "Once", "SimFrame", "Second", "Millisecond", }, "SimFrame", Id = "UpdPer", Label = "Update Period")]
    [TouchPortalActionNumeric(0, 0, int.MaxValue, false, Id = "UpdInt", Label = "Interval")]
    [TouchPortalActionNumeric(SimVarItem.DELTA_EPSILON_DEFAULT, 0.0, float.MaxValue, true, Id = "Epsilon", Label = "Delta Epsilon")]
    public static readonly object AddCalculatedValue;

    [TouchPortalAction(PluginActions.UpdateVarValue, "Update a Variable Value",
      "Request a value update for an added variable. This is especially useful for variables with a \"Once\" type Update Period.",
      "From Category {0} Update Variable {1}")]
    [TouchPortalActionChoice("[connect plugin]", "", Id = "CatId", Label = "Category")]
    [TouchPortalActionChoice("[select a category]", "", Id = "VarName", Label = "Simulator Variable")]
    public static readonly object UpdateVarValue;

    [TouchPortalAction(PluginActions.RemoveSimVar, "Remove a Simulator Variable",
      "Remove an existing Simulator Variable.",
      "From Category {0} Remove Variable {1}")]
    [TouchPortalActionChoice("[connect plugin]", "", Id = "CatId", Label = "Category")]
    [TouchPortalActionChoice("[select a category]", "", Id = "VarName", Label = "Simulator Variable")]
    public static readonly object RemoveSimVar;

    [TouchPortalAction(PluginActions.ClearSimVars, "Clear Variable Definitions",
      "Removes either all or only custom-added variable state definitions.",
      "Clear {0} states")]
    [TouchPortalActionChoice(Id = "VarsSet", Label = "Variables Set")]
    [TouchPortalActionMapping(PluginActions.ClearCustomSimVars, "Custom")]
    [TouchPortalActionMapping(PluginActions.ClearAllSimVars, "All")]
    public static readonly object ClearSimVars;

    [TouchPortalAction(PluginActions.LoadSimVars, "Load Variable Definitions From File",
      "Load a set of variable state definitions from a configuration file.",
      "Load definitions from file {0} (file name only for user config. folder, or full path with file name)")]
    [TouchPortalActionText("CustomStates.ini", Id = "VarsFile", Label = "Load From File")]
    //[TouchPortalActionFile("CustomStates.ini", Id = "VarsFile", Label = "Load From File")]  // doesn't allow freeform entry and is misleading as of TP 3.1
    public static readonly object LoadSimVars;

    [TouchPortalAction(PluginActions.SaveSimVars, "Save Variable Definitions To File",
      "Save the current simulator variable state definitions to a configuration file.",
      "Save {0} states to file {1} (file name only for user config. folder, or full path with file name)")]
    [TouchPortalActionChoice(Id = "VarsSet", Label = "Variables Set")]
    [TouchPortalActionText("CustomStates.ini", Id = "VarsFile", Label = "Save to File")]
    //[TouchPortalActionFile("CustomStates.ini", Id = "VarsFile", Label = "Save to File")]  // doesn't allow freeform entry or selecting a non-existing file as of TP 3.0.10
    [TouchPortalActionMapping(PluginActions.SaveCustomSimVars, "Custom")]
    [TouchPortalActionMapping(PluginActions.SaveAllSimVars, "All")]
    public static readonly object SaveSimVars;

  }
}
