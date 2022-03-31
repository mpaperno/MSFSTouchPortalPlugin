using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MSFSTouchPortalPlugin.Configuration;
using MSFSTouchPortalPlugin.Interfaces;
using MSFSTouchPortalPlugin.Objects.Plugin;
using MSFSTouchPortalPlugin.Types;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TouchPortalExtension.Enums;
using TouchPortalSDK;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Events;
using Timer = MSFSTouchPortalPlugin.Helpers.UnthreadedTimer;
using MSFSTouchPortalPlugin.Helpers;

namespace MSFSTouchPortalPlugin.Services
{
  /// <inheritdoc cref="IPluginService" />
  internal class PluginService : IPluginService, IDisposable, ITouchPortalEventHandler {
    private CancellationToken _cancellationToken;
    private CancellationTokenSource _simConnectCancellationTokenSource;

    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly ILogger<PluginService> _logger;
    private readonly ITouchPortalClient _client;
    private readonly ISimConnectService _simConnectService;
    private readonly IReflectionService _reflectionService;
    private static readonly System.Data.DataTable _expressionEvaluator = new();  // used to evaluate basic math in action data
    private bool autoReconnectSimConnect = false;
    private bool _quitting;

    public string PluginId => "MSFSTouchPortalPlugin";

    private Dictionary<string, ActionEventType> actionsDictionary = new();
    private Dictionary<Definition, SimVarItem> statesDictionary = new();
    private Dictionary<string, PluginSetting> pluginSettingsDictionary = new();

    private readonly ConcurrentDictionary<string, Timer> repeatingActionTimers = new();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="messageProcessor">Message Processor Object</param>
    public PluginService(IHostApplicationLifetime hostApplicationLifetime, ILogger<PluginService> logger,
      ITouchPortalClientFactory clientFactory, ISimConnectService simConnectService, IReflectionService reflectionService) {
      _hostApplicationLifetime = hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _simConnectService = simConnectService ?? throw new ArgumentNullException(nameof(simConnectService));
      _reflectionService = reflectionService ?? throw new ArgumentNullException(nameof(reflectionService));

      _client = clientFactory?.Create(this) ?? throw new ArgumentNullException(nameof(clientFactory));
    }

    /// <summary>
    /// Runs the plugin services
    /// </summary>
    /// <param name="simConnectCancelToken">The Cancellation Token</param>
    public async Task RunPluginServices(CancellationToken simConnectCancelToken) {
      // Run Data Polling and repeating actions timers
      var runTimersTask = RunTimedEventsTask(simConnectCancelToken);

      await Task.WhenAll(new Task[] {
        runTimersTask,
        // Run Listen and pairing
        _simConnectService.WaitForMessage(simConnectCancelToken)
      }).ConfigureAwait(false);
    }

    /// <summary>
    /// Async task for running various timed SimConnect events on one thread.
    /// This is primarily used for data polling and repeating (held) actions.
    /// </summary>
    /// <param name="cancellationToken">The Cancellation Token</param>
    private async Task RunTimedEventsTask(CancellationToken cancellationToken) {
      // Run Data Polling
      var dataPollTimer = new Timer(250);
      dataPollTimer.Elapsed += delegate { CheckPendingRequests(); };
      dataPollTimer.Start();

      while (_simConnectService.IsConnected() && !cancellationToken.IsCancellationRequested) {
        dataPollTimer.Tick();
        foreach (Timer tim in repeatingActionTimers.Values)
          tim.Tick();
        await Task.Delay(25, cancellationToken);
      }

      dataPollTimer.Dispose();
    }

    /// <summary>
    /// Starts the plugin service
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    public Task StartAsync(CancellationToken cancellationToken) {
      _cancellationToken = cancellationToken;

      _hostApplicationLifetime.ApplicationStarted.Register(() => {
        if (!Initialize()) {
          _hostApplicationLifetime.StopApplication();
          return;
        }

        Task.WhenAll(TryConnect());
      });

      _hostApplicationLifetime.ApplicationStopping.Register(() => {
        // Disconnect from SimConnect
        _quitting = true;
        autoReconnectSimConnect = false;
        _simConnectService.Disconnect();
        if (_client?.IsConnected ?? false) {
          try { _client.Close(); }  // exits the event loop keeping us alive
          catch (Exception) { /* ignore */ }
        }
      });

      // register ctrl-c exit handler
      Console.CancelKeyPress += (_, _) => {
        _logger.LogInformation("Quitting due to keyboard interrupt.");
        _hostApplicationLifetime.StopApplication();
        //Environment.Exit(0);
      };

      return Task.CompletedTask;
    }

