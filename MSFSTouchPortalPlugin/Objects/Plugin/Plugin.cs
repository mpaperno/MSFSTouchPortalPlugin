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
using MSFSTouchPortalPlugin.Types;

namespace MSFSTouchPortalPlugin.Objects.Plugin
{
  [TouchPortalCategory(Groups.Plugin)]
  internal static class PluginMapping
  {
    [TouchPortalAction(PluginActions.Connection, "Connect & Update",
      "Plugin actions: Toggle/On/Off SimConnect Connection, Reload State Config Files, Update HubHop Presets",
      "Plugin Action: {0}")]
    [TouchPortalActionChoice(new[] { "Toggle", "On", "Off", "Reload State Files", "Update Local Var. List", "Update HubHop Data" }, Id = "Action")]
    [TouchPortalActionMapping(PluginActions.ToggleConnection, "Toggle")]
    [TouchPortalActionMapping(PluginActions.Connect, "On")]
    [TouchPortalActionMapping(PluginActions.Disconnect, "Off")]
    [TouchPortalActionMapping(PluginActions.ReloadStates, "Reload State Files")]
    [TouchPortalActionMapping(PluginActions.UpdateLocalVarsList, "Update Local Var. List")]
    [TouchPortalActionMapping(PluginActions.UpdateHubHopPresets, "Update HubHop Data")]
    public static readonly object Connection;

    [TouchPortalAction(PluginActions.ActionRepeatInterval, "Action Repeat Interval",
      "Held Action Repeat Rate (ms)",
      "Repeat Interval: {0} to/by: {1} ms",
      holdable: true)]
    [TouchPortalActionChoice(new[] { "Set", "Increment", "Decrement" }, Id = "Action")]
    [TouchPortalActionText("450", 50, int.MaxValue, Id = "Value")]
    [TouchPortalActionMapping(PluginActions.ActionRepeatIntervalSet, "Set")]
    [TouchPortalActionMapping(PluginActions.ActionRepeatIntervalInc, "Increment")]
    [TouchPortalActionMapping(PluginActions.ActionRepeatIntervalDec, "Decrement")]
    public static readonly object ActionRepeatInterval;

    [TouchPortalConnector(PluginActions.ActionRepeatIntervalSet, "Set Action Repeat Interval",
      "Sets the held action repeat rate in milliseconds.",
      "Set Repeat Interval in Range: {0}-{1} ms"
    )]
    [TouchPortalActionNumeric(1000, 50, int.MaxValue, Id = "RangeMin", Label = "Value Range Minimum")]
    [TouchPortalActionNumeric(50, 50, int.MaxValue, Id = "RangeMax", Label = "Value Range Maximum")]
    [TouchPortalActionText("Plugin", Id = "FbCatId")]
    [TouchPortalActionText("[ActionRepeatInterval]", Id = "FbVarName")]
    public static readonly object ActionRepeatIntervalConn;


    [TouchPortalAction(PluginActions.SetCustomSimEvent, "Activate a Named Simulator Event",
      "Trigger a Simulator Event by name.\n" +
        "The value, if any, should evaluate to numeric. Using basic math operators and dynamic state values is possible.",
      "Activate Event {0} with value {1} (if any)",
      holdable: true)]
    [TouchPortalActionText("SIMULATOR_EVENT_NAME", Id = "EvtId", Label = "Event ID")]
    [TouchPortalActionText("", Id = "Value", Label = "Value")]
    public static readonly object SetCustomSimEvent;

    [TouchPortalConnector(PluginActions.SetCustomSimEvent, "Set a Named Simulator Event Value",
      "Set a Simulator Event value by event name.",
      "Set Event {0}Value\nRange:{1}-{2}| Feedback From\n| State (opt):{3}{4}\nRange:{5}-{6}"
    )]
    [TouchPortalActionText("SIMULATOR_EVENT_NAME", Id = "EvtId", Label = "Event ID")]
    [TouchPortalActionNumeric(-16384, float.MinValue, float.MaxValue, true, Id = "RangeMin", Label = "Value Range Minimum")]
    [TouchPortalActionNumeric(16384, float.MinValue, float.MaxValue, true, Id = "RangeMax", Label = "Value Range Maximum")]
    [TouchPortalActionChoice("[connect plugin]", "", Id = "FbCatId", Label = "Feedback Category")]
    [TouchPortalActionChoice("[select a category]", "", Id = "FbVarName", Label = "Feedback Variable")]
    [TouchPortalActionText("", float.MinValue, float.MaxValue, Id = "FbRangeMin", Label = "Feedback Range Minimum")]
    [TouchPortalActionText("", float.MinValue, float.MaxValue, Id = "FbRangeMax", Label = "Feedback Range Maximum")]
    public static readonly object SetCustomSimEventConn;


