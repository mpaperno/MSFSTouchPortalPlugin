using MSFSTouchPortalPlugin.Attributes;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.InstrumentsSystems {

  [TouchPortalCategory("InstrumentsSystems.Fuel", "MSFS - Fuel")]
  internal class FuelMapping {

    [TouchPortalAction("AddFuel", "Add Fuel", "MSFS", "Adds 25% amount of Fuel", "Add 25% amount of fuel")]
    public object ADD_FUEL { get; }

    [TouchPortalAction("FuelSelector_1", "Fuel Selector 1", "MSFS", "Fuel Selector 1", "Fuel Selector 1 - {0}")]
    [TouchPortalActionChoice(new string[] { "All", "Off", "Left", "Right", "Left - Aux", "Right - Aux", "Center", "Set" }, "All")]
    public object FUEL_SELECTOR_1 { get; }

    [TouchPortalAction("FuelSelector_2", "Fuel Selector 2", "MSFS", "Fuel Selector 2", "Fuel Selector 2 - {0}")]
    [TouchPortalActionChoice(new string[] { "All", "Off", "Left", "Right", "Left - Aux", "Right - Aux", "Center", "Set" }, "All")]
    public object FUEL_SELECTOR_2 { get; }

    [TouchPortalAction("FuelSelector_3", "Fuel Selector 3", "MSFS", "Fuel Selector 3", "Fuel Selector 3 - {0}")]
    [TouchPortalActionChoice(new string[] { "All", "Off", "Left", "Right", "Left - Aux", "Right - Aux", "Center", "Set" }, "All")]
    public object FUEL_SELECTOR_3 { get; }

    [TouchPortalAction("FuelSelector_4", "Fuel Selector 4", "MSFS", "Fuel Selector 4", "Fuel Selector 4 - {0}")]
    [TouchPortalActionChoice(new string[] { "All", "Off", "Left", "Right", "Left - Aux", "Right - Aux", "Center", "Set" }, "All")]
    public object FUEL_SELECTOR_4 { get; }

    [TouchPortalAction("Primers", "Toggle All/Specific Primers", "MSFS", "Toggle All/Specific Primers", "Toggle Primers - {0}")]
    [TouchPortalActionChoice(new string[] { "All", "1", "2", "3", "4" }, "All")]
    public object PRIMERS { get; }


    [TouchPortalAction("FuelDump", "Fuel Dump - Toggle", "MSFS", "Toggles the Fuel Dump", "Toggle Fuel Dump")]
    public object FUEL_DUMP { get; }

    [TouchPortalAction("CrossFeed", "Toggle/Open/Off Cross Feed", "MSFS", "Toggle/Open/Off Cross Feed", "Cross Feed - {0}")]
    [TouchPortalActionChoice(new string[] { "Toggle", "Open", "Off" }, "Open")]
    public object CROSS_FEED { get; }

    [TouchPortalAction("FuelValve", "Toggle All/Specific Fuel Valve", "MSFS", "Toggle All/Specific Fuel Valve", "Toggle Fuel Valve - {0}")]
    [TouchPortalActionChoice(new string[] { "All", "1", "2", "3", "4" }, "All")]
    public object FUEL_VALVE { get; }
  }

  [SimNotificationGroup(SimConnectWrapper.Groups.Fuel)]
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
    [TouchPortalActionMapping("FuelSelector_1", "Off")]
    FUEL_SELECTOR_OFF,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelector_1", "All")]
    FUEL_SELECTOR_ALL,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelector_1", "Left")]
    FUEL_SELECTOR_LEFT,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelector_1", "Right")]
    FUEL_SELECTOR_RIGHT,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelector_1", "Left - Aux")]
    FUEL_SELECTOR_LEFT_AUX,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelector_1", "Right - Aux")]
    FUEL_SELECTOR_RIGHT_AUX,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelector_1", "Center")]
    FUEL_SELECTOR_CENTER,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelector_1", "Set")]
    FUEL_SELECTOR_SET,

    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelector_2", "Off")]
    FUEL_SELECTOR_2_OFF,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelector_2", "All")]
    FUEL_SELECTOR_2_ALL,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelector_2", "Left")]
    FUEL_SELECTOR_2_LEFT,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelector_2", "Right")]
    FUEL_SELECTOR_2_RIGHT,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelector_2", "Left - Aux")]
    FUEL_SELECTOR_2_LEFT_AUX,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelector_2", "Right - Aux")]
    FUEL_SELECTOR_2_RIGHT_AUX,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelector_2", "Center")]
    FUEL_SELECTOR_2_CENTER,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelector_2", "Set")]
    FUEL_SELECTOR_2_SET,

    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelector_3", "Off")]
    FUEL_SELECTOR_3_OFF,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelector_3", "All")]
    FUEL_SELECTOR_3_ALL,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelector_3", "Left")]
    FUEL_SELECTOR_3_LEFT,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelector_3", "Right")]
    FUEL_SELECTOR_3_RIGHT,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelector_3", "Left - Aux")]
    FUEL_SELECTOR_3_LEFT_AUX,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelector_3", "Right - Aux")]
    FUEL_SELECTOR_3_RIGHT_AUX,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelector_3", "Center")]
    FUEL_SELECTOR_3_CENTER,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelector_3", "Set")]
    FUEL_SELECTOR_3_SET,

    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelector_4", "Off")]
    FUEL_SELECTOR_4_OFF,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelector_4", "All")]
    FUEL_SELECTOR_4_ALL,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelector_4", "Left")]
    FUEL_SELECTOR_4_LEFT,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelector_4", "Right")]
    FUEL_SELECTOR_4_RIGHT,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelector_4", "Left - Aux")]
    FUEL_SELECTOR_4_LEFT_AUX,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelector_4", "Right - Aux")]
    FUEL_SELECTOR_4_RIGHT_AUX,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelector_4", "Center")]
    FUEL_SELECTOR_4_CENTER,
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelector_4", "Set")]
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
    FUEL_DUMP_TOGGLE
  }
}