    /// <summary>
    /// Stops the plugin service
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    public Task StopAsync(CancellationToken cancellationToken) {
      return Task.CompletedTask;
    }

    #region IDisposable Support
    private bool disposedValue; // To detect redundant calls

    protected virtual void Dispose(bool disposing) {
      if (!disposedValue) {
        if (disposing) {
          // Dispose managed state (managed objects).
          _simConnectCancellationTokenSource?.Dispose();
        }

        disposedValue = true;
      }
    }

    // This code added to correctly implement the disposable pattern.
    public void Dispose() {
      // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
      Dispose(true);
      GC.SuppressFinalize(this);
    }
    #endregion

    /// <summary>
    /// Initialized the Touch Portal Message Processor
    /// </summary>
    private bool Initialize() {
      SetupEventLists();  // set these up first to have access to defined plugin settings

      if (!_client.Connect()) {
        return false;
      }

      // Setup SimConnect Events
      _simConnectService.OnDataUpdateEvent += SimConnectEvent_OnDataUpdateEvent;
      _simConnectService.OnConnect += SimConnectEvent_OnConnect;
      _simConnectService.OnDisconnect += SimConnectEvent_OnDisconnect;

      return true;
    }

    #region SimConnect Events

    private void SimConnectEvent_OnConnect() {
      _simConnectCancellationTokenSource = new CancellationTokenSource();

      UpdateSimConnectState();

      // Register Actions
      var eventMap = _reflectionService.GetClientEventIdToNameMap();
      foreach (var m in eventMap) {
        _simConnectService.MapClientEventToSimEvent(m.Key, m.Value.EventName);
        _simConnectService.AddNotification(m.Value.GroupId, m.Key);
      }
      // must be called after adding notifications
      _simConnectService.SetNotificationGroupPriorities();

      SetupSimVars();

      Task.WhenAll(RunPluginServices(_simConnectCancellationTokenSource.Token));
    }

    private void SimConnectEvent_OnDataUpdateEvent(Definition def, Definition req, object data) {
      // Lookup State Mapping.
      if (!statesDictionary.TryGetValue(def, out SimVarItem simVar))
        return;

      // Update SimVarItem value and TP state on changes.
      // TODO: sim vars on a regular request interval will only be sent when changed, so skip the equality check.
      // SimVarItem.SetValue() takes care of setting the correct value type and also any unit conversions as needed. Returns false if conversion failed.
      if (!simVar.ValueEquals(data) && simVar.SetValue(data))
        _client.StateUpdate(simVar.TouchPortalStateId, simVar.FormattedValue);
      // clear pending flag last
      simVar.SetPending(false);
    }

    private void SimConnectEvent_OnDisconnect() {
      _simConnectCancellationTokenSource?.Cancel();
      ClearRepeatingActions();
      UpdateSimConnectState();
    }

    #endregion

    private void SetupEventLists() {
      actionsDictionary = _reflectionService.GetActionEvents();
      pluginSettingsDictionary = _reflectionService.GetSettings();
    }

    private void SetupSimVars() {
      var pc = PluginConfig.Instance;
      var configStates = pc.LoadSimVarItems(false);
      if (pc.HaveErrors) {
        foreach (var e in pc.ErrorsList)
          _logger.LogWarning(e, "Configuration reader error:");
      }
      statesDictionary = configStates.ToDictionary(s => s.Def, s => s);
      // Register SimVars
      _simConnectService.ClearAllDataDefinitions();
      foreach (var s in statesDictionary)
        _simConnectService.RegisterToSimConnect(s.Value);
    }

    private Task TryConnect() {
      short i = 0;
      while (!_cancellationToken.IsCancellationRequested) {
        if (autoReconnectSimConnect && !_simConnectService.IsConnected()) {
          if (i == 0) {
            if (!_simConnectService.Connect())
              i = 10;  // delay reconnect attempt on error
          }
          else {
            --i;
          }
        }

        // SimConnect is typically available even before loading into a flight. This should connect and be ready by the time a flight is started.
        Thread.Sleep(1000);
      }

      return Task.CompletedTask;
    }

