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
using Microsoft.Extensions.Logging;

namespace MSFSTouchPortalPlugin.Services
{
  /// <summary>
  /// Delivers tersely formatted log messages to any handler attached to the OnMessageReady event.
  /// This is used to pass through events and messages to the PluginService for centralized handling of events.
  /// </summary>
  class PluginLogger : ILogger
  {
    public static event MessageReadyHandler OnMessageReady;
    public delegate void MessageReadyHandler(string message, LogLevel logLevel, EventId eventId);

    public static string LogFormat { get; set; } = "{0:mm:ss} [{1}] {2}";

    public PluginLogger(string categoryName)
    {
      _ = categoryName;
      //_categoryName = categoryName.Split('.')[^1];
    }
    //readonly string _categoryName;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
      if (OnMessageReady == null || formatter == null)
        return;

      string message;
      try {
        message = string.Format(LogFormat, DateTime.Now, GetLogLevelStr(logLevel), formatter(state, exception));
      }
      catch (Exception e) {
        message = $"<formatting error: {e.Message}>";
      }

      OnMessageReady.Invoke(message, logLevel, eventId);
    }

    static readonly string[] _logLevelStrings = new [] { "TRC", "DBG", "INF", "WRN", "ERR", "CRT", "UNK" };
    static string GetLogLevelStr(LogLevel logLevel)
    {
      try { return _logLevelStrings[(int)logLevel]; }
      catch { return "???"; }
    }

    public bool IsEnabled(LogLevel logLevel) => logLevel > LogLevel.Debug && logLevel < LogLevel.None;
    public IDisposable BeginScope<TState>(TState state) => NullDisposable.Instance;
  }

  [ProviderAlias("PluginLogger")]
  internal class PluginLoggerProvider : ILoggerProvider
  {
    PluginLoggerProvider() { }

    public static ILoggerProvider Instance { get; } = new PluginLoggerProvider();
    public ILogger CreateLogger(string categoryName) => new PluginLogger(categoryName);

    protected virtual void Dispose(bool d) { /* nothing to dispose */ }
    public void Dispose() {
      Dispose(true);
      GC.SuppressFinalize(this);
    }
  }

  internal class NullDisposable : IDisposable
  {
    public static IDisposable Instance { get; } = new NullDisposable();
    protected virtual void Dispose(bool d) { /* nothing to dispose */ }
    public void Dispose() {
      Dispose(true);
      GC.SuppressFinalize(this);
    }
  }
}