    [TouchPortalAction(PluginActions.SetKnownSimEvent, "Activate a Simulator Event From List",
      "Trigger a Simulator Event.\n" +
        "The value, if any, should evaluate to numeric. Using basic math operators and dynamic state values is possible.",
      "From Category {0} Activate Event {1} with value {2} (if any)",
      holdable: true)]
    [TouchPortalActionChoice("[plugin not connected]", "", Id = "SimCatName", Label = "Category")]
    [TouchPortalActionChoice("[select a category]", "", Id = "EvtId", Label = "Event ID")]
    [TouchPortalActionText("", Id = "Value", Label = "Value")]
    public static readonly object SetKnownSimEvent;

    [TouchPortalConnector(PluginActions.SetKnownSimEvent, "Set a Known Simulator Event Value",
      "Set value of a Simulator Event selected from a list of imported events.",
      "From\nCategory:{0}Activate\nEvent:{1}Value\nRange:{2}-{3}| Feedback From\n| State (opt):{4}{5}\nRange:{6}-{7}"
    )]
    [TouchPortalActionChoice("[plugin not connected]", "", Id = "SimCatName", Label = "Category")]
    [TouchPortalActionChoice("[select a category]", "", Id = "EvtId", Label = "Event ID")]
    [TouchPortalActionNumeric(-16384, float.MinValue, float.MaxValue, true, Id = "RangeMin", Label = "Value Range Minimum")]
    [TouchPortalActionNumeric(16384, float.MinValue, float.MaxValue, true, Id = "RangeMax", Label = "Value Range Maximum")]
    [TouchPortalActionChoice("[connect plugin]", "", Id = "FbCatId", Label = "Feedback Category")]
    [TouchPortalActionChoice("[select a category]", "", Id = "FbVarName", Label = "Feedback Variable")]
    [TouchPortalActionText("", float.MinValue, float.MaxValue, Id = "FbRangeMin", Label = "Feedback Range Minimum")]
    [TouchPortalActionText("", float.MinValue, float.MaxValue, Id = "FbRangeMax", Label = "Feedback Range Maximum")]
    public static readonly object SetKnownSimEventConn;


    [TouchPortalAction(PluginActions.SetHubHopEvent, "Activate a Simulator Event From HubHop",
      "\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t" +
        "** Requires WASimModule or MobiFlight. **\n" +
        "Trigger a Simulator Event from loaded HubHop data.\t\t\t\t\t" +
        "\"Potentiometer\" type events are only supported with WASimModule (using the provided value, which should evaluate to numeric).",
      "Aircraft/Device: {0} System: {1} Event Name: {2} with value {3} (if any)",
      holdable: true)]
    [TouchPortalActionChoice("[plugin not connected]", "", Id = "VendorAircraft", Label = "Aircraft")]
    [TouchPortalActionChoice("[select an aircraft]", "", Id = "System", Label = "System")]
    [TouchPortalActionChoice("[select a system]", "", Id = "EvtId", Label = "Event Name")]
    [TouchPortalActionText("", Id = "Value", Label = "Value")]
    public static readonly object SetHubHopEvent;

