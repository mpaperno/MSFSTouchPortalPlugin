# MSFS 2020 TouchPortal Plugin - MP fork

[![verify-build](https://github.com/mpaperno/MSFSTouchPortalPlugin/actions/workflows/verify-build.yml/badge.svg)](https://github.com/mpaperno/MSFSTouchPortalPlugin/actions/workflows/verify-build.yml)
[![GitHub release (latest by date including pre-releases)](https://img.shields.io/github/v/release/mpaperno/MSFSTouchPortalPlugin?include_prereleases)](https://github.com/mpaperno/MSFSTouchPortalPlugin/releases)
[![Downloads](https://img.shields.io/github/downloads/mpaperno/MSFSTouchPortalPlugin/total.svg)](https://github.com/mpaperno/MSFSTouchPortalPlugin/releases)
[![Downloads of latest release](https://img.shields.io/github/downloads/mpaperno/MSFSTouchPortalPlugin/latest/total)](https://github.com/mpaperno/MSFSTouchPortalPlugin/releases/latest)
[![Stars](https://img.shields.io/github/stars/mpaperno/MSFSTouchPortalPlugin)](https://github.com/mpaperno/MSFSTouchPortalPlugin/stargazers)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

Please note you're on the main branch for this fork (version). This README is specific to this version and repository. The `master` branch will stay synced with the original.

## Overview

This plugin will provide a two way interface between Touch Portal and Microsoft Flight Simulator 2020 through SimConnect. This may work for other simulators that use SimConnect.
A good starting point for pages would be to look at FordMustang0288's repo: [MSFSTouchPortalPages](https://github.com/FordMustang0288/MSFSTouchPortalPages).
Another good page for use is created by TaxTips at: [FltSim-msfs2020-Control](https://github.com/HiDTH/FltSim-msfs2020-Control).
A (slightly dated) installation and usage guide is available at _Simvol_ [How to use Touch Portal [with MSFS]](https://www.simvol.org/en/articles/tutorials/use-touch-portal).


## Features

* Connects automatically through SimConnect
* Allows getting data variables from MSFS such as flight instruments or button/switch states.
* Allows triggering various aircraft components from a Touch Portal panel.

## Documentation

Auto-generated documentation on all actions, states, and settings can be found in [DOCUMENTATION.MD](DOCUMENTATION.MD).

A [nascent wiki](https://github.com/tlewis17/MSFSTouchPortalPlugin/wiki/) is available with some tips.

## Installation & Usage

1. Get the latest release of this plugin from the  [Releases](https://github.com/mpaperno/MSFSTouchPortalPlugin/releases) page.
2. The plugin is distributed and installed as a standard Touch Portal `.tpp` plugin file. If you know how to import a plugin,
just do that and skip to step 4. There is also a [short guide](https://www.touch-portal.com/blog/post/tutorials/import-plugin-guide.php) on the Touch Portal site.
3. Import the plugin:
    1. Start _Touch Portal_ (if not already running).
    2. Click the "gear" icon at the top and select "Import plugin..." from the menu.
    3. Browse to where you downloaded this plugin's `.tpp` file and select it.
4. Restart _Touch Portal_
    * When prompted by _Touch Portal_ to trust the plugin startup script, select "Yes" (the source code is public!).


After that you will have a list of new actions you can choose from. Also "Dynamic Text" variables are now available. You can see them from the Dynamic Text Updater,
or you can add an option for "On Plugin State Change" then select the corresponding state and "Changes to".

For buttons you use this like:

"On Plugin State Changes AutoPilot Master changes to 1" Then add whatever logic you want like button visuals. Duplicate this and add one for "does not change to 1" and that is when a button turns off.

This is very much a work in progress!

A comprehensive installation and usage guide was published on the _Simvol_ Web site: [How to use Touch Portal [with MSFS]](https://www.simvol.org/en/articles/tutorials/use-touch-portal).

Sample Touch Portal page files can be found at FordMustang0288's [MSFSTouchPortalPages](https://github.com/FordMustang0288/MSFSTouchPortalPages)
and HiDTH's [FltSim-msfs2020-Control](https://github.com/HiDTH/FltSim-msfs2020-Control)

## Update Notifications

The latest version of this software is always published on the GitHub [Releases](https://github.com/mpaperno/MSFSTouchPortalPlugin/releases) page.
GitHub publishes an "Atom" (like RSS) feed with the latest version of any repository by adding `.atom` to the end o the URL. This reposotiry's
release feed URL is:<br/>
https://github.com/mpaperno/MSFSTouchPortalPlugin/releases.atom

You have several options for getting **automatically notified** about new releases:
* **If you have a GitHub account**, just open the _Watch_ menu of this repo in the top right of this page, then go to  _Custom_ and select the
_Releases_ option, then hit _Apply_ button.
* **If you already use an RSS/Atom feed reader**, just subscribe to the feed URL shown above.
* **Use an RSS/Atom feed notification service**, either one specific for GitHub or a generic one, such as
(a list of services I found, I haven't necessarily tried nor do I endorse any of these):
  * https://blogtrottr.com/  (generic RSS feed notifications, no account required, use feed URL shown above)
  * https://coderelease.io/  (no account required)
  * https://newreleases.io/
  * https://gitpunch.com/

I will also post update notices in the Touch Portal Discord server room [#msfs2020](https://discord.com/channels/548426182698467339/750791488501448887)

## Support and Discussion

Please use the GitHub [Issues](https://github.com/mpaperno/MSFSTouchPortalPlugin/issues) pages for bug reports and concise feature requests.
Use the [Discussions](https://github.com/mpaperno/MSFSTouchPortalPlugin/discussions) pages for general conversation on any related topic like suggestions or support questions.

There is also a Touch Portal Discord server discussion room at [#msfs2020](https://discord.com/channels/548426182698467339/750791488501448887)

## Related Plugin(s)

My [TJoy Touch Portal Plugin](https://github.com/mpaperno/TJoy) is an interface between Touch Portal and several virtual joystick/gamepad emulation drivers like _vJoy_, _vXBox_, and _ViGEm Bus_.

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
