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

namespace MSFSTouchPortalPlugin.Objects.InstrumentsSystems
{
  [TouchPortalCategory(Groups.Communication)]
  internal static class CommunicationMapping {

    [TouchPortalAction("Radios", "Radio Interaction", "Radio {0} - {1}", true)]
    [TouchPortalActionChoice()]
    [TouchPortalActionChoice()]
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
