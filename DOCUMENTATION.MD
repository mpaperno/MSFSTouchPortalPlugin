# MSFS 2020 TouchPortal Plugin

This plugin will provide a two way interface between Touch Portal and Microsoft Flight Simulator 2020 through SimConnect.

---

## Table of Contents

[MSFS - Plugin](#msfs-plugin)

[MFS - Menu](#mfs-menu)

[MFS - Electrical](#mfs-electrical)

[MFS - Engine](#mfs-engine)

[MFS - Environment](#mfs-environment)

[MSFS - Fuel](#msfs-fuel)

[MSFS - AutoPilot](#msfs-autopilot)

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

## MFS - Menu

---

## MFS - Electrical

### Actions

| Name | Description | Type | Format | Data (Default in bold) |
| --- | --- | --- | --- | --- |
| Master Alternator | Toggle Master Alternator | communicate | Toggle Master Alternator |  |
| Master Battery | Toggle Master Battery | communicate | Toggle Master Battery |  |
| Master Battery & Alternator | Toggle Master Battery & Alternator | communicate | Toggle Master Battery & Alternator |  |
| Alternator - Specific | Toggle Specific Alternator | communicate | Toggle Altenator - {0} | **1**,2,3,4 |


---

## MFS - Engine

### Actions

| Name | Description | Type | Format | Data (Default in bold) |
| --- | --- | --- | --- | --- |
| Master Ignition Switch | Toggle Master Ignition Switch | communicate | Toggle Master Ignition Switch |  |


---

## MFS - Environment

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

## MSFS - Fuel

### Actions

| Name | Description | Type | Format | Data (Default in bold) |
| --- | --- | --- | --- | --- |
| Add Fuel | Adds X amount of Fuel | communicate | Add {0}% amount of fuel |  |
| Select Left Fuel Tank | Selects the left fuel tank | communicate |  |  |
| Select Right Fuel Tank | Selects the right fuel tank | communicate |  |  |
| Fuel Dump - Toggle | Toggles the Fuel Dump | communicate | Toggle Fuel Dump |  |


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

