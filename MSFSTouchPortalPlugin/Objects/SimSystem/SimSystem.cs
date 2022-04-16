using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Enums;
using MSFSTouchPortalPlugin.Types;

namespace MSFSTouchPortalPlugin.Objects.SimSystem
{
  [TouchPortalCategory(Groups.SimSystem)]
  internal static class SimSystemMapping
  {
    [TouchPortalAction("SimulationRate", "Simulation Rate", "MSFS", "Simulation Rate", "Rate {0}", true)]
    [TouchPortalActionChoice(new [] { "Increase", "Decrease" }, "Decrease")]
    [TouchPortalActionMapping("SIM_RATE_INCR", "Increase")]
    [TouchPortalActionMapping("SIM_RATE_DECR", "Decrease")]
    public static readonly object SimulationRate;

    [TouchPortalAction("SelectedParameter", "Change Selected Value (+/-)", "MSFS", "Selected Value", "Value {0}", true)]
    [TouchPortalActionChoice(new[] { "Increase", "Decrease" }, "Decrease")]
    [TouchPortalActionMapping("PLUS", "Increase")]
    [TouchPortalActionMapping("MINUS", "Decrease")]
    public static readonly object SELECTED_PARAMETER_CHANGE;


    // Event

    public static readonly TouchPortalEvent SimSystemEvent = new("SimSystemEvent", "Simulator System Event", "On Simulator Event $val",
      new System.Collections.Generic.Dictionary<EventIds, string>() {
       { EventIds.SimConnecting,         "Connecting"              },
       { EventIds.SimConnected,          "Connected"               },
       { EventIds.SimDisconnected,       "Disconnected"            },
       { EventIds.SimTimedOut,           "Connection Timed Out"    },
       { EventIds.SimError,              "SimConnect Error"        },
       { EventIds.PluginError,           "Plugin Error"            },
       { EventIds.PluginInfo,            "Plugin Information"      },
       { EventIds.Paused,                "Paused"                  },
       { EventIds.Unpaused,              "Unpaused"                },
       { EventIds.Pause,                 "Pause Toggled"           },
     //{ EventIds.PauseFrame,            "Pause Frame"             },  // doesn't seem to work
       { EventIds.SimStart,              "Flight Started"          },
       { EventIds.SimStop,               "Flight Stopped"          },
       { EventIds.Sim,                   "Flight  Toggled"         },
       { EventIds.AircraftLoaded,        "Aircraft Loaded"         },
       { EventIds.Crashed,               "Crashed"                 },
       { EventIds.CrashReset,            "Crash Reset"             },
       { EventIds.FlightLoaded,          "Flight Loaded"           },
       { EventIds.FlightSaved,           "Flight Saved"            },
       { EventIds.FlightPlanActivated,   "Flight Plan Activated"   },
       { EventIds.FlightPlanDeactivated, "Flight Plan Deactivated" },
       { EventIds.PositionChanged,       "Position Changed"        },
       { EventIds.Sound,                 "Sound Toggled"           },
       { EventIds.ViewCockpit,           "View 3D Cockpit"         },
       { EventIds.ViewExternal,          "View External"           },
    });
  }
}
