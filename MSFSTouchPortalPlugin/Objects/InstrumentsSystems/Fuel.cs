using MSFSTouchPortalPlugin.Attributes;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.InstrumentsSystems {

  [TouchPortalCategory("InstrumentsSystems.Fuel", "MSFS - Fuel")]
  internal class FuelMapping {
    
    [TouchPortalAction("AddFuel", "Add Fuel", "MSFS", "Adds X amount of Fuel", "Add {0}% amount of fuel")]
    public object ADD_FUEL { get; }

    [TouchPortalAction("FuelSelectorLeft", "Select Left Fuel Tank", "MSFS", "Selects the left fuel tank", "")]
    public object FUEL_SELECTOR_LEFT { get; }

    [TouchPortalAction("FuelSelectorRight", "Select Right Fuel Tank", "MSFS", "Selects the right fuel tank", "")]
    public object FUEL_SELECTOR_RIGHT { get; }

    [TouchPortalAction("FuelDump", "Fuel Dump - Toggle", "MSFS", "Toggles the Fuel Dump", "Toggle Fuel Dump")]
    public object FUEL_DUMP { get; }
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

    // Fuel Selector Left
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelectorLeft")]
    FUEL_SELECTOR_LEFT,

    // Fuel Selector Right
    [SimActionEvent]
    [TouchPortalActionMapping("FuelSelectorRight")]
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

    // Fuel Dump
    [SimActionEvent]
    [TouchPortalActionMapping("FuelDump")]
    FUEL_DUMP_TOGGLE
  }
}
