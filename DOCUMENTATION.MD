# MSFS 2020 TouchPortal Plugin

This plugin will provide a two way interface between Touch Portal and Microsoft Flight Simulator 2020 through SimConnect.

---

## Table of Contents

[MSFS - AutoPilot](#msfs-autopilot)

[MSFS - Electrical](#msfs-electrical)

[MSFS - Engine](#msfs-engine)

[MSFS - Environment](#msfs-environment)

[MSFS - Failures](#msfs-failures)

[MSFS - Flight Instruments](#msfs-flightinstruments)

[MSFS - Flight Systems](#msfs-flightsystems)

[MSFS - Fuel](#msfs-fuel)

[MSFS - Menu](#msfs-menu)

[MSFS - Plugin](#msfs-plugin)

---

## MSFS - AutoPilot

### Actions

| Name | Description | Type | Format | Data (Default in bold) |
| --- | --- | --- | --- | --- |
| AutoPilot | Toggle/On/Off Auto Pilot | communicate | Auto Pilot Master - {0} | **Toggle**,On,Off |
| Attitude Hold | Toggle/On/Off the attitude hold for auto pilot | communicate | Attitude Hold - {0} | **Toggle**,On,Off |
| Approach Mode | Toggle/On/Off the approach mode for auto pilot | communicate | Approach Mode - {0} | **Toggle**,On,Off |
| Heading Hold | Toggle/On/Off the heading hold for auto pilot | communicate | Heading Hold - {0} | **Toggle**,On,Off |
| Altitude Hold | Toggle/On/Off the altitude hold for auto pilot | communicate | Altitude Hold - {0} | **Toggle**,On,Off |
| Back Course Mode | Toggle/On/Off the back course mode for auto pilot | communicate | Back Course Mode - {0} | **Toggle**,On,Off |
| Nav1 Mode | Toggle/On/Off the Nav1 mode for auto pilot | communicate | Nav1 Mode - {0} | **Toggle**,On,Off |


---

## MSFS - Electrical

### Actions

| Name | Description | Type | Format | Data (Default in bold) |
| --- | --- | --- | --- | --- |
| Master Alternator | Toggle Master Alternator | communicate | Toggle Master Alternator |  |
| Master Battery | Toggle Master Battery | communicate | Toggle Master Battery |  |
| Master Battery & Alternator | Toggle Master Battery & Alternator | communicate | Toggle Master Battery & Alternator |  |
| Alternator - Specific | Toggle Specific Alternator | communicate | Toggle Altenator - {0} | **1**,2,3,4 |
| Toggle/On/Off/Set Strobe Lights | Toggle/On/Off/Set Strobe Lights | communicate | Strobe Lights - {0} | **Toggle**,On,Off,Set |
| Toggle/On/Off/Set Panel Lights | Toggle/On/Off/Set Panel Lights | communicate | Panel Lights - {0} | **Toggle**,On,Off,Set |
| Toggle/On/Off/Set Landing Lights | Toggle/On/Off/Set Landing Lights | communicate | Landing Lights - {0} | **Toggle**,On,Off,Set |
| Toggle All/Specific Lights | Toggle All/Specific Lights | communicate | Toggle Lights - {0} | **All**,Beacon,Taxi,Logo,Recognition,Wing,Nav,Cabin |


---

## MSFS - Engine

### Actions

| Name | Description | Type | Format | Data (Default in bold) |
| --- | --- | --- | --- | --- |
| Master Ignition Switch | Toggle Master Ignition Switch | communicate | Toggle Master Ignition Switch |  |
| Engine Auto Start/Shutdown | Start/Shutdown Engine | communicate | Engine - {0} | **Start**,Shutdown |
| Toggle All Magnetos | Toggle All Magnetos | communicate | Toggle All Magnetos - {0} | **Start**,Off,Right,Left,Both,Decrease,Increase |
| Toggle Magneto 1 | Toggle Magneto 1 | communicate | Toggle Magneto 1 - {0} | **Start**,Off,Right,Left,Both,Decrease,Increase |
| Toggle Magneto 2 | Toggle Magneto 2 | communicate | Toggle Magneto 2 - {0} | **Start**,Off,Right,Left,Both,Decrease,Increase |
| Toggle Magneto 3 | Toggle Magneto 3 | communicate | Toggle Magneto 3 - {0} | **Start**,Off,Right,Left,Both,Decrease,Increase |
| Toggle Magneto 4 | Toggle Magneto 4 | communicate | Toggle Magneto 4 - {0} | **Start**,Off,Right,Left,Both,Decrease,Increase |
| Toggle Starters | Toggle Starters | communicate | Toggle Starter - {0} | **All**,1,2,3,4 |


---

## MSFS - Environment

### Actions

| Name | Description | Type | Format | Data (Default in bold) |
| --- | --- | --- | --- | --- |
| Anti-Ice | Toggle/On/Off Anti Ice | communicate | Anti Ice - {0} | **Toggle**,On,Off,Set |
| Anti-Ice Engine 1 | Toggle/On/Off Anti Ice Engine 1 | communicate | Anti Ice Engine 1 - {0} | **Toggle**,Set |
| Anti-Ice Engine 2 | Toggle/On/Off Anti Ice Engine 2 | communicate | Anti Ice Engine 2 - {0} | **Toggle**,Set |
| Anti-Ice Engine 3 | Toggle/On/Off Anti Ice Engine 3 | communicate | Anti Ice Engine 3 - {0} | **Toggle**,Set |
| Anti-Ice Engine 4 | Toggle/On/Off Anti Ice Engine 4 | communicate | Anti Ice Engine 4 - {0} | **Toggle**,Set |
| Structural De-ice | Toggle Structural DeIce | communicate | Toggle Structural DeIce |  |
| Propeller De-ice | Toggle Propeller DeIce | communicate | Toggle Propeller DeIce |  |
| Pitot Heat | Toggle/On/Off Pitot Heat | communicate | Pitot Heat - {0} | **Toggle**,On,Off,Set |


---

## MSFS - Failures

### Actions

| Name | Description | Type | Format | Data (Default in bold) |
| --- | --- | --- | --- | --- |
| Failures | Toggle Failures | communicate | Toggle Failures - {0} | **Electrical**,Vacuum,Pitot,Static Port,Hydraulic,Total Brake,Left Brake,Right Brake,Engine 1,Engine 2,Engine 3,Engine 4 |


---

## MSFS - Flight Instruments

---

## MSFS - Flight Systems

---

## MSFS - Fuel

### Actions

| Name | Description | Type | Format | Data (Default in bold) |
| --- | --- | --- | --- | --- |
| Add Fuel | Adds 25% amount of Fuel | communicate | Add 25% amount of fuel |  |
| Fuel Selector 1 | Fuel Selector 1 | communicate | Fuel Selector 1 - {0} | **All**,Off,Left,Right,Left - Aux,Right - Aux,Center,Set |
| Fuel Selector 2 | Fuel Selector 2 | communicate | Fuel Selector 2 - {0} | **All**,Off,Left,Right,Left - Aux,Right - Aux,Center,Set |
| Fuel Selector 3 | Fuel Selector 3 | communicate | Fuel Selector 3 - {0} | **All**,Off,Left,Right,Left - Aux,Right - Aux,Center,Set |
| Fuel Selector 4 | Fuel Selector 4 | communicate | Fuel Selector 4 - {0} | **All**,Off,Left,Right,Left - Aux,Right - Aux,Center,Set |
| Toggle All/Specific Primers | Toggle All/Specific Primers | communicate | Toggle Primers - {0} | **All**,1,2,3,4 |
| Fuel Dump - Toggle | Toggles the Fuel Dump | communicate | Toggle Fuel Dump |  |
| Toggle/Open/Off Cross Feed | Toggle/Open/Off Cross Feed | communicate | Cross Feed - {0} | Toggle,**Open**,Off |
| Toggle All/Specific Fuel Valve | Toggle All/Specific Fuel Valve | communicate | Toggle Fuel Valve - {0} | **All**,1,2,3,4 |


---

## MSFS - Menu

---

## MSFS - Plugin

### Actions

| Name | Description | Type | Format | Data (Default in bold) |
| --- | --- | --- | --- | --- |
| Connection | Toggle/On/Off SimConnect Connection | communicate | SimConnect Connection - {0} | **Toggle**,On,Off |


### States

| Id | Type | Description | DefaultValue |
| --- | --- | --- | --- |
| MSFSTouchPortalPlugin.Plugin.State.Connected | text | The status of SimConnect | false |


---

