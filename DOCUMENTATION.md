# MSFS Touch Portal Plugin Documentation

This plugin provides a two-way interface between Touch Portal and Flight Simulators which use SimConnect, such as Microsoft Flight Simulator 2020 and FS-X.

For further documentation, please see https://github.com/mpaperno/MSFSTouchPortalPlugin/wiki

This documentation generated for plugin v1.1.0.3-beta3

---

## Table of Contents

[Plugin Settings](#plugin-settings)

[Actions and States by Category](#actions-and-states-by-category)

* [Plugin](#plugin)

* [AutoPilot](#autopilot)

* [Communication](#communication)

* [Electrical](#electrical)

* [Engine](#engine)

* [Environment](#environment)

* [Failures](#failures)

* [Flight Instruments](#flight-instruments)

* [Flight Systems](#flight-systems)

* [Fuel](#fuel)

* [System](#system)

---

## Plugin Settings
<details><summary><sub>Click to expand</sub></summary>

### Connect To Flight Sim on Startup (0/1)

| Type | Default Value | Min. Value | Max. Value |
| --- | --- | --- | --- |
| boolean (on/off) | 0 | 0 | 1 |

Set to 1 to automatically attempt connection to flight simulator upon Touch Portal startup. Set to 0 to only connect manually via the Action *MSFS - Plugin -> Connect & Update*.

### Sim Variable State Config File(s) (blank = Default)

| Type | Default Value | Max. Length |
| --- | --- | --- |
| text | Default | 255 |

Here you can specify one or more custom configuration files which define SimConnect variables to request as Touch Portal States. This plugin comes with an extensive default set of states, however since the possibilities between which variables are requested, which units they are displayed in,and how they are formatted are almost endless. This option provides a way to customize the output as desired.

Enter a file name here, with or w/out the suffix (`.ini` is assumed). Separate multiple files with commas (and optional space). To include the default set of variables/states, use the name `Default` as one of the file names (in any position of the list).

Files are loaded in the order in which they appear in the list, and in case of conflicting state IDs, the last one found will be used.

The custom file(s) are expected to be in the folder specified in the "User Config Files Path" setting (see below).

### SimConnect.cfg Index (0 for MSFS, 1 for FSX, or custom)

| Type | Default Value | Min. Value | Max. Value |
| --- | --- | --- | --- |
| number | 0 | 0 | 20 |

A __SimConnect.cfg__ file can contain a number of connection configurations, identified in sections with the `[SimConnect.N]` title. A default __SimConnect.cfg__ is included with this plugin (in the installation folder). You may also use a custom configuration file stored in the "User Config Files Path" folder (see below). 

The index number can be specified in this setting. This is useful for 
  1. compatibility with FSX, and/or 
  2. custom configurations over network connections (running Touch Portal on a different computer than the sim). 

The default configuration index is zero, which (in the included default SimConnect.cfg) is suitable for MSFS (2020). Use the index 1 for compatibility with FSX (or perhaps other sims).

See here for more info about the file format: https://docs.flightsimulator.com/html/Programming_Tools/SimConnect/SimConnect_CFG_Definition.htm  

For more information on using Touch Portal remotely see [Multiple Touch Portal Device Setup](https://github.com/mpaperno/MSFSTouchPortalPlugin/wiki/Multiple-Touch-Portal-Device-Setup)

### User Config Files Path (blank for default)

| Type | Default Value | Max. Length |
| --- | --- | --- |
| text |  | 255 |

The system path where plugin settings are stored, including custom user State configuration files for sate definitions & _SimConnect.cfg_ .
 Keep it blank for default, which is `C:\Users\<UserName>\AppData\Roaming\MSFSTouchPortalPlugin`.

Note that using this plugin's installation folder for custom data storage is not recommended, since anything in there will likely get overwritten during a plugin update/re-install.

### Ignore Local Number Format Rules (0/1)

| Type | Default Value | Min. Value | Max. Value |
| --- | --- | --- | --- |
| boolean (on/off) | 1 | 0 | 1 |

Touch Portal cannot perform math or numeric comparison operations on decimal numbers formatted with comma decimal separator, even in locations where this is the norm. Set this setting to 1/true toalways format numbers in "neutral" format with period decimal separators. **NOTE** that this affects **input** number formatting as well (the plugin will expect all numbers with period decimal separators regardless of your location).

### Update HubHop Data on Startup (0/1)

| Type | Default Value | Min. Value | Max. Value |
| --- | --- | --- | --- |
| boolean (on/off) | 0 | 0 | 1 |

Set to 1 to automatically load latest HubHop data when plugin starts. Set to 0 to disable. Updates can always be triggered manually via the Action *MSFS - Plugin -> Connect & Update*. **Updates require a working Internet connection!**

### HubHop Data Update Timeout (seconds)

| Type | Default Value | Min. Value | Max. Value |
| --- | --- | --- | --- |
| number | 60 | 0 | 600 |

Maximum number of seconds to wait for a HubHop data update check or download via the Internet.

### Sort Local Variables Alphabetically (0/1)

| Type | Default Value | Min. Value | Max. Value |
| --- | --- | --- | --- |
| boolean (on/off) | 1 | 0 | 1 |

Set to `1` to have all Local ('L') simulator variables sorted in alphabetical order within selection lists. Setting to `0` will keep them in the original order they're loaded in on the simulator (by ascending ID).

</details>

---

## Actions and States by Category

### Plugin
<details><summary><sub>Click to expand</sub></summary>

#### Actions

<table>
<tr valign='bottom'><th>Name</th><th>Description</th><th>Format</th><th nowrap>Data<br/><div align=left><sub>index. &nbsp; [type] &nbsp; &nbsp; choices/default (in bold)</th><th>On<br/>Hold</sub></div></th></tr>
<tr valign='top'><td>Connect & Update</td><td>Control connection to the Simulator, or perform various data update tasks.</td><td>Plugin Action: {0}</td><td><ol start=0>
<li>[choice] &nbsp; <b>Toggle Simulator Connection</b>, Connect to Simulator, Disconnect from Simulator, Reload State Files, Re-Send All State Values, Update Local Var. List, Re-Submit Local Var. Requests, Update HubHop Data</li>
</ol></td>
<td align='center'></td></tr>
<tr valign='top'><td>Action Repeat Interval</td><td>Held Action Repeat Rate (ms)</td><td>Repeat Interval: {0} to/by: {1} ms</td><td><ol start=0>
<li>[choice] &nbsp; <b>Set</b>, Increment, Decrement</li>
<li>[text] &nbsp; <b>450</b> &nbsp; <sub>&lt;min: 50&gt;</sub> <sub>&lt;max: 2147483647&gt;</sub></li>
<li>[text] &nbsp; <b>Plugin</b></li>
<li>[text] &nbsp; <b>[ActionRepeatInterval]</b></li>
</ol></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Activate a Named Simulator Event</td><td>Trigger or set value of a Simulator Event by name.
The value, if any, should evaluate to numeric. Using basic math operators and dynamic state values is possible.</td><td>Activate Event {0} with value {1} (if any)</td><td><ol start=0>
<li>[text] &nbsp; <b>SIMULATOR_EVENT_NAME</b></li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
</ol></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Activate a Simulator Event From List</td><td>Trigger or set value of a Simulator Event selected from a list of imported events.
The value, if any, should evaluate to numeric. Using basic math operators and dynamic state values is possible.</td><td>From Category {0} Activate Event {1} with value {2} (if any)</td><td><ol start=0>
<li>[choice] &nbsp; <b></b>[plugin not connected]</li>
<li>[choice] &nbsp; <b></b>[select a category]</li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
</ol></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Activate an Input Event From HubHop</td><td>																** Requires WASimModule or MobiFlight. **
Trigger a Simulator Event from loaded HubHop data.					"Potentiometer" type events are only supported with WASimModule (using the provided value, which should evaluate to numeric).</td><td>Aircraft/Device: {0} System: {1} Event Name: {2} with value {3} (if any)</td><td><ol start=0>
<li>[choice] &nbsp; <b></b>[plugin not connected]</li>
<li>[choice] &nbsp; <b></b>[select an aircraft]</li>
<li>[choice] &nbsp; <b></b>[select a system]</li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
</ol></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Set Simulator Variable (SimVar)</td><td>Sets the value of a Simulator Variable selected from a list of Sim Vars which are marked as settable.</td><td>From Category:{0}Set Variable:{1}To:{2} (Release AI:{3})</td><td><ol start=0>
<li>[choice] &nbsp; <b></b>[connect to plugin]</li>
<li>[choice] &nbsp; <b></b>[select a category or type]</li>
<li>[text] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
<li>[switch] &nbsp; <b>False</b></li>
</ol></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Set Airplane Local Variable</td><td>Sets a value on a Local variable from currently loaded aircraft.					** Requires WASimModule **</td><td>Set Variable:{0}To:{1}in Units (opt){2}</td><td><ol start=0>
<li>[choice] &nbsp; <b></b>[connect to plugin]</li>
<li>[text] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
<li>[choice] &nbsp; <b></b>[connect to plugin]</li>
</ol></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Set Named Variable Value</td><td>Set a Named Variable
Sets a value on any named variable, by type of variable. Local (L) variables can also be created. SimVar types require a Unit specifier.					** Requires WASimModule **</td><td>Set
Variable Type:{0}Named:{1} to Value:{2} in Units
(opt){3} (Create 'L' var: {4}) (release AI: {5})</td><td><ol start=0>
<li>[choice] &nbsp; <b>A: SimVar</b>, C: GPS, H: HTML Event, K: Key Event, L: Local, Z: Custom SimVar</li>
<li>[text] &nbsp; &lt;empty&gt;</li>
<li>[text] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
<li>[choice] &nbsp; <b></b>[connect to plugin]</li>
<li>[choice] &nbsp; <b>N/A</b>, No, Yes</li>
<li>[switch] &nbsp; <b>False</b></li>
</ol></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Execute Calculator Code</td><td>Execute Calculator Code
Runs any entered string of RPN code through the 'execute_calculator_code' Gauge API function. You may use TP state value macros to insert dynamic data.</td><td>Execute this code: {0} (must be valid RPN format)                             ** Requires WASimModule **</td><td><ol start=0>
<li>[text] &nbsp; <b>1 (>H:AS1000_PFD_SOFTKEYS_1)</b></li>
</ol></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Request a Custom Simulator Variable</td><td>Request Simulator Variable Name:                            Index (if req'd):               Units:                                                                Category:                                    Format:                            Default Value:                 Settable:           Update Period:                   Update Interval:               Delta Epsilon:     </td><td>{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}</td><td><ol start=0>
<li>[text] &nbsp; <b>SIMULATOR VARIABLE FULL NAME</b></li>
<li>[number] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: 0&gt;</sub> <sub>&lt;max: 99&gt;</sub></li>
<li>[choice] &nbsp; <b></b>[connect to plugin]                                 </li>
<li>[choice] &nbsp; <b></b>[connect plugin]</li>
<li>[text] &nbsp; <b>F2</b></li>
<li>[text] &nbsp; <b>0</b></li>
<li>[switch] &nbsp; <b>False</b></li>
<li>[choice] &nbsp; Once, <b>SimFrame</b>, Second, Millisecond</li>
<li>[number] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: 0&gt;</sub> <sub>&lt;max: 2147483647&gt;</sub></li>
<li>[number] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: 0.00&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440.00&gt;</sub></li>
</ol></td>
<td align='center'></td></tr>
<tr valign='top'><td>Request a Variable From List</td><td>Request a Variable from Simulator					** Local Variables support requires WASimModule **
Category or Local Aircraft          Request Variable                                                                                               Index (if req'd):                Units:                                                                 Plugin Category:                         Format:                            Default Value:                 Update Period:                  Update Interval:              Delta Epsilon:      </td><td>{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}</td><td><ol start=0>
<li>[choice] &nbsp; <b></b>[connect to plugin]</li>
<li>[choice] &nbsp; <b></b>[select a category]                                                            </li>
<li>[number] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: 0&gt;</sub> <sub>&lt;max: 99&gt;</sub></li>
<li>[choice] &nbsp; [connect to plugin]                                 </li>
<li>[choice] &nbsp; <b></b>[connect plugin]</li>
<li>[text] &nbsp; <b>F2</b></li>
<li>[text] &nbsp; <b>0</b></li>
<li>[choice] &nbsp; Once, <b>SimFrame</b>, Second, Millisecond</li>
<li>[number] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: 0&gt;</sub> <sub>&lt;max: 2147483647&gt;</sub></li>
<li>[number] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: 0.00&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440.00&gt;</sub></li>
</ol></td>
<td align='center'></td></tr>
<tr valign='top'><td>Request a Named Variable</td><td>Request a Named Variable                       For indexed SimVars, include it in the name after a : (colon).					** Requires WASimModule **
Variable Type:                Name:                                                   Units (optional):                                               Plugin Category:                         Format:                            Default Value:                 Update Period:                Update Interval:              Delta Epsilon:     </td><td>{0}{1}{2}{3}{4}{5}{6}{7}{8}</td><td><ol start=0>
<li>[choice] &nbsp; <b>A: SimVar</b>, B: Input, C: GPS, E: Env., L: Local, M: Mouse, R: Rsrc., T: Token, Z: Custom</li>
<li>[text] &nbsp; <b>FULL VARIABLE NAME</b></li>
<li>[choice] &nbsp; [connect to plugin]                                 </li>
<li>[choice] &nbsp; <b></b>[connect plugin]</li>
<li>[text] &nbsp; <b>F2</b></li>
<li>[text] &nbsp; <b>0</b></li>
<li>[choice] &nbsp; Once, <b>SimFrame</b>, Second, Millisecond</li>
<li>[number] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: 0&gt;</sub> <sub>&lt;max: 2147483647&gt;</sub></li>
<li>[number] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: 0.00&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440.00&gt;</sub></li>
</ol></td>
<td align='center'></td></tr>
<tr valign='top'><td>Request a Calculated Value</td><td>Request a Calculated Value					** Requires WASimModule **
Calculator Code:                                                                                                         Result Type:                  Plugin Category:                         State Name:                                                Format:                            Default Value:                 Update Period:                Update Interval:              Delta Epsilon:     </td><td>{0}{1}{2}{3}{4}{5}{6}{7}{8}</td><td><ol start=0>
<li>[text] &nbsp; <b>(A:TRAILING EDGE FLAPS LEFT ANGLE, degrees) 30 - abs 0.1 <</b></li>
<li>[choice] &nbsp; <b>Double</b>, Integer, String, Formatted</li>
<li>[choice] &nbsp; <b></b>[connect plugin]</li>
<li>[text] &nbsp; <b>A name for the States list</b></li>
<li>[text] &nbsp; <b>F2</b></li>
<li>[text] &nbsp; <b>0</b></li>
<li>[choice] &nbsp; Once, <b>SimFrame</b>, Second, Millisecond</li>
<li>[number] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: 0&gt;</sub> <sub>&lt;max: 2147483647&gt;</sub></li>
<li>[number] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: 0.00&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440.00&gt;</sub></li>
</ol></td>
<td align='center'></td></tr>
<tr valign='top'><td>Update a Variable Value</td><td>Request a value update for an added variable. This is especially useful for variables with a "Once" type Update Period.</td><td>From Category {0} Update Variable {1}</td><td><ol start=0>
<li>[choice] &nbsp; <b></b>[connect plugin]</li>
<li>[choice] &nbsp; <b></b>[select a category]</li>
</ol></td>
<td align='center'></td></tr>
<tr valign='top'><td>Remove a Simulator Variable</td><td>Remove an existing Simulator Variable.</td><td>From Category {0} Remove Variable {1}</td><td><ol start=0>
<li>[choice] &nbsp; <b></b>[connect plugin]</li>
<li>[choice] &nbsp; <b></b>[select a category]</li>
</ol></td>
<td align='center'></td></tr>
<tr valign='top'><td>Clear Variable Definitions</td><td>Removes either all or only custom-added variable state definitions.</td><td>Clear {0} states</td><td><ol start=0>
<li>[choice] &nbsp; <b>Custom</b>, All</li>
</ol></td>
<td align='center'></td></tr>
<tr valign='top'><td>Load Variable Definitions From File</td><td>Load a set of variable state definitions from a configuration file.</td><td>Load definitions from file {0} (file name only for user config. folder, or full path with file name)</td><td><ol start=0>
<li>[text] &nbsp; <b>CustomStates.ini</b></li>
</ol></td>
<td align='center'></td></tr>
<tr valign='top'><td>Save Variable Definitions To File</td><td>Save the current simulator variable state definitions to a configuration file.</td><td>Save {0} states to file {1} (file name only for user config. folder, or full path with file name)</td><td><ol start=0>
<li>[choice] &nbsp; <b>Custom</b>, All</li>
<li>[text] &nbsp; <b>CustomStates.ini</b></li>
</ol></td>
<td align='center'></td></tr>
</table>


#### Connectors

<table>
<tr valign='bottom'><th>Name</th><th>Description</th><th>Format</th><th nowrap>Data<br/><div align=left><sub>index. &nbsp; [type] &nbsp; &nbsp; choices/default (in bold)</th></tr>
<tr valign='top'><td>Action Repeat Interval</td><td>Held Action Repeat Rate (ms)</td><td>Set Repeat Interval in Range (ms):{3}-{4}</td><td><ol start=0>
<li>[choice] &nbsp; <b>Set</b>, Increment, Decrement</li>
<li>[text] &nbsp; <b>Plugin</b></li>
<li>[text] &nbsp; <b>[ActionRepeatInterval]</b></li>
<li>[text] &nbsp; <b>1000</b> &nbsp; <sub>&lt;min: 50&gt;</sub> <sub>&lt;max: 2147483647&gt;</sub></li>
<li>[text] &nbsp; <b>50</b> &nbsp; <sub>&lt;min: 50&gt;</sub> <sub>&lt;max: 2147483647&gt;</sub></li>
</ol></td>
<tr valign='top'><td>Activate a Named Simulator Event</td><td>Trigger or set value of a Simulator Event by name.
The value, if any, should evaluate to numeric. Using basic math operators and dynamic state values is possible.</td><td>Set Event:{0}Value
Range:{1}-{2}| Feedback From
| State (opt):{3}{4}
Range:{5}-{6}</td><td><ol start=0>
<li>[text] &nbsp; <b>SIMULATOR_EVENT_NAME</b></li>
<li>[text] &nbsp; <b>-16384</b> &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440.00&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440.00&gt;</sub></li>
<li>[text] &nbsp; <b>16384</b> &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440.00&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440.00&gt;</sub></li>
<li>[choice] &nbsp; <b></b>[connect plugin]</li>
<li>[choice] &nbsp; <b></b>[select a category]</li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
</ol></td>
<tr valign='top'><td>Activate a Simulator Event From List</td><td>Trigger or set value of a Simulator Event selected from a list of imported events.
The value, if any, should evaluate to numeric. Using basic math operators and dynamic state values is possible.</td><td>From
Category:{0}Activate
Event:{1}Value
Range:{2}-{3}| Feedback From
| State (opt):{4}{5}
Range:{6}-{7}</td><td><ol start=0>
<li>[choice] &nbsp; <b></b>[plugin not connected]</li>
<li>[choice] &nbsp; <b></b>[select a category]</li>
<li>[text] &nbsp; <b>-16384</b> &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440.00&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440.00&gt;</sub></li>
<li>[text] &nbsp; <b>16384</b> &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440.00&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440.00&gt;</sub></li>
<li>[choice] &nbsp; <b></b>[connect plugin]</li>
<li>[choice] &nbsp; <b></b>[select a category]</li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
</ol></td>
<tr valign='top'><td>Activate an Input Event From HubHop</td><td>																** Requires WASimModule or MobiFlight. **
Trigger a Simulator Event from loaded HubHop data.					"Potentiometer" type events are only supported with WASimModule (using the provided value, which should evaluate to numeric).</td><td>Aircraft
Device:{0}System:{1}Event
Name:{2}Value
Range:{3}-{4}| Feedback From
| State (opt):{5}{6}
Range:{7}-{8}</td><td><ol start=0>
<li>[choice] &nbsp; <b></b>[plugin not connected]</li>
<li>[choice] &nbsp; <b></b>[select an aircraft]</li>
<li>[choice] &nbsp; <b></b>[select a system]</li>
<li>[text] &nbsp; <b>-16384</b> &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440.00&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440.00&gt;</sub></li>
<li>[text] &nbsp; <b>16384</b> &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440.00&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440.00&gt;</sub></li>
<li>[choice] &nbsp; <b></b>[connect plugin]</li>
<li>[choice] &nbsp; <b></b>[select a category]</li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
</ol></td>
<tr valign='top'><td>Set Simulator Variable (SimVar)</td><td>Sets the value of a Simulator Variable selected from a list of Sim Vars which are marked as settable.</td><td>From Category:{0}Set Variable:{1}Value
Range:{3}-{4}</td><td><ol start=0>
<li>[choice] &nbsp; <b></b>[connect to plugin]</li>
<li>[choice] &nbsp; <b></b>[select a category or type]</li>
<li>[switch] &nbsp; <b>False</b></li>
<li>[text] &nbsp; <b>-16384</b> &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440.00&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440.00&gt;</sub></li>
<li>[text] &nbsp; <b>16384</b> &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440.00&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440.00&gt;</sub></li>
</ol></td>
<tr valign='top'><td>Set Airplane Local Variable</td><td>Sets a value on a Local variable from currently loaded aircraft.					** Requires WASimModule **</td><td>Set Variable:{0}Units:
(opt){1}Value
Range{2}-{3}| Feedback From
| State (opt):{4}{5}
Range:{6}-{7}</td><td><ol start=0>
<li>[choice] &nbsp; <b></b>[connect to plugin]</li>
<li>[choice] &nbsp; <b></b>[connect to plugin]</li>
<li>[text] &nbsp; <b>-16384</b> &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440.00&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440.00&gt;</sub></li>
<li>[text] &nbsp; <b>16384</b> &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440.00&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440.00&gt;</sub></li>
<li>[choice] &nbsp; <b></b>[connect plugin]</li>
<li>[choice] &nbsp; <b></b>[select a category]</li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
</ol></td>
<tr valign='top'><td>Set Named Variable Value</td><td>Sets a value on any named variable of various types. SimVar types require a Unit specifier.					** Requires WASimModule **</td><td>Variable
Type:{0}Name:{1}Units
(opt):{2}Value
Range:{3}-{4}| Feedback From
| State (opt):{5}{6}
Range:{7}-{8}</td><td><ol start=0>
<li>[choice] &nbsp; <b>A: SimVar</b>, C: GPS, H: HTML Event, K: Key Event, L: Local, Z: Custom SimVar</li>
<li>[text] &nbsp; &lt;empty&gt;</li>
<li>[choice] &nbsp; <b></b>[connect to plugin]</li>
<li>[text] &nbsp; <b>-16384</b> &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440.00&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440.00&gt;</sub></li>
<li>[text] &nbsp; <b>16384</b> &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440.00&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440.00&gt;</sub></li>
<li>[choice] &nbsp; <b></b>[connect plugin]</li>
<li>[choice] &nbsp; <b></b>[select a category]</li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
</ol></td>
<tr valign='top'><td>Execute Calculator Code</td><td>Runs any entered string of RPN code through the 'execute_calculator_code' Gauge API function. Use an '@' placeholder for the connector value.			** Requires WASimModule **</td><td>Calculator Code:
(@ as placeholder(s) for value){0}Value
Range:{1}-{2}| Feedback From
| State (opt):{3}{4}
Range:{5}-{6}</td><td><ol start=0>
<li>[text] &nbsp; <b>@ 1 (>K:2:PANEL_LIGHTS_POWER_SETTING_SET)</b></li>
<li>[text] &nbsp; <b>-16384</b> &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440.00&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440.00&gt;</sub></li>
<li>[text] &nbsp; <b>16384</b> &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440.00&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440.00&gt;</sub></li>
<li>[choice] &nbsp; <b></b>[connect plugin]</li>
<li>[choice] &nbsp; <b></b>[select a category]</li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
</ol></td>
<tr valign='top'><td>Visual Feedback Connector</td><td>This connector provides only visual feedback by setting the position of a slider based on an existing State/variable value.</td><td>Set Connector Value Based on
Feedback From Category:{0}Variable:{1}Value Range:{2}-{3}</td><td><ol start=0>
<li>[choice] &nbsp; <b></b>[connect plugin]</li>
<li>[choice] &nbsp; <b></b>[select a category]</li>
<li>[text] &nbsp; <b>-16384</b> &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440.00&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440.00&gt;</sub></li>
<li>[text] &nbsp; <b>16384</b> &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440.00&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440.00&gt;</sub></li>
</ol></td>
</table>


#### States

 **Base Id:** MSFSTouchPortalPlugin.Plugin.State.

| Id | SimVar Name | Description | Unit | Format | DefaultValue | Settable |
| --- | --- | --- | --- | --- | --- | --- |
| ActionRepeatInterval |  | The current Held Action Repeat Rate (ms) | millisecond |  | 450|  |
| Connected |  | The status of SimConnect (true/false/connecting) | string |  | false|  |
| RunningVersion |  | The running plugin version number. | number |  | 0|  |
| EntryVersion |  | The loaded entry.tp plugin version number. | number |  | 0|  |
| LogMessages |  | Most recent plugin log messages. | string |  | |  |


</details>

---

### AutoPilot
<details><summary><sub>Click to expand</sub></summary>

#### Actions

<table>
<tr valign='bottom'><th>Name</th><th>Description</th><th>Format</th><th nowrap>Data<br/><div align=left><sub>index. &nbsp; [type] &nbsp; &nbsp; choices/default (in bold)</th><th>Sim Event(s)</th><th>On<br/>Hold</sub></div></th></tr>
<tr valign='top'><td>Airspeed Hold Value  Adj/Sel/Hold</td><td></td><td>Airspeed Hold Value - {0}</td><td><ol start=0>
<li>[choice] &nbsp; <b>Select</b>, Increase, Decrease, Hold Current</li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>Select</dt><dd>AIRSPEED_BUG_SELECT</dd><dt>Increase</dt><dd>AP_SPD_VAR_INC</dd><dt>Decrease</dt><dd>AP_SPD_VAR_DEC</dd><dt>Hold Current</dt><dd>AUTOPILOT_AIRSPEED_HOLD_CURRENT</dd></dl></details></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Airspeed Hold Value Set</td><td></td><td>Set Airspeed Hold Value to {0} kts</td><td><ol start=0>
<li>[text] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: 0.00&gt;</sub> <sub>&lt;max: 50000.00&gt;</sub></li>
</ol></td>
<td><dl><dd>AP_SPD_VAR_SET</dd></dl></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Altitude Hold Value Adj/Sel</td><td></td><td>Altitude Hold Value - {0}</td><td><ol start=0>
<li>[choice] &nbsp; <b>Select</b>, Increase, Decrease</li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>Select</dt><dd>ALTITUDE_BUG_SELECT</dd><dt>Increase</dt><dd>AP_ALT_VAR_INC</dd><dt>Decrease</dt><dd>AP_ALT_VAR_DEC</dd></dl></details></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Altitude Hold Value Set</td><td></td><td>Altitude Hold Value {0} to {1}</td><td><ol start=0>
<li>[choice] &nbsp; <b>Set English</b>, Set Metric</li>
<li>[text] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: -1000.00&gt;</sub> <sub>&lt;max: 150000.00&gt;</sub></li>
</ol></td>
<td><dl><dt>Set English</dt><dd>AP_ALT_VAR_SET_ENGLISH</dd><dt>Set Metric</dt><dd>AP_ALT_VAR_SET_METRIC</dd></dl></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>AP Switches</td><td></td><td>Auto Pilot {0} Switch {1}</td><td><ol start=0>
<li>[choice] &nbsp; <b>Master</b>, Airspeed Hold, Altitude Hold, Approach Mode, Attitude Hold, Back Course Mode, Heading Hold, Localizer, Mach Hold, Nav1 Hold, Vertical Speed, Wing Leveler, Yaw Dampener</li>
<li>[choice] &nbsp; <b>Toggle</b>, On, Off</li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>Master+Toggle</dt><dd>AP_MASTER</dd><dt>Master+On</dt><dd>AUTOPILOT_ON</dd><dt>Master+Off</dt><dd>AUTOPILOT_OFF</dd><dt>Airspeed Hold+Toggle</dt><dd>AP_AIRSPEED_HOLD</dd><dt>Airspeed Hold+On</dt><dd>AP_AIRSPEED_ON</dd><dt>Airspeed Hold+Off</dt><dd>AP_AIRSPEED_OFF</dd><dt>Altitude Hold+Toggle</dt><dd>AP_ALT_HOLD</dd><dt>Altitude Hold+On</dt><dd>AP_ALT_HOLD_ON</dd><dt>Altitude Hold+Off</dt><dd>AP_ALT_HOLD_OFF</dd><dt>Approach Mode+Toggle</dt><dd>AP_APR_HOLD</dd><dt>Approach Mode+On</dt><dd>AP_APR_HOLD_ON</dd><dt>Approach Mode+Off</dt><dd>AP_APR_HOLD_OFF</dd><dt>Attitude Hold+Toggle</dt><dd>AP_ATT_HOLD</dd><dt>Attitude Hold+On</dt><dd>AP_ATT_HOLD_ON</dd><dt>Attitude Hold+Off</dt><dd>AP_ATT_HOLD_OFF</dd><dt>Back Course Mode+Toggle</dt><dd>AP_BC_HOLD</dd><dt>Back Course Mode+On</dt><dd>AP_BC_HOLD_ON</dd><dt>Back Course Mode+Off</dt><dd>AP_BC_HOLD_OFF</dd><dt>Heading Hold+Toggle</dt><dd>AP_HDG_HOLD</dd><dt>Heading Hold+On</dt><dd>AP_HDG_HOLD_ON</dd><dt>Heading Hold+Off</dt><dd>AP_HDG_HOLD_OFF</dd><dt>Localizer+Toggle</dt><dd>AP_LOC_HOLD</dd><dt>Localizer+On</dt><dd>AP_LOC_HOLD_ON</dd><dt>Localizer+Off</dt><dd>AP_LOC_HOLD_OFF</dd><dt>Mach Hold+Toggle</dt><dd>AP_MACH_HOLD</dd><dt>Mach Hold+On</dt><dd>AP_MACH_ON</dd><dt>Mach Hold+Off</dt><dd>AP_MACH_OFF</dd><dt>Nav1 Hold+Toggle</dt><dd>AP_NAV1_HOLD</dd><dt>Nav1 Hold+On</dt><dd>AP_NAV1_HOLD_ON</dd><dt>Nav1 Hold+Off</dt><dd>AP_NAV1_HOLD_OFF</dd><dt>Vertical Speed+Toggle</dt><dd>AP_VS_HOLD</dd><dt>Vertical Speed+On</dt><dd>AP_VS_ON</dd><dt>Vertical Speed+Off</dt><dd>AP_VS_OFF</dd><dt>Wing Leveler+Toggle</dt><dd>AP_WING_LEVELER</dd><dt>Wing Leveler+On</dt><dd>AP_WING_LEVELER_ON</dd><dt>Wing Leveler+Off</dt><dd>AP_WING_LEVELER_OFF</dd><dt>Yaw Dampener+Toggle</dt><dd>YAW_DAMPER_TOGGLE</dd><dt>Yaw Dampener+On</dt><dd>YAW_DAMPER_ON</dd><dt>Yaw Dampener+Off</dt><dd>YAW_DAMPER_OFF</dd></dl></details></td>
<td align='center'></td></tr>
<tr valign='top'><td>Attitude Hold Pitch Value Adj/Sel</td><td></td><td>Attitude Hold Pitch Value {0}</td><td><ol start=0>
<li>[choice] &nbsp; <b>Select</b>, Increase, Decrease</li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>Select</dt><dd>AP_PITCH_REF_SELECT</dd><dt>Increase</dt><dd>AP_PITCH_REF_INC_UP</dd><dt>Decrease</dt><dd>AP_PITCH_REF_INC_DN</dd></dl></details></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Attitude Hold Pitch Value Set</td><td></td><td>Set Attitude Hold Pitch Value to {0} (-16384 to +16384)</td><td><ol start=0>
<li>[text] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: -16384&gt;</sub> <sub>&lt;max: 16384&gt;</sub></li>
</ol></td>
<td><dl><dd>AP_PITCH_REF_SET</dd></dl></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Auto Brake Adjust</td><td></td><td>Auto Brake - {0}</td><td><ol start=0>
<li>[choice] &nbsp; <b>Increase</b>, Decrease</li>
</ol></td>
<td><dl><dt>Increase</dt><dd>INCREASE_AUTOBRAKE_CONTROL</dd><dt>Decrease</dt><dd>DECREASE_AUTOBRAKE_CONTROL</dd></dl></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Auto Throttle Mode Switch</td><td></td><td>Toggle Auto Throttle - {0}</td><td><ol start=0>
<li>[choice] &nbsp; <b>Arm</b>, GoAround</li>
</ol></td>
<td><dl><dt>Arm</dt><dd>AUTO_THROTTLE_ARM</dd><dt>GoAround</dt><dd>AUTO_THROTTLE_TO_GA</dd></dl></td>
<td align='center'></td></tr>
<tr valign='top'><td>Flight Director Switches</td><td></td><td>Toggle Flight Director {0} Switch On/Off</td><td><ol start=0>
</ol></td>
<td><dl><dt>Master</dt><dd>TOGGLE_FLIGHT_DIRECTOR</dd><dt>Pitch Sync</dt><dd>SYNC_FLIGHT_DIRECTOR_PITCH</dd></dl></td>
<td align='center'></td></tr>
<tr valign='top'><td>Heading Hold Value Adj/Sel</td><td></td><td>Heading Hold Value - {0}</td><td><ol start=0>
<li>[choice] &nbsp; <b>Select</b>, Increase, Decrease</li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>Select</dt><dd>HEADING_BUG_SELECT</dd><dt>Increase</dt><dd>HEADING_BUG_INC</dd><dt>Decrease</dt><dd>HEADING_BUG_DEC</dd></dl></details></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Heading Hold Value Set</td><td></td><td>Set Heading Hold Value - {0}</td><td><ol start=0>
<li>[text] &nbsp; <b>1</b> &nbsp; <sub>&lt;min: 0.00&gt;</sub> <sub>&lt;max: 359.00&gt;</sub></li>
</ol></td>
<td><dl><dd>HEADING_BUG_SET</dd></dl></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Mach Hold Value  Adj/Hold</td><td></td><td>Mach Hold Value - {0}</td><td><ol start=0>
<li>[choice] &nbsp; <b>Increase</b>, Decrease, Hold Current</li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>Increase</dt><dd>AP_MACH_VAR_INC</dd><dt>Decrease</dt><dd>AP_MACH_VAR_DEC</dd><dt>Hold Current</dt><dd>AUTOPILOT_MACH_HOLD_CURRENT</dd></dl></details></td>
<td align='center'></td></tr>
<tr valign='top'><td>Mach Hold Value Set</td><td></td><td>Set Mach Hold Value to {0}</td><td><ol start=0>
<li>[text] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: 0.00&gt;</sub> <sub>&lt;max: 20.00&gt;</sub></li>
</ol></td>
<td><dl><dd>AP_MACH_VAR_SET</dd></dl></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Max Bank Angle Adjust</td><td></td><td>Max Bank Angle - {0}</td><td><ol start=0>
<li>[choice] &nbsp; <b>Increase</b>, Decrease</li>
</ol></td>
<td><dl><dt>Increase</dt><dd>AP_MAX_BANK_INC</dd><dt>Decrease</dt><dd>AP_MAX_BANK_DEC</dd></dl></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Nav Mode Select</td><td></td><td>Select Nav Mode {0}</td><td><ol start=0>
<li>[choice] &nbsp; <b>1</b>, 2</li>
</ol></td>
<td><dl><dt>1</dt><dd>AP_NAV_SELECT_SET</dd><dt>2</dt><dd>AP_NAV_SELECT_SET</dd></dl></td>
<td align='center'></td></tr>
<tr valign='top'><td>Vertical Speed Value Adj/Sel/Hold</td><td></td><td>Vertical Speed Value - {0}</td><td><ol start=0>
<li>[choice] &nbsp; <b>Select</b>, Increase, Decrease, Set Current</li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>Select</dt><dd>VSI_BUG_SELECT</dd><dt>Increase</dt><dd>AP_VS_VAR_INC</dd><dt>Decrease</dt><dd>AP_VS_VAR_DEC</dd><dt>Hold Current</dt><dd>AP_VS_VAR_SET_CURRENT</dd></dl></details></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Vertical Speed Value Set</td><td></td><td>Vertical Speed Hold {0} to Value {1}</td><td><ol start=0>
<li>[choice] &nbsp; <b>Set English</b>, Set Metric</li>
<li>[text] &nbsp; <b>1</b> &nbsp; <sub>&lt;min: -5000.00&gt;</sub> <sub>&lt;max: 5000.00&gt;</sub></li>
</ol></td>
<td><dl><dt>Set English</dt><dd>AP_VS_VAR_SET_ENGLISH</dd><dt>Set Metric</dt><dd>AP_VS_VAR_SET_METRIC</dd></dl></td>
<td align='center'>&#9745;</td></tr>
</table>


#### Connectors

<table>
<tr valign='bottom'><th>Name</th><th>Description</th><th>Format</th><th nowrap>Data<br/><div align=left><sub>index. &nbsp; [type] &nbsp; &nbsp; choices/default (in bold)</th></tr>
<tr valign='top'><td>Airspeed Hold Value Set</td><td></td><td>Set Airspeed Hold
in Value Range (kts):{0}-{1}| Feedback From
| State (opt):{2}{3}
Range:{4}-{5}</td><td><ol start=0>
<li>[text] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: 0.00&gt;</sub> <sub>&lt;max: 50000.00&gt;</sub></li>
<li>[text] &nbsp; <b>50000</b> &nbsp; <sub>&lt;min: 0.00&gt;</sub> <sub>&lt;max: 50000.00&gt;</sub></li>
<li>[choice] &nbsp; <b></b>[connect plugin]</li>
<li>[choice] &nbsp; <b></b>[select a category]</li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
</ol></td>
<tr valign='top'><td>Attitude Hold Pitch Value Set</td><td></td><td>Set Attitude Pitch Hold
in Value Range:{0}-{1}| Feedback From
| State (opt):{2}{3}
Range:{4}-{5}</td><td><ol start=0>
<li>[text] &nbsp; <b>-16384</b> &nbsp; <sub>&lt;min: -16384&gt;</sub> <sub>&lt;max: 16384&gt;</sub></li>
<li>[text] &nbsp; <b>16384</b> &nbsp; <sub>&lt;min: -16384&gt;</sub> <sub>&lt;max: 16384&gt;</sub></li>
<li>[choice] &nbsp; <b></b>[connect plugin]</li>
<li>[choice] &nbsp; <b></b>[select a category]</li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
</ol></td>
<tr valign='top'><td>Heading Hold Value Set</td><td></td><td>Set Heading Hold
in Value Range:{0}-{1}| Feedback From
| State (opt):{2}{3}
Range:{4}-{5}</td><td><ol start=0>
<li>[text] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: 0.00&gt;</sub> <sub>&lt;max: 359.00&gt;</sub></li>
<li>[text] &nbsp; <b>359</b> &nbsp; <sub>&lt;min: 0.00&gt;</sub> <sub>&lt;max: 359.00&gt;</sub></li>
<li>[choice] &nbsp; <b></b>[connect plugin]</li>
<li>[choice] &nbsp; <b></b>[select a category]</li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
</ol></td>
<tr valign='top'><td>Mach Hold Value Set</td><td></td><td>Set Mach Hold
in Value Range:{0}-{1}| Feedback From
| State (opt):{2}{3}
Range:{4}-{5}</td><td><ol start=0>
<li>[text] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: 0.00&gt;</sub> <sub>&lt;max: 20.00&gt;</sub></li>
<li>[text] &nbsp; <b>20</b> &nbsp; <sub>&lt;min: 0.00&gt;</sub> <sub>&lt;max: 20.00&gt;</sub></li>
<li>[choice] &nbsp; <b></b>[connect plugin]</li>
<li>[choice] &nbsp; <b></b>[select a category]</li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
</ol></td>
<tr valign='top'><td>Vertical Speed Value Set</td><td></td><td>Vertical Speed Hold {0}in Value
Range:{1}-{2}| Feedback From
| State (opt):{3}{4}
Range:{5}-{6}</td><td><ol start=0>
<li>[choice] &nbsp; <b>Set English</b>, Set Metric</li>
<li>[text] &nbsp; <b>-5000</b> &nbsp; <sub>&lt;min: -5000.00&gt;</sub> <sub>&lt;max: 5000.00&gt;</sub></li>
<li>[text] &nbsp; <b>5000</b> &nbsp; <sub>&lt;min: -5000.00&gt;</sub> <sub>&lt;max: 5000.00&gt;</sub></li>
<li>[choice] &nbsp; <b></b>[connect plugin]</li>
<li>[choice] &nbsp; <b></b>[select a category]</li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
</ol></td>
</table>


#### States

 **Base Id:** MSFSTouchPortalPlugin.AutoPilot.State.

| Id | SimVar Name | Description | Unit | Format | DefaultValue | Settable |
| --- | --- | --- | --- | --- | --- | --- |
| AutoThrottleArm | AUTOPILOT THROTTLE ARM | Auto Throttle Armed | Bool |  | |  |
| AutoThrottleGoAround | AUTOPILOT TAKEOFF POWER ACTIVE | Auto Throttle GoAround | Bool |  | |  |
| AutoPilotAirSpeedHold | AUTOPILOT AIRSPEED HOLD | AutoPilot Air Speed Status | Bool |  | |  |
| AutoPilotAirSpeedVar | AUTOPILOT AIRSPEED HOLD VAR | AutoPilot Air Speed Value | knots | 0.0# | |  |
| AutoPilotAltitudeHold | AUTOPILOT ALTITUDE LOCK | AutoPilot Altitude Status | Bool |  | |  |
| AutoPilotAltitudeVar | AUTOPILOT ALTITUDE LOCK VAR | AutoPilot Altitude Value | feet |  | | &#9745; |
| AutoPilotApproachHold | AUTOPILOT APPROACH HOLD | AutoPilot Approach Status | Bool |  | |  |
| AutoPilotAttitudeHold | AUTOPILOT ATTITUDE HOLD | AutoPilot Attitude Status | Bool |  | |  |
| AutoPilotAvailable | AUTOPILOT AVAILABLE | AutoPilot Availability | Bool |  | |  |
| AutoPilotBackCourseHold | AUTOPILOT BACKCOURSE HOLD | AutoPilot Back Course Status | Bool |  | |  |
| AutoPilotFlightDirector | AUTOPILOT FLIGHT DIRECTOR ACTIVE | AutoPilot Flight Director Status | Bool |  | |  |
| AutoPilotHeadingVar | AUTOPILOT HEADING LOCK DIR | AutoPilot Heading Direction | degrees | F0 | | &#9745; |
| AutoPilotHeadingHold | AUTOPILOT HEADING LOCK | AutoPilot Heading Status | Bool |  | |  |
| AutoPilotMach | AUTOPILOT MACH HOLD | AutoPilot Mach Hold | Bool |  | |  |
| AutoPilotMachVar | AUTOPILOT MACH HOLD VAR | AutoPilot Mach Value | number |  | |  |
| AutoPilotMaster | AUTOPILOT MASTER | AutoPilot Master Status | Bool |  | |  |
| AutoPilotBanking | AUTOPILOT MAX BANK | AutoPilot Max Bank Angle | degrees | F2 | |  |
| AutoPilotNavSelected | AUTOPILOT NAV SELECTED | AutoPilot Nav Selected Index | number |  | |  |
| AutoPilotNav1Hold | AUTOPILOT NAV1 LOCK | AutoPilot Nav1 Status | Bool |  | |  |
| AutoPilotAttitudeVar | AUTOPILOT PITCH HOLD REF | AutoPilot Pitch Reference Value | degrees | F2 | |  |
| AutoPilotVerticalSpeedHold | AUTOPILOT VERTICAL HOLD | AutoPilot Vertical Speed Status | Bool |  | |  |
| AutoPilotVerticalSpeedVar | AUTOPILOT VERTICAL HOLD VAR | AutoPilot Vertical Speed Value | feet/minute |  | | &#9745; |
| AutoPilotWingLeveler | AUTOPILOT WING LEVELER | AutoPilot Wing Leveler | Bool |  | |  |
| AutoPilotFlightDirectorCurrentBank | AUTOPILOT FLIGHT DIRECTOR BANK | Flight Director Current Bank | degrees | F2 | |  |
| AutoPilotFlightDirectorCurrentPitch | AUTOPILOT FLIGHT DIRECTOR PITCH | Flight Director Current Pitch | degrees | F2 | |  |
| AutoPilotPitchHold | AUTOPILOT PITCH HOLD | The status of Auto Pilot Pitch Hold button | Bool |  | |  |
| AutoPilotYawDampener | AUTOPILOT YAW DAMPER | Yaw Dampener Status | Bool |  | |  |


</details>

---

### Communication
<details><summary><sub>Click to expand</sub></summary>

#### Actions

<table>
<tr valign='bottom'><th>Name</th><th>Description</th><th>Format</th><th nowrap>Data<br/><div align=left><sub>index. &nbsp; [type] &nbsp; &nbsp; choices/default (in bold)</th><th>Sim Event(s)</th><th>On<br/>Hold</sub></div></th></tr>
<tr valign='top'><td>Radio Interaction</td><td></td><td>Radio {0} - {1}</td><td><ol start=0>
<li>[choice] &nbsp; <b>COM1</b>, COM2, COM3, NAV1, NAV2, NAV3, NAV4</li>
<li>[choice] &nbsp; <b>Standby Swap</b>, Decrease 1Mhz, Increase 1 MHz, Decrease 25 KHz, Decrease 25 KHz w/ Carry Digits, Increase 25 KHz, Increase 25 KHz w/ Carry Digits, Volume Increase, Volume Decrease, Transmit Select, Receive Select, Toggle Spacing Mode (COM only), Autoswitch Toggle</li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>COM1+Standby Swap</dt><dd>COM1_RADIO_SWAP</dd><dt>COM1+Decrease 1Mhz</dt><dd>COM_RADIO_WHOLE_DEC</dd><dt>COM1+Increase 1 MHz</dt><dd>COM_RADIO_WHOLE_INC</dd><dt>COM1+Decrease 25 KHz</dt><dd>COM_RADIO_FRACT_DEC</dd><dt>COM1+Decrease 25 KHz w/ Carry Digits</dt><dd>COM_RADIO_FRACT_DEC_CARRY</dd><dt>COM1+Increase 25 KHz</dt><dd>COM_RADIO_FRACT_INC</dd><dt>COM1+Increase 25 KHz w/ Carry Digits</dt><dd>COM_RADIO_FRACT_INC_CARRY</dd><dt>COM1+Volume Increase</dt><dd>COM1_VOLUME_INC</dd><dt>COM1+Volume Decrease</dt><dd>COM1_VOLUME_DEC</dd><dt>COM1+Transmit Select</dt><dd>COM1_TRANSMIT_SELECT</dd><dt>COM1+Receive Select</dt><dd>COM1_RECEIVE_SELECT</dd><dt>COM1+Toggle Spacing Mode (COM only)</dt><dd>COM_1_SPACING_MODE_SWITCH</dd><dt>COM1+Autoswitch Toggle</dt><dd>RADIO_COMM1_AUTOSWITCH_TOGGLE</dd><dt>COM2+Standby Swap</dt><dd>COM2_RADIO_SWAP</dd><dt>COM2+Decrease 1Mhz</dt><dd>COM2_RADIO_WHOLE_DEC</dd><dt>COM2+Increase 1 MHz</dt><dd>COM2_RADIO_WHOLE_INC</dd><dt>COM2+Decrease 25 KHz</dt><dd>COM2_RADIO_FRACT_DEC</dd><dt>COM2+Decrease 25 KHz w/ Carry Digits</dt><dd>COM2_RADIO_FRACT_DEC_CARRY</dd><dt>COM2+Increase 25 KHz</dt><dd>COM2_RADIO_FRACT_INC</dd><dt>COM2+Increase 25 KHz w/ Carry Digits</dt><dd>COM2_RADIO_FRACT_INC_CARRY</dd><dt>COM2+Volume Increase</dt><dd>COM2_VOLUME_INC</dd><dt>COM2+Volume Decrease</dt><dd>COM2_VOLUME_DEC</dd><dt>COM2+Transmit Select</dt><dd>COM2_TRANSMIT_SELECT</dd><dt>COM2+Receive Select</dt><dd>COM2_RECEIVE_SELECT</dd><dt>COM2+Toggle Spacing Mode (COM only)</dt><dd>COM_2_SPACING_MODE_SWITCH</dd><dt>COM2+Autoswitch Toggle</dt><dd>RADIO_COMM2_AUTOSWITCH_TOGGLE</dd><dt>COM3+Standby Swap</dt><dd>COM3_RADIO_SWAP</dd><dt>COM3+Decrease 1Mhz</dt><dd>COM3_RADIO_WHOLE_DEC</dd><dt>COM3+Increase 1 MHz</dt><dd>COM3_RADIO_WHOLE_INC</dd><dt>COM3+Decrease 25 KHz</dt><dd>COM3_RADIO_FRACT_DEC</dd><dt>COM3+Decrease 25 KHz w/ Carry Digits</dt><dd>COM3RADIO_FRACT_DEC_CARRY</dd><dt>COM3+Increase 25 KHz</dt><dd>COM3_RADIO_FRACT_INC</dd><dt>COM3+Increase 25 KHz w/ Carry Digits</dt><dd>COM3_RADIO_FRACT_INC_CARRY</dd><dt>COM3+Volume Increase</dt><dd>COM3_VOLUME_INC</dd><dt>COM3+Volume Decrease</dt><dd>COM3_VOLUME_DEC</dd><dt>COM3+Transmit Select</dt><dd>COM3_TRANSMIT_SELECT</dd><dt>COM3+Receive Select</dt><dd>COM3_RECEIVE_SELECT</dd><dt>COM3+Toggle Spacing Mode (COM only)</dt><dd>COM_3_SPACING_MODE_SWITCH</dd><dt>NAV1+Standby Swap</dt><dd>NAV1_RADIO_SWAP</dd><dt>NAV1+Decrease 1Mhz</dt><dd>NAV1_RADIO_WHOLE_DEC</dd><dt>NAV1+Increase 1 MHz</dt><dd>NAV1_RADIO_WHOLE_INC</dd><dt>NAV1+Decrease 25 KHz</dt><dd>NAV1_RADIO_FRACT_DEC</dd><dt>NAV1+Decrease 25 KHz w/ Carry Digits</dt><dd>NAV1_RADIO_FRACT_DEC_CARRY</dd><dt>NAV1+Increase 25 KHz</dt><dd>NAV1_RADIO_FRACT_INC</dd><dt>NAV1+Increase 25 KHz w/ Carry Digits</dt><dd>NAV1_RADIO_FRACT_INC_CARRY</dd><dt>NAV1+Volume Increase</dt><dd>NAV1_VOLUME_INC</dd><dt>NAV1+Volume Decrease</dt><dd>NAV1_VOLUME_DEC</dd><dt>NAV1+Autoswitch Toggle</dt><dd>RADIO_NAV1_AUTOSWITCH_TOGGLE</dd><dt>NAV2+Standby Swap</dt><dd>NAV2_RADIO_SWAP</dd><dt>NAV2+Decrease 1Mhz</dt><dd>NAV2_RADIO_WHOLE_DEC</dd><dt>NAV2+Increase 1 MHz</dt><dd>NAV2_RADIO_WHOLE_INC</dd><dt>NAV2+Decrease 25 KHz</dt><dd>NAV2_RADIO_FRACT_DEC</dd><dt>NAV2+Decrease 25 KHz w/ Carry Digits</dt><dd>NAV2_RADIO_FRACT_DEC_CARRY</dd><dt>NAV2+Increase 25 KHz</dt><dd>NAV2_RADIO_FRACT_INC</dd><dt>NAV2+Increase 25 KHz w/ Carry Digits</dt><dd>NAV2_RADIO_FRACT_INC_CARRY</dd><dt>NAV2+Volume Increase</dt><dd>NAV2_VOLUME_INC</dd><dt>NAV2+Volume Decrease</dt><dd>NAV2_VOLUME_DEC</dd><dt>NAV2+Autoswitch Toggle</dt><dd>RADIO_NAV2_AUTOSWITCH_TOGGLE</dd><dt>NAV3+Standby Swap</dt><dd>NAV3_RADIO_SWAP</dd><dt>NAV3+Decrease 1Mhz</dt><dd>NAV3_RADIO_WHOLE_DEC</dd><dt>NAV3+Increase 1 MHz</dt><dd>NAV3_RADIO_WHOLE_INC</dd><dt>NAV3+Decrease 25 KHz</dt><dd>NAV3_RADIO_FRACT_DEC</dd><dt>NAV3+Decrease 25 KHz w/ Carry Digits</dt><dd>NAV3_RADIO_FRACT_DEC_CARRY</dd><dt>NAV3+Increase 25 KHz</dt><dd>NAV3_RADIO_FRACT_INC</dd><dt>NAV3+Increase 25 KHz w/ Carry Digits</dt><dd>NAV3_RADIO_FRACT_INC_CARRY</dd><dt>NAV3+Volume Increase</dt><dd>NAV3_VOLUME_INC</dd><dt>NAV3+Volume Decrease</dt><dd>NAV3_VOLUME_DEC</dd><dt>NAV4+Standby Swap</dt><dd>NAV4_RADIO_SWAP</dd><dt>NAV4+Decrease 1Mhz</dt><dd>NAV4_RADIO_WHOLE_DEC</dd><dt>NAV4+Increase 1 MHz</dt><dd>NAV4_RADIO_WHOLE_INC</dd><dt>NAV4+Decrease 25 KHz</dt><dd>NAV4_RADIO_FRACT_DEC</dd><dt>NAV4+Decrease 25 KHz w/ Carry Digits</dt><dd>NAV4_RADIO_FRACT_DEC_CARRY</dd><dt>NAV4+Increase 25 KHz</dt><dd>NAV4_RADIO_FRACT_INC</dd><dt>NAV4+Increase 25 KHz w/ Carry Digits</dt><dd>NAV4_RADIO_FRACT_INC_CARRY</dd><dt>NAV4+Volume Increase</dt><dd>NAV4_VOLUME_INC</dd><dt>NAV4+Volume Decrease</dt><dd>NAV4_VOLUME_DEC</dd></dl></details></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Radio Values Set</td><td></td><td>Set Radio {0} {1} to Value {2}</td><td><ol start=0>
<li>[choice] &nbsp; <b>COM1</b>, COM2, COM3, NAV1, NAV2, NAV3, NAV4</li>
<li>[choice] &nbsp; <b>Frequency (Hz)</b>, Frequency (BCD16), Standby Frequency (Hz), Standby Frequency (BCD16), Stored Frequency (Hz) (COM only), Stored Frequency (BCD16) (COM only), Volume (0.0-1.0)</li>
<li>[text] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440.00&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440.00&gt;</sub></li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>COM1+Frequency (Hz)</dt><dd>COM_RADIO_SET_HZ</dd><dt>COM1+Frequency (BCD16)</dt><dd>COM_RADIO_SET</dd><dt>COM1+Standby Frequency (Hz)</dt><dd>COM_STBY_RADIO_SET_HZ</dd><dt>COM1+Standby Frequency (BCD16)</dt><dd>COM_STBY_RADIO_SET</dd><dt>COM1+Stored Frequency (Hz) (COM only)</dt><dd>COM1_STORED_FREQUENCY_SET_HZ</dd><dt>COM1+Stored Frequency (BCD16) (COM only)</dt><dd>COM1_STORED_FREQUENCY_SET</dd><dt>COM1+Volume (0.0-1.0)</dt><dd>COM1_VOLUME_SET</dd><dt>COM2+Frequency (Hz)</dt><dd>COM2_RADIO_SET_HZ</dd><dt>COM2+Frequency (BCD16)</dt><dd>COM2_RADIO_SET</dd><dt>COM2+Standby Frequency (Hz)</dt><dd>COM2_STBY_RADIO_SET_HZ</dd><dt>COM2+Standby Frequency (BCD16)</dt><dd>COM2_STBY_RADIO_SET</dd><dt>COM2+Stored Frequency (Hz) (COM only)</dt><dd>COM2_STORED_FREQUENCY_SET_HZ</dd><dt>COM2+Stored Frequency (BCD16) (COM only)</dt><dd>COM2_STORED_FREQUENCY_SET</dd><dt>COM2+Volume (0.0-1.0)</dt><dd>COM2_VOLUME_SET</dd><dt>COM3+Frequency (Hz)</dt><dd>COM3_RADIO_SET_HZ</dd><dt>COM3+Frequency (BCD16)</dt><dd>COM3_RADIO_SET</dd><dt>COM3+Standby Frequency (Hz)</dt><dd>COM3_STBY_RADIO_SET_HZ</dd><dt>COM3+Standby Frequency (BCD16)</dt><dd>COM3_STBY_RADIO_SET</dd><dt>COM3+Stored Frequency (Hz) (COM only)</dt><dd>COM3_STORED_FREQUENCY_SET_HZ</dd><dt>COM3+Stored Frequency (BCD16) (COM only)</dt><dd>COM3_STORED_FREQUENCY_SET</dd><dt>COM3+Volume (0.0-1.0)</dt><dd>COM3_VOLUME_SET</dd><dt>NAV1+Frequency (Hz)</dt><dd>NAV1_RADIO_SET_HZ</dd><dt>NAV1+Frequency (BCD16)</dt><dd>NAV1_RADIO_SET</dd><dt>NAV1+Standby Frequency (Hz)</dt><dd>NAV1_STBY_SET_HZ</dd><dt>NAV1+Standby Frequency (BCD16)</dt><dd>NAV1_STBY_SET</dd><dt>NAV1+Volume (0.0-1.0)</dt><dd>NAV1_VOLUME_SET</dd><dt>NAV2+Frequency (Hz)</dt><dd>NAV2_RADIO_SET_HZ</dd><dt>NAV2+Frequency (BCD16)</dt><dd>NAV2_RADIO_SET</dd><dt>NAV2+Standby Frequency (Hz)</dt><dd>NAV2_STBY_SET_HZ</dd><dt>NAV2+Standby Frequency (BCD16)</dt><dd>NAV2_STBY_SET</dd><dt>NAV2+Volume (0.0-1.0)</dt><dd>NAV2_VOLUME_SET</dd><dt>NAV3+Frequency (Hz)</dt><dd>NAV3_RADIO_SET_HZ</dd><dt>NAV3+Frequency (BCD16)</dt><dd>NAV3_RADIO_SET</dd><dt>NAV3+Standby Frequency (Hz)</dt><dd>NAV3_STBY_SET_HZ</dd><dt>NAV3+Standby Frequency (BCD16)</dt><dd>NAV3_STBY_SET</dd><dt>NAV3+Volume (0.0-1.0)</dt><dd>NAV3_VOLUME_SET</dd><dt>NAV4+Frequency (Hz)</dt><dd>NAV4_RADIO_SET_HZ</dd><dt>NAV4+Frequency (BCD16)</dt><dd>NAV4_RADIO_SET</dd><dt>NAV4+Standby Frequency (Hz)</dt><dd>NAV4_STBY_SET_HZ</dd><dt>NAV4+Standby Frequency (BCD16)</dt><dd>NAV4_STBY_SET</dd><dt>NAV4+Volume (0.0-1.0)</dt><dd>NAV4_VOLUME_SET</dd></dl></details></td>
<td align='center'>&#9745;</td></tr>
</table>


#### Connectors

<table>
<tr valign='bottom'><th>Name</th><th>Description</th><th>Format</th><th nowrap>Data<br/><div align=left><sub>index. &nbsp; [type] &nbsp; &nbsp; choices/default (in bold)</th></tr>
<tr valign='top'><td>Radio Values Set</td><td></td><td>Set Radio {0} {1}in Value
Range:{2}-{3}| Feedback From
| State (opt):{4}{5}
Range:{6}-{7}</td><td><ol start=0>
<li>[choice] &nbsp; <b>COM1</b>, COM2, COM3, NAV1, NAV2, NAV3, NAV4</li>
<li>[choice] &nbsp; <b>Frequency (Hz)</b>, Frequency (BCD16), Standby Frequency (Hz), Standby Frequency (BCD16), Stored Frequency (Hz) (COM only), Stored Frequency (BCD16) (COM only), Volume (0.0-1.0)</li>
<li>[text] &nbsp; <b>-3.4028234663852886E+38</b> &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440.00&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440.00&gt;</sub></li>
<li>[text] &nbsp; <b>3.4028234663852886E+38</b> &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440.00&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440.00&gt;</sub></li>
<li>[choice] &nbsp; <b></b>[connect plugin]</li>
<li>[choice] &nbsp; <b></b>[select a category]</li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
</ol></td>
</table>


#### States

 **Base Id:** MSFSTouchPortalPlugin.Communication.State.

| Id | SimVar Name | Description | Unit | Format | DefaultValue | Settable |
| --- | --- | --- | --- | --- | --- | --- |
| Com1ActiveFrequency | COM ACTIVE FREQUENCY:1 | The frequency of the active COM1 radio | MHz | 0.000# | |  |
| Com2ActiveFrequency | COM ACTIVE FREQUENCY:2 | The frequency of the active COM2 radio | MHz | 0.000# | |  |
| Nav1ActiveFrequency | NAV ACTIVE FREQUENCY:1 | The frequency of the active NAV1 radio | MHz | 0.000# | |  |
| Nav2ActiveFrequency | NAV ACTIVE FREQUENCY:2 | The frequency of the active NAV2 radio | MHz | 0.000# | |  |
| Com1StandbyFrequency | COM STANDBY FREQUENCY:1 | The frequency of the standby COM1 radio | MHz | 0.000# | |  |
| Com2StandbyFrequency | COM STANDBY FREQUENCY:2 | The frequency of the standby COM2 radio | MHz | 0.000# | |  |
| Nav1StandbyFrequency | NAV STANDBY FREQUENCY:1 | The frequency of the standby NAV1 radio | MHz | 0.000# | |  |
| Nav2StandbyFrequency | NAV STANDBY FREQUENCY:2 | The frequency of the standby NAV2 radio | MHz | 0.000# | |  |


</details>

---

### Electrical
<details><summary><sub>Click to expand</sub></summary>

#### Actions

<table>
<tr valign='bottom'><th>Name</th><th>Description</th><th>Format</th><th nowrap>Data<br/><div align=left><sub>index. &nbsp; [type] &nbsp; &nbsp; choices/default (in bold)</th><th>Sim Event(s)</th><th>On<br/>Hold</sub></div></th></tr>
<tr valign='top'><td>Alternator Switches</td><td></td><td>Toggle Alternator - {0}</td><td><ol start=0>
<li>[choice] &nbsp; <b>Master</b>, 1, 2, 3, 4</li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>Master</dt><dd>TOGGLE_MASTER_ALTERNATOR</dd><dt>1</dt><dd>TOGGLE_ALTERNATOR1</dd><dt>2</dt><dd>TOGGLE_ALTERNATOR2</dd><dt>3</dt><dd>TOGGLE_ALTERNATOR3</dd><dt>4</dt><dd>TOGGLE_ALTERNATOR4</dd></dl></details></td>
<td align='center'></td></tr>
<tr valign='top'><td>Avionics Master Switch</td><td></td><td>Toggle Avionics Master</td><td><ol start=0>
</ol></td>
<td><dl><dd>TOGGLE_AVIONICS_MASTER</dd></dl></td>
<td align='center'></td></tr>
<tr valign='top'><td>Landing Lights Switch/Direction</td><td></td><td>Landing Lights - {0}</td><td><ol start=0>
<li>[choice] &nbsp; <b>Toggle</b>, On, Off, Left, Right, Up, Down, Home</li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>Toggle</dt><dd>LANDING_LIGHTS_TOGGLE</dd><dt>On</dt><dd>LANDING_LIGHTS_ON</dd><dt>Off</dt><dd>LANDING_LIGHTS_OFF</dd><dt>Left</dt><dd>LANDING_LIGHT_LEFT</dd><dt>Right</dt><dd>LANDING_LIGHT_RIGHT</dd><dt>Up</dt><dd>LANDING_LIGHT_UP</dd><dt>Down</dt><dd>LANDING_LIGHT_DOWN</dd><dt>Home</dt><dd>LANDING_LIGHT_HOME</dd></dl></details></td>
<td align='center'></td></tr>
<tr valign='top'><td>Light Dimming</td><td></td><td>Set Light Potentiometer {0} to {0} (0 to 100)</td><td><ol start=0>
<li>[choice] &nbsp; <b>0</b>, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30</li>
<li>[text] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: 0&gt;</sub> <sub>&lt;max: 100&gt;</sub></li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>0</dt><dd>LIGHT_POTENTIOMETER_SET</dd><dt>1</dt><dd>LIGHT_POTENTIOMETER_1_SET</dd><dt>2</dt><dd>LIGHT_POTENTIOMETER_2_SET</dd><dt>3</dt><dd>LIGHT_POTENTIOMETER_3_SET</dd><dt>4</dt><dd>LIGHT_POTENTIOMETER_4_SET</dd><dt>5</dt><dd>LIGHT_POTENTIOMETER_5_SET</dd><dt>6</dt><dd>LIGHT_POTENTIOMETER_6_SET</dd><dt>7</dt><dd>LIGHT_POTENTIOMETER_7_SET</dd><dt>8</dt><dd>LIGHT_POTENTIOMETER_8_SET</dd><dt>9</dt><dd>LIGHT_POTENTIOMETER_9_SET</dd><dt>10</dt><dd>LIGHT_POTENTIOMETER_10_SET</dd><dt>11</dt><dd>LIGHT_POTENTIOMETER_11_SET</dd><dt>12</dt><dd>LIGHT_POTENTIOMETER_12_SET</dd><dt>13</dt><dd>LIGHT_POTENTIOMETER_13_SET</dd><dt>14</dt><dd>LIGHT_POTENTIOMETER_14_SET</dd><dt>15</dt><dd>LIGHT_POTENTIOMETER_15_SET</dd><dt>16</dt><dd>LIGHT_POTENTIOMETER_16_SET</dd><dt>17</dt><dd>LIGHT_POTENTIOMETER_17_SET</dd><dt>18</dt><dd>LIGHT_POTENTIOMETER_18_SET</dd><dt>19</dt><dd>LIGHT_POTENTIOMETER_19_SET</dd><dt>20</dt><dd>LIGHT_POTENTIOMETER_20_SET</dd><dt>21</dt><dd>LIGHT_POTENTIOMETER_21_SET</dd><dt>22</dt><dd>LIGHT_POTENTIOMETER_22_SET</dd><dt>23</dt><dd>LIGHT_POTENTIOMETER_23_SET</dd><dt>24</dt><dd>LIGHT_POTENTIOMETER_24_SET</dd><dt>25</dt><dd>LIGHT_POTENTIOMETER_25_SET</dd><dt>26</dt><dd>LIGHT_POTENTIOMETER_26_SET</dd><dt>27</dt><dd>LIGHT_POTENTIOMETER_27_SET</dd><dt>28</dt><dd>LIGHT_POTENTIOMETER_28_SET</dd><dt>29</dt><dd>LIGHT_POTENTIOMETER_29_SET</dd><dt>30</dt><dd>LIGHT_POTENTIOMETER_30_SET</dd></dl></details></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Light Switches</td><td>NOTE: The 'All' lights can only be Toggled.</td><td>Switch {0} Light {1}</td><td><ol start=0>
<li>[choice] &nbsp; <b>All</b>, Beacon, Cabin, Glareshield, Landing, Logo, Nav, Panel, Pedestal, Recognition, Strobe, Taxi, Wing</li>
<li>[choice] &nbsp; <b>Toggle</b>, On, Off</li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>All+Toggle</dt><dd>ALL_LIGHTS_TOGGLE</dd><dt>Beacon+Toggle</dt><dd>TOGGLE_BEACON_LIGHTS</dd><dt>Cabin+Toggle</dt><dd>TOGGLE_CABIN_LIGHTS</dd><dt>Glareshield+Toggle</dt><dd>GLARESHIELD_LIGHTS_TOGGLE</dd><dt>Landing+Toggle</dt><dd>LANDING_LIGHTS_TOGGLE</dd><dt>Logo+Toggle</dt><dd>TOGGLE_LOGO_LIGHTS</dd><dt>Nav+Toggle</dt><dd>TOGGLE_NAV_LIGHTS</dd><dt>Panel+Toggle</dt><dd>PANEL_LIGHTS_TOGGLE</dd><dt>Pedestal+Toggle</dt><dd>PEDESTRAL_LIGHTS_TOGGLE</dd><dt>Recognition+Toggle</dt><dd>TOGGLE_RECOGNITION_LIGHTS</dd><dt>Strobe+Toggle</dt><dd>STROBES_TOGGLE</dd><dt>Taxi+Toggle</dt><dd>TOGGLE_TAXI_LIGHTS</dd><dt>Wing+Toggle</dt><dd>TOGGLE_WING_LIGHTS</dd><dt>Beacon+On</dt><dd>BEACON_LIGHTS_ON</dd><dt>Cabin+On</dt><dd>CABIN_LIGHTS_ON</dd><dt>Glareshield+On</dt><dd>GLARESHIELD_LIGHTS_ON</dd><dt>Landing+On</dt><dd>LANDING_LIGHTS_ON</dd><dt>Logo+On</dt><dd>LOGO_LIGHTS_SET</dd><dt>Nav+On</dt><dd>NAV_LIGHTS_ON</dd><dt>Panel+On</dt><dd>PANEL_LIGHTS_ON</dd><dt>Pedestal+On</dt><dd>PEDESTRAL_LIGHTS_ON</dd><dt>Recognition+On</dt><dd>RECOGNITION_LIGHTS_SET</dd><dt>Strobe+On</dt><dd>STROBES_ON</dd><dt>Taxi+On</dt><dd>TAXI_LIGHTS_ON</dd><dt>Wing+On</dt><dd>WING_LIGHTS_ON</dd><dt>Beacon+Off</dt><dd>BEACON_LIGHTS_OFF</dd><dt>Cabin+Off</dt><dd>CABIN_LIGHTS_OFF</dd><dt>Glareshield+Off</dt><dd>GLARESHIELD_LIGHTS_OFF</dd><dt>Landing+Off</dt><dd>LANDING_LIGHTS_OFF</dd><dt>Logo+Off</dt><dd>LOGO_LIGHTS_SET</dd><dt>Nav+Off</dt><dd>NAV_LIGHTS_OFF</dd><dt>Panel+Off</dt><dd>PANEL_LIGHTS_OFF</dd><dt>Pedestal+Off</dt><dd>PEDESTRAL_LIGHTS_OFF</dd><dt>Recognition+Off</dt><dd>RECOGNITION_LIGHTS_SET</dd><dt>Strobe+Off</dt><dd>STROBES_OFF</dd><dt>Taxi+Off</dt><dd>TAXI_LIGHTS_OFF</dd><dt>Wing+Off</dt><dd>WING_LIGHTS_OFF</dd></dl></details></td>
<td align='center'></td></tr>
<tr valign='top'><td>Master Battery</td><td></td><td>Toggle Master Battery</td><td><ol start=0>
</ol></td>
<td><dl><dd>TOGGLE_MASTER_BATTERY</dd></dl></td>
<td align='center'></td></tr>
<tr valign='top'><td>Master Battery & Alternator</td><td></td><td>Toggle Master Battery & Alternator</td><td><ol start=0>
</ol></td>
<td><dl><dd>TOGGLE_MASTER_BATTERY_ALTERNATOR</dd></dl></td>
<td align='center'></td></tr>
</table>


#### Connectors

<table>
<tr valign='bottom'><th>Name</th><th>Description</th><th>Format</th><th nowrap>Data<br/><div align=left><sub>index. &nbsp; [type] &nbsp; &nbsp; choices/default (in bold)</th></tr>
<tr valign='top'><td>Light Dimming</td><td></td><td>Set Light Potentiometer {0}in Value
Range:{1}-{2}| Feedback From
| State (opt):{3}{4}
Range:{5}-{6}</td><td><ol start=0>
<li>[choice] &nbsp; <b>0</b>, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30</li>
<li>[text] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: 0&gt;</sub> <sub>&lt;max: 100&gt;</sub></li>
<li>[text] &nbsp; <b>100</b> &nbsp; <sub>&lt;min: 0&gt;</sub> <sub>&lt;max: 100&gt;</sub></li>
<li>[choice] &nbsp; <b></b>[connect plugin]</li>
<li>[choice] &nbsp; <b></b>[select a category]</li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
</ol></td>
</table>


#### States

 **Base Id:** MSFSTouchPortalPlugin.Electrical.State.

| Id | SimVar Name | Description | Unit | Format | DefaultValue | Settable |
| --- | --- | --- | --- | --- | --- | --- |
| AvionicsMasterSwitch | AVIONICS MASTER SWITCH | Avionics Master Switch | Bool |  | |  |
| LightBeaconOn | LIGHT BEACON | Light Beacon Switch Status | Bool |  | |  |
| LightBrakeOn | LIGHT BRAKE ON | Light Brake Switch or Light Status | Bool |  | |  |
| LightCabinOn | LIGHT CABIN | Light Cabin Switch Status | Bool |  | |  |
| LightHeadOn | LIGHT HEAD ON | Light Head Switch or Light Status | Bool |  | |  |
| LightLandingOn | LIGHT LANDING | Light Landing Switch Status | Bool |  | |  |
| LightLogoOn | LIGHT LOGO | Light Logo Switch Status | Bool |  | |  |
| LightNavOn | LIGHT NAV | Light Nav Switch Status | Bool |  | |  |
| LightPanelOn | LIGHT PANEL | Light Panel Switch Status | Bool |  | |  |
| LightRecognitionOn | LIGHT RECOGNITION | Light Recognition Switch Status | Bool |  | |  |
| LightStrobeOn | LIGHT STROBE | Light Strobe Switch Status | Bool |  | |  |
| LightTaxiOn | LIGHT TAXI | Light Taxi Switch Status | Bool |  | |  |
| LightWingOn | LIGHT WING | Light Wing Switch Status | Bool |  | |  |
| MasterAlternator | GENERAL ENG MASTER ALTERNATOR:1 | Master Alternator Status | Bool |  | |  |
| MasterBattery | ELECTRICAL MASTER BATTERY | Master Battery Status | Bool |  | | &#9745; |


</details>

---

### Engine
<details><summary><sub>Click to expand</sub></summary>

#### Actions

<table>
<tr valign='bottom'><th>Name</th><th>Description</th><th>Format</th><th nowrap>Data<br/><div align=left><sub>index. &nbsp; [type] &nbsp; &nbsp; choices/default (in bold)</th><th>Sim Event(s)</th><th>On<br/>Hold</sub></div></th></tr>
<tr valign='top'><td>Engine Auto Start/Shutdown</td><td>Start/Shutdown Engine</td><td>Engine - {0}</td><td><ol start=0>
<li>[choice] &nbsp; <b>Start</b>, Shutdown</li>
</ol></td>
<td><dl><dt>Start</dt><dd>ENGINE_AUTO_START</dd><dt>Shutdown</dt><dd>ENGINE_AUTO_SHUTDOWN</dd></dl></td>
<td align='center'></td></tr>
<tr valign='top'><td>Magnetos Switch - All</td><td></td><td>All Magnetos - {0}</td><td><ol start=0>
<li>[choice] &nbsp; <b>Start</b>, Off, Right, Left, Both, Decrease, Increase, Select (for +/-)</li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>Start</dt><dd>MAGNETO_START</dd><dt>Off</dt><dd>MAGNETO_OFF</dd><dt>Right</dt><dd>MAGNETO_RIGHT</dd><dt>Left</dt><dd>MAGNETO_LEFT</dd><dt>Both</dt><dd>MAGNETO_BOTH</dd><dt>Decrease</dt><dd>MAGNETO_DECR</dd><dt>Increase</dt><dd>MAGNETO_INCR</dd><dt>Select (for +/-)</dt><dd>MAGNETO</dd></dl></details></td>
<td align='center'></td></tr>
<tr valign='top'><td>Magnetos Switch - Individual</td><td></td><td>Magneto {0} - {1}</td><td><ol start=0>
<li>[choice] &nbsp; <b>1</b>, 2, 3, 4</li>
<li>[choice] &nbsp; <b>Start</b>, Off, Right, Left, Both, Decrease, Increase</li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>1+Start</dt><dd>MAGNETO1_START</dd><dt>1+Off</dt><dd>MAGNETO1_OFF</dd><dt>1+Right</dt><dd>MAGNETO1_RIGHT</dd><dt>1+Left</dt><dd>MAGNETO1_LEFT</dd><dt>1+Both</dt><dd>MAGNETO1_BOTH</dd><dt>1+Decrease</dt><dd>MAGNETO1_DECR</dd><dt>1+Increase</dt><dd>MAGNETO1_INCR</dd><dt>2+Start</dt><dd>MAGNETO2_START</dd><dt>2+Off</dt><dd>MAGNETO2_OFF</dd><dt>2+Right</dt><dd>MAGNETO2_RIGHT</dd><dt>2+Left</dt><dd>MAGNETO2_LEFT</dd><dt>2+Both</dt><dd>MAGNETO2_BOTH</dd><dt>2+Decrease</dt><dd>MAGNETO2_DECR</dd><dt>2+Increase</dt><dd>MAGNETO2_INCR</dd><dt>3+Start</dt><dd>MAGNETO3_START</dd><dt>3+Off</dt><dd>MAGNETO3_OFF</dd><dt>3+Right</dt><dd>MAGNETO3_RIGHT</dd><dt>3+Left</dt><dd>MAGNETO3_LEFT</dd><dt>3+Both</dt><dd>MAGNETO3_BOTH</dd><dt>3+Decrease</dt><dd>MAGNETO3_DECR</dd><dt>3+Increase</dt><dd>MAGNETO3_INCR</dd><dt>4+Start</dt><dd>MAGNETO4_START</dd><dt>4+Off</dt><dd>MAGNETO4_OFF</dd><dt>4+Right</dt><dd>MAGNETO4_RIGHT</dd><dt>4+Left</dt><dd>MAGNETO4_LEFT</dd><dt>4+Both</dt><dd>MAGNETO4_BOTH</dd><dt>4+Decrease</dt><dd>MAGNETO4_DECR</dd><dt>4+Increase</dt><dd>MAGNETO4_INCR</dd></dl></details></td>
<td align='center'></td></tr>
<tr valign='top'><td>Magnetos Switch Set</td><td></td><td>Set Magneto Switch {0} to position {1} (0-4)</td><td><ol start=0>
<li>[choice] &nbsp; <b>1</b>, 2, 3, 4</li>
<li>[number] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: 0&gt;</sub> <sub>&lt;max: 4&gt;</sub></li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>1</dt><dd>MAGNETO1_SET</dd><dt>2</dt><dd>MAGNETO2_SET</dd><dt>3</dt><dd>MAGNETO3_SET</dd><dt>4</dt><dd>MAGNETO4_SET</dd></dl></details></td>
<td align='center'></td></tr>
<tr valign='top'><td>Master Ignition Switch</td><td>Toggle Master Ignition Switch</td><td>Toggle Master Ignition Switch</td><td><ol start=0>
</ol></td>
<td><dl><dd>TOGGLE_MASTER_IGNITION_SWITCH</dd></dl></td>
<td align='center'></td></tr>
<tr valign='top'><td>Mixture Adjust - All</td><td></td><td>All Mixtures - {0}</td><td><ol start=0>
<li>[choice] &nbsp; <b>Rich</b>, Increase, Increase Small, Decrease, Decrease Small, Lean, Best</li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>Rich</dt><dd>MIXTURE_RICH</dd><dt>Increase</dt><dd>MIXTURE_INCR</dd><dt>Increase Small</dt><dd>MIXTURE_INCR_SMALL</dd><dt>Decrease</dt><dd>MIXTURE_DECR</dd><dt>Decrease Small</dt><dd>MIXTURE_DECR_SMALL</dd><dt>Lean</dt><dd>MIXTURE_LEAN</dd><dt>Best</dt><dd>MIXTURE_SET_BEST</dd></dl></details></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Mixture Adjust - Individual</td><td></td><td>Mixture {0} - {1}</td><td><ol start=0>
<li>[choice] &nbsp; <b>1</b>, 2, 3, 4</li>
<li>[choice] &nbsp; <b>Rich</b>, Increase, Increase Small, Decrease, Decrease Small, Lean</li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>1+Rich</dt><dd>MIXTURE1_RICH</dd><dt>1+Increase</dt><dd>MIXTURE1_INCR</dd><dt>1+Increase Small</dt><dd>MIXTURE1_INCR_SMALL</dd><dt>1+Decrease</dt><dd>MIXTURE1_DECR</dd><dt>1+Decrease Small</dt><dd>MIXTURE1_DECR_SMALL</dd><dt>1+Lean</dt><dd>MIXTURE1_LEAN</dd><dt>2+Rich</dt><dd>MIXTURE2_RICH</dd><dt>2+Increase</dt><dd>MIXTURE2_INCR</dd><dt>2+Increase Small</dt><dd>MIXTURE2_INCR_SMALL</dd><dt>2+Decrease</dt><dd>MIXTURE2_DECR</dd><dt>2+Decrease Small</dt><dd>MIXTURE2_DECR_SMALL</dd><dt>2+Lean</dt><dd>MIXTURE2_LEAN</dd><dt>3+Rich</dt><dd>MIXTURE3_RICH</dd><dt>3+Increase</dt><dd>MIXTURE3_INCR</dd><dt>3+Increase Small</dt><dd>MIXTURE3_INCR_SMALL</dd><dt>3+Decrease</dt><dd>MIXTURE3_DECR</dd><dt>3+Decrease Small</dt><dd>MIXTURE3_DECR_SMALL</dd><dt>3+Lean</dt><dd>MIXTURE3_LEAN</dd><dt>4+Rich</dt><dd>MIXTURE4_RICH</dd><dt>4+Increase</dt><dd>MIXTURE4_INCR</dd><dt>4+Increase Small</dt><dd>MIXTURE4_INCR_SMALL</dd><dt>4+Decrease</dt><dd>MIXTURE4_DECR</dd><dt>4+Decrease Small</dt><dd>MIXTURE4_DECR_SMALL</dd><dt>4+Lean</dt><dd>MIXTURE4_LEAN</dd></dl></details></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Mixture Set</td><td></td><td>Set Mixture {0} to {1} (-16384 to +16384)</td><td><ol start=0>
<li>[choice] &nbsp; <b>All</b>, 1, 2, 3, 4</li>
<li>[text] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: 0&gt;</sub> <sub>&lt;max: 16384&gt;</sub></li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>All</dt><dd>MIXTURE_SET</dd><dt>1</dt><dd>MIXTURE1_SET</dd><dt>2</dt><dd>MIXTURE2_SET</dd><dt>3</dt><dd>MIXTURE3_SET</dd><dt>4</dt><dd>MIXTURE4_SET</dd></dl></details></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Propeller Pitch Adjust</td><td></td><td>Pitch {0} - {1}</td><td><ol start=0>
<li>[choice] &nbsp; <b>All</b>, 1, 2, 3, 4</li>
<li>[choice] &nbsp; <b>Increment</b>, Increment Small, Decrement, Decrement Small, Min (hi pitch), Max (lo pitch), Toggle Feather Switch</li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>All+Increment</dt><dd>PROP_PITCH_INCR</dd><dt>All+Increment Small</dt><dd>PROP_PITCH_INCR_SMALL</dd><dt>All+Decrement</dt><dd>PROP_PITCH_DECR</dd><dt>All+Decrement Small</dt><dd>PROP_PITCH_DECR_SMALL</dd><dt>All+Min (hi pitch)</dt><dd>PROP_PITCH_HI</dd><dt>All+Max (lo pitch)</dt><dd>PROP_PITCH_LO</dd><dt>All+Toggle Feather Switch</dt><dd>TOGGLE_FEATHER_SWITCHES</dd><dt>1+Increment</dt><dd>PROP_PITCH1_INCR</dd><dt>1+Increment Small</dt><dd>PROP_PITCH1_INCR_SMALL</dd><dt>1+Decrement</dt><dd>PROP_PITCH1_DECR</dd><dt>1+Decrement Small</dt><dd>PROP_PITCH1_DECR_SMALL</dd><dt>1+Min (hi pitch)</dt><dd>PROP_PITCH1_HI</dd><dt>1+Max (lo pitch)</dt><dd>PROP_PITCH1_LO</dd><dt>1+Toggle Feather Switch</dt><dd>TOGGLE_FEATHER_SWITCH_1</dd><dt>2+Increment</dt><dd>PROP_PITCH2_INCR</dd><dt>2+Increment Small</dt><dd>PROP_PITCH2_INCR_SMALL</dd><dt>2+Decrement</dt><dd>PROP_PITCH2_DECR</dd><dt>2+Decrement Small</dt><dd>PROP_PITCH2_DECR_SMALL</dd><dt>2+Min (hi pitch)</dt><dd>PROP_PITCH2_HI</dd><dt>2+Max (lo pitch)</dt><dd>PROP_PITCH2_LO</dd><dt>2+Toggle Feather Switch</dt><dd>TOGGLE_FEATHER_SWITCH_2</dd><dt>3+Increment</dt><dd>PROP_PITCH3_INCR</dd><dt>3+Increment Small</dt><dd>PROP_PITCH3_INCR_SMALL</dd><dt>3+Decrement</dt><dd>PROP_PITCH3_DECR</dd><dt>3+Decrement Small</dt><dd>PROP_PITCH3_DECR_SMALL</dd><dt>3+Min (hi pitch)</dt><dd>PROP_PITCH3_HI</dd><dt>3+Max (lo pitch)</dt><dd>PROP_PITCH3_LO</dd><dt>3+Toggle Feather Switch</dt><dd>TOGGLE_FEATHER_SWITCH_3</dd><dt>4+Increment</dt><dd>PROP_PITCH4_INCR</dd><dt>4+Increment Small</dt><dd>PROP_PITCH4_INCR_SMALL</dd><dt>4+Decrement</dt><dd>PROP_PITCH4_DECR</dd><dt>4+Decrement Small</dt><dd>PROP_PITCH4_DECR_SMALL</dd><dt>4+Min (hi pitch)</dt><dd>PROP_PITCH4_HI</dd><dt>4+Max (lo pitch)</dt><dd>PROP_PITCH4_LO</dd><dt>4+Toggle Feather Switch</dt><dd>TOGGLE_FEATHER_SWITCH_4</dd></dl></details></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Propeller Pitch Set</td><td></td><td>Set Propeller {0} Pitch to {1} (0 to 16384)</td><td><ol start=0>
<li>[choice] &nbsp; <b>All</b>, 1, 2, 3, 4</li>
<li>[text] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: 0&gt;</sub> <sub>&lt;max: 16384&gt;</sub></li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>All</dt><dd>PROP_PITCH_SET</dd><dt>1</dt><dd>PROP_PITCH1_SET</dd><dt>2</dt><dd>PROP_PITCH2_SET</dd><dt>3</dt><dd>PROP_PITCH3_SET</dd><dt>4</dt><dd>PROP_PITCH4_SET</dd></dl></details></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Starters</td><td></td><td>Toggle Starter - {0}</td><td><ol start=0>
<li>[choice] &nbsp; <b>All</b>, 1, 2, 3, 4</li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>All</dt><dd>TOGGLE_ALL_STARTERS</dd><dt>1</dt><dd>TOGGLE_STARTER1</dd><dt>2</dt><dd>TOGGLE_STARTER2</dd><dt>3</dt><dd>TOGGLE_STARTER3</dd><dt>4</dt><dd>TOGGLE_STARTER4</dd></dl></details></td>
<td align='center'></td></tr>
<tr valign='top'><td>Throttle Adjust - All</td><td></td><td>All Throttles - {0}</td><td><ol start=0>
<li>[choice] &nbsp; <b>Full</b>, Increase, Increase Small, Decrease, Decrease Small, Cut, 10%, 20%, 30%, 40%, 50%, 60%, 70%, 80%, 90%</li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>Full</dt><dd>THROTTLE_FULL</dd><dt>Increase</dt><dd>THROTTLE_INCR</dd><dt>Increase Small</dt><dd>THROTTLE_INCR_SMALL</dd><dt>Decrease</dt><dd>THROTTLE_DECR</dd><dt>Decrease Small</dt><dd>THROTTLE_DECR_SMALL</dd><dt>Cut</dt><dd>THROTTLE_CUT</dd><dt>10%</dt><dd>THROTTLE_10</dd><dt>20%</dt><dd>THROTTLE_20</dd><dt>30%</dt><dd>THROTTLE_30</dd><dt>40%</dt><dd>THROTTLE_40</dd><dt>50%</dt><dd>THROTTLE_50</dd><dt>60%</dt><dd>THROTTLE_60</dd><dt>70%</dt><dd>THROTTLE_70</dd><dt>80%</dt><dd>THROTTLE_80</dd><dt>90%</dt><dd>THROTTLE_90</dd></dl></details></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Throttle Adjust - Individual</td><td></td><td>Throttle {0} - {1}</td><td><ol start=0>
<li>[choice] &nbsp; <b>1</b>, 2, 3, 4</li>
<li>[choice] &nbsp; <b>Full</b>, Increase, Increase Small, Decrease, Decrease Small, Cut</li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>1+Full</dt><dd>THROTTLE1_FULL</dd><dt>1+Increase</dt><dd>THROTTLE1_INCR</dd><dt>1+Increase Small</dt><dd>THROTTLE1_INCR_SMALL</dd><dt>1+Decrease</dt><dd>THROTTLE1_DECR</dd><dt>1+Decrease Small</dt><dd>THROTTLE1_DECR_SMALL</dd><dt>1+Cut</dt><dd>THROTTLE1_CUT</dd><dt>2+Full</dt><dd>THROTTLE2_FULL</dd><dt>2+Increase</dt><dd>THROTTLE2_INCR</dd><dt>2+Increase Small</dt><dd>THROTTLE2_INCR_SMALL</dd><dt>2+Decrease</dt><dd>THROTTLE2_DECR</dd><dt>2+Decrease Small</dt><dd>THROTTLE2_DECR_SMALL</dd><dt>2+Cut</dt><dd>THROTTLE2_CUT</dd><dt>3+Full</dt><dd>THROTTLE3_FULL</dd><dt>3+Increase</dt><dd>THROTTLE3_INCR</dd><dt>3+Increase Small</dt><dd>THROTTLE3_INCR_SMALL</dd><dt>3+Decrease</dt><dd>THROTTLE3_DECR</dd><dt>3+Decrease Small</dt><dd>THROTTLE3_DECR_SMALL</dd><dt>3+Cut</dt><dd>THROTTLE3_CUT</dd><dt>4+Full</dt><dd>THROTTLE4_FULL</dd><dt>4+Increase</dt><dd>THROTTLE4_INCR</dd><dt>4+Increase Small</dt><dd>THROTTLE4_INCR_SMALL</dd><dt>4+Decrease</dt><dd>THROTTLE4_DECR</dd><dt>4+Decrease Small</dt><dd>THROTTLE4_DECR_SMALL</dd><dt>4+Cut</dt><dd>THROTTLE4_CUT</dd></dl></details></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Throttle Set</td><td></td><td>Set Throttle {0} to {1} (-16384 to +16384)</td><td><ol start=0>
<li>[choice] &nbsp; <b>All</b>, 1, 2, 3, 4</li>
<li>[text] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: -16384&gt;</sub> <sub>&lt;max: 16384&gt;</sub></li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>All</dt><dd>THROTTLE_SET</dd><dt>1</dt><dd>THROTTLE1_SET</dd><dt>2</dt><dd>THROTTLE2_SET</dd><dt>3</dt><dd>THROTTLE3_SET</dd><dt>4</dt><dd>THROTTLE4_SET</dd></dl></details></td>
<td align='center'>&#9745;</td></tr>
</table>


#### Connectors

<table>
<tr valign='bottom'><th>Name</th><th>Description</th><th>Format</th><th nowrap>Data<br/><div align=left><sub>index. &nbsp; [type] &nbsp; &nbsp; choices/default (in bold)</th></tr>
<tr valign='top'><td>Magnetos Switch Set</td><td></td><td>Set Magneto Switch{0}in Range:{1}-{2}| Feedback From
| State (opt):{3}{4}
Range:{5}-{6}</td><td><ol start=0>
<li>[choice] &nbsp; <b>1</b>, 2, 3, 4</li>
<li>[text] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: 0&gt;</sub> <sub>&lt;max: 4&gt;</sub></li>
<li>[text] &nbsp; <b>4</b> &nbsp; <sub>&lt;min: 0&gt;</sub> <sub>&lt;max: 4&gt;</sub></li>
<li>[choice] &nbsp; <b></b>[connect plugin]</li>
<li>[choice] &nbsp; <b></b>[select a category]</li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
</ol></td>
<tr valign='top'><td>Mixture Set</td><td></td><td>Set Mixture{0}in Value
Range:{1}-{2}| Feedback From
| State (opt):{3}{4}
Range:{5}-{6}</td><td><ol start=0>
<li>[choice] &nbsp; <b>All</b>, 1, 2, 3, 4</li>
<li>[text] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: 0&gt;</sub> <sub>&lt;max: 16384&gt;</sub></li>
<li>[text] &nbsp; <b>16384</b> &nbsp; <sub>&lt;min: 0&gt;</sub> <sub>&lt;max: 16384&gt;</sub></li>
<li>[choice] &nbsp; <b></b>[connect plugin]</li>
<li>[choice] &nbsp; <b></b>[select a category]</li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
</ol></td>
<tr valign='top'><td>Propeller Pitch Set</td><td></td><td>Set Propeller{0}Pitch in
Value Range:{1}-{2}| Feedback From
| State (opt):{3}{4}
Range:{5}-{6}</td><td><ol start=0>
<li>[choice] &nbsp; <b>All</b>, 1, 2, 3, 4</li>
<li>[text] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: 0&gt;</sub> <sub>&lt;max: 16384&gt;</sub></li>
<li>[text] &nbsp; <b>16384</b> &nbsp; <sub>&lt;min: 0&gt;</sub> <sub>&lt;max: 16384&gt;</sub></li>
<li>[choice] &nbsp; <b></b>[connect plugin]</li>
<li>[choice] &nbsp; <b></b>[select a category]</li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
</ol></td>
<tr valign='top'><td>Throttle Set</td><td></td><td>Set Throttle{0}in Value
Range:{1}-{2}| Feedback From
| State (opt):{3}{4}
Range:{5}-{6}</td><td><ol start=0>
<li>[choice] &nbsp; <b>All</b>, 1, 2, 3, 4</li>
<li>[text] &nbsp; <b>-16384</b> &nbsp; <sub>&lt;min: -16384&gt;</sub> <sub>&lt;max: 16384&gt;</sub></li>
<li>[text] &nbsp; <b>16384</b> &nbsp; <sub>&lt;min: -16384&gt;</sub> <sub>&lt;max: 16384&gt;</sub></li>
<li>[choice] &nbsp; <b></b>[connect plugin]</li>
<li>[choice] &nbsp; <b></b>[select a category]</li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
</ol></td>
</table>


#### States

 **Base Id:** MSFSTouchPortalPlugin.Engine.State.

| Id | SimVar Name | Description | Unit | Format | DefaultValue | Settable |
| --- | --- | --- | --- | --- | --- | --- |
| MasterIgnitionSwitch | MASTER IGNITION SWITCH | Master Ignition Switch Status | Bool |  | |  |
| MixtureEngine1 | GENERAL ENG MIXTURE LEVER POSITION:1 | Mixture - Engine 1 - Percentage | percent | 0.0# | | &#9745; |
| MixtureEngine2 | GENERAL ENG MIXTURE LEVER POSITION:2 | Mixture - Engine 2 - Percentage | percent | 0.0# | | &#9745; |
| MixtureEngine3 | GENERAL ENG MIXTURE LEVER POSITION:3 | Mixture - Engine 3 - Percentage | percent | 0.0# | | &#9745; |
| MixtureEngine4 | GENERAL ENG MIXTURE LEVER POSITION:4 | Mixture - Engine 4 - Percentage | percent | 0.0# | | &#9745; |
| Propeller1FeatherSw | PROP FEATHER SWITCH:1 | Propeller - Engine 1 - Feather Switch State (bool) | Bool |  | |  |
| Propeller1Feathered | PROP FEATHERED:1 | Propeller - Engine 1 - Feathered (bool) | Bool |  | |  |
| PropellerEngine1 | GENERAL ENG PROPELLER LEVER POSITION:1 | Propeller - Engine 1 - Percentage | percent | 0.0# | | &#9745; |
| Propeller2FeatherSw | PROP FEATHER SWITCH:2 | Propeller - Engine 2 - Feather Switch State (bool) | Bool |  | |  |
| Propeller2Feathered | PROP FEATHERED:2 | Propeller - Engine 2 - Feathered (bool) | Bool |  | |  |
| PropellerEngine2 | GENERAL ENG PROPELLER LEVER POSITION:2 | Propeller - Engine 2 - Percentage | percent | 0.0# | | &#9745; |
| Propeller3FeatherSw | PROP FEATHER SWITCH:3 | Propeller - Engine 3 - Feather Switch State (bool) | Bool |  | |  |
| Propeller3Feathered | PROP FEATHERED:3 | Propeller - Engine 3 - Feathered (bool) | Bool |  | |  |
| PropellerEngine3 | GENERAL ENG PROPELLER LEVER POSITION:3 | Propeller - Engine 3 - Percentage | percent | 0.0# | | &#9745; |
| Propeller4FeatherSw | PROP FEATHER SWITCH:4 | Propeller - Engine 4 - Feather Switch State (bool) | Bool |  | |  |
| Propeller4Feathered | PROP FEATHERED:4 | Propeller - Engine 4 - Feathered (bool) | Bool |  | |  |
| PropellerEngine4 | GENERAL ENG PROPELLER LEVER POSITION:4 | Propeller - Engine 4 - Percentage | percent | 0.0# | | &#9745; |
| RPMN1Engine1 | ENG N1 RPM:1 | RPM - Engine 1 | percent | 0.0# | |  |
| RPMN1Engine2 | ENG N1 RPM:2 | RPM - Engine 2 | percent | 0.0# | |  |
| RPMN1Engine3 | ENG N1 RPM:3 | RPM - Engine 3 | percent | 0.0# | |  |
| RPMN1Engine4 | ENG N1 RPM:4 | RPM - Engine 4 | percent | 0.0# | |  |
| RPMPropeller1 | PROP RPM:1 | RPM - Propeller 1 | rpm | 0.0# | | &#9745; |
| RPMPropeller2 | PROP RPM:2 | RPM - Propeller 2 | rpm | 0.0# | | &#9745; |
| RPMPropeller3 | PROP RPM:3 | RPM - Propeller 3 | rpm | 0.0# | | &#9745; |
| RPMPropeller4 | PROP RPM:4 | RPM - Propeller 4 | rpm | 0.0# | | &#9745; |
| ThrottleEngine1 | GENERAL ENG THROTTLE LEVER POSITION:1 | Throttle - Engine 1 - Percentage | percent | 0.# | | &#9745; |
| ThrottleEngine2 | GENERAL ENG THROTTLE LEVER POSITION:2 | Throttle - Engine 2 - Percentage | percent | 0.# | | &#9745; |
| ThrottleEngine3 | GENERAL ENG THROTTLE LEVER POSITION:3 | Throttle - Engine 3 - Percentage | percent | 0.# | | &#9745; |
| ThrottleEngine4 | GENERAL ENG THROTTLE LEVER POSITION:4 | Throttle - Engine 4 - Percentage | percent | 0.# | | &#9745; |


</details>

---

### Environment
<details><summary><sub>Click to expand</sub></summary>

#### Actions

<table>
<tr valign='bottom'><th>Name</th><th>Description</th><th>Format</th><th nowrap>Data<br/><div align=left><sub>index. &nbsp; [type] &nbsp; &nbsp; choices/default (in bold)</th><th>Sim Event(s)</th><th>On<br/>Hold</sub></div></th></tr>
<tr valign='top'><td>Anti-Ice</td><td></td><td>Anti Ice - {0}</td><td><ol start=0>
<li>[choice] &nbsp; <b>Toggle</b>, On, Off</li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>Toggle</dt><dd>ANTI_ICE_TOGGLE</dd><dt>On</dt><dd>ANTI_ICE_ON</dd><dt>Off</dt><dd>ANTI_ICE_OFF</dd></dl></details></td>
<td align='center'></td></tr>
<tr valign='top'><td>Anti-Ice Engine</td><td></td><td>Anti Ice Engine {0} Toggle</td><td><ol start=0>
<li>[choice] &nbsp; <b>1</b>, 2, 3, 4</li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>1</dt><dd>ANTI_ICE_TOGGLE_ENG1</dd><dt>2</dt><dd>ANTI_ICE_TOGGLE_ENG2</dd><dt>3</dt><dd>ANTI_ICE_TOGGLE_ENG3</dd><dt>4</dt><dd>ANTI_ICE_TOGGLE_ENG4</dd></dl></details></td>
<td align='center'></td></tr>
<tr valign='top'><td>Anti-Ice Engine Set</td><td></td><td>Anti Ice Engine {0} - {1}</td><td><ol start=0>
<li>[choice] &nbsp; <b>1</b>, 2, 3, 4</li>
<li>[switch] &nbsp; <b>False</b></li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>1</dt><dd>ANTI_ICE_SET_ENG1</dd><dt>2</dt><dd>ANTI_ICE_SET_ENG2</dd><dt>3</dt><dd>ANTI_ICE_SET_ENG3</dd><dt>4</dt><dd>ANTI_ICE_SET_ENG4</dd></dl></details></td>
<td align='center'></td></tr>
<tr valign='top'><td>Pitot Heat</td><td></td><td>Pitot Heat - {0}</td><td><ol start=0>
<li>[choice] &nbsp; <b>Toggle</b>, On, Off</li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>Toggle</dt><dd>PITOT_HEAT_TOGGLE</dd><dt>On</dt><dd>PITOT_HEAT_ON</dd><dt>Off</dt><dd>PITOT_HEAT_OFF</dd></dl></details></td>
<td align='center'></td></tr>
<tr valign='top'><td>Propeller De-ice</td><td></td><td>Toggle Propeller DeIce</td><td><ol start=0>
</ol></td>
<td><dl><dd>TOGGLE_PROPELLER_DEICE</dd></dl></td>
<td align='center'></td></tr>
<tr valign='top'><td>Structural De-ice</td><td></td><td>Toggle Structural DeIce</td><td><ol start=0>
</ol></td>
<td><dl><dd>TOGGLE_STRUCTURAL_DEICE</dd></dl></td>
<td align='center'></td></tr>
</table>


#### States

 **Base Id:** MSFSTouchPortalPlugin.Environment.State.

| Id | SimVar Name | Description | Unit | Format | DefaultValue | Settable |
| --- | --- | --- | --- | --- | --- | --- |
| AntiIceEng1 | GENERAL ENG ANTI ICE POSITION:1 | Anti-Ice Engine 1 | Bool |  | |  |
| AntiIceEng2 | GENERAL ENG ANTI ICE POSITION:2 | Anti-Ice Engine 2 | Bool |  | |  |
| AntiIceEng3 | GENERAL ENG ANTI ICE POSITION:3 | Anti-Ice Engine 3 | Bool |  | |  |
| AntiIceEng4 | GENERAL ENG ANTI ICE POSITION:4 | Anti-Ice Engine 4 | Bool |  | |  |
| AntiIcePanelSwitch | PANEL ANTI ICE SWITCH | Panel Anti-Ice Switch | Bool |  | |  |
| PitotHeat | PITOT HEAT | Pitot Heat Status | Bool |  | |  |
| PitotHeatSwitch1 | PITOT HEAT SWITCH:1 | Pitot Heat Switch 1 State (0=Off; 1=On; 2=Auto) | Enum |  | |  |
| PitotHeatSwitch2 | PITOT HEAT SWITCH:2 | Pitot Heat Switch 2 State (0=Off; 1=On; 2=Auto) | Enum |  | |  |
| PitotHeatSwitch3 | PITOT HEAT SWITCH:3 | Pitot Heat Switch 3 State (0=Off; 1=On; 2=Auto) | Enum |  | |  |
| PitotHeatSwitch4 | PITOT HEAT SWITCH:4 | Pitot Heat Switch 4 State (0=Off; 1=On; 2=Auto) | Enum |  | |  |
| AntiIcePropeller1Switch | PROP DEICE SWITCH:1 | Propeller 1 Deice Switch | Bool |  | |  |
| AntiIcePropeller2Switch | PROP DEICE SWITCH:2 | Propeller 2 Deice Switch | Bool |  | |  |
| AntiIcePropeller3Switch | PROP DEICE SWITCH:3 | Propeller 3 Deice Switch | Bool |  | |  |
| AntiIcePropeller4Switch | PROP DEICE SWITCH:4 | Propeller 4 Deice Switch | Bool |  | |  |
| AntiIceStructuralSwitch | STRUCTURAL DEICE SWITCH | Structural Deice Switch | Bool |  | |  |
| AntiIceWindshieldSwitch | WINDSHIELD DEICE SWITCH | Windshield Deice Switch | Bool |  | |  |


</details>

---

### Failures
<details><summary><sub>Click to expand</sub></summary>

#### Actions

<table>
<tr valign='bottom'><th>Name</th><th>Description</th><th>Format</th><th nowrap>Data<br/><div align=left><sub>index. &nbsp; [type] &nbsp; &nbsp; choices/default (in bold)</th><th>Sim Event(s)</th><th>On<br/>Hold</sub></div></th></tr>
<tr valign='top'><td>Failures</td><td></td><td>Toggle Failure - {0}</td><td><ol start=0>
<li>[choice] &nbsp; <b>Electrical</b>, Vacuum, Pitot, Static Port, Hydraulic, Total Brake, Left Brake, Right Brake, Engine 1, Engine 2, Engine 3, Engine 4</li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>Electrical</dt><dd>TOGGLE_ELECTRICAL_FAILURE</dd><dt>Vacuum</dt><dd>TOGGLE_VACUUM_FAILURE</dd><dt>Pitot</dt><dd>TOGGLE_PITOT_BLOCKAGE</dd><dt>Static Port</dt><dd>TOGGLE_STATIC_PORT_BLOCKAGE</dd><dt>Hydraulic</dt><dd>TOGGLE_HYDRAULIC_FAILURE</dd><dt>Total Brake</dt><dd>TOGGLE_TOTAL_BRAKE_FAILURE</dd><dt>Left Brake</dt><dd>TOGGLE_LEFT_BRAKE_FAILURE</dd><dt>Right Brake</dt><dd>TOGGLE_RIGHT_BRAKE_FAILURE</dd><dt>Engine 1</dt><dd>TOGGLE_ENGINE1_FAILURE</dd><dt>Engine 2</dt><dd>TOGGLE_ENGINE2_FAILURE</dd><dt>Engine 3</dt><dd>TOGGLE_ENGINE3_FAILURE</dd><dt>Engine 4</dt><dd>TOGGLE_ENGINE4_FAILURE</dd></dl></details></td>
<td align='center'></td></tr>
</table>


</details>

---

### Flight Instruments
<details><summary><sub>Click to expand</sub></summary>

#### States

 **Base Id:** MSFSTouchPortalPlugin.FlightInstruments.State.

| Id | SimVar Name | Description | Unit | Format | DefaultValue | Settable |
| --- | --- | --- | --- | --- | --- | --- |
| AirSpeedIndicated | AIRSPEED INDICATED | Air Speed indicated (knots) | knots | 0.0# | | &#9745; |
| AirSpeedMach | AIRSPEED MACH | Air Speed indicated (Mach) | mach | 0.0# | |  |
| AirSpeedTrue | AIRSPEED TRUE | Air Speed true (knots) | knots | 0.0# | | &#9745; |
| PlaneAltitudeAGL | PLANE ALT ABOVE GROUND | Altitude Above Ground (feet) | feet | 0.# | | &#9745; |
| PlaneAltitudeIndicated | INDICATED ALTITUDE | Altitude Indicated (feet) | feet | F1 | | &#9745; |
| PlaneAltitude | PLANE ALTITUDE | Altitude True (feet) | feet | 0.# | | &#9745; |
| PlaneBankAngle | PLANE BANK DEGREES | Bank Angle (degrees) | degrees | 0 | | &#9745; |
| FlapSpeedExceeeded | FLAP SPEED EXCEEDED | Flap Speed Exceeded Warning (0/1) | Bool |  | |  |
| GroundAltitude | GROUND ALTITUDE | Ground level (feet) | feet | 0.# | |  |
| GroundVelocity | GROUND VELOCITY | Ground Speed (knots) | knots | 0.0# | |  |
| PlaneHeadingMagnetic | PLANE HEADING DEGREES MAGNETIC | Heading (Magnetic North) (degrees) | degrees | 0 | | &#9745; |
| PlaneHeadingTrue | PLANE HEADING DEGREES TRUE | Heading (True North) (degrees) | degrees | 0 | | &#9745; |
| OverspeedWarning | OVERSPEED WARNING | Overspeed Warning (0/1) | Bool |  | |  |
| PlanePitchAngle | PLANE PITCH DEGREES | Pitch Angle (degrees) | degrees | 0 | | &#9745; |
| StallWarning | STALL WARNING | Stall Warning (0/1) | Bool |  | |  |
| VerticalSpeed | VERTICAL SPEED | Vertical Speed (f/m) | feet/minute | 0.0# | | &#9745; |


</details>

---

### Flight Systems
<details><summary><sub>Click to expand</sub></summary>

#### Actions

<table>
<tr valign='bottom'><th>Name</th><th>Description</th><th>Format</th><th nowrap>Data<br/><div align=left><sub>index. &nbsp; [type] &nbsp; &nbsp; choices/default (in bold)</th><th>Sim Event(s)</th><th>On<br/>Hold</sub></div></th></tr>
<tr valign='top'><td>Aileron Trim Adjust</td><td></td><td>Adjust Aileron Trim {0}</td><td><ol start=0>
<li>[choice] &nbsp; <b>Left</b>, Right</li>
</ol></td>
<td><dl><dt>Left</dt><dd>AILERON_TRIM_LEFT</dd><dt>Right</dt><dd>AILERON_TRIM_RIGHT</dd></dl></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Aileron Trim Set</td><td></td><td>Set Aileron Trim to {0}% (-100 - +100)</td><td><ol start=0>
<li>[text] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: -100&gt;</sub> <sub>&lt;max: 100&gt;</sub></li>
</ol></td>
<td><dl><dd>AILERON_TRIM_SET</dd></dl></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Ailerons Adjust</td><td></td><td>Adjust Ailerons {0}</td><td><ol start=0>
<li>[choice] &nbsp; <b>Center</b>, Left, Right</li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>Center</dt><dd>CENTER_AILER_RUDDER</dd><dt>Left</dt><dd>AILERONS_LEFT</dd><dt>Right</dt><dd>AILERONS_RIGHT</dd></dl></details></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Ailerons Set</td><td> Set Ailerons</td><td>Set Ailerons to {0} (-16384 to +16384)</td><td><ol start=0>
<li>[text] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: -16384&gt;</sub> <sub>&lt;max: 16384&gt;</sub></li>
</ol></td>
<td><dl><dd>AILERON_SET</dd></dl></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Brake Axis Set</td><td></td><td>Set Brake Axis {0} to {1} (0 to 16384)</td><td><ol start=0>
<li>[choice] &nbsp; <b>Left</b>, Right</li>
<li>[text] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: 0&gt;</sub> <sub>&lt;max: 16384&gt;</sub></li>
</ol></td>
<td><dl><dt>Left</dt><dd>AXIS_LEFT_BRAKE_SET</dd><dt>Right</dt><dd>AXIS_RIGHT_BRAKE_SET</dd></dl></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Brakes Activate</td><td></td><td>Activate {0} Brakes</td><td><ol start=0>
<li>[choice] &nbsp; <b>All</b>, Left, Right</li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>All</dt><dd>BRAKES</dd><dt>Left</dt><dd>BRAKES_LEFT</dd><dt>Right</dt><dd>BRAKES_RIGHT</dd></dl></details></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Cowl Flap Levers Adjust</td><td></td><td>Cowl Flaps Lever {0} {1}</td><td><ol start=0>
<li>[choice] &nbsp; <b>All</b>, 1, 2, 3, 4</li>
<li>[choice] &nbsp; <b>Increase</b>, Decrease</li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>All+Increase</dt><dd>INC_COWL_FLAPS</dd><dt>All+Decrease</dt><dd>DEC_COWL_FLAPS</dd><dt>1+Increase</dt><dd>INC_COWL_FLAPS1</dd><dt>1+Decrease</dt><dd>DEC_COWL_FLAPS1</dd><dt>2+Increase</dt><dd>INC_COWL_FLAPS2</dd><dt>2+Decrease</dt><dd>DEC_COWL_FLAPS2</dd><dt>3+Increase</dt><dd>INC_COWL_FLAPS3</dd><dt>3+Decrease</dt><dd>DEC_COWL_FLAPS3</dd><dt>4+Increase</dt><dd>INC_COWL_FLAPS4</dd><dt>4+Decrease</dt><dd>DEC_COWL_FLAPS4</dd></dl></details></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Cowl Flaps Lever Set</td><td></td><td>Set Cowl {0} Flaps Lever to {0} (0 to 16384)</td><td><ol start=0>
<li>[choice] &nbsp; <b>1</b>, 2, 3, 4</li>
<li>[text] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: 0&gt;</sub> <sub>&lt;max: 16384&gt;</sub></li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>1</dt><dd>COWLFLAP1_SET</dd><dt>2</dt><dd>COWLFLAP2_SET</dd><dt>3</dt><dd>COWLFLAP3_SET</dd><dt>4</dt><dd>COWLFLAP4_SET</dd></dl></details></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Elevator Adjust</td><td></td><td>Adjust Elevator {0}</td><td><ol start=0>
<li>[choice] &nbsp; <b>Up</b>, Down</li>
</ol></td>
<td><dl><dt>Up</dt><dd>ELEV_UP</dd><dt>Down</dt><dd>ELEV_DOWN</dd></dl></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Elevator Set</td><td>Set Elevator</td><td>Set Elevator to {0} (-16384 to +16384)</td><td><ol start=0>
<li>[text] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: -16384&gt;</sub> <sub>&lt;max: 16384&gt;</sub></li>
</ol></td>
<td><dl><dd>ELEVATOR_SET</dd></dl></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Elevator Trim Adjust</td><td></td><td>Adjust Elevator Trim {0}</td><td><ol start=0>
<li>[choice] &nbsp; <b>Up</b>, Down</li>
</ol></td>
<td><dl><dt>Up</dt><dd>ELEV_TRIM_UP</dd><dt>Down</dt><dd>ELEV_TRIM_DN</dd></dl></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Elevator Trim Set</td><td></td><td>Set Elevator Trim to {0} (-16384 to +16384)</td><td><ol start=0>
<li>[text] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: -16384&gt;</sub> <sub>&lt;max: 16384&gt;</sub></li>
</ol></td>
<td><dl><dd>ELEVATOR_TRIM_SET</dd></dl></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Flaps Adjust</td><td></td><td>Flaps {0}</td><td><ol start=0>
<li>[choice] &nbsp; <b>Up</b>, Down, Increase, Decrease, 1, 2, 3, 4</li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>Up</dt><dd>FLAPS_UP</dd><dt>Down</dt><dd>FLAPS_DOWN</dd><dt>Increase</dt><dd>FLAPS_INCR</dd><dt>Decrease</dt><dd>FLAPS_DECR</dd><dt>1</dt><dd>FLAPS_1</dd><dt>2</dt><dd>FLAPS_2</dd><dt>3</dt><dd>FLAPS_3</dd><dt>4</dt><dd>FLAPS_3</dd></dl></details></td>
<td align='center'></td></tr>
<tr valign='top'><td>Flaps Handle Set</td><td>Set Flaps Handle Position</td><td>Set Flaps Handle to {0} (0 to 16384)</td><td><ol start=0>
<li>[text] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: 0&gt;</sub> <sub>&lt;max: 16384&gt;</sub></li>
</ol></td>
<td><dl><dd>FLAPS_SET</dd></dl></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Gear Manipulation</td><td></td><td>Gear {0}</td><td><ol start=0>
<li>[choice] &nbsp; <b>Toggle</b>, Up, Down, Pump</li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>Toggle</dt><dd>GEAR_TOGGLE</dd><dt>Up</dt><dd>GEAR_UP</dd><dt>Down</dt><dd>GEAR_DOWN</dd><dt>Pump</dt><dd>GEAR_PUMP</dd></dl></details></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Parking Brake Toggle</td><td></td><td>Toggle the Parking Brake On/Off</td><td><ol start=0>
</ol></td>
<td><dl><dd>PARKING_BRAKES</dd></dl></td>
<td align='center'></td></tr>
<tr valign='top'><td>Rudder Adjust</td><td></td><td>Adjust Rudder {0}</td><td><ol start=0>
<li>[choice] &nbsp; <b>Center</b>, Left, Right</li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>Center</dt><dd>RUDDER_CENTER</dd><dt>Left</dt><dd>RUDDER_LEFT</dd><dt>Right</dt><dd>RUDDER_RIGHT</dd></dl></details></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Rudder Set</td><td> Set Rudder</td><td>Set Rudder to {0} (-16384 to +16384)</td><td><ol start=0>
<li>[text] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: -16384&gt;</sub> <sub>&lt;max: 16384&gt;</sub></li>
</ol></td>
<td><dl><dd>RUDDER_SET</dd></dl></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Rudder Trim Adjust</td><td></td><td>Adjust Rudder Trim {0}</td><td><ol start=0>
<li>[choice] &nbsp; <b>Left</b>, Right</li>
</ol></td>
<td><dl><dt>Left</dt><dd>RUDDER_TRIM_LEFT</dd><dt>Right</dt><dd>RUDDER_TRIM_RIGHT</dd></dl></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Rudder Trim Set</td><td></td><td>Set Rudder Trim to {0}% (-100 - +100)</td><td><ol start=0>
<li>[text] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: -100&gt;</sub> <sub>&lt;max: 100&gt;</sub></li>
</ol></td>
<td><dl><dd>RUDDER_TRIM_SET</dd></dl></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Spoilers Action</td><td></td><td>Spoilers {0}</td><td><ol start=0>
<li>[choice] &nbsp; <b>Toggle</b>, On, Off</li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>Toggle</dt><dd>SPOILERS_TOGGLE</dd><dt>On</dt><dd>SPOILERS_ON</dd><dt>Off</dt><dd>SPOILERS_OFF</dd></dl></details></td>
<td align='center'></td></tr>
<tr valign='top'><td>Spoilers Arm</td><td></td><td>Spoilers Arm {0}</td><td><ol start=0>
<li>[choice] &nbsp; <b>Toggle</b>, On, Off</li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>Toggle</dt><dd>SPOILERS_ARM_TOGGLE</dd><dt>On</dt><dd>SPOILERS_ARM_ON</dd><dt>Off</dt><dd>SPOILERS_ARM_OFF</dd></dl></details></td>
<td align='center'></td></tr>
<tr valign='top'><td>Spoilers Set</td><td></td><td>Set Spoilers handle position to {0} (0 to 16384)</td><td><ol start=0>
<li>[text] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: 0&gt;</sub> <sub>&lt;max: 16384&gt;</sub></li>
</ol></td>
<td><dl><dd>SPOILERS_SET</dd></dl></td>
<td align='center'>&#9745;</td></tr>
</table>


#### Connectors

<table>
<tr valign='bottom'><th>Name</th><th>Description</th><th>Format</th><th nowrap>Data<br/><div align=left><sub>index. &nbsp; [type] &nbsp; &nbsp; choices/default (in bold)</th></tr>
<tr valign='top'><td>Aileron Trim Set</td><td></td><td>Set Aileron Trim
in Value Range (%):{0}-{1}| Feedback From
| State (opt):{2}{3}
Range:{4}-{5}</td><td><ol start=0>
<li>[text] &nbsp; <b>-100</b> &nbsp; <sub>&lt;min: -100&gt;</sub> <sub>&lt;max: 100&gt;</sub></li>
<li>[text] &nbsp; <b>100</b> &nbsp; <sub>&lt;min: -100&gt;</sub> <sub>&lt;max: 100&gt;</sub></li>
<li>[choice] &nbsp; <b></b>[connect plugin]</li>
<li>[choice] &nbsp; <b></b>[select a category]</li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
</ol></td>
<tr valign='top'><td>Ailerons Set</td><td> Set Ailerons</td><td>Set Ailerons
in Value Range:{0}-{1}| Feedback From
| State (opt):{2}{3}
Range:{4}-{5}</td><td><ol start=0>
<li>[text] &nbsp; <b>-16384</b> &nbsp; <sub>&lt;min: -16384&gt;</sub> <sub>&lt;max: 16384&gt;</sub></li>
<li>[text] &nbsp; <b>16384</b> &nbsp; <sub>&lt;min: -16384&gt;</sub> <sub>&lt;max: 16384&gt;</sub></li>
<li>[choice] &nbsp; <b></b>[connect plugin]</li>
<li>[choice] &nbsp; <b></b>[select a category]</li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
</ol></td>
<tr valign='top'><td>Brake Axis Set</td><td></td><td>Set Brake Axis{0}in Value
 Range:{1}-{2}| Feedback From
| State (opt):{3}{4}
Range:{5}-{6}</td><td><ol start=0>
<li>[choice] &nbsp; <b>Left</b>, Right</li>
<li>[text] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: 0&gt;</sub> <sub>&lt;max: 16384&gt;</sub></li>
<li>[text] &nbsp; <b>16384</b> &nbsp; <sub>&lt;min: 0&gt;</sub> <sub>&lt;max: 16384&gt;</sub></li>
<li>[choice] &nbsp; <b></b>[connect plugin]</li>
<li>[choice] &nbsp; <b></b>[select a category]</li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
</ol></td>
<tr valign='top'><td>Cowl Flaps Lever Set</td><td></td><td>Set Cowl{0}Flaps Lever
in Value Range:{1}-{2}| Feedback From
| State (opt):{3}{4}
Range:{5}-{6}</td><td><ol start=0>
<li>[choice] &nbsp; <b>1</b>, 2, 3, 4</li>
<li>[text] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: 0&gt;</sub> <sub>&lt;max: 16384&gt;</sub></li>
<li>[text] &nbsp; <b>16384</b> &nbsp; <sub>&lt;min: 0&gt;</sub> <sub>&lt;max: 16384&gt;</sub></li>
<li>[choice] &nbsp; <b></b>[connect plugin]</li>
<li>[choice] &nbsp; <b></b>[select a category]</li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
</ol></td>
<tr valign='top'><td>Elevator Set</td><td>Set Elevator</td><td>Set Elevator
in Value Range:{0}-{1}| Feedback From
| State (opt):{2}{3}
Range:{4}-{5}</td><td><ol start=0>
<li>[text] &nbsp; <b>-16384</b> &nbsp; <sub>&lt;min: -16384&gt;</sub> <sub>&lt;max: 16384&gt;</sub></li>
<li>[text] &nbsp; <b>16384</b> &nbsp; <sub>&lt;min: -16384&gt;</sub> <sub>&lt;max: 16384&gt;</sub></li>
<li>[choice] &nbsp; <b></b>[connect plugin]</li>
<li>[choice] &nbsp; <b></b>[select a category]</li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
</ol></td>
<tr valign='top'><td>Elevator Trim Set</td><td></td><td>Set Elevator Trim
in Value Range:{0}-{1}| Feedback From
| State (opt):{2}{3}
Range:{4}-{5}</td><td><ol start=0>
<li>[text] &nbsp; <b>-16384</b> &nbsp; <sub>&lt;min: -16384&gt;</sub> <sub>&lt;max: 16384&gt;</sub></li>
<li>[text] &nbsp; <b>16384</b> &nbsp; <sub>&lt;min: -16384&gt;</sub> <sub>&lt;max: 16384&gt;</sub></li>
<li>[choice] &nbsp; <b></b>[connect plugin]</li>
<li>[choice] &nbsp; <b></b>[select a category]</li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
</ol></td>
<tr valign='top'><td>Flaps Handle Set</td><td>Set Flaps Handle Position</td><td>Set Flaps Handle
in Value Range:{0}-{1}| Feedback From
| State (opt):{2}{3}
Range:{4}-{5}</td><td><ol start=0>
<li>[text] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: 0&gt;</sub> <sub>&lt;max: 16384&gt;</sub></li>
<li>[text] &nbsp; <b>16384</b> &nbsp; <sub>&lt;min: 0&gt;</sub> <sub>&lt;max: 16384&gt;</sub></li>
<li>[choice] &nbsp; <b></b>[connect plugin]</li>
<li>[choice] &nbsp; <b></b>[select a category]</li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
</ol></td>
<tr valign='top'><td>Rudder Set</td><td> Set Rudder</td><td>Set Rudder
in Value Range:{0}-{1}| Feedback From
| State (opt):{2}{3}
Range:{4}-{5}</td><td><ol start=0>
<li>[text] &nbsp; <b>-16384</b> &nbsp; <sub>&lt;min: -16384&gt;</sub> <sub>&lt;max: 16384&gt;</sub></li>
<li>[text] &nbsp; <b>16384</b> &nbsp; <sub>&lt;min: -16384&gt;</sub> <sub>&lt;max: 16384&gt;</sub></li>
<li>[choice] &nbsp; <b></b>[connect plugin]</li>
<li>[choice] &nbsp; <b></b>[select a category]</li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
</ol></td>
<tr valign='top'><td>Rudder Trim Set</td><td></td><td>Set Rudder Trim
in Value Range (%):{0}-{1}| Feedback From
| State (opt):{2}{3}
Range:{4}-{5}</td><td><ol start=0>
<li>[text] &nbsp; <b>-100</b> &nbsp; <sub>&lt;min: -100&gt;</sub> <sub>&lt;max: 100&gt;</sub></li>
<li>[text] &nbsp; <b>100</b> &nbsp; <sub>&lt;min: -100&gt;</sub> <sub>&lt;max: 100&gt;</sub></li>
<li>[choice] &nbsp; <b></b>[connect plugin]</li>
<li>[choice] &nbsp; <b></b>[select a category]</li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
</ol></td>
<tr valign='top'><td>Spoilers Set</td><td></td><td>Set Spoilers Handle
Position in Value Range:{0}-{1}| Feedback From
| State (opt):{2}{3}
Range:{4}-{5}</td><td><ol start=0>
<li>[text] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: 0&gt;</sub> <sub>&lt;max: 16384&gt;</sub></li>
<li>[text] &nbsp; <b>16384</b> &nbsp; <sub>&lt;min: 0&gt;</sub> <sub>&lt;max: 16384&gt;</sub></li>
<li>[choice] &nbsp; <b></b>[connect plugin]</li>
<li>[choice] &nbsp; <b></b>[select a category]</li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
</ol></td>
</table>


#### States

 **Base Id:** MSFSTouchPortalPlugin.FlightSystems.State.

| Id | SimVar Name | Description | Unit | Format | DefaultValue | Settable |
| --- | --- | --- | --- | --- | --- | --- |
| AileronTrim | AILERON TRIM | Aileron Trim Angle | degrees | F2 | |  |
| AileronTrimPct | AILERON TRIM PCT | Aileron Trim Percent | percent | F1 | 0| &#9745; |
| CowlFlaps1Percent | RECIP ENG COWL FLAP POSITION:1 | Cowl Flaps 1 Opened Percentage | percent | 0.0# | | &#9745; |
| CowlFlaps2Percent | RECIP ENG COWL FLAP POSITION:2 | Cowl Flaps 2 Opened Percentage | percent | 0.0# | | &#9745; |
| CowlFlaps3Percent | RECIP ENG COWL FLAP POSITION:3 | Cowl Flaps 3 Opened Percentage | percent | 0.0# | | &#9745; |
| CowlFlaps4Percent | RECIP ENG COWL FLAP POSITION:4 | Cowl Flaps 4 Opened Percentage | percent | 0.0# | | &#9745; |
| ElevatorTrim | ELEVATOR TRIM POSITION | Elevator Trim Angle | degrees | F2 | | &#9745; |
| ElevatorTrimPct | ELEVATOR TRIM PCT | Elevator Trim Percent | percent | F1 | 0|  |
| FlapsHandlePercent | FLAPS HANDLE PERCENT | Flaps Handle Percentage | percent | 0.0# | |  |
| ParkingBrakeIndicator | BRAKE PARKING POSITION | Parking Brake Indicator (0/1) | Bool |  | |  |
| RudderTrim | RUDDER TRIM | Rudder Trim Angle | degrees | F2 | |  |
| RudderTrimPct | RUDDER TRIM PCT | Rudder Trim Percent | percent | F1 | 0| &#9745; |
| SpoilersArmed | SPOILERS ARMED | Spoilers Armed (0/1) | Bool |  | 0|  |
| SpoilersAvailable | SPOILER AVAILABLE | Spoilers Available (0/1) | Bool |  | 0|  |
| SpoilersHandlePosition | SPOILERS HANDLE POSITION | Spoilers Handle Position (0 - 16384) | position 16k |  | 0| &#9745; |
| SpoilersLeftPosition | SPOILERS LEFT POSITION | Spoilers Left Position Percent | percent | F1 | 0|  |
| SpoilersRightPosition | SPOILERS RIGHT POSITION | Spoilers Right Position Percent | percent | F1 | 0|  |
| GearTotalExtended | GEAR TOTAL PCT EXTENDED | Total percentage of gear extended | percent | F0 | |  |


</details>

---

### Fuel
<details><summary><sub>Click to expand</sub></summary>

#### Actions

<table>
<tr valign='bottom'><th>Name</th><th>Description</th><th>Format</th><th nowrap>Data<br/><div align=left><sub>index. &nbsp; [type] &nbsp; &nbsp; choices/default (in bold)</th><th>Sim Event(s)</th><th>On<br/>Hold</sub></div></th></tr>
<tr valign='top'><td>Add Fuel</td><td>Add Fuel (1 to 65535 or zero for 25% of capacity)</td><td>Add {0} amount of Fuel</td><td><ol start=0>
<li>[text] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: 0&gt;</sub> <sub>&lt;max: 65535&gt;</sub></li>
</ol></td>
<td><dl><dd>ADD_FUEL_QUANTITY</dd></dl></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Cross Feed Switch</td><td></td><td>Cross Feed - {0}</td><td><ol start=0>
<li>[choice] &nbsp; <b>Toggle</b>, Open, Off</li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>Toggle</dt><dd>CROSS_FEED_TOGGLE</dd><dt>Open</dt><dd>CROSS_FEED_OPEN</dd><dt>Off</dt><dd>CROSS_FEED_OFF</dd></dl></details></td>
<td align='center'></td></tr>
<tr valign='top'><td>Electric Fuel Pump</td><td></td><td>Toggle Electric Fuel Pump: {0}</td><td><ol start=0>
<li>[choice] &nbsp; <b>All</b>, 1, 2, 3, 4</li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>All</dt><dd>TOGGLE_ELECT_FUEL_PUMP</dd><dt>1</dt><dd>TOGGLE_ELECT_FUEL_PUMP1</dd><dt>2</dt><dd>TOGGLE_ELECT_FUEL_PUMP2</dd><dt>3</dt><dd>TOGGLE_ELECT_FUEL_PUMP3</dd><dt>4</dt><dd>TOGGLE_ELECT_FUEL_PUMP4</dd></dl></details></td>
<td align='center'></td></tr>
<tr valign='top'><td>Fuel Dump - Toggle</td><td></td><td>Toggle Fuel Dump</td><td><ol start=0>
</ol></td>
<td><dl><dd>FUEL_DUMP_TOGGLE</dd></dl></td>
<td align='center'></td></tr>
<tr valign='top'><td>Fuel Pump</td><td></td><td>Toggle Fuel Pump</td><td><ol start=0>
</ol></td>
<td><dl><dd>FUEL_PUMP</dd></dl></td>
<td align='center'></td></tr>
<tr valign='top'><td>Fuel Selectors</td><td></td><td>Fuel Selector {0} - {1}</td><td><ol start=0>
<li>[choice] &nbsp; <b>1</b>, 2, 3, 4</li>
<li>[choice] &nbsp; <b>All</b>, Off, Left, Right, Left - Main, Right - Main, Left - Aux, Right - Aux, Center</li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>1+All</dt><dd>FUEL_SELECTOR_ALL</dd><dt>1+Off</dt><dd>FUEL_SELECTOR_OFF</dd><dt>1+Left</dt><dd>FUEL_SELECTOR_LEFT</dd><dt>1+Right</dt><dd>FUEL_SELECTOR_RIGHT</dd><dt>1+Left - Main</dt><dd>FUEL_SELECTOR_LEFT_MAIN</dd><dt>1+Right - Main</dt><dd>FUEL_SELECTOR_RIGHT_MAIN</dd><dt>1+Left - Aux</dt><dd>FUEL_SELECTOR_LEFT_AUX</dd><dt>1+Right - Aux</dt><dd>FUEL_SELECTOR_RIGHT_AUX</dd><dt>1+Center</dt><dd>FUEL_SELECTOR_CENTER</dd><dt>2+All</dt><dd>FUEL_SELECTOR_2_ALL</dd><dt>2+Off</dt><dd>FUEL_SELECTOR_2_OFF</dd><dt>2+Left</dt><dd>FUEL_SELECTOR_2_LEFT</dd><dt>2+Right</dt><dd>FUEL_SELECTOR_2_RIGHT</dd><dt>2+Left - Main</dt><dd>FUEL_SELECTOR_2_LEFT_MAIN</dd><dt>2+Right - Main</dt><dd>FUEL_SELECTOR_2_RIGHT_MAIN</dd><dt>2+Left - Aux</dt><dd>FUEL_SELECTOR_2_LEFT_AUX</dd><dt>2+Right - Aux</dt><dd>FUEL_SELECTOR_2_RIGHT_AUX</dd><dt>2+Center</dt><dd>FUEL_SELECTOR_2_CENTER</dd><dt>3+All</dt><dd>FUEL_SELECTOR_3_ALL</dd><dt>3+Off</dt><dd>FUEL_SELECTOR_3_OFF</dd><dt>3+Left</dt><dd>FUEL_SELECTOR_3_LEFT</dd><dt>3+Right</dt><dd>FUEL_SELECTOR_3_RIGHT</dd><dt>3+Left - Main</dt><dd>FUEL_SELECTOR_3_LEFT_MAIN</dd><dt>3+Right - Main</dt><dd>FUEL_SELECTOR_3_RIGHT_MAIN</dd><dt>3+Left - Aux</dt><dd>FUEL_SELECTOR_3_LEFT_AUX</dd><dt>3+Right - Aux</dt><dd>FUEL_SELECTOR_3_RIGHT_AUX</dd><dt>3+Center</dt><dd>FUEL_SELECTOR_3_CENTER</dd><dt>4+All</dt><dd>FUEL_SELECTOR_4_ALL</dd><dt>4+Off</dt><dd>FUEL_SELECTOR_4_OFF</dd><dt>4+Left</dt><dd>FUEL_SELECTOR_4_LEFT</dd><dt>4+Right</dt><dd>FUEL_SELECTOR_4_RIGHT</dd><dt>4+Right - Main</dt><dd>FUEL_SELECTOR_4_RIGHT_MAIN</dd><dt>4+Left - Main</dt><dd>FUEL_SELECTOR_4_LEFT_MAIN</dd><dt>4+Left - Aux</dt><dd>FUEL_SELECTOR_4_LEFT_AUX</dd><dt>4+Right - Aux</dt><dd>FUEL_SELECTOR_4_RIGHT_AUX</dd><dt>4+Center</dt><dd>FUEL_SELECTOR_4_CENTER</dd></dl></details></td>
<td align='center'></td></tr>
<tr valign='top'><td>Fuel Valve</td><td></td><td>Toggle Fuel Valve: {0}</td><td><ol start=0>
<li>[choice] &nbsp; <b>All</b>, 1, 2, 3, 4</li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>All</dt><dd>TOGGLE_FUEL_VALVE_ALL</dd><dt>1</dt><dd>TOGGLE_FUEL_VALVE_ENG1</dd><dt>2</dt><dd>TOGGLE_FUEL_VALVE_ENG2</dd><dt>3</dt><dd>TOGGLE_FUEL_VALVE_ENG3</dd><dt>4</dt><dd>TOGGLE_FUEL_VALVE_ENG4</dd></dl></details></td>
<td align='center'></td></tr>
<tr valign='top'><td>Primers</td><td></td><td>Toggle Primer(s): {0}</td><td><ol start=0>
<li>[choice] &nbsp; <b>All</b>, 1, 2, 3, 4</li>
</ol></td>
<td><details><summary><sub>details</sub></summary><dl><dt>All</dt><dd>TOGGLE_PRIMER</dd><dt>1</dt><dd>TOGGLE_PRIMER1</dd><dt>2</dt><dd>TOGGLE_PRIMER2</dd><dt>3</dt><dd>TOGGLE_PRIMER3</dd><dt>4</dt><dd>TOGGLE_PRIMER4</dd></dl></details></td>
<td align='center'></td></tr>
</table>


#### Connectors

<table>
<tr valign='bottom'><th>Name</th><th>Description</th><th>Format</th><th nowrap>Data<br/><div align=left><sub>index. &nbsp; [type] &nbsp; &nbsp; choices/default (in bold)</th></tr>
<tr valign='top'><td>Add Fuel</td><td>Add Fuel (1 to 65535 or zero for 25% of capacity)</td><td>Add Fuel
in Value Range:{0}-{1}| Feedback From
| State (opt):{2}{3}
Range:{4}-{5}</td><td><ol start=0>
<li>[text] &nbsp; <b>0</b> &nbsp; <sub>&lt;min: 0&gt;</sub> <sub>&lt;max: 65535&gt;</sub></li>
<li>[text] &nbsp; <b>65535</b> &nbsp; <sub>&lt;min: 0&gt;</sub> <sub>&lt;max: 65535&gt;</sub></li>
<li>[choice] &nbsp; <b></b>[connect plugin]</li>
<li>[choice] &nbsp; <b></b>[select a category]</li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
<li>[text] &nbsp; &lt;empty&gt; &nbsp; <sub>&lt;min: -340282346638528859811704183484516925440&gt;</sub> <sub>&lt;max: 340282346638528859811704183484516925440&gt;</sub></li>
</ol></td>
</table>


</details>

---

### System
<details><summary><sub>Click to expand</sub></summary>

#### Actions

<table>
<tr valign='bottom'><th>Name</th><th>Description</th><th>Format</th><th nowrap>Data<br/><div align=left><sub>index. &nbsp; [type] &nbsp; &nbsp; choices/default (in bold)</th><th>Sim Event(s)</th><th>On<br/>Hold</sub></div></th></tr>
<tr valign='top'><td>Change Selected Value (+/-)</td><td></td><td>{0} Selected Value</td><td><ol start=0>
<li>[choice] &nbsp; <b>Increase</b>, Decrease</li>
</ol></td>
<td><dl><dt>Increase</dt><dd>PLUS</dd><dt>Decrease</dt><dd>MINUS</dd></dl></td>
<td align='center'>&#9745;</td></tr>
<tr valign='top'><td>Simulation Rate</td><td></td><td>{0} Simulation Rate</td><td><ol start=0>
<li>[choice] &nbsp; <b>Increase</b>, Decrease</li>
</ol></td>
<td><dl><dt>Increase</dt><dd>SIM_RATE_INCR</dd><dt>Decrease</dt><dd>SIM_RATE_DECR</dd></dl></td>
<td align='center'>&#9745;</td></tr>
</table>


#### Events

 **Base Id:** MSFSTouchPortalPlugin.SimSystem.Event. &nbsp; &nbsp; **Base State Id** MSFSTouchPortalPlugin.

<table>
<tr valign='bottom'><th>Id</th><th>Name</th><th nowrap>Evaluated State Id</th><th>Format</th><th>Type</th><th>Choice(s)</th></tr>
<tr valign='top'><td>SimSystemEvent</td><td>Simulator System Event</td><td>SimSystem.State.SimSystemEvent</td><td>On Simulator Event $choice</td><td>choice</td><td><details><summary><sub>details</sub></summary>
<ul><li><b>Connecting</b> - Upon every connection attempt to the Simulator.</li>
<li><b>Connected</b> - Upon successful connection to the Simulator.</li>
<li><b>Disconnected</b> - Upon disconnection from the Simulator.</li>
<li><b>Connection Timed Out</b> - When a connection attempt to the Simulator times out, eg. when sim is not running.</li>
<li><b>SimConnect Error</b> - When a Simulator (SimConnect) error or warning is detected. Error details (log entry) are sent in the SimSystemEventData state value.</li>
<li><b>Plugin Error</b> - When a Plugin-specific error or warning is detected (eg. could not parse action data, load a file, etc). Error details (log entry) are sent in the SimSystemEventData state value.</li>
<li><b>Plugin Information</b> - When a notable plugin informational ("success") action is detected. Information details (log entry) are sent in the SimSystemEventData state value.</li>
<li><b>Paused</b> - When the flight is paused (but not active pause nor 'esc' pause).</li>
<li><b>Unpaused</b> - When the flight is un-paused  (but not active pause nor 'esc' pause).</li>
<li><b>Pause Toggled</b> - When the flight is paused or unpaused (but not active pause nor 'esc' pause).</li>
<li><b>Flight Started</b> - The simulator is running. Typically the user is actively controlling the aircraft on the ground or in the air. However, in some cases additional pairs of SimStart/SimStop events are sent. For example, when a flight is reset the events that are sent are SimStop, SimStart, SimStop, SimStart.</li>
<li><b>Flight Stopped</b> - The simulator is not running. Typically the user is loading a flight, navigating the shell or in a dialog.</li>
<li><b>Flight Toggled</b> - When the flight changes between running and not.</li>
<li><b>Aircraft Loaded</b> - When the aircraft flight dynamics file is changed. These files have a .AIR extension. The filename is sent in the SimSystemEventData state value.</li>
<li><b>Crashed</b> - When the user aircraft crashes.</li>
<li><b>Crash Reset</b> - When the crash cut-scene has completed.</li>
<li><b>Flight Loaded</b> - When a flight is loaded. Note that when a flight is ended, a default flight is typically loaded, so these events will occur when flights and missions are started and finished. The filename of the flight loaded is sent in the SimSystemEventData state value.</li>
<li><b>Flight Saved</b> - When a flight is saved correctly. The filename of the flight saved is sent in the SimSystemEventData state value.</li>
<li><b>Flight Plan Activated</b> - When a new flight plan is activated. The filename of the activated flight plan is sent in the SimSystemEventData state value.</li>
<li><b>Flight Plan Deactivated</b> - When the active flight plan is de-activated.</li>
<li><b>Position Changed</b> - When the user changes the position of their aircraft through a dialog or loading a flight.</li>
<li><b>Sound Toggled</b> - When the master sound switch is changed.</li>
<li><b>View 3D Cockpit</b> - When the view changes to the 3D virtual cockpit view.</li>
<li><b>View External</b> - When the view changes to an external view.</li></ul></details></td></tr>
</table>


#### States

 **Base Id:** MSFSTouchPortalPlugin.SimSystem.State.

| Id | SimVar Name | Description | Unit | Format | DefaultValue | Settable |
| --- | --- | --- | --- | --- | --- | --- |
| AtcId | ATC ID | Aircraft Id used by ATC | string |  | | &#9745; |
| AircraftTitle | TITLE | Aircraft Title | string |  | |  |
| AtcAirline | ATC AIRLINE | Airline used by ATC | string |  | | &#9745; |
| SimSystemEventData |  | Data from the most recent Simulator System Event, if any. | string |  | |  |
| AtcFlightNumber | ATC FLIGHT NUMBER | Flight Number used by ATC | string |  | | &#9745; |
| AtcModel | ATC MODEL | Model of aircraft used by ATC | string |  | |  |
| SimSystemEvent |  | Most recent Simulator System Event name. | string |  | |  |
| SimulationRate | SIMULATION RATE | The current simulation rate | number |  | |  |
| AtcType | ATC TYPE | Type of aircraft used by ATC | string |  | |  |


</details>

---

