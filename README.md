# MSFS 2020 TouchPortal Plugin - MP fork

[![GitHub release (latest by date including pre-releases)](https://img.shields.io/github/v/release/mpaperno/MSFSTouchPortalPlugin?include_prereleases)](https://github.com/mpaperno/MSFSTouchPortalPlugin/releases)
[![Downloads](https://img.shields.io/github/downloads/mpaperno/MSFSTouchPortalPlugin/total.svg)](https://github.com/mpaperno/MSFSTouchPortalPlugin/releases)
[![Downloads](https://img.shields.io/github/downloads/mpaperno/MSFSTouchPortalPlugin/0.5.4-mp/total)](https://github.com/mpaperno/MSFSTouchPortalPlugin/releases/tag/0.5.4-mp)
[![Stars](https://img.shields.io/github/stars/mpaperno/MSFSTouchPortalPlugin)](https://github.com/Touch-Portal-MSFS/MSFSTouchPortalPlugin/stargazers)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

Please note you're on the main branch for this fork (version). This README is specific to this version and repository. The `master` branch will stay synced with the original.

## Overview

This plugin will provide a two way interface between Touch Portal and Microsoft Flight Simulator 2020 through SimConnect. This may work for other simulators that use SimConnect. A good starting point for pages would be to look at FordMustang0288's repo: [MSFSTouchPortalPages](https://github.com/FordMustang0288/MSFSTouchPortalPages).
Another good page for use is created by TaxTips at: [FltSim-msfs2020-Control](https://github.com/HiDTH/FltSim-msfs2020-Control)

## Features

* Connects automatically through SimConnect
* Allows getting data variables from MSFS such as flight instruments or button/switch states.
* Allows triggering various aircraft components from a Touch Portal panel.

## Documentation

Auto-generated documentation on all actions, states, and settings can be found in [DOCUMENTATION.MD](DOCUMENTATION.MD).

## Installation Guide

Go to the releases:
https://github.com/mpaperno/MSFSTouchPortalPlugin/releases

Get the latest version and there will be a TPP file you can download. From Touch Portal go to Import Plugin. Once you have done that restart Touch Portal. After that you will have a list of new actions you can choose from. Also "Dynamic Text" variables are now available. You can see them from the Dynamic Text Updater, or you can add an option for "On Plugin State Change" then select the corresponding state and "Changes to".

For buttons you use this like:

"On Plugin State Changes AutoPilot Master changes to 1" Then add whatever logic you want like button visuals. Duplicate this and add one for "does not change to 1" and that is when a button turns off.

This is very much a work in progress!
Sample TML files can be found by FordMustang:
https://github.com/FordMustang0288/MSFSTouchPortalPages

## Support and Discussion

Please use the GitHub Issues pages for bug reports and concise feature requests. Use the Discussions pages for general conversation on any related topic like general support questions.

## Known Issues

* "Set" actions require data to be passed in and do not currently work.

## TODO

* Flight Instruments
    * Pitch/Bank need to be times by -1.
* Sliders ("connectors")
* Events?

**Documentation revamp**

* Create interactive documentation site.
* Also generate base doc data into JSON file from code.
* Version selector
* Note tested/not tested
* Note Sim compatibility, may be useful on tested/not tested
* Filter by category
* Search
* Filter by Sim

## References

* [MSFS EventIds](https://docs.microsoft.com/en-us/previous-versions/microsoft-esp/cc526980\(v=msdn.10\))
* [MSFS Variables](https://docs.microsoft.com/en-us/previous-versions/microsoft-esp/cc526981\(v=msdn.10\))
* [FlightSimulator.com SDK Reference](https://docs.flightsimulator.com/html/index.htm#t=Programming_Tools%2FSimVars%2FSimulation_Variables.htm)