    [TouchPortalConnector(PluginActions.SetHubHopEvent, "Set HubHop Input Event Value",
      "Set value of an Input (Potentiometer) type Event selected from HubHop data.",
      "Aircraft\nDevice:{0}System:{1}Event\nName:{2}Value\nRange:{3}-{4}| Feedback From\n| State (opt):{5}{6}\nRange:{7}-{8}"
    )]
    [TouchPortalActionChoice("[plugin not connected]", "", Id = "VendorAircraft", Label = "Aircraft")]
    [TouchPortalActionChoice("[select an aircraft]", "", Id = "System", Label = "System")]
    [TouchPortalActionChoice("[select a system]", "", Id = "EvtId", Label = "Event Name")]
    [TouchPortalActionNumeric(-16384, float.MinValue, float.MaxValue, true, Id = "RangeMin", Label = "Value Range Minimum")]
    [TouchPortalActionNumeric(16384, float.MinValue, float.MaxValue, true, Id = "RangeMax", Label = "Value Range Maximum")]
    [TouchPortalActionChoice("[connect plugin]", "", Id = "FbCatId", Label = "Feedback Category")]
    [TouchPortalActionChoice("[select a category]", "", Id = "FbVarName", Label = "Feedback Variable")]
    [TouchPortalActionText("", float.MinValue, float.MaxValue, Id = "FbRangeMin", Label = "Feedback Range Minimum")]
    [TouchPortalActionText("", float.MinValue, float.MaxValue, Id = "FbRangeMax", Label = "Feedback Range Maximum")]
    public static readonly object SetHubHopEventConn;


    [TouchPortalAction(PluginActions.SetSimVar, "Set Simulator Variable (SimVar)",
      "Sets the value of a Simulator Variable selected from a list of Sim Vars which are marked as settable.",
      "From Category:{0}Set Variable:{1}To:{2} (Release AI:{3})",
      holdable: true)]
    [TouchPortalActionChoice("[connect to plugin]", "", Id = "CatId", Label = "Category")]
    [TouchPortalActionChoice("[select a category or type]", "", Id = "VarName", Label = "Variable Name")]
    [TouchPortalActionText("0", Id = "Value", Label = "Value")]
    [TouchPortalActionSwitch(false, Id = "RelAI", Label = "Release AI")]
    public static readonly object SetSimVar;

    [TouchPortalConnector(PluginActions.SetSimVar, "Set Simulator Variable (SimVar)",
      "Sets the value of a Simulator Variable selected from a list of Sim Vars which are marked as settable.",
      "From Category:{0}Set Variable:{1}Value\nRange:{2}-{3}" /* | Feedback From\n| State (opt):{5}{6}\nRange:{7}-{8} */
    )]
    [TouchPortalActionChoice("[connect to plugin]", "", Id = "CatId", Label = "Category")]
    [TouchPortalActionChoice("[select a category]", "", Id = "VarName", Label = "Variable Name")]
    [TouchPortalActionNumeric(-16384, float.MinValue, float.MaxValue, true, Id = "RangeMin", Label = "Value Range Minimum")]
    [TouchPortalActionNumeric(16384, float.MinValue, float.MaxValue, true, Id = "RangeMax", Label = "Value Range Maximum")]
    //[TouchPortalActionChoice("[connect plugin]", "", Id = "FbCatId", Label = "Feedback Category")]
    //[TouchPortalActionChoice("[select a category]", "", Id = "FbVarName", Label = "Feedback Variable")]
    //[TouchPortalActionText("", float.MinValue, float.MaxValue, Id = "FbRangeMin", Label = "Feedback Range Minimum")]
    //[TouchPortalActionText("", float.MinValue, float.MaxValue, Id = "FbRangeMax", Label = "Feedback Range Maximum")]
    public static readonly object SetSimVarConn;


    [TouchPortalAction(PluginActions.SetLocalVar, "Set Airplane Local Variable",
      "Sets a value on a Local variable from currently loaded aircraft.\t\t\t\t\t** Requires WASimModule **",
      "Set Variable:{0}To:{1}in Units (opt){2}",
      holdable: true)]
    [TouchPortalActionChoice("[connect to plugin]", "", Id = "VarName", Label = "Variable Name")]
    [TouchPortalActionText("0", Id = "Value", Label = "Value")]
    [TouchPortalActionChoice("[connect to plugin]", "", Id = "Unit", Label = "Unit Name")]
    public static readonly object SetLocalVar;

