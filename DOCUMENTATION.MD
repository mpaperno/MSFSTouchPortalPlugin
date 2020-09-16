# MSFS 2020 TouchPortal Plugin

This plugin will provide a two way interface between Touch Portal and Microsoft Flight Simulator 2020 through SimConnect.

---

## Table of Contents

[MSFS - AutoPilot](#msfs-autopilot)

[MSFS - Communication](#msfs-communication)

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
| Auto Brake | Increase/Decrease the auto brake | communicate | Auto Brake - {0} | Increaes,Decrease |
| Localizer | Toggle/On/Off the localizer for auto pilot | communicate | Localizer - {0} | **Toggle**,On,Off |
| AutoPilot | Toggle/On/Off Auto Pilot | communicate | Auto Pilot Master - {0} | **Toggle**,On,Off |
| Attitude Hold | Toggle/On/Off the attitude hold for auto pilot | communicate | Attitude Hold - {0} | **Toggle**,On,Off |
| Pitch Reference Value | Sets the pitch reference value | communicate | Pitch Reference Value - {0} | **Select**,Increase,Decrease |
| Approach Mode | Toggle/On/Off the approach mode for auto pilot | communicate | Approach Mode - {0} | **Toggle**,On,Off |
| AP Max Bank Angle | Increase/Decrease the max bank angle | communicate | Max Bank Angle - {0} | **Increase**,Decrease |
| Heading Hold | Toggle/On/Off the heading hold for auto pilot | communicate | Heading Hold - {0} | **Toggle**,On,Off |
| Heading Hold Value | Sets the heading hold value | communicate | Heading Hold Value - {0} | **Select**,Increase,Decrease,Set |
| Altitude Hold | Toggle/On/Off the altitude hold for auto pilot | communicate | Altitude Hold - {0} | **Toggle**,On,Off |
| Altitude Hold Value | Sets the altitude hold value | communicate | Altitude Hold Value - {0} | **Select**,Increase,Decrease,Set,Set Metric |
| Back Course Mode | Toggle/On/Off the back course mode for auto pilot | communicate | Back Course Mode - {0} | **Toggle**,On,Off |
| Nav1 Mode | Toggle/On/Off the Nav1 mode for auto pilot | communicate | Nav1 Mode - {0} | **Toggle**,On,Off |
| Nav Mode - Set | Sets the nav to 1 or 2 for Nav mode | communicate | Nav Mode - {0}  | **1**,2 |
| Vertical Speed Value | Sets the vertical speed value | communicate | Vertical Speed Value - {0} | **Select**,Increase,Decrease,Set,Set Metric |
| Airspeed Hold | Toggle/On/Off/Set the airspeed hold for auto pilot | communicate | Airspeed Hold - {0} | **Toggle**,On,Off,Set |
| Airspeed Hold Value | Sets the airspeed hold value | communicate | Airspeed Hold Value - {0} | **Select**,Increase,Decrease,Set |
| Auto Throttle Mode | Toggles the Arm/GoAround modes for auto throttle | communicate | Toggle Auto Throttle - {0} | **Arm**,GoAround |
| Mach Hold | Toggle/On/Off/Set the mach hold for auto pilot | communicate | Mach Hold - {0} | **Toggle**,On,Off,Set |
| Mach Hold Value | Sets the mach hold value | communicate | Mach Hold Value - {0} | Select,**Increase**,Decrease |
| Flight Director | Toggle the Flight Director for auto pilot | communicate | Flight Director - {0} | **Toggle** |
| Flight Director Pitch Sync | Syncs the Flight Director with the current pitch | communicate | Flight Director Pitch Sync |  |
| Wing Leveler | Toggle/On/Off the Wing Leveler for auto pilot | communicate | Wing Leveler - {0} | **Toggle**,On,Off |
| Yaw Dampener | Toggle/On/Off/Set the Yaw Dampener | communicate | Yaw Dampener - {0} | **Toggle**,On,Off,Set |


---

## MSFS - Communication

---

## MSFS - Electrical

### Actions

| Name | Description | Type | Format | Data (Default in bold) |
| --- | --- | --- | --- | --- |
| Master Battery & Alternator | Toggle Master Battery & Alternator | communicate | Toggle Master Battery & Alternator |  |
| Alternator - Specific | Toggle Specific Alternator | communicate | Toggle Altenator - {0} | **1**,2,3,4 |
| Toggle/On/Off/Set Strobe Lights | Toggle/On/Off/Set Strobe Lights | communicate | Strobe Lights - {0} | **Toggle**,On,Off,Set |
| Toggle/On/Off/Set Panel Lights | Toggle/On/Off/Set Panel Lights | communicate | Panel Lights - {0} | **Toggle**,On,Off,Set |
| Toggle/On/Off/Set Landing Lights | Toggle/On/Off/Set Landing Lights | communicate | Landing Lights - {0} | **Toggle**,On,Off,Set |
| Toggle All/Specific Lights | Toggle All/Specific Lights | communicate | Toggle Lights - {0} | **All**,Beacon,Taxi,Logo,Recognition,Wing,Nav,Cabin |
| Master Alternator | Toggle Master Alternator | communicate | Toggle Master Alternator |  |
| Master Battery | Toggle Master Battery | communicate | Toggle Master Battery |  |


---

## MSFS - Engine

### Actions

| Name | Description | Type | Format | Data (Default in bold) |
| --- | --- | --- | --- | --- |
| Engine Auto Start/Shutdown | Start/Shutdown Engine | communicate | Engine - {0} | **Start**,Shutdown |
| Toggle All Magnetos | Toggle All Magnetos | communicate | Toggle All Magnetos - {0} | **Start**,Off,Right,Left,Both,Decrease,Increase |
| Magnetos Specific | Toggle Magneto Specific | communicate | Toggle Magneto {0} - {1} | **1**,2,3,4 |
| Toggle Starters | Toggle Starters | communicate | Toggle Starter - {0} | **All**,1,2,3,4 |
| Throttle | Sets all throttles | communicate | All Throttles - {0} | **Full**,Increase,Increase Small,Decrease,Decrease Small,Cut,Set,10%,20%,30%,40%,50%,60%,70%,80%,90% |
| Mixture | Sets all mixtures | communicate | All Mixtures - {0} | **Rich**,Increase,Increase Small,Decrease,Decrease Small,Lean,Best,Set |
| Mixture Specific | Sets mixture on specific engine | communicate | Mixture {0} - {1} | **1**,2,3,4 |
| Master Ignition Switch | Toggle Master Ignition Switch | communicate | Toggle Master Ignition Switch |  |


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

### Actions

| Name | Description | Type | Format | Data (Default in bold) |
| --- | --- | --- | --- | --- |
| Ailerons | Ailerons | communicate | Ailerons - {0} | **Center**,Left,Right,Set |
| Brakes | Brakes | communicate | Brakes - {0} | **All**,Left,Right |
| Toggle Parking Brake | Toggle Parking Brake | communicate | Toggle Parking Brake |  |
| Flaps | Flaps | communicate | Flaps - {0} | **Up**,Down,Increase,Decrease,1,2,3,Set |
| Cowl Flaps All | Cowl Flaps All | communicate | Cowl Flaps All - {0} | **Increase**,Decrease |
| Cowl Flaps 1 | Cowl Flaps 1 | communicate | Cowl Flaps 1 - {0} | **Increase**,Decrease |
| Cowl Flaps 2 | Cowl Flaps 2 | communicate | Cowl Flaps 2 - {0} | **Increase**,Decrease |
| Cowl Flaps 3 | Cowl Flaps 3 | communicate | Cowl Flaps 3 - {0} | **Increase**,Decrease |
| Cowl Flaps 4 | Cowl Flaps 4 | communicate | Cowl Flaps 4 - {0} | **Increase**,Decrease |
| Rudder | Rudder | communicate | Rudder - {0} | **Center**,Left,Right,Set |
| Spoilers | Spoilers | communicate | Spoilers - {0} | **Toggle**,On,Off,Set |
| Spoilers Arm | Spoilers Arm | communicate | Spoilers Arm - {0} | **Toggle**,On,Off,Set |
| Gear Manipulation | Gear Manipulation | communicate | Gear - {0} | **Toggle**,Up,Down,Set,Pump |
| Aileron Trim | Aileron Trim | communicate | Aileron Trim - {0} | **Left**,Right |
| Elevator Trim | Elevator Trim | communicate | Elevator Trim - {0} | **Up**,Down |
| Rudder Trim | Rudder Trim | communicate | Rudder Trim - {0} | **Left**,Right |


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
| Fuel Pump - Toggle | Toggles the Fuel Pump | communicate | Toggle Fuel Pump |  |
| Electric Fuel Pump - Toggle | Toggles the Electric Fuel Pump | communicate | Toggle Electric Fuel Pump - {0} | **All**,1,2,3,4 |


---

## MSFS - Menu

---

## MSFS - Plugin

### Actions

| Name | Description | Type | Format | Data (Default in bold) |
| --- | --- | --- | --- | --- |
| Connection | Toggle/On/Off SimConnect Connection | communicate | SimConnect Connection - {0} | **Toggle**,On,Off |


---