    private void ProcessEvent(ActionEvent actionEvent) {
      if (!actionsDictionary.TryGetValue(actionEvent.ActionId, out ActionEventType action))
        return;

      var dataArry = actionEvent.Data.Values.ToArray();
      if (!action.TryGetEventMapping(in dataArry, out Enum eventId))
        return;

      if (action.InternalEvent)
        ProcessInternalEvent(action, eventId, in dataArry);
      else
        ProcessSimEvent(action, eventId, in dataArry);
    }

    private void ProcessInternalEvent(ActionEventType action, Enum eventId, in string[] dataArry) {
      Plugin pluginEventId = (Plugin)eventId;
      _logger.LogInformation($"Firing Internal Event - action: {action.ActionId}; enum: {pluginEventId}; data: {string.Join(", ", dataArry)}");

      switch (pluginEventId) {
        case Plugin.ToggleConnection:
          autoReconnectSimConnect = !autoReconnectSimConnect;
          if (_simConnectService.IsConnected())
            _simConnectService.Disconnect();
          else
            UpdateSimConnectState();
          break;
        case Plugin.Connect:
          autoReconnectSimConnect = true;
          UpdateSimConnectState();
          break;
        case Plugin.Disconnect:
          autoReconnectSimConnect = false;
          if (_simConnectService.IsConnected())
            _simConnectService.Disconnect();
          else
            UpdateSimConnectState();
          break;

        case Plugin.ActionRepeatIntervalInc:
        case Plugin.ActionRepeatIntervalDec:
        case Plugin.ActionRepeatIntervalSet:
          if (action.ValueIndex < dataArry.Length && double.TryParse(dataArry[action.ValueIndex], out var interval)) {
            if (pluginEventId == Plugin.ActionRepeatIntervalInc)
              interval = Settings.ActionRepeatInterval.ValueAsDbl() + interval;
            else if (pluginEventId == Plugin.ActionRepeatIntervalDec)
              interval = Settings.ActionRepeatInterval.ValueAsDbl() - interval;
            interval = Math.Clamp(interval, Settings.ActionRepeatInterval.MinValue, Settings.ActionRepeatInterval.MaxValue);
            if (interval != Settings.ActionRepeatInterval.ValueAsDbl())
              _client.SettingUpdate(Settings.ActionRepeatInterval.Name, $"{interval:F0}");  // this will trigger the actual value update
          }
          break;

        default:
          // No other types of events supported right now.
          break;
      }
    }

    private void ProcessSimEvent(ActionEventType action, Enum eventId, in string[] dataArry) {
      uint dataUint = 0;
      string eventName = _reflectionService.GetSimEventNameById(eventId);  // just for logging
      if (action.ValueIndex > -1 && action.ValueIndex < dataArry.Length) {
        double dataReal = double.NaN;
        var valStr = dataArry[action.ValueIndex];
        try {
          switch (action.ValueType) {
            case DataType.Number:
            case DataType.Text:
              dataReal = Convert.ToDouble(_expressionEvaluator.Compute(valStr, null));
              break;
            case DataType.Switch:
              dataReal = new BooleanString(valStr);
              break;
          }
          if (!double.IsNaN(dataReal)) {
            if (!double.IsNaN(action.MinValue))
              dataReal = Math.Max(dataReal, action.MinValue);
            if (!double.IsNaN(action.MaxValue))
              dataReal = Math.Min(dataReal, action.MaxValue);
            dataUint = (uint)Math.Round(dataReal);
          }
        }
        catch (Exception e) {
          _logger.LogWarning(e, $"Action {action.ActionId} for sim event {eventName} with data string '{valStr}' - Failed to convert data to numeric value.");
        }
      }
      _logger.LogInformation($"Firing Sim Event - action: {action.ActionId}; group: {action.SimConnectGroup}; name: {eventName}; data {dataUint}");
      _simConnectService.TransmitClientEvent(action.SimConnectGroup, eventId, dataUint);
    }

    private void CheckPendingRequests() {
      foreach (var s in statesDictionary.Values) {
        // Expire pending if more than 30 seconds
        if (s.PendingTimeout())
          _logger.LogDebug($"Request for SimVar '{s.SimVarName}' timed out!");

        // Check if Pending data request in play
        if (!s.PendingRequest) {
          s.SetPending(true);
          _simConnectService.RequestDataOnSimObjectType(s);
        }
      }
    }

