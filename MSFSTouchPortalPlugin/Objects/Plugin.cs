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
using MSFSTouchPortalPlugin.Configuration;
using MSFSTouchPortalPlugin.Enums;
using MSFSTouchPortalPlugin.Types;
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
#if WASIM
    [TouchPortalActionMapping(PluginActions.UpdateLocalVarsList, "Update Airplane Local Vars List")]
#endif
#if !FSX
    [TouchPortalActionMapping(PluginActions.UpdateInputEventsList, "Update Airplane Input Events List")]
    [TouchPortalActionMapping(PluginActions.ReRegisterInputEventVars, "Re-Submit Input Event Value Requests")]
    [TouchPortalActionMapping(PluginActions.UpdateHubHopPresets, "Update HubHop Data")]
#endif
    // deprecated mappings
    [TouchPortalActionMapping(PluginActions.ToggleConnection, "Toggle", Deprecated = true)]
    [TouchPortalActionMapping(PluginActions.Connect, "On", Deprecated = true)]
    [TouchPortalActionMapping(PluginActions.Disconnect, "Off", Deprecated = true)]
    [TouchPortalActionMapping(PluginActions.UpdateLocalVarsList, "Update Local Var. List", Deprecated = true)]
    public static readonly object Connection;

    [TouchPortalAction(PluginActions.ActionRepeatInterval, "Action Repeat Interval",
      $"Set the default held action repeat rate, in milliseconds. The minimum value is {PluginConfig.ACTION_REPEAT_RATE_MIN_MS_STR}ms.",
      "Repeat Interval: {0} to/by: {1}",
      $"Set Repeat Interval Milliseconds in Range (min: {PluginConfig.ACTION_REPEAT_RATE_MIN_MS_STR}ms):",
      holdable: true)]
    [TouchPortalActionChoice(Id = "Action")]
    [TouchPortalActionMapping(PluginActions.ActionRepeatIntervalSet, "Set")]
    [TouchPortalActionMapping(PluginActions.ActionRepeatIntervalInc, "Increment")]
    [TouchPortalActionMapping(PluginActions.ActionRepeatIntervalDec, "Decrement")]
    [TouchPortalActionText("450", PluginConfig.ACTION_REPEAT_RATE_MIN_MS, int.MaxValue, Id = "Value")]
    // hidden data fields for connector
    [TouchPortalActionText("Plugin", Id = "FbCatId", SkipForValIndex = true)]
    [TouchPortalActionText("[ActionRepeatInterval]", Id = "FbVarName", SkipForValIndex = true)]
    [TouchPortalConnectorMeta(1000, 50, PluginConfig.ACTION_REPEAT_RATE_MIN_MS, int.MaxValue, false, false, RangeStartIndex = 3)]
    public static readonly object ActionRepeatInterval;

    // Trigger Events

    [TouchPortalAction(PluginActions.SetKnownSimEvent, "Activate a Selected Simulator Event",
      "Activate a Selected Simulator Event. Parameter values are optional and event-specific. The list is imported from MSFS SDK Event IDs Docs (may contain errors or omissions).\n" +
        "The value(s), if any, should evaluate to numeric or remain blank if the value is unused by the event. Using basic math operators and dynamic states in values is possible.",
#if !FSX
      "System /\nCategory {0} Event\nName{1} with\nValue(s) 0:{2} \n1:{3} \n2:{4}",
#else
      "System / Category {0} Event Name {1} with Value {2} (optional)",
#endif
      holdable: true)]
    [TouchPortalActionChoice("[plugin not connected]", "", Id = "SimCatName", Label = "Category")]
    [TouchPortalActionChoice("[select a category]", "", Id = "EvtId", Label = "Event ID")]
    [TouchPortalActionText("", int.MinValue, uint.MaxValue, Id = "Value", Label = "Value 1")]
#if !FSX
    [TouchPortalActionText("", int.MinValue, uint.MaxValue, Id = "Value2", Label = "Value 2")]
    [TouchPortalActionText("", int.MinValue, uint.MaxValue, Id = "Value3", Label = "Value 3")]
#endif
    public static readonly object SetKnownSimEvent;

    [TouchPortalConnector(PluginActions.SetKnownSimEvent, "Activate a Selected Simulator Event",
      "Activate a Selected Simulator Event, setting one parameter value with a slider, with optional extra parameter value(s). The list is imported from MSFS SDK Event IDs Docs.\n" +
        "The extra value(s), if any, should evaluate to numeric or remain blank if the value is unused by the event. Using basic math operators and dynamic states in the extra values is possible.",
#if !FSX
      "System /\nCategory {0} Event\nName {1} in Value\nRange {2}-{3} | With Other\n| Values (opt) {4} {5} Connector\nValue Index {6}"
#else
      "System / Category {0} Event Name {1} in Value Range {2}-{3}"
#endif
    )]
    [TouchPortalActionChoice("[plugin not connected]", "", Id = "SimCatName", Label = "Category")]
    [TouchPortalActionChoice("[select a category]", "", Id = "EvtId", Label = "Event ID")]
    [TouchPortalActionText("-16384", int.MinValue, uint.MaxValue, Id = "RangeMin", Label = "Value Range Minimum")]
    [TouchPortalActionText("16384", int.MinValue, uint.MaxValue, Id = "RangeMax", Label = "Value Range Maximum")]
