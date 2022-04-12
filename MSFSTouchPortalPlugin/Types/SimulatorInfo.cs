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
    }

    public override string ToString() {
      return "Simulator " +
        $"\"{ApplicationName}\" v{AppVersionMaj}.{AppVersionMin}.{AppBuildMaj}.{AppBuildMin}" +
        ", with SimConnect v" + $"{SimConnectVersionMaj}.{SimConnectVersionMin}.{SimConnectBuildMaj}.{SimConnectBuildMin}" +
        $" (Server v{SimConnectServerVersion})";
    }
  }
}
