using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MSFSTouchPortalPlugin.Configuration;
using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Enums;
using MSFSTouchPortalPlugin.Helpers;
using MSFSTouchPortalPlugin.Interfaces;
using MSFSTouchPortalPlugin.Objects.Plugin;
using MSFSTouchPortalPlugin.Types;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TouchPortalSDK;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Events;
using Timer = MSFSTouchPortalPlugin.Helpers.UnthreadedTimer;

namespace MSFSTouchPortalPlugin.Services
{
  /// <inheritdoc cref="IPluginService" />
  internal class PluginService : IPluginService, ITouchPortalEventHandler
  {
    public string PluginId => "MSFSTouchPortalPlugin";  // for ITouchPortalEventHandler

    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly ILogger<PluginService> _logger;
    private readonly ITouchPortalClient _client;
    private readonly ISimConnectService _simConnectService;
    private readonly IReflectionService _reflectionService;
    private readonly PluginConfig _pluginConfig;

    private CancellationToken _cancellationToken;
    private CancellationTokenSource _simTasksCTS;
    private CancellationToken _simTasksCancelToken;
    private Task _timerEventsTask;
    private readonly ManualResetEventSlim _simConnectionRequest = new(false);
    private bool autoReconnectSimConnect = false;
    private EntryFileType _entryFileType = EntryFileType.Default;
    private bool _quitting;

    private Dictionary<string, ActionEventType> actionsDictionary = new();
    private Dictionary<Definition, SimVarItem> statesDictionary = new();
    private Dictionary<string, PluginSetting> pluginSettingsDictionary = new();
    private readonly ConcurrentBag<Definition> _customIntervalStates = new();  // IDs of SimVars which need periodic value polling; concurrent because the polling happens in separate task from setter.
    private readonly Dictionary<string, Definition> _settableSimVarIds = new();   // mapping of settable SimVar IDs for lookup in statesDictionary
    private readonly List<string> _dynamicStateIds = new();  // keep track of dynamically created states for clearing them if/when reloading sim var state files
    private readonly ConcurrentDictionary<string, Timer> _repeatingActionTimers = new();

    private static readonly System.Data.DataTable _expressionEvaluator = new();  // used to evaluate basic math in action data

    private enum EntryFileType { Default, NoStates, Custom };

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="messageProcessor">Message Processor Object</param>
    public PluginService(IHostApplicationLifetime hostApplicationLifetime, ILogger<PluginService> logger,
      ITouchPortalClientFactory clientFactory, ISimConnectService simConnectService, IReflectionService reflectionService,
      PluginConfig pluginConfig)
    {
      _hostApplicationLifetime = hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _simConnectService = simConnectService ?? throw new ArgumentNullException(nameof(simConnectService));
      _reflectionService = reflectionService ?? throw new ArgumentNullException(nameof(reflectionService));

      _client = clientFactory?.Create(this) ?? throw new ArgumentNullException(nameof(clientFactory));
      _pluginConfig = pluginConfig ?? throw new ArgumentNullException(nameof(pluginConfig));
    }

    #region Startup, Shutdown and Processing Tasks      //////////////////

    /// <summary>
    /// Starts the plugin service
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    public Task StartAsync(CancellationToken cancellationToken) {
      _logger.LogDebug("Starting up...");

      _cancellationToken = cancellationToken;

      if (!Initialize()) {
        _hostApplicationLifetime.StopApplication();
        return Task.CompletedTask;
      }

      // register ctrl-c exit handler
      Console.CancelKeyPress += (_, _) => {
        _logger.LogInformation("Quitting due to keyboard interrupt.");
        _hostApplicationLifetime.StopApplication();
        //Environment.Exit(0);
      };

      if (autoReconnectSimConnect)
        _simConnectionRequest.Set();  // enable connection attempts
      return Task.WhenAll(SimConnectionMonitor());
    }

    /// <summary>
    /// Stops the plugin service
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    public Task StopAsync(CancellationToken cancellationToken) {
      if (_quitting)
        return Task.CompletedTask;
      // Shut down
      _quitting = true;
      _logger.LogDebug("Shutting down...");
      _simConnectionRequest.Reset();  // just in case
      _simConnectService?.Disconnect();
      if (_client?.IsConnected ?? false) {
        try { _client.Close(); }
        catch (Exception) { /* ignore */ }
      }
      return Task.CompletedTask;
    }

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

