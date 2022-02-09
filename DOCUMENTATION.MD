# MSFS 2020 TouchPortal Plugin

This plugin will provide a two way interface between Touch Portal and Microsoft Flight Simulator 2020 through SimConnect.

---

## Table of Contents

[Plugin Settings](#pluginsettings)

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

## Plugin Settings

### Connect To Flight Sim on Startup (0/1)

| Read-only | Type | Default Value | Max. Length | Min. Value | Max. Value |
| --- | --- | --- | --- | --- | --- |
| False | number | 1 | N/A | 0 | 1 |

Set to 1 to automatically attempt connection to flight simulator upon Touch Portal startup. Set to 0 to only connect manually via the provided Action.

### Held Action Repeat Rate (ms)

| Read-only | Type | Default Value | Max. Length | Min. Value | Max. Value |
| --- | --- | --- | --- | --- | --- |
| True | number | 450 | N/A | 50 | 2147483647 |

Stores the held action repeat rate, which can be set via the 'MSFS - Plugin - Action Repeat Interval' action.

---

## MSFS - AutoPilot

### Actions

| Name | Description | Type | Format | Data (Default in bold) | Hold |
| --- | --- | --- | --- | --- | --- |
| Airspeed Hold | Toggle/On/Off/Set the airspeed hold for auto pilot | communicate | Airspeed Hold - {0} | **Toggle**,On,Off,Set | False |
| Airspeed Hold Value | Sets the airspeed hold value | communicate | Airspeed Hold Value - {0} | **Select**,Increase,Decrease,Set | True |
| Altitude Hold | Toggle/On/Off the altitude hold for auto pilot | communicate | Altitude Hold - {0} | **Toggle**,On,Off | False |
| Altitude Hold Value | Sets the altitude hold value | communicate | Altitude Hold Value - {0} | **Select**,Increase,Decrease,Set,Set Metric | True |
| AP Max Bank Angle | Increase/Decrease the max bank angle | communicate | Max Bank Angle - {0} | **Increase**,Decrease | True |
| Approach Mode | Toggle/On/Off the approach mode for auto pilot | communicate | Approach Mode - {0} | **Toggle**,On,Off | False |
| Attitude Hold | Toggle/On/Off the attitude hold for auto pilot | communicate | Attitude Hold - {0} | **Toggle**,On,Off | False |
| Attitude Hold Value | Sets the attitude hold value | communicate | Attitude Hold Value - {0} | **Select**,Increase,Decrease | True |
| Auto Brake | Increase/Decrease the auto brake | communicate | Auto Brake - {0} | Increaes,Decrease | True |
| Auto Throttle Mode | Toggles the Arm/GoAround modes for auto throttle | communicate | Toggle Auto Throttle - {0} | **Arm**,GoAround | False |
| AutoPilot | Toggle/On/Off Auto Pilot | communicate | Auto Pilot Master - {0} | **Toggle**,On,Off | False |
| Back Course Mode | Toggle/On/Off the back course mode for auto pilot | communicate | Back Course Mode - {0} | **Toggle**,On,Off | False |
| Flight Director | Toggle the Flight Director for auto pilot | communicate | Flight Director - {0} | **Toggle** | False |
| Flight Director Pitch Sync | Syncs the Flight Director with the current pitch | communicate | Flight Director Pitch Sync |  | False |
| Heading Hold | Toggle/On/Off the heading hold for auto pilot | communicate | Heading Hold - {0} | **Toggle**,On,Off | False |
| Heading Hold Value | Sets the heading hold value | communicate | Heading Hold Value - {0} | **Select**,Increase,Decrease,Set | True |
| Localizer | Toggle/On/Off the localizer for auto pilot | communicate | Localizer - {0} | **Toggle**,On,Off | False |
| Mach Hold | Toggle/On/Off/Set the mach hold for auto pilot | communicate | Mach Hold - {0} | **Toggle**,On,Off,Set | False |
| Mach Hold Value | Sets the mach hold value | communicate | Mach Hold Value - {0} | Select,**Increase**,Decrease | False |
| Nav Mode - Set | Sets the nav to 1 or 2 for Nav mode | communicate | Nav Mode - {0}  | **1**,2 | False |
| Nav1 Mode | Toggle/On/Off the Nav1 mode for auto pilot | communicate | Nav1 Mode - {0} | **Toggle**,On,Off | False |
| Vertical Speed | Toggle the Vertical Speed for auto pilot | communicate | Vertical Speed - {0} | **Toggle** | False |
| Vertical Speed Value | Sets the vertical speed value | communicate | Vertical Speed Value - {0} | **Select**,Increase,Decrease,Set,Set Metric | True |
| Wing Leveler | Toggle/On/Off the Wing Leveler for auto pilot | communicate | Wing Leveler - {0} | **Toggle**,On,Off | False |
| Yaw Dampener | Toggle/On/Off/Set the Yaw Dampener | communicate | Yaw Dampener - {0} | **Toggle**,On,Off,Set | False |


### States

| Id | Type | Description | DefaultValue |
| --- | --- | --- | --- |
| MSFSTouchPortalPlugin.AutoPilot.State.AutoThrottleArm | text | Auto Throttle Armed |  |
| MSFSTouchPortalPlugin.AutoPilot.State.AutoThrottleGoAround | text | Auto Throttle GoAround |  |
| MSFSTouchPortalPlugin.AutoPilot.State.AutoPilotAirSpeedHold | text | AutoPilot Air Speed Status |  |
| MSFSTouchPortalPlugin.AutoPilot.State.AutoPilotAirSpeedVar | text | AutoPilot Air Speed Value |  |
| MSFSTouchPortalPlugin.AutoPilot.State.AutoPilotAltitudeHold | text | AutoPilot Altitude Status |  |
| MSFSTouchPortalPlugin.AutoPilot.State.AutoPilotAltitudeVar | text | AutoPilot Altitude Value |  |
| MSFSTouchPortalPlugin.AutoPilot.State.AutoPilotApproachHold | text | AutoPilot Approach Status |  |
| MSFSTouchPortalPlugin.AutoPilot.State.AutoPilotAttitudeHold | text | AutoPilot Attitude Status |  |
| MSFSTouchPortalPlugin.AutoPilot.State.AutoPilotAvailable | text | AutoPilot Availability |  |
| MSFSTouchPortalPlugin.AutoPilot.State.AutoPilotBackCourseHold | text | AutoPilot Back Course Status |  |
| MSFSTouchPortalPlugin.AutoPilot.State.AutoPilotFlightDirector | text | AutoPilot Flight Director Status |  |
| MSFSTouchPortalPlugin.AutoPilot.State.AutoPilotHeadingVar | text | AutoPilot Heading Direction |  |
| MSFSTouchPortalPlugin.AutoPilot.State.AutoPilotHeadingHold | text | AutoPilot Heading Status |  |
| MSFSTouchPortalPlugin.AutoPilot.State.AutoPilotMach | text | AutoPilot Mach Hold |  |
| MSFSTouchPortalPlugin.AutoPilot.State.AutoPilotMachVar | text | AutoPilot Mach Value |  |
| MSFSTouchPortalPlugin.AutoPilot.State.AutoPilotMaster | text | AutoPilot Master Status |  |
| MSFSTouchPortalPlugin.AutoPilot.State.AutoPilotBanking | text | AutoPilot Max Bank Angle |  |
| MSFSTouchPortalPlugin.AutoPilot.State.AutoPilotNavSelected | text | AutoPilot Nav Selected Index |  |
| MSFSTouchPortalPlugin.AutoPilot.State.AutoPilotNav1Hold | text | AutoPilot Nav1 Status |  |
| MSFSTouchPortalPlugin.AutoPilot.State.AutoPilotAttitudeVar | text | AutoPilot Pitch Reference Value |  |
| MSFSTouchPortalPlugin.AutoPilot.State.AutoPilotVerticalSpeedHold | text | AutoPilot Vertical Speed Status |  |
| MSFSTouchPortalPlugin.AutoPilot.State.AutoPilotVerticalSpeedVar | text | AutoPilot Vertical Speed Value |  |
| MSFSTouchPortalPlugin.AutoPilot.State.AutoPilotWingLeveler | text | AutoPilot Wing Leveler |  |
| MSFSTouchPortalPlugin.AutoPilot.State.AutoPilotFlightDirectorCurrentBank | text | Flight Director Current Bank |  |
| MSFSTouchPortalPlugin.AutoPilot.State.AutoPilotFlightDirectorCurrentPitch | text | Flight Director Current Pitch |  |
| MSFSTouchPortalPlugin.AutoPilot.State.AutoPilotPitchHold | text | The status of Auto Pilot Pitch Hold button |  |
| MSFSTouchPortalPlugin.AutoPilot.State.AutoPilotYawDampener | text | Yaw Dampener Status |  |


---

## MSFS - Communication

### Actions

| Name | Description | Type | Format | Data (Default in bold) | Hold |
| --- | --- | --- | --- | --- | --- |
| Radio Interaction | Radio Interaction | communicate | Radio {0} - {1} | **COM1**,COM2,NAV1,NAV2 | True |


### States

| Id | Type | Description | DefaultValue |
| --- | --- | --- | --- |
| MSFSTouchPortalPlugin.Communication.State.Com1ActiveFrequency | text | The frequency of the active COM1 radio |  |
| MSFSTouchPortalPlugin.Communication.State.Com2ActiveFrequency | text | The frequency of the active COM2 radio |  |
| MSFSTouchPortalPlugin.Communication.State.Nav1ActiveFrequency | text | The frequency of the active NAV1 radio |  |
| MSFSTouchPortalPlugin.Communication.State.Nav2ActiveFrequency | text | The frequency of the active NAV2 radio |  |
| MSFSTouchPortalPlugin.Communication.State.Com1StandbyFrequency | text | The frequency of the standby COM1 radio |  |
| MSFSTouchPortalPlugin.Communication.State.Com2StandbyFrequency | text | The frequency of the standby COM2 radio |  |
| MSFSTouchPortalPlugin.Communication.State.Nav1StandbyFrequency | text | The frequency of the standby NAV1 radio |  |
| MSFSTouchPortalPlugin.Communication.State.Nav2StandbyFrequency | text | The frequency of the standby NAV2 radio |  |


---

## MSFS - Electrical

### Actions

| Name | Description | Type | Format | Data (Default in bold) | Hold |
| --- | --- | --- | --- | --- | --- |
| Alternator - Specific | Toggle Specific Alternator | communicate | Toggle Altenator - {0} | **1**,2,3,4 | False |
| Master Alternator | Toggle Master Alternator | communicate | Toggle Master Alternator |  | False |
| Master Battery | Toggle Master Battery | communicate | Toggle Master Battery |  | False |
| Master Battery & Alternator | Toggle Master Battery & Alternator | communicate | Toggle Master Battery & Alternator |  | False |
| Toggle All/Specific Lights | Toggle All/Specific Lights | communicate | Toggle Lights - {0} | **All**,Beacon,Taxi,Logo,Recognition,Wing,Nav,Cabin | False |
| Toggle/On/Off/Set Landing Lights | Toggle/On/Off/Set Landing Lights | communicate | Landing Lights - {0} | **Toggle**,On,Off,Set | False |
| Toggle/On/Off/Set Panel Lights | Toggle/On/Off/Set Panel Lights | communicate | Panel Lights - {0} | **Toggle**,On,Off,Set | False |
| Toggle/On/Off/Set Strobe Lights | Toggle/On/Off/Set Strobe Lights | communicate | Strobe Lights - {0} | **Toggle**,On,Off,Set | False |


### States

| Id | Type | Description | DefaultValue |
| --- | --- | --- | --- |
| MSFSTouchPortalPlugin.Electrical.State.LightBeaconOn | text | Light Beacon Switch Status |  |
| MSFSTouchPortalPlugin.Electrical.State.LightBrakeOn | text | Light Brake Switch or Light Status |  |
| MSFSTouchPortalPlugin.Electrical.State.LightCabinOn | text | Light Cabin Switch Status |  |
| MSFSTouchPortalPlugin.Electrical.State.LightHeadOn | text | Light Head Switch or Light Status |  |
| MSFSTouchPortalPlugin.Electrical.State.LightLandingOn | text | Light Landing Switch Status |  |
| MSFSTouchPortalPlugin.Electrical.State.LightLogoOn | text | Light Logo Switch Status |  |
| MSFSTouchPortalPlugin.Electrical.State.LightNavOn | text | Light Nav Switch Status |  |
| MSFSTouchPortalPlugin.Electrical.State.LightPanelOn | text | Light Panel Switch Status |  |
| MSFSTouchPortalPlugin.Electrical.State.LightRecognitionOn | text | Light Recognition Switch Status |  |
| MSFSTouchPortalPlugin.Electrical.State.LightStrobeOn | text | Light Strobe Switch Status |  |
| MSFSTouchPortalPlugin.Electrical.State.LightTaxiOn | text | Light Taxi Switch Status |  |
| MSFSTouchPortalPlugin.Electrical.State.LightWingOn | text | Light Wing Switch Status |  |
| MSFSTouchPortalPlugin.Electrical.State.MasterAlternator | text | Master Alternator Status |  |
| MSFSTouchPortalPlugin.Electrical.State.MasterBattery | text | Master Battery Status |  |


---

## MSFS - Engine

### Actions

| Name | Description | Type | Format | Data (Default in bold) | Hold |
| --- | --- | --- | --- | --- | --- |
| Engine Auto Start/Shutdown | Start/Shutdown Engine | communicate | Engine - {0} | **Start**,Shutdown | False |
| Magnetos Specific | Toggle Magneto Specific | communicate | Toggle Magneto {0} - {1} | **1**,2,3,4 | False |
| Master Ignition Switch | Toggle Master Ignition Switch | communicate | Toggle Master Ignition Switch |  | False |
| Mixture | Sets all mixtures | communicate | All Mixtures - {0} | **Rich**,Increase,Increase Small,Decrease,Decrease Small,Lean,Best,Set | True |
| Mixture Specific | Sets mixture on specific engine | communicate | Mixture {0} - {1} | **1**,2,3,4 | True |
| Propeller Pitch | Adjusts propeller pitch levers/feather | communicate | Pitch {0} - {1} | **All**,1,2,3,4 | True |
| Throttle | Sets all throttles | communicate | All Throttles - {0} | **Full**,Increase,Increase Small,Decrease,Decrease Small,Cut,Set,10%,20%,30%,40%,50%,60%,70%,80%,90% | False |
| Throttle Specific | Sets Throttle on specific engine | communicate | Throttle {0} - {1} | **1**,2,3,4 | True |
| Toggle All Magnetos | Toggle All Magnetos | communicate | Toggle All Magnetos - {0} | **Start**,Off,Right,Left,Both,Decrease,Increase | False |
| Toggle Starters | Toggle Starters | communicate | Toggle Starter - {0} | **All**,1,2,3,4 | False |


### States

| Id | Type | Description | DefaultValue |
| --- | --- | --- | --- |
| MSFSTouchPortalPlugin.Engine.State.MasterIgnitionSwitch | text | Master Ignition Switch Status |  |
| MSFSTouchPortalPlugin.Engine.State.MixtureEngine1 | text | Mixture - Engine 1 - Percentage |  |
| MSFSTouchPortalPlugin.Engine.State.MixtureEngine2 | text | Mixture - Engine 2 - Percentage |  |
| MSFSTouchPortalPlugin.Engine.State.MixtureEngine3 | text | Mixture - Engine 3 - Percentage |  |
| MSFSTouchPortalPlugin.Engine.State.MixtureEngine4 | text | Mixture - Engine 4 - Percentage |  |
| MSFSTouchPortalPlugin.Engine.State.Propeller1FeatherSw | text | Propeller - Engine 1 - Feather Switch State (bool) |  |
| MSFSTouchPortalPlugin.Engine.State.Propeller1Feathered | text | Propeller - Engine 1 - Feathered (bool) |  |
| MSFSTouchPortalPlugin.Engine.State.PropellerEngine1 | text | Propeller - Engine 1 - Percentage |  |
| MSFSTouchPortalPlugin.Engine.State.Propeller2FeatherSw | text | Propeller - Engine 2 - Feather Switch State (bool) |  |
| MSFSTouchPortalPlugin.Engine.State.Propeller2Feathered | text | Propeller - Engine 2 - Feathered (bool) |  |
| MSFSTouchPortalPlugin.Engine.State.PropellerEngine2 | text | Propeller - Engine 2 - Percentage |  |
| MSFSTouchPortalPlugin.Engine.State.Propeller3FeatherSw | text | Propeller - Engine 3 - Feather Switch State (bool) |  |
| MSFSTouchPortalPlugin.Engine.State.Propeller3Feathered | text | Propeller - Engine 3 - Feathered (bool) |  |
| MSFSTouchPortalPlugin.Engine.State.PropellerEngine3 | text | Propeller - Engine 3 - Percentage |  |
| MSFSTouchPortalPlugin.Engine.State.Propeller4FeatherSw | text | Propeller - Engine 4 - Feather Switch State (bool) |  |
| MSFSTouchPortalPlugin.Engine.State.Propeller4Feathered | text | Propeller - Engine 4 - Feathered (bool) |  |
| MSFSTouchPortalPlugin.Engine.State.PropellerEngine4 | text | Propeller - Engine 4 - Percentage |  |
| MSFSTouchPortalPlugin.Engine.State.RPMN1Engine1 | text | RPM - Engine 1 |  |
| MSFSTouchPortalPlugin.Engine.State.RPMN1Engine2 | text | RPM - Engine 2 |  |
| MSFSTouchPortalPlugin.Engine.State.RPMN1Engine3 | text | RPM - Engine 3 |  |
| MSFSTouchPortalPlugin.Engine.State.RPMN1Engine4 | text | RPM - Engine 4 |  |
| MSFSTouchPortalPlugin.Engine.State.RPMPropeller1 | text | RPM - Propeller 1 |  |
| MSFSTouchPortalPlugin.Engine.State.RPMPropeller2 | text | RPM - Propeller 2 |  |
| MSFSTouchPortalPlugin.Engine.State.RPMPropeller3 | text | RPM - Propeller 3 |  |
| MSFSTouchPortalPlugin.Engine.State.RPMPropeller4 | text | RPM - Propeller 4 |  |
| MSFSTouchPortalPlugin.Engine.State.ThrottleEngine1 | text | Throttle - Engine 1 - Percentage |  |
| MSFSTouchPortalPlugin.Engine.State.ThrottleEngine2 | text | Throttle - Engine 2 - Percentage |  |
| MSFSTouchPortalPlugin.Engine.State.ThrottleEngine3 | text | Throttle - Engine 3 - Percentage |  |
| MSFSTouchPortalPlugin.Engine.State.ThrottleEngine4 | text | Throttle - Engine 4 - Percentage |  |


---

## MSFS - Environment

### Actions

| Name | Description | Type | Format | Data (Default in bold) | Hold |
| --- | --- | --- | --- | --- | --- |
| Anti-Ice | Toggle/On/Off Anti Ice | communicate | Anti Ice - {0} | **Toggle**,On,Off,Set | False |
| Anti-Ice Engine | Toggle/On/Off Anti Ice Engine | communicate | Anti Ice Engine {0} - {1} | **1**,2,3,4 | False |
| Pitot Heat | Toggle/On/Off Pitot Heat | communicate | Pitot Heat - {0} | **Toggle**,On,Off,Set | False |
| Propeller De-ice | Toggle Propeller DeIce | communicate | Toggle Propeller DeIce |  | False |
| Structural De-ice | Toggle Structural DeIce | communicate | Toggle Structural DeIce |  | False |


### States

| Id | Type | Description | DefaultValue |
| --- | --- | --- | --- |
| MSFSTouchPortalPlugin.Environment.State.AntiIceEng1 | text | Anti-Ice Engine 1 |  |
| MSFSTouchPortalPlugin.Environment.State.AntiIceEng2 | text | Anti-Ice Engine 2 |  |
| MSFSTouchPortalPlugin.Environment.State.AntiIceEng3 | text | Anti-Ice Engine 3 |  |
| MSFSTouchPortalPlugin.Environment.State.AntiIceEng4 | text | Anti-Ice Engine 4 |  |
| MSFSTouchPortalPlugin.Environment.State.AntiIcePanelSwitch | text | Panel Anti-Ice Switch |  |
| MSFSTouchPortalPlugin.Environment.State.PitotHeat | text | Pitot Heat Status |  |
| MSFSTouchPortalPlugin.Environment.State.PitotHeatSwitch1 | text | Pitot Heat Switch 1 State (0=Off; 1=On; 2=Auto) |  |
| MSFSTouchPortalPlugin.Environment.State.PitotHeatSwitch2 | text | Pitot Heat Switch 2 State (0=Off; 1=On; 2=Auto) |  |
| MSFSTouchPortalPlugin.Environment.State.PitotHeatSwitch3 | text | Pitot Heat Switch 3 State (0=Off; 1=On; 2=Auto) |  |
| MSFSTouchPortalPlugin.Environment.State.PitotHeatSwitch4 | text | Pitot Heat Switch 4 State (0=Off; 1=On; 2=Auto) |  |
| MSFSTouchPortalPlugin.Environment.State.AntiIcePropeller1Switch | text | Propeller 1 Deice Switch |  |
| MSFSTouchPortalPlugin.Environment.State.AntiIcePropeller2Switch | text | Propeller 2 Deice Switch |  |
| MSFSTouchPortalPlugin.Environment.State.AntiIcePropeller3Switch | text | Propeller 3 Deice Switch |  |
| MSFSTouchPortalPlugin.Environment.State.AntiIcePropeller4Switch | text | Propeller 4 Deice Switch |  |
| MSFSTouchPortalPlugin.Environment.State.AntiIceStructuralSwitch | text | Structural Deice Switch |  |
| MSFSTouchPortalPlugin.Environment.State.AntiIceWindshieldSwitch | text | Windshield Deice Switch |  |


---

## MSFS - Failures

### Actions

| Name | Description | Type | Format | Data (Default in bold) | Hold |
| --- | --- | --- | --- | --- | --- |
| Failures | Toggle Failures | communicate | Toggle Failures - {0} | **Electrical**,Vacuum,Pitot,Static Port,Hydraulic,Total Brake,Left Brake,Right Brake,Engine 1,Engine 2,Engine 3,Engine 4 | False |


---

## MSFS - Flight Instruments

### States

| Id | Type | Description | DefaultValue |
| --- | --- | --- | --- |
| MSFSTouchPortalPlugin.FlightInstruments.State.AirSpeedIndicated | text | Air speed indicated in Knots |  |
| MSFSTouchPortalPlugin.FlightInstruments.State.AirSpeedMach | text | Air speed indicated in mach |  |
| MSFSTouchPortalPlugin.FlightInstruments.State.AirSpeedTrue | text | Air speed true in Knots |  |
| MSFSTouchPortalPlugin.FlightInstruments.State.FlapSpeedExceeeded | text | Flap Speed Exceeded Warning true/false |  |
| MSFSTouchPortalPlugin.FlightInstruments.State.GroundAltitude | text | Ground level in Feet |  |
| MSFSTouchPortalPlugin.FlightInstruments.State.GroundVelocity | text | Ground Speed in Knots |  |
| MSFSTouchPortalPlugin.FlightInstruments.State.OverspeedWarning | text | Overspeed Warning true/false |  |
| MSFSTouchPortalPlugin.FlightInstruments.State.PlaneAltitudeAGL | text | Plane Altitude AGL in Feet |  |
| MSFSTouchPortalPlugin.FlightInstruments.State.PlaneAltitude | text | Plane Altitude in Feet |  |
| MSFSTouchPortalPlugin.FlightInstruments.State.PlaneBankAngle | text | Plane Bank Angle in Degrees |  |
| MSFSTouchPortalPlugin.FlightInstruments.State.PlaneHeadingMagnetic | text | Plane Heading (Magnetic North) in Degrees |  |
| MSFSTouchPortalPlugin.FlightInstruments.State.PlaneHeadingTrue | text | Plane Heading (True North) in Degrees |  |
| MSFSTouchPortalPlugin.FlightInstruments.State.PlanePitchAngle | text | Plane Pitch Angle in Degrees |  |
| MSFSTouchPortalPlugin.FlightInstruments.State.StallWarning | text | Stall Warning true/false |  |
| MSFSTouchPortalPlugin.FlightInstruments.State.VerticalSpeed | text | Vertical Speed in feet per minute |  |


---

## MSFS - Flight Systems

### Actions

| Name | Description | Type | Format | Data (Default in bold) | Hold |
| --- | --- | --- | --- | --- | --- |
| Aileron Trim | Aileron Trim | communicate | Aileron Trim - {0} | **Left**,Right | True |
| Ailerons | Ailerons | communicate | Ailerons - {0} | **Center**,Left,Right,Set | True |
| Brakes | Brakes | communicate | Brakes - {0} | **All**,Left,Right | True |
| Cowl Flaps 1 | Cowl Flaps 1 | communicate | Cowl Flaps 1 - {0} | **Increase**,Decrease | True |
| Cowl Flaps 2 | Cowl Flaps 2 | communicate | Cowl Flaps 2 - {0} | **Increase**,Decrease | True |
| Cowl Flaps 3 | Cowl Flaps 3 | communicate | Cowl Flaps 3 - {0} | **Increase**,Decrease | True |
| Cowl Flaps 4 | Cowl Flaps 4 | communicate | Cowl Flaps 4 - {0} | **Increase**,Decrease | True |
| Cowl Flaps All | Cowl Flaps All | communicate | Cowl Flaps All - {0} | **Increase**,Decrease | True |
| Elevator Trim | Elevator Trim | communicate | Elevator Trim - {0} | **Up**,Down | True |
| Flaps | Flaps | communicate | Flaps - {0} | **Up**,Down,Increase,Decrease,1,2,3,Set | False |
| Gear Manipulation | Gear Manipulation | communicate | Gear - {0} | **Toggle**,Up,Down,Set,Pump | False |
| Rudder | Rudder | communicate | Rudder - {0} | **Center**,Left,Right,Set | True |
| Rudder Trim | Rudder Trim | communicate | Rudder Trim - {0} | **Left**,Right | True |
| Spoilers | Spoilers | communicate | Spoilers - {0} | **Toggle**,On,Off,Set | False |
| Spoilers Arm | Spoilers Arm | communicate | Spoilers Arm - {0} | **Toggle**,On,Off,Set | False |
| Toggle Parking Brake | Toggle Parking Brake | communicate | Toggle Parking Brake |  | False |


### States

| Id | Type | Description | DefaultValue |
| --- | --- | --- | --- |
| MSFSTouchPortalPlugin.FlightSystems.State.AileronTrim | text | Aileron Trim Angle |  |
| MSFSTouchPortalPlugin.FlightSystems.State.AileronTrimPct | text | Aileron Trim Percent | 0 |
| MSFSTouchPortalPlugin.FlightSystems.State.CowlFlaps1Percent | text | Cowl Flaps 1 Opened Percentage |  |
| MSFSTouchPortalPlugin.FlightSystems.State.CowlFlaps2Percent | text | Cowl Flaps 2 Opened Percentage |  |
| MSFSTouchPortalPlugin.FlightSystems.State.CowlFlaps3Percent | text | Cowl Flaps 3 Opened Percentage |  |
| MSFSTouchPortalPlugin.FlightSystems.State.CowlFlaps4Percent | text | Cowl Flaps 4 Opened Percentage |  |
| MSFSTouchPortalPlugin.FlightSystems.State.ElevatorTrim | text | Elevator Trim Angle |  |
| MSFSTouchPortalPlugin.FlightSystems.State.ElevatorTrimPct | text | Elevator Trim Percent | 0 |
| MSFSTouchPortalPlugin.FlightSystems.State.FlapsHandlePercent | text | Flaps Handle Percentage |  |
| MSFSTouchPortalPlugin.FlightSystems.State.ParkingBrakeIndicator | text | Parking Brake Indicator true/false |  |
| MSFSTouchPortalPlugin.FlightSystems.State.RudderTrim | text | Rudder Trim Angle |  |
| MSFSTouchPortalPlugin.FlightSystems.State.RudderTrimPct | text | Rudder Trim Percent | 0 |
| MSFSTouchPortalPlugin.FlightSystems.State.GearTotalExtended | text | Total percentage of gear extended |  |


---

## MSFS - Fuel

### Actions

| Name | Description | Type | Format | Data (Default in bold) | Hold |
| --- | --- | --- | --- | --- | --- |
| Add Fuel | Adds 25% amount of Fuel | communicate | Add 25% amount of fuel |  | False |
| Electric Fuel Pump - Toggle | Toggles the Electric Fuel Pump | communicate | Toggle Electric Fuel Pump - {0} | **All**,1,2,3,4 | False |
| Fuel Dump - Toggle | Toggles the Fuel Dump | communicate | Toggle Fuel Dump |  | False |
| Fuel Pump - Toggle | Toggles the Fuel Pump | communicate | Toggle Fuel Pump |  | False |
| Fuel Selectors | Fuel Selectors | communicate | Fuel Selector {0} - {1} | **1**,2,3,4 | False |
| Toggle All/Specific Fuel Valve | Toggle All/Specific Fuel Valve | communicate | Toggle Fuel Valve - {0} | **All**,1,2,3,4 | False |
| Toggle All/Specific Primers | Toggle All/Specific Primers | communicate | Toggle Primers - {0} | **All**,1,2,3,4 | False |
| Toggle/Open/Off Cross Feed | Toggle/Open/Off Cross Feed | communicate | Cross Feed - {0} | Toggle,**Open**,Off | False |


---

## MSFS - Plugin

### Actions

| Name | Description | Type | Format | Data (Default in bold) | Hold |
| --- | --- | --- | --- | --- | --- |
| Action Repeat Interval | Held Action Repeat Rate (ms) | communicate | Repeat Interval: {0} milliseconds | Increment 50ms,Decrement 50ms,50,100,200,250,300,350,400,**450**,500,550,600,700,800,900,1000 | True |
| Connection | Toggle/On/Off SimConnect Connection | communicate | SimConnect Connection - {0} | **Toggle**,On,Off | False |


### States

| Id | Type | Description | DefaultValue |
| --- | --- | --- | --- |
| MSFSTouchPortalPlugin.Plugin.State.ActionRepeatInterval | text | The current Held Action Repeat Rate (ms) | 450 |


---

## MSFS - System

### Actions

| Name | Description | Type | Format | Data (Default in bold) | Hold |
| --- | --- | --- | --- | --- | --- |
| Change Selected Value (+/-) | Selected Value | communicate | Value {0} | Increase,**Decrease** | True |
| Simulation Rate | Simulation Rate | communicate | Rate {0} | Increase,**Decrease** | True |


### States

| Id | Type | Description | DefaultValue |
| --- | --- | --- | --- |
| MSFSTouchPortalPlugin.SimSystem.State.AtcId | text | Aircraft Id used by ATC |  |
| MSFSTouchPortalPlugin.SimSystem.State.AircraftTitle | text | Aircraft Title |  |
| MSFSTouchPortalPlugin.SimSystem.State.AtcAirline | text | Airline used by ATC |  |
| MSFSTouchPortalPlugin.SimSystem.State.AtcFlightNumber | text | Flight Number used by ATC |  |
| MSFSTouchPortalPlugin.SimSystem.State.AtcModel | text | Model of aircraft used by ATC |  |
| MSFSTouchPortalPlugin.SimSystem.State.SimulationRate | text | The current simulation rate |  |
| MSFSTouchPortalPlugin.SimSystem.State.AtcType | text | Type of aircraft used by ATC |  |


---

