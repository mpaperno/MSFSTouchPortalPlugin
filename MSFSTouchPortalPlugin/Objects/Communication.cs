/*
This file is part of the MSFS Touch Portal Plugin project.
https://github.com/mpaperno/MSFSTouchPortalPlugin

COPYRIGHT:
(c) 2020 Tim Lewis;
(c) Maxim Paperno; All Rights Reserved.

This file may be used under the terms of the GNU General Public License (GPL)
as published by the Free Software Foundation, either version 3 of the Licenses,
or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

A copy of the GNU GPL is included with this project
and is also available at <http://www.gnu.org/licenses/>.
*/

using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Enums;

namespace MSFSTouchPortalPlugin.Objects
{
  [TouchPortalCategory(Groups.Communication)]
  internal static class CommunicationMapping
  {

    [TouchPortalAction("AdfAdjust", "ADF Adjust", "ADF {0} - {1}", true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("ADF1_RADIO_SWAP",         "1", "Standby Swap")]
    [TouchPortalActionMapping("ADF_1_DEC",               "1", "Decrement frequency by 1 KHz")]
    [TouchPortalActionMapping("ADF_1_INC",               "1", "Increment frequency by 1 KHz")]
    [TouchPortalActionMapping("ADF_10_DEC",              "1", "Decrement frequency by 10 KHz")]
    [TouchPortalActionMapping("ADF_10_INC",              "1", "Increment frequency by 10 KHz")]
    [TouchPortalActionMapping("ADF_100_DEC",             "1", "Decrement frequency by 100 KHz")]
    [TouchPortalActionMapping("ADF_100_INC",             "1", "Increment frequency by 100 KHz")]
    [TouchPortalActionMapping("ADF1_WHOLE_DEC",          "1", "Decrement frequency by 1 KHz with carry")]
    [TouchPortalActionMapping("ADF1_WHOLE_INC",          "1", "Increment frequency by 1 KHz with carry")]
    [TouchPortalActionMapping("ADF1_RADIO_TENTHS_DEC",   "1", "Decrement frequency by 0.1 KHz with carry")]
    [TouchPortalActionMapping("ADF1_RADIO_TENTHS_INC",   "1", "Increment frequency by 0.1 KHz with carry")]
#if !FSX
    [TouchPortalActionMapping("ADF_VOLUME_INC",          "1", "Volume Increase" )]
    [TouchPortalActionMapping("ADF_VOLUME_DEC",          "1", "Volume Decrease" )]
#endif
    [TouchPortalActionMapping("ADF_CARD_DEC",            "1", "Decrement Card by 1° (ADF1 Only)")]
    [TouchPortalActionMapping("ADF_CARD_INC",            "1", "Increment Card by 1° (ADF1 Only)")]
    [TouchPortalActionMapping("RADIO_ADF_IDENT_DISABLE", "1", "IDENT Disable")]
    [TouchPortalActionMapping("RADIO_ADF_IDENT_ENABLE",  "1", "IDENT Enable")]
    [TouchPortalActionMapping("RADIO_ADF_IDENT_TOGGLE",  "1", "IDENT Toggle")]
    [TouchPortalActionMapping("ADF2_RADIO_SWAP",          "2", "Standby Swap")]
    [TouchPortalActionMapping("ADF2_1_DEC",               "2", "Decrement frequency by 1 KHz")]
    [TouchPortalActionMapping("ADF2_1_INC",               "2", "Increment frequency by 1 KHz")]
    [TouchPortalActionMapping("ADF2_10_DEC",              "2", "Decrement frequency by 10 KHz")]
    [TouchPortalActionMapping("ADF2_10_INC",              "2", "Increment frequency by 10 KHz")]
    [TouchPortalActionMapping("ADF2_100_DEC",             "2", "Decrement frequency by 100 KHz")]
    [TouchPortalActionMapping("ADF2_100_INC",             "2", "Increment frequency by 100 KHz")]
    [TouchPortalActionMapping("ADF2_WHOLE_DEC",           "2", "Decrement frequency by 1 KHz with carry")]
    [TouchPortalActionMapping("ADF2_WHOLE_INC",           "2", "Increment frequency by 1 KHz with carry")]
    [TouchPortalActionMapping("ADF2_RADIO_TENTHS_DEC",    "2", "Decrement frequency by 0.1 KHz with carry")]
    [TouchPortalActionMapping("ADF2_RADIO_TENTHS_INC",    "2", "Increment frequency by 0.1 KHz with carry")]
#if !FSX
    [TouchPortalActionMapping("ADF2_VOLUME_INC",          "2", "Volume Increase" )]
    [TouchPortalActionMapping("ADF2_VOLUME_DEC",          "2", "Volume Decrease" )]
#endif
    [TouchPortalActionMapping("RADIO_ADF2_IDENT_DISABLE", "2", "IDENT Disable")]
    [TouchPortalActionMapping("RADIO_ADF2_IDENT_ENABLE",  "2", "IDENT Enable")]
    [TouchPortalActionMapping("RADIO_ADF2_IDENT_TOGGLE",  "2", "IDENT Toggle")]
    public static readonly object AdfAdjust;

    [TouchPortalAction("AdfSet", "ADF Values Set", true,
      "Set ADF {0} {1} to Value {2}",
      "Set ADF {0} {1} in Value\nRange:"
    )]
    [TouchPortalActionChoice()]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("ADF_COMPLETE_SET",    "1", "Active Frequency (BCD32)")]
    [TouchPortalActionMapping("RADIO_ADF_IDENT_SET", "1", "IDENT (0/1)")]
    [TouchPortalActionMapping("ADF_NEEDLE_SET",      "1", "Needle (radians)")]
    [TouchPortalActionMapping("ADF_OUTSIDE_SOURCE",  "1", "Outside Source (0/1)")]
#if !FSX
    [TouchPortalActionMapping("ADF_STBY_SET",        "1", "Standby Frequency (BCD32)")]
    [TouchPortalActionMapping("ADF_VOLUME_SET",      "1", "Volume (0-100)" )]
#endif
    [TouchPortalActionMapping("ADF2_COMPLETE_SET",    "2", "Active Frequency (BCD32)")]
    [TouchPortalActionMapping("RADIO_ADF2_IDENT_SET", "2", "IDENT (0/1)")]
#if !FSX
    [TouchPortalActionMapping("ADF2_NEEDLE_SET",      "2", "Needle (radians)")]
    [TouchPortalActionMapping("ADF2_OUTSIDE_SOURCE",  "2", "Outside Source (0/1)")]
    [TouchPortalActionMapping("ADF2_STBY_SET",        "2", "Standby Frequency (BCD32)")]
    [TouchPortalActionMapping("ADF2_VOLUME_SET",      "2", "Volume (0-100)" )]
#endif
    [TouchPortalActionText("0", float.MinValue, float.MaxValue, AllowDecimals = true)]
    public static readonly object AdfSet;


