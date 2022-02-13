using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Enums;
using TouchPortalExtension.Attributes;

namespace MSFSTouchPortalPlugin.Objects.InstrumentsSystems 
{
  [SimVarDataRequestGroup]
  [SimNotificationGroup(Groups.Communication)]
  [TouchPortalCategory("Communication", "MSFS - Communication")]
  internal static class CommunicationMapping {

    [TouchPortalAction("Radios", "Radio Interaction", "MSFS", "Radio Interaction", "Radio {0} - {1}", true)]
    [TouchPortalActionChoice(new [] { "COM1", "COM2", "NAV1", "NAV2" })]
    [TouchPortalActionChoice(new [] { "Increase 25 KHz", "Increase 1 MHz", "Increase 25 KHz w/ Carry Digits", "Decrease 25 KHz", "Decrease 1Mhz", "Decrease 25 KHz w/ Carry Digits", "Standby Swap" })]
    [TouchPortalActionMapping("COM_STBY_RADIO_SWAP",        new[] { "COM1", "Standby Swap" })]
    [TouchPortalActionMapping("COM_RADIO_WHOLE_DEC",        new[] { "COM1", "Decrease 1Mhz" })]
    [TouchPortalActionMapping("COM_RADIO_WHOLE_INC",        new[] { "COM1", "Increase 1 MHz" })]
    [TouchPortalActionMapping("COM_RADIO_FRACT_DEC",        new[] { "COM1", "Decrease 25 KHz" })]
    [TouchPortalActionMapping("COM_RADIO_FRACT_DEC_CARRY",  new[] { "COM1", "Decrease 25 KHz w/ Carry Digits" })]
    [TouchPortalActionMapping("COM_RADIO_FRACT_INC",        new[] { "COM1", "Increase 25 KHz" })]
    [TouchPortalActionMapping("COM_RADIO_FRACT_INC_CARRY",  new[] { "COM1", "Increase 25 KHz w/ Carry Digits" })]
    [TouchPortalActionMapping("COM2_RADIO_SWAP",            new[] { "COM2", "Standby Swap" })]
    [TouchPortalActionMapping("COM2_RADIO_WHOLE_DEC",       new[] { "COM2", "Decrease 1Mhz" })]
    [TouchPortalActionMapping("COM2_RADIO_WHOLE_INC",       new[] { "COM2", "Increase 1 MHz" })]
    [TouchPortalActionMapping("COM2_RADIO_FRACT_DEC",       new[] { "COM2", "Decrease 25 KHz" })]
    [TouchPortalActionMapping("COM2_RADIO_FRACT_DEC_CARRY", new[] { "COM2", "Decrease 25 KHz w/ Carry Digits" })]
    [TouchPortalActionMapping("COM2_RADIO_FRACT_INC",       new[] { "COM2", "Increase 25 KHz" })]
    [TouchPortalActionMapping("COM2_RADIO_FRACT_INC_CARRY", new[] { "COM2", "Increase 25 KHz w/ Carry Digits" })]
    [TouchPortalActionMapping("NAV1_RADIO_SWAP",            new[] { "NAV1", "Standby Swap" })]
    [TouchPortalActionMapping("NAV1_RADIO_WHOLE_DEC",       new[] { "NAV1", "Decrease 1Mhz" })]
    [TouchPortalActionMapping("NAV1_RADIO_WHOLE_INC",       new[] { "NAV1", "Increase 1 MHz" })]
    [TouchPortalActionMapping("NAV1_RADIO_FRACT_DEC",       new[] { "NAV1", "Decrease 25 KHz" })]
    [TouchPortalActionMapping("NAV1_RADIO_FRACT_DEC_CARRY", new[] { "NAV1", "Decrease 25 KHz w/ Carry Digits" })]
    [TouchPortalActionMapping("NAV1_RADIO_FRACT_INC",       new[] { "NAV1", "Increase 25 KHz" })]
    [TouchPortalActionMapping("NAV1_RADIO_FRACT_INC_CARRY", new[] { "NAV1", "Increase 25 KHz w/ Carry Digits" })]
    [TouchPortalActionMapping("NAV2_RADIO_SWAP",            new[] { "NAV2", "Standby Swap" })]
    [TouchPortalActionMapping("NAV2_RADIO_WHOLE_DEC",       new[] { "NAV2", "Decrease 1Mhz" })]
    [TouchPortalActionMapping("NAV2_RADIO_WHOLE_INC",       new[] { "NAV2", "Increase 1 MHz" })]
    [TouchPortalActionMapping("NAV2_RADIO_FRACT_DEC",       new[] { "NAV2", "Decrease 25 KHz" })]
    [TouchPortalActionMapping("NAV2_RADIO_FRACT_DEC_CARRY", new[] { "NAV2", "Decrease 25 KHz w/ Carry Digits" })]
    [TouchPortalActionMapping("NAV2_RADIO_FRACT_INC",       new[] { "NAV2", "Increase 25 KHz" })]
    [TouchPortalActionMapping("NAV2_RADIO_FRACT_INC_CARRY", new[] { "NAV2", "Increase 25 KHz w/ Carry Digits" })]
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

/*
  COM_RADIO_SET,
  COM_STBY_RADIO_SET,
  COM2_RADIO_SET,
  COM2_STBY_RADIO_SET,
  NAV1_RADIO_SET,
  NAV1_STBY_SET,
  NAV2_RADIO_SET,
  NAV2_STBY_SET,
*/

}
