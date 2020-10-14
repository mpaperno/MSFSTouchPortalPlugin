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

[MSFS - Plugin](#msfs-plugin)

[MSFS - System](#msfs-system)

---

## MSFS - AutoPilot

### Actions

| Name | Description | Type | Format | Data (Default in bold) |
| --- | --- | --- | --- | --- |
| Airspeed Hold | Toggle/On/Off/Set the airspeed hold for auto pilot | communicate | Airspeed Hold - {0} | **Toggle**,On,Off,Set |
| Airspeed Hold Value | Sets the airspeed hold value | communicate | Airspeed Hold Value - {0} | **Select**,Increase,Decrease,Set |
| Altitude Hold | Toggle/On/Off the altitude hold for auto pilot | communicate | Altitude Hold - {0} | **Toggle**,On,Off |
| Altitude Hold Value | Sets the altitude hold value | communicate | Altitude Hold Value - {0} | **Select**,Increase,Decrease,Set,Set Metric |
| AP Max Bank Angle | Increase/Decrease the max bank angle | communicate | Max Bank Angle - {0} | **Increase**,Decrease |
| Approach Mode | Toggle/On/Off the approach mode for auto pilot | communicate | Approach Mode - {0} | **Toggle**,On,Off |
| Attitude Hold | Toggle/On/Off the attitude hold for auto pilot | communicate | Attitude Hold - {0} | **Toggle**,On,Off |
| Attitude Hold Value | Sets the attitude hold value | communicate | Attitude Hold Value - {0} | **Select**,Increase,Decrease |
| Auto Brake | Increase/Decrease the auto brake | communicate | Auto Brake - {0} | Increaes,Decrease |
| Auto Throttle Mode | Toggles the Arm/GoAround modes for auto throttle | communicate | Toggle Auto Throttle - {0} | **Arm**,GoAround |
| AutoPilot | Toggle/On/Off Auto Pilot | communicate | Auto Pilot Master - {0} | **Toggle**,On,Off |
| Back Course Mode | Toggle/On/Off the back course mode for auto pilot | communicate | Back Course Mode - {0} | **Toggle**,On,Off |
| Flight Director | Toggle the Flight Director for auto pilot | communicate | Flight Director - {0} | **Toggle** |
| Flight Director Pitch Sync | Syncs the Flight Director with the current pitch | communicate | Flight Director Pitch Sync |  |
| Heading Hold | Toggle/On/Off the heading hold for auto pilot | communicate | Heading Hold - {0} | **Toggle**,On,Off |
| Heading Hold Value | Sets the heading hold value | communicate | Heading Hold Value - {0} | **Select**,Increase,Decrease,Set |
| Localizer | Toggle/On/Off the localizer for auto pilot | communicate | Localizer - {0} | **Toggle**,On,Off |
| Mach Hold | Toggle/On/Off/Set the mach hold for auto pilot | communicate | Mach Hold - {0} | **Toggle**,On,Off,Set |
| Mach Hold Value | Sets the mach hold value | communicate | Mach Hold Value - {0} | Select,**Increase**,Decrease |
| Nav Mode - Set | Sets the nav to 1 or 2 for Nav mode | communicate | Nav Mode - {0}  | **1**,2 |
| Nav1 Mode | Toggle/On/Off the Nav1 mode for auto pilot | communicate | Nav1 Mode - {0} | **Toggle**,On,Off |
| Vertical Speed | Toggle the Vertical Speed for auto pilot | communicate | Vertical Speed - {0} | **Toggle** |
| Vertical Speed Value | Sets the vertical speed value | communicate | Vertical Speed Value - {0} | **Select**,Increase,Decrease,Set,Set Metric |
| Wing Leveler | Toggle/On/Off the Wing Leveler for auto pilot | communicate | Wing Leveler - {0} | **Toggle**,On,Off |
| Yaw Dampener | Toggle/On/Off/Set the Yaw Dampener | communicate | Yaw Dampener - {0} | **Toggle**,On,Off,Set |


---

## MSFS - Communication

### Actions

| Name | Description | Type | Format | Data (Default in bold) |
| --- | --- | --- | --- | --- |
| Radio Interaction | Radio Interaction | communicate | Radio {0} - {1} | **COM1**,COM2,NAV1,NAV2 |


---

## MSFS - Electrical

### Actions

| Name | Description | Type | Format | Data (Default in bold) |
| --- | --- | --- | --- | --- |
| Alternator - Specific | Toggle Specific Alternator | communicate | Toggle Altenator - {0} | **1**,2,3,4 |
| Master Alternator | Toggle Master Alternator | communicate | Toggle Master Alternator |  |
| Master Battery | Toggle Master Battery | communicate | Toggle Master Battery |  |
| Master Battery & Alternator | Toggle Master Battery & Alternator | communicate | Toggle Master Battery & Alternator |  |
| Toggle All/Specific Lights | Toggle All/Specific Lights | communicate | Toggle Lights - {0} | **All**,Beacon,Taxi,Logo,Recognition,Wing,Nav,Cabin |
| Toggle/On/Off/Set Landing Lights | Toggle/On/Off/Set Landing Lights | communicate | Landing Lights - {0} | **Toggle**,On,Off,Set |
| Toggle/On/Off/Set Panel Lights | Toggle/On/Off/Set Panel Lights | communicate | Panel Lights - {0} | **Toggle**,On,Off,Set |
| Toggle/On/Off/Set Strobe Lights | Toggle/On/Off/Set Strobe Lights | communicate | Strobe Lights - {0} | **Toggle**,On,Off,Set |


---

## MSFS - Engine

### Actions

| Name | Description | Type | Format | Data (Default in bold) |
| --- | --- | --- | --- | --- |
| Engine Auto Start/Shutdown | Start/Shutdown Engine | communicate | Engine - {0} | **Start**,Shutdown |
| Magnetos Specific | Toggle Magneto Specific | communicate | Toggle Magneto {0} - {1} | **1**,2,3,4 |
| Master Ignition Switch | Toggle Master Ignition Switch | communicate | Toggle Master Ignition Switch |  |
| Mixture | Sets all mixtures | communicate | All Mixtures - {0} | **Rich**,Increase,Increase Small,Decrease,Decrease Small,Lean,Best,Set |
| Mixture Specific | Sets mixture on specific engine | communicate | Mixture {0} - {1} | **1**,2,3,4 |
| Throttle | Sets all throttles | communicate | All Throttles - {0} | **Full**,Increase,Increase Small,Decrease,Decrease Small,Cut,Set,10%,20%,30%,40%,50%,60%,70%,80%,90% |
| Throttle Specific | Sets Throttle on specific engine | communicate | Throttle {0} - {1} | **1**,2,3,4 |
| Toggle All Magnetos | Toggle All Magnetos | communicate | Toggle All Magnetos - {0} | **Start**,Off,Right,Left,Both,Decrease,Increase |
| Toggle Starters | Toggle Starters | communicate | Toggle Starter - {0} | **All**,1,2,3,4 |


---

## MSFS - Environment

### Actions

| Name | Description | Type | Format | Data (Default in bold) |
| --- | --- | --- | --- | --- |
| Anti-Ice | Toggle/On/Off Anti Ice | communicate | Anti Ice - {0} | **Toggle**,On,Off,Set |
| Anti-Ice Engine | Toggle/On/Off Anti Ice Engine | communicate | Anti Ice Engine {0} - {1} | **1**,2,3,4 |
| Pitot Heat | Toggle/On/Off Pitot Heat | communicate | Pitot Heat - {0} | **Toggle**,On,Off,Set |
| Propeller De-ice | Toggle Propeller DeIce | communicate | Toggle Propeller DeIce |  |
| Structural De-ice | Toggle Structural DeIce | communicate | Toggle Structural DeIce |  |


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
| Aileron Trim | Aileron Trim | communicate | Aileron Trim - {0} | **Left**,Right |
| Ailerons | Ailerons | communicate | Ailerons - {0} | **Center**,Left,Right,Set |
| Brakes | Brakes | communicate | Brakes - {0} | **All**,Left,Right |
| Cowl Flaps 1 | Cowl Flaps 1 | communicate | Cowl Flaps 1 - {0} | **Increase**,Decrease |
| Cowl Flaps 2 | Cowl Flaps 2 | communicate | Cowl Flaps 2 - {0} | **Increase**,Decrease |
| Cowl Flaps 3 | Cowl Flaps 3 | communicate | Cowl Flaps 3 - {0} | **Increase**,Decrease |
| Cowl Flaps 4 | Cowl Flaps 4 | communicate | Cowl Flaps 4 - {0} | **Increase**,Decrease |
| Cowl Flaps All | Cowl Flaps All | communicate | Cowl Flaps All - {0} | **Increase**,Decrease |
| Elevator Trim | Elevator Trim | communicate | Elevator Trim - {0} | **Up**,Down |
| Flaps | Flaps | communicate | Flaps - {0} | **Up**,Down,Increase,Decrease,1,2,3,Set |
| Gear Manipulation | Gear Manipulation | communicate | Gear - {0} | **Toggle**,Up,Down,Set,Pump |
| Rudder | Rudder | communicate | Rudder - {0} | **Center**,Left,Right,Set |
| Rudder Trim | Rudder Trim | communicate | Rudder Trim - {0} | **Left**,Right |
| Spoilers | Spoilers | communicate | Spoilers - {0} | **Toggle**,On,Off,Set |
| Spoilers Arm | Spoilers Arm | communicate | Spoilers Arm - {0} | **Toggle**,On,Off,Set |
| Toggle Parking Brake | Toggle Parking Brake | communicate | Toggle Parking Brake |  |


---

## MSFS - Fuel

### Actions

| Name | Description | Type | Format | Data (Default in bold) |
| --- | --- | --- | --- | --- |
| Add Fuel | Adds 25% amount of Fuel | communicate | Add 25% amount of fuel |  |
| Electric Fuel Pump - Toggle | Toggles the Electric Fuel Pump | communicate | Toggle Electric Fuel Pump - {0} | **All**,1,2,3,4 |
| Fuel Dump - Toggle | Toggles the Fuel Dump | communicate | Toggle Fuel Dump |  |
| Fuel Pump - Toggle | Toggles the Fuel Pump | communicate | Toggle Fuel Pump |  |
| Fuel Selectors | Fuel Selectors | communicate | Fuel Selector {0} - {1} | **1**,2,3,4 |
| Toggle All/Specific Fuel Valve | Toggle All/Specific Fuel Valve | communicate | Toggle Fuel Valve - {0} | **All**,1,2,3,4 |
| Toggle All/Specific Primers | Toggle All/Specific Primers | communicate | Toggle Primers - {0} | **All**,1,2,3,4 |
| Toggle/Open/Off Cross Feed | Toggle/Open/Off Cross Feed | communicate | Cross Feed - {0} | Toggle,**Open**,Off |


---

## MSFS - Plugin

### Actions

| Name | Description | Type | Format | Data (Default in bold) |
| --- | --- | --- | --- | --- |
| Connection | Toggle/On/Off SimConnect Connection | communicate | SimConnect Connection - {0} | **Toggle**,On,Off |


---

## MSFS - System

### Actions

| Name | Description | Type | Format | Data (Default in bold) |
| --- | --- | --- | --- | --- |
| Simulation Rate | Simulation Rate | communicate | Rate {0} | Increase,**Decrease** |


---