    [TouchPortalConnector(PluginActions.SetLocalVar, "Set Airplane Local Variable",
      "Sets the value of a Local variable from currently loaded aircraft..\t\t\t\t\t** Requires WASimModule **",
        "Set Variable:{0}Units:\n(opt){1}Value\nRange:{2}-{3} | Feedback From\n| State (opt):{4}{5}\nRange:{6}-{7}"
    )]
    [TouchPortalActionChoice("[connect to plugin]", "", Id = "VarName", Label = "Variable Name")]
    [TouchPortalActionChoice("[connect to plugin]", "", Id = "Unit", Label = "Unit Name")]
    [TouchPortalActionNumeric(-16384, float.MinValue, float.MaxValue, true, Id = "RangeMin", Label = "Value Range Minimum")]
    [TouchPortalActionNumeric(16384, float.MinValue, float.MaxValue, true, Id = "RangeMax", Label = "Value Range Maximum")]
    [TouchPortalActionChoice("[connect plugin]", "", Id = "FbCatId", Label = "Feedback Category")]
    [TouchPortalActionChoice("[select a category]", "", Id = "FbVarName", Label = "Feedback Variable")]
    [TouchPortalActionText("", float.MinValue, float.MaxValue, Id = "FbRangeMin", Label = "Feedback Range Minimum")]
    [TouchPortalActionText("", float.MinValue, float.MaxValue, Id = "FbRangeMax", Label = "Feedback Range Maximum")]
    public static readonly object SetLocalVarConn;


    [TouchPortalAction(PluginActions.SetVariable, "Set Named Variable Value",
      "Set a Named Variable\n" +
        "Sets a value on any named variable, by type of variable. Local (L) variables can also be created. SimVar types require a Unit specifier.\t\t\t\t\t** Requires WASimModule **",
        "Set\nVariable Type:{0}Named:{1} to Value:{2} in Units\n(opt){3} (Create 'L' var: {4}) (release AI: {5})",
      holdable: true)]
    [TouchPortalActionChoice(new[] { "A: SimVar", "C: GPS", "H: HTML Event", "K: Key Event", "L: Local", "Z: Custom SimVar" }, Id = "VarType", Label = "Variable Type")]
    [TouchPortalActionText("", Id = "VarName", Label = "Variable Name")]
    [TouchPortalActionText("0", Id = "Value", Label = "Value")]
    [TouchPortalActionChoice("[connect to plugin]", "", Id = "Unit", Label = "Unit Name")]
    [TouchPortalActionChoice(new[] { "N/A", "No", "Yes" }, Id = "Create", Label = "Create Local Var")]
    [TouchPortalActionSwitch(false, Id = "RelAI", Label = "Release AI")]
    public static readonly object SetVariable;

    [TouchPortalConnector(PluginActions.SetVariable, "Set Named Variable Value",
      "Sets a value on any named variable of various types. SimVar types require a Unit specifier.\t\t\t\t\t** Requires WASimModule **",
      "Variable\nType:{0}Name:{1}Units\n(opt):{2}Value\nRange:{3}-{4}| Feedback From\n| State (opt):{5}{6}\nRange:{7}-{8}"
    )]
    [TouchPortalActionChoice(new[] { "A: SimVar", "C: GPS", "H: HTML Event", "K: Key Event", "L: Local", "Z: Custom SimVar" }, Id = "VarType", Label = "Variable Type")]
    [TouchPortalActionText("", Id = "VarName", Label = "Variable Name")]
    [TouchPortalActionChoice("[connect to plugin]", "", Id = "Unit", Label = "Unit Name")]
    [TouchPortalActionNumeric(-16384, float.MinValue, float.MaxValue, true, Id = "RangeMin", Label = "Value Range Minimum")]
    [TouchPortalActionNumeric(16384, float.MinValue, float.MaxValue, true, Id = "RangeMax", Label = "Value Range Maximum")]
    [TouchPortalActionChoice("[connect plugin]", "", Id = "FbCatId", Label = "Feedback Category")]
    [TouchPortalActionChoice("[select a category]", "", Id = "FbVarName", Label = "Feedback Variable")]
    [TouchPortalActionText("", float.MinValue, float.MaxValue, Id = "FbRangeMin", Label = "Feedback Range Minimum")]
    [TouchPortalActionText("", float.MinValue, float.MaxValue, Id = "FbRangeMax", Label = "Feedback Range Maximum")]
    public static readonly object SetVariableConn;


