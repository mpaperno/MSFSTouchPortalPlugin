/*
This file is part of the MSFS Touch Portal Plugin project.
https://github.com/mpaperno/MSFSTouchPortalPlugin

COPYRIGHT: (c) Maxim Paperno; All Rights Reserved.

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

namespace MSFSTouchPortalPlugin.Enums
{
  // SIMCONNECT_RECV_EVENT.dwData values for Pause_EX1 event, from SimConnect_SubscribeToSystemEvent SDK docs
  [System.Flags]
  public enum SimPauseStates : byte
  {
    OFF             = 0,     // No Pause
    FULL            = 0x01,  // Full Pause with time (sim + traffic + etc...)  (SET_PAUSE 1 / Dev -> Options -> Pause)
    FULL_WITH_SOUND = 0x02,  // FSX Legacy Pause (not used anymore)
    ACTIVE          = 0x04,  // Pause was activated using the "Active Pause" Button (position/attitude freeze)
    SIM             = 0x08,  // Pause the player sim but traffic, multi, etc... will still run (SET_PAUSE_ON / ESC menu pause)
  }
}
