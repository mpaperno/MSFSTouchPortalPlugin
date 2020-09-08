# MSFS 2020 TouchPortal Plugin

## Overview
This plugin will provide a two way interface between Touch Portal and Microsoft Flight Simulator 2020 through SimConnect. 

## Features
* Generates entry.tp automatically. 

## Documentation
Documentation can be found here: [link](DOCUMENTATION.MD)

## TODO

* Connect/Disconnect Re-register services
* States pushed back to Touch Portal
  * Done but need more mappings. Also need to update MD with state info. 
* Define SimVars
* Documentation - Events needed and additional rows of data to display. 
* Automate generation of Entry.tp and Docs on build and put in root. 

* More Controls
  * Communications

## Known Issues

* "Set" buttons require data to be passed in and do not currently work. 
* SimConnect will fail if the simulator isn't running but the plugin starts up. 
To resolve after Sim startup, go into Touch Portal Plugin settings and stop/start the plugin.

## References

* [EventIdss](https://docs.microsoft.com/en-us/previous-versions/microsoft-esp/cc526980(v=msdn.10))
* [Variables](https://docs.microsoft.com/en-us/previous-versions/microsoft-esp/cc526981(v=msdn.10))