      // Check for custom SimConnect.cfg and try copy it to application dir (may require elevated privileges)
      if (_pluginConfig.CopySimConnectConfig())
        _logger.LogInformation("Using custom SimConnect.cfg file from user's AppData folder.");

      return true;
    }

    private async Task SimConnectionMonitor() {
      _logger.LogDebug("SimConnectionMonitor task started.");
      try {
        while (!_cancellationToken.IsCancellationRequested) {
          _simConnectionRequest.Wait(_cancellationToken);
          if (!_simConnectService.IsConnected() && !_simConnectService.Connect(Settings.SimConnectConfigIndex.UIntValue))
            await Task.Delay(10000, _cancellationToken);  // delay 10s on connection error
        }
      }
      catch (OperationCanceledException) { /* ignore but exit */ }
      catch (ObjectDisposedException) { /* ignore but exit */ }
      catch (Exception e) {
        _logger.LogError("Exception in SimConnectionMonitor task, cannot continue.", e);
      }
      _logger.LogDebug("SimConnectionMonitor task stopped.");
    }

    /// <summary>
    /// Task for running various timed SimConnect events on one thread.
    /// This is primarily used for data polling and repeating (held) actions.
    /// </summary>
    private async Task PluginEventsTask() {
      _logger.LogDebug("PluginEventsTask task started.");
      try {
        while (_simConnectService.IsConnected() && !_simTasksCancelToken.IsCancellationRequested) {
          foreach (Timer tim in _repeatingActionTimers.Values)
            tim.Tick();
          CheckPendingRequests();
          await Task.Delay(25, _simTasksCancelToken);
        }
      }
      catch (OperationCanceledException) { /* ignore but exit */ }
      catch (ObjectDisposedException) { /* ignore but exit */ }
      catch (Exception e) {
        _logger.LogError("Exception in PluginEventsTask task, cannot continue.", e);
      }
      _logger.LogDebug("PluginEventsTask task stopped.");
    }

    // runs in PluginEventsTask
    private void CheckPendingRequests() {
      foreach (var def in _customIntervalStates) {
        // Check if a value update is required based on the SimVar's internal tracking mechanism.
        if (statesDictionary.TryGetValue(def, out var s) && s.UpdateRequired) {
          s.SetPending(true);
          _simConnectService.RequestDataOnSimObjectType(s);
        }
      }
    }

    #endregion Startup, Shutdown and Processing Tasks

    #region SimConnect Events   /////////////////////////////////////

    private void SimConnectEvent_OnConnect() {
      _simConnectionRequest.Reset();
      _simTasksCTS = new CancellationTokenSource();
      _simTasksCancelToken = _simTasksCTS.Token;

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

      // start checking timer events
      _timerEventsTask = Task.Run(PluginEventsTask);
      _timerEventsTask.ConfigureAwait(false);  // needed?
    }

    private void SimConnectEvent_OnDataUpdateEvent(Definition def, Definition req, object data) {
      // Lookup State Mapping.
      if (!statesDictionary.TryGetValue(def, out SimVarItem simVar))
        return;

      // Update SimVarItem value and TP state on changes.
      // Sim vars sent on a standard SimConnect request period will only be sent when changed by > simVar.DeltaEpsilon anyway, so skip the equality check.
      // SimVarItem.SetValue() takes care of setting the correct value type, any unit conversions needed, and resets any expiry timers. Returns false if value is of the wrong type.
      if ((!simVar.NeedsScheduledRequest || !simVar.ValueEquals(data)) && simVar.SetValue(data))
        _client.StateUpdate(simVar.TouchPortalStateId, simVar.FormattedValue);
    }

    private void SimConnectEvent_OnDisconnect() {
      _simTasksCTS?.Cancel();
      if (_timerEventsTask.Status == TaskStatus.Running && !_timerEventsTask.Wait(5000))
        _logger.LogWarning("Timed events task timed out while stopping.");
      try { _timerEventsTask.Dispose(); }
      catch { /* ignore in case it hung */ }

      ClearRepeatingActions();
      UpdateSimConnectState();

      _timerEventsTask = null;
      _simTasksCTS?.Dispose();
      _simTasksCTS = null;

      if (autoReconnectSimConnect && !_quitting)
        _simConnectionRequest.Set();  // re-enable connection attempts
    }

    #endregion SimConnect Events

    #region Plugin Events and Handlers   /////////////////////////////////////

