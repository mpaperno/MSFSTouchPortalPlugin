using MSFSTouchPortalPlugin.Attributes;

namespace MSFSTouchPortalPlugin.InstrumentsSystems {
  [TouchPortalCategory("InstrumentsSystems.Fuel", "MSFS - Fuel")]
  internal enum Fuel {
    [SimActionEvent]
    [TouchPortalAction("AddFuel", "Add Fuel", "MSFS", "Adds 25% Fuel", "")]
    ADD_FUEL_QUANTITY,

    CROSS_FEED_OFF,
    CROSS_FEED_OPEN,
    CROSS_FEED_TOGGLE,

    TOGGLE_FUEL_VALVE_ALL,
    TOGGLE_FUEL_VALVE_ENG1,
    TOGGLE_FUEL_VALVE_ENG2,
    TOGGLE_FUEL_VALVE_ENG3,
    TOGGLE_FUEL_VALVE_ENG4,

    FUEL_SELECTOR_OFF,
    FUEL_SELECTOR_ALL,

    [SimActionEvent]
    [TouchPortalAction("FuelSelectorLeft", "Select Left Fuel Tank", "MSFS", "Selects the left fuel tank", "")]
    FUEL_SELECTOR_LEFT,

    [SimActionEvent] // Broken?
    [TouchPortalAction("FuelSelectorRight", "Select Right Fuel Tank", "MSFS", "Selects the right fuel tank", "")]
    FUEL_SELECTOR_RIGHT,

    FUEL_SELECTOR_LEFT_AUX,
    FUEL_SELECTOR_RIGHT_AUX,
    FUEL_SELECTOR_CENTER,
    FUEL_SELECTOR_SET,

    FUEL_SELECTOR_2_OFF,
    FUEL_SELECTOR_2_ALL,
    FUEL_SELECTOR_2_LEFT,
    FUEL_SELECTOR_2_RIGHT,
    FUEL_SELECTOR_2_LEFT_AUX,
    FUEL_SELECTOR_2_RIGHT_AUX,
    FUEL_SELECTOR_2_CENTER,
    FUEL_SELECTOR_2_SET,

    FUEL_SELECTOR_3_OFF,
    FUEL_SELECTOR_3_ALL,
    FUEL_SELECTOR_3_LEFT,
    FUEL_SELECTOR_3_RIGHT,
    FUEL_SELECTOR_3_LEFT_AUX,
    FUEL_SELECTOR_3_RIGHT_AUX,
    FUEL_SELECTOR_3_CENTER,
    FUEL_SELECTOR_3_SET,

    FUEL_SELECTOR_4_OFF,
    FUEL_SELECTOR_4_ALL,
    FUEL_SELECTOR_4_LEFT,
    FUEL_SELECTOR_4_RIGHT,
    FUEL_SELECTOR_4_LEFT_AUX,
    FUEL_SELECTOR_4_RIGHT_AUX,
    FUEL_SELECTOR_4_CENTER,
    FUEL_SELECTOR_4_SET,

    TOGGLE_PRIMER,
    TOGGLE_PRIMER1,
    TOGGLE_PRIMER2,
    TOGGLE_PRIMER3,
    TOGGLE_PRIMER4,

    [SimActionEvent]
    [TouchPortalAction("FuelDumpToggle", "Fuel Dump - Toggle", "MSFS", "Toggles the Fuel Dump", "")]
    FUEL_DUMP_TOGGLE
  }
}