    [TouchPortalAction(PluginActions.ExecCalcCode, "Execute Calculator Code",
      "Execute Calculator Code\n" +
      "Runs any entered string of RPN code through the 'execute_calculator_code' Gauge API function. You may use TP state value macros to insert dynamic data.",
      "Execute this code: {0} (must be valid RPN format)                             ** Requires WASimModule **",
      holdable: true)]
    [TouchPortalActionText("1 (>H:AS1000_PFD_SOFTKEYS_1)", Id = "Code", Label = "Code")]
    public static readonly object ExecCalcCode;

    [TouchPortalConnector(PluginActions.ExecCalcCode, "Execute Calculator Code",
      "Runs any entered string of RPN code through the 'execute_calculator_code' Gauge API function. Use an '@' placeholder for the connector value.\t\t\t** Requires WASimModule **",
      "Calculator Code:\n(@ as placeholder(s) for value){0}Value\nRange:{1}-{2}| Feedback From\n| State (opt):{3}{4}\nRange:{5}-{6}"
    )]
    [TouchPortalActionText("@ 1 (>K:2:PANEL_LIGHTS_POWER_SETTING_SET)", Id = "Code", Label = "Code")]
    [TouchPortalActionNumeric(-16384, float.MinValue, float.MaxValue, true, Id = "RangeMin", Label = "Value Range Minimum")]
    [TouchPortalActionNumeric(16384, float.MinValue, float.MaxValue, true, Id = "RangeMax", Label = "Value Range Maximum")]
    [TouchPortalActionChoice("[connect plugin]", "", Id = "FbCatId", Label = "Feedback Category")]
    [TouchPortalActionChoice("[select a category]", "", Id = "FbVarName", Label = "Feedback Variable")]
    [TouchPortalActionText("", float.MinValue, float.MaxValue, Id = "FbRangeMin", Label = "Feedback Range Minimum")]
    [TouchPortalActionText("", float.MinValue, float.MaxValue, Id = "FbRangeMax", Label = "Feedback Range Maximum")]
    public static readonly object ExecCalcCodeConn;


    // The spacing and titles here are very carefully chosen to help align labels on the top row with entry fields on the bottom row, including the default lists/values.
    // Do not change without testing in TP!
    [TouchPortalAction(PluginActions.AddCustomSimVar, "Request a Custom Simulator Variable",
      "Request Simulator Variable Name:                            Index (if req'd):               Category:                                    " +
        "Units:                                                                " +
        "Format:                            Default Value:                 " +
        "Settable:           Update Period:                   Update Interval:               Delta Epsilon:     ",
      "{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}")]
    [TouchPortalActionText("SIMULATOR VARIABLE FULL NAME", Id = "VarName", Label = "Simulator Variable Name")]
    [TouchPortalActionNumeric(0, 0, 99, false, Id = "VarIndex", Label = "Variable Index")]
    [TouchPortalActionChoice("[connect plugin]", "", Id = "CatId", Label = "Category")]
    [TouchPortalActionChoice("[connect to plugin]                                 ", "", Id = "Unit", Label = "Unit Name")]
    [TouchPortalActionText("F2", Id = "Format", Label = "Formatting String")]
    [TouchPortalActionText("0", Id = "DfltVal", Label = "Default Value")]
    [TouchPortalActionSwitch(false, Id = "CanSet", Label = "Settable")]
    [TouchPortalActionChoice(new[] { /*"Never",*/ "Once", /*"VisualFrame",*/ "SimFrame", "Second", "Millisecond", }, "SimFrame", Id = "UpdPer", Label = "Update Period")]
    [TouchPortalActionNumeric(0, 0, int.MaxValue, false, Id = "UpdInt", Label = "Interval")]
    [TouchPortalActionNumeric(0.009, 0.0, float.MaxValue, true, Id = "Epsilon", Label = "Delta Epsilon")]
    public static readonly object AddCustomSimVar;

