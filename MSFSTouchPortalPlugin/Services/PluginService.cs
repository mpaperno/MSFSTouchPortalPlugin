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

    const int SIM_RECONNECT_DELAY_SEC = 30;   // SimConnect connection attempts delay on failure

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
    private Dictionary<string, PluginSetting> pluginSettingsDictionary = new();
    private readonly ConcurrentDictionary<Definition, SimVarItem> _statesDictionary = new();
    private readonly ConcurrentDictionary<Definition, SimVarItem> _customIntervalStates = new();  // IDs of SimVars which need periodic value polling; concurrent because the polling happens in separate task from setter.
    private readonly Dictionary<string, Definition> _settableSimVarIds = new();   // mapping of settable SimVar IDs for lookup in statesDictionary
    private readonly Dictionary<Definition, string> _addedSimVars = new();  // keep track of definitions added by user
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
      _cancellationToken = cancellationToken;

      _logger.LogInformation($"======= {PluginId} Starting =======");

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
      _logger.LogInformation($"======= {PluginId} Stopped =======");
      return Task.CompletedTask;
    }

    /// <summary>
    /// Initialized the Touch Portal Message Processor
    /// </summary>
    private bool Initialize() {
      // set up data which may be sent to TP upon initial connection
      _pluginConfig.Init();
      actionsDictionary = _reflectionService.GetActionEvents();
      pluginSettingsDictionary = _reflectionService.GetSettings();

      if (!_client.Connect()) {
        _logger.LogCritical("Failed to connect to Touch Portal! Quitting.");
        return false;
      }

      // Setup SimConnect Events
      _simConnectService.OnDataUpdateEvent += SimConnectEvent_OnDataUpdateEvent;
      _simConnectService.OnConnect += SimConnectEvent_OnConnect;
      _simConnectService.OnDisconnect += SimConnectEvent_OnDisconnect;
      _simConnectService.OnException += SimConnectEvent_OnException;

      return true;
    }

    private async Task SimConnectionMonitor() {
      _logger.LogDebug("SimConnectionMonitor task started.");
      uint hResult;
      try {
        while (!_cancellationToken.IsCancellationRequested) {
          _simConnectionRequest.Wait(_cancellationToken);
          if (!_simConnectService.IsConnected() && (hResult = _simConnectService.Connect(Settings.SimConnectConfigIndex.UIntValue)) != SimConnectService.S_OK) {
            if (hResult != SimConnectService.E_FAIL) {
              if (hResult == SimConnectService.E_INVALIDARG)
                _logger.LogError("SimConnect returned IVALID ARGUMENT for SimConnect.cfg index value " + Settings.SimConnectConfigIndex.UIntValue.ToString() +
                  ". Connection attempts aborted. Please fix setting or config. file and retry.");
              else
                _logger.LogError("Unknown exception occurred trying to connect to SimConnect. Connection attempts aborted, please check plugin logs. Error code/message: " + $"{hResult:X}");
              autoReconnectSimConnect = false;
              _simConnectionRequest.Reset();
              UpdateSimConnectState();
              continue;
            }
            _logger.LogWarning("Connection to Simulator failed, retrying in " + SIM_RECONNECT_DELAY_SEC.ToString() + " seconds...");
            await Task.Delay(SIM_RECONNECT_DELAY_SEC * 1000, _cancellationToken);  // delay on connection error
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
          IReadOnlyCollection<Timer> timers = _repeatingActionTimers.Values.ToArray();
          foreach (Timer tim in timers)
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
      IReadOnlyCollection<SimVarItem> vars = _customIntervalStates.Values.ToArray();
      foreach (var simVar in vars) {
        // Check if a value update is required based on the SimVar's internal tracking mechanism.
        if (simVar.UpdateRequired) {
          simVar.SetPending(true);
          _simConnectService.RequestDataOnSimObjectType(simVar);
        }
      }
    }

    #endregion Startup, Shutdown and Processing Tasks

    #region SimConnect Events   /////////////////////////////////////

    private void SimConnectEvent_OnConnect(SimulatorInfo info) {
      _logger.LogInformation("Connected to " + info.ToString());

      _simConnectionRequest.Reset();
      _simTasksCTS = new CancellationTokenSource();
      _simTasksCancelToken = _simTasksCTS.Token;

      // Register Action events
      var eventMap = _reflectionService.GetClientEventIdToNameMap();
      foreach (var m in eventMap)
        _simConnectService.MapClientEventToSimEvent(m.Key, m.Value.EventName, m.Value.GroupId);
      // must be called after adding notifications
      _simConnectService.SetNotificationGroupPriorities();

      // Register SimVars for States
      RegisterAllSimVars();

      // start checking timer events
      _pluginEventsTask = Task.Run(PluginEventsTask);
      _pluginEventsTask.ConfigureAwait(false);  // needed?

      UpdateSimConnectState();
    }

    private void SimConnectEvent_OnDataUpdateEvent(Definition def, Definition req, object data) {
      // Lookup State Mapping.
      if (!_statesDictionary.TryGetValue(def, out SimVarItem simVar))
        return;

      // Update SimVarItem value and TP state on changes.
      // Sim vars sent on a standard SimConnect request period will only be sent when changed by > simVar.DeltaEpsilon anyway, so skip the equality check.
      // SimVarItem.SetValue() takes care of setting the correct value type, any unit conversions needed, and resets any expiry timers. Returns false if value is of the wrong type.
      if ((!simVar.NeedsScheduledRequest || !simVar.ValueEquals(data)) && simVar.SetValue(data))
        _client.StateUpdate(simVar.TouchPortalStateId, simVar.FormattedValue);
    }

    private void SimConnectEvent_OnDisconnect() {
      _logger.LogInformation("SimConnect Disconnected");

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

    private void SimConnectEvent_OnException(RequestTrackingData data) {
      _logger.LogWarning($"SimConnect Request Exception: " + data.ToString());
    }

    #endregion SimConnect Events

    #region SimVar Setup Handlers   /////////////////////////////////////

    // This is done once after TP connects or upon the "Reload States" action.
    // It is safe to be called multiple times if config files need to be reloaded. SimConnect does not have to be connected.
    private void SetupSimVars() {
      // We may need to generate all, some, or no states dynamically.
      bool allStatesDynamic = (_entryFileType == EntryFileType.NoStates);
      bool someStateDynamic = false;
      IReadOnlyCollection<SimVarItem> simVars;
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
      simVars = _pluginConfig.LoadSimVarStateConfigs();
      _logger.LogInformation($"Loaded {simVars.Count} SimVars from file(s).");

      // Now create the SimVars and track them. We're probably not connected to SimConnect at this point so the registration may happen later.
      foreach (var simVar in simVars)
        AddSimVar(simVar, allStatesDynamic || (someStateDynamic && !defaultStates.Contains(simVar.Id)), true);

      UpdateAllSimVarsList();
      UpdateSettableSimVarsList();
    }

    private void RegisterAllSimVars() {
      if (_simConnectService.IsConnected())
        foreach (var simVar in _statesDictionary.Values)
          _simConnectService.RegisterToSimConnect(simVar);
    }

    private bool AddSimVar(SimVarItem simVar, bool dynamicState = true, bool postponeUpdate = false) {
      if (simVar == null)
        return false;

      if (_statesDictionary.ContainsKey(simVar.Def))
        RemoveSimVar(simVar.Def, true);

      // Register it. If the SimVar gets regular updates (not custom polling) then this also starts the data requests for this value.
      // If we're not connected now then the var will be registered in RegisterAllSimVars() when we do connect.
      if (_simConnectService.IsConnected() && !_simConnectService.RegisterToSimConnect(simVar))
        return false;

      _statesDictionary.TryAdd(simVar.Def, simVar);

      // Does this var need value polling?
      if (simVar.NeedsScheduledRequest)
        _customIntervalStates.TryAdd(simVar.Def, simVar);
      if (simVar.CanSet)
        _settableSimVarIds.TryAdd(simVar.Id, simVar.Def);
      // Need a dynamic state?
      if (dynamicState && !_dynamicStateIds.Contains(simVar.TouchPortalStateId)) {
        _dynamicStateIds.Add(simVar.TouchPortalStateId);  // keep track for removing them later if needed
        _client.CreateState(simVar.TouchPortalStateId, Categories.PrependFullCategoryName(simVar.CategoryId, simVar.Name), simVar.DefaultValue);
        _logger.LogTrace($"Created dynamic state {simVar.TouchPortalStateId}'.");
      }

      if (!postponeUpdate) {
        UpdateAllSimVarsList();
        UpdateSettableSimVarsList();
      }
      _logger.LogTrace($"Added SimVar: {simVar.ToDebugString()}");
      return true;
    }

    // note that the stored list of custom added SimVars is not affected here so that we can reload those during a SimConnect restart
    private bool RemoveSimVar(Definition def, bool postponeUpdate = false) {
      if (!_statesDictionary.TryRemove(def, out var simVar))
        return false;
      _customIntervalStates.TryRemove(simVar.Def, out _);
      _settableSimVarIds.Remove(simVar.Id);
      _simConnectService.ClearDataDefinition(simVar.Def);
      if (_dynamicStateIds.Remove(simVar.TouchPortalStateId))
        _client.RemoveState(simVar.TouchPortalStateId);
      if (!postponeUpdate) {
        UpdateAllSimVarsList();
        UpdateSettableSimVarsList();
      }
      return true;
    }

    private bool AddCustomSimVar(SimVarItem simVar) {
      if (_addedSimVars.FirstOrDefault(s => s.Value == simVar.Id) is var old && !string.IsNullOrEmpty(old.Value)) {
        RemoveSimVar(old.Key, true);
        _addedSimVars.Remove(old.Key);
      }
      return AddSimVar(simVar) && _addedSimVars.TryAdd(simVar.Def, simVar.Id);
    }

    private bool RemoveCustomSimVar(SimVarItem simVar) {
      if (RemoveSimVar(simVar.Def))
        return _addedSimVars.Remove(simVar.Def);
      return false;
    }

    private void LoadCustomSimVarsFromFile(string filepath) {
      var simVars = _pluginConfig.LoadSimVarItems(true, filepath);
      foreach (var simVar in simVars) {
        if (_addedSimVars.ContainsKey(simVar.Def))
          RemoveCustomSimVar(simVar);
        AddCustomSimVar(simVar);
      }
      _logger.LogInformation($"Loaded {simVars.Count} SimVar States from file '{filepath}'");
    }

    private void SaveSimVarsToFile(string filepath, bool customOnly = true) {
      bool ok = false;
      if (customOnly)
        ok = _pluginConfig.SaveSimVarItems((from s in _statesDictionary.Values where _addedSimVars.ContainsKey(s.Def) select s), true, filepath);
      else
        ok = _pluginConfig.SaveSimVarItems(_statesDictionary.Values, true, filepath);
      if (ok)
        _logger.LogInformation($"Saved {(customOnly ? "Custom" : "All")} SimVar States to file '{filepath}'");
    }

    #endregion SimVar Setup Handlers

    #region Data Updaters    /////////////////////////////////////

    // Action data list updaters

    // common handler for other action list updaters
    private void UpdateActionDataList(PluginActions actionId, string dataId, IEnumerable<string> data, string instanceId = null) {
      _client.ChoiceUpdate(PluginId + $".Plugin.Action.{actionId}.Data.{dataId}", data.ToArray(), instanceId);
    }

    // List of settable SimVars
    private void UpdateSettableSimVarsList() {
      UpdateActionDataList(PluginActions.SetSimVar, "VarName", GetSimVarSelectorList(settable: true));
    }

    // List of all current SimVars
    private void UpdateAllSimVarsList() {
      UpdateActionDataList(PluginActions.RemoveSimVar, "VarName", GetSimVarSelectorList(settable: false));
    }

    // Unit lists
    private void UpdateUnitsLists() {
      UpdateActionDataList(PluginActions.AddCustomSimVar, "Unit", Units.ListUsable);
      UpdateActionDataList(PluginActions.AddKnownSimVar, "Unit", Units.ListUsable);
    }

    // Available plugin's state/action categories
    private void UpdateUCategoryLists() {
      UpdateActionDataList(PluginActions.AddCustomSimVar, "CatId", Categories.ListUsable);
      UpdateActionDataList(PluginActions.AddKnownSimVar, "CatId", Categories.ListUsable);
    }

    // List of imported SimVariable categories
    void UpdateSimVarCategories() {
      UpdateActionDataList(PluginActions.AddKnownSimVar, "SimCatName", _pluginConfig.ImportedSimVarCategoryNames);
    }

    // List of imported SimVariables per category
    private void UpdateKnownSimVars(string categoryName, string instanceId) {
      // select variable names in category and mark if already used
      if (_pluginConfig.TryGetImportedSimVarsForCateogy(categoryName, out var vars)) {
        var list = vars.Select(v => (_statesDictionary.Values.FirstOrDefault(s => s.SimVarName == v.SimVarName) == null ? "" : "* ") + v.TouchPortalSelectorName);
        UpdateActionDataList(PluginActions.AddKnownSimVar, "VarName", list, instanceId);
      }
    }

    // This will re-populate the list of Units for an action instance, but put the default Unit for the selected SimVar at the top.
    // Possible future improvement would be to only present units of a compatible conversion type.
    private void UpdateUnitsListForKnownSimVar(string varName, string instanceId) {
      if (_pluginConfig.TryGetImportedSimVarBySelector(varName, out SimVariable var)) {
        var list = new[] { var.Unit }.Concat(Units.ListUsable);
        UpdateActionDataList(PluginActions.AddKnownSimVar, "Unit", list, instanceId);
      }
    }

    // List of imported Sim Event categories
    void UpdateSimEventCategories() {
      UpdateActionDataList(PluginActions.SetKnownSimEvent, "SimCatName", _pluginConfig.ImportedSimEvenCategoryNames);
    }

    // List of imported SimEvents per category
    private void UpdateKnownSimEventsForCategory(string categoryName, string instanceId) {
      if (_pluginConfig.TryGetImportedSimEventNamesForCateogy(categoryName, out var list))
        UpdateActionDataList(PluginActions.SetKnownSimEvent, "EvtId", list, instanceId);
    }

    // Misc. data update/clear

    private void UpdateSimConnectState() {
      string stat = "true";
      if (!_simConnectService.IsConnected())
        stat = _simConnectionRequest.IsSet ? "connecting" : "false";
      _client.StateUpdate(PluginId + ".Plugin.State.Connected", stat);
    }

    private void ClearRepeatingActions() {
      foreach (var act in _repeatingActionTimers) {
        if (_repeatingActionTimers.TryRemove(act.Key, out var tim)) {
          tim.Dispose();
        }
      }
    }

    #endregion Data Updaters

    #region Plugin Action/Event Handlers    /////////////////////////////////////

    // Handles some basic actions like sim connection and repeat rate, with optional data value(s).
    private void ProcessPluginCommandAction(PluginActions actionId, ActionData data = null) {
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
        case PluginActions.ActionRepeatIntervalSet: {
          // preserve backwards compatibility with old actions which used indexed data IDs
          if (data == null || !(data.TryGetValue("Value", out var sVal) || data.TryGetValue("1", out sVal)) || !TryEvaluateValue(sVal, out var value)) {
            _logger.LogWarning($"Could not find or parse numeric value for repeat rate from data: {ActionDataToKVPairString(data)}");
            break;
          }
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
    }

    // Parse and process PluginActions.SetSimVar action
    private void SetSimVarValueFromActionData(ActionData data) {
      if (!data.TryGetValue("VarName", out var varName) ||
          !data.TryGetValue("Value", out var value) ||
          !TryGetSimVarIdFromActionData(varName, out string varId)) {
        _logger.LogWarning($"Could not parse required action parameters for {PluginActions.SetSimVar} from data: {ActionDataToKVPairString(data)}");
        return;
      }
      if (!_settableSimVarIds.TryGetValue(varId, out Definition def) || !_statesDictionary.TryGetValue(def, out SimVarItem simVar)) {
        _logger.LogError($"Could not find definition for settable SimVar Id: '{varId}' Name: '{varName}'");
        return;
      }

      if (simVar.IsStringType) {
        if (!simVar.SetValue(new StringVal(value))) {
          _logger.LogError($"Could not set string value '{value}' for SimVar Id: '{varId}' Name: '{varName}'");
          return;
        }
      }
      else if (!TryEvaluateValue(value, out double dVal) || !simVar.SetValue(dVal)) {
        _logger.LogError($"Could not set numeric value '{value}' for SimVar Id: '{varId}' Name: '{varName}'");
        return;
      }
      if (data.TryGetValue("RelAi", out var relAi) && new BooleanString(relAi) && !_simConnectService.ReleaseAIControl(simVar.Def))
        return;

      _simConnectService.SetDataOnSimObject(simVar);
    }

    private void AddSimVarFromActionData(PluginActions actId, ActionData data) {
      if (!data.TryGetValue("VarName", out var varName) || string.IsNullOrWhiteSpace(varName) ||
          !data.TryGetValue("CatId", out var sCatId)    || !Categories.TryGetCategoryId(sCatId, out Groups catId) ||
          !data.TryGetValue("Unit", out var sUnit)
      ) {
        _logger.LogWarning($"Could not parse required action parameters for {actId} from data: {ActionDataToKVPairString(data)}");
        return;
      }

      // check if we've imported this var and have meta data
      SimVariable impSimVar = _pluginConfig.GetOrCreateImportedSimVariable(varName);
      if (impSimVar == null)  // highly unlikely
        return;

      uint index = 0;
      bool haveIndexValue = (data.TryGetValue("VarIndex", out var sIndex) && uint.TryParse(sIndex, out index) && index > 0);
      impSimVar.Indexed = impSimVar.Indexed || haveIndexValue;
      if (impSimVar.Indexed)
        impSimVar.SimVarName = string.Concat(impSimVar.SimVarName, ":", Math.Clamp(index, 1, 99).ToString());
      impSimVar.CanSet = impSimVar.CanSet || (data.TryGetValue("CanSet", out var sCanSet) && new BooleanString(sCanSet));

      // create the SimVarItem from collected data
      var simVar = new SimVarItem() {
        Id = impSimVar.Id,
        Name = impSimVar.Name,
        SimVarName = impSimVar.SimVarName,
        CategoryId = catId,
        Unit = sUnit ?? "number",
        CanSet = impSimVar.CanSet,
        StringFormat = data.GetValueOrDefault("Format", string.Empty).Trim(),
        DefaultValue = data.GetValueOrDefault("DfltVal", string.Empty).Trim(),
        TouchPortalStateId = $"{PluginId}.{catId}.State.{impSimVar.Id}"
      };
      if (data.TryGetValue("UpdPer", out var sPeriod) && Enum.TryParse(sPeriod, out UpdatePeriod period))
        simVar.UpdatePeriod = period;
      if (data.TryGetValue("UpdInt", out var sInterval) && uint.TryParse(sInterval, out uint interval))
        simVar.UpdateInterval = interval;
      if (data.TryGetValue("Epsilon", out var sEpsilon) && float.TryParse(sEpsilon, out float epsilon))
        simVar.DeltaEpsilon = epsilon;

      if (AddCustomSimVar(simVar))
        _logger.LogInformation($"Added new SimVar state from action data: {simVar.ToDebugString()}");
      else
        _logger.LogError($"Failed to add SimVar from action data, check previous log messages. Action data: {ActionDataToKVPairString(data)}");
    }

    private void RemoveSimVarByActionDataName(ActionData data) {
      if (!data.TryGetValue("VarName", out var varName) || !TryGetSimVarIdFromActionData(varName, out string varId)) {
        _logger.LogWarning($"Could not find valid SimVar ID in action data: {ActionDataToKVPairString(data)}'");
        return;
      }
      SimVarItem simVar = _statesDictionary.Values.FirstOrDefault(s => s.Id == varId);
      if (simVar != null && RemoveSimVar(simVar.Def))
        _logger.LogInformation($"Removed SimVar '{simVar.SimVarName}'");
      else
        _logger.LogWarning($"Could not find definition for settable SimVar Id: '{varId}' from Name: '{varName}'");
    }

    // Dynamic sim Events (actions)
    private void ProcessSimEventFromActionData(PluginActions actId, ActionData data) {
      if (!data.TryGetValue("EvtId", out var evtId) || !data.TryGetValue("Value", out var sValue)) {
        _logger.LogWarning($"Could not find required action parameters for {actId} from data: {ActionDataToKVPairString(data)}");
        return;
      }
      // Check for known/imported type, which may have special formatting applied to the name
      if (actId != PluginActions.SetKnownSimEvent || !_pluginConfig.TryGetImportedSimEventIdFromSelector(evtId, out evtId))
        evtId = evtId.Trim();

      Enum eventId;
      // dynamically added event actions have no mappings, the ActionEventType.Id is the SimEventClientId
      if (actionsDictionary.TryGetValue(evtId, out ActionEventType ev)) {
        eventId = ev.Id;
      }
      else {
        ev = new ActionEventType(evtId, Groups.SimSystem, !string.IsNullOrWhiteSpace(sValue), out eventId);
        actionsDictionary[evtId] = ev;
        _reflectionService.AddSimEventNameMapping(eventId, new SimEventRecord(ev.CategoryId, evtId));
        _simConnectService.MapClientEventToSimEvent(eventId, evtId, ev.CategoryId);  // no-op if not connected, will get mapped in the OnConnected handler
      }

      if (_simConnectService.IsConnected())
        ProcessSimEvent(ev, eventId, sValue);
    }

    // Main TP Action event handlers

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
      if (action.TryGetEventMapping(in dataArry, out Enum eventId)) {
        string valStr = null;
        if (action.ValueIndex > -1 && action.ValueIndex < dataArry.Length)
          valStr = dataArry[action.ValueIndex];
        ProcessSimEvent(action, eventId, valStr);
      }
    }

    private void ProcessInternalEvent(ActionEventType action, ActionData data) {
      PluginActions pluginEventId = (PluginActions)action.Id;
      _logger.LogDebug($"Firing Internal Event - action: {action.ActionId}; enum: {pluginEventId}; data: {ActionDataToKVPairString(data)}");
      switch (pluginEventId) {
        case PluginActions.Connection:
        case PluginActions.ActionRepeatInterval: {
          // preserve backwards compatibility with old actions which used indexed data IDs
          if ((data.TryGetValue("Action", out var actId) || data.TryGetValue("0", out actId)) && action.TryGetEventMapping(actId, out Enum eventId))
            ProcessPluginCommandAction((PluginActions)eventId, data);
          else
            _logger.LogWarning($"Could not parse required action parameters for {action.ActionId} from data: {ActionDataToKVPairString(data)}");
          break;
        }

        case PluginActions.SetSimVar:
          SetSimVarValueFromActionData(data);
          break;

        case PluginActions.SetCustomSimEvent:
        case PluginActions.SetKnownSimEvent:
          ProcessSimEventFromActionData(pluginEventId, data);
          break;

        case PluginActions.AddCustomSimVar:
        case PluginActions.AddKnownSimVar:
          AddSimVarFromActionData(pluginEventId, data);
          break;

        case PluginActions.RemoveSimVar:
          RemoveSimVarByActionDataName(data);
          break;

        case PluginActions.LoadSimVars: {
          if (data.TryGetValue("VarsFile", out var filepath) && !string.IsNullOrWhiteSpace(filepath))
            LoadCustomSimVarsFromFile(filepath.Trim());
          break;
        }

        case PluginActions.SaveSimVars: {
          if (data.TryGetValue("VarsFile", out var filepath) && !string.IsNullOrWhiteSpace(filepath)) {
            bool customOnly = !action.TryGetEventMapping(data.GetValueOrDefault("VarsSet", "Custom"), out Enum eventId) || (PluginActions)eventId == PluginActions.SaveCustomSimVars;
            SaveSimVarsToFile(filepath.Trim(), customOnly);
          }
          break;
        }

        default:
          break;
      }
    }

    private void ProcessSimEvent(ActionEventType action, Enum eventId, string value = null) {
      uint dataUint = 0;
      if (!string.IsNullOrWhiteSpace(value)) {
        double dataReal = double.NaN;
        switch (action.ValueType) {
          case DataType.Number:
          case DataType.Text:
            if (!TryEvaluateValue(value, out dataReal)) {
              _logger.LogWarning($"Data conversion failed for action '{action.ActionId}' on sim event '{_reflectionService.GetSimEventNameById(eventId)}'.");
              return;
            }
            break;
          case DataType.Switch:
            dataReal = new BooleanString(value);
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
      if (!_statesDictionary.Any() || p[0] != PluginConfig.UserConfigFolder || p[1] != PluginConfig.UserStateFiles)
        SetupSimVars();
    }

    #endregion Plugin Action/Event Handlers

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

      UpdateUCategoryLists();
      UpdateUnitsLists();
      UpdateSimVarCategories();
      UpdateSimEventCategories();
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
      // Handle dynamic list updates based on value selected in another list, indicated by the message.ListId which is an action data ID.
      // There's also a ListChangeEvent.ActionId property, but since our data IDs already contain the action ID, this is moot.
      if (string.IsNullOrWhiteSpace(message.InstanceId) || string.IsNullOrWhiteSpace(message.Value) || string.IsNullOrWhiteSpace(message.ListId))
        return;
      // get the last 3 parts of the data ID, which is in the form of: <ActionId>.Data.<DataId>
      var listParts = message.ListId.Split('.')[^3..];
      if (listParts.Length != 3 || !Enum.TryParse(listParts[0], true, out PluginActions actId))
        return;
      switch (actId) {
        case PluginActions.AddKnownSimVar:
          if (listParts[2] == "SimCatName")
            UpdateKnownSimVars(message.Value, message.InstanceId);
          else if (listParts[2] == "VarName")
            UpdateUnitsListForKnownSimVar(message.Value, message.InstanceId);
          break;
        case PluginActions.SetKnownSimEvent:
          if (listParts[2] == "SimCatName")
            UpdateKnownSimEventsForCategory(message.Value, message.InstanceId);
          break;
        default:
          break;
      }
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

    #endregion TouchPortalSDK Events

    #region Utilities       ///////////////////////////////

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

    private IOrderedEnumerable<string> GetSimVarSelectorList(bool settable = false) =>
      (from s in _statesDictionary.Values where !settable || s.CanSet select Categories.PrependCategoryName(s.CategoryId, s.Name) + $"  [{s.Id}]").OrderBy(n => n);

    private static bool TryGetSimVarIdFromActionData(string varName, out string varId) {
      if (varName.EndsWith(']') && (varName.IndexOf('[') is var brIdx && brIdx++ > -1)) {
        varId = varName[brIdx..^1];
        return true;
      }
      varId = string.Empty;
      return false;
    }

    static string ActionDataToKVPairString(ActionData data) =>
      '{' + string.Join(", ", data.Select(d => $"{d.Key}={d.Value}")) + '}';

    #endregion Utilities

  }

}
