# MSFS/SimConnect Touch Portal Plugin

[![verify-build](https://github.com/mpaperno/MSFSTouchPortalPlugin/actions/workflows/verify-build.yml/badge.svg)](https://github.com/mpaperno/MSFSTouchPortalPlugin/actions/workflows/verify-build.yml)
[![GitHub release (latest by date including pre-releases)](https://img.shields.io/github/v/release/mpaperno/MSFSTouchPortalPlugin?include_prereleases)](https://github.com/mpaperno/MSFSTouchPortalPlugin/releases)
[![Downloads](https://img.shields.io/github/downloads/mpaperno/MSFSTouchPortalPlugin/total.svg)](https://github.com/mpaperno/MSFSTouchPortalPlugin/releases)
[![Downloads of latest release](https://img.shields.io/github/downloads/mpaperno/MSFSTouchPortalPlugin/latest/total)](https://github.com/mpaperno/MSFSTouchPortalPlugin/releases/latest)
[![License](https://img.shields.io/badge/license-GPL3-blue.svg)](LICENSE)

<div align="center">
<img src="https://github.com/mpaperno/MSFSTouchPortalPlugin/wiki/images/logo/banner_top-768x204.png" />
</div>

## Overview

This plugin provided tools to build two-way interactive interfaces between
[Touch Portal](https://www.touch-portal.com/) macro launcher software and Flight Simulators which use SimConnect,
such as Microsoft Flight Simulator 2020 (MSFS) and FS-X. The plugin makes available new Touch Portal
Actions, Connectors, States, and Events for creating buttons and pages suitable for virtually any
simulated aircraft, component, or system.

This project is a continuation of the original [MSFSTouchPortalPlugin by Tim Lewis](https://github.com/tlewis17/MSFSTouchPortalPlugin).


## Features

* Connects to local or remote simulators with SimConnect.
* Allows getting data variables from simulator at regular intervals, such as flight instrument readings, control surface positions, or switch states.
* Allows triggering any interactive aircraft event via Touch Portal Actions, such as setting switches, adjusting control surfaces, radio frequencies, and so on.
* Use Touch Portal "Sliders" to control a value within any range, and/or provide visual feedback to simulator variable changes
  (eg. a throttle slider can both control the sim throttle and show the actual position when the throttle is moved with mouse/joystick/keyboard).
* Completely configurable to request any variable or trigger any event supported by the connected simulator, including with custom extensions like MobiFlight.
* Supports simulator system events (such as "flight loaded" or "sim started") as Touch Portal Events.
* Allows simultaneous usage from multiple networked Touch Portal devices.
* Optional WASM module integration allows even greater expansion, with access to many variable types (including "Local" variables) and events/actions not normally
  accessible via SimConnect alone.
* Integrates live HubHop data for activating thousands of available Input Events provided by the community.


## Documentation

See the [Wiki](https://github.com/mpaperno/MSFSTouchPortalPlugin/wiki/) for guides, tips, and example
[pages and buttons](https://github.com/mpaperno/MSFSTouchPortalPlugin/wiki/Pages-Buttons-and-Graphics) to get started with

Auto-generated documentation on all actions, connectors, events, settings, and default included states can be found in [DOCUMENTATION.md](DOCUMENTATION.md).


## Installation

1. Get the latest release of this plugin from the  [Releases](https://github.com/mpaperno/MSFSTouchPortalPlugin/releases) page.
2. The plugin is distributed and installed as a standard Touch Portal `.tpp` plugin file. If you know how to import a plugin,
just do that and skip to step 5. There is also a [short guide](https://www.touch-portal.com/blog/post/tutorials/import-plugin-guide.php) on the Touch Portal site.
3. Import the plugin:
    1. Start/open _Touch Portal_.
    2. Click the "gear" icon at the top and select "Import plugin..." from the menu.
    3. Browse to where you downloaded this plugin's `.tpp` file and select it.
4. Restart _Touch Portal_
    * When prompted by _Touch Portal_ to trust the plugin startup script, select "Yes" (the source code is public!).
5. **By default the plugin will not attempt to connect to a flight simulator on startup.** You have two options:
    1. Recommended: Create/import a [Touch Portal button](https://github.com/mpaperno/MSFSTouchPortalPlugin/wiki/Pages-Buttons-and-Graphics#the-connect-button)
     which triggers the "_MSFS - Plugin - Connect & Update -> Toggle Simulator Connection_" action. (Also a good place to show the current connection status.)
    2. Change the plugin's settings: Click the Touch Portal "gear" icon at top right of the main screen,
    then navigate to _Settings -> Plugins -> "MSFS Touch Portal Plugin"_. Set the
    "Connect To Flight Sim on Startup" setting to a value of `1` (one) and save the settings.
    The plugin will keep attempting to connect to the simulator every 30 seconds.
6. **For use with FS-X** (and compatible sims): Change the "SimConnect.cfg Index" plugin setting to `1` (one).

### Optional WASM Module (only for MSFS 2020 on PC)

7. The optional `WASimModule` MSFS component is **highly recommended** as a companion to this plugin. It it not required to use most
of the basic plugin features, but it will provide a more advanced feature set (such as access to local "L" variables and HubHop Input Events)
and further optimizations.
    1. Download the `WASimModule` .zip file from the same published Release as the plugin.
    2. Extract the contents into your MSFS _Community_ folder (so that the folder _wasimcommander-module_ is directly inside the _Community_ folder).
    3. If already running, MSFS would need to be restarted after adding the module.

Check out the [Pages, Buttons, & Graphics](https://github.com/mpaperno/MSFSTouchPortalPlugin/wiki/Pages-Buttons-and-Graphics) for examples to get started with, and see below for
a full list of known pages.

### Installation Guides

Keep in mind that while guides can be helpful as an overview and to get started,
they do get outdated and also may not cover all that is possible to do or configure.

* A video tutorial about the whole setup process was published by _OverKill Simulations_ on YouTube: [Microsoft Flight Simulator | MSFS Touch Portal | YOU NEED THIS!](https://www.youtube.com/watch?v=S4Pms-7oHf0)
* An older installation and usage guide was published on the _Simvol_ Web site: [How to use Touch Portal [with MSFS]](https://www.simvol.org/en/articles/tutorials/use-touch-portal).


---
## Pages and Examples

Here is a list of known pages which use this plugin. In no particular order, no endorsement, nor have I necessarily tried/tested all these. YMMV.

* [Pages, Buttons, & Graphics](https://github.com/mpaperno/MSFSTouchPortalPlugin/wiki/Pages-Buttons-and-Graphics) in the Wiki
* [FltSim-msfs2020-Control](https://github.com/HiDTH/FltSim-msfs2020-Control) by HiDTH @ GitHub<br />
  (**Page background graphic will cause issues, especially on iOS.**
  [Get a replacement here.](https://github.com/mpaperno/MSFSTouchPortalPlugin/wiki/Pages-Buttons-and-Graphics#replacement-for-hidths-fltsim-msfs2020-control-page-backgrounds)
  This is a cool page, but is complex and somewhat outdated, and some parts may not work properly! The author seems to have stopped updates/support for it.)
* [Touch Portal Page H145](https://flightsim.to/file/35625/touch-portal-page-h145) by redheronde @ flighsim.to
* [Touch Portal Page "Piston Single"](https://flightsim.to/file/7394/touch-portal-page-piston-single) by yushu @ flightsim.to
* [Touch Portal Page for Fenix A320](https://flightsim.to/file/35834/touch-portal-page-for-fenix-a320) by FFEENNIIXX @ flightsim.to
* [MSFSTouchPortalPages](https://github.com/FordMustang0288/MSFSTouchPortalPages) by FordMustang0288 @ GitHub
* [Special page for A32NX](https://www.simvol.org/en/downloads/miscellaneous/touch-portal-special-page-a32nx) by Pacha35 @ SimVol
* [A320neo FlyByWire + CJ4 Working Title](https://flightsim.to/file/7406/touchportal-a320neo-flybywire) by fgrooten @ flightsim.to
* [Touch Portal Page FCU and EFIS for FBWA32NX](https://flightsim.to/file/38012/touch-portal-page-fcu-and-efis-for-fbwa32nx) by Condor7777 @ flightsim.to
* [Touch Portal page for TBM 930](https://flightsim.to/file/37413/touch-portal-page-for-tbm-930) by GoodSeb @ flightsim.to
* [Touch Portal pages for FlyByWire A32NX](https://flightsim.to/file/40431/touch-portal-pages-for-flybywire-a32nx) by GoodSeb @ flightsim.to
* [Touch Portal page for KODIAK 100](https://flightsim.to/file/38161/touch-portal-page-for-kodiak-100) by GoodSeb @ flightsim.to
* [Touch Portal pages for CJ4 Working Title](https://flightsim.to/file/42677/touch-portal-pages-for-cj4-working-title) by GoodSeb @ flightsim.to
* [MSFS Throttle Quad - Views - AP](https://github.com/mpaperno/TJoy/tree/main/assets) by mpaperno @ GitHub (requires my TJoy Plugin - [screenshot](https://github.com/mpaperno/TJoy/blob/main/assets/MSFS%20Throttle%20Quad%20-%20Views%20-%20AP.jpeg))

Please let me know if you publish a page (or buttons/assets) which I could add to this list.


---
## Troubleshooting

The plugin logs errors and warnings to a plain-text file. 7 days worth of logs are kept by default (a new file is started for each day).
The log files are located within the plugin's installation folder, which is in Touch Portal's configuration directory:<br />
`C:\Users\<User_Name>\AppData\Roaming\TouchPortal\plugins\MSFS-TouchPortal-Plugin\logs` folder, where `<User_Name>` is your Windows user name.

**If something isn't working as expected, check the log.**

Another way to quickly see latest log entries is by using the provided TP States and displaying them in a button area.

- _MSFS - Plugin -> Most recent plugin log messages_ (`MSFSTouchPortalPlugin.Plugin.State.LogMessages`) - Shows the last dozen logged messages. Give this
  one a good size "button" (eg. cell size 4x3 or so).
- _MSFS - System -> Data from most recent Simulator System Event_ (`MSFSTouchPortalPlugin.SimSystem.State.SimSystemEventData`) - Shows one line of text from the last
  significant "simulator event." In case an error or warning is logged, the log entry with the error should show here.

You could also monitor the _MSFS - System -> Simulator System Event_ for the `Plugin Error` and/or `SimConnect Error` events.
For example you could have a button light up red when this event happens, so you can know to go check the log.


## Support and Discussion

Please use the GitHub [Issues](https://github.com/mpaperno/MSFSTouchPortalPlugin/issues) pages for bug reports and concise feature requests.
Use the [Discussions](https://github.com/mpaperno/MSFSTouchPortalPlugin/discussions) pages for general conversation on any related topic like suggestions or support questions.

There is also a Touch Portal Discord server discussion room at [#msfs2020](https://discord.com/channels/548426182698467339/750791488501448887)


---
## Update Notifications

The latest version of this software is always published on the GitHub [Releases](https://github.com/mpaperno/MSFSTouchPortalPlugin/releases) page.

You have several options for getting **automatically notified** about new releases:
* **If you have a GitHub account**, just open the _Watch_ menu of this repo in the top right of this page, then go to  _Custom_ and select the
_Releases_ option, then hit _Apply_ button.
* The plugin and updates are [published on Flightsim.to](https://flightsim.to/file/36546/msfs-touch-portal-plugin) where one could "subscribe" to release notificaations (account required).
* **If you already use an RSS/Atom feed reader**, just subscribe to the [feed URL](https://github.com/mpaperno/MSFSTouchPortalPlugin/releases.atom).
* **Use an RSS/Atom feed notification service**, either one specific for GitHub or a generic one, such as
(a list of services I found, I haven't necessarily tried nor do I endorse any of these):
  * https://blogtrottr.com/  (generic RSS feed notifications, no account required, use the [feed URL](https://github.com/mpaperno/MSFSTouchPortalPlugin/releases.atom))
  * https://coderelease.io/  (no account required)
  * https://newreleases.io/
  * https://gitpunch.com/

I will also post update notices in the Touch Portal Discord server room [#msfs2020](https://discord.com/channels/548426182698467339/750791488501448887)


## Related Plugin(s)

My [TJoy Touch Portal Plugin](https://github.com/mpaperno/TJoy) is an interface between Touch Portal and several virtual joystick/game pad emulation drivers like _vJoy_, _vXBox_, and _ViGEm Bus_.


## References

* [FlightSimulator.com SDK Reference (current)](https://docs.flightsimulator.com/html/Programming_Tools/SimConnect/SimConnect_SDK.htm)
* [SDK Event IDs (current)](https://docs.flightsimulator.com/html/Programming_Tools/Event_IDs/Event_IDs.htm)
* [SDK Simulator Variables (current)](https://docs.flightsimulator.com/html/Programming_Tools/SimVars/Simulation_Variables.htm)
* [HubHop Community Database](https://hubhop.mobiflight.com)


---
## Credits
Currently maintained by Maxim Paperno at https://github.com/mpaperno/MSFSTouchPortalPlugin ; see copyright and licensing details below.

Originally created by Tim Lewis at https://github.com/tlewis17/MSFSTouchPortalPlugin and published under the MIT License.

Uses components of the [WASimCommander project](https://github.com/mpaperno/WASimCommander) under terms of the GNU Public License v3 (GPLv3).

Uses the [Touch Portal C# and .NET API](https://github.com/mpaperno/TouchPortal-CS-API) library, under terms of the MIT License.

Uses a modified version of [SharpConfig](https://github.com/cemdervis/SharpConfig) library, under terms of the MIT License.
Change log is included in this repo alongside the library files.

Uses the _Microsoft SimConnect SDK_ under the terms of the _MS Flight Simulator SDK EULA (11/2019)_ document.

Uses several publicly available Microsoft .NET component libraries under the MIT License.

Uses the [Newtonsoft Json.NET](https://www.newtonsoft.com/json) library under terms of the MIT License.

Uses [Serilog Logging Extensions](https://github.com/serilog/serilog-extensions-logging) components under terms of the Apache-2.0 License.

Uses the [SQLite-net](https://github.com/praeclarum/sqlite-net) library from Krueger Systems, Inc. under terms of the MIT License.


---
## Copyright, License, and Disclaimer

MSFSTouchPortalPlugin Project<br/>
Copyright (c) 2020 Tim Lewis;<br />
Copyright Maxim Paperno, all rights reserved;<br />
Copyright MSFSTouchPortalPlugin Project Contributors

This program and associated files may be used under the terms of the GNU
General Public License as published by the Free Software Foundation,
either version 3 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

A copy of the GNU General Public License is included in this repository
and is also available at <http://www.gnu.org/licenses/>.

This project may also use 3rd-party Open Source software under the terms
of their respective licenses. The copyright notice above does not apply
to any 3rd-party components used within.