    private void ClearRepeatingActions() {
      foreach (var act in repeatingActionTimers) {
        if (repeatingActionTimers.TryRemove(act.Key, out var tim)) {
          tim.Dispose();
        }
      }
    }

    /// <summary>
    /// Handles an array of `Setting` types sent from TP. This could come from either the
    /// initial `OnInfoEvent` message, or the dedicated `OnSettingsEvent` message.
    /// </summary>
    /// <param name="settings"></param>
    private void ProcessPluginSettings(IReadOnlyCollection<TouchPortalSDK.Messages.Models.Setting> settings) {
      if (settings == null)
        return;
      foreach (var s in settings) {
        if (pluginSettingsDictionary.TryGetValue(s.Name, out PluginSetting setting)) {
          setting.SetValueFromString(s.Value);
          if (!string.IsNullOrWhiteSpace(setting.TouchPortalStateId))
            _client.StateUpdate(setting.TouchPortalStateId, setting.ValueAsStr());
        }
      }
    }

    private void UpdateSimConnectState() {
      string stat = "true";
      if (!_simConnectService.IsConnected())
        stat = autoReconnectSimConnect ? "connecting" : "false";
      _client.StateUpdate("MSFSTouchPortalPlugin.Plugin.State.Connected", stat);
    }

    #region TouchPortalSDK Events

    public void OnInfoEvent(InfoEvent message) {
      _logger?.LogInformation(
        $"Touch Portal Connected with: TP v{message.TpVersionString}, SDK v{message.SdkVersion}, {PluginId} entry.tp v{message.PluginVersion}, " +
        $"{VersionInfo.AssemblyName} running v{VersionInfo.GetProductVersionString()} ({VersionInfo.GetProductVersionNumber():X})"
      );

      ProcessPluginSettings(message.Settings);
      autoReconnectSimConnect = (Settings.ConnectSimOnStartup.ValueAsInt() != 0);
    }

    public void OnSettingsEvent(SettingsEvent message) {
      ProcessPluginSettings(message.Values);
    }

    public void OnActionEvent(ActionEvent message) {
      // Actions used in TP "On Hold" events will send "down" and "up" events, in that order (usually).
      // "On Pressed" actions will have an "action" event. These are all abstracted into TouchPortalSDK enums.
      // Note that an "On Hold" action ("down" event) is _not_ triggered upon initial press. This allow different
      // actions per button, and button designer can still choose to duplicate the Hold action in the Pressed handler.
      switch (message.GetPressState()) {
        case TouchPortalSDK.Messages.Models.Enums.Press.Down:
          // "On Hold" activated ("down" event). Try to add this action to the repeating/scheduled actions queue, unless it already exists.
          var timer = new Timer(Settings.ActionRepeatInterval.ValueAsInt());
          timer.Elapsed += delegate { ProcessEvent(message); };
          if (repeatingActionTimers.TryAdd(message.ActionId, timer))
            timer.Start();
          else
            timer.Dispose();
          break;

        case TouchPortalSDK.Messages.Models.Enums.Press.Up:
          // "On Hold" released ("up" event). Mark action for removal from repeating queue.
          if (repeatingActionTimers.TryRemove(message.ActionId, out var tim))
            tim.Dispose();
          // No further processing for this action.
          break;

        case TouchPortalSDK.Messages.Models.Enums.Press.Tap:
          // Process an "On Pressed" ("action" event) action.
          ProcessEvent(message);
          break;
      }
    }

    public void OnClosedEvent(string message) {
      _logger?.LogInformation($"TouchPortal Disconnected with message: {message}");

      if (!_quitting) {
        _hostApplicationLifetime.StopApplication();
        //Environment.Exit(0);
      }
    }

    public void OnListChangedEvent(ListChangeEvent message) {
      // not implemented yet
    }

    public void OnConnecterChangeEvent(ConnectorChangeEvent message) {
      // not implemented yet
    }

    public void OnShortConnectorIdNotificationEvent(ShortConnectorIdNotificationEvent message) {
      // not implemented yet
    }

    public void OnNotificationOptionClickedEvent(NotificationOptionClickedEvent message) {
      // not implemented yet
    }

    public void OnBroadcastEvent(BroadcastEvent message) {
      // not implemented yet
    }

    public void OnUnhandledEvent(string jsonMessage) {
      _logger?.LogDebug($"Unhandled message: {jsonMessage}");
    }

    #endregion
  }
}