    // The spacing and titles here are very carefully chosen to help align labels on the top row with entry fields on the bottom row, including the default lists/values.
    // Do not change without testing in TP!
    [TouchPortalAction(PluginActions.AddKnownSimVar, "Request a Variable From List",
      "Request a Variable from Simulator\t\t\t\t\t** Local Variables support requires WASimModule **\n" +
        "Category or Local Aircraft          " +
        "Request Variable                                                                                        " +
        "Index (if req'd):                " +
        "Plugin Category:                         Units:                                                                 " +
        "Format:                            Default Value:                 Update Period:                  Update Interval:              Delta Epsilon:      ",
      "{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}")]
    [TouchPortalActionChoice("[connect to plugin]", "", Id = "SimCatName", Label = "Category or Type")]
    [TouchPortalActionChoice("[select a category]                                                            ", "", Id = "VarName", Label = "Simulator Variable Name")]
    [TouchPortalActionNumeric(0, 0, 99, false, Id = "VarIndex", Label = "Variable Index")]
    [TouchPortalActionChoice("[connect plugin]", "", Id = "CatId", Label = "Category")]
    [TouchPortalActionChoice("[connect to plugin]                                 ", "number", Id = "Unit", Label = "Unit Name")]
    [TouchPortalActionText("F2", Id = "Format", Label = "Formatting String")]
    [TouchPortalActionText("0", Id = "DfltVal", Label = "Default Value")]
    //[TouchPortalActionSwitch(false, Id = "CanSet", Label = "Settable")]  // we should know if it's settable from the imported data
    [TouchPortalActionChoice(new[] { /*"Never",*/ "Once", /*"VisualFrame",*/ "SimFrame", "Second", "Millisecond", }, "SimFrame", Id = "UpdPer", Label = "Update Period")]
    [TouchPortalActionNumeric(0, 0, int.MaxValue, false, Id = "UpdInt", Label = "Interval")]
    [TouchPortalActionNumeric(0.009, 0.0, float.MaxValue, true, Id = "Epsilon", Label = "Delta Epsilon")]
    public static readonly object AddKnownSimVar;

    // The spacing and titles here are very carefully chosen to help align labels on the top row with entry fields on the bottom row, including the default lists/values.
    // Do not change without testing in TP!
    [TouchPortalAction(PluginActions.AddNamedVariable, "Request a Named Variable",
      "Request a Named Variable                       For indexed SimVars, include it in the name after a : (colon).\t\t\t\t\t** Requires WASimModule **\n" +
        "Variable Type:                Name:                                                   " +
        "Plugin Category:                         Units (optional):                                               " +
        "Format:                            Default Value:                 Update Period:                Update Interval:              Delta Epsilon:     ",
      "{0}{1}{2}{3}{4}{5}{6}{7}{8}")]
    [TouchPortalActionChoice(new[] { "A: SimVar", "B: Input", "C: GPS", "E: Env.", "L: Local", "M: Mouse", "R: Rsrc.", "T: Token", "Z: Custom" }, Id = "VarType", Label = "Variable Type")]
    [TouchPortalActionText("FULL VARIABLE NAME", Id = "VarName", Label = "Variable Name")]
    //[TouchPortalActionNumeric(0, 0, 99, false, Id = "VarIndex", Label = "Variable Index")]
    [TouchPortalActionChoice("[connect plugin]", "", Id = "CatId", Label = "Category")]
    [TouchPortalActionChoice("[connect to plugin]                                 ", "number", Id = "Unit", Label = "Unit Name")]
    [TouchPortalActionText("F2", Id = "Format", Label = "Formatting String")]
    [TouchPortalActionText("0", Id = "DfltVal", Label = "Default Value")]
    //[TouchPortalActionSwitch(false, Id = "CanSet", Label = "Settable")]
    [TouchPortalActionChoice(new[] { /*"Never",*/ "Once", "SimFrame", "Second", "Millisecond", }, "SimFrame", Id = "UpdPer", Label = "Update Period")]
    [TouchPortalActionNumeric(0, 0, int.MaxValue, false, Id = "UpdInt", Label = "Interval")]
    [TouchPortalActionNumeric(0.009, 0.0, float.MaxValue, true, Id = "Epsilon", Label = "Delta Epsilon")]
    public static readonly object AddNamedVariable;