#if !FSX
    [TouchPortalActionText("", int.MinValue, uint.MaxValue, Id = "Value", Label = "Value 1")]
    [TouchPortalActionText("", int.MinValue, uint.MaxValue, Id = "Value2", Label = "Value 2")]
    //[TouchPortalActionText("", int.MinValue, uint.MaxValue, Id = "Value3", Label = "Value 3")]
    [TouchPortalActionChoice(new[] { "0-first", "1-mid", "2-last", /*"3-last"*/ }, Id = "ConnValIdx", Label = "Conn. Value Index")]
#endif
    [TouchPortalConnectorMeta(InsertValueRange = false)]
    public static readonly object SetKnownSimEventConn;


    [TouchPortalAction(PluginActions.SetCustomSimEvent, "Activate a Named Simulator Event",
      "Trigger any Simulator Event by name with optional parameter value(s). See MSFS SDK Documentation 'Event IDs' for reference.\n" +
        "The value(s), if any, should evaluate to numeric or remain blank if the value is unused by the event. Using basic math operators and dynamic state values is possible.",
#if !FSX
      "Activate Event {0} with\nValue(s) 0:{1} \n1:{2} \n2:{3} \n3:{4} \n4:{5}",  // non-breaking narrow space U+202F (TP ignores "no-break space" U+00AD)
#else
      "Activate Event {0} with Value {1} (optional)",
#endif
      holdable: true)]
    [TouchPortalActionText("SIMULATOR_EVENT_NAME", Id = "EvtId", Label = "Event ID")]
    [TouchPortalActionText("", int.MinValue, uint.MaxValue, Id = "Value", Label = "Value 1")]
#if !FSX
    [TouchPortalActionText("", int.MinValue, uint.MaxValue, Id = "Value2", Label = "Value 2")]
    [TouchPortalActionText("", int.MinValue, uint.MaxValue, Id = "Value3", Label = "Value 3")]
    [TouchPortalActionText("", int.MinValue, uint.MaxValue, Id = "Value4", Label = "Value 4")]
    [TouchPortalActionText("", int.MinValue, uint.MaxValue, Id = "Value5", Label = "Value 5")]
#endif
    public static readonly object SetCustomSimEvent;

    [TouchPortalConnector(PluginActions.SetCustomSimEvent, "Activate a Named Simulator Event",
      "Trigger any Simulator Event by name setting one parameter value with a slider, with optional extra parameter value(s). See MSFS SDK Documentation 'Event IDs' for reference.\n" +
        "The extra value(s), if any, should evaluate to numeric or remain blank if the value is unused by the event. Using basic math operators and dynamic state the extra values is possible.",
#if !FSX
      "Set Event:{0} in Value\nRange {1}-{2} | With Other\n| Values (opt) {3} {4} Connector\nValue Index {5}"
#else
      "Set Event:{0} in Value\nRange {1}-{2}"
#endif
    )]
    [TouchPortalActionText("SIMULATOR_EVENT_NAME", Id = "EvtId", Label = "Event ID")]
    [TouchPortalActionText("-16384", int.MinValue, uint.MaxValue, Id = "RangeMin", Label = "Value Range Minimum")]
    [TouchPortalActionText("16384", int.MinValue, uint.MaxValue, Id = "RangeMax", Label = "Value Range Maximum")]
#if !FSX
    [TouchPortalActionText("", int.MinValue, uint.MaxValue, Id = "Value", Label = "Value 1")]
    [TouchPortalActionText("", int.MinValue, uint.MaxValue, Id = "Value2", Label = "Value 2")]
    //[TouchPortalActionText("", int.MinValue, uint.MaxValue, Id = "Value3", Label = "Value 3")]
    [TouchPortalActionChoice(new[] { "0-first", "1-mid", "2-last", /*"3-last"*/ }, Id = "ConnValIdx", Label = "Conn. Value Index")]
#endif
    [TouchPortalConnectorMeta(InsertValueRange = false)]
    public static readonly object SetCustomSimEventConn;