    private void SetupEventLists() {
      actionsDictionary = _reflectionService.GetActionEvents();
      pluginSettingsDictionary = _reflectionService.GetSettings();
    }

    private void SetupSimVars() {
      // We may need to generate all, some, or no states dynamically.
      bool allStatesDynamic = (_entryFileType == EntryFileType.NoStates);
      bool someStateDynamic = false;
      IEnumerable<string> defaultStates = Array.Empty<string>();  // in case we need to create _some_ custom states dynamically

      // First we figure out which file(s) to load.
      bool useCustomCfg = !string.IsNullOrWhiteSpace(Settings.UserStateFiles.StringValue) && Settings.UserStateFiles.StringValue.ToLower() != "default";
      if (useCustomCfg) {
        // Create the file(s) list
        string[] cfgFiles = Settings.UserStateFiles.StringValue.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        _logger.LogInformation($"Loading custom state file(s) '{string.Join(", ", cfgFiles)}' from folder '{PluginConfig.UserConfigFolder}'.");
        // load the file(s); the special file name "default" (if present) will actually use the one from plugin's install folder.
        statesDictionary = _pluginConfig.LoadCustomSimVars(cfgFiles).ToDictionary(s => s.Def, s => s);

        // if the user is NOT using dynamic states for everything (entry_no-states.tp) nor a custom entry.tp file,
        // then lets figure out what the default states are so we can create any missing ones dynamically if needed.
        if (_entryFileType == EntryFileType.Default) {
          // get the defaults
          defaultStates = _pluginConfig.LoadSimVarItems(false).Select(s => s.Id);
          someStateDynamic = defaultStates.Any();
        }
      }
      else {  // Default config
        _logger.LogInformation($"Loading default SimVar state file '{PluginConfig.AppConfigFolder}/{PluginConfig.StatesConfigFile}'.");
        statesDictionary = _pluginConfig.LoadSimVarItems(false).ToDictionary(s => s.Def, s => s);
      }

      // clear out any old data
      _customIntervalStates.Clear();
      _settableSimVarIds.Clear();
      RemoveDynamicStates();  // if any
      _simConnectService.ClearAllDataDefinitions();  // if any (SimConnectService keeps track of this internally)

      // Now register SimVars and possibly create TP states
      foreach (var simVar in statesDictionary.Values) {
        // Does this var need value polling?
        if (simVar.NeedsScheduledRequest)
          _customIntervalStates.Add(simVar.Def);
        if (simVar.CanSet)
          _settableSimVarIds.Add(simVar.Id, simVar.Def);
        // Need a dynamic state?
        if (allStatesDynamic || (someStateDynamic && !defaultStates.Contains(simVar.Id))) {
          _dynamicStateIds.Add(simVar.TouchPortalStateId);  // keep track for removing them later if needed
          _client.CreateState(simVar.TouchPortalStateId, Categories.PrependFullCategoryName(simVar.CategoryId, simVar.Name), simVar.DefaultValue);
          _logger.LogDebug($"Created dynamic state {simVar.TouchPortalStateId}'.");
        }
        // Register it. If the SimVar gets regular updates (not custom polling) then this also starts the data requests for this value.
        _simConnectService.RegisterToSimConnect(simVar);
      }

      UpdateSettableStatesList();
    }

    private void RemoveDynamicStates() {
      foreach (var id in _dynamicStateIds)
        _client.RemoveState(id);
    }

    private void UpdateSettableStatesList() {
      var list = (from s in statesDictionary.Values where s.CanSet select Categories.PrependCategoryName(s.CategoryId, s.Name) + $"  [{s.Id}]").OrderBy(n => n).ToArray();
      _client.ChoiceUpdate(PluginId + ".Plugin.Action.SetSimVar.Data.0", list);
    }

    private void ClearRepeatingActions() {
      foreach (var act in _repeatingActionTimers) {
        if (_repeatingActionTimers.TryRemove(act.Key, out var tim)) {
          tim.Dispose();
        }
      }
    }

    private void ProcessEvent(ActionEvent actionEvent) {
      if (!actionsDictionary.TryGetValue(actionEvent.ActionId, out ActionEventType action))
        return;

      var dataArry = actionEvent.Data.Values.ToArray();
      if (!action.TryGetEventMapping(in dataArry, out Enum eventId))
        return;

      if (action.InternalEvent)
        ProcessInternalEvent(action, eventId, in dataArry);
      else if (_simConnectService.IsConnected())
        ProcessSimEvent(action, eventId, in dataArry);
    }