    [TouchPortalAction("Radios", "Radio Interaction", "Radio {0} - {1}", true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionChoice()]
    // COM1
#if !FSX
    [TouchPortalActionMapping("COM1_RADIO_SWAP", new[] { "COM1", "Standby Swap" })]
#else
    [TouchPortalActionMapping("COM_STBY_RADIO_SWAP", new[] { "COM1", "Standby Swap" })]
#endif
    [TouchPortalActionMapping("COM_RADIO_WHOLE_DEC", new[] { "COM1", "Decrease 1Mhz" })]
    [TouchPortalActionMapping("COM_RADIO_WHOLE_INC", new[] { "COM1", "Increase 1 MHz" })]
    [TouchPortalActionMapping("COM_RADIO_FRACT_DEC", new[] { "COM1", "Decrease 25 KHz" })]
    [TouchPortalActionMapping("COM_RADIO_FRACT_DEC_CARRY", new[] { "COM1", "Decrease 25 KHz w/ Carry Digits" })]
    [TouchPortalActionMapping("COM_RADIO_FRACT_INC", new[] { "COM1", "Increase 25 KHz" })]
    [TouchPortalActionMapping("COM_RADIO_FRACT_INC_CARRY", new[] { "COM1", "Increase 25 KHz w/ Carry Digits" })]
    [TouchPortalActionMapping("RADIO_COMM1_AUTOSWITCH_TOGGLE", new[] { "COM1", "Autoswitch Toggle" })]
#if !FSX
    [TouchPortalActionMapping("COM1_VOLUME_INC", new[] { "COM1", "Volume Increase" })]
    [TouchPortalActionMapping("COM1_VOLUME_DEC", new[] { "COM1", "Volume Decrease" })]
    [TouchPortalActionMapping("PILOT_TRANSMITTER_SET", "COM1", "Pilot Transmit Select (COM only)",   0)]
    [TouchPortalActionMapping("COPILOT_TRANSMITTER_SET", "COM1", "Copilot Transmit Select (COM only)", 0)]
    [TouchPortalActionMapping("COM1_RECEIVE_SELECT",   "COM1", "Receive Select (COM only)",    1)]
    [TouchPortalActionMapping("COM1_RECEIVE_SELECT",   "COM1", "Receive De-select (COM only)", 0)]
    [TouchPortalActionMapping("COM_1_SPACING_MODE_SWITCH", new[] { "COM1", "Toggle Spacing Mode (COM only)" })]
#endif
    // COM2
    [TouchPortalActionMapping("COM2_RADIO_SWAP", new[] { "COM2", "Standby Swap" })]
    [TouchPortalActionMapping("COM2_RADIO_WHOLE_DEC", new[] { "COM2", "Decrease 1Mhz" })]
    [TouchPortalActionMapping("COM2_RADIO_WHOLE_INC", new[] { "COM2", "Increase 1 MHz" })]
    [TouchPortalActionMapping("COM2_RADIO_FRACT_DEC", new[] { "COM2", "Decrease 25 KHz" })]
    [TouchPortalActionMapping("COM2_RADIO_FRACT_DEC_CARRY", new[] { "COM2", "Decrease 25 KHz w/ Carry Digits" })]
    [TouchPortalActionMapping("COM2_RADIO_FRACT_INC", new[] { "COM2", "Increase 25 KHz" })]
    [TouchPortalActionMapping("COM2_RADIO_FRACT_INC_CARRY", new[] { "COM2", "Increase 25 KHz w/ Carry Digits" })]
    [TouchPortalActionMapping("RADIO_COMM2_AUTOSWITCH_TOGGLE", new[] { "COM2", "Autoswitch Toggle" })]
#if !FSX
    [TouchPortalActionMapping("COM2_VOLUME_INC", new[] { "COM2", "Volume Increase" })]
    [TouchPortalActionMapping("COM2_VOLUME_DEC", new[] { "COM2", "Volume Decrease" })]
    [TouchPortalActionMapping("PILOT_TRANSMITTER_SET", "COM2", "Pilot Transmit Select (COM only)",   1)]
    [TouchPortalActionMapping("COPILOT_TRANSMITTER_SET", "COM2", "Copilot Transmit Select (COM1 only)", 1)]
    [TouchPortalActionMapping("COM2_RECEIVE_SELECT",   "COM2", "Receive Select (COM only)",    1)]
    [TouchPortalActionMapping("COM2_RECEIVE_SELECT",   "COM2", "Receive De-select (COM only)", 0)]
    [TouchPortalActionMapping("COM_2_SPACING_MODE_SWITCH", new[] { "COM2", "Toggle Spacing Mode (COM only)" })]
    // COM3
    [TouchPortalActionMapping("COM3_RADIO_SWAP", new[] { "COM3", "Standby Swap" })]
    [TouchPortalActionMapping("COM3_RADIO_WHOLE_DEC", new[] { "COM3", "Decrease 1Mhz" })]
    [TouchPortalActionMapping("COM3_RADIO_WHOLE_INC", new[] { "COM3", "Increase 1 MHz" })]
    [TouchPortalActionMapping("COM3_RADIO_FRACT_DEC", new[] { "COM3", "Decrease 25 KHz" })]
    [TouchPortalActionMapping("COM3_RADIO_FRACT_DEC_CARRY", new[] { "COM3", "Decrease 25 KHz w/ Carry Digits" })]
    [TouchPortalActionMapping("COM3_RADIO_FRACT_INC", new[] { "COM3", "Increase 25 KHz" })]
    [TouchPortalActionMapping("COM3_RADIO_FRACT_INC_CARRY", new[] { "COM3", "Increase 25 KHz w/ Carry Digits" })]
    [TouchPortalActionMapping("COM3_VOLUME_INC", new[] { "COM3", "Volume Increase" })]
    [TouchPortalActionMapping("COM3_VOLUME_DEC", new[] { "COM3", "Volume Decrease" })]
    [TouchPortalActionMapping("PILOT_TRANSMITTER_SET", "COM3", "Pilot Transmit Select (COM only)",   2)]
    [TouchPortalActionMapping("COPILOT_TRANSMITTER_SET", "COM3", "Copilot Transmit Select (COM only)", 2)]
    [TouchPortalActionMapping("COM3_RECEIVE_SELECT",   "COM3", "Receive Select (COM only)",    1)]
    [TouchPortalActionMapping("COM3_RECEIVE_SELECT",   "COM3", "Receive De-select (COM only)", 0)]
    [TouchPortalActionMapping("COM_3_SPACING_MODE_SWITCH", new[] { "COM3", "Toggle Spacing Mode (COM only)" })]
#endif
    // NAV1
    [TouchPortalActionMapping("NAV1_RADIO_SWAP", new[] { "NAV1", "Standby Swap" })]
    [TouchPortalActionMapping("NAV1_RADIO_WHOLE_DEC", new[] { "NAV1", "Decrease 1Mhz" })]
    [TouchPortalActionMapping("NAV1_RADIO_WHOLE_INC", new[] { "NAV1", "Increase 1 MHz" })]
    [TouchPortalActionMapping("NAV1_RADIO_FRACT_DEC", new[] { "NAV1", "Decrease 25 KHz" })]
    [TouchPortalActionMapping("NAV1_RADIO_FRACT_DEC_CARRY", new[] { "NAV1", "Decrease 25 KHz w/ Carry Digits" })]
    [TouchPortalActionMapping("NAV1_RADIO_FRACT_INC", new[] { "NAV1", "Increase 25 KHz" })]
    [TouchPortalActionMapping("NAV1_RADIO_FRACT_INC_CARRY", new[] { "NAV1", "Increase 25 KHz w/ Carry Digits" })]
    [TouchPortalActionMapping("RADIO_NAV1_AUTOSWITCH_TOGGLE", new[] { "NAV1", "Autoswitch Toggle" })]
#if !FSX
    [TouchPortalActionMapping("NAV1_VOLUME_INC", new[] { "NAV1", "Volume Increase" })]
    [TouchPortalActionMapping("NAV1_VOLUME_DEC", new[] { "NAV1", "Volume Decrease" })]
#endif
    // NAV2
    [TouchPortalActionMapping("NAV2_RADIO_SWAP", new[] { "NAV2", "Standby Swap" })]
    [TouchPortalActionMapping("NAV2_RADIO_WHOLE_DEC", new[] { "NAV2", "Decrease 1Mhz" })]
    [TouchPortalActionMapping("NAV2_RADIO_WHOLE_INC", new[] { "NAV2", "Increase 1 MHz" })]
    [TouchPortalActionMapping("NAV2_RADIO_FRACT_DEC", new[] { "NAV2", "Decrease 25 KHz" })]
    [TouchPortalActionMapping("NAV2_RADIO_FRACT_DEC_CARRY", new[] { "NAV2", "Decrease 25 KHz w/ Carry Digits" })]
    [TouchPortalActionMapping("NAV2_RADIO_FRACT_INC", new[] { "NAV2", "Increase 25 KHz" })]
    [TouchPortalActionMapping("NAV2_RADIO_FRACT_INC_CARRY", new[] { "NAV2", "Increase 25 KHz w/ Carry Digits" })]
    [TouchPortalActionMapping("RADIO_NAV2_AUTOSWITCH_TOGGLE", new[] { "NAV2", "Autoswitch Toggle" })]
#if !FSX
    [TouchPortalActionMapping("NAV2_VOLUME_INC", new[] { "NAV2", "Volume Increase" })]
    [TouchPortalActionMapping("NAV2_VOLUME_DEC", new[] { "NAV2", "Volume Decrease" })]
    // NAV3
    [TouchPortalActionMapping("NAV3_RADIO_SWAP", new[] { "NAV3", "Standby Swap" })]
    [TouchPortalActionMapping("NAV3_RADIO_WHOLE_DEC", new[] { "NAV3", "Decrease 1Mhz" })]
    [TouchPortalActionMapping("NAV3_RADIO_WHOLE_INC", new[] { "NAV3", "Increase 1 MHz" })]
    [TouchPortalActionMapping("NAV3_RADIO_FRACT_DEC", new[] { "NAV3", "Decrease 25 KHz" })]
    [TouchPortalActionMapping("NAV3_RADIO_FRACT_DEC_CARRY", new[] { "NAV3", "Decrease 25 KHz w/ Carry Digits" })]
    [TouchPortalActionMapping("NAV3_RADIO_FRACT_INC", new[] { "NAV3", "Increase 25 KHz" })]
    [TouchPortalActionMapping("NAV3_RADIO_FRACT_INC_CARRY", new[] { "NAV3", "Increase 25 KHz w/ Carry Digits" })]
    [TouchPortalActionMapping("NAV3_VOLUME_INC", new[] { "NAV3", "Volume Increase" })]
    [TouchPortalActionMapping("NAV3_VOLUME_DEC", new[] { "NAV3", "Volume Decrease" })]
    // NAV4
    [TouchPortalActionMapping("NAV4_RADIO_SWAP", new[] { "NAV4", "Standby Swap" })]
    [TouchPortalActionMapping("NAV4_RADIO_WHOLE_DEC", new[] { "NAV4", "Decrease 1Mhz" })]
    [TouchPortalActionMapping("NAV4_RADIO_WHOLE_INC", new[] { "NAV4", "Increase 1 MHz" })]
    [TouchPortalActionMapping("NAV4_RADIO_FRACT_DEC", new[] { "NAV4", "Decrease 25 KHz" })]
    [TouchPortalActionMapping("NAV4_RADIO_FRACT_DEC_CARRY", new[] { "NAV4", "Decrease 25 KHz w/ Carry Digits" })]
    [TouchPortalActionMapping("NAV4_RADIO_FRACT_INC", new[] { "NAV4", "Increase 25 KHz" })]
    [TouchPortalActionMapping("NAV4_RADIO_FRACT_INC_CARRY", new[] { "NAV4", "Increase 25 KHz w/ Carry Digits" })]
    [TouchPortalActionMapping("NAV4_VOLUME_INC", new[] { "NAV4", "Volume Increase" })]
    [TouchPortalActionMapping("NAV4_VOLUME_DEC", new[] { "NAV4", "Volume Decrease" })]
    // deprecated
    [TouchPortalActionMapping("COM1_TRANSMIT_SELECT", new[] { "COM1", "Transmit Select" }, Deprecated = true)]
    [TouchPortalActionMapping("COM2_TRANSMIT_SELECT", new[] { "COM2", "Transmit Select" }, Deprecated = true)]
    [TouchPortalActionMapping("COM3_TRANSMIT_SELECT", new[] { "COM3", "Transmit Select" }, Deprecated = true)]
#endif
    public static readonly object Radios;

    [TouchPortalAction("RadiosSet", "Radio Values Set", true,
      "Set Radio {0} {1} to Value {2}",
      "Set Radio {0} {1}in Value\nRange:"
    )]
    [TouchPortalActionChoice()]
    [TouchPortalActionChoice()]
#if !FSX
    // COM1
    [TouchPortalActionMapping("COM_RADIO_SET_HZ", new[] { "COM1", "Frequency (Hz)" })]
    [TouchPortalActionMapping("COM_RADIO_SET", new[] { "COM1", "Frequency (BCD16)" })]
    [TouchPortalActionMapping("COM_STBY_RADIO_SET_HZ", new[] { "COM1", "Standby Frequency (Hz)" })]
    [TouchPortalActionMapping("COM_STBY_RADIO_SET", new[] { "COM1", "Standby Frequency (BCD16)" })]
    [TouchPortalActionMapping("COM1_STORED_FREQUENCY_SET_HZ", new[] { "COM1", "Stored Frequency (Hz) (COM only)" })]
    [TouchPortalActionMapping("COM1_STORED_FREQUENCY_SET", new[] { "COM1", "Stored Frequency (BCD16) (COM only)" })]
    [TouchPortalActionMapping("COM1_VOLUME_SET", new[] { "COM1", "Volume (0.0-1.0)" })]
    [TouchPortalActionMapping("COM1_RECEIVE_SELECT", "COM1", "Receive Select (0/1) (COM only)")]
    // COM2
    [TouchPortalActionMapping("COM2_RADIO_SET_HZ", new[] { "COM2", "Frequency (Hz)" })]
    [TouchPortalActionMapping("COM2_RADIO_SET", new[] { "COM2", "Frequency (BCD16)" })]
    [TouchPortalActionMapping("COM2_STBY_RADIO_SET_HZ", new[] { "COM2", "Standby Frequency (Hz)" })]
    [TouchPortalActionMapping("COM2_STBY_RADIO_SET", new[] { "COM2", "Standby Frequency (BCD16)" })]
    [TouchPortalActionMapping("COM2_STORED_FREQUENCY_SET_HZ", new[] { "COM2", "Stored Frequency (Hz) (COM only)" })]
    [TouchPortalActionMapping("COM2_STORED_FREQUENCY_SET", new[] { "COM2", "Stored Frequency (BCD16) (COM only)" })]
    [TouchPortalActionMapping("COM2_VOLUME_SET", new[] { "COM2", "Volume (0.0-1.0)" })]
    [TouchPortalActionMapping("COM2_RECEIVE_SELECT", "COM2", "Receive Select (0/1) (COM only)")]
    // COM3
    [TouchPortalActionMapping("COM3_RADIO_SET_HZ", new[] { "COM3", "Frequency (Hz)" })]
    [TouchPortalActionMapping("COM3_RADIO_SET", new[] { "COM3", "Frequency (BCD16)" })]
    [TouchPortalActionMapping("COM3_STBY_RADIO_SET_HZ", new[] { "COM3", "Standby Frequency (Hz)" })]
    [TouchPortalActionMapping("COM3_STBY_RADIO_SET", new[] { "COM3", "Standby Frequency (BCD16)" })]
    [TouchPortalActionMapping("COM3_STORED_FREQUENCY_SET_HZ", new[] { "COM3", "Stored Frequency (Hz) (COM only)" })]
    [TouchPortalActionMapping("COM3_STORED_FREQUENCY_SET", new[] { "COM3", "Stored Frequency (BCD16) (COM only)" })]
    [TouchPortalActionMapping("COM3_VOLUME_SET", new[] { "COM3", "Volume (0.0-1.0)" })]
    [TouchPortalActionMapping("COM3_RECEIVE_SELECT", "COM3", "Receive Select (0/1) (COM only)")]
    // NAV1
    [TouchPortalActionMapping("NAV1_RADIO_SET_HZ", new[] { "NAV1", "Frequency (Hz)" })]
    [TouchPortalActionMapping("NAV1_RADIO_SET", new[] { "NAV1", "Frequency (BCD16)" })]
    [TouchPortalActionMapping("NAV1_STBY_SET_HZ", new[] { "NAV1", "Standby Frequency (Hz)" })]
    [TouchPortalActionMapping("NAV1_STBY_SET", new[] { "NAV1", "Standby Frequency (BCD16)" })]
    [TouchPortalActionMapping("NAV1_VOLUME_SET", new[] { "NAV1", "Volume (0.0-1.0)" })]
    // NAV2
    [TouchPortalActionMapping("NAV2_RADIO_SET_HZ", new[] { "NAV2", "Frequency (Hz)" })]
    [TouchPortalActionMapping("NAV2_RADIO_SET", new[] { "NAV2", "Frequency (BCD16)" })]
    [TouchPortalActionMapping("NAV2_STBY_SET_HZ", new[] { "NAV2", "Standby Frequency (Hz)" })]
    [TouchPortalActionMapping("NAV2_STBY_SET", new[] { "NAV2", "Standby Frequency (BCD16)" })]
    [TouchPortalActionMapping("NAV2_VOLUME_SET", new[] { "NAV2", "Volume (0.0-1.0)" })]
    // NAV3
    [TouchPortalActionMapping("NAV3_RADIO_SET_HZ", new[] { "NAV3", "Frequency (Hz)" })]
    [TouchPortalActionMapping("NAV3_RADIO_SET", new[] { "NAV3", "Frequency (BCD16)" })]
    [TouchPortalActionMapping("NAV3_STBY_SET_HZ", new[] { "NAV3", "Standby Frequency (Hz)" })]
    [TouchPortalActionMapping("NAV3_STBY_SET", new[] { "NAV3", "Standby Frequency (BCD16)" })]
    [TouchPortalActionMapping("NAV3_VOLUME_SET", new[] { "NAV3", "Volume (0.0-1.0)" })]
    // NAV4
    [TouchPortalActionMapping("NAV4_RADIO_SET_HZ", new[] { "NAV4", "Frequency (Hz)" })]
    [TouchPortalActionMapping("NAV4_RADIO_SET", new[] { "NAV4", "Frequency (BCD16)" })]
    [TouchPortalActionMapping("NAV4_STBY_SET_HZ", new[] { "NAV4", "Standby Frequency (Hz)" })]
    [TouchPortalActionMapping("NAV4_STBY_SET", new[] { "NAV4", "Standby Frequency (BCD16)" })]
    [TouchPortalActionMapping("NAV4_VOLUME_SET", new[] { "NAV4", "Volume (0.0-1.0)" })]
#else  // FSX
    [TouchPortalActionMapping("COM_RADIO_SET", "COM1", "Frequency (BCD16)")]
    [TouchPortalActionMapping("COM_STBY_RADIO_SET", "COM1", "Standby Frequency (BCD16)")]
    [TouchPortalActionMapping("COM2_RADIO_SET", "COM2", "Frequency (BCD16)")]
    [TouchPortalActionMapping("COM2_STBY_RADIO_SET", "COM2", "Standby Frequency (BCD16)")]
    [TouchPortalActionMapping("NAV1_RADIO_SET", "NAV1", "Frequency (BCD16)")]
    [TouchPortalActionMapping("NAV1_STBY_SET", "NAV1", "Standby Frequency (BCD16)")]
    [TouchPortalActionMapping("NAV2_RADIO_SET", "NAV2", "Frequency (BCD16)")]
    [TouchPortalActionMapping("NAV2_STBY_SET", "NAV2", "Standby Frequency (BCD16)")]
#endif
    [TouchPortalActionText("0", float.MinValue, float.MaxValue, AllowDecimals = true)]
    public static readonly object RadiosSet;

    [TouchPortalAction("XpndrAdjust", "Transponder Adjust", "Transponder Action: {0}", true)]
    [TouchPortalActionChoice()]
#if !FSX
    [TouchPortalActionMapping("XPNDR_IDENT_OFF", "IDENT Off")]
    [TouchPortalActionMapping("XPNDR_IDENT_ON", "IDENT On")]
    [TouchPortalActionMapping("XPNDR_IDENT_TOGGLE", "IDENT Toggle")]
#endif
    [TouchPortalActionMapping("XPNDR", "Cycle Selected Digit for +/-")]
    [TouchPortalActionMapping("XPNDR_1000_DEC", "Decrement the first digit")]
    [TouchPortalActionMapping("XPNDR_1000_INC", "Increment the first digit")]
    [TouchPortalActionMapping("XPNDR_100_DEC", "Decrement the second digit")]
    [TouchPortalActionMapping("XPNDR_100_INC", "Increment the second digit")]
    [TouchPortalActionMapping("XPNDR_10_DEC", "Decrement the third digit")]
    [TouchPortalActionMapping("XPNDR_10_INC", "Increment the third digit")]
    [TouchPortalActionMapping("XPNDR_1_DEC", "Decrement the fourth digit")]
    [TouchPortalActionMapping("XPNDR_1_INC", "Increment the fourth digit")]
    [TouchPortalActionMapping("XPNDR_DEC_CARRY", "Decrement the fourth digit with carry")]
    [TouchPortalActionMapping("XPNDR_INC_CARRY", "Increment the fourth digit with carry")]
    public static readonly object TransponderAdjust;

    [TouchPortalAction("XpndrSet", "Transponder Set", "Set Transponder {0} to Value {1}")]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("XPNDR_SET", "Frequency Code (BCD16)")]
#if !FSX
    [TouchPortalActionMapping("XPNDR_IDENT_SET", "IDENT (0/1)")]
#endif
    [TouchPortalActionText("0", 0, 0x7777, AllowDecimals = false)]
    public static readonly object TransponderSet;

  }

}
