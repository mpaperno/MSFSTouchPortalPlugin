
namespace MSFSTouchPortalPlugin.Enums
{
  public enum EventIds : short
  {
    None = 0,

    // Plugin events
    SimConnecting,     // attempting to connect
    SimConnected,
    SimDisconnected,
    SimTimedOut,       // connection error, sim not running
    SimError,          // SimConnect error
    PluginError,       // internal plugin error

    // SimConnect events (must match names accepted by SimConnect_SubscribeToSystemEvent, see comment block at EOF)
    SimEventNone,      // marker
    //1sec,
    //4sec,
    //6Hz,
    AircraftLoaded,
    Crashed,
    CrashReset,
    FlightLoaded,
    FlightSaved,
    FlightPlanActivated,
    FlightPlanDeactivated,
    //Frame,           // no notification
    Pause,
    Paused,
    //PauseFrame,      // no notification
    PositionChanged,
    Sim,
    SimStart,
    SimStop,
    Sound,
    Unpaused,
    View,              // this one is actually 2 events, ViewCockpit or ViewExternal  (below)
    SimEventLast,      // marker

    // "virtual" events
    ViewCockpit,       // View event if dwData == 2
    ViewExternal,      // View event if dwData == 0

    // start marker for dynamically generated events (TP actions)
    DynamicEventInit = 1000,
  }
}

/*  https://docs.flightsimulator.com/html/Programming_Tools/SimConnect/API_Reference/Events_And_Data/SimConnect_SubscribeToSystemEvent.htm

1sec                   Request a notification every second.
4sec                   Request a notification every four seconds.
6Hz                    Request notifications six times per second. This is the same rate that joystick movement events are transmitted.
AircraftLoaded         Request a notification when the aircraft flight dynamics file is changed. These files have a .AIR extension. The filename is returned in a SIMCONNECT_RECV_EVENT_FILENAME structure.
Crashed                Request a notification if the user aircraft crashes.
CrashReset             Request a notification when the crash cut-scene has completed.
FlightLoaded           Request a notification when a flight is loaded. Note that when a flight is ended, a default flight is typically loaded, so these events will occur when flights and missions are started and finished. The filename of the flight loaded is returned in a SIMCONNECT_RECV_EVENT_FILENAME structure.
FlightSaved            Request a notification when a flight is saved correctly. The filename of the flight saved is returned in a SIMCONNECT_RECV_EVENT_FILENAME structure.
FlightPlanActivated    Request a notification when a new flight plan is activated. The filename of the activated flight plan is returned in a SIMCONNECT_RECV_EVENT_FILENAME structure.
FlightPlanDeactivated  Request a notification when the active flight plan is de-activated.
Frame                  Request notifications every visual frame. Information is returned in a SIMCONNECT_RECV_EVENT structure.
Pause                  Request notifications when the flight is paused or unpaused, and also immediately returns the current pause state (1 = paused or 0 = unpaused). The state is returned in the dwData parameter.
Paused                 Request a notification when the flight is paused.
PauseFrame             Request notifications for every visual frame that the simulation is paused. Information is returned in a SIMCONNECT_RECV_EVENT structure.
PositionChanged        Request a notification when the user changes the position of their aircraft through a dialog.
Sim                    Request notifications when the flight is running or not, and also immediately returns the current state (1 = running or 0 = not running). The state is returned in the dwData parameter.
SimStart               The simulator is running. Typically the user is actively controlling the aircraft on the ground or in the air. However, in some cases additional pairs of SimStart/SimStop events are sent. For example, when a flight is reset the events that are sent are SimStop, SimStart, SimStop, SimStart. Also when a flight is started with the SHOW_OPENING_SCREEN value set to zero, then an additional SimStart/SimStop pair are sent before a second SimStart event is sent when the scenery is fully loaded. The opening screen provides the options to change aircraft, departure airport, and so on.
SimStop                The simulator is not running. Typically the user is loading a flight, navigating the shell or in a dialog.
Sound                  Requests a notification when the master sound switch is changed. This request will also return the current state of the master sound switch immediately. A flag is returned in the dwData parameter, 0 if the switch is off, SIMCONNECT_SOUND_SYSTEM_EVENT_DATA_MASTER (0x1) if the switch is on.
Unpaused               Request a notification when the flight is un-paused.
View                   Requests a notification when the user aircraft view is changed. This request will also return the current view immediately. A flag is returned in the dwData parameter, one of: SIMCONNECT_VIEW_SYSTEM_EVENT_DATA_COCKPIT_2D SIMCONNECT_VIEW_SYSTEM_EVENT_DATA_COCKPIT_VIRTUAL SIMCONNECT_VIEW_SYSTEM_EVENT_DATA_ORTHOGONAL (the map view).
 */