#if WASIM
    [TouchPortalAction(PluginActions.SetHubHopEvent, "Activate an Input Event From HubHop",
      "\t\t\t\t\t\t\t\t\t\t\t\t\t** Requires WASimModule or MobiFlight. **\n" +
        "Trigger a Simulator Event from loaded HubHop data.\t\t" +
        "\"Potentiometer\" type events are indicated with (@) in the name and are only supported with WASimModule (using the provided numeric value).",
      format: "Aircraft/Device: {0} System: {1} Event Name: {2} with value {3} (if any)",
      connectorFormat: "Aircraft\nDevice:{0}System:{1}Event\nName:{2}Value\nRange:",
      holdable: true)]
    [TouchPortalActionChoice("[plugin not connected]", "", Id = "VendorAircraft", Label = "Aircraft")]
    [TouchPortalActionChoice("[select an aircraft]", "", Id = "System", Label = "System")]
    [TouchPortalActionChoice("[select a system]", "", Id = "EvtId", Label = "Event Name")]
    [TouchPortalActionText("", int.MinValue, uint.MaxValue, Id = "Value", Label = "Value")]
    [TouchPortalConnectorMeta()]
    public static readonly object SetHubHopEvent;
#endif

    // Set Variables

    [TouchPortalAction(PluginActions.SetSimulatorVar, "Set a Selected Simulator Variable",
      "Set a Selected Simulator Variable.\n" +
        "The list of Sim Vars is imported from MSFS SDK Simulator Variables Docs (may contain errors or omissions). Variables with \":N\" in the name require an Index value.",
      "System /\nCategory{0} Variable:{1} Index\n(if req'd):{2} Unit:{3} Value:{4}",  /* | Release\n| AI (opt):{5} */
      holdable: true)]
    [TouchPortalActionChoice("[plugin not connected]", "", Id = "CatId", Label = "Category")]
    [TouchPortalActionChoice("[select a category]", "", Id = "VarName", Label = "Variable Name")]
    [TouchPortalActionNumeric(0, 0, 99, false, Id = "VarIndex", Label = "Variable Index")]
    [TouchPortalActionChoice("[select a variable]", "", Id = "Unit", Label = "Unit Name")]
    [TouchPortalActionText("0", float.MinValue, float.MaxValue, Id = "Value", Label = "Value")]
    //[TouchPortalActionSwitch(false, Id = "RelAI", Label = "Release AI")]
    public static readonly object SetSimulatorVar;

    [TouchPortalConnector(PluginActions.SetSimulatorVar, "Set a Selected Simulator Variable",
      "Set a Selected Simulator Variable.\n" +
        "The list of Sim Vars is imported from MSFS SDK Simulator Variables Docs (may contain errors or omissions). Variables with \":N\" in the name require an Index value.",
      "System /\nCategory{0} Variable:{1} Index\n(if req'd):{2} Unit:{3} in Value\nRange:{4}-{5} | With\n| Feedback:{6}" /* | Release\n| AI (opt):{7}*/
    )]
    [TouchPortalActionChoice("[plugin not connected]", "", Id = "CatId", Label = "Category")]
    [TouchPortalActionChoice("[select a category]", "", Id = "VarName", Label = "Variable Name")]
    [TouchPortalActionNumeric(0, 0, 99, false, Id = "VarIndex", Label = "Variable Index")]
    [TouchPortalActionChoice("[select a variable]", "", Id = "Unit", Label = "Unit Name")]
    [TouchPortalActionText("-16384", int.MinValue, uint.MaxValue, Id = "RangeMin", Label = "Value Range Minimum")]
    [TouchPortalActionText("16384", int.MinValue, uint.MaxValue, Id = "RangeMax", Label = "Value Range Maximum")]
    [TouchPortalActionSwitch(false, Id = "FB", Label = "With Feedback")]
    //[TouchPortalActionSwitch(false, Id = "RelAI", Label = "Release AI")]
    public static readonly object SetSimulatorVarConn;

#if !FSX
    [TouchPortalAction(PluginActions.SetLocalVar, "Set a Selected Airplane Local Variable",
      "Sets a value on a Local variable from currently loaded aircraft.\n" +
        "The Unit type is usually \"number\" and will be ignored except in a few specific instances for some 3rd-party models.",
      "Set Variable:{0} To Value:{1} in Units:\n(opt){2}",
      "Set Variable:{0} Unit:\n(opt){1} in Value\nRange:",
      holdable: true)]
    [TouchPortalActionChoice("[plugin not connected]", "", Id = "VarName", Label = "Variable Name")]
    [TouchPortalActionText("0", float.MinValue, float.MaxValue, Id = "Value", Label = "Value")]
    [TouchPortalActionChoice("number", "number", Id = "Unit", Label = "Unit Name")]
    [TouchPortalConnectorMeta()]
    public static readonly object SetLocalVar;

    [TouchPortalAction(PluginActions.SetInputEvent, "Set a Selected Input Event Value",
      "Sets a value on an Input Event from currently loaded aircraft. These are sometimes also referred to as 'B' type variables and are always model-specific.\n" +
        "The value meaning and type (numeric/string) depends entirely on the specific event. A list of Input Events can be found in the MSFS Dev Mode 'Behaviors' window.",
      "Set Input Event: {0} To Value: {1}",
      "Set Input Event: {0} in Value\nRange:",
      holdable: true)]
    [TouchPortalActionChoice("[plugin not connected]", "", Id = "VarName", Label = "Input Event Name")]
    [TouchPortalActionText("0", float.MinValue, float.MaxValue, Id = "Value", Label = "Value")]
    [TouchPortalConnectorMeta()]
    public static readonly object SetInputEvent;
