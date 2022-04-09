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
using TouchPortalSDK.Messages.Models;
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
    private Task _pluginEventsTask;
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

      TouchPortalOptions.ActionDataIdSeparator = '.';  // split up action Data Ids
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
        _logger.LogCritical("Failed to connect to Touch Portal! Quitting.");
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
          if (!_simConnectService.IsConnected() && !_simConnectService.Connect(Settings.SimConnectConfigIndex.UIntValue)) {
            _logger.LogWarning("Connection to Simulator failed, retrying in 10 seconds...");
            await Task.Delay(10000, _cancellationToken);  // delay 10s on connection error
          }
        }
      }
      catch (OperationCanceledException) { /* ignore but exit */ }
      catch (ObjectDisposedException) { /* ignore but exit */ }
      catch (Exception e) {
        _logger.LogError(e, "Exception in SimConnectionMonitor task, cannot continue.");
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
        _logger.LogError(e, "Exception in PluginEventsTask task, cannot continue.");
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
      _pluginEventsTask = Task.Run(PluginEventsTask);
      _pluginEventsTask.ConfigureAwait(false);  // needed?
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
      if (_pluginEventsTask.Status == TaskStatus.Running && !_pluginEventsTask.Wait(5000))
        _logger.LogWarning("PluginEventsTask timed out while stopping.");
      try { _pluginEventsTask.Dispose(); }
      catch { /* ignore in case it hung */ }

      ClearRepeatingActions();
      UpdateSimConnectState();

      _pluginEventsTask = null;
      _simTasksCTS?.Dispose();
      _simTasksCTS = null;

      if (autoReconnectSimConnect && !_quitting)
        _simConnectionRequest.Set();  // re-enable connection attempts
    }

    #endregion SimConnect Events

    #region Plugin Events and Handlers   /////////////////////////////////////

    private void SetupSimVars() {
      // We may need to generate all, some, or no states dynamically.
      bool allStatesDynamic = (_entryFileType == EntryFileType.NoStates);
      bool someStateDynamic = false;
      IEnumerable<string> defaultStates = Array.Empty<string>();  // in case we need to create _some_ custom states dynamically

      // First check if we're using a custom config.
      if (PluginConfig.HaveUserStateFiles) {
        // if the user is NOT using dynamic states for everything (entry_no-states.tp) nor a custom entry.tp file,
        // then lets figure out what the default states are so we can create any missing ones dynamically if needed.
        if (_entryFileType == EntryFileType.Default) {
          // get the defaults
          defaultStates = _pluginConfig.LoadSimVarItems(false).Select(s => s.Id);
          someStateDynamic = defaultStates.Any();
        }
        _logger.LogInformation($"Loading custom state file(s) '{PluginConfig.UserStateFiles}' from '{PluginConfig.UserConfigFolder}'.");
      }
      else {  // Default config
        _logger.LogInformation($"Loading default SimVar State file '{PluginConfig.AppConfigFolder}/{PluginConfig.StatesConfigFile}'.");
      }
      // Load the vars.  PluginConfig tracks which files should be loaded, defaults or custom.
      statesDictionary = _pluginConfig.LoadSimVarStateConfigs().ToDictionary(s => s.Def, s => s);
      _logger.LogInformation($"Loaded {simVars.Count} SimVars from file(s).");

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
      _client.ChoiceUpdate(PluginId + ".Plugin.Action.SetSimVar.Data.VarName", list);
    }

    private void ClearRepeatingActions() {
      foreach (var act in _repeatingActionTimers) {
        if (_repeatingActionTimers.TryRemove(act.Key, out var tim)) {
          tim.Dispose();
        }
      }
    }

    private void ProcessPluginCommandAction(PluginActions actionId, double value = double.NaN) {
      switch (actionId) {
        case PluginActions.ToggleConnection:
          ProcessPluginCommandAction(autoReconnectSimConnect ? PluginActions.Disconnect : PluginActions.Connect);
          break;

        case PluginActions.Connect:
          autoReconnectSimConnect = true;
          if (!_simConnectService.IsConnected())
            _simConnectionRequest.Set();
          UpdateSimConnectState();
          break;

        case PluginActions.Disconnect:
          autoReconnectSimConnect = false;
          bool wasSet = _simConnectionRequest.IsSet;
          _simConnectionRequest.Reset();
          if (_simConnectService.IsConnected())
            _simConnectService.Disconnect();
          else if (wasSet)
            _logger.LogInformation("Connection attempts to Simulator were canceled.");
          UpdateSimConnectState();
          break;

        case PluginActions.ReloadStates:
          SetupSimVars();
          break;

        case PluginActions.ActionRepeatIntervalInc:
        case PluginActions.ActionRepeatIntervalDec:
        case PluginActions.ActionRepeatIntervalSet:
          if (double.IsNaN(value))
            break;
          if (actionId == PluginActions.ActionRepeatIntervalInc)
            value = Settings.ActionRepeatInterval.RealValue + value;
          else if (actionId == PluginActions.ActionRepeatIntervalDec)
            value = Settings.ActionRepeatInterval.RealValue - value;
          value = Math.Clamp(value, Settings.ActionRepeatInterval.MinValue, Settings.ActionRepeatInterval.MaxValue);
          if (value != Settings.ActionRepeatInterval.RealValue)
            _client.SettingUpdate(Settings.ActionRepeatInterval.Name, $"{value:F0}");  // this will trigger the actual value update
        break;
      }
    }

    private void SetSimVarValueFromActionData(string varName, string value, bool releaseAi) {
      if (!varName.EndsWith(']') || (varName.IndexOf('[') is var brIdx && brIdx++ < 0)) {
        _logger.LogWarning($"Could not find ID in SimVar Name: '{varName}'");
        return;
      }

      var varId = varName[brIdx..^1];
      if (!_settableSimVarIds.TryGetValue(varId, out Definition def) || !statesDictionary.TryGetValue(def, out SimVarItem simVar)) {
        _logger.LogWarning($"Could not find definition for settable SimVar Id: '{varId}' Name: '{varName}'");
        return;
      }
      if (simVar.IsStringType) {
        if (!simVar.SetValue(new StringVal(value))) {
          _logger.LogWarning($"Could not set string value '{value}' for SimVar Id: '{varId}' Name: '{varName}'");
          return;
        }
      }
      else if (!TryEvaluateValue(value, out double dVal) || !simVar.SetValue(dVal)) {
        _logger.LogWarning($"Could not set numeric value '{value}' for SimVar Id: '{varId}' Name: '{varName}'");
        return;
      }
      if (releaseAi && !_simConnectService.ReleaseAIControl(simVar.Def))
        return;

      _simConnectService.SetDataOnSimObject(simVar);
    }

    private void ProcessEvent(ActionEvent actionEvent) {
      if (!actionsDictionary.TryGetValue(actionEvent.ActionId, out ActionEventType action))
        return;

      if (action.CategoryId == Groups.Plugin) {
        ProcessInternalEvent(action, actionEvent.Data);
        return;
      }

      if (!_simConnectService.IsConnected())
        return;

      var dataArry = actionEvent.Data.Values.ToArray();
      if (action.TryGetEventMapping(in dataArry, out Enum eventId))
        ProcessSimEvent(action, eventId, in dataArry);
    }

    private void ProcessInternalEvent(ActionEventType action, ActionData data) {
      PluginActions pluginEventId = (PluginActions)action.Id;
      _logger.LogDebug($"Firing Internal Event - action: {action.ActionId}; enum: {pluginEventId}; data: {string.Join(", ", data.Select(d => $"{d.Key}={d.Value}"))}");
      switch (pluginEventId) {
        case PluginActions.Connection:
        case PluginActions.ActionRepeatInterval: {
          // preserve backwards compatibility with old actions which used indexed data IDs
          if ((data.TryGetValue("Action", out var actId) || data.TryGetValue("0", out actId)) && action.TryGetEventMapping(actId, out Enum eventId)) {
            if (!(data.TryGetValue("Value", out var sVal) || data.TryGetValue("1", out sVal)) || !double.TryParse(sVal, out var value))
              value = double.NaN;
            ProcessPluginCommandAction((PluginActions)eventId, value);
          }
          break;
        }

        case PluginActions.SetSimVar: {
          if (data.TryGetValue("VarName", out var varName) && data.TryGetValue("Value", out var value))
            SetSimVarValueFromActionData(varName, value, data.TryGetValue("RelAi", out var relAi) && new BooleanString(relAi));
          break;
        }

        default:
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

    // Handles an array of `Setting` types sent from TP/API. This could come from either the
    // initial `OnInfoEvent` message, or the dedicated `OnSettingsEvent` message.
    private void ProcessPluginSettings(IReadOnlyCollection<Setting> settings) {
      if (settings == null)
        return;
      // loop over incoming new settings
      foreach (var s in settings) {
        if (pluginSettingsDictionary.TryGetValue(s.Name, out PluginSetting setting)) {
          setting.SetValueFromString(s.Value);
          if (!string.IsNullOrWhiteSpace(setting.TouchPortalStateId))
            _client.StateUpdate(setting.TouchPortalStateId, setting.StringValue);
        }
      }
      // change tracking
      string[] p = new[] { PluginConfig.UserConfigFolder, PluginConfig.UserStateFiles };
      PluginConfig.UserConfigFolder = Settings.UserConfigFilesPath.StringValue;  // will (re-)set to default if needed.
      PluginConfig.UserStateFiles = Settings.UserStateFiles.StringValue;         // will (re-)set to default if needed.
      // compare with actual current config values (not Settings) because they may not have changed even if settings string did
      // states dict will be empty on initial startup
      if (p[0] != PluginConfig.UserConfigFolder || p[1] != PluginConfig.UserStateFiles)
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
      _logger?.LogInformation(new EventId(1, "Connected"),
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
      autoReconnectSimConnect = Settings.ConnectSimOnStartup.BoolValue;  // we only care about the Settings value at startup

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
