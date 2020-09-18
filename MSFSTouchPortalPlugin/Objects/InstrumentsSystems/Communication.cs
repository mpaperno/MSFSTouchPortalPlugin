using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Enums;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.InstrumentsSystems {
  [SimVarDataRequestGroup]
  [TouchPortalCategory("Communication", "MSFS - Communication")]
  internal class CommunicationMapping {

    [TouchPortalAction("Radios", "Fuel Selectors", "MSFS", "Fuel Selectors", "Fuel Selector {0} - {1}")]
    [TouchPortalActionChoice(new string[] { "COM1", "COM2", "NAV1", "NAV2" }, "COM1")]
    [TouchPortalActionChoice(new string[] { "Increase 25 KHz", "Increase 1 MHz", "Decrease 25 KHz", "Decrease 1Mhz" }, "Increase 25 KHz")]
    public object Radios { get; }
  }

  [SimNotificationGroup(Groups.Communication)]
  [TouchPortalCategoryMapping("Communication")]
  internal enum Communication {
    // Placeholder to offset each enum for SimConnect
    Init = 10000,

    #region Radios

    COM_STBY_RADIO_SWAP,
    COM_RADIO_WHOLE_DEC,
    COM_RADIO_WHOLE_INC,
    COM_RADIO_FRACT_DEC,
    COM_RADIO_FRACT_DEC_CARRY,
    COM_RADIO_FRACT_INC,
    COM_RADIO_FRACT_INC_CARRY,

    COM2_RADIO_SWAP,
    COM2_RADIO_WHOLE_DEC,
    COM2_RADIO_WHOLE_INC,
    COM2_RADIO_FRACT_DEC,
    COM2_RADIO_FRACT_DEC_CARRY,
    COM2_RADIO_FRACT_INC,
    COM2_RADIO_FRACT_INC_CARRY,

    NAV1_RADIO_SWAP,
    NAV1_RADIO_WHOLE_DEC,
    NAV1_RADIO_WHOLE_INC,
    NAV1_RADIO_FRACT_DEC,
    NAV1_RADIO_FRACT_DEC_CARRY,
    NAV1_RADIO_FRACT_INC,
    NAV1_RADIO_FRACT_INC_CARRY,

    NAV2_RADIO_SWAP,
    NAV2_RADIO_WHOLE_DEC,
    NAV2_RADIO_WHOLE_INC,
    NAV2_RADIO_FRACT_DEC,
    NAV2_RADIO_FRACT_DEC_CARRY,
    NAV2_RADIO_FRACT_INC,
    NAV2_RADIO_FRACT_INC_CARRY,

    COM_RADIO_SET,
    COM_STBY_RADIO_SET,
    COM2_RADIO_SET,
    COM2_STBY_RADIO_SET,
    NAV1_RADIO_SET,
    NAV1_STBY_SET,
    NAV2_RADIO_SET,
    NAV2_STBY_SET,


    #endregion
  }
}
