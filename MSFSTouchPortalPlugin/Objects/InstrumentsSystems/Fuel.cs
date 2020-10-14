using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Enums;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.InstrumentsSystems {
  [SimVarDataRequestGroup]
  [TouchPortalCategory("InstrumentsSystems.Fuel", "MSFS - Fuel")]
  internal static class FuelMapping {

    [TouchPortalAction("AddFuel", "Add Fuel", "MSFS", "Adds 25% amount of Fuel", "Add 25% amount of fuel")]
    public static object ADD_FUEL { get; }

    [TouchPortalAction("FuelSelectors", "Fuel Selectors", "MSFS", "Fuel Selectors", "Fuel Selector {0} - {1}")]
    [TouchPortalActionChoice(new [] { "1", "2", "3", "4" }, "1")]
    [TouchPortalActionChoice(new [] { "All", "Off", "Left", "Right", "Left - Main", "Right - Main", "Left - Aux", "Right - Aux", "Center" }, "All")]
    public static object FUEL_SELECTORS { get; }

    [TouchPortalAction("Primers", "Toggle All/Specific Primers", "MSFS", "Toggle All/Specific Primers", "Toggle Primers - {0}")]
    [TouchPortalActionChoice(new [] { "All", "1", "2", "3", "4" }, "All")]
    public static object PRIMERS { get; }


    [TouchPortalAction("FuelDump", "Fuel Dump - Toggle", "MSFS", "Toggles the Fuel Dump", "Toggle Fuel Dump")]
    public static object FUEL_DUMP { get; }

    [TouchPortalAction("CrossFeed", "Toggle/Open/Off Cross Feed", "MSFS", "Toggle/Open/Off Cross Feed", "Cross Feed - {0}")]
    [TouchPortalActionChoice(new [] { "Toggle", "Open", "Off" }, "Open")]
    public static object CROSS_FEED { get; }

    [TouchPortalAction("FuelValve", "Toggle All/Specific Fuel Valve", "MSFS", "Toggle All/Specific Fuel Valve", "Toggle Fuel Valve - {0}")]
    [TouchPortalActionChoice(new [] { "All", "1", "2", "3", "4" }, "All")]
    public static object FUEL_VALVE { get; }

    #region Fuel Pump

    [TouchPortalAction("FuelPump", "Fuel Pump - Toggle", "MSFS", "Toggles the Fuel Pump", "Toggle Fuel Pump")]
    public static object FUEL_PUMP { get; }

    [TouchPortalAction("ElectricFuelPump", "Electric Fuel Pump - Toggle", "MSFS", "Toggles the Electric Fuel Pump", "Toggle Electric Fuel Pump - {0}")]
    [TouchPortalActionChoice(new [] { "All", "1", "2", "3", "4" }, "All")]
    public static object ELECTRIC_FUEL_PUMP { get; }

    #endregion
  }

  [SimNotificationGroup(Groups.Fuel)]
  [TouchPortalCategoryMapping("InstrumentsSystems.Fuel")]
  internal enum Fuel {
    // Placeholder to offset each enum for SimConnect
    Init = 2000,

    // Add Fuel
    [SimActionEvent]
    [TouchPortalActionMapping("AddFuel")]
    ADD_FUEL_QUANTITY,

    [SimActionEvent]
    [TouchPortalActionMapping("CrossFeed", "Off")]
    CROSS_FEED_OFF,
    [SimActionEvent]
    [TouchPortalActionMapping("CrossFeed", "Open")]
    CROSS_FEED_OPEN,
    [SimActionEvent]
    [TouchPortalActionMapping("CrossFeed", "Toggle")]
    CROSS_FEED_TOGGLE,

    [SimActionEvent]
    [TouchPortalActionMapping("FuelValve", "All")]
    TOGGLE_FUEL_VALVE_ALL,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelValve", "1")]
    TOGGLE_FUEL_VALVE_ENG1,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelValve", "2")]
    TOGGLE_FUEL_VALVE_ENG2,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelValve", "3")]
    TOGGLE_FUEL_VALVE_ENG3,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelValve", "4")]
    TOGGLE_FUEL_VALVE_ENG4,

    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelectors", new [] { "1", "Off" })]
    FUEL_SELECTOR_OFF,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelectors", new [] { "1", "All" })]
    FUEL_SELECTOR_ALL,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelectors", new [] { "1", "Left" })]
    FUEL_SELECTOR_LEFT,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelectors", new [] { "1", "Right" })]
    FUEL_SELECTOR_RIGHT,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelectors", new [] { "1", "Left - Main"})]
    FUEL_SELECTOR_LEFT_MAIN,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelectors", new [] { "1", "Right - Main" })]
    FUEL_SELECTOR_RIGHT_MAIN,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelectors", new [] { "1", "Left - Aux" })]
    FUEL_SELECTOR_LEFT_AUX,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelectors", new [] { "1", "Right - Aux" })]
    FUEL_SELECTOR_RIGHT_AUX,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelectors", new [] { "1", "Center" })]
    FUEL_SELECTOR_CENTER,

    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelectors", new [] { "2", "Off" })]
    FUEL_SELECTOR_2_OFF,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelectors", new [] { "2", "All" })]
    FUEL_SELECTOR_2_ALL,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelectors", new [] { "2", "Left" })]
    FUEL_SELECTOR_2_LEFT,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelectors", new [] { "2", "Right" })]
    FUEL_SELECTOR_2_RIGHT,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelectors", new [] { "2", "Left - Main" })]
    FUEL_SELECTOR_2_LEFT_MAIN,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelectors", new [] { "2", "Right - Main" })]
    FUEL_SELECTOR_2_RIGHT_MAIN,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelectors", new [] { "2", "Left - Aux" })]
    FUEL_SELECTOR_2_LEFT_AUX,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelectors", new [] { "2", "Right - Aux" })]
    FUEL_SELECTOR_2_RIGHT_AUX,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelectors", new [] { "2", "Center" })]
    FUEL_SELECTOR_2_CENTER,

    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelectors", new [] { "3", "Off" })]
    FUEL_SELECTOR_3_OFF,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelectors", new [] { "3", "All" })]
    FUEL_SELECTOR_3_ALL,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelectors", new [] { "3", "Left" })]
    FUEL_SELECTOR_3_LEFT,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelectors", new [] { "3", "Right" })]
    FUEL_SELECTOR_3_RIGHT,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelectors", new [] { "3", "Left - Main" })]
    FUEL_SELECTOR_3_LEFT_MAIN,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelectors", new [] { "3", "Right - Main" })]
    FUEL_SELECTOR_3_RIGHT_MAIN,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelectors", new [] { "3", "Left - Aux" })]
    FUEL_SELECTOR_3_LEFT_AUX,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelectors", new [] { "3", "Right - Aux" })]
    FUEL_SELECTOR_3_RIGHT_AUX,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelectors", new [] { "3", "Center" })]
    FUEL_SELECTOR_3_CENTER,

    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelectors", new [] { "4", "Off" })]
    FUEL_SELECTOR_4_OFF,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelectors", new [] { "4", "All" })]
    FUEL_SELECTOR_4_ALL,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelectors", new [] { "4", "Left" })]
    FUEL_SELECTOR_4_LEFT,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelectors", new [] { "4", "Right" })]
    FUEL_SELECTOR_4_RIGHT,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelectors", new [] { "4", "Right - Main" })]
    FUEL_SELECTOR_4_RIGHT_MAIN,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelectors", new [] { "4", "Left - Main" })]
    FUEL_SELECTOR_4_LEFT_MAIN,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelectors", new [] { "4", "Left - Aux" })]
    FUEL_SELECTOR_4_LEFT_AUX,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelectors", new [] { "4", "Right - Aux" })]
    FUEL_SELECTOR_4_RIGHT_AUX,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelectors", new [] { "4", "Center" })]
    FUEL_SELECTOR_4_CENTER,

    /* 
     * (b'FUEL_SELECTOR_SET', '''Sets selector 1 position (see code list below),
	FUEL_TANK_SELECTOR_OFF = 0
	FUEL_TANK_SELECTOR_ALL = 1
	FUEL_TANK_SELECTOR_LEFT = 2
	FUEL_TANK_SELECTOR_RIGHT = 3
	FUEL_TANK_SELECTOR_LEFT_AUX = 4
	FUEL_TANK_SELECTOR_RIGHT_AUX = 5
	FUEL_TANK_SELECTOR_CENTER = 6
	FUEL_TANK_SELECTOR_CENTER2 = 7
	FUEL_TANK_SELECTOR_CENTER3 = 8
	FUEL_TANK_SELECTOR_EXTERNAL1 = 9
	FUEL_TANK_SELECTOR_EXTERNAL2 = 10
	FUEL_TANK_SELECTOR_RIGHT_TIP = 11
	FUEL_TANK_SELECTOR_LEFT_TIP = 12
	FUEL_TANK_SELECTOR_CROSSFEED = 13
	FUEL_TANK_SELECTOR_CROSSFEED_L2R = 14
	FUEL_TANK_SELECTOR_CROSSFEED_R2L = 15
	FUEL_TANK_SELECTOR_BOTH = 16
	FUEL_TANK_SELECTOR_EXTERNAL_ALL = 17
	FUEL_TANK_SELECTOR_ISOLATE = 18''', "Shared Cockpit"), */

    FUEL_SELECTOR_SET,
    FUEL_SELECTOR_2_SET,
    FUEL_SELECTOR_3_SET,
    FUEL_SELECTOR_4_SET,

    [SimActionEvent]
    [TouchPortalActionMapping("Primers", "All")]
    TOGGLE_PRIMER,
    [SimActionEvent]
    [TouchPortalActionMapping("Primers", "1")]
    TOGGLE_PRIMER1,
    [SimActionEvent]
    [TouchPortalActionMapping("Primers", "2")]
    TOGGLE_PRIMER2,
    [SimActionEvent]
    [TouchPortalActionMapping("Primers", "3")]
    TOGGLE_PRIMER3,
    [SimActionEvent]
    [TouchPortalActionMapping("Primers", "4")]
    TOGGLE_PRIMER4,

    // Fuel Dump
    [SimActionEvent]
    [TouchPortalActionMapping("FuelDump")]
    FUEL_DUMP_TOGGLE,

    #region Fuel Pump

    [SimActionEvent]
    [TouchPortalActionMapping("FuelPump")]
    FUEL_PUMP,
    [SimActionEvent]
    [TouchPortalActionMapping("ElectricFuelPump", "All")]
    TOGGLE_ELECT_FUEL_PUMP,
    [SimActionEvent]
    [TouchPortalActionMapping("ElectricFuelPump", "1")]
    TOGGLE_ELECT_FUEL_PUMP1,
    [SimActionEvent]
    [TouchPortalActionMapping("ElectricFuelPump", "2")]
    TOGGLE_ELECT_FUEL_PUMP2,
    [SimActionEvent]
    [TouchPortalActionMapping("ElectricFuelPump", "3")]
    TOGGLE_ELECT_FUEL_PUMP3,
    [SimActionEvent]
    [TouchPortalActionMapping("ElectricFuelPump", "4")]
    TOGGLE_ELECT_FUEL_PUMP4

    #endregion
  }
}
