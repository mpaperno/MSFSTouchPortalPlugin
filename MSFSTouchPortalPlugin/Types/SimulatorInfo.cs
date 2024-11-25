/*
This file is part of the MSFS Touch Portal Plugin project.
https://github.com/mpaperno/MSFSTouchPortalPlugin

COPYRIGHT:
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSFSTouchPortalPlugin.Types
{
  struct SimulatorInfo
  {
    public string ApplicationName;
    public uint AppVersionMaj;
    public uint AppVersionMin;
    public uint AppBuildMaj;
    public uint AppBuildMin;
    public uint SimConnectVersionMaj;
    public uint SimConnectVersionMin;
    public uint SimConnectBuildMaj;
    public uint SimConnectBuildMin;
    public uint SimConnectServerVersion;
    public string AppVersionString;
    public Version AppVersion;
    public string SimConnectVersionString;

    public SimulatorInfo(Microsoft.FlightSimulator.SimConnect.SIMCONNECT_RECV_OPEN data) {
      ApplicationName = data.szApplicationName;
      AppVersionMaj = data.dwApplicationVersionMajor;
      AppVersionMin = data.dwApplicationVersionMinor;
      AppBuildMaj = data.dwApplicationBuildMajor;
      AppBuildMin = data.dwApplicationBuildMinor;
      SimConnectVersionMaj = data.dwSimConnectVersionMajor;
      SimConnectVersionMin = data.dwSimConnectVersionMinor;
      SimConnectBuildMaj = data.dwSimConnectBuildMajor;
      SimConnectBuildMin = data.dwSimConnectBuildMinor;
      SimConnectServerVersion = data.dwVersion;
      AppVersionString = $"{AppVersionMaj}.{AppVersionMin}.{AppBuildMaj}.{AppBuildMin}";
      SimConnectVersionString = $"{SimConnectVersionMaj}.{SimConnectVersionMin}.{SimConnectBuildMaj}.{SimConnectBuildMin}";
      try {
        AppVersion = new Version((int)AppVersionMaj, (int)AppVersionMin, (int)AppBuildMaj, (int)AppBuildMin);
      }
      catch (Exception) {
        AppVersion = null;
      }
    }

    public override string ToString() {
      return
        $"Simulator \"{ApplicationName}\" v{AppVersionString}, with SimConnect v{SimConnectVersionString} (Server v{SimConnectServerVersion})";
    }
  }
}
