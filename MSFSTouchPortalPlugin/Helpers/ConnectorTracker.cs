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

using System.Collections.Generic;
using System.Collections.Concurrent;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace MSFSTouchPortalPlugin.Helpers
{

  internal class ConnectorTrackingData
  {
    public string connectorId = default;
    public int lastValue = -1;
    public bool isDown = false;
    public long lastUpdate = 0;

    public ConnectorTrackingData(string id, int value, bool isdown = false)
    {
      connectorId = id;
      lastValue = value;
      isDown = isdown;
      lastUpdate = Stopwatch.GetTimestamp();
    }
  }

  internal class ConnectorInstanceTrackingData
  {
    public string shortId = default;
    public int fbRangeMin = 0;
    public int fbRangeMax = 0;

    public ConnectorInstanceTrackingData(string shortId, int rangeMin, int rangeMax)
    {
      this.shortId = shortId;
      fbRangeMin = rangeMin;
      fbRangeMax = rangeMax;
    }
  }

  internal class ConnectorTracker
  {
    private readonly ConcurrentDictionary<string, ConnectorTrackingData> _connectors = new();
    private readonly ConcurrentDictionary<string, Dictionary<string, ConnectorInstanceTrackingData>> _instances = new();
    //private readonly ConcurrentDictionary<string, string> _connectorsLongToShortMap = new();

    /// <summary> Creates or updates a connector record and returns true if the passed value doesn't equal the last value or if this is a new connector. </summary>
    public bool UpdateConnectorValue(string connectorId, int value)
    {
      if (!_connectors.TryGetValue(connectorId, out ConnectorTrackingData data)) {
        data = new ConnectorTrackingData(connectorId, value, true);
        _connectors.TryAdd(connectorId, data);
        return true;
      }
      data.isDown = data.lastValue != value;
      data.lastValue = value;
      data.lastUpdate = Stopwatch.GetTimestamp();
      return data.isDown;
    }

    public ConnectorInstanceTrackingData SaveConnectorInstance(Enums.Groups catId, string varId, string shortId, int rangeMin, int rangeMax)
    {
      return SaveConnectorInstance(catId.ToString() + '.' + varId, shortId, rangeMin, rangeMax);
    }

    public ConnectorInstanceTrackingData SaveConnectorInstance(string stateId, string shortId, int rangeMin, int rangeMax)
    {
      var list = _instances.GetOrAdd(stateId, new Dictionary<string, ConnectorInstanceTrackingData>());
      if (!list.TryGetValue(shortId, out ConnectorInstanceTrackingData data)) {
        data = new ConnectorInstanceTrackingData(shortId, rangeMin, rangeMax);
        list.Add(shortId, data);
      }
      return data;
    }

#nullable enable
    public IReadOnlyCollection<ConnectorInstanceTrackingData>? GetInstancesForStateId(Enums.Groups catId, string varId)
    {
      return GetInstancesForStateId(catId.ToString() + '.' + varId);
    }

    public IReadOnlyCollection<ConnectorInstanceTrackingData>? GetInstancesForStateId(string stateId)
    {
      if (_instances.TryGetValue(stateId, out var ret))
        return ret.Values;
      return null;
    }
#nullable restore

  }
}
