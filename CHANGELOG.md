# MSFS Touch Portal Plugin

## &gt; Next (unreleased) version &lt;
* Added support for sending numeric data values to the simulator with the various "Set" actions. [#55](https://github.com/tlewis17/MSFSTouchPortalPlugin/pull/55)
  * Most/all "Set" actions have been broken out into their own separate action types. Since they now have a data field, it doesn't make sense to pair them with actions which don't use them, such as "Increment."
    * Buttons which did *not* use the old "Set" choices, such as "Increment," etc, will continue to work (although they will still show the old choices in existing TP buttons until replaced with the new versions). 
    However any buttons which relied on the old behavior of the "Set" action value always being `0` (zero) will need to be updated.
  * Simple arithmetic operations (`+`, `-`, `*`, `/`, `%`, precedence with `( )`, scientific notation) are supported in most of the data fields which can be sent to the sim. 
That means you can do things like `25 * 30`, but more interestingly one could use states/variables, like:  
    `"AP Set Alt. Hold to:" ${value:MSFSTouchPortalPlugin.AutoPilot.State.AutoPilotAltitudeVar} + 1000`  
This evaluation could be expanded upon later if there is a need (to include higher math, rounding, etc).
  * For some hints on using the new Set commands, check out the wiki page
  [Tips And Tricks For Setting Simulator Values](https://github.com/tlewis17/MSFSTouchPortalPlugin/wiki/Tips-And-Tricks-For-Setting-Simulator-Values).
* Added support for "On Hold" type Touch Portal actions with a configurable repeat time.  [#49](https://github.com/tlewis17/MSFSTouchPortalPlugin/pull/49)
  * All current actions which may make sense to repeat (such as for control surfaces or AP adjustments) should now be available in the "On Hold" TP button configuration page.
  * The generated documentation now shows which actions can be held.
  * Note that "On Hold" actions do _not_ trigger upon first button press, you need to  configure an "On Pressed" action as well, which is a bit more setup but is more flexible
  in case a single press should do some different action.
* Added support for Touch Portal plugin Settings (in the TP _Settings -> Plug-ins -> MSFSTouchPortalPlugin_ page).  [#49](https://github.com/tlewis17/MSFSTouchPortalPlugin/pull/49)
* Added setting to control automatic connection to SimConnect upon TP (or plugin) startup. When disabled, connection must be made manually via the existing Connect action.  [#49](https://github.com/tlewis17/MSFSTouchPortalPlugin/pull/49)
* Fixed text values like ATC ID and Aircraft Title not updating properly after the first time. [#42](https://github.com/tlewis17/MSFSTouchPortalPlugin/issues/42) [#43](https://github.com/tlewis17/MSFSTouchPortalPlugin/pull/43)
* Fixed/changed light switch states to only reflect switch status, not the light OR switch being on. [#5](https://github.com/tlewis17/MSFSTouchPortalPlugin/issues/5) [#45](https://github.com/tlewis17/MSFSTouchPortalPlugin/pull/45)
* Fixed that Elevevator Trim Position (degrees) was actually reporting the percentage-open value (percents also added, see below). [#48](https://github.com/tlewis17/MSFSTouchPortalPlugin/pull/48)
* Fixed issue with re-connecting to SimConnect automatically despite user's Disconnect/Toggle Off action. [#44](https://github.com/tlewis17/MSFSTouchPortalPlugin/pull/44)
* More robust recovery (reconnection) in case of (some) unexpected SimConnect errors. [#44](https://github.com/tlewis17/MSFSTouchPortalPlugin/pull/44)
* ![msfs-blue](https://user-images.githubusercontent.com/1366615/153997641-e0bf23c4-2a19-4844-aa0a-53be8954119f.png) New TP UI color for MSFS Plugin actions, "MSFS Blue."  [#55](https://github.com/tlewis17/MSFSTouchPortalPlugin/pull/55)

* Documentation
  * Fixed generated documentation to include all TP States per category. It now also includes settings information.
  * Documentation updated to include all Action Data attributes and mappings to the actual SimConnect events. Also added the corresponding SimVar names to the list of States.

* Internals
  * Less verbose logging and maximum log retention of 7 days by default, with more control via `appsettings.json` file (changes in config reflected w/out restarting the plugin). [#51](https://github.com/tlewis17/MSFSTouchPortalPlugin/pull/51)
  * General performance and stability improvements. [#47](https://github.com/tlewis17/MSFSTouchPortalPlugin/pull/47)
  * Action-to-event mapping syntax/scheme somewhat simplified and consolidated, for easier additions and maintenance. [#55](https://github.com/tlewis17/MSFSTouchPortalPlugin/pull/55)

* New Actions:
  * Avionics Master State Toggle
  * AP Set Airspeed Hold Value
  * AP Set Altitude Hold Value
  * AP Set Heading Hold Value
  * AP Set Mach Hold Value
  * AP Set Vertical Speed Hold Value
  * Selected value (knob) increase/decrease (eg. for autopilot settings, same as +/- keys)
  * Magneto Switch 1/2/3/4 Set Position (0-4)
  * Propeller All/1/2/3/4 Set Pitch Lever Value (0 to 16383)
  * Propeller Pitch Adjust All/1/2/3/4 Incr/Incr Small/Decr/Decr Small/Min/Max/Feather Switch
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
  * Held Action Repeat Rate, Increment/Decrement 50ms or choose one of several presets.

* Modified actions:
  * Toggle Flight Director removed useless choice selector with the one "toggle" option (there are no SimConnect events for specific on/off), **BREAKS CURRENT BUTTONS**
  * Vertical Speed Hold added On/Off choices (in addition to Toggle which was the only one)
  * Vertical Speed Value removed "Set Current" choice since there's no such Sim event to map to
  * AP Nav Mode Switch 1/2, which never worked, now takes a value of 1 or 2 (that's how we have to set it up for now)
  * Toggle All Magnetos - added new "Select for +/-" choice
  * Engine Anti Ice Toggle removed Set choice (new action, above) which left only Toggle as 2nd choice, so that was removed **BREAKS CURRENT BUTTONS**

* New states:
  * Avionics Master Switch state (0/1)
  - Ele/Ail/Rud Trim angles in Percent
  - Cowl Flaps 1-4 Opened Percentage
  - Propeller 1-4 Feather Switch and Feathered State
  - Deice and Pitot Heat switch states for Panel, Structural, Windshield, and Pitot 1-4
  - Current Held Action Repeat Rate (in ms)

* Modified states:
  * Plugin.State.Connected added "connecting" status which is active while SimConnect is not connected but is trying to be. "false" now indicates that it is disconnected and no 
    attempts are being made to connect.  **POSSIBLY BREAKS CURRENT BUTTONS** if they rely on the "false" status. [#63](https://github.com/tlewis17/MSFSTouchPortalPlugin/pull/63)


## 0.5.3 (10-14-2020)
* Added new states:
  * AtcType - Type of aircraft used by ATC
  * AtcModel - Model of aircraft used by ATC
  * AtcId - Aircraft Id used by ATC
  * AtcAirline -- Airline used by ATC
  * AtcFlightNumber - Flight Number used by ATC
  * AircraftTitle - Aircraft Title

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

## 0.5.0 (9-27-2020)
* Added Vertical Speed Hold Toggle
* Fixed plugin connected status states
* Improved startup and resiliency

## 0.4.0 (9-15-2020)
* Now only allows for a single instance of the executable to run.
* Exe will keep running. Will properly disconnect from MSFS when you quit MSFS and then you can start up the simulator again and it will reconnect without having to restart the plugin.
* Aileron/Rudder/Elevator values
* More Autopilot related button states
* Landing gear state - 1 means extended.

## 0.3.0 (9-11-2020)
More controls, many button states, and flight instrument variables.

Polling of state data is done at 250ms intervals. Should update quick enough.

## 0.2.2 (9-8-2020)
Fixed path for the plugin start command.

## 0.2.1 (9-7-2020)
Added Yaw Dampener controls.

## 0.2.0 (9-7-2020)
Expanded with lots of controls for Flight Control Systems and Electrical.

* Trimming
* Flaps
* Brakes
* Gear
* Lights
* Additional Auto Pilot options and settings
* More!

## 0.1.0 (9-7-2020)
This release is primarily as a test run. It Supports a handful of Autopilot functions and a Fuel Selector.
AddFuel doesn't work at the moment.