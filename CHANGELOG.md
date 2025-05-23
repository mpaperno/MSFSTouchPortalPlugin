# MSFS/SimConnect Touch Portal Plugin Change Log

## 1.6.2.0 (May-21-2024)
Version number: `1060200`

* Fixed that many of the basic plugin actions were broken in v1.6.1. The errors were logged as "Could not parse required action parameters for [action name] from data: ..."

---
## 1.6.1.0 (May-20-2024)
Version number: `1060100`

### Fixes
* Fixed validation of L vars to allow leading `_` (fixes [#73]). Thanks to @Mushupm on GitHub for reporting and testing a fix.
* Simulator ('A') type variables can now also have leading index/component name elements (eg. "3:VAR NAME:1").
* Fixed an issue which apparently affected a very limited number of systems (only 1 known occurrence) which resulted in most of the plugin v1.6.0 actions not working at all,
  except when used in a button's "On Hold" setup. This would only have affected actions added to buttons/flows since plugin v1.6.0, older versions of the same actions are unaffected.<br/>
  **Note** For the fix to take effect, any existing plugin v1.6.0 actions need to be replaced with the new ones from this version.<br/>
  Thanks to `@Raeas` on TP's Discord for reporting and troubleshooting a similar issue on another plugin, which led to the fix. (This turned out to be due to a silent Touch Portal error which prevented the action from being sent to the plugin.)
* The "_Data from the most recent Simulator System Event_" State is now updated _before_ the "_Simulator System Event_" is triggered so that the data will be available in event handlers immediately.
  Fixes possible issue with needing to add a delay before trying to use the data value, eg. in "AirplaneLoaded" event handler to read the model file name.

### New States
* Plugin
  * _Plugin Running State_ (`stopped` / `starting` / `started`) - Changes based on plugin status. Can be used with Touch Portal's "Start/Stop Plug-in" action for a toggle effect, for example.
  * _Status of WASM module_ (`unknown` / `undetected` / `disconnected` / `connected`) - (MSFS edition only) Reports connection status of the optional WASM module.
    Starts as "unknown" before initial simulator connection is established. `undetected` means the module could not be connected to at all, due to it not being installed or a connection error.
    Otherwise the status should match the main SimConnect connection status of either connected or disconnected.
* Simulator System
  * _Simulator Version_ (`0` / `10` / `11` / `12`) - Last connected simulator "major" version (default before any connection is `0`).  
    These are the version numbers reported by SimConnect: 10 = FSX, 11 = FS20, 12 = FS24. (No idea what P3D reports, sorry!)

### New Events
Implemented new Touch Portal v4+ event types that can contain associated data values as "local states," which can be used like other "local values" in actions that are placed inside the event block.<br/>
These events are alternatives to current options of listening for plugin state changes or the individual events in "_Simulator System Event_" choices.

Further details about each event and their local states is available in the generated plugin documentation on the [Wiki] (expand the category details and then go to "Events" link).

* Plugin
  * _Plugin Message Event_ - emitted when the plugin logs an informative message. Local states contain the message source and severity plus the actual message text.
  * _Simulator Connection Change_ - emitted when connection to the simulator changes. Alternative to watching the 'The status of SimConnect' State for changes, or using the individual event types in "_Simulator System Event_",
  * _Touch Portal Device Page Change_ - emitted when the plugin detects a new page has been loaded on a Touch Portal device. The local states contain the new and previous page names and information about the device.
* Simulator System
  * _Simulator Pause State Change_ - emitted when the Pause State of the simulator changes (MSFS edition only). Local states contain new and previous pause states in numeric and text formats.
  * _Simulator System Filename Event_ - emitted in response to any simulator system event which has an associated file name, such as when loading aircraft, flights, or flight plans.
    It has two local states which describe the event and contain the file name.

[#73]: https://github.com/mpaperno/MSFSTouchPortalPlugin/issues/73

---
## 1.6.0.0 (Nov-30-2024)
Version number: `1060000`

### New Features for Touch Portal v4.3
* Added ability to customize the "On Hold" behavior for each individual action:
  * Can now set if an action will be activated on Press, Release, both, or neither (repeat only).
  * Repeating on hold can be turned off.
  * The Repeat Rate (interval) can be set per action.
  * A new Repeat Delay property is added for setting an initial wait period before an action starts repeating (a new plugin setting has been added to set the default delay, see "Other New Features").
  * You can now have separate actions triggering for a button's "press" vs. "release" by placing them in both the "On Hold" and "On Pressed" button setup panels, respectively.
    The action(s) in "On Pressed" is/are only activated when the button is _released_, while the one in "On Hold" can be set to fire only on the initial button _press_.
* All action & connector category groups are now placed inside one main group for the whole plugin. They also now have new individual icons.
* The long actions from "Custom States & Variables" category are now formatted in a multi-row form-style layout.

### Other New Features
* Added "Request an Input Event Value" action for "Custom States & Variables" category which allows selection of an event name from a live list.
* The lists of available Simulator Variables and Events in various actions (like "Activate a Selected Simulator Event") are now filtered by connected simulator version.
  The database distinguishes between MSFS X, 2020, and 2024 versions. 'MSFS' plugin edition loads FS 24 data by default (before first connection to sim), 'FSX' edition loads FS-X data
  (P3D users are on their own for now, sorry!).
* Event names in the HubHop Events list now indicate if it is a "potentiometer" event (expects an input value) by appending an "@" symbol. A snippet of the event description,
  if any exists, has also been added.

### New Settings
* Added "Check For New Plugin Version on Startup" setting, which does as it says. This is disabled by default. Minimum time between update checks is 6 hours, even if plugin is restarted.
* Added "Default Held Action Repeat Delay" plugin setting. Initial delay value is going to be the same as the current repeat interval (rate). (This affects _all_ held/repeating actions,
  not just the new "TP v4" style added in this version.)
* Added "Maximum Length of Descriptions in Action Lists" setting to control how much description text appears in lists of Simulator Variables, Key Events, and HubHop Events.
  The descriptions can also be disabled entirely by setting this option to zero.
* Added "Maximum Message Log Lines" setting to control how many logging entries are sent in the "Most recent plugin log messages" state value. This can also be set to zero to suppress
  all updates of that state.

### Fixes
* Fixed being able to set string type values on Simulator Variables (like ATC IDs) in MSFS edition since plugin v1.5 (FSX was OK).
* Fixed missing value field in "Set Cowl Flaps Lever" action (cowl index was duplicated instead).
* Fixed action "Radio Interaction" with options "COM3 Decrease 25 KHz w/ Carry Digits" which used an invalid event ID.

### Changes
* Moved "Failures" action into "Simulator System" category. This removes the "Failures" category from the actions menu since that was the only action in it.
* Renamed "Set a Selected Airplane Input Event Value" -> "Set a Selected Input Event Value".
* Minimum held action repeat interval (rate) was reduced from 50ms to 25ms.
* The lists of available Simulator Variables and Key Events in actions now include those marked as deprecated in sim documentation, with a "(DEPRECATED)" note added in details. Some of those may still work.
* Improved version comparison check when using `SimVersion` property in variable request INI files and added example of usage in the [Custom States - INI Files] wiki and included [States.ini].
* When using the "Plugin - Status of SimConnect" state in an `IF` action, the 3 possible value choices (true/false/connecting) will be available for selection.
* **Touch Portal v3 users** may wish to use the alternate `entry.tp` plugin definitions file as described in the [Replacing The entry.tp File For Touch Portal v3] wiki page. The TP v3 file is already included with the plugin.

### For MSFS 2024
* Setting 'B' type variables (triggering Input Events) is now possible for the "_Set", "_Inc", "_Dec", "_Toggle" etc, suffixes found in the sim's dev mode "Behaviors" inspector.
  These do not show up in the list of Input Events the plugin gets from the simulator itself, but can be manually entered in the "Set Named Variable" or "Execute Calculator Code" actions.
* Validation of Simulator Variable ('A' type) names will now accept the new "VARIABLE NAME:'ComponentName'_n" syntax in MSFS 24.
* New Simulator Variables and Events are available in action selection lists when connected to FS 24.

### 'FSX' Edition
* Removed all actions/connectors which use Key Events that don't exist in FS-X.
* Events and Sim Vars shown in action selection lists will now be limited only to those that FS-X is known to support.
* The "Set a Named Variable" and "Request a Named Variable" actions have been simplified to exclude the irrelevant variable type selector.

### Other Improvements
* Updated database of Simulator Variables and Event IDs imported from MSFS 2020 & 2024 SDK Web site documentation as of Nov. 25 '24. The groupings now reflect published FS '24 documentation (a few events/vars were moved around).
  Also added 91 Key Events that aren't documented anywhere except in the game itself (these are sorted into the "Miscellaneous - Undocumented" category).
* Generated documentation layout of plugin actions, connectors, and events has been updated for clarity; Separate MSFS and FSX plugin edition versions are now generated, and the documentation has been moved to the [Wiki].
* Documentation of plugin's settings has been improved for the new Touch Portal v4.3 layout. Actual (dotted) version number is now shown at the top as well.
* HubHop data update has been streamlined a bit, and now fixes any mis-categorized "potentiometer" type events (that expect input values) in the downloaded data,
  where some events that did not expect input were marked as if they do, and vice versa. The included database of events is current as of Nov. 30th.
* Improved error checking on some submitted action data to provide more useful diagnostics in the plugin logs in case of failure.
* Some minor improvements on shutdown when dealing with unexpected simulator/SimConnect disconnections and Touch Portal exceptions.
* Updated to use .NET 8 runtime for MSFS edition (up from v7). Most external dependencies also got an update. (All required components are included in the distribution as before.)
* MSFS edition built with latest MSFS 2020 SDK/SimConnect [v0.24.3][MSFS_SDK_ChangeLog].
* Updated WASimCommander components from v1.2.0 to [v1.3.1](https://github.com/mpaperno/WASimCommander/releases/tag/1.3.1.0) for features and fixes.


[MSFS_SDK_ChangeLog]: https://docs.flightsimulator.com/html/Introduction/Release_Notes.htm
[States.ini]: https://github.com/mpaperno/MSFSTouchPortalPlugin/blob/next/MSFSTouchPortalPlugin/Configuration/States.ini
[Wiki]: https://github.com/mpaperno/MSFSTouchPortalPlugin/wiki
[Custom States - INI Files]: https://github.com/mpaperno/MSFSTouchPortalPlugin/wiki/Using-Custom-States-and-Simulator-Variables#editing-configuration-files-manually
[Replacing The entry.tp File For Touch Portal v3]: https://github.com/mpaperno/MSFSTouchPortalPlugin/wiki/Replacing-The-entry.tp-File-For-Touch-Portal-v3
[Release 1.6.0]: https://github.com/mpaperno/MSFSTouchPortalPlugin/releases/tag/1.6.0.0


---
## 1.5.0.0 (Nov-7-2023)
Version number: `1050000`

### Additions
* Added _experimental_ ability to set and read simulator "Input Events" values, which is a new feature in MSFS SU13. This provides access to aircraft-specific controls which may not be available otherwise.
  These events can be seen in MSFS "developer mode" using the "Behaviors" tool window.
  They are also referred-to as "B" type variables, and custom variable requests can be set up for them like for other variable types.
  * **IMPORTANT NOTES**
    * **The SU13 implementation of this is BUGGY!!**
      - First off, it **doesn't work at all unless developer mode is enabled in MSFS.**
      - **The simulator will crash** if there are any Input Event values being requested/read while switching airplane models. So either don't use that feature for now or make sure to manually
        remove any requested 'B' type variables before changing models (including in he main menu/World Map). See new feature below for removing all requests of particular variable type.
    * Both issues are supposedly fixed in SU14 (currently in beta), but I have not tested it personally.
    * Only the currently loaded model's Input Events exist in the simulator at any given time.
      Plugin errors are logged when trying to set or request an input event ('B' var) value which doesn't exist in the current model.
    * There may be other changes in SU14 which will modify or break how the whole system works... the current system is rather wonky. So, no guarantees (as usual).
  * Added "Set a Selected Airplane Input Event Value" action with a list of the current model's Input Events.
  * Added 'B' variable type choice to "Set a Named Variable" and "Request a Named Variable" actions.
  * Added ""Update Airplane Input Events List" and "Re-Submit Input Event Value Requests" choices in the "Connect & Update" action.
* Added ability in the "Clear Variable Definitions" action to remove variable requests by the file they were loaded from and by the type of variable.

### Changes
* Setting ('L') type variables with a specific Unit type is now (again) handled by WASimModule (when available) for greater efficiency.
  * 'L' vars are created on the simulator if they don't already exist, using the specified Unit type (if any), which mimics SimConnect behavior.
  * The "Create Local Variable" option has been removed from the "Set a Named Variable" action.
* For Touch Portal v4:
  * The plugin will now show up in the "Games" category instead of "Misc."
  * Descriptions of each setting are now available in TP's "Plug-in Settings" dialog by hovering over the question mark icons.
* Updated to use .NET 7 runtime environment (.NET 6 for FSX version) and dependencies (up from .NET 5). All required components are included in the distribution as before.

### Fixes
* Fixed "Plug-in performance issue detected" Touch Portal v4 warning message. ([#63](https://github.com/mpaperno/MSFSTouchPortalPlugin/issues/63))
* Fixed that very long lists (thousands) of currently loaded Airplane Local variables were sometimes incomplete in the corresponding "Set" and "Request" actions.
* Fixed a possible plugin crash when list of Local variables was being updated multiple times in quick succession. Also prevents the list from being updated unnecessarily.
* Fixed possible simulator hang when exiting to desktop while plugin is still connected and using WASimModule (sim had to be terminated from task manager). This seems to have been new behavior since SU13.

### Related
* The user-interface part of my WASimCommander project, [WASimUI](https://github.com/mpaperno/WASimCommander/tree/next#desktop-gui), now supports importing and exporting custom variable requests in the plugin's format.
  That includes a new interface for editing plugin-specific information like category, state ID, name, default value, and formatting.
  It can be downloaded at that project's [Releases](https://github.com/mpaperno/WASimCommander/releases/tag/1.2.0.0) page.

---
## 1.4.3.3 (Sept-3-2023)
Version number: `1040303`

* Fixed simulator event actions not working after a re-connection to the sim when WASM integration is unavailable (eg. FSX edition).
* Fixed "Avionics Master Switch" action missing choices list of what to actually do.
* Fixed that "Starters Set" action was not available in On Hold tab.
* Added better handling of Sim Var request errors based on SimConnect responses -- invalid requests should now be automatically removed or suspended (with details logged).
* Added ability to specify a required simulator version for particular Sim Var request definitions (eg. only MSFS/v11).

---
## 1.4.3.2 (Sept-2-2023 @ flightsim.to)
Version number: `1040301`

* Allow running the FSX plugin concurrently with MSFS plugin.

---
## 1.4.3.1 (Sept-2-2023 @ flightsim.to)
Version number: `1040301`

* Initial release of "FSX" version of the plugin for older sims (FSX Deluxe SP2 or newer).

---
## 1.4.3.0 (Sept-1-2023 - limited test)
Version number: `1040300`

* Fixed requests for local ('L') variables with 'Millisecond' type update period not working when WASM integration is used.

---
## 1.4.2.0 (Aug-5-2023)
Version number: `1040200`

* Fixes variable name validation issue with some Local ("L") type simulator variables which contained `:` in their names (fixes https://github.com/mpaperno/MSFSTouchPortalPlugin/issues/57).
* Re-introduce ability to specify Unit types when setting or requesting Local type variables. Note that most L vars will ignore/not use the Unit type -- this seems only to be relevant
  for a very few 3rd-party models/vars (closes https://github.com/mpaperno/MSFSTouchPortalPlugin/issues/58).
* Improve logging of variable request actions to the TP "Log Messages" State (eg. the States Editor page shows more verbose details including validation errors).
* Slightly optimized the Touch Portal client communication library for quicker message output.
* Updated database of imported Simulator Variables and Event IDs from MSFS SDK online documentation as of August 5th.
* Updated SimConnect libraries to latest versions (SDK v0.21.1.0).

---
## 1.4.1.0 (Apr-15-2023)
Version number: `1040100`

* Fixes WASM client connection issue with plugin release v1.4.0.

---
## 1.4.0.0 (Apr-15-2023)
Version number: `1040000`

* Added ability to request and set Local ("L") type simulator variables without WASM add-on (now a native SimConnect feature since SU12).
  * Note: SimConnect automatically creates L vars if they are requested before they actually exist in the sim. "Missing" L vars are no longer reported as warnings.
    This could lead to some confusion, for example if a variable name is misspelled or never used by any model in the first place.
* Added some basic validation of variable request parameters when loading from .INI configuration files. Invalid requests are rejected. Validation errors and warnings are reported in the plugin's log.
* The "Paused", "Unpaused" and "Pause Toggled" Touch Portal Events now work correctly (or at least more logically/consistently) due to a fix in SU12.
  * Note: the "Pause Toggled" event triggers on any/every "pause mode" change, even if the sim is not actually "unpaused" completely.
    The name has been kept for backwards compatibility. See the new event descriptions below for more details on pause modes.
  * The "Paused" event is also triggered when _any_ pause mode is activated, even if the simulator already had another pause mode active.
  * "Unpaused" at least behaves as expected and only triggers when _all_ pause modes have been deactivated.
* Prevent a rare condition when connecting to MSFS where the main SimConnect connection gets established properly but the WASM client's connection fails unexpectedly.
* Updated database of Simulator Variables and Event IDs imported from MSFS SDK Web site documentation as of April 12th.
* Updated SimConnect libraries to latest versions (SDK v0.21.0.0).

### Added or Updated Actions/Connectors
_MSFS - Simulator System_ category:
* Added "Pause - Full" - Action, pauses the simulation completely, including any time passing in the simulated world. Same as "Dev Mode Pause" (in developer toolbar "Options" menu).
* Added "Pause - Simulator" - Action, pauses some aspects of the simulation, but not time. Same as "Menu (ESC) Pause" but w/out the actual menu.
* Added "Simulation Rate Set" - Action and connector.
* Renamed "Simulation Rate" to "Simulation Rate Adjust" and added "Select (for +/- adjustment)" option.
* Renamed "Change Selected Value (+/-)" to "Adjust a Selected Value (+/-)" and added a description/help text.

### Added Events
Added to _MSFS - Simulator System -> Simulator System Event_ choices:
* "Full Pause" - Indicates a complete pause including any time passing. Triggered when a flight is loading, by "Dev Mode Pause" (in developer toolbar "Options"),
  the new "Pause - Full" plugin action, or a SimConnect "PAUSE_SET" Event with a value of `1`. (Possibly other ways as well?)
* "Active Pause" - Simulator has been paused using "active pause" key binding or toolbar button.
* "Simulator Pause" - The simulator has been paused with the "ESC" key to the menu, or with "Pause - Simulator" action, or "PAUSE_ON" SimConnect Event.
* "Full Pause (FSX Legacy)" - Not used in FS2020

### Added State
_MSFS - Simulator System_ category:
* "Simulator Pause State Flag(s) (OFF|FULL|ACTIVE|SIM)" - Lists any "pause states" which are currently active, separated by commas, or "OFF" if the simulation is not paused in any way.
  Eg. if "active pause" is enabled and the sim is also paused with the "ESC" key, the value of this state would be "ACTIVE,SIM". See descriptions of new Events for more details on pause modes.

---
## 1.3.2.0 (Mar-09-2023)
Version number: `1030200`

* Fixed "Clear Custom States" and "Save Custom States to File" actions not properly selecting variable requests which were loaded from custom configuration file(s).
  The default states from the included States.ini configuration file were being deleted/saved instead.  Thanks to Glenn#6307 @ Discord for reporting!

---
## 1.3.1.0 (Feb-05-2023)
Version number: `1030100`

* Fixed updating the list of local variables for "Request an Airplane Local Variable" action (https://github.com/mpaperno/MSFSTouchPortalPlugin/issues/49).

---
## 1.3.0.0 (Jan-31-2023)
Version number: `1030000`

* Added database of Events, SimVars, and Unit types imported ("scraped") from MSFS SDK Web site documentation (part of my separate "MSFS Tools" project).
* Event and SimVar actions which allowed selection from lists now refer to imported MSFS documentation. These now show partial descriptions and Event parameter meanings (when available).
* _Any_ SimVar marked settable in MSFS docs is now available in (the updated) "Set a Selected Simulator Variable" action/connector with a selectable Unit type.
  * The SimVar no longer has to be "requested" first. This works with or without _WASimModule_ integration (_SimConnect_ only).
    * In the latter case, _SimConnect_ only, any action/connector which sets the same SimVar (at the same index, if any) must use the same Unit type.
  * Connector type now has "feedback" as an option; when enabled, the corresponding SimVar is automatically requested from the simulator if it has not been already.
* When selecting imported SimVars in actions (for Set or Request), only compatible Unit types are now shown (eg. all distance type measures for an "altitude" value).
* Fixed that selecting "Camera & Views" category didn't properly show the available variable requests in some actions (https://github.com/mpaperno/MSFSTouchPortalPlugin/issues/47).
* Removed the "Can Set" SimVar request property (`CanSet` is ignored if found in existing .INI config files); Also removes the corresponding indicator column from generated documentation.
* Removed the "Release AI" option on all "set variable" type actions (doesn't seem to have any effect on user aircraft; please let me know if you did, in fact, use that option).
* Plugin now only sends "fatal" level messages to Touch Portal's log file, all others go only to plugin's own log (eliminates duplicate logging).
* Updated WASimModule to v1.1.1 with optimized build for MSFS SU11+ and updated Key Event data.
* Updated SimConnect libraries to latest versions (SDK v0.20.5.0).

### Added or Updated Actions/Connectors
In the list, `+` means added and `~` means updated. A few of these actions were renamed as detailed in the next section.

* `~` "AP Switches" - added Panel Switch On/Off/Toggle for: Speed Hold, Altitude Hold, Heading Hold, Mach Hold, VS (on/off).
* `+` "N1 Reference Value Adjust/Hold" and "Set" AP actions.
* `+` "ADF Adjust" and "Set" actions to control ADF1/2 radios.
* `+` "Transponder Adjust" and "Set" actions to control XPNDR code and IDENT state.
* `~` "Radio Interaction" - COM1-3 added "Copilot Transmit Select" and "Receive De-select"; Fixed that "Receive Select" option was not a toggle.
* `~` "Radio Values Set" - Added "Receive Select (0/1)" for COM1-3.
* `~` "Avionics Master Switch" - Added "Off" and "On" options for individual Master 1 and 2 switches.
* `+` "Alternator Control" to set On/Off/Toggle any indexed alternator.
* `~` "Landing Lights Switch/Direction" - added `circuit index` value field.
* `~` "Light Switch Control" - renamed and added `circuit index` value field and "Circuit Toggle" option.
* `+` "External Power" On/Off/Toggle with optional index value.
* `+` "Engine Master Toggle" and "Set" actions for engines All/1/2/3/4.
* `~` "Starters Toggle" - added Master Switch option.
* `+` "Starters Set" action with "Set" and "Set Held" options.
* `+` "Afterburner Toggle" for engines All/1/2/3/4.
* `+` "Engine Condition Lever Adjust" and "Set" (by Position/Axis) actions for engines All/1/2/3/4.
* `+` "Refuel & Repair" - Request Fuel (parked) and Repair & Refuel (depends on realism settings) actions.
* `~` "Fuel Selectors" - added Crossfeed and Isolate actions for each selector.
* `+` "Fuel System Component" action for controlling Pumps, Triggers, and Valves indexed fuel systems.
* `~` "Cross Feed Switch" - added Left to Right and Right to Left options.
* `+` "Electric Fuel Pump Set" On/Off/Auto for pumps 1-4.
* `~` "Fuel Dump / Tank Drop" - renamed and added "Release Drop Tank" (All/1/2) options.

### Renamed or Replaced Actions/Connectors
* "Set Simulator Variable (SimVar)" --> "Set a Selected Simulator Variable"
* "Set Airplane Local Variable" --> "Set a Selected Airplane Local Variable"
* "Set Named Variable Value" --> "Set a Named Variable"
* "Request a Custom Simulator Variable" --> "Request a Named Variable"
* "Request a Variable From List" --> split to:
  * "Request a Selected Simulator Variable"
  * "Request an Airplane Local Variable"
* "Alternator Switches" --> "Alternator Toggle"
* "Light Switches" --> "Light Switch Control"
* "Starters" --> "Starters Toggle"
* "Electric Fuel Pump" --> "Electric Fuel Pump Toggle"
* "Fuel Dump - Toggle" --> "Fuel Dump / Tank Drop"
* [Fuel] "Primers" action moved to Engine category.

### DEPRECATED Actions/Connectors
Existing instances will still work but cannot be edited. **Please update to new versions and report any regression issues with existing instances.**
* "Set Simulator Variable (SimVar)" (use "Set a Selected Simulator Variable" instead).
* "Request a Custom Simulator Variable" (use "Request a Named Variable" instead).
* "Request a Variable From List" (use "Request a Selected Simulator Variable" or "Request an Airplane Local Variable" instead).
* "Fuel Pump" (use "Electric Fuel Pump Set" and "Toggle" actions).

---
## 1.2.0.0 (Nov-2-2022)
Version number: `1020000`

* The "Activate a Named Simulator Event" and "Activate a Selected Simulator Event" actions and connectors can now transmit multiple values
  for Simulator Key Events which require multiple parameters.
* Renamed plugin action/state categories:
  * "Communication" to "Radio & Navigation"
  * "System" to "Simulator System"
* All the custom variable request editing actions moved to new "Custom States & Variables" category in TP's' sidebar.

---
## 1.1.0.6 (Sept-11-2022)
Version number: `1010006`

* Fixed possible plugin startup issue when Windows user name has space(s) in it (adds quotes around startup command).
* Added new "Miscellaneous" category for sorting custom states, by user request.

---
## 1.1.0.5-rc2 (Aug-24-2022)
Version number: `1010005`

* Fixed reporting initial value of Action Repeat Interval state at plugin startup (thanks @ magicnorm on TP's Discord for reports).
* Fixed: "Electrical - Light Dimming" action was missing the Value field (the pots list was repeated instead; connector version was OK).
* Fixed duplicates appearing in "List of currently loaded state configuration file(s)" state when files were being reloaded.
* Any Connectors (Sliders) which use "feedback" are now set to the corresponding default state values upon initial plugin startup.
* Added new action to force update of all Connectors (Sliders) which use "feedback" states to represent current values ("MSFS Plugin -> Connect & Update -> Re-Send All Connector Feedback Values").
* Added experimental color coding around log level indicator in "Most recent plugin log messages" state.
* The "Update Local Var. List" and "Re-Submit Local Var. Requests" actions were renamed with "... Airplane Local ..." for clarity (backwards compatible).

---
## 1.1.0.4-rc1 (Aug-13-2022)
Version number: `1010004`

* Fixed: Requests for indexed SimVars with an Update Period of type "Millisecond" were not working.
* Fixed "AutoPilot - Flight Director Switches" action missing choice selector for Master/Pitch Sync switch.
* Fixed "Auto Pilot - Vertical Speed -> Hold Current" action.
* Added Auto Pilot actions:
  * "Bank Mode" and "Flight Level Change" switches (on/off/toggle).
  * "Auto Throttle Disconnect"
  * "Max. Bank Set" - Preset position, Angle, or Velocity  (action/connector)
  * Auto Brake - "Set Off/Low/Medium/Maximum" switch actions & "Set to Value" (action/connector)
* Consolidated "Anti/De Ice System" switch actions (on/off/toggle)
* Added Anti/De Ice System actions:
  * "Windshield De Ice Switch" Toggle/On/Off/Set.
  * Set actions for "Engines 1-4 Anti Ice" and "Pitot Heat" Switch states.
  * "Engines 1-4 Anti Ice Set" value range (0 - 16384) action/connector.
* Adjusted Delta Epsilon properties for the default included variable requests to match the displayed decimal precision.
* Added new states in "MSFS - Plugin" category to facilitate handling of custom variable request configuration files and page imports:
  * Plugin configuration files path.
  * Touch Portal configuration files path.
  * List of currently loaded state configuration file(s).
  * The current device Touch Portal page name (including folder, if any).
* Changes to the "Connect To Flight Sim on Startup" setting now take effect w/out a plugin restart.

---
## 1.1.0.3-beta3 (Aug-2-2022)
Version number: `1010003`

* Enabled WASimModule integration from multiple simultaneous plugin instances (when using multiple Touch Portal servers/devices).
* Improved several aspects of custom variable requests for stability and usability, such as utilizing WASM integration when available, and more informative logging.
* Improved aspects of triggering Simulator Events, including utilizing WASM module integration when available, for greater efficiency.
* Added automatic retry of failed Local ('L') variable requests when airplane model changes or a new flight is loaded.
* Added new action to re-submit requests for Local variables which could not be registered the first time (because they didn't exist yet).
* Added new action to clear all/custom variable data requests.
* Added new action to re-send all current State values.
* Added more COM/NAV options to current "Radio Interaction" action (COM3/NAV3-4/Volume/etc) and added new "Radio Values Set" action/connector.
* Fixed possible Simulator crash due to SimConnect exception when removing variable requests (or updating existing ones which first removed the old versions).
* Fixed: SimVar ('A') requests with Update Period type of Millisecond were not updating. (Thanks to @ _schroewo_ on TP's Discord for reports.)
* Fixed: Writing State/variable configuration files didn't properly save the Update Period and Delta Epsilon properties (if the latter was zero).
* Fixed sorting of variables by category in saved state configuration files.
* Changed default variable request Delta Epsilon property value to `0.0`.
* Changed the data field in "Load Variable Definitions From File" action to plain text entry instead of a file selector control.
* Changed the slider/connector min/max range data entry fields from numeric to more permissive text types.
* Improved detection of duplicate/replacement variable requests (new requests for existing variables).
* Renamed the simulator connection (toggle/on/off) actions for clarity (preserves backwards compatibility with existing buttons).
* The read-only setting "Held action repeat interval" has been removed from the TP Settings interface.
* A plugin settings configuration file is now always created in user's config file path (default AppData/Roaming) if one doesn't already exist.

---
## 1.1.0.2-beta2 (July-22-2022)
Version number: `1010002`

* Added new Setting for always using "neutral" numeric formatting with period decimal separators, ignoring any region-specific formatting conventions.
  Works around Touch Portal issue with not being able to do math comparison operations on numeric values with comma decimal separators.<br />
  !! This setting is now _enabled_ by default. !!  (With apologies to everyone who expects proper number formatting for their locale.)
* Fixed that some actions without data values were broken in Beta 1 version (some light switches, perhaps others).
* Consolidated several groups of separate actions of the same type into single actions with target selector (light switches, AP switches, cowl flaps).
  Maintains backwards compatibility with existing actions/buttons.
* Added Light Dimming (potentiometer) actions, some more light switch types, and Landing lights directional control actions.

---
## 1.1.0.0-beta1 (July-19-2022)
Version number: `1010000`

* Added Connector (Slider) functionality with feedback capabilities (eg. move slider in response to simulator event).
* Added 29 new connectors to set variables, trigger events with value ranges, or use as visual value indicators.
* Added new "Camera & Views" category for custom states.
* **Changed** "On Hold" actions to fire the associated event right away instead of waiting for the first delay time.<br />
  (Seems TP now (?) sends "Press" events on button release vs. press, so the previous feature of being able to map a different event to the initial press no longer really works out.)
* Failed actions will no longer repeat when "held."

### New Connectors
* All 18 existing (and 2 new) "Set" Actions (engines, surfaces, AP bugs, etc).
* Set Action Repeat Interval
* Set a Named Simulator Event Value
* Set a Known Simulator Event Value
* Set HubHop Input Event Value  <sup>(requires WASimModule)</sup>
* Set Simulator Variable (SimVar)
* Set Airplane Local Variable  <sup>(requires WASimModule)</sup>
* Set Named Variable Value  <sup>(requires WASimModule)</sup>
* Execute Calculator Code With Value  <sup>(requires WASimModule)</sup>
* Visual Feedback Connector (for reflecting the value of a State)

### New Actions and Connectors
* Mixture Lever Set
* Brake Axis Left/Right Set

### Updated Actions
* The "Set Simulator Variable" and "Set Local Variable" actions have been split into 2 (restores pre-v1.0 version of the former).
* Cowl Flaps 1-4 Set consolidated into one action/connector with choice of flaps.
* Updated some action names in AP, Fuel, Engine, Flight System categories for consistency and clarity. Removed some redundant descriptions.

---
## 1.0.1.0 (July-19-2022)
Version number: `1000100`

* Removed all static Sim Var states from entry.tp file -- all states are now dynamic.
* Added new Setting option to sort Local ('L') Airplane variables list alphabetically.
* Added new Setting option to control HubHop data update timeout value.
* Fixed exception error when requesting variables with an empty Unit type.
* Fixed selections in "Activate a Simulator Event From HubHop" action not updating properly after HubHop data update until plugin is restarted
  ([#32](https://github.com/mpaperno/MSFSTouchPortalPlugin/issues/32)).
* Fixed "Activate a Simulator Event From List" was broken in last version (produced SimConnect errors).
* Fixed that state names for requested indexed SimVars didn't include the proper index number.
* Improved user feedback for HubHop data update events (initiated/updated/failed).
* Increased length of version number shown in Touch Portal by 2 digits to include all 4 parts of dotted version (1.0.0.1 = 1000001).
* New icon for the plugin executable.

---
## 1.0.0.0-beta1 (July-09-2022)
Version number: `1000000`

* Adds support for integration with custom WASM module from the [WASimCommander project](https://github.com/mpaperno/WASimCommander) (WASimModule).
    * Get and Set "Local" variables as well as practically any other
      [variable type](https://docs.flightsimulator.com/html/Additional_Information/Reverse_Polish_Notation.htm#Types) available in the MSFS "Gauge API".
    * Provides a listing of currently available Local variables, automatically refreshed when aircraft changes in the simulator, and on demand.
    * Option to _create_ new Local variables inside the Simulator engine.
    * Execute any Calculator Code (using RPN, see link above) from within the Simulator engine, bringing control over practically any situation.
    * Request calculated values (results from calculator code) as TP States, using the same options as current simulator variables
      (formatting, refresh rate, categorization, etc). Results can be in numeric or string format, and data can also be formatted as strings using RPN functions.
    * All the new request types (local/other variables or calculated values) can be saved/loaded to/from configuration files, just like was already possible with SimVars.
* Adds support for loading input event presets from the [HubHop database](https://hubhop.mobiflight.com).
    * New Action: _Activate a Simulator Event From HubHop_ - Presents a "drill down" list of loaded events, selectable by vendor/aircraft and system.
      **Requires WASimModule or MobiFlight WASM modules** installed (more features available with the former).
    * HubHop data can, optionally, be updated upon plugin startup and manually via a new action choice (_MSFS Plugin -> Connect & Update -> Update HubHop_).
* Added support for using numeric values in hexadecimal notation (0xNNN). Easy "BCD" values, eg. frequency 339KHz is simply `0x03390000`.
* Dynamic TP States (custom-added variables) are now sorted into their respective categories in TP selectors
  (instead of being in a "Dynamic" category or just listed after the categories). Requires TP 3.0.10 or higher.
* Added actions to _Set_ cowl flaps 1-4 to specific position.
* The list of imported sim vars updated to include new `HSI_STATION_IDENT` and fix "settable" flag on `GPS_*` variables like `GPS_APPROACH_WP_TYPE`, etc.

### New Actions
* Activate a Simulator Event From HubHop  <sup>(requires WASimModule or MobiFlight WASM)</sup>
* Set Airplane Local Variable  <sup>(requires WASimModule)</sup>
* Set Named Variable Value  <sup>(requires WASimModule)</sup>
* Execute Calculator Code  <sup>(requires WASimModule)</sup>
* Request a Named Variable  <sup>(requires WASimModule)</sup>
* Request a Calculated Value  <sup>(requires WASimModule)</sup>
* Update a Variable Value  <sup>(requires WASimModule)</sup>
* Cowl Flaps 1-4 Lever Set
* Cowl Flap Levers - Adjust All
* Update Local (Airplane) Variables List  <sup>(requires WASimModule)</sup>
* Update HubHopData

---
## 0.7.0.2-mp (April-21-2022)
* Fixes an issue that prevented MobiFlight events from working.
* Fixes possible plugin crash when trying to re-connect to MSFS after it has crashed.
* Improved detection of an actual valid SimConnect link when trying to connect. SimConnect seems sometimes "confused" after the sim has exited unexpectedly (crashed).

---
## 0.7.0.1-mp (April-18-2022)

* This release fixes missing .Net 5.0 dependency issue with v0.7.0. This would only affect users who did not have .Net 5 libraries already installed on their PC.
* There is also a minor bug fix for SimConnect.cfg file not being copied to the correct folder.
* Lastly, the distribution size has been reduced by almost half.

---
## 0.7.0.0-mp (April-17-2022)

### Major Features

#### Simulator Variables (SimVars)
* All SimVar/Touch Portal State definitions loaded from easily editable .ini configuration files.
* Can load custom state config file(s) at startup or via new TP Action.
* Request any arbitrary [Simulation Variable](https://docs.flightsimulator.com/html/Programming_Tools/SimVars/Simulation_Variables.htm) by name to be sent as a TP State via 2 new TP Actions,
  with ability to save added variables to a config. file.
* New TP Action to **Set** a value on any settable SimVar. The Action presents a dynamic list of loaded SimVars which are configured to be settable.

#### Events
* Run any arbitrary [Simulator Event](https://docs.flightsimulator.com/html/Programming_Tools/Event_IDs/Event_IDs.htm) (action), with optional value,
  either selected from a list of known (imported) events or any custom event name (allows full MobiFlight compatibility, for example).
* Adds ability to monitor [Simulator System Events](https://docs.flightsimulator.com/html/Programming_Tools/SimConnect/API_Reference/Events_And_Data/SimConnect_SubscribeToSystemEvent.htm)
  via a new _Touch Portal_ Event (and corresponding State).
* New _Touch Portal_ Events to indicate simulator connection status, connection timeout, simulator errors/warnings, and plugin errors and informative events.
* A new TP State to transmit corresponding event data, if any, such as name of flight being loaded (for sim System Events) or details about error/information events (log messages).

#### Performance
The changes below lead to considerable performance improvements and much quicker state updates (when the values change), while also being more efficient.
* SimVars/states are now updated via "push" from SimConnect at a settable period and interval (defaults to every "Sim Frame" eg. ~60Hz) vs. being polled individually every 250ms.
  Custom update polling period can also be set per SimVar in milliseconds.
* SimVars are only updated if changed by a configurable epsilon value, eg. for a decimal value SimConnect will not send update unless it changes by more than 0.001.
* Implemented custom version of the Touch Portal C# client/API layer which is several times more performant and, among other things,
  allows our plugin to exit cleanly if stopped (send any last state updates, shut down SimConnect connection properly, etc).

#### Others
* Add ability to use a custom [SimConnect.cfg](https://docs.flightsimulator.com/html/Programming_Tools/SimConnect/SimConnect_CFG_Definition.htm) file for simulator connection properties --
  this allows (among other things) to easily use the plugin on a Touch Portal instance running on a different networked computer than the simulator,
  including multiple plugin instances which can be used on multiple TP client devices.
* Add new LogMessags TP State which contains the last 12 plugin log entries (same entries as can be found in the log file, but fewer of them and with shorter formatting).

### Fixes and Minor Improvements
* Fix missing values for states CowlFlaps1/2/3/4 and AutoPilotFlightDirectorCurrentBank. [29](https://github.com/mpaperno/MSFSTouchPortalPlugin/issues/29)
* Fix that the +/-16383 value limits are actually +/- 16384 regardless of what the SimConnect docs claim (0x4000, which is actually more logical and an even number).
* Fix formatting of _AutoPilotAirSpeedVar_ state. [26](https://github.com/mpaperno/MSFSTouchPortalPlugin/issues/26)
* Fix formatting on "Total percentage of gear extended," "AutoPilot Pitch Reference Value," "AutoPilot Max Bank Angle," "Flight Director Current Bank," and "Flight Director Current Pitch" states.
* Fix _Throttle Specific Decrement_ and _Decrement Small_ actions broken due to spelling.
* Add FlightSystems spoiler states: _SpoilersAvailable_, _SpoilersArmed_, _SpoilersHandlePosition_, _SpoilersLeftPosition_, _SpoilersRightPosition_. [7](https://github.com/mpaperno/MSFSTouchPortalPlugin/issues/7)
* Add plugin version number information States, for both running and entry.tp version numbers. [11](https://github.com/mpaperno/MSFSTouchPortalPlugin/issues/11)
* Add optional value for "Add Fuel" action.
* Make most "Set" actions repeatable (on hold).
* Unit conversions (eg. radians to degrees) are no longer necessary since we can request simulator variable values in specific units right from SimConnect.
* More reliable SimConnect re-connection attempts and connection/disconnection/timeout/error detection and reporting.
* Changed (again) the plugin's TP UI colors to a darker blue and lighter black to help with readability.
* Adjust logging format to include ms, implement better template for log file, allow setting log levels from env. vars, file logging is now asynchronous.
* A lot of the logging in general has been improved/added/made more consistent throughout all the parts of the plugin. Eliminates noise and focuses on important details.
* Numerous internal code optimizations, smaller improvements, organization, and documentation.

### Documentation
* Add SimVar state's Unit, Format, and Settable status to generated docs.
* Add documentation for new TP Events with full descriptions of each choice (currently there's only the one event in "System" category).
* Add plugin version number to generated docs.
* MSFSTouchPortalPlugin-Generator.exe utility is now included with the plugin for optionally generating entry.tp from custom state configs. Run with `--help` for options.

### Upgrade Notes
* **The plugin's Settings will be reset to default.** Specifically the "connect on startup" and "action repeat interval" options.
* The new default for the _Connect To Flight Sim on Startup_ setting is **false**.
* **For use with FS-X** (and compatible sims), the new setting "_SimConnect.cfg Index (0 for MSFS, 1 for FSX, or custom)_" should be set to `1`.
* The delay time between reconnection attempts to the simulator (eg. when it's not yet running) has been increased from 10 to 30 seconds.

(See DOCUMENTATION.md for details on all the new items listed below.)

#### New Settings
* Sim Variable State Config File(s) (blank = Default)
* SimConnect.cfg Index (0 for MSFS, 1 for FSX, or custom)
* User Config Files Path (blank for default)

#### New Actions
##### Plugin
* Activate a Custom Simulator Event
* Activate a Simulator Event From List
* Set Simulator Variable Value
* Request a Custom Simulator Variable
* Request a Simulator Variable From List
* Remove a Simulator Variable
* Load SimVar Definitions From File
* Save SimVar Definitions To File
* Reload SimVar Definition Files (as configured in Settings)

#### New States
##### Plugin
* RunningVersion  -- The running plugin version number.
* EntryVersion - The loaded entry.tp plugin version number.
* ConfigVersion - The loaded entry.tp custom configuration version.
* LogMessages - Most recent plugin log messages.
##### Flight Systems
* Spoilers Armed (0/1)
* Spoilers Available (0/1)
* Spoilers Handle Position (0 - 16384)
* Spoilers Left & Right Position Percent
##### System
* SimSystemEvent - Most recent Simulator System Event name (triggers the new events below).
* SimSystemEventData - Data from the most recent Simulator System Event, if any.

#### New Events
##### System
* Simulator Connecting
* Simulator Connected
* Simulator Disconnected
* Simulator Connection Timed Out
* SimConnect Error
* Plugin Error
* Plugin Information
* Paused
* Unpaused
* Pause Toggled
* Flight Started
* Flight Stopped
* Flight  Toggled
* Aircraft Loaded
* Crashed
* Crash Reset
* Flight Loaded
* Flight Saved
* Flight Plan Activated
* Flight Plan Deactivated
* Position Changed
* Sound Toggled
* View 3D Cockpit
* View External

---
## 0.6.0-mp (Feb-15-2022)
* Added support for sending numeric data values to the simulator with the various "Set" actions.
  * Most/all "Set" actions have been broken out into their own separate action types. Since they now have a data field, it doesn't make sense to pair them with actions which don't use them, such as "Increment."
    * Buttons which did *not* use the old "Set" choices, such as "Increment," etc, will continue to work (although they will still show the old choices in existing TP buttons until replaced with the new versions). However any buttons which relied on the old behavior of the "Set" action value always being `0` (zero) will need to be updated.
  * Simple arithmetic operations (`+`, `-`, `*`, `/`, `%`, precedence with `( )`, scientific notation) are supported in most of the data fields which can be sent to the sim.
That means you can do things like `25 * 30`, but more interestingly one could use states/variables, like:
    `"AP Set Alt. Hold to:" ${value:MSFSTouchPortalPlugin.AutoPilot.State.AutoPilotAltitudeVar} + 1000`
This evaluation could be expanded upon later if there is a need (to include higher math, rounding, etc).
* Generated documentation updated to include all Action Data attributes and mappings to the actual SimConnect events. Also added SimVar names to the list of States.
* New TP UI color for MSFS Plugin actions, "MSFS Blue."
* Action-to-event mapping syntax/scheme somewhat simplified and consolidated, for easier additions and maintenance. Now also allows custom event names (with a dot).
* Added new actions:
  * AP Set Airspeed Hold Value
  * AP Set Altitude Hold Value
  * AP Set Heading Hold Value
  * AP Set Mach Hold Value
  * AP Set Vertical Speed Hold Value
  * Magneto Switch 1/2/3/4 Set Position (0-4)
  * Propeller All/1/2/3/4 Set Pitch Lever Value (0 to 16383)
  * Throttle All/1/2/3/4 Set (-16383 - +16383)
  * Engine 1/2/3/4 Anti-ice Switch Set On/Off
  * Aileron Trim Set % (-100 - +100)
  * Ailerons Position Set (-16383 - +16383)
  * Elevator Trim Set (0 - 16383)
  * Elevator Position Set (-16383 - +16383)
  * Elevator Increment Up/Down
  * Rudder Trim Set % (-100 - +100)
  * Rudder Position Set (-16383 - +16383)
  * Flaps Position Set (0 - 16383)
  * Spoilers Position Set (0 - 16383)
* Modified actions:
  * Toggle Flight Director removed useless choice selector with the one "toggle" option (there are no SimConnect events for specific on/off), **BREAKS CURRENT BUTTONS**
  * Vertical Speed Hold added On/Off choices (in addition to Toggle which was the only one)
  * Vertical Speed Value removed "Set Current" choice since there's no such Sim event to map to
  * AP Nav Mode Switch 1/2, which never worked, now takes a value of 1 or 2 (that's how we have to set it up for now)
  * Toggle All Magnetos - added new "Select for +/-" choice
  * Engine Anti Ice Toggle removed Set choice (new action, above) which left only Toggle as 2nd choice, so that was removed **BREAKS CURRENT BUTTONS**
  * Held Action Repeat Rate changed entirely, can now be Set to a specific value or Increment/Decrement by any step amount (in ms)  **BREAKS CURRENT BUTTONS**
* Modified states:
  * Plugin.State.Connected added "connecting" status which is active while SimConnect is not connected but is trying to be. "false" now indicates that it is disconnected and no attempts are being made to connect.  **POSSIBLY BREAKS CURRENT BUTTONS** if they rely on the "false" status.

For some hints on using the new Set commands, check out the wiki page [Tips And Tricks For Setting Simulator Values](https://github.com/mpaperno/MSFSTouchPortalPlugin/wiki/Tips-And-Tricks-For-Setting-Simulator-Values).

---
## 0.5.4-mp (Feb-08-2022)
* Added support for "On Hold" type Touch Portal actions with a configurable repeat time.
  All current actions which may make sense to repeat (such as for control surfaces or AP adjustments) should now be available in the "On Hold" TP button configuration page.
  The generated documentation now shows which actions can be held.
  - Note that "On Hold" actions do _not_ trigger upon first button press, you need to  configure an "On Pressed" action as well, which is a bit more setup but is more flexible
  in case a single press should do some different action.
* Added support for Touch Portal plugin Settings (in the TP _Settings -> Plug-ins -> MSFSTouchPortalPlugin_ page).
* Added setting to control automatic connection to SimConnect upon TP (or plugin) startup. When disabled, connection must be made manually via the existing Connect action.
* Fixed issue with re-connecting to SimConnect automatically despite user's Disconnect/Toggle Off action.
* Fixed text values like ATC ID and Aircraft Title not updating properly after the first time. [#42](https://github.com/tlewis17/MSFSTouchPortalPlugin/issues/42)
* Fixed/changed light switch states to only reflect switch status, not the light OR switch being on. [#5](https://github.com/tlewis17/MSFSTouchPortalPlugin/issues/5)
* Fixed that Elevator Trim Position (degrees) was actually reporting the percentage-open value (percents also added, see below).
* More robust recovery (reconnection) in case of (some) unexpected SimConnect errors.
* Fixed generated documentation to include all TP States per category. It now also includes settings information.
* Less verbose logging by default, with more control via `appsettings.json` file (changes in config reflected w/out restarting the plugin).
* General performance and stability improvements.
* Added new actions:
  - Propeller Pitch Adjust All/1/2/3/4 Incr/Incr Small/Decr/Decr Small/Min/Max/Feather Switch
  - Selected value (knob) increase/decrease (eg. for autopilot settings, same as +/- keys)
  - Held Action Repeat Rate, Increment/Decrement 50ms or choose one of several presets.
* Added new states:
  - Ele/Ail/Rud Trim angles in Percent
  - Cowl Flaps 1-4 Opened Percentage
  - Propeller 1-4 Feather Switch and Feathered State
  - Deice and Pitot Heat switch states for Panel, Structural, Windshield, and Pitot 1-4
  - Current Held Action Repeat Rate (in ms)

---
## 0.5.3 (10-14-2020)
* Added new states:
  * AtcType - Type of aircraft used by ATC
  * AtcModel - Model of aircraft used by ATC
  * AtcId - Aircraft Id used by ATC
  * AtcAirline -- Airline used by ATC
  * AtcFlightNumber - Flight Number used by ATC
  * AircraftTitle - Aircraft Title

---
## 0.5.2 (10-13-2020)
* Modified states to not have units. These can be added through Touch Portal if user chooses.
* De-Ice engine actions have been combined. Will require re-configuring on Page.

### New/Modified Actions
* Flaps - Up, Down, Increase, Decrease, 1, 2, 3
* Parking Brake - Toggle
* Rudder Trim - Left / Right
* Elevator Trim - Up / Down
* Aileron Trim - Left / Right
* Throttle All Engines - Full, Increase, Increase Small, Decrease, Decrease Small, Cut, 10%-90%
* Throttle Specific - Full, Increase, Increase Small, Decrease, Decrease Small, Cut
* Anti-Ice - Toggle / On / Off
* Anti-Ice Engine - Toggle for engines 1, 2, 3, 4
* PitotHeat - Toggle / On / Off

### New/Modified States
* Aileron Trim
* Elevator Trim
* Flaps Handle Percentage
* Parking Brake Indicator
* Rudder Trim
* MixtureEngine1
* MixtureEngine2
* MixtureEngine3
* MixtureEngine4
* PropellerEngine1
* PropellerEngine2
* PropellerEngine3
* PropellerEngine4
* RPMN1Engine1
* RPMN1Engine2
* RPMN1Engine3
* RPMN1Engine4
* ThrottleEngine1
* ThrottleEngine2
* ThrottleEngine3
* ThrottleEngine4
* AntiIceEng1
* AntiIceEng2
* AntiIceEng3
* AntiIceEng4
* PitotHeat
* AirSpeedIndicated
* AirSpeedMach
* AirSpeedTrue
* FlapSpeedExceeded
* OverspeedWarning
* StallWarning
* VerticalSpeed

---
## 0.5.1 (10-9-2020)
Entry.tp file is now properly alphabetized.

* New States:
  * SimulationRate
  * Com1ActiveFrequency
  * Com1StandbyFrequency
  * Com2ActiveFrequency
  * Com2StandbyFrequency
  * Nav1ActiveFrequency
  * Nav1StandbyFrequency
  * Nav2ActiveFrequency
  * Nav2StandbyFrequency
* New Actions:
  * Simulation Rate - Increase / Decrease
  * Radio Interaction
    * COM1/COM2/NAV1/NAV2 - Increase/Decrease 25 KHz/1 MHz / Carry Digits / Standby Swap

* Fixed Auto Throttle Go Around, it was previously sharing a state value with the Auto Throttle Arm.

---
## 0.5.0 (9-27-2020)
* Added Vertical Speed Hold Toggle
* Fixed plugin connected status states
* Improved startup and resiliency

---
## 0.4.0 (9-15-2020)
* Now only allows for a single instance of the executable to run.
* Exe will keep running. Will properly disconnect from MSFS when you quit MSFS and then you can start up the simulator again and it will reconnect without having to restart the plugin.
* Aileron/Rudder/Elevator values
* More Autopilot related button states
* Landing gear state - 1 means extended.

---
## 0.3.0 (9-11-2020)
More controls, many button states, and flight instrument variables.

Polling of state data is done at 250ms intervals. Should update quick enough.

---
## 0.2.2 (9-8-2020)
Fixed path for the plugin start command.

---
## 0.2.1 (9-7-2020)
Added Yaw Dampener controls.

---
## 0.2.0 (9-7-2020)
Expanded with lots of controls for Flight Control Systems and Electrical.

* Trimming
* Flaps
* Brakes
* Gear
* Lights
* Additional Auto Pilot options and settings
* More!

---
## 0.1.0 (9-7-2020)
This release is primarily as a test run. It Supports a handful of Autopilot functions and a Fuel Selector.
AddFuel doesn't work at the moment.
