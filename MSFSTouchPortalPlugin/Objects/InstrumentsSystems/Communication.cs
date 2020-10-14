using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Enums;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.InstrumentsSystems {
  [SimVarDataRequestGroup]
  [TouchPortalCategory("Communication", "MSFS - Communication")]
  internal static class CommunicationMapping {

    [TouchPortalAction("Radios", "Radio Interaction", "MSFS", "Radio Interaction", "Radio {0} - {1}")]
    [TouchPortalActionChoice(new [] { "COM1", "COM2", "NAV1", "NAV2" }, "COM1")]
    [TouchPortalActionChoice(new [] { "Increase 25 KHz", "Increase 1 MHz", "Increase 25 KHz w/ Carry Digits", "Decrease 25 KHz", "Decrease 1Mhz", "Decrease 25 KHz w/ Carry Digits", "Standby Swap" }, "Increase 25 KHz")]
    public static object Radios { get; }

    [SimVarDataRequest]
    [TouchPortalState("Com1ActiveFrequency", "text", "The frequency of the active COM1 radio", "")]
    public static readonly SimVarItem Com1ActiveFrequency =
      new SimVarItem { Def = Definition.Com1ActiveFrequency, SimVarName = "COM ACTIVE FREQUENCY:1", Unit = Units.MHz, CanSet = false, StringFormat = "{0:0.000#}" };

    [SimVarDataRequest]
    [TouchPortalState("Com1StandbyFrequency", "text", "The frequency of the standby COM1 radio", "")]
    public static readonly SimVarItem Com1StandbyFrequency =
      new SimVarItem { Def = Definition.Com1StandbyFrequency, SimVarName = "COM STANDBY FREQUENCY:1", Unit = Units.MHz, CanSet = false, StringFormat = "{0:0.000#}" };

    [SimVarDataRequest]
    [TouchPortalState("Com2ActiveFrequency", "text", "The frequency of the active COM2 radio", "")]
    public static readonly SimVarItem Com2ActiveFrequency =
      new SimVarItem { Def = Definition.Com2ActiveFrequency, SimVarName = "COM ACTIVE FREQUENCY:2", Unit = Units.MHz, CanSet = false, StringFormat = "{0:0.000#}" };

    [SimVarDataRequest]
    [TouchPortalState("Com2StandbyFrequency", "text", "The frequency of the standby COM2 radio", "")]
    public static readonly SimVarItem Com2StandbyFrequency =
      new SimVarItem { Def = Definition.Com2StandbyFrequency, SimVarName = "COM STANDBY FREQUENCY:2", Unit = Units.MHz, CanSet = false, StringFormat = "{0:0.000#}" };

    [SimVarDataRequest]
    [TouchPortalState("Nav1ActiveFrequency", "text", "The frequency of the active NAV1 radio", "")]
    public static readonly SimVarItem Nav1ActiveFrequency =
      new SimVarItem { Def = Definition.Nav1ActiveFrequency, SimVarName = "NAV ACTIVE FREQUENCY:1", Unit = Units.MHz, CanSet = false, StringFormat = "{0:0.000#}" };

    [SimVarDataRequest]
    [TouchPortalState("Nav1StandbyFrequency", "text", "The frequency of the standby NAV1 radio", "")]
    public static readonly SimVarItem Nav1StandbyFrequency =
      new SimVarItem { Def = Definition.Nav1StandbyFrequency, SimVarName = "NAV STANDBY FREQUENCY:1", Unit = Units.MHz, CanSet = false, StringFormat = "{0:0.000#}" };

    [SimVarDataRequest]
    [TouchPortalState("Nav2ActiveFrequency", "text", "The frequency of the active NAV2 radio", "")]
    public static readonly SimVarItem Nav2ActiveFrequency =
      new SimVarItem { Def = Definition.Nav2ActiveFrequency, SimVarName = "NAV ACTIVE FREQUENCY:2", Unit = Units.MHz, CanSet = false, StringFormat = "{0:0.000#}" };

    [SimVarDataRequest]
    [TouchPortalState("Nav2StandbyFrequency", "text", "The frequency of the standby NAV2 radio", "")]
    public static readonly SimVarItem Nav2StandbyFrequency =
      new SimVarItem { Def = Definition.Nav2StandbyFrequency, SimVarName = "NAV STANDBY FREQUENCY:2", Unit = Units.MHz, CanSet = false, StringFormat = "{0:0.000#}" };
  }

  [SimNotificationGroup(Groups.Communication)]
  [TouchPortalCategoryMapping("Communication")]
  internal enum Communication {
    // Placeholder to offset each enum for SimConnect
    Init = 10000,

    #region Radios

    [SimActionEvent]
    [TouchPortalActionMapping("Radios", new [] { "COM1", "Standby Swap" })]
    COM_STBY_RADIO_SWAP,
    [SimActionEvent]
    [TouchPortalActionMapping("Radios", new [] { "COM1", "Decrease 1Mhz" })]
    COM_RADIO_WHOLE_DEC,
    [SimActionEvent]
    [TouchPortalActionMapping("Radios", new [] { "COM1", "Increase 1 MHz" })]
    COM_RADIO_WHOLE_INC,
    [SimActionEvent]
    [TouchPortalActionMapping("Radios", new [] { "COM1", "Decrease 25 KHz" })]
    COM_RADIO_FRACT_DEC,
    [SimActionEvent]
    [TouchPortalActionMapping("Radios", new [] { "COM1", "Decrease 25 KHz w/ Carry Digits" })]
    COM_RADIO_FRACT_DEC_CARRY,
    [SimActionEvent]
    [TouchPortalActionMapping("Radios", new [] { "COM1", "Increase 25 KHz" })]
    COM_RADIO_FRACT_INC,
    [SimActionEvent]
    [TouchPortalActionMapping("Radios", new [] { "COM1", "Increase 25 KHz w/ Carry Digits" })]
    COM_RADIO_FRACT_INC_CARRY,

    [SimActionEvent]
    [TouchPortalActionMapping("Radios", new [] { "COM2", "Standby Swap" })]
    COM2_RADIO_SWAP,
    [SimActionEvent]
    [TouchPortalActionMapping("Radios", new [] { "COM2", "Decrease 1Mhz" })]
    COM2_RADIO_WHOLE_DEC,
    [SimActionEvent]
    [TouchPortalActionMapping("Radios", new [] { "COM2", "Increase 1 MHz" })]
    COM2_RADIO_WHOLE_INC,
    [SimActionEvent]
    [TouchPortalActionMapping("Radios", new [] { "COM2", "Decrease 25 KHz" })]
    COM2_RADIO_FRACT_DEC,
    [SimActionEvent]
    [TouchPortalActionMapping("Radios", new [] { "COM2", "Decrease 25 KHz w/ Carry Digits" })]
    COM2_RADIO_FRACT_DEC_CARRY,
    [SimActionEvent]
    [TouchPortalActionMapping("Radios", new [] { "COM2", "Increase 25 KHz" })]
    COM2_RADIO_FRACT_INC,
    [SimActionEvent]
    [TouchPortalActionMapping("Radios", new [] { "COM2", "Increase 25 KHz w/ Carry Digits" })]
    COM2_RADIO_FRACT_INC_CARRY,

    [SimActionEvent]
    [TouchPortalActionMapping("Radios", new [] { "NAV1", "Standby Swap" })]
    NAV1_RADIO_SWAP,
    [SimActionEvent]
    [TouchPortalActionMapping("Radios", new [] { "NAV1", "Decrease 1Mhz" })]
    NAV1_RADIO_WHOLE_DEC,
    [SimActionEvent]
    [TouchPortalActionMapping("Radios", new [] { "NAV1", "Increase 1 MHz" })]
    NAV1_RADIO_WHOLE_INC,
    [SimActionEvent]
    [TouchPortalActionMapping("Radios", new [] { "NAV1", "Decrease 25 KHz" })]
    NAV1_RADIO_FRACT_DEC,
    [SimActionEvent]
    [TouchPortalActionMapping("Radios", new [] { "NAV1", "Decrease 25 KHz w/ Carry Digits" })]
    NAV1_RADIO_FRACT_DEC_CARRY,
    [SimActionEvent]
    [TouchPortalActionMapping("Radios", new [] { "NAV1", "Increase 25 KHz" })]
    NAV1_RADIO_FRACT_INC,
    [SimActionEvent]
    [TouchPortalActionMapping("Radios", new [] { "NAV1", "Increase 25 KHz w/ Carry Digits" })]
    NAV1_RADIO_FRACT_INC_CARRY,

    [SimActionEvent]
    [TouchPortalActionMapping("Radios", new [] { "NAV2", "Standby Swap" })]
    NAV2_RADIO_SWAP,
    [SimActionEvent]
    [TouchPortalActionMapping("Radios", new [] { "NAV2", "Decrease 1Mhz" })]
    NAV2_RADIO_WHOLE_DEC,
    [SimActionEvent]
    [TouchPortalActionMapping("Radios", new [] { "NAV2", "Increase 1 MHz" })]
    NAV2_RADIO_WHOLE_INC,
    [SimActionEvent]
    [TouchPortalActionMapping("Radios", new [] { "NAV2", "Decrease 25 KHz" })]
    NAV2_RADIO_FRACT_DEC,
    [SimActionEvent]
    [TouchPortalActionMapping("Radios", new [] { "NAV2", "Decrease 25 KHz w/ Carry Digits" })]
    NAV2_RADIO_FRACT_DEC_CARRY,
    [SimActionEvent]
    [TouchPortalActionMapping("Radios", new [] { "NAV2", "Increase 25 KHz" })]
    NAV2_RADIO_FRACT_INC,
    [SimActionEvent]
    [TouchPortalActionMapping("Radios", new [] { "NAV2", "Increase 25 KHz w/ Carry Digits" })]
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