    // The spacing and titles here are very carefully chosen to help align labels on the top row with entry fields on the bottom row, including the default lists/values.
    // Do not change without testing in TP!
    [TouchPortalAction(PluginActions.AddCalculatedValue, "Request a Calculated Value",
      "Request a Calculated Value\t\t\t\t\t** Requires WASimModule **\n" +
        "Calculator Code:                                                                                                         " +
        "Result Type:                  " +
        "Plugin Category:                         State Name:                                                " +
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
    [TouchPortalActionNumeric(0.009, 0.0, float.MaxValue, true, Id = "Epsilon", Label = "Delta Epsilon")]
    public static readonly object AddCalculatedValue;

    [TouchPortalAction(PluginActions.UpdateVarValue, "Update a Variable Value",
      "Request a value update for an added variable. This is especially useful for variables with a \"Once\" type Update Period.",
      "From Category {0} Update Variable {1}")]
    [TouchPortalActionChoice("[connect plugin]", "", Id = "CatId", Label = "Category")]
    [TouchPortalActionChoice("[select a category]", "", Id = "VarName", Label = "Simulator Variable")]
    public static readonly object UpdateVarValue;

    [TouchPortalAction(PluginActions.RemoveSimVar, "Remove a Simulator Variable",
      "Remove an existing Simulator Variable. Note that static TP States cannot be removed.",
      "From Category {0} Remove Variable {1}")]
    [TouchPortalActionChoice("[connect plugin]", "", Id = "CatId", Label = "Category")]
    [TouchPortalActionChoice("[select a category]", "", Id = "VarName", Label = "Simulator Variable")]
    public static readonly object RemoveSimVar;

    [TouchPortalAction(PluginActions.LoadSimVars, "Load SimVar Definitions From File",
      "Load a set of simulator variable state definitions from a configuration file.",
      "Load from file {0} (name only for user config. folder, or full path with file name)")]
    [TouchPortalActionFile("CustomStates.ini", Id = "VarsFile", Label = "Load From File")]
    public static readonly object LoadSimVars;

    [TouchPortalAction(PluginActions.SaveSimVars, "Save SimVar Definitions To File",
      "Save the current simulator variable state definitions to a configuration file.",
      "Save {0} states to file {1} (name only for user config. folder, or full path with file name)")]
    [TouchPortalActionChoice(new[] { "Custom", "All" }, Id = "VarsSet", Label = "Variables Set")]
    [TouchPortalActionText("CustomStates.ini", Id = "VarsFile", Label = "Save to File")]
    //[TouchPortalActionFile("CustomStates.ini", Id = "VarsFile", Label = "Save to File")]  // doesn't allow freeform entry or selecting a non-existing file as of TP 3.0.10
    [TouchPortalActionMapping(PluginActions.SaveCustomSimVars, "Custom")]
    [TouchPortalActionMapping(PluginActions.SaveAllSimVars, "All")]
    public static readonly object SaveSimVars;


    [TouchPortalConnector(PluginActions.SetConnectorValue, "Visual Feedback Connector",
      "This connector provides only visual feedback by setting the position of a slider based on an existing State/variable value.",
      "Set Connector Value Based on\nFeedback From Category:{0}Variable:{1}Value Range:{2}-{3}"
    )]
    [TouchPortalActionChoice("[connect plugin]", "", Id = "FbCatId", Label = "Feedback Category")]
    [TouchPortalActionChoice("[select a category]", "", Id = "FbVarName", Label = "Feedback Variable")]
    [TouchPortalActionNumeric(-16384, float.MinValue, float.MaxValue, true, Id = "FbRangeMin", Label = "Feedback Range Minimum")]
    [TouchPortalActionNumeric(16384, float.MinValue, float.MaxValue, true, Id = "FbRangeMax", Label = "Feedback Range Maximum")]
    public static readonly object SetConnectorValue;

  }

  [TouchPortalSettingsContainer]
  public static class Settings
  {
    public static readonly PluginSetting ConnectSimOnStartup = new PluginSetting("ConnectSimOnStartup", DataType.Switch) {
      Name = "Connect To Flight Sim on Startup (0/1)",
      Description = "Set to 1 to automatically attempt connection to flight simulator upon Touch Portal startup. Set to 0 to only connect manually via the Action *MSFS - Plugin -> Connect & Update*.",
      Default = "0",
    };

    public static readonly PluginSetting UserStateFiles = new PluginSetting("UserStateFiles", DataType.Text) {
      Name = "Sim Variable State Config File(s) (blank = Default)",
      Description = "Here you can specify one or more custom configuration files which define SimConnect variables to request as Touch Portal States. " +
        "This plugin comes with an extensive default set of states, however since the possibilities between which variables are requested, which units they are displayed in," +
        "and how they are formatted are almost endless. This option provides a way to customize the output as desired.\n\n" +
        "Enter a file name here, with or w/out the suffix (`.ini` is assumed). Separate multiple files with commas (and optional space). " +
        "To include the default set of variables/states, use the name `Default` as one of the file names (in any position of the list).\n\n" +
        "Files are loaded in the order in which they appear in the list, and in case of conflicting state IDs, the last one found will be used.\n\n" +
        "The custom file(s) are expected to be in the folder specified in the \"User Config Files Path\" setting (see below).",
      Default = "Default",
      MaxLength = 255
    };

    public static readonly PluginSetting SimConnectConfigIndex = new PluginSetting("SimConnectConfigIndex", DataType.Number) {
      Name = "SimConnect.cfg Index (0 for MSFS, 1 for FSX, or custom)",
      Description =
        "A __SimConnect.cfg__ file can contain a number of connection configurations, identified in sections with the `[SimConnect.N]` title. " +
        "A default __SimConnect.cfg__ is included with this plugin (in the installation folder). " +
        "You may also use a custom configuration file stored in the \"User Config Files Path\" folder (see below). \n\n" +
        "The index number can be specified in this setting. This is useful for \n" +
        "  1. compatibility with FSX, and/or \n" +
        "  2. custom configurations over network connections (running Touch Portal on a different computer than the sim). \n\n" +
        "The default configuration index is zero, which (in the included default SimConnect.cfg) is suitable for MSFS (2020). Use the index 1 for compatibility with FSX (or perhaps other sims). \n\n" +
        "See here for more info: https://docs.flightsimulator.com/html/Programming_Tools/SimConnect/SimConnect_CFG_Definition.htm",
      Default = "0",
      MinValue = 0,
      MaxValue = 20
    };

    public static readonly PluginSetting UserConfigFilesPath = new PluginSetting("UserConfigFilesPath", DataType.Text) {
      Name = "User Config Files Path (blank for default)",
      Description = "The system path where custom user configuration files for sate definitions, SimConnect.cfg, etc, are stored. " +
        "Keep it blank for default, which is `C:\\Users\\<UserName>\\AppData\\Roaming\\MSFSTouchPortalPlugin`.\n\n" +
        "Note that using this plugin's installation folder for custom data storage is not recommended, since anything in there will likely get overwritten during a plugin update/re-install.",
      Default = "",
      MaxLength = 255
    };

    public static readonly PluginSetting UpdateHubHopOnStartup = new PluginSetting("UpdateHubHopOnStartup", DataType.Switch) {
      Name = "Update HubHop Data on Startup (0/1)",
      Description = "Set to 1 to automatically load latest HubHop data when plugin starts. Set to 0 to disable. Updates can always be triggered manually via the Action *MSFS - Plugin -> Connect & Update*. " +
                    "**Updates require a working Internet connection!**",
      Default = "0",
    };

    public static readonly PluginSetting HubHopUpdateTimeout = new PluginSetting("HubHopUpdateTimeout", DataType.Number) {
      Name = "HubHop Data Update Timeout (seconds)",
      Description = "Maximum number of seconds to wait for a HubHop data update check or download via the Internet.",
      Default = "60",
      MinValue = 0,
      MaxValue = 600
    };

    public static readonly PluginSetting SortLVarsAlpha = new PluginSetting("SortLVarsAlpha", DataType.Switch) {
      Name = "Sort Local Variables Alphabetically (0/1)",
      Description = "Set to `1` to have all Local ('L') simulator variables sorted in alphabetical order within selection lists. Setting to `0` will keep them in the original order they're loaded in on the simulator (by ascending ID).",
      Default = "1",
    };

    public static readonly PluginSetting ActionRepeatInterval = new PluginSetting("ActionRepeatInterval", DataType.Number) {
      Name = "Held Action Repeat Rate (ms)",
      Description = "Stores the held action repeat rate, which can be set via the 'MSFS - Plugin - Action Repeat Interval' action. This setting cannot be changed from the TP Plugin Settings page (even though it appears to be editable on there).",
      Default = "450",
      MinValue = 50,
      MaxValue = int.MaxValue,
      ReadOnly = true,
      TouchPortalStateId = "ActionRepeatInterval"
    };

  }
}