#endif

    [TouchPortalAction(PluginActions.SetVariable, "Set a Named Variable",
#if FSX
      "Set a Named Simulator Variable.\nFor SimVars requiring an index, include it in the name after a : (colon), eg. \"VARIABLE NAME:1\".",
      "Variable\nName{0} Unit {1} Value {2} ",
      "Variable\nName{0} Unit {1} Value\nRange:",
#else
      "Set a Named Variable.\tSets a value on any named variable of various types." +
        "\t\t\t\t\t** Using 'C', 'H', 'K', and 'Z' types require WASimModule. **\n" +
        "'L' type variables can also be created. 'A' and 'C' types require a Unit, for 'B' and 'L' it is optional (default is 'number'). " +
        "For SimVars requiring an index, include it in the name after a : (colon), eg. \"VARIABLE NAME:1\".",
      "Variable\nType{0} Variable\nName{1} Unit {2} Value {3} ",
      "Variable\nType{0} Variable\nName{1} Unit {2} Value\nRange:",
#endif
      holdable: true)]
#if WASIM
    [TouchPortalActionChoice(new[] { "A: SimVar", "B: Input Event", "C: GPS", "H: HTML Event", "K: Key Event", "L: Local", "Z: Custom SimVar" }, Id = "VarType", Label = "Variable Type")]
#elif !FSX
    [TouchPortalActionChoice(new[] { "A: SimVar", "B: Input Event", "L: Local" }, Id = "VarType", Label = "Variable Type")]
#endif
    [TouchPortalActionText("", Id = "VarName", Label = "Variable Name")]
    [TouchPortalActionChoice("[plugin not connected]", "", Id = "Unit", Label = "Unit Name")]
    [TouchPortalActionText("0", float.MinValue, float.MaxValue, Id = "Value", Label = "Value")]
    [TouchPortalConnectorMeta()]
    public static readonly object SetVariable;


    // Exec Calc Code
#if WASIM
    [TouchPortalAction(PluginActions.ExecCalcCode, "Execute Calculator Code",
      "Execute Calculator Code.\t\t\t\t\t** Requires WASimModule **\n" +
      "Runs any entered string of RPN code through the 'execute_calculator_code' Gauge API function. You may use TP state value macros to insert dynamic data.",
      "Execute this code: {0} (must be valid RPN format)                             ",
      holdable: true)]
    [TouchPortalActionText("1 (>H:AS1000_PFD_SOFTKEYS_1)", Id = "Code", Label = "Code")]
    public static readonly object ExecCalcCode;

    [TouchPortalConnector(PluginActions.ExecCalcCode, "Execute Calculator Code",
      "Runs any entered string of RPN code through the 'execute_calculator_code' Gauge API function. Use a '@' placeholder for the connector value.",
      "Calculator Code:\n(use @ as placeholder(s) for slider value){0} Value\nRange:"
    )]
    [TouchPortalActionText("@ 1 (>K:2:PANEL_LIGHTS_POWER_SETTING_SET)", Id = "Code", Label = "Code")]
    [TouchPortalConnectorMeta()]
    public static readonly object ExecCalcCodeConn;
