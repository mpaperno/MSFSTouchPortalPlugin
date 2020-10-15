# MSFS Touch Portal Plugin

## 0.5.3 (10-14-2020)
* Added new states:
  * AtcType - Type of aircraft used by ATC
  * AtcModel - Model of aircraft used by ATC
  * AtcId - Aircraft Id used by ATC
  * AtcAirline -- Airline used by ATC
  * AtcFlightNumber - Flight Number used by ATC
  * AircraftTitle - Aircraft Title

## 0.5.2 (10-13-2020)
* Modified states to not have units. These can be added through Touch Portal if user chooses.
* De-Ice engine actions have been combined. Will require re-configuring on Page.

### New/Modified Actions
* Flaps - Up, Down, Increase, Decrease, 1, 2, 3
* Parking Brake - Toggle
* Rudder Trim - Left / Right
* Elevator Trim - Up / Down
* Aileron Trim - Left / Right
* Throttle All Engines - Full, Increase, Increase Small, Decrease, Decrease Small, Cut, 10%-90%
* Throttle Specific - Full, Increase, Increase Small, Decrease, Decrease Small, Cut
* Anti-Ice - Toggle / On / Off
* Anti-Ice Engine - Toggle for engines 1, 2, 3, 4
* PitotHeat - Toggle / On / Off

### New/Modified States
* Aileron Trim
* Elevator Trim
* Flaps Handle Percentage
* Parking Brake Indicator
* Rudder Trim
* MixtureEngine1
* MixtureEngine2
* MixtureEngine3
* MixtureEngine4
* PropellerEngine1
* PropellerEngine2
* PropellerEngine3
* PropellerEngine4
* RPMN1Engine1
* RPMN1Engine2
* RPMN1Engine3
* RPMN1Engine4
* ThrottleEngine1
* ThrottleEngine2
* ThrottleEngine3
* ThrottleEngine4
* AntiIceEng1
* AntiIceEng2
* AntiIceEng3
* AntiIceEng4
* PitotHeat
* AirSpeedIndicated
* AirSpeedMach
* AirSpeedTrue
* FlapSpeedExceeded
* OverspeedWarning
* StallWarning
* VerticalSpeed

## 0.5.1 (10-9-2020)
Entry.tp file is now properly alphabetized.

* New States:
  * SimulationRate
  * Com1ActiveFrequency
  * Com1StandbyFrequency
  * Com2ActiveFrequency
  * Com2StandbyFrequency
  * Nav1ActiveFrequency
  * Nav1StandbyFrequency
  * Nav2ActiveFrequency
  * Nav2StandbyFrequency
* New Actions:
  * Simulation Rate - Increase / Decrease
  * Radio Interaction
    * COM1/COM2/NAV1/NAV2 - Increase/Decrease 25 KHz/1 MHz / Carry Digits / Standby Swap

* Fixed Auto Throttle Go Around, it was previously sharing a state value with the Auto Throttle Arm.

## 0.5.0 (9-27-2020)
* Added Vertical Speed Hold Toggle
* Fixed plugin connected status states
* Improved startup and resiliency

## 0.4.0 (9-15-2020)
* Now only allows for a single instance of the executable to run.
* Exe will keep running. Will properly disconnect from MSFS when you quit MSFS and then you can start up the simulator again and it will reconnect without having to restart the plugin.
* Aileron/Rudder/Elevator values
* More Autopilot related button states
* Landing gear state - 1 means extended.

## 0.3.0 (9-11-2020)
More controls, many button states, and flight instrument variables.

Polling of state data is done at 250ms intervals. Should update quick enough.

## 0.2.2 (9-8-2020)
Fixed path for the plugin start command.

## 0.2.1 (9-7-2020)
Added Yaw Dampener controls.

## 0.2.0 (9-7-2020)
Expanded with lots of controls for Flight Control Systems and Electrical.

* Trimming
* Flaps
* Brakes
* Gear
* Lights
* Additional Auto Pilot options and settings
* More!

## 0.1.0 (9-7-2020)
This release is primarily as a test run. It Supports a handful of Autopilot functions and a Fuel Selector.
AddFuel doesn't work at the moment.