# MSFS 2020 TouchPortal Plugin

<!--![.NET Core](https://github.com/tlewis17/TouchPortalAPI/workflows/.NET%20Core/badge.svg?branch=master)-->
[![GitHub release (latest by date including pre-releases)](https://img.shields.io/github/v/release/tlewis17/MSFSTouchPortalPlugin?include_prereleases)](https://github.com/tlewis17/MSFSTouchPortalPlugin/releases)
[![Downloads](https://img.shields.io/github/downloads/tlewis17/MSFSTouchPortalPlugin/total.svg)](https://github.com/tlewis17/MSFSTouchPortalPlugin/releases)
[![Stars](https://img.shields.io/github/stars/tlewis17/MSFSTouchPortalPlugin)](https://github.com/tlewis17/MSFSTouchPortalPlugin/stargazers)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.md)

## Overview

This plugin will provide a two way interface between Touch Portal and Microsoft Flight Simulator 2020 through SimConnect. This may work for other simulators that use SimConnect such as X-Plane 11. A good starting point for pages would be to look at FordMustang0288's repo: [MSFSTouchPortalPages](https://github.com/FordMustang0288/MSFSTouchPortalPages)

## Features

* Connects automatically through SimConnect
* Allows getting data variables from MSFS such as flight instruments or button/switch states.
* Allows triggering various aircraft components from a Touch Portal panel.

## Documentation

Events and states available are documented in the readme below. Automated Documentation can be found here: [link](DOCUMENTATION.md).

## Installation Guide

Go to the releases:
https://github.com/tlewis17/MSFSTouchPortalPlugin/releases

Get the latest version and there will be a TPP file you can download. From Touch Portal go to Import Plugin. Once you have done that restart Touch Portal. After that you will have a list of new actions you can choose from. Also "Dynamic Text" variables are now available. You can see them from the Dynamic Text Updater, or you can add an option for "On Plugin State Change" then select the corresponding state and "Changes to".

For buttons you use this like:

"On Plugin State Changes AutoPilot Master changes to 1" Then add whatever logic you want like button visuals. Duplicate this and add one for "does not change to 1" and that is when a button turns off.

This is very much a work in progress!
Sample TML files can be found by FordMustang:
https://github.com/FordMustang0288/MSFSTouchPortalPages

## TODO

* Flight Instruments
  * Pitch/Bank need to be times by -1.
* Documentation - Events needed and additional rows of data to display.


**Documentation revamp**
* Create interactive documentation site.
* Also generate base doc data into JSON file from code.
* Version selector
* Note tested/not tested
* Note Sim compatibility, may be useful on tested/not tested
* Filter by category
* Search
* Filter by Sim

## Known Issues

* "Set" buttons require data to be passed in and do not currently work.

## Available Controls

### AutoPilot

#### Actions

* Airspeed Hold - (Not Tested) - This will enable/disable the airspeed hold and set to the current airspeed.
  * Toggle / On /Off / Set (DNU - TODO)
* Airspeed Hold Value - (Not Tested) - Used to modify the current value of the airspeed hold.
  * Select / Increase / Decrease / Set (DNU - TODO)
* Altitude Hold -  (Tested) - This will enable/disable the altitude hold and set to the current altitude.
  * Toggle / On / Off
* Altitude Hold Value - (Tested) - used to modify the curent value of the altitude hold.
  * Select / Increase / Decrease / Set (DNU - TODO) / Set Metric (DNU - TODO)
* AP Max Bank Angle - (Not Tested) - Used to set the max bank angle for auto pilot.
  * Increase / Decrease
* Approach Mode - (Tested) - This will enable/disable the approach mode.
  * Toggle / On / Off
* Attitude Hold - (Not Tested) - This will enable/disable the attitude hold.
  * Toggle / On / Off
* Attitude Hold Value - (Not Tested) - Used to set the attitude hold value.
  * Increase / Decrease / Select
* Auto Brake - (Not Tested) - Increases/Decreases the auto brake value.
  * Increase / Decrease
* Auto Throttle Mode - (Not Tested) - Sets the auto throttle mode.
  * Arm / GoAround
* AutoPilot - (Tested) - Enables/Disables the Auto Pilot.
  * Toggle / On / Off
* Back Course Mode - (Tested) - Enables/Disables the back course mode.
  * Toggle / On / Off
* Flight Director - (Not Tested) - Toggles the Flight Director mode.
  * Toggle
* Flight Director Pitch Sync - (Not Tested) - Sets the FD Pitch Sync to the current pitch.
* Heading Hold - (Tested) - Will enable/disable the heading hold and will set the value to the current heading.
  * Toggle / On / Off
* Heading Hold Value - (Tested) - Used to modify the heading hold value.
  * Increase / Decrease / Select / Set (DNU - TODO)
* Localizer - (Not Tested) - Will enable/disable the localizer.
  * Toggle / On / Off
* Mach Hold - (Not Tested) - Will enable/disable the mach hold and set to current mach value.
  * Toggle / On / Off
* Mach Hold Value - (Not Tested) - Used to set the mach hold value.
  * Increase / Decrease / Select / Set (DNU - TODO)
* Nav Mode - Set - (Not Tested) - Sets the nav mode to the specified nav radio.
  * 1 / 2
* Nav1 Mode - (Not Tested) - Sets the auto pilot to Nav mode.
  * Toggle / On / Off
* Vertical Speed - (Tested) - Will enable the vertical speed hold.
  * Toggle
* Vertical Speed Value - (Tested) - Used to increase/decrease the vertical speed value.
  * Increase / Decrease / Select / Set (DNU - TODO) / Set Metric (DNU - TODO)
* Wing Leveler - (Not Tested) - Used to enable/disable the wing leveler.
  * Toggle / On / Off
* Yaw Dampener - (Not Tested) - Used to enable/disable the yaw dampener.
  * Toggle / On / Off / Set (DNU - TODO)

#### States

* Auto Throttle Armed - (Not Tested) - 1/0
* Auto Thorttle GoAround - (Not Tested) - 1/0
* Auto Pilot Air Speed Status - (Not Tested) - 1/0
* Auto Pilot Air Speed Value - (Not Tested) - In Knots


| Control | State Variable(s) | Modes | v1/v2 | Tested |
| --- | --- | --- | --- | --- |
| - | AutoPilotAvailable | - | v2 | Yes |
| AP Flight Level Control TBD | AutoPilotPitchHold | TBD | v1 | ?? |

### Communications

#### Actions

* Radio Interaction (Tested)
  * Select Radio to interact with - COM1/COM2/NAV1/NAV2
  * Action to perform on radio - Increase/Decrease 25 KHz/1 MHz / Carry Digits / Standby Swap

  **NOTE** Decrease /w Carry works for NAV1/2, but not COM1/2, appears to be MSFS or SimConnect bug.

#### States

* Com1ActiveFrequency - (Tested) - ###.###
* Com1StandbyFrequency - (Tested) - ###.###
* Com2ActiveFrequency - (Tested) - ###.###
* Com2StandbyFrequency - (Tested) - ###.###
* Nav1ActiveFrequency - (Tested) - ###.###
* Nav1StandbyFrequency - (Tested) - ###.###
* Nav2ActiveFrequency - (Tested) - ###.###
* Nav2StandbyFrequency - (Tested) - ###.###


### Failures

| Control | State Variable(s) | Modes | v1/v2 | Tested |
| --- | --- | --- | --- | --- |
| Toggle Failure - X | - | Electrical, Vacuum, Pitot, Static Port, Hydraulic, Total Brake, Left Brake, Right Brake, Engine 1 - 4 | v2 | |

### Flight Systems

| Control | State Variable(s) | Modes | v1/v2 | Tested |
| --- | --- | --- | --- | --- |
| Ailerons | TBD | Center, Left, Right, Set | v1 | |
| Brakes | TBD | All, Left, Right | v1 | |
| Cowl Flaps All | TBD | Increase, Decrease | v1 | |
| Cowl Flaps 1 | TBD | Increase, Decrease | v1 | |
| Cowl Flaps 2 | TBD | Increase, Decrease | v1 | |
| Cowl Flaps 3 | TBD | Increase, Decrease | v1 | |
| Cowl Flaps 4 | TBD | Increase, Decrease | v1 | |
| Gear | GearTotalExtended | Toggle, Up, Down, Set, Pump | v2 | |
| Rudder | TBD | Center, Left, Right, Set | v1 | |
| Spoilers | TBD | Toggle, On, Off, Set | v1 | |
| Spoilers Arm | TBD | Toggle, On, Off, Set | v1 | |

#### Actions

* Flaps - (Tested) - Changes the position of the flaps
  * Up / Down / Increase / Decrease / 1 / 2 / 3
* Toggle Parking Brake - (Tested) - Toggles the parking brake on or off
* Rudder Trim - (Tested) - Left / Right
* Elevator Trim - (Tested) - Up / Down
* Aileron Trim - (Tested) - Left / Right

#### States

* Aileron Trim - (Tested) - Percent
* Elevator Trim - (Tested) - Percent
* FlapsHandlePercent - (Tested) - Percent
* ParkingBrakeIndicator - (Tested) - True/False
* RudderTrim - (Tested) - Percent

### Electrical

| Control | State Variable(s) | Modes | v1/v2 | Tested |
| --- | --- | --- | --- | --- |
| Master Alternator | MasterAlternator | Toggle | v2 | |
| Master Battery | MasterBattery | Toggle | v2 | |
| Master Battery & Alternator | MasterAlternator / MasterBattery | Toggle | v2 | |
| Toggle Specific Alternator | TBD | 1, 2, 3, 4 | v1 | |
| Strobe Lights | LightStrobeOn | Toggle, On, Off, Set | v1 | |
| Panel Lights | LightPanelOn | Toggle, On, Off, Set | v1 | |
| Landing Lights | LightLandingOn | Toggle, On, Off, Set | v1 | |
| All Lights - Toggle All/Specific | - | All, Beacon, Taxi, Logo, Recognition, Wing, Nav, Cabin | v1 | |

Extra States for Lights:

| State | Light | v1\v2 | Tested |
| --- | --- | --- | --- | --- |
| LightBeaconOn | Beacon Light | v1 | |
| LightBrakeOn | Brake Light | v1 | |
| LightCabinOn | Cabin Light | v1 | |
| LightHeadOn | Head Light | v1 | |
| LightLandingOn | Landing Light | v1 | |
| LightLogoOn | Logo Light | v1 | |
| LightNavOn | Nav Light | v1 | |
| LightPanelOn | Panel Light | v1 | |
| LightRecognitionOn | Recognition Light | v1 | |
| LightStrobeOn | Strobe Light | v1 | |
| LightTaxiOn | Taxi Light | v1 | |
| LightWingOn | Wing Light | v1 | |

### Engine

| Control | State Variable(s) | Modes | v1/v2 | Tested |
| --- | --- | --- | --- | --- |
| Ignition | MasterIgnitionSwitch | Toggle | v2 | |
| Engine Auto Start/Shutdown | TBD | Start, Shutdown | v1 | Start/Stop works, no state. |
| Magneto - All | TBD | Start, Off, Right, Left, Both, Decrease, Increase | v1 | |
| Magneto - 1 | TBD | Start, Off, Right, Left, Both, Decrease, Increase | v1 | |
| Magneto - 2 | TBD | Start, Off, Right, Left, Both, Decrease, Increase | v1 | |
| Magneto - 3 | TBD | Start, Off, Right, Left, Both, Decrease, Increase | v1 | |
| Magneto - 4 | TBD | Start, Off, Right, Left, Both, Decrease, Increase | v1 | |
| Toggle Starters | TBD | All, 1, 2, 3, 4 | v1 | |

#### Actions

* Throttle (All Engines) (Partial Test) - Full, Increase, Increase Small, Decrease, Decrease Small, Cut, Set, 10%, 20%, 30%, 40%, 50%, 60%, 70%, 80%, 90%
    * Do not use Set, all "Set" commands for all actions are not implemented.
* Throttle Specific (Not Tested) - Full, Increase, Increase Small, Decrease, Decrease Small, Cut

#### States

* MixtureEngine1 - (Tested) - Percentage
* MixtureEngine2 - (Tested) - Percentage
* MixtureEngine3 - (Not Tested) - Percentage
* MixtureEngine4 - (Not Tested) - Percentage
* PropellerEngine1 - (Tested) - Percentage
* PropellerEngine2 - (Tested) - Percentage
* PropellerEngine3 - (Not Tested) - Percentage
* PropellerEngine4 - (Not Tested) - Percentage
* RPMN1Engine1 - (Tested) - RPM
* RPMN1Engine2 - (Tested) - RPM
* RPMN1Engine3 - (Tested) - RPM
* RPMN1Engine4 - (Tested) - RPM
* ThrottleEngine1 - (Tested) - Percentage
* ThrottleEngine2 - (Tested) - Percentage
* ThrottleEngine3 - (Tested) - Percentage
* ThrottleEngine4 - (Tested) - Percentage

### Environment

#### Actions

* Anti-Ice - (Not Tested) - Toggles / On / Off Anti-Ice
* Anti-Ice Engine - (Not Tested) - Toggles Anti-Ice for engines 1, 2, 3, or 4.
* Propeller De-Ice - (Not Tested) - Toggle
* Structural De-Ice - (Not Tested) - Toggle
* Pitot Heat - (Not Tested) - Toggle / On / Off Pitot Heat

#### States

* AntiIceEng1 - (Not Tested) - True/False
* AntiIceEng2 - (Not Tested) - True/False
* AntiIceEng3 - (Not Tested) - True/False
* AntiIceEng4 - (Not Tested) - True/False
* PitotHeat - (Not Tested) - True/False

### Flight Instruments

| Control | State Variable(s) | Modes | v1/v2 | Tested |
| --- | --- | --- | --- | --- |
| - | GroundVelocity | - | v2 | Yes |
| - | PlaneAltitude | - | v2 | Yes |
| - | PlaneAltitudeAGL | - | v2 | Yes |
| - | GroundAltitude | - | v2 | Yes |
| - | PlaneHeadingTrue | - | v2 | Yes |
| - | PlaneHeadingMagnetic | - | v2 | Yes |
| - | PlaneBankAngle | - | v2 | Yes but values are opposite TODO: Flip values |
| - | PlanePitchAngle | - | v2 | Yes but values are opposite TODO: Flip values |

#### Actions

#### States

* AirSpeedIndicated - (Tested) - Knots
* AirSpeedMach - (Tested) - Mach
* AirSpeedTrue - (Tested) - Knots
* FlapSpeedExceeeded - (Not Tested) - True/False
* GroundVelocity - (Tested) - Knots
* GroundAltitude - (Tested) - Feet
* OverspeedWarning - (Tested) - True/False
* PlaneAltitude - (Tested) - Feet
* PlanteAltitudeAGL - (Tested) - Feet
* PlaneBankAngle - (Tested) - Degrees
* PlaneHeadingMagnetic - (Tested) - Degrees
* PlaneHeadingTrue - (Tested) - Degrees
* PlanePitchAngle - (Tested) - Degrees
* StallWarning - (Tested) - True/False
* VerticalSpeed - (Tested) - Feet per minute

### Fuel System

| Control | State Variable(s) | Modes | v1/v2 | Tested |
| --- | --- | --- | --- | --- |
| AddFuel | TBD | - | v1 | |
| Fuel Selector 1 | TBD | All, Off, Left, Right, Left - Aux, Right - Aux, Center, Set | v1 | |
| Fuel Selector 2 | TBD | All, Off, Left, Right, Left - Aux, Right - Aux, Center, Set | v1 | |
| Fuel Selector 3 | TBD | All, Off, Left, Right, Left - Aux, Right - Aux, Center, Set | v1 | |
| Fuel Selector 4 | TBD | All, Off, Left, Right, Left - Aux, Right - Aux, Center, Set | v1 | |
| Primers | TBD | All, 1, 2, 3, 4 | v1 | |
| Fuel Dump | TBD | Toggle | v1 | |
| Cross Feed | TBD | Toggle, Open, Off | v1 | |
| Fuel Valve | TBD | All, 1, 2, 3, 4 | v1 | |
| Fuel Pump | TBD | Toggle | v1 | |
| Electric Fuel Pump | TBD | All, 1, 2, 3, 4 | v1 | |

### SimSystem

#### Actions

* Simulation Rate (Tested)
  * Increase / Decrease

  **NOTE** Decrease /w Carry works for NAV1/2, but not COM1/2, appears to be MSFS or SimConnect bug.

#### States

* SimulationRate - (Tested) - Number
* AtcType - (Tested) - Type of aircraft used by ATC
* AtcModel - (Tested) - Model of aircraft used by ATC
* AtcId - (Tested) - Aircraft Id used by ATC
* AtcAirline - (Tested) - Airline used by ATC
* AtcFlightNumber - (Tested) - Flight Number used by ATC
* AircraftTitle - (Tested) - Aircraft Title

### Plugin

| Control | State Variable(s) | Modes | v1/v2 | Tested |
| --- | --- | --- | --- | --- |
| Connection | Connected | Toggle, On, Off | v1 | |

## References

* [EventIdss](https://docs.microsoft.com/en-us/previous-versions/microsoft-esp/cc526980(v=msdn.10))
* [Variables](https://docs.microsoft.com/en-us/previous-versions/microsoft-esp/cc526981(v=msdn.10))


