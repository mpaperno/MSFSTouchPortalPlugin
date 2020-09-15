# MSFS 2020 TouchPortal Plugin

## Overview

This plugin will provide a two way interface between Touch Portal and Microsoft Flight Simulator 2020 through SimConnect. This may work for other simulators that use SimConnect such as X-Plane 11.

## Features

* Connects automatically through SimConnect
* Allows getting data variables from MSFS such as flight instruments or button/switch states.
* Allows triggering various aircraft components from a Touch Portal panel.

## Documentation

Documentation can be found here: [link](DOCUMENTATION.MD)

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

* States pushed back to Touch Portal
  * Done but need more mappings.
  * Also need to update MD with state info and possible values.
  * Need to force update on first load.
  * Flight Instruments
    * Pitch/Bank need to be times by -1.
  * Reset states on start/end
* Define SimVars
* Documentation - Events needed and additional rows of data to display.

* More Controls
  * VS AutoPilot
  * Communications

## Known Issues

* "Set" buttons require data to be passed in and do not currently work.
* On TP quit, the plugin won't stop itself.
  * This is coded, but not testable while debugging. Need to test with plugin installed.

## Available Controls

### AutoPilot

| Control | State Variable(s) | Modes | v1/v2 | Tested |
| --- | --- | --- | --- | --- |
| AP Master | AutoPilotMaster | Toggle, On, Off | v2 | Yes |
| - | AutoPilotAvailable | - | v2 | Yes |
| AP Altitude Hold | AutoPilotAttitudeHold | Toggle, On, Off | v2 | On/Off working, Toggle not working even with correct id to SimConnect |
| AP Approach Mode | AutoPilotApproachHold | Toggle, On, Off | v2 | Yes |
| AP Heading Hold | AutoPilotHeadingHold | Toggle, On, Off | v2 | Yes |
| AP Heading Var | AutoPilotHeadingVar | Select, Increase, Decrease, Set | v2 | Inc/Dec working. Select/Set do nothing |
| AP Altitude Hold | AutoPilotAltitudeHold | Toggle, On, Off | v2 | |
| AP Altitude Var | AutoPilotAltitudeVar | Select, Increase, Decrease | v2 | |
| AP Back Course Mode | AutoPilotBackCourseHold |  Toggle, On, Off | v2 | |
| AP Nav1 Mode | AutoPilotNav1Hold | Toggle, On, Off | v2 | |
| AP Vertical Speed Hold | AutoPilotVerticalSpeedHold | ? | ? | VS and Vertical Hold might be mixed up. Need to redo this sections. |
| AP Air Speed Hold | AutoPilotAirSpeedHold | Toggle, On, Off, Set | v2 | |
| AP Air Speed Var | AutoPilotAirSpeedVar | Select, Increase, Decrease, Set | v2 | |
| AP Mach Hold | TBD | Toggle, On, Off, Set | v2 | |
| AP Mach Var | TBD | Select, Increase, Decrease | v2 | |
| AP Flight Director | TBD | Toggle | v2 | |
| AP Wing Leveler | TBD | Toggle, On, Off | v2 | |
| AP Localizer | TBD | Toggle, On, Off | v1 | ?? How to handle? |
| AP Yaw Dampener | TBD | Toggle, On, Off, Set | v2 | |
| AP Flight Level Control TBD | AutoPilotPitchHold | TBD | v1 | ?? |

### Failures

| Control | State Variable(s) | Modes | v1/v2 | Tested |
| --- | --- | --- | --- | --- |
| Toggle Failure - X | - | Electrical, Vacuum, Pitot, Static Port, Hydraulic, Total Brake, Left Brake, Right Brake, Engine 1 - 4 | v2 | |

### Flight Systems

| Control | State Variable(s) | Modes | v1/v2 | Tested |
| --- | --- | --- | --- | --- |
| Ailerons | TBD | Center, Left, Right, Set | v1 | |
| Brakes | TBD | All, Left, Right | v1 | |
| Parking Brake | TBD | Toggle | v1 | |
| Flaps | TBD | Up, Down, Increase, Decrease, 1, 2, 3, Set | v1 | |
| Cowl Flaps All | TBD | Increase, Decrease | v1 | |
| Cowl Flaps 1 | TBD | Increase, Decrease | v1 | |
| Cowl Flaps 2 | TBD | Increase, Decrease | v1 | |
| Cowl Flaps 3 | TBD | Increase, Decrease | v1 | |
| Cowl Flaps 4 | TBD | Increase, Decrease | v1 | |
| Gear | TBD | Toggle, Up, Down, Set, Pump | v1 | |
| Rudder | TBD | Center, Left, Right, Set | v1 | |
| Spoilers | TBD | Toggle, On, Off, Set | v1 | |
| Spoilers Arm | TBD | Toggle, On, Off, Set | v1 | |
| Aileron Trim | AileronTrimPct TBD | Left, RIght | v1 | |
| Elevator Trim | TBD | Up, Down | v1 | |
| Rudder Trim | RudderTrimPct TBD | Left, Right | v1 | |

### Communications

| Control | State Variable(s) | Modes | v1/v2 | Tested |
| --- | --- | --- | --- | --- |

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

### Environment

| Control | State Variable(s) | Modes | v1/v2 | Tested |
| --- | --- | --- | --- | --- |
| Anti-Ice | TBD | Toggle, On, Off, Set | v1 | |
| Anti-Ice Engine 1 | TBD | Toggle, On, Off, Set | v1 | |
| Anti-Ice Engine 2 | TBD | Toggle, On, Off, Set | v1 | |
| Anti-Ice Engine 3 | TBD | Toggle, On, Off, Set | v1 | |
| Anti-Ice Engine 4 | TBD | Toggle, On, Off, Set | v1 | |
| Anti-Ice Structural | TBD | Toggle | v1 | |
| Anti-Ice Propeller | TBD | Toggle | v1 | |
| Pitot Head | TBD | Toggle, On, Off, Set | v1 | |

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

### Menu

| Control | State Variable(s) | Modes | v1/v2 | Tested |
| --- | --- | --- | --- | --- |

### Plugin

| Control | State Variable(s) | Modes | v1/v2 | Tested |
| --- | --- | --- | --- | --- |
| Connection | Connected | Toggle, On, Off | v1 | |

## References

* [EventIdss](https://docs.microsoft.com/en-us/previous-versions/microsoft-esp/cc526980(v=msdn.10))
* [Variables](https://docs.microsoft.com/en-us/previous-versions/microsoft-esp/cc526981(v=msdn.10))