#endif

    // Visual Feedback

    [TouchPortalConnector(PluginActions.SetConnectorValue, "Visual Feedback Connector",
      "This connector provides only visual feedback by setting the position of a slider based on an existing State/variable value.",
      "Set Connector Value Based on\nFeedback From Category:{0}Variable:{1}Value Range:{2}-{3}"
    )]
    [TouchPortalActionChoice("[plugin not connected]", "", Id = "FbCatId", Label = "Feedback Category")]
    [TouchPortalActionChoice("[select a category]", "", Id = "FbVarName", Label = "Feedback Variable")]
    [TouchPortalActionText("-16384", float.MinValue, float.MaxValue, true, Id = "FbRangeMin", Label = "Feedback Range Minimum")]
    [TouchPortalActionText("16384", float.MinValue, float.MaxValue, true, Id = "FbRangeMax", Label = "Feedback Range Maximum")]
    public static readonly object SetConnectorValue;

    // DEPRECATED

    [TouchPortalAction(PluginActions.SetSimVar, "Set Simulator Variable (SimVar)",
      "", "{0}{1}{2}{3})", "{0}{1}", holdable: true, Deprecated = true)]
    [TouchPortalActionChoice("[connect to plugin]", "", Id = "CatId", Label = "Category")]
    [TouchPortalActionChoice("[select a category or type]", "", Id = "VarName", Label = "Variable Name")]
    [TouchPortalActionText("0", float.MinValue, float.MaxValue, Id = "Value", Label = "Value")]
    [TouchPortalActionSwitch(false, Id = "RelAI", Label = "Release AI")]
    [TouchPortalConnectorMeta(decimals: true, feedback: false, RangeStartIndex = 3)]
    public static readonly object SetSimVar;


    //
    // Events
    //

    public static readonly TouchPortalEvent MessageEvent = new (
      "PluginMessageEvent",
      "Plugin Message Event",
      "When the plugin sends an important informational message",
      [
        [ "Type",    "Event Type (PluginInfo, PluginError, SimError)" ],
        [ "Message", "Event Message" ],
      ]
    ) {
      Description = "This event is emitted when the plugin logs an informative message. This could be an error, warning, or simply informational. " +
        "These are the same messages as contained in the 'Most recent plugin log messages' State, but arrive individually.\n" +
        "- `Type` - source and severity of the message; Can be one of `PluginInfo`, `PluginError`, or `SimError`.\n" +
        "- `Message` - the message text."
    };

    public static readonly TouchPortalEvent SimConnectionEvent = new(
      "SimConnectionEvent",
      "Simulator Connection Change",
      "When simulator connection status changes",
      [
        [ "Status",  "Status (disconnected/connecting/connected)" ],
      ]
    ) {
      Description = "This event is emitted when connection to the simulator changes. " +
        "Using this event is an alternative to watching the 'The status of SimConnect' State for changes, or using the individual event types in 'Simulator System Event'.\n" +
        "- `Status` - the current simulator connection status, which can be one of `disconnected`, `connecting`, or `connected`."
    };

    public static readonly TouchPortalEvent PageChange = new(
      "PageChangeEvent",
      "Touch Portal Device Page Change",
      "When the current page on a Touch Portal device changes",
      [
        [ "PageName",     "New Page Name" ],
        [ "PreviousPage", "Previous Page Name" ],
        [ "DeviceName",   "Device Name" ],
        [ "DeviceId",     "Device ID" ],
        [ "DeviceIP",     "Device IP Address" ],
      ]
    ) {
      Description = "This event is emitted when the plugin detects a new page has been loaded on a Touch Portal device. " +
        "The local states contain the new and previous page names and information about the device, all forwarded from the original Touch Portal notification the plugin received.\n" +
        "- `New Page Name` - Name of the page that was loaded, including the full folder path, if any.\n" +
        "- `Previous Page Name` - Name of the page that was unloaded, including any folder path.\n" +
        "- `Device Name` - Name of the TP device on which the page was loaded.\n" +
        "- `Device ID` - ID of the TP device on which the page was loaded.\n" +
        "- `Device IP Address` - IP Address of the TP device on which the page was loaded.\n"
    };

  }  // PluginMapping

  // Actions for editing variable requests has own category in TP UI, though actually these act as if they were in `Groups.Plugin`.
  [TouchPortalCategory(Groups.StatesEditor)]
  internal static class StatesEditorMapping
  {

    [TouchPortalAction(PluginActions.AddSimulatorVar, "Request a Selected Simulator Variable",
      "Request a Selected Simulator Variable.\n" +
        "The list of Sim Vars is imported from MSFS SDK Simulator Variables Docs (may contain errors or omissions). Variables with \":N\" in the name require an Index value.",
      "System{0} Variable {1} Index\n(if req'd){2} Unit{3} for Plugin\nCategory{4} Format{5} Default\nValue{6} Update\nPeriod{7} Update\nInterval{8} Delta\nEpsilon{9}",
      LayoutAsForm = true
    )]
    [TouchPortalActionChoice("[plugin not connected]", "", Id = "SimCatName", Label = "Simulator System")]
    [TouchPortalActionChoice("[select a category]", "", Id = "VarName", Label = "Variable Name")]
    [TouchPortalActionNumeric(0, 0, 99, false, Id = "VarIndex", Label = "Variable Index")]
    [TouchPortalActionChoice("[select a variable]", "number", Id = "Unit", Label = "Unit Name")]
    [TouchPortalActionChoice("[plugin not connected]", "", Id = "CatId", Label = "Plugin Category", LabelSuffix = "(for sorting in Touch Portal)")]
    [TouchPortalActionText("F2", Id = "Format", Label = "Formatting String", LabelSuffix = "(.NET-style formatting for numeric values)")]
    [TouchPortalActionText("0", Id = "DfltVal", Label = "Default Value")]
    [TouchPortalActionChoice(new[] { /*"Never",*/ "Once", /*"VisualFrame",*/ "SimFrame", "Second", "Millisecond", }, "SimFrame", Id = "UpdPer", Label = "Update Period")]
    [TouchPortalActionNumeric(0, 0, int.MaxValue, false, Id = "UpdInt", Label = "Update Interval", LabelSuffix = "(# of Update Periods to skip between updates)")]
    [TouchPortalActionNumeric(SimVarItem.DELTA_EPSILON_DEFAULT, 0.0, float.MaxValue, true, Id = "Epsilon", Label = "Delta Epsilon", LabelSuffix = "(only for floating-point value types)")]
    public static readonly object AddSimulatorVar;

#if !FSX
    [TouchPortalAction(PluginActions.AddLocalVar, "Request an Airplane Local Variable",
      "Request an Airplane Local Variable.\t\t\t\t\t** Requires WASimModule for list (otherwise use \"Request Named Variable\") **\n" +
      "The list of variables is loaded live from the Simulator." +
        "The Unit type is usually \"number\" and will be ignored except in a few specific instances for some 3rd-party models.",
      "Request\nVariable {0} Unit\n(opt) {1} for Plugin\nCategory{2} Format{3} Default\nValue{4} Update\nPeriod{5} Update\nInterval{6} Delta\nEpsilon{7}",
      LayoutAsForm = true
    )]
    [TouchPortalActionChoice("[simulator not connected]", "", Id = "VarName", Label = "Variable Name")]
    [TouchPortalActionChoice("number", "number", Id = "Unit", Label = "Unit Name")]
    [TouchPortalActionChoice("[plugin not connected]", "", Id = "CatId", Label = "Plugin Category", LabelSuffix = "(for sorting in Touch Portal)")]
    [TouchPortalActionText("F2", Id = "Format", Label = "Formatting String", LabelSuffix = "(.NET-style formatting for numeric values)")]
    [TouchPortalActionText("0", Id = "DfltVal", Label = "Default Value")]
    [TouchPortalActionChoice(new[] { /*"Never",*/ "Once", /*"VisualFrame",*/ "SimFrame", "Second", "Millisecond", }, "SimFrame", Id = "UpdPer", Label = "Update Period")]
    [TouchPortalActionNumeric(0, 0, int.MaxValue, false, Id = "UpdInt", Label = "Update Interval", LabelSuffix = "(# of Update Periods to skip between updates)")]
    [TouchPortalActionNumeric(SimVarItem.DELTA_EPSILON_DEFAULT, 0.0, float.MaxValue, true, Id = "Epsilon", Label = "Delta Epsilon", LabelSuffix = "(only for floating-point value types)")]
    public static readonly object AddLocalVar;

    [TouchPortalAction(PluginActions.AddInputEventVar, "Request an Input Event Value",
      "Request an \"Input Event\" Value.\n" +
      "The list of available variables is loaded live from the Simulator. Input Events are a.k.a. 'B' variables and are always model-specific.",
      "Request\nVariable {0} for Plugin\nCategory{1} Format{2} Default\nValue{3} Update\nPeriod{4} Update\nInterval{5} Delta\nEpsilon{6}",
      LayoutAsForm = true
    )]
    [TouchPortalActionChoice("[simulator not connected]", "", Id = "VarName", Label = "Variable Name")]
    [TouchPortalActionChoice("[plugin not connected]", "", Id = "CatId", Label = "Plugin Category", LabelSuffix = "(for sorting in Touch Portal)")]
    [TouchPortalActionText("F2", Id = "Format", Label = "Formatting String", LabelSuffix = "(.NET-style formatting for numeric values)")]
    [TouchPortalActionText("0", Id = "DfltVal", Label = "Default Value")]
    [TouchPortalActionChoice(new[] { /*"Never",*/ "Once", /*"VisualFrame",*/ "SimFrame", "Second", "Millisecond", }, "SimFrame", Id = "UpdPer", Label = "Update Period")]
    [TouchPortalActionNumeric(0, 0, int.MaxValue, false, Id = "UpdInt", Label = "Update Interval", LabelSuffix = "(# of Update Periods to skip between updates)")]
    [TouchPortalActionNumeric(SimVarItem.DELTA_EPSILON_DEFAULT, 0.0, float.MaxValue, true, Id = "Epsilon", Label = "Delta Epsilon", LabelSuffix = "(only for floating-point value types)")]
    public static readonly object AddInputEventVar;
#endif

    [TouchPortalAction(PluginActions.AddNamedVariable, "Request a Named Variable",
#if FSX
      "Request a Named Simulator Variable.\nFor SimVars requiring an index, include it in the name after a : (colon), eg. \"VARIABLE NAME:1\".",
      "Name{0} Unit{1} for Plugin\nCategory{2} Format{3} Default\nValue{4} Update\nPeriod{5} Update\nInterval{6} Delta\nEpsilon{7}",
#else
      "Request a Named Variable.\t\t\t\t\t** Using types other than 'A', 'B', or 'L' requires WASimModule. **\n" +
        "'A', 'C', & 'E' types require a Unit type, and for 'L' it is optional (default is 'number'). " +
        "For SimVars requiring an index, include it in the name after a : (colon), eg. \"VARIABLE NAME:1\".",
      "Type{0} Name{1} Unit{2} for Plugin\nCategory{3} Format{4} Default\nValue{5} Update\nPeriod{6} Update\nInterval{7} Delta\nEpsilon{8}",
#endif
      LayoutAsForm = true
    )]
#if WASIM
    [TouchPortalActionChoice(new[] { "A: SimVar", "B: Input Event", "C: GPS", "E: Env.", "L: Local", "M: Mouse", "R: Rsrc.", "T: Token", "Z: Custom" }, Id = "VarType", Label = "Variable Type")]
#elif !FSX
    [TouchPortalActionChoice(new[] { "A: SimVar", "B: Input Event", "L: Local" }, Id = "VarType", Label = "Variable Type")]
#endif
    [TouchPortalActionText("FULL VARIABLE NAME", Id = "VarName", Label = "Variable Name", LabelSuffix = "(include SimVar index in name after colon, if required)")]
    [TouchPortalActionChoice("[plugin not connected]", "number", Id = "Unit", Label = "Unit Name"
#if !FSX
      , LabelSuffix = "(required for 'A', 'C', & 'E' types, optional for 'L')"
#endif
     )]
    [TouchPortalActionChoice("[plugin not connected]", "", Id = "CatId", Label = "Plugin Category", LabelSuffix = "(for sorting in Touch Portal)")]
    [TouchPortalActionText("F2", Id = "Format", Label = "Formatting String", LabelSuffix = "(.NET-style formatting for numeric values)")]
    [TouchPortalActionText("0", Id = "DfltVal", Label = "Default Value")]
    [TouchPortalActionChoice(new[] { /*"Never",*/ "Once", "SimFrame", "Second", "Millisecond", }, "SimFrame", Id = "UpdPer", Label = "Update Period")]
    [TouchPortalActionNumeric(0, 0, int.MaxValue, false, Id = "UpdInt", Label = "Update Interval", LabelSuffix = "(# of Update Periods to skip between updates)")]
    [TouchPortalActionNumeric(SimVarItem.DELTA_EPSILON_DEFAULT, 0.0, float.MaxValue, true, Id = "Epsilon", Label = "Delta Epsilon", LabelSuffix = "(only for floating-point value types)")]
    public static readonly object AddNamedVariable;

#if WASIM
    [TouchPortalAction(PluginActions.AddCalculatedValue, "Request a Calculated Value",
      "Request a Calculated Value.\t\t\t\t\t** Requires WASimModule **",
      "Calculator\nCode{0} Result\nType{1} State\nName{2} for Plugin\nCategory{3} Format{4} Default\nValue{5} Update\nPeriod{6} Update\nInterval{7} Delta\nEpsilon{8}",
      LayoutAsForm = true
    )]
    [TouchPortalActionText("(A:TRAILING EDGE FLAPS LEFT ANGLE, degrees) 30 - abs 0.1 <", Id = "CalcCode", Label = "Calculator Code")]
    [TouchPortalActionChoice(new[] { "Double", "Integer", "String", "Formatted" }, Id = "ResType", Label = "Result Type")]
    [TouchPortalActionText("A name for the States list", Id = "VarName", Label = "Touch Portal State Name")]
    [TouchPortalActionChoice("[plugin not connected]", "", Id = "CatId", Label = "Plugin Category", LabelSuffix = "(for sorting in Touch Portal)")]
    [TouchPortalActionText("F2", Id = "Format", Label = "Formatting String", LabelSuffix = "(.NET-style formatting for numeric values)")]
    [TouchPortalActionText("0", Id = "DfltVal", Label = "Default Value")]
    [TouchPortalActionChoice(new[] { /*"Never",*/ "Once", "SimFrame", "Second", "Millisecond", }, "SimFrame", Id = "UpdPer", Label = "Update Period")]
    [TouchPortalActionNumeric(0, 0, int.MaxValue, false, Id = "UpdInt", Label = "Update Interval", LabelSuffix = "(# of Update Periods to skip between updates)")]
    [TouchPortalActionNumeric(SimVarItem.DELTA_EPSILON_DEFAULT, 0.0, float.MaxValue, true, Id = "Epsilon", Label = "Delta Epsilon", LabelSuffix = "(only for floating-point value types)")]
    public static readonly object AddCalculatedValue;
#endif

    [TouchPortalAction(PluginActions.UpdateVarValue, "Update a Variable Value",
      "Request a value update for an added variable. This is especially useful for variables with a \"Once\" type Update Period.",
      "From Category {0} Update Variable {1}")]
    [TouchPortalActionChoice("[plugin not connected]", "", Id = "CatId", Label = "Plugin Category")]
    [TouchPortalActionChoice("[select a category]", "", Id = "VarName", Label = "Variable Name")]
    public static readonly object UpdateVarValue;

    [TouchPortalAction(PluginActions.RemoveSimVar, "Remove a Simulator Variable",
      "Remove an existing Simulator Variable.",
      "From Category {0} Remove Variable {1}")]
    [TouchPortalActionChoice("[plugin not connected]", "", Id = "CatId", Label = "Plugin Category")]
    [TouchPortalActionChoice("[select a category]", "", Id = "VarName", Label = "Variable Name")]
    public static readonly object RemoveSimVar;

    [TouchPortalAction(PluginActions.ClearSimVars, "Clear Variable Definitions",
      "Removes either all or only custom-added variable state definitions.",
      "Clear Variables: {0} Select File/Type: {1}")]
    [TouchPortalActionChoice(Id = "VarsSet", Label = "Variables Set")]
    [TouchPortalActionMapping(PluginActions.ClearCustomSimVars, "Custom")]
    [TouchPortalActionMapping(PluginActions.ClearAllSimVars, "All")]
    [TouchPortalActionMapping(PluginActions.ClearSimVarsFromFile, "Loaded From Selected File")]
    [TouchPortalActionMapping(PluginActions.ClearSimVarsOfType, "Of Selected Type")]
    [TouchPortalActionChoice("N/A", "", Id = "VarsSpec", Label = "Variable File/Type", UsedInMapping = false)]
    public static readonly object ClearSimVars;

    [TouchPortalAction(PluginActions.LoadSimVars, "Load Variable Definitions From File",
      "Load a set of variable state definitions from a configuration file.",
      "Load definitions from file {0} (file name only for user config. folder, or full path with file name)")]
    [TouchPortalActionFile("CustomStates.ini", Id = "VarsFile", Label = "Load From File")]
    public static readonly object LoadSimVars;

    [TouchPortalAction(PluginActions.SaveSimVars, "Save Variable Definitions To File",
      "Save Variable Definitions To File.\nSave the current simulator variable state definitions to a configuration file.",
      "Save {0} states to file {1} (file name only for user config. folder, or full path with file name)")]
    //"Save Variable Definitions To File. Save the current simulator variable state definitions to a configuration file.\n" +
    //  "'File' can be with a full path or just file name, default path is in Settings. The File '...' selector does not allow creating new files, select a Folder and type in a File name instead.",
    //"Save {0} variable requests to File {1} (in Folder {2} optional: empty for user config. folder, or specify a path in File)")]
    [TouchPortalActionChoice(Id = "VarsSet", Label = "Variables Set")]
    [TouchPortalActionMapping(PluginActions.SaveCustomSimVars, "Custom")]
    [TouchPortalActionMapping(PluginActions.SaveAllSimVars, "All")]
    [TouchPortalActionText("CustomStates.ini", Id = "VarsFile", Label = "Save to File")]
    //[TouchPortalActionFile("CustomStates.ini", Id = "VarsFile", Label = "Save to File")]  // doesn't allow selecting a non-existing file as of TP 3.1b8
    //[TouchPortalActionFolder("", Id = "VarsDir", Label = "Save in Folder")]  // doesn't allow freeform entry as of TP 3.1b8
    public static readonly object SaveSimVars;


    // DEPRECATED

    [TouchPortalAction(PluginActions.AddCustomSimVar, "Request a Custom Simulator Variable", "", "{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}", Deprecated = true)]
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

    [TouchPortalAction(PluginActions.AddKnownSimVar, "Request a Variable From List", "", "{0}{1}{2}{3}{4}{5}{6}{7}{8}", Deprecated = true)]
    [TouchPortalActionChoice("[connect to plugin]", "", Id = "SimCatName", Label = "Category or Type")]
    [TouchPortalActionChoice("[select a category]", "", Id = "VarName", Label = "Simulator Variable Name")]
    [TouchPortalActionNumeric(0, 0, 99, false, Id = "VarIndex", Label = "Variable Index")]
    [TouchPortalActionChoice("[select a variable]", "number", Id = "Unit", Label = "Unit Name")]
    [TouchPortalActionChoice("[connect plugin]", "", Id = "CatId", Label = "Category")]
    [TouchPortalActionText("F2", Id = "Format", Label = "Formatting String")]
    [TouchPortalActionText("0", Id = "DfltVal", Label = "Default Value")]
    [TouchPortalActionChoice(new[] { /*"Never",*/ "Once", /*"VisualFrame",*/ "SimFrame", "Second", "Millisecond", }, "SimFrame", Id = "UpdPer", Label = "Update Period")]
    [TouchPortalActionNumeric(0, 0, int.MaxValue, false, Id = "UpdInt", Label = "Interval")]
    [TouchPortalActionNumeric(SimVarItem.DELTA_EPSILON_DEFAULT, 0.0, float.MaxValue, true, Id = "Epsilon", Label = "Delta Epsilon")]
    public static readonly object AddKnownSimVar;

  }
}
