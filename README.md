# MSFS/SimConnect Touch Portal Plugin

[![verify-build](https://github.com/mpaperno/MSFSTouchPortalPlugin/actions/workflows/verify-build.yml/badge.svg)](https://github.com/mpaperno/MSFSTouchPortalPlugin/actions/workflows/verify-build.yml)
[![GitHub release (latest by date including pre-releases)](https://img.shields.io/github/v/release/mpaperno/MSFSTouchPortalPlugin?include_prereleases)](https://github.com/mpaperno/MSFSTouchPortalPlugin/releases)
[![Downloads](https://img.shields.io/github/downloads/mpaperno/MSFSTouchPortalPlugin/total.svg)](https://github.com/mpaperno/MSFSTouchPortalPlugin/releases)
[![Downloads of latest release](https://img.shields.io/github/downloads/mpaperno/MSFSTouchPortalPlugin/latest/total)](https://github.com/mpaperno/MSFSTouchPortalPlugin/releases/latest)
[![Stars](https://img.shields.io/github/stars/mpaperno/MSFSTouchPortalPlugin)](https://github.com/mpaperno/MSFSTouchPortalPlugin/stargazers)
[![License](https://img.shields.io/badge/license-GPL3-blue.svg)](LICENSE)

<div align="center">
<img src="https://github.com/mpaperno/MSFSTouchPortalPlugin/wiki/images/logo/banner_top-768x204.png" />
</div>

## Overview

This plugin provided a two way interface between [Touch Portal](https://www.touch-portal.com/) software and Flight Simulators which use SimConnect,
such as Microsoft Flight Simulator 2020 (MSFS) and FS-X.

This project is a continuation of the original [MSFSTouchPortalPlugin by Tim Lewis](https://github.com/tlewis17/MSFSTouchPortalPlugin).

## Features

* Connects automatically through SimConnect.
* Allows getting data variables from simulator at regular intervals, such as flight instruments, control surfaces, or switch states.
* Allows triggering any interactive aircraft event via a Touch Portal Action, such as setting switches, adjusting control surfaces, radio frequencies, and so on.
* Completely configurable to request any variable or trigger any event supported by the connected simulator, including with custom extensions like MobiFlight.
* Supports simulator system events (such as "flight loaded" or "sim started") as Touch Portal Events.
* Allows simultaneous usage from multiple networked Touch Portal devices.
* Optional WASM module integration allows even greater expansion, with access to many variable types (including "Local" variables) and events/actions not normally
  accessible via SimConnect alone.

## Documentation

See the [Wiki](https://github.com/mpaperno/MSFSTouchPortalPlugin/wiki/) for some guides and tips.

Auto-generated documentation on all actions, states, and settings can be found in [DOCUMENTATION.MD](DOCUMENTATION.MD).

## Installation & Usage

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
   1. Create a new Touch Portal button which triggers the "MSFS - Plugin - Toggle/On/Off SimConnect Connection" action. (Also a good place to show the current connection status.)
   2. Go to the plugin's settings, via the Touch Portal "gear" icon then navigate to _Settings -> Plugins -> "MSFS Touch Portal Plugin"_ and set the
    "Connect To Flight Sim on Startup" setting to a value of `1` (one). Then save the settings. To restart the plugin, go back to the same plugin settings page and
    press the "Stop" button, then the "Start" button.  The plugin will keep attempting to connect to the simulator every 30 seconds.
6. **For use with FS-X** (and compatible sims): Change the "SimConnect.cfg Index" plugin setting to `1` (one).


After that you will have a list of new actions you can choose from. Also "Dynamic Text" variables are now available. You can see them from the Dynamic Text Updater,
or you can add an option for "On Plugin State Change" then select the corresponding state and "Changes to".

For buttons you use this like:

"On Plugin State Changes AutoPilot Master changes to 1" Then add whatever logic you want like button visuals. Duplicate this and add one for "does not change to 1" and that is when a button turns off.

This is very much a work in progress!

A comprehensive installation and usage guide was published on the _Simvol_ Web site: [How to use Touch Portal [with MSFS]](https://www.simvol.org/en/articles/tutorials/use-touch-portal).


## Pages and Examples

Here is a list of known pages which use this plugin. In no particular order, no endorsement, nor have I necessarily tried/tested all these. YMMV.

* [FltSim-msfs2020-Control](https://github.com/HiDTH/FltSim-msfs2020-Control) by HiDTH @ GitHub
* [Touch Portal Page H145](https://flightsim.to/file/35625/touch-portal-page-h145) by edheronde @ flighsim.to
* [Touch Portal Page "Piston Single"](https://flightsim.to/file/7394/touch-portal-page-piston-single) by yushu @ flightsim.to
* [Touch Portal Page for Fenix A320](https://flightsim.to/file/35834/touch-portal-page-for-fenix-a320) by FFEENNIIXX @ flightsim.to
* [MSFSTouchPortalPages](https://github.com/FordMustang0288/MSFSTouchPortalPages) by FordMustang0288 @ GitHub
* [Special page for A32NX](https://www.simvol.org/en/downloads/miscellaneous/touch-portal-special-page-a32nx) by Pacha35 @ SimVol
* [A320neo FlyByWire + CJ4 Working Title](https://flightsim.to/file/7406/touchportal-a320neo-flybywire) by fgrooten @ flightsim.to  (possibly missing a credit to HiDTH)
* [MSFS Throttle Quad - Views - AP](https://github.com/mpaperno/TJoy/tree/main/assets) by mpaperno @ GitHub (requires my TJoy Plugin - [screenshot](https://github.com/mpaperno/TJoy/blob/main/assets/MSFS%20Throttle%20Quad%20-%20Views%20-%20AP.jpeg))

Please let me know if you publish a page (or buttons/assets) which I could add to this list.

## Troubleshooting

The plugin logs errors and warnings to a plain-text file. 7 days worth of logs are kept by default (a new file is started for each day). 
The log files are located within the plugin's installation folder, which is in Touch Portal's configuration directory:<br />
`C:\Users<User Name>\AppData\Roaming\TouchPortal\plugins\MSFSTouchPortalPlugin\logs` folder, where `<User Name>` is your Windows user name.

**If something isn't working as expected, check the log.**

Another way to quickly see latest log entries is by using the provided TP States and displaying them in a button area.

- _MSFS - Plugin -> Most recent plugin log messages_ (`MSFSTouchPortalPlugin.Plugin.State.LogMessages`) - Shows the last dozen logged messages. Give this
  one a good size "button" (eg. cell size 4x3 or so).
- _MSFS - System -> Data from most recent Simulator System Event_ (`MSFSTouchPortalPlugin.SimSystem.State.SimSystemEventData`) - Shows one line of text from the last
  significant "simulator event." In case an error or warning is logged, the log entry with the error should show here.

You could also monitor the _MSFS - System -> Simulator System Event_ for the `Plugin Error` and/or `SimConnect Error` events. 
For example you could have a button light up red when this event happens, so you can know to go check the log.


## Update Notifications

The latest version of this software is always published on the GitHub [Releases](https://github.com/mpaperno/MSFSTouchPortalPlugin/releases) page.
GitHub publishes an "Atom" (like RSS) feed with the latest version of any repository by adding `.atom` to the end o the URL. This repository's
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

My [TJoy Touch Portal Plugin](https://github.com/mpaperno/TJoy) is an interface between Touch Portal and several virtual joystick/game pad emulation drivers like _vJoy_, _vXBox_, and _ViGEm Bus_.

## TODO

* Sliders ("connectors")
* Custom SimVar/states setup GUI

## References

* [MSFS EventIDs (old but has all events on one page)](https://docs.microsoft.com/en-us/previous-versions/microsoft-esp/cc526980\(v=msdn.10\))
* [MSFS Variables (also old but also all on one page)](https://docs.microsoft.com/en-us/previous-versions/microsoft-esp/cc526981\(v=msdn.10\))
* [FlightSimulator.com SDK Reference (current)](https://docs.flightsimulator.com/html/Programming_Tools/SimConnect/SimConnect_SDK.htm)
* [SDK Event IDs (current)](https://docs.flightsimulator.com/html/Programming_Tools/Event_IDs/Event_IDs.htm)
* [SDK Simulator Variables (current)](https://docs.flightsimulator.com/html/Programming_Tools/SimVars/Simulation_Variables.htm)
* [HubHop Community Database](https://hubhop.mobiflight.com)

## Credits
Currently maintained by Maxim Paperno at https://github.com/mpaperno/MSFSTouchPortalPlugin ; see copyright and licensing details below.

Originally created by Tim Lewis at https://github.com/tlewis17/MSFSTouchPortalPlugin and published under the MIT License.

Uses a modified version of [TouchPortalSDK for C#](https://github.com/oddbear/TouchPortalSDK) library
binaries, used under the MIT License. The modified source is [published here](https://github.com/mpaperno/TouchPortal-CS-API).

Uses a modified version of [SharpConfig](https://github.com/cemdervis/SharpConfig) library, used under the MIT License.
Change log is included in this repo alongside the library files.

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
