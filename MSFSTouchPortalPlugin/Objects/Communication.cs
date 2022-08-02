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

    [TouchPortalAction("Radios", "Radio Interaction", "Radio {0} - {1}", true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("COM1_RADIO_SWAP", new[] { "COM1", "Standby Swap" })]
    [TouchPortalActionMapping("COM_RADIO_WHOLE_DEC", new[] { "COM1", "Decrease 1Mhz" })]
    [TouchPortalActionMapping("COM_RADIO_WHOLE_INC", new[] { "COM1", "Increase 1 MHz" })]
    [TouchPortalActionMapping("COM_RADIO_FRACT_DEC", new[] { "COM1", "Decrease 25 KHz" })]
    [TouchPortalActionMapping("COM_RADIO_FRACT_DEC_CARRY", new[] { "COM1", "Decrease 25 KHz w/ Carry Digits" })]
    [TouchPortalActionMapping("COM_RADIO_FRACT_INC", new[] { "COM1", "Increase 25 KHz" })]
    [TouchPortalActionMapping("COM_RADIO_FRACT_INC_CARRY", new[] { "COM1", "Increase 25 KHz w/ Carry Digits" })]
    [TouchPortalActionMapping("COM1_VOLUME_INC", new[] { "COM1", "Volume Increase" })]
    [TouchPortalActionMapping("COM1_VOLUME_DEC", new[] { "COM1", "Volume Decrease" })]
    [TouchPortalActionMapping("COM1_TRANSMIT_SELECT", new[] { "COM1", "Transmit Select" })]
    [TouchPortalActionMapping("COM1_RECEIVE_SELECT", new[] { "COM1", "Receive Select" })]
    [TouchPortalActionMapping("COM_1_SPACING_MODE_SWITCH", new[] { "COM1", "Toggle Spacing Mode (COM only)" })]
    [TouchPortalActionMapping("RADIO_COMM1_AUTOSWITCH_TOGGLE", new[] { "COM1", "Autoswitch Toggle" })]
    [TouchPortalActionMapping("COM2_RADIO_SWAP", new[] { "COM2", "Standby Swap" })]
    [TouchPortalActionMapping("COM2_RADIO_WHOLE_DEC", new[] { "COM2", "Decrease 1Mhz" })]
    [TouchPortalActionMapping("COM2_RADIO_WHOLE_INC", new[] { "COM2", "Increase 1 MHz" })]
    [TouchPortalActionMapping("COM2_RADIO_FRACT_DEC", new[] { "COM2", "Decrease 25 KHz" })]
    [TouchPortalActionMapping("COM2_RADIO_FRACT_DEC_CARRY", new[] { "COM2", "Decrease 25 KHz w/ Carry Digits" })]
    [TouchPortalActionMapping("COM2_RADIO_FRACT_INC", new[] { "COM2", "Increase 25 KHz" })]
    [TouchPortalActionMapping("COM2_RADIO_FRACT_INC_CARRY", new[] { "COM2", "Increase 25 KHz w/ Carry Digits" })]
    [TouchPortalActionMapping("COM2_VOLUME_INC", new[] { "COM2", "Volume Increase" })]
    [TouchPortalActionMapping("COM2_VOLUME_DEC", new[] { "COM2", "Volume Decrease" })]
    [TouchPortalActionMapping("COM2_TRANSMIT_SELECT", new[] { "COM2", "Transmit Select" })]
    [TouchPortalActionMapping("COM2_RECEIVE_SELECT", new[] { "COM2", "Receive Select" })]
    [TouchPortalActionMapping("COM_2_SPACING_MODE_SWITCH", new[] { "COM2", "Toggle Spacing Mode (COM only)" })]
    [TouchPortalActionMapping("RADIO_COMM2_AUTOSWITCH_TOGGLE", new[] { "COM2", "Autoswitch Toggle" })]
    [TouchPortalActionMapping("COM3_RADIO_SWAP", new[] { "COM3", "Standby Swap" })]
    [TouchPortalActionMapping("COM3_RADIO_WHOLE_DEC", new[] { "COM3", "Decrease 1Mhz" })]
    [TouchPortalActionMapping("COM3_RADIO_WHOLE_INC", new[] { "COM3", "Increase 1 MHz" })]
    [TouchPortalActionMapping("COM3_RADIO_FRACT_DEC", new[] { "COM3", "Decrease 25 KHz" })]
    [TouchPortalActionMapping("COM3RADIO_FRACT_DEC_CARRY", new[] { "COM3", "Decrease 25 KHz w/ Carry Digits" })]
    [TouchPortalActionMapping("COM3_RADIO_FRACT_INC", new[] { "COM3", "Increase 25 KHz" })]
    [TouchPortalActionMapping("COM3_RADIO_FRACT_INC_CARRY", new[] { "COM3", "Increase 25 KHz w/ Carry Digits" })]
    [TouchPortalActionMapping("COM3_VOLUME_INC", new[] { "COM3", "Volume Increase" })]
    [TouchPortalActionMapping("COM3_VOLUME_DEC", new[] { "COM3", "Volume Decrease" })]
    [TouchPortalActionMapping("COM3_TRANSMIT_SELECT", new[] { "COM3", "Transmit Select" })]
    [TouchPortalActionMapping("COM3_RECEIVE_SELECT", new[] { "COM3", "Receive Select" })]
    [TouchPortalActionMapping("COM_3_SPACING_MODE_SWITCH", new[] { "COM3", "Toggle Spacing Mode (COM only)" })]
    [TouchPortalActionMapping("NAV1_RADIO_SWAP", new[] { "NAV1", "Standby Swap" })]
    [TouchPortalActionMapping("NAV1_RADIO_WHOLE_DEC", new[] { "NAV1", "Decrease 1Mhz" })]
    [TouchPortalActionMapping("NAV1_RADIO_WHOLE_INC", new[] { "NAV1", "Increase 1 MHz" })]
    [TouchPortalActionMapping("NAV1_RADIO_FRACT_DEC", new[] { "NAV1", "Decrease 25 KHz" })]
    [TouchPortalActionMapping("NAV1_RADIO_FRACT_DEC_CARRY", new[] { "NAV1", "Decrease 25 KHz w/ Carry Digits" })]
    [TouchPortalActionMapping("NAV1_RADIO_FRACT_INC", new[] { "NAV1", "Increase 25 KHz" })]
    [TouchPortalActionMapping("NAV1_RADIO_FRACT_INC_CARRY", new[] { "NAV1", "Increase 25 KHz w/ Carry Digits" })]
    [TouchPortalActionMapping("NAV1_VOLUME_INC", new[] { "NAV1", "Volume Increase" })]
    [TouchPortalActionMapping("NAV1_VOLUME_DEC", new[] { "NAV1", "Volume Decrease" })]
    [TouchPortalActionMapping("RADIO_NAV1_AUTOSWITCH_TOGGLE", new[] { "NAV1", "Autoswitch Toggle" })]
    [TouchPortalActionMapping("NAV2_RADIO_SWAP", new[] { "NAV2", "Standby Swap" })]
    [TouchPortalActionMapping("NAV2_RADIO_WHOLE_DEC", new[] { "NAV2", "Decrease 1Mhz" })]
    [TouchPortalActionMapping("NAV2_RADIO_WHOLE_INC", new[] { "NAV2", "Increase 1 MHz" })]
    [TouchPortalActionMapping("NAV2_RADIO_FRACT_DEC", new[] { "NAV2", "Decrease 25 KHz" })]
    [TouchPortalActionMapping("NAV2_RADIO_FRACT_DEC_CARRY", new[] { "NAV2", "Decrease 25 KHz w/ Carry Digits" })]
    [TouchPortalActionMapping("NAV2_RADIO_FRACT_INC", new[] { "NAV2", "Increase 25 KHz" })]
    [TouchPortalActionMapping("NAV2_RADIO_FRACT_INC_CARRY", new[] { "NAV2", "Increase 25 KHz w/ Carry Digits" })]
    [TouchPortalActionMapping("NAV2_VOLUME_INC", new[] { "NAV2", "Volume Increase" })]
    [TouchPortalActionMapping("NAV2_VOLUME_DEC", new[] { "NAV2", "Volume Decrease" })]
    [TouchPortalActionMapping("RADIO_NAV2_AUTOSWITCH_TOGGLE", new[] { "NAV2", "Autoswitch Toggle" })]
    [TouchPortalActionMapping("NAV3_RADIO_SWAP", new[] { "NAV3", "Standby Swap" })]
    [TouchPortalActionMapping("NAV3_RADIO_WHOLE_DEC", new[] { "NAV3", "Decrease 1Mhz" })]
    [TouchPortalActionMapping("NAV3_RADIO_WHOLE_INC", new[] { "NAV3", "Increase 1 MHz" })]
    [TouchPortalActionMapping("NAV3_RADIO_FRACT_DEC", new[] { "NAV3", "Decrease 25 KHz" })]
    [TouchPortalActionMapping("NAV3_RADIO_FRACT_DEC_CARRY", new[] { "NAV3", "Decrease 25 KHz w/ Carry Digits" })]
    [TouchPortalActionMapping("NAV3_RADIO_FRACT_INC", new[] { "NAV3", "Increase 25 KHz" })]
    [TouchPortalActionMapping("NAV3_RADIO_FRACT_INC_CARRY", new[] { "NAV3", "Increase 25 KHz w/ Carry Digits" })]
    [TouchPortalActionMapping("NAV3_VOLUME_INC", new[] { "NAV3", "Volume Increase" })]
    [TouchPortalActionMapping("NAV3_VOLUME_DEC", new[] { "NAV3", "Volume Decrease" })]
    [TouchPortalActionMapping("NAV4_RADIO_SWAP", new[] { "NAV4", "Standby Swap" })]
    [TouchPortalActionMapping("NAV4_RADIO_WHOLE_DEC", new[] { "NAV4", "Decrease 1Mhz" })]
    [TouchPortalActionMapping("NAV4_RADIO_WHOLE_INC", new[] { "NAV4", "Increase 1 MHz" })]
    [TouchPortalActionMapping("NAV4_RADIO_FRACT_DEC", new[] { "NAV4", "Decrease 25 KHz" })]
    [TouchPortalActionMapping("NAV4_RADIO_FRACT_DEC_CARRY", new[] { "NAV4", "Decrease 25 KHz w/ Carry Digits" })]
    [TouchPortalActionMapping("NAV4_RADIO_FRACT_INC", new[] { "NAV4", "Increase 25 KHz" })]
    [TouchPortalActionMapping("NAV4_RADIO_FRACT_INC_CARRY", new[] { "NAV4", "Increase 25 KHz w/ Carry Digits" })]
    [TouchPortalActionMapping("NAV4_VOLUME_INC", new[] { "NAV4", "Volume Increase" })]
    [TouchPortalActionMapping("NAV4_VOLUME_DEC", new[] { "NAV4", "Volume Decrease" })]
    public static readonly object Radios;

    [TouchPortalAction("RadiosSet", "Radio Values Set", true,
      "Set Radio {0} {1} to Value {2}",
      "Set Radio {0} {1}in Value\nRange:"
    )]
    [TouchPortalActionChoice()]
    [TouchPortalActionChoice()]
    [TouchPortalActionMapping("COM_RADIO_SET_HZ", new[] { "COM1", "Frequency (Hz)" })]
    [TouchPortalActionMapping("COM_RADIO_SET", new[] { "COM1", "Frequency (BCD16)" })]
    [TouchPortalActionMapping("COM_STBY_RADIO_SET_HZ", new[] { "COM1", "Standby Frequency (Hz)" })]
    [TouchPortalActionMapping("COM_STBY_RADIO_SET", new[] { "COM1", "Standby Frequency (BCD16)" })]
    [TouchPortalActionMapping("COM1_STORED_FREQUENCY_SET_HZ", new[] { "COM1", "Stored Frequency (Hz) (COM only)" })]
    [TouchPortalActionMapping("COM1_STORED_FREQUENCY_SET", new[] { "COM1", "Stored Frequency (BCD16) (COM only)" })]
    [TouchPortalActionMapping("COM1_VOLUME_SET", new[] { "COM1", "Volume (0.0-1.0)" })]
    [TouchPortalActionMapping("COM2_RADIO_SET_HZ", new[] { "COM2", "Frequency (Hz)" })]
    [TouchPortalActionMapping("COM2_RADIO_SET", new[] { "COM2", "Frequency (BCD16)" })]
    [TouchPortalActionMapping("COM2_STBY_RADIO_SET_HZ", new[] { "COM2", "Standby Frequency (Hz)" })]
    [TouchPortalActionMapping("COM2_STBY_RADIO_SET", new[] { "COM2", "Standby Frequency (BCD16)" })]
    [TouchPortalActionMapping("COM2_STORED_FREQUENCY_SET_HZ", new[] { "COM2", "Stored Frequency (Hz) (COM only)" })]
    [TouchPortalActionMapping("COM2_STORED_FREQUENCY_SET", new[] { "COM2", "Stored Frequency (BCD16) (COM only)" })]
    [TouchPortalActionMapping("COM2_VOLUME_SET", new[] { "COM2", "Volume (0.0-1.0)" })]
    [TouchPortalActionMapping("COM3_RADIO_SET_HZ", new[] { "COM3", "Frequency (Hz)" })]
    [TouchPortalActionMapping("COM3_RADIO_SET", new[] { "COM3", "Frequency (BCD16)" })]
    [TouchPortalActionMapping("COM3_STBY_RADIO_SET_HZ", new[] { "COM3", "Standby Frequency (Hz)" })]
    [TouchPortalActionMapping("COM3_STBY_RADIO_SET", new[] { "COM3", "Standby Frequency (BCD16)" })]
    [TouchPortalActionMapping("COM3_STORED_FREQUENCY_SET_HZ", new[] { "COM3", "Stored Frequency (Hz) (COM only)" })]
    [TouchPortalActionMapping("COM3_STORED_FREQUENCY_SET", new[] { "COM3", "Stored Frequency (BCD16) (COM only)" })]
    [TouchPortalActionMapping("COM3_VOLUME_SET", new[] { "COM3", "Volume (0.0-1.0)" })]
    [TouchPortalActionMapping("NAV1_RADIO_SET_HZ", new[] { "NAV1", "Frequency (Hz)" })]
    [TouchPortalActionMapping("NAV1_RADIO_SET", new[] { "NAV1", "Frequency (BCD16)" })]
    [TouchPortalActionMapping("NAV1_STBY_SET_HZ", new[] { "NAV1", "Standby Frequency (Hz)" })]
    [TouchPortalActionMapping("NAV1_STBY_SET", new[] { "NAV1", "Standby Frequency (BCD16)" })]
    [TouchPortalActionMapping("NAV1_VOLUME_SET", new[] { "NAV1", "Volume (0.0-1.0)" })]
    [TouchPortalActionMapping("NAV2_RADIO_SET_HZ", new[] { "NAV2", "Frequency (Hz)" })]
    [TouchPortalActionMapping("NAV2_RADIO_SET", new[] { "NAV2", "Frequency (BCD16)" })]
    [TouchPortalActionMapping("NAV2_STBY_SET_HZ", new[] { "NAV2", "Standby Frequency (Hz)" })]
    [TouchPortalActionMapping("NAV2_STBY_SET", new[] { "NAV2", "Standby Frequency (BCD16)" })]
    [TouchPortalActionMapping("NAV2_VOLUME_SET", new[] { "NAV2", "Volume (0.0-1.0)" })]
    [TouchPortalActionMapping("NAV3_RADIO_SET_HZ", new[] { "NAV3", "Frequency (Hz)" })]
    [TouchPortalActionMapping("NAV3_RADIO_SET", new[] { "NAV3", "Frequency (BCD16)" })]
    [TouchPortalActionMapping("NAV3_STBY_SET_HZ", new[] { "NAV3", "Standby Frequency (Hz)" })]
    [TouchPortalActionMapping("NAV3_STBY_SET", new[] { "NAV3", "Standby Frequency (BCD16)" })]
    [TouchPortalActionMapping("NAV3_VOLUME_SET", new[] { "NAV3", "Volume (0.0-1.0)" })]
    [TouchPortalActionMapping("NAV4_RADIO_SET_HZ", new[] { "NAV4", "Frequency (Hz)" })]
    [TouchPortalActionMapping("NAV4_RADIO_SET", new[] { "NAV4", "Frequency (BCD16)" })]
    [TouchPortalActionMapping("NAV4_STBY_SET_HZ", new[] { "NAV4", "Standby Frequency (Hz)" })]
    [TouchPortalActionMapping("NAV4_STBY_SET", new[] { "NAV4", "Standby Frequency (BCD16)" })]
    [TouchPortalActionMapping("NAV4_VOLUME_SET", new[] { "NAV4", "Volume (0.0-1.0)" })]
    [TouchPortalActionText("0", float.MinValue, float.MaxValue, AllowDecimals = true)]
    public static readonly object RadiosSet;

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
