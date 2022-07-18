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
using System.Linq;
using Stopwatch = System.Diagnostics.Stopwatch;
using ConnectorEvent = TouchPortalSDK.Messages.Events.ConnectorChangeEvent;
using ShortIdEvent = TouchPortalSDK.Messages.Events.ShortConnectorIdNotificationEvent;
using ActionData = TouchPortalSDK.Messages.Models.ActionData;

namespace MSFSTouchPortalPlugin.Helpers
{

  internal class ConnectorTrackingData
  {
    public static int CONNECTOR_DOWN_TIMEOUT_SEC = 5;

    public string connectorId = default;
    public string shortId = default;
    public int lastValue = -1;
    public bool isDown = false;
    public long nextTimeout = 0;
    public float fbRangeMin = 0;
    public float fbRangeMax = 0;
    //public string fbVariable;
    //public string mappingId;

    public bool IsStillDown {
      get {
        if (isDown && Stopwatch.GetTimestamp() >= nextTimeout)
          isDown = false;
        return isDown;
      }
    }

    public void UpdateTimeout()
    {
      if (isDown)
        nextTimeout = Stopwatch.GetTimestamp() + (Stopwatch.Frequency * CONNECTOR_DOWN_TIMEOUT_SEC);
    }

    public ConnectorTrackingData(string id = default, int value = -1, bool isdown = false)
    {
      connectorId = id;
      lastValue = value;
      isDown = isdown;
      UpdateTimeout();
    }
  }

  internal class ConnectorTracker
  {
    private readonly ConcurrentDictionary<string, ConnectorTrackingData> _connectors = new();
    private readonly ConcurrentDictionary<string, Dictionary<string, ConnectorTrackingData>> _stateIdIndex = new();

    static string MappingId(string connectorId, ActionData data)
    {
      return connectorId.Split('.').Last() + "|" + string.Join("|", data.Select(d => d.Key + "=" + d.Value));
    }

    ConnectorTrackingData GetOrCreateTrackingData(string connectorId, ActionData actionData, int value = -1)
    {
      string mappingId = MappingId(connectorId, actionData);
      if (!_connectors.TryGetValue(mappingId, out ConnectorTrackingData data)) {
        data = new ConnectorTrackingData(connectorId, value);
        //data.mappingId = mappingId;
        bool ok;
        try { ok = _connectors.TryAdd(mappingId, data); }
        catch { ok = false; }
        if (!ok)
          return null;
      }
      return data;
    }

    public ConnectorTrackingData GetDataForEvent(ConnectorEvent ev)
    {
      return _connectors.GetValueOrDefault(MappingId(ev.ConnectorId, ev.Data));
    }

    public ConnectorTrackingData GetDataForEvent(ShortIdEvent ev)
    {
      return _connectors.GetValueOrDefault(MappingId(ev.ActualConnectorId, ev.Data));
    }

    /// <summary> Creates or updates a connector record and returns true if the passed value doesn't equal the last value or if this is a new connector. </summary>
    public bool UpdateConnectorValue(ConnectorEvent ev)
    {
      if (GetOrCreateTrackingData(ev.ConnectorId, ev.Data, ev.Value) is var data && data != null) {
        data.isDown = data.lastValue != ev.Value;
        data.lastValue = ev.Value;
        data.UpdateTimeout();
        return data.isDown;
      }
      return false;
    }

    /// <summary> Creates or updates a connector record from a ShortConnectorIdNotification event and pre-parsed category ID, state ID, and feedback range values. </summary>
    public void SaveConnectorInstance(ShortIdEvent ev, Enums.Groups catId, string varId, float rangeMin, float rangeMax)
    {
      if (GetOrCreateTrackingData(ev.ActualConnectorId, ev.Data) is var data && data != null) {
        data.shortId = ev.ShortId;
        data.fbRangeMin = rangeMin;
        data.fbRangeMax = rangeMax;
        SaveConnectorInstance(catId.ToString() + '.' + varId, data);
      }
    }

    void SaveConnectorInstance(string stateId, ConnectorTrackingData trackingData)
    {
      var list = _stateIdIndex.GetOrAdd(stateId, new Dictionary<string, ConnectorTrackingData>());
      list[trackingData.shortId] = trackingData;
    }

#nullable enable
    public IReadOnlyCollection<ConnectorTrackingData>? GetInstancesForStateId(Enums.Groups catId, string varId)
    {
      return GetInstancesForStateId(catId.ToString() + '.' + varId);
    }

    public IReadOnlyCollection<ConnectorTrackingData>? GetInstancesForStateId(string stateId)
    {
      if (_stateIdIndex.TryGetValue(stateId, out var ret))
        return ret.Values;
      return null;
    }
#nullable restore

  }
}
