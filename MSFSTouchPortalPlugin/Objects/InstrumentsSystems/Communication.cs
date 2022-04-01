using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Enums;

namespace MSFSTouchPortalPlugin.Objects.InstrumentsSystems
{
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
    public static readonly object Radios;

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
