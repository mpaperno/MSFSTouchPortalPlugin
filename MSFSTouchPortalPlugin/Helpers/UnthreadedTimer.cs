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
    /// <summary>
    /// The Elapsed event is triggered when the timer expires.
    /// </summary>
    public event EventHandler Elapsed = null;

    /// <summary>
    /// Determines if the timer will re-start itself after each timeout.
    /// </summary>
    public bool AutoReset { get; set; } = true;

    /// <summary>
    /// How often the timer will invoke the callback, in milliseconds.
    /// Setting this value on an active (`Enabled == true`) timer will restart the counter from that moment,
    /// discarding any time that may have accumulated so far before the interval value was changed.
    /// </summary>
    public int Interval
    {
      get { return interval; }
      set {
        if (value == interval)
          return;
        interval = Math.Max(value, 1);
        tickInterval = interval * (int)(Stopwatch.Frequency / 1000L);
        UpdateNextTick();
      }
    }

    /// <summary>
    /// The initial delay before the first time the timer is fired. If `Delay < 0` (default) then the `Interval` is used.
    /// If set, this becomes the first interval at which the `Elapsed` handler is invoked after `Timer.Start()` is called.
    /// This is only useful for timers that automatically reset so that the first invocation may have a different interval
    /// than the following ones (for example like a keyboard repeat).
    /// Setting or changing this value after the timer has started, but before the first elapsed time, will reset the delay time from that moment
    /// discarding any time that may have accumulated so far before the delay value was set/changed.
    /// </summary>
    public int Delay
    {
      get { return delay; }
      set {
        if (value == delay)
          return;
        delay = value;
        if (delay > -1)
          delayInterval = delay * (int)(Stopwatch.Frequency / 1000L);
        UpdateNextTick();
      }
    }

    /// <summary>
    /// Indicates if the timer is currently enabled.
    /// Setting this value to `true` or `false` is equivalent to calling `Start()` and `Stop()` methods respectively.
    /// </summary>
    public bool Enabled
    {
      get { return enabled; }
      set {
        if (value == enabled)
          return;
        enabled = value;
        if (enabled) {
          hasFired = false;
          UpdateNextTick();
        }
      }
    }

    public UnthreadedTimer() { }
    public UnthreadedTimer(int interval, int delay = -1) {
      Interval = interval;
      Delay = delay;
    }

    /// <summary>
    /// This method must be called periodically to keep the timer "running."
    /// </summary>
    public void Tick() {
      if (enabled && Stopwatch.GetTimestamp() >= nextTick) {
        enabled = false;
        if (Elapsed != null)
          Elapsed.Invoke(this, EventArgs.Empty);
        hasFired = true;
        if (AutoReset) {
          enabled = true;
          UpdateNextTick();
        }
      }
    }

    /// <summary> Starts the time counter. Equivalent to setting `Enabled = true`. </summary>
    public void Start() {
      Enabled = true;
    }

    /// <summary> Stops the time counter. Equivalent to setting `Enabled = false`. </summary>
    public void Stop() {
      Enabled = false;
    }

    /// <summary> Same as using `Stop(); Start();`. </summary>
    public void Restart() {
      Stop();
      Start();
    }

    // private:

    private bool enabled = false;
    private bool hasFired = false;
    private int interval = 1000;
    private int delay = -1;
    private long tickInterval;
    private long delayInterval;
    private long nextTick = 0;

    private void UpdateNextTick() {
      nextTick = Stopwatch.GetTimestamp() + (hasFired || delay < 0 ? tickInterval : delayInterval);
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