    private void ProcessInternalEvent(ActionEventType action, Enum eventId, in string[] dataArry) {
      Plugin pluginEventId = (Plugin)eventId;
      _logger.LogDebug($"Firing Internal Event - action: {action.ActionId}; enum: {pluginEventId}; data: {string.Join(", ", dataArry)}");

      switch (pluginEventId) {
        case Plugin.ToggleConnection:
          autoReconnectSimConnect = !autoReconnectSimConnect;
          if (_simConnectService.IsConnected()) {
            _simConnectionRequest.Reset();
            _simConnectService.Disconnect();
          }
          else {
            _simConnectionRequest.Set();
            UpdateSimConnectState();
          }
          break;
        case Plugin.Connect:
          autoReconnectSimConnect = true;
          if (!_simConnectService.IsConnected())
            _simConnectionRequest.Set();
          UpdateSimConnectState();
          break;
        case Plugin.Disconnect:
          autoReconnectSimConnect = false;
          _simConnectionRequest.Reset();
          if (_simConnectService.IsConnected())
            _simConnectService.Disconnect();
          else
            UpdateSimConnectState();
          break;
        case Plugin.ReloadStates:
          SetupSimVars();
          break;

        case Plugin.ActionRepeatIntervalInc:
        case Plugin.ActionRepeatIntervalDec:
        case Plugin.ActionRepeatIntervalSet:
          if (action.ValueIndex < dataArry.Length && double.TryParse(dataArry[action.ValueIndex], out var interval)) {
            if (pluginEventId == Plugin.ActionRepeatIntervalInc)
              interval = Settings.ActionRepeatInterval.RealValue + interval;
            else if (pluginEventId == Plugin.ActionRepeatIntervalDec)
              interval = Settings.ActionRepeatInterval.RealValue - interval;
            interval = Math.Clamp(interval, Settings.ActionRepeatInterval.MinValue, Settings.ActionRepeatInterval.MaxValue);
            if (interval != Settings.ActionRepeatInterval.RealValue)
              _client.SettingUpdate(Settings.ActionRepeatInterval.Name, $"{interval:F0}");  // this will trigger the actual value update
          }
          break;

        case Plugin.SetSimVar:
          if (dataArry.Length > 2 && dataArry[0].EndsWith(']') && (dataArry[0].IndexOf('[') is var brIdx && brIdx++ > -1)) {
            var varId = dataArry[0][brIdx..^1];
            if (!_settableSimVarIds.TryGetValue(varId, out Definition def) || !statesDictionary.TryGetValue(def, out SimVarItem simVar)) {
              _logger.LogWarning($"Could not find definition for settable SimVar Id: '{varId}' Name: '{dataArry[0]}'");
              break;
            }
            if (simVar.IsStringType) {
              if (!simVar.SetValue(new StringVal(dataArry[1]))) {
                _logger.LogWarning($"Could not set string value '{dataArry[1]}' for SimVar Id: '{varId}' Name: '{dataArry[0]}'");
                break;
              }
            }
            else if (!TryEvaluateValue(dataArry[1], out double dVal) || !simVar.SetValue(dVal)) {
              _logger.LogWarning($"Could not set numeric value '{dataArry[1]}' for SimVar Id: '{varId}' Name: '{dataArry[0]}'");
              break;
            }
            if (new BooleanString(dataArry[2]) && !_simConnectService.ReleaseAIControl(simVar.Def))
              break;
            _simConnectService.SetDataOnSimObject(simVar);
          }
          break;

        default:
          // No other types of events supported right now.
          break;
      }
    }

