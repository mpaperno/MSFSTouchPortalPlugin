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
using Stopwatch = System.Diagnostics.Stopwatch;

namespace MSFSTouchPortalPlugin.Helpers
{
  /// <summary>
  /// Internal timer type for interaction with SimConnect.
  /// This is to ensure that async calls always run on the same thread,
  /// because SimConnect often crashes due to threading issues with regular timers.
  /// Note that this timer type needs an event loop to call the Tick() method periodically.
  /// </summary>
  public class UnthreadedTimer : IDisposable
  {
    public event EventHandler Elapsed = null;

    public bool AutoReset { get; set; } = true;
    public int Interval
    {
      get { return interval; }
      set {
        interval = Math.Max(value, 1);
        tickInterval = interval * (int)(Stopwatch.Frequency / 1000L);
        UpdateNextTick();
      }
    }
    public bool Enabled
    {
      get { return enabled; }
      set {
        enabled = value;
        if (enabled)
          UpdateNextTick();
      }
    }

    private bool enabled = false;
    private int interval = 1000;
    private long tickInterval;
    private long nextTick = 0;

    public UnthreadedTimer() { }
    public UnthreadedTimer(int interval) {
      Interval = interval;
    }

    public void Tick() {
      if (enabled && Stopwatch.GetTimestamp() >= nextTick) {
        enabled = false;
        if (Elapsed != null)
          Elapsed.Invoke(this, EventArgs.Empty);
        if (AutoReset)
          Start();
      }
    }

    public void Start() {
      Enabled = true;
    }

    public void Stop() {
      Enabled = false;
    }

    private void UpdateNextTick() {
      nextTick = Stopwatch.GetTimestamp() + tickInterval;
    }

    public void Dispose() {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
      if (disposing) {
        Stop();
        Elapsed = null;
      }
    }

  }
}
