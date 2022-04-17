using System.ComponentModel.DataAnnotations;

namespace MSFSTouchPortalPlugin.Enums
{
  // Note that the Display.Name properties are used in TP in the event type selector and cannot be changed w/out breaking any events a plugin user may have already set up.
  // The Display.Description properties are used for documentation generation.
  public enum EventIds : short
  {
    None = 0,
    Ignore,            // a way to flag > Info log entries which shouldn't trigger a PluginError/SimError events

    // Plugin events
    [Display(Name = "Connecting", Description = "Upon every connection attempt to the Simulator.")]
    SimConnecting,

    [Display(Name = "Connected", Description = "Upon successful connection to the Simulator.")]
    SimConnected,

    [Display(Name = "Disconnected", Description = "Upon disconnection from the Simulator.")]
    SimDisconnected,

    [Display(Name = "Connection Timed Out", Description = "When a connection attempt to the Simulator times out, eg. when sim is not running.")]
    SimTimedOut,

    [Display(Name = "SimConnect Error", Description = "When a Simulator (SimConnect) error or warning is detected. Error details (log entry) are sent in the SimSystemEventData state value.")]
    SimError,

    [Display(Name = "Plugin Error", Description = "When a Plugin-specific error or warning is detected (eg. could not parse action data, load a file, etc). Error details (log entry) are sent in the SimSystemEventData state value.")]
    PluginError,

    [Display(Name = "Plugin Information", Description = "When a notable plugin informational (\"success\") action is detected. Information details (log entry) are sent in the SimSystemEventData state value.")]
    PluginInfo,

    // SimConnect events (must match names accepted by SimConnect_SubscribeToSystemEvent)
    // https://docs.flightsimulator.com/html/Programming_Tools/SimConnect/API_Reference/Events_And_Data/SimConnect_SubscribeToSystemEvent.htm
    SimEventNone,      // marker

    [Display(Name = "Aircraft Loaded", Description = "When the aircraft flight dynamics file is changed. These files have a .AIR extension. The filename is sent in the SimSystemEventData state value.")]
    AircraftLoaded,

    [Display(Name = "Crashed", Description = "When the user aircraft crashes.")]
    Crashed,

    [Display(Name = "Crash Reset", Description = "When the crash cut-scene has completed.")]
    CrashReset,

    [Display(Name = "Flight Loaded", Description = "When a flight is loaded. Note that when a flight is ended, a default flight is typically loaded, so these events will occur when flights and missions are started and finished. The filename of the flight loaded is sent in the SimSystemEventData state value.")]
    FlightLoaded,

    [Display(Name = "Flight Saved", Description = "When a flight is saved correctly. The filename of the flight saved is sent in the SimSystemEventData state value.")]
    FlightSaved,

    [Display(Name = "Flight Plan Activated", Description = "When a new flight plan is activated. The filename of the activated flight plan is sent in the SimSystemEventData state value.")]
    FlightPlanActivated,

    [Display(Name = "Flight Plan Deactivated", Description = "When the active flight plan is de-activated.")]
    FlightPlanDeactivated,

    //[Display(Description = "Upon every visual frame")]
    //Frame,      // not working, no notification

    [Display(Name = "Pause Toggled", Description = "When the flight is paused or unpaused, and also immediately returns the current pause state (1 = paused or 0 = unpaused).")]
    Pause,

    [Display(Name = "Paused", Description = "When the flight is paused.")]
    Paused,

    //[Display(Description = "Upon every visual frame that the simulation is paused.")]
    //PauseFrame,  // no notification (I've read this could be used to detect active pause, but...?)

    [Display(Name = "Position Changed", Description = "When the user changes the position of their aircraft through a dialog or loading a flight.")]
    PositionChanged,

    [Display(Name = "Flight Toggled", Description = "When the flight changes between running and not.")]
    Sim,

    [Display(Name = "Flight Started", Description = "The simulator is running. Typically the user is actively controlling the aircraft on the ground or in the air. However, in some cases additional pairs of SimStart/SimStop events are sent. For example, when a flight is reset the events that are sent are SimStop, SimStart, SimStop, SimStart.")]
    SimStart,

    [Display(Name = "Flight Stopped", Description = "The simulator is not running. Typically the user is loading a flight, navigating the shell or in a dialog.")]
    SimStop,

    [Display(Name = "Sound Toggled", Description = "When the master sound switch is changed.")]
    Sound,

    [Display(Name = "Unpaused", Description = "When the flight is un-paused.")]
    Unpaused,

    [Display(Name = "View Changed", Description = "When the user aircraft view is changed. This request will also return the current view immediately. A Enum type is returned in the dwData parameter (0 = External, 2 = Virtual cockpit, .. possibly others for FSX?).")]
    View,              // this one is actually 2 events, ViewCockpit or ViewExternal (below) and is currently not presented to the user as a choice.

    SimEventLast,      // marker

    // "virtual" SimConnect events
    [Display(Name = "View 3D Cockpit", Description = "When the view changes to the 3D virtual cockpit view.")]
    ViewCockpit,       // View event if dwData == 2

    [Display(Name = "View External", Description = "When the view changes to an external view.")]
    ViewExternal,      // View event if dwData == 0

    // Start marker for dynamically generated events (TP actions)
    // Dynamically generated SimConnect client event IDs are "parented" to this enum type,
    // meaning they become of this Type when they need to be cast to en Enum type (eg. for SimConnect C# API).
    // This is done by ActionEventType as needed to generate unique event IDs for SimConnect.
    DynamicEventInit = 1000,
  }
}