    private void ProcessSimEvent(ActionEventType action, Enum eventId, in string[] dataArry) {
      uint dataUint = 0;
      if (action.ValueIndex > -1 && action.ValueIndex < dataArry.Length) {
        double dataReal = double.NaN;
        var valStr = dataArry[action.ValueIndex];
        switch (action.ValueType) {
          case DataType.Number:
          case DataType.Text:
            if (!TryEvaluateValue(valStr, out dataReal)) {
              _logger.LogWarning($"Data conversion failed for action '{action.ActionId}' on sim event '{_reflectionService.GetSimEventNameById(eventId)}'.");
              return;
            }
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
      _logger.LogDebug($"Firing Sim Event - action: {action.ActionId}; category: {action.CategoryId}; name: {_reflectionService.GetSimEventNameById(eventId)}; data {dataUint}");
      _simConnectService.TransmitClientEvent(action.CategoryId, eventId, dataUint);
    }

    /// <summary>
    /// Handles an array of `Setting` types sent from TP. This could come from either the
    /// initial `OnInfoEvent` message, or the dedicated `OnSettingsEvent` message.
    /// </summary>
    /// <param name="settings"></param>
    private void ProcessPluginSettings(IReadOnlyCollection<TouchPortalSDK.Messages.Models.Setting> settings) {
      if (settings == null)
        return;
      // change tracking
      string oldCfgPath = Settings.UserConfigFilesPath.StringValue;
      string oldCfgFiles = Settings.UserStateFiles.StringValue;
      // loop over incoming new settings
      foreach (var s in settings) {
        if (pluginSettingsDictionary.TryGetValue(s.Name, out PluginSetting setting)) {
          setting.SetValueFromString(s.Value);
          if (!string.IsNullOrWhiteSpace(setting.TouchPortalStateId))
            _client.StateUpdate(setting.TouchPortalStateId, setting.StringValue);
        }
      }
      PluginConfig.UserConfigFolder = Settings.UserConfigFilesPath.StringValue;  // will (re-)set to default if blank.
      bool stateReload = oldCfgPath != Settings.UserConfigFilesPath.StringValue || oldCfgFiles != Settings.UserStateFiles.StringValue;
      if (stateReload && _simConnectService.IsConnected())
        SetupSimVars();
    }

    private void UpdateSimConnectState() {
      string stat = "true";
      if (!_simConnectService.IsConnected())
        stat = _simConnectionRequest.IsSet ? "connecting" : "false";
      _client.StateUpdate(PluginId + ".Plugin.State.Connected", stat);
    }

    private bool TryEvaluateValue(string strValue, out double value) {
      value = double.NaN;
      try {
        value = Convert.ToDouble(_expressionEvaluator.Compute(strValue, null));
      }
      catch (Exception e) {
        _logger.LogWarning(e, $"Failed to convert data value '{strValue}' to numeric.");
        return false;
      }
      return true;
    }

    #endregion Plugin Events and Handlers

    #region TouchPortalSDK Events       ///////////////////////////////

    public void OnInfoEvent(InfoEvent message) {
      var runtimeVer = string.Format("{0:X}", VersionInfo.GetProductVersionNumber() >> 8);  // strip the patch version
      _logger?.LogInformation(
        $"Touch Portal Connected with: TP v{message.TpVersionString}, SDK v{message.SdkVersion}, {PluginId} entry.tp v{message.PluginVersion}, " +
        $"{VersionInfo.AssemblyName} running v{VersionInfo.GetProductVersionString()} ({runtimeVer})"
      );

      // convert the entry.tp version back to the actual decimal value
      uint tpVer;
      try   { tpVer = uint.Parse($"{message.PluginVersion}", System.Globalization.NumberStyles.HexNumber); }
      catch { tpVer = VersionInfo.GetProductVersionNumber() >> 8; }
      _entryFileType = (tpVer & PluginConfig.ENTRY_FILE_VER_MASK_NOSTATES) > 0 ? EntryFileType.NoStates
          : (tpVer & PluginConfig.ENTRY_FILE_VER_MASK_CUSTOM) > 0 ? EntryFileType.Custom
          : EntryFileType.Default;

      _logger.LogInformation($"Detected {_entryFileType} type entry.tp definition file.");

      ProcessPluginSettings(message.Settings);
      autoReconnectSimConnect = Settings.ConnectSimOnStartup.BoolValue;  // we only care about this at startup

      _client.StateUpdate(PluginId + ".Plugin.State.RunningVersion", runtimeVer);
      _client.StateUpdate(PluginId + ".Plugin.State.EntryVersion", $"{tpVer & 0xFFFFFF:X}");
      _client.StateUpdate(PluginId + ".Plugin.State.ConfigVersion", $"{tpVer >> 24:X}");
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
          var timer = new Timer(Settings.ActionRepeatInterval.IntValue);
          timer.Elapsed += delegate { ProcessEvent(message); };
          if (_repeatingActionTimers.TryAdd(message.ActionId, timer))
            timer.Start();
          else
            timer.Dispose();
          break;

        case TouchPortalSDK.Messages.Models.Enums.Press.Up:
          // "On Hold" released ("up" event). Mark action for removal from repeating queue.
          if (_repeatingActionTimers.TryRemove(message.ActionId, out var tim))
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
