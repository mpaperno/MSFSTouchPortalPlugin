/*
This file is part of the MSFS Touch Portal Plugin project.
https://github.com/mpaperno/MSFSTouchPortalPlugin

COPYRIGHT:
(c) 2020 Tim Lewis;
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

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MSFSTouchPortalPlugin.Configuration;
using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Enums;
using MSFSTouchPortalPlugin.Helpers;
using MSFSTouchPortalPlugin.Interfaces;
using MSFSTouchPortalPlugin.Objects;
using MSFSTouchPortalPlugin.Types;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Globalization;
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
    const string PLUGIN_ID = "MSFSTouchPortalPlugin";
    public string PluginId => PLUGIN_ID;  // for ITouchPortalEventHandler

    const int SIM_RECONNECT_DELAY_SEC = 30;   // SimConnect connection attempts delay on failure
    const int MAX_LOG_MSGS_FOR_STATE  = 12;   // maximum number of log lines to send in the LogMessages state

    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly ILogger<PluginService> _logger;
    private readonly ITouchPortalClient _client;
    private readonly ISimConnectService _simConnectService;
    private readonly IReflectionService _reflectionService;
    private readonly PluginConfig _pluginConfig;
    private readonly SimVarCollection _simVarCollection;
    private readonly HubHopPresetsCollection _presets = new();
    private readonly DocImportsCollection _imports = new();
    private readonly ConnectorTracker _connectorTracker = new();

    private bool _quitting;                          // prevent recursion at shutdown
    private CancellationToken _cancellationToken;    // main run enable token passed from Program startup in StartAsync()
    private CancellationTokenSource _simTasksCTS;    // for _simTasksCancelToken
    private CancellationToken _simTasksCancelToken;  // used to shut down local task(s) only needed while simulator is connected
    private Task _pluginEventsTask;                  // runs timed events needed while simulator is connected
    private readonly ManualResetEventSlim _simConnectionRequest = new(false);  // is Set when connection should be attempted, SimConnectionMonitor task will wait until this, or cancellation token, is set.
    private readonly ManualResetEventSlim _simAutoConnectDisable = new(true);  // basically the opposite, used to break out of the reconnection wait timeout and as a flag to indicate if _simConnectionRequest should be Set (eg. at startup).

    private Dictionary<string, ActionEventType> actionsDictionary = new();
    private Dictionary<string, PluginSetting> pluginSettingsDictionary = new();
    private IReadOnlyDictionary<int, string> _localVariablesList = null;
    private readonly ConcurrentDictionary<string, Timer> _repeatingActionTimers = new();  // storage for temporary repeating (held) action timers, index by action ID
    private readonly ConcurrentQueue<string> _logMessages = new();  // stores the last MAX_LOG_MSGS_FOR_STATE log messages for the LogMessages state value, used in PluginLogger callback

    private static readonly System.Data.DataTable _expressionEvaluator = new();  // used to evaluate basic math in action data

    private CultureInfo _cultureInfo = CultureInfo.CurrentCulture;
    private readonly string _defaultCultureId = CultureInfo.CurrentCulture.Name;

    SimPauseStates _simPauseState = SimPauseStates.OFF;  // Tracks pause state reported from last Pause_EX1 sim event.

    public PluginService(IHostApplicationLifetime hostApplicationLifetime, ILogger<PluginService> logger,
      ITouchPortalClientFactory clientFactory, ISimConnectService simConnectService, IReflectionService reflectionService,
      PluginConfig pluginConfig, SimVarCollection simVarCollection)
    {
      _hostApplicationLifetime = hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _simConnectService = simConnectService ?? throw new ArgumentNullException(nameof(simConnectService));
      _reflectionService = reflectionService ?? throw new ArgumentNullException(nameof(reflectionService));

      _client = clientFactory?.Create(this) ?? throw new ArgumentNullException(nameof(clientFactory));
      _pluginConfig = pluginConfig ?? throw new ArgumentNullException(nameof(pluginConfig));
      _simVarCollection = simVarCollection ?? throw new ArgumentNullException(nameof(simVarCollection));

      Configuration.HubHop.Common.Logger = _logger;
      _presets.OnDataUpdateEvent += HubHop_OnDataUpdate;
      _presets.OnDataErrorEvent += Collections_OnDataError;

      DocImportsCollection.Logger = _logger;
      _imports.OnDataErrorEvent += Collections_OnDataError;

      PluginLogger.OnMessageReady += new PluginLogger.MessageReadyHandler(OnPluginLoggerMessage);
      TouchPortalOptions.ActionDataIdSeparator = '.';  // split up action Data Ids
      TouchPortalOptions.ValidateCommandParameters = false;  // bypass validation and exceptions
    }

    #region Startup, Shutdown and Processing Tasks      //////////////////

    /// <summary>
    /// Starts the plugin service
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    public Task StartAsync(CancellationToken cancellationToken) {
      _cancellationToken = cancellationToken;

      _logger.LogInformation("======= " + PLUGIN_ID + " Starting =======");

      // register ctrl-c exit handler
      Console.CancelKeyPress += (_, _) => {
        _logger.LogInformation("Quitting due to keyboard interrupt.");
        _hostApplicationLifetime.StopApplication();
      };

      if (!Initialize()) {
        _hostApplicationLifetime.StopApplication();
        return Task.CompletedTask;
      }

      return SimConnectionMonitor();
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
      DisconnectSimConnect();
      if (_pluginConfig.SaveSettings())
        _logger.LogInformation("Saved Settings to file {file}", PluginConfig.SettingsConfigFile);
      if (_client?.IsConnected ?? false) {
        try { _client.Close(); }
        catch (Exception) { /* ignore */ }
      }
      _simConnectService?.Dispose();
      _simConnectionRequest?.Dispose();
      _simAutoConnectDisable?.Dispose();
      _presets?.Dispose();

      _logger.LogInformation("======= " + PLUGIN_ID + " Stopped =======");
      return Task.CompletedTask;
    }

    bool Initialize() {
      // set up data which may be sent to TP upon initial connection
      _pluginConfig.Init();
      actionsDictionary = _reflectionService.GetActionEvents();
      pluginSettingsDictionary = _reflectionService.GetSettings();

      if (!_client.Connect()) {
        _logger.LogCritical("Failed to connect to Touch Portal! Quitting.");
        return false;
      }

      // Set ID of WASIM integration client.
      _simConnectService.UpdateWasmClientId(Settings.WasimClientIdHighByte.ByteValue);
      // Setup SimConnect Events
      _simConnectService.OnDataUpdateEvent += SimConnectEvent_OnDataUpdateEvent;
      _simConnectService.OnConnect += SimConnectEvent_OnConnect;
      _simConnectService.OnDisconnect += SimConnectEvent_OnDisconnect;
      _simConnectService.OnException += SimConnectEvent_OnException;
      _simConnectService.OnEventReceived += SimConnectEvent_OnEventReceived;
      _simConnectService.OnLVarsListUpdated += SimConnect_OnLVarsListUpdated;

      return true;
    }

    private Task SimConnectionMonitor() {
      _logger.LogDebug("SimConnectionMonitor task started.");
      uint hResult;
      // reconnection delay WaitHandle will exit on any of these handles being set (or SIM_RECONNECT_DELAY_SEC timeout)
      var waitHandles = new WaitHandle[] { _cancellationToken.WaitHandle, _simAutoConnectDisable.WaitHandle };
      try {
        while (!_cancellationToken.IsCancellationRequested) {
          _simConnectionRequest.Wait(_cancellationToken);
          if (!_simConnectService.IsConnected && (hResult = _simConnectService.Connect(Settings.SimConnectConfigIndex.UIntValue)) != SimConnectService.S_OK) {
            switch (hResult) {
              case SimConnectService.E_FAIL:   // sim not running, keep trying
                _logger.LogWarning((int)EventIds.SimTimedOut, "Connection to Simulator failed, retrying in {retrySec} seconds...", SIM_RECONNECT_DELAY_SEC.ToString());
                break;

              case SimConnectService.E_TIMEOUT:  // unexpected timeout, keep trying... ?
                _logger.LogWarning((int)EventIds.SimError, "Unexpected timeout while trying to connect to SimConnect. Will keep trying...");
                break;

              case SimConnectService.E_INVALIDARG:  // invalid SimConnect.cfg index value
                _logger.LogError((int)EventIds.SimError,
                  "SimConnect returned IVALID ARGUMENT for SimConnect.cfg index value {cfgIndex}" +
                  ". Connection attempts aborted. Please fix setting or config. file and retry.", Settings.SimConnectConfigIndex.UIntValue.ToString());
                DisconnectSimConnect();
                continue;

              default:  // other unexpected error
                _logger.LogError((int)EventIds.SimError,
                  "Unknown exception occurred trying to connect to SimConnect. Connection attempts aborted, please check plugin logs. " +
                  "Error code/message: {hResult:X}", hResult);
                DisconnectSimConnect();
                continue;

            }
            WaitHandle.WaitAny(waitHandles, SIM_RECONNECT_DELAY_SEC * 1000);  // delay on connection error
          }
        }
      }
      catch (OperationCanceledException) { /* ignore but exit */ }
      catch (ObjectDisposedException) { /* ignore but exit */ }
      catch (Exception e) {
        _logger.LogError((int)EventIds.Ignore, e, "Exception in SimConnectionMonitor task, cannot continue.");
      }
      _logger.LogDebug("SimConnectionMonitor task stopped.");
      return Task.CompletedTask;
    }

    /// <summary>
    /// Task for running various timed SimConnect events on one thread.
    /// This is primarily used for data polling and repeating (held) actions.
    /// </summary>
    async Task PluginEventsTask()
    {
      _logger.LogDebug("PluginEventsTask task started.");
      try {
        while (_simConnectService.IsConnected && !_simTasksCancelToken.IsCancellationRequested) {
          IReadOnlyCollection<Timer> timers = _repeatingActionTimers.Values.ToArray();
          foreach (Timer tim in timers)
            tim.Tick();
          if (!_simConnectService.WasmAvailable)
            CheckPendingRequests();
          await Task.Delay(25, _simTasksCancelToken);
        }
      }
      catch (OperationCanceledException) { /* ignore but exit */ }
      catch (ObjectDisposedException) { /* ignore but exit */ }
      catch (Exception e) {
        _logger.LogError((int)EventIds.Ignore, e, "Exception in PluginEventsTask task, cannot continue.");
      }
      _logger.LogDebug("PluginEventsTask task stopped.");
    }

    // runs in PluginEventsTask
    private void CheckPendingRequests() {
      var vars = _simVarCollection.PolledUpdateVars;
      foreach (var simVar in vars) {
        // Check if a value update is required based on the SimVar's internal tracking mechanism.
        if (simVar.NeedsScheduledRequest) {
          if (simVar.UpdateRequired) {
            simVar.SetPending(true);
            _simConnectService.RequestDataOnSimObjectType(simVar);
          }
        }
        else {
          _simVarCollection.RemoveFromPolled(simVar);
        }
      }
    }

    // PluginLogger callback
    private void OnPluginLoggerMessage(string message, LogLevel logLevel = LogLevel.Information, EventId eventId = default) {
      //_logger.LogDebug($"Got message from our logger! {logLevel}: {message}");
      var evId = (EventIds)eventId.Id;
      if (evId == EventIds.Ignore)
        return;
      while (_logMessages.Count >= MAX_LOG_MSGS_FOR_STATE)
        _logMessages.TryDequeue(out _);
      _logMessages.Enqueue(message);
      if (_client.IsConnected) {
        if (evId == EventIds.None && logLevel > LogLevel.Information)
          evId = EventIds.PluginError;
        if (evId != EventIds.None)
          UpdateSimSystemEventState(evId, message);
        UpdateTpStateValue("LogMessages", string.Join('\n', _logMessages.ToArray()));
      }
    }

    // HubHopPresetsCollection data update callback
    void HubHop_OnDataUpdate(bool updated)
    {
      if (updated)
        UpdateSimEventAircraft();
      string logMsg = updated ? "HubHop Data Updated" : "No HubHop Updates Detected";
      _logger.LogInformation((int)EventIds.PluginInfo, "{logMsg}; Latest entry date: {time:u}", logMsg, _presets.LatestUpdateTime);
    }

    void Collections_OnDataError(LogLevel severity, string message) {
      _logger.Log(severity, (int)EventIds.PluginError, "{message}", message);
    }

    #endregion Startup, Shutdown and Processing Tasks

    #region SimConnect Events   /////////////////////////////////////

    void SimConnectEvent_OnConnect(SimulatorInfo info)
    {
      _logger.LogInformation((int)EventIds.PluginInfo, "Connected to {info}", info.ToString());

      _simConnectionRequest.Reset();
      _simTasksCTS = new CancellationTokenSource();
      _simTasksCancelToken = _simTasksCTS.Token;
      // start checking timer events
      _pluginEventsTask = Task.Run(PluginEventsTask);
      _pluginEventsTask.ConfigureAwait(false);  // needed?

      UpdateSimConnectState();
    }

    void SimConnectEvent_OnDataUpdateEvent(Definition def, Definition _ /*dwRequestID*/, object data)
    {
      // Lookup State Mapping.
      if (!_simVarCollection.TryGet(def, out SimVarItem simVar))
        return;

      // Update SimVarItem value and TP state on changes.
      // Sim vars sent on a standard SimConnect request period will only be sent when changed by > simVar.DeltaEpsilon anyway, so skip the equality check.
      if (simVar.NeedsScheduledRequest) {
        simVar.SetPending(false);
        if (simVar.ValueEquals(data))
          return;
      }
      // SimVarItem.SetValue() takes care of setting the correct value type and resets any expiry timers. Returns false if value is of the wrong type.
      if (simVar.SetValue(data)) {
        if (simVar.CategoryId != Groups.None)  // assumes anything for an actual category will have a State ID
          _client.StateUpdate(simVar.TouchPortalStateId, simVar.FormattedValue);
        // Check for any Connectors (sliders) which use this state as feedback mechanism.
        if (!simVar.IsStringType)
          UpdateRelatedConnectors(simVar.Id, (double)simVar);
      }
    }

    private void SimConnectEvent_OnDisconnect() {
      _logger.LogInformation("SimConnect Disconnected");

      _simTasksCTS?.Cancel();
      if (_pluginEventsTask != null){
        if (_pluginEventsTask.Status == TaskStatus.Running && !_pluginEventsTask.Wait(5000))
          _logger.LogWarning((int)EventIds.Ignore, "PluginEventsTask timed out while stopping.");
        try { _pluginEventsTask.Dispose(); }
        catch { /* ignore in case it hung */ }
      }
      ClearRepeatingActions();

      _pluginEventsTask = null;
      _simTasksCTS?.Dispose();
      _simTasksCTS = null;

      if (!_simAutoConnectDisable.IsSet && !_quitting)
        _simConnectionRequest.Set();  // re-enable connection attempts

      UpdateSimConnectState();
    }

    private void SimConnectEvent_OnEventReceived(EventIds eventId, Groups categoryId, object data) {
      if (eventId > EventIds.SimEventNone && eventId < EventIds.SimEventLast) {
        if (eventId == EventIds.View) {
          eventId = (uint)data == SimConnectService.VIEW_EVENT_DATA_COCKPIT_3D ? EventIds.ViewCockpit : EventIds.ViewExternal;
        }
        else if (eventId == EventIds.Pause_EX1) {
          SimPauseStates lastState = _simPauseState;
          _simPauseState = (SimPauseStates)Convert.ToByte(data);
          UpdateTpStateValue("SimPauseState", _simPauseState.ToString("G"), Groups.SimSystem);
          if (_simPauseState == SimPauseStates.OFF) {
            // "Unpaused" is reported as a separate event, do not duplicate.
            return;
          }
          else {
            // Report new pause state as an event
            switch (_simPauseState & ~lastState) {
              case SimPauseStates.OFF:
                // no change
                return;
              case SimPauseStates.FULL:
                eventId = EventIds.PauseFull;
                break;
              case SimPauseStates.ACTIVE:
                eventId = EventIds.PauseActive;
                break;
              case SimPauseStates.SIM:
                eventId = EventIds.PauseSimulator;
                break;
              case SimPauseStates.FULL_WITH_SOUND:
                eventId = EventIds.PauseFullWithSound;
                break;
            }
          }
        }
        UpdateSimSystemEventState(eventId, data);
      }
    }

    void SimConnectEvent_OnException(RequestTrackingData data) {
      _logger.LogWarning((int)EventIds.SimError, "SimConnect Request Error: {error}", data.ToString());
    }

    void SimConnect_OnLVarsListUpdated(IReadOnlyDictionary<int, string> list) {
      _localVariablesList = list;
      UpdateLocalVarsLists();
    }

    #endregion SimConnect Events

    #region SimVar Setup Handlers   /////////////////////////////////////

    // This is done once after TP connects or upon the "Reload States" action.
    // It is safe to be called multiple times if config files need to be reloaded. SimConnect does not have to be connected.
    void SetupSimVars()
    {
      // Load the vars.  PluginConfig tracks which files should be loaded, defaults or custom.
      IReadOnlyCollection<SimVarItem> simVars = _pluginConfig.LoadSimVarStateConfigs();
      // Now create the SimVars and track them. We're probably not connected to SimConnect at this point so the registration may happen later.
      int count = 0;
      foreach (var simVar in simVars) {
        byte res = AddSimVar(simVar);
        count += res == 2 ? 1 : res;
      }
      _logger.LogInformation((int)EventIds.PluginInfo, "Registered {count} Variable Request States.", count);

      UpdateTpStateValue("LoadedStateConfigFiles", string.Join(',', _pluginConfig.LoadedStateConfigFiles));
    }

    // Returns: 0 = failed; 1 = added; 2 = replaced
    byte AddSimVar(SimVarItem simVar)
    {
      if (simVar == null)
        return 0;

      byte ret = 1;
      if (_simVarCollection.TryGet(simVar.Id, out var old)) {
        RemoveSimVar(old);
        ret = 2;
      }

      // Create a dynamic state first in case any data updates arrive right away.
      _client.CreateState(simVar.TouchPortalStateId, Categories.PrependFullCategoryName(simVar.CategoryId, simVar.Name), simVar.DefaultValue, Categories.FullCategoryName(simVar.CategoryId));
      // This will trigger registration with SimConnectService, either immediate or when we next connect to the sim.
      _simVarCollection.Add(simVar);

      if (simVar.RegistrationStatus == SimVarRegistrationStatus.Error)
        ret = 0;

      _logger.LogTrace("{action} Request: {simVar}", (ret == 1 ? "Added" : "Replaced"), simVar.ToDebugString());
      return ret;
    }

    bool RemoveSimVar(SimVarItem simVar)
    {
      if (simVar == null || !_simVarCollection.Remove(simVar.Id))
        return false;

      _client.RemoveState(simVar.TouchPortalStateId);
      _logger.LogTrace("Removed Request: {simVar}", simVar.ToDebugString());
      return true;
    }

    // Removes all, or just custom-added, variable definitions
    void RemoveSimVars(bool customOnly)
    {
      var list = customOnly ? _simVarCollection.CustomVariables : _simVarCollection.RequestedVariables;
      foreach (SimVarItem simVar in list)
        RemoveSimVar(simVar);
      bool usingDefaultConfig = _pluginConfig.LoadedStateConfigFiles.Contains(PluginConfig.StatesConfigFile);
      _pluginConfig.LoadedStateConfigFiles.Clear();
      if (customOnly && usingDefaultConfig)
        _pluginConfig.LoadedStateConfigFiles.Add(PluginConfig.StatesConfigFile);
      UpdateTpStateValue("LoadedStateConfigFiles", string.Join(',', _pluginConfig.LoadedStateConfigFiles));
      _logger.LogInformation((int)EventIds.PluginInfo, "Removed {count}{type} variable requests.", list.Count(), (customOnly ? " Custom" : ""));
    }

    void LoadCustomSimVarsFromFile(string filepath)
    {
      var simVars = _pluginConfig.LoadSimVarItems(true, filepath);
      int count = 0;
      if (simVars.Any()) {
        foreach (var simVar in simVars) {
          byte res = AddSimVar(simVar);
          count += res == 2 ? 1 : res;
        }
        UpdateTpStateValue("LoadedStateConfigFiles", string.Join(',', _pluginConfig.LoadedStateConfigFiles));
      }

      if (count == 0)
        _logger.LogError("Did not load any variable requests from file '{file}'", filepath);
      else if (count != simVars.Count)
        _logger.LogWarning((int)EventIds.PluginError, "Loaded only {count} out of {fileCount} variable requests from file '{file}'", count, simVars.Count, filepath);
      // Note: PluginConfig logs what it did -- do not duplicate.
    }

    void SaveSimVarsToFile(string filepath, bool customOnly = true)
    {
      int count;
      if (customOnly)
        count = _pluginConfig.SaveSimVarItems(_simVarCollection.CustomVariables, true, filepath);
      else
        count = _pluginConfig.SaveSimVarItems(_simVarCollection.RequestedVariables, true, filepath);
      if (count > 0)
        _logger.LogInformation((int)EventIds.PluginInfo, "Saved {count}{type} variable requests to file '{filepath}'", count, (customOnly ? " Custom" : ""), filepath);
      else
        _logger.LogError("Error saving SimVar States to file '{filepath}'; Please check log messages.", filepath);
    }

    #endregion SimVar Setup Handlers

    #region Data Updaters    /////////////////////////////////////

    // Action data list updaters

    // common handler for other action list updaters
    private void UpdateActionDataList(PluginActions actionId, string dataId, IEnumerable<string> data, string instanceId = null, bool isConnector = false) {
      UpdateActionDataList(Groups.Plugin, actionId.ToString(), dataId, data, instanceId, isConnector);
    }
    private void UpdateActionDataList(Groups categoryId, string actionId, string dataId, IEnumerable<string> data, string instanceId = null, bool isConnector = false) {
      if (data != null)
        _client.ChoiceUpdate(PLUGIN_ID + $".{categoryId}.{(isConnector ? "Conn" : "Action")}.{actionId}.Data.{dataId}", data.ToArray(), instanceId);
    }

    // Variables, setters and requesters  ----------------------------

    // List of Local vars for all instances
    private void UpdateLocalVarsLists()
    {
      UpdateLocalVarsList(PluginActions.SetLocalVar, null, false);
      UpdateLocalVarsList(PluginActions.SetLocalVar, null, true);
      UpdateLocalVarsList(PluginActions.AddLocalVar, null, false);
    }

    // Unit lists
    void UpdateUnitsLists()
    {
      var units = _imports.UnitNames();
      UpdateActionDataList(PluginActions.AddNamedVariable, "Unit", units);
      UpdateActionDataList(PluginActions.SetVariable, "Unit", units);
      UpdateActionDataList(PluginActions.SetVariable, "Unit", units, null, true);
      // L var unit type is usually "number", except for a few special cases, so put that on top.
      var lunits = units.Prepend("number");
      UpdateActionDataList(PluginActions.AddLocalVar, "Unit", lunits);
      UpdateActionDataList(PluginActions.SetLocalVar, "Unit", lunits);
      UpdateActionDataList(PluginActions.SetLocalVar, "Unit", lunits, null, true);
    }

    // Available plugin's state/action categories
    void UpdateCategoryLists() {
      UpdateActionDataList(PluginActions.AddSimulatorVar, "CatId", Categories.ListUsable);
      UpdateActionDataList(PluginActions.AddLocalVar, "CatId", Categories.ListUsable);
      UpdateActionDataList(PluginActions.AddNamedVariable, "CatId", Categories.ListUsable);
      UpdateActionDataList(PluginActions.AddCalculatedValue, "CatId", Categories.ListUsable);
      UpdateActionDataList(PluginActions.UpdateVarValue, "CatId", Categories.ListUsable);
      UpdateActionDataList(PluginActions.RemoveSimVar, "CatId", Categories.ListUsable);
      foreach (var cat in _reflectionService.GetCategoryAttributes())
        foreach (var conn in cat.Connectors)
          UpdateActionDataList(cat.Id, conn.Id, "FbCatId", Categories.ListUsable, null, true);
    }

    // List of imported SimVariable categories
    void UpdateSimVarCategories()
    {
      // settable
      var cats = _imports.SimVarSystemCategories(true);
      UpdateActionDataList(PluginActions.SetSimulatorVar, "CatId", cats);
      UpdateActionDataList(PluginActions.SetSimulatorVar, "CatId", cats, null, true);
      // all
      UpdateActionDataList(PluginActions.AddSimulatorVar, "SimCatName", _imports.SimVarSystemCategories());
    }

    void UpdateDeprecatedOptions()
    {
      var list = new[] { "[deprecated action, please use new version]" };
      UpdateActionDataList(PluginActions.AddCustomSimVar, "CatId", list);
      UpdateActionDataList(PluginActions.AddCustomSimVar, "Unit", list);
      UpdateActionDataList(PluginActions.AddKnownSimVar, "CatId", list);
      UpdateActionDataList(PluginActions.AddKnownSimVar, "SimCatName", list);
      UpdateActionDataList(PluginActions.AddKnownSimVar, "VarName", list);
      UpdateActionDataList(PluginActions.SetSimVar, "CatId", list);
      UpdateActionDataList(PluginActions.SetSimVar, "CatId", list, null, true);
      UpdateActionDataList(PluginActions.SetSimVar, "VarName", list);
      UpdateActionDataList(PluginActions.SetSimVar, "VarName", list, null, true);
    }

    // List of all current variables per category
    private void UpdateVariablesListPerCategory(Groups categoryId, string actId, string category, string instanceId, bool isConnector) {
      if (Categories.TryGetCategoryId(category, out Groups catId))
        UpdateActionDataList(categoryId, actId, isConnector ? "FbVarName" : "VarName", _simVarCollection.GetSimVarSelectorList(catId), instanceId, isConnector);
    }

    // List of Local vars per action instance
    private void UpdateLocalVarsList(PluginActions action, string instanceId, bool isConnector) {
      if (_localVariablesList == null || !_localVariablesList.Any())
        UpdateActionDataList(action, "VarName", new[] { "<local variables list not available>" }, instanceId, isConnector);
      else if (Settings.SortLVarsAlpha.BoolValue)
        UpdateActionDataList(action, "VarName", _localVariablesList.Values.OrderBy(m => m), instanceId, isConnector);
      else
        UpdateActionDataList(action, "VarName", _localVariablesList.Values, instanceId, isConnector);
    }

    // Update list of settable SimVariables based on selected imported category; for PluginActions.SetSimulatorVar
    void UpdateRegisteredSimVarsList(string categoryName, string instanceId, bool isConnector)
    {
      IEnumerable<string> list = _imports.SimVarNamesForSelector(categoryName, true);
      if (!list.Any())  // unlikely since var choices should already be filtered
        list = new [] {"[no settable variables in this category]"};
      UpdateActionDataList(PluginActions.SetSimulatorVar, "VarName", list, instanceId, isConnector);
    }

    // List of imported SimVariables per category for PluginActions.AddNamedVariable, or Local vars if that type is selected for PluginActions.AddKnownSimVar (deprecated);
    void UpdateKnownSimVars(PluginActions action, string categoryName, string instanceId)
    {
      if (action == PluginActions.AddKnownSimVar) {  // deprecated
        if (categoryName.StartsWith("---"))  // divider
            return;
        if (categoryName.StartsWith("L:")) {
          UpdateLocalVarsList(action, instanceId, false);
          return;
        }
      }
      // select variable names in category and mark if already used
      UpdateActionDataList(action, "VarName", _imports.SimVarNamesForSelector(categoryName, null, _simVarCollection.SimVarNames), instanceId);
    }

    // This will re-populate the list of Units for an action instance with unit types compatible with the selected SimVar's listed unit
    // (eg. if var primary unit is "Feet" then show all length types), and put the default Unit for the selected SimVar at the top.
    void UpdateUnitsListForKnownSimVar(PluginActions action, string varName, string instanceId, bool isConnector)
    {
      var unit = _imports.UnitForSimVarBySelectorName(varName);
      IEnumerable<string> list = null;
      if (unit != null)
        list = _imports.GetCompatibleUnits(unit);  // this will return `unit` at the top even if no other compatible units matched
      //_logger.LogDebug("'{v}' '{u}' {l}", varName, unit, list);
      // If something goes wrong and we didn't find any compatible units then just show them all.
      if (list == null || !list.Any())
        list = _imports.UnitShortNames();
      UpdateActionDataList(action, "Unit", list, instanceId, isConnector);
    }

    static readonly string[] __strArry_NA = new[] { "N/A" };
    static readonly string[] __strArry_NoYes = new[] { "No", "Yes" };

    void UpdateCreatableVariableType(string varType, string instanceId, bool isConnector) {
      if (!String.IsNullOrEmpty(varType) && varType[0] == 'L')
        UpdateActionDataList(PluginActions.SetVariable, "Create", __strArry_NoYes, instanceId, isConnector);
      else
        UpdateActionDataList(PluginActions.SetVariable, "Create", __strArry_NA, instanceId, isConnector);
    }

    void UpdateUnitsForVariableType(PluginActions action, string varType, string instanceId, bool isConnector)
    {
      if (String.IsNullOrEmpty(varType) || SimVarItem.RequiresUnitType(varType[0]))
        UpdateActionDataList(action, "Unit", _imports.UnitNames(), instanceId, isConnector);
      else if (varType[0] == 'L')
        UpdateActionDataList(action, "Unit", _imports.UnitNames().Prepend("number"), instanceId, isConnector);
      else
        UpdateActionDataList(action, "Unit", __strArry_NA, instanceId, isConnector);
    }

    // Events -------------------------

    // these are to cache the last user selections for HubHop presets; works since user can only edit one action at a time in TP
    string _lastSelectedVendor = string.Empty;
    string _lastSelectedAircraft = string.Empty;
    List<string> _lastLoadedSystems = new();

    // List of HubHop events vendor - aircraft
    void UpdateSimEventAircraft() {
      UpdateActionDataList(PluginActions.SetHubHopEvent, "VendorAircraft", _presets.VendorAircraft(HubHopType.AllInputs));
      UpdateActionDataList(PluginActions.SetHubHopEvent, "VendorAircraft", _presets.VendorAircraft(HubHopType.InputPotentiometer), null, true);
    }

    // List of HubHop event Systems per vendor - aircraft
    void UpdateHubHopEventSystems(string vendorAircraft, string instanceId, bool isConnector)
    {
      var va = HubHopPresetsCollection.SplitVendorAircraft(vendorAircraft);
      _lastSelectedVendor = va.Item1;
      _lastSelectedAircraft = va.Item2;
      HubHopType presetType = isConnector ? HubHopType.InputPotentiometer : HubHopType.AllInputs;
      var list = _presets.Systems(presetType, _lastSelectedAircraft, _lastSelectedVendor);
      if (list.Count > 0 && !list.SequenceEqual(_lastLoadedSystems)) {
        UpdateActionDataList(PluginActions.SetHubHopEvent, "System", list, instanceId, isConnector);
        _lastLoadedSystems = list;
      }
      // if only one system, also update the list of commands
      if (list.Count == 1)
        UpdateHubHopEventPresets(list[0], instanceId, isConnector);
    }

    // List of HubHop events for vendor/aircraft/system
    void UpdateHubHopEventPresets(string system, string instanceId, bool isConnector) {
      HubHopType presetType = isConnector ? HubHopType.InputPotentiometer : HubHopType.AllInputs;
      UpdateActionDataList(PluginActions.SetHubHopEvent, "EvtId", _presets.Names(presetType, _lastSelectedAircraft, system, _lastSelectedVendor), instanceId, isConnector);
    }

    // List of imported Sim Event IDs categories; for PluginActions.SetKnownSimEvent
    void UpdateSimEventCategories() {
      var cats = _imports.EventSystemCategories();
      UpdateActionDataList(PluginActions.SetKnownSimEvent, "SimCatName", cats);
      UpdateActionDataList(PluginActions.SetKnownSimEvent, "SimCatName", cats, null, true);
    }

    // List of imported SimEvents per imported category; for PluginActions.SetKnownSimEvent
    void UpdateKnownSimEventsForCategory(string categoryName, string instanceId, bool isConnector) {
      UpdateActionDataList(PluginActions.SetKnownSimEvent, "EvtId", _imports.EventNamesForSelector(categoryName), instanceId, isConnector);
    }

    // Misc. data update/clear  -------------------------------

    void SendAllSimVarStates()
    {
      foreach (SimVarItem simVar in _simVarCollection.RequestedVariables)
        _client.StateUpdate(simVar.TouchPortalStateId, simVar.FormattedValue);
    }

    // common handler for state updates
    void UpdateTpStateValue(string stateId, string value, Groups catId = Groups.Plugin) {
      _client.StateUpdate(PLUGIN_ID + "." + catId.ToString() + ".State." + stateId, value);
    }

    // update SimSystemEvent and (maybe) SimSystemEventData states in SimSystem group
    private void UpdateSimSystemEventState(EventIds eventId, object data = null) {
      if (SimSystemMapping.SimSystemEvent.ChoiceMappings.TryGetValue(eventId, out var eventName)) {
        UpdateTpStateValue("SimSystemEvent", eventName, Groups.SimSystem);
        _logger.LogTrace("Updated SimSystemEvent state value to '{eventName}'", eventName);
        if (data is string)
          UpdateTpStateValue("SimSystemEventData", data.ToString(), Groups.SimSystem);
      }
    }

    // update Connected state and trigger corresponding UpdateSimSystemEventState update
    private void UpdateSimConnectState() {
      EventIds evtId = EventIds.SimConnected;
      if (!_simConnectService.IsConnected)
        evtId = _simConnectionRequest.IsSet ? EventIds.SimConnecting : EventIds.SimDisconnected;
      UpdateTpStateValue("Connected", evtId switch { EventIds.SimConnected => "true", EventIds.SimDisconnected => "false", _ => "connecting" });
      UpdateSimSystemEventState(evtId);
    }

    private void ClearRepeatingActions() {
      foreach (var act in _repeatingActionTimers) {
        if (_repeatingActionTimers.TryRemove(act.Key, out var tim)) {
          tim.Dispose();
        }
      }
    }

    void UpdateRelatedConnectors(string varName, double value)
    {
      if (_connectorTracker.GetInstancesForStateId(varName) is var list && list != null) {
        foreach (var instance in list) {
          //_logger.LogDebug("{connId} - {shortId} - {isDn}", instance.connectorId, instance.shortId, instance.IsStillDown);
          if (string.IsNullOrEmpty(instance.shortId) || instance.fbRangeMin == instance.fbRangeMax || instance.IsStillDown)
            continue;
          int iValue = RangeValueToPercent(value, instance.fbRangeMin, instance.fbRangeMax);
          _logger.LogTrace("Sending connector update to {shortId} value {value}", instance.shortId, iValue);
          _client.ConnectorUpdateShort(instance.shortId, iValue);
        }
      }
    }

    void UpdateAllRelatedConnectors()
    {
      foreach (SimVarItem simVar in _simVarCollection)
        UpdateRelatedConnectors(simVar.Id, simVar);
      // don't forget the plugin internal values too
      UpdateRelatedConnectors(Settings.ActionRepeatInterval.SettingID, Settings.ActionRepeatInterval.RealValue);
    }

    #endregion Data Updaters

    #region Plugin Action/Event Handlers    /////////////////////////////////////

    void ConnectSimConnect() {
      _simAutoConnectDisable.Reset();
      if (!_simConnectService.IsConnected)
        _simConnectionRequest.Set();
      UpdateSimConnectState();
    }

    void DisconnectSimConnect() {
      _simAutoConnectDisable.Set();
      bool wasSet = _simConnectionRequest.IsSet;
      _simConnectionRequest.Reset();
      if (_simConnectService.IsConnected)
        _simConnectService.Disconnect();
      else if (wasSet)
        _logger.LogInformation((int)EventIds.PluginInfo, "Connection attempts to Simulator were canceled.");
      UpdateSimConnectState();
    }

    void UpdateHubHopData()
    {
      _logger.LogInformation((int)EventIds.PluginInfo, "HubHop Data Update Requested...");
      _presets.UpdateIfNeededAsync(Settings.HubHopUpdateTimeout.IntValue).ConfigureAwait(false);
    }

    // Handles some basic actions like sim connection and repeat rate, with optional data value(s).
    bool ProcessPluginCommandAction(PluginActions actionId, ActionData data, int connValue)
    {
      switch (actionId) {
        case PluginActions.ToggleConnection:
          if (_simAutoConnectDisable.IsSet)
            ConnectSimConnect();
          else
            DisconnectSimConnect();
          break;

        case PluginActions.Connect:
          ConnectSimConnect();
          break;

        case PluginActions.Disconnect:
          DisconnectSimConnect();
          break;

        case PluginActions.ReloadStates:
          SetupSimVars();
          break;

        case PluginActions.ResendStates:
          SendAllSimVarStates();
          break;

        case PluginActions.UpdateConnectorValues:
          UpdateAllRelatedConnectors();
          break;

        case PluginActions.UpdateHubHopPresets:
          UpdateHubHopData();
          break;

        case PluginActions.UpdateLocalVarsList:
          return _simConnectService.RequestLookupList(WASimCommander.CLI.Enums.LookupItemType.LocalVariable);

        case PluginActions.ReRegisterLocalVars:
          _simConnectService.RetryRegisterLocalVars();
          break;

        case PluginActions.ActionRepeatIntervalInc:
        case PluginActions.ActionRepeatIntervalDec:
        case PluginActions.ActionRepeatIntervalSet: {
          double value;
          if (connValue < 0) {
            string errMsg = null;
            // preserve backwards compatibility with old actions which used indexed data IDs
            if (data == null || !(data.TryGetValue("Value", out var sVal) || data.TryGetValue("1", out sVal)) || !TryEvaluateValue(sVal, out value, out errMsg)) {
              if (string.IsNullOrEmpty(errMsg))
                  errMsg = "Required parameter 'Value' missing or invalid.";
              _logger.LogError("Error getting value for repeat rate: '{errMsg}'; From data: {data}", errMsg, ActionDataToKVPairString(data));
              break;
            }
          }
          else if (!ConvertSliderValueRange(connValue, data, out value)) {
            return false;
          }

          if (actionId == PluginActions.ActionRepeatIntervalInc)
            value = Settings.ActionRepeatInterval.RealValue + value;
          else if (actionId == PluginActions.ActionRepeatIntervalDec)
            value = Settings.ActionRepeatInterval.RealValue - value;
          value = Math.Clamp(value, Settings.ActionRepeatInterval.MinValue, Settings.ActionRepeatInterval.MaxValue);
          if (value != Settings.ActionRepeatInterval.RealValue) {
            Settings.ActionRepeatInterval.Value = (uint)value;
            UpdateTpStateValue(Settings.ActionRepeatInterval.SettingID, Settings.ActionRepeatInterval.StringValue);
            UpdateRelatedConnectors(Settings.ActionRepeatInterval.SettingID, value);
          }
          break;
        }
      }
      return true;
    }

    // Send a Key Event to the sim
    bool TransmitSimEvent(ActionEventType action, EventMappingRecord eventRecord, uint[] values)
    {
      if (eventRecord == null || values.Length < 5)
        return false;
      _logger.LogDebug("Sending Sim Event: action: {actionId}; category: {catId}; name: {name}; data: {data}", action.ActionId, action.CategoryId, eventRecord.EventName, values);
      if (_simConnectService.IsConnected)
        return _simConnectService.TransmitClientEvent(eventRecord, values);
      return false;
    }

    // Variable Setters
    //

    // PluginActions.SetSimulatorVar
    // PluginActions.SetSimVar  // deprecated
    bool SetSimVarValueFromActionData(PluginActions action, ActionData data, int connValue = -1)
    {
      if (!data.TryGetValue("VarName", out string varName) || string.IsNullOrWhiteSpace(varName)) {
        _logger.LogError("Variable Name is missing or empty for Set SimVar from data: {data}", ActionDataToKVPairString(data));
        return false;
      }

      // v1.3 no longer requires the SimVar to have been requested first.
      bool newVersion = action == PluginActions.SetSimulatorVar;
      string varId = null;
      if ((newVersion && !TryGetSimVarNameFromActionData(varName, out varId)) || (!newVersion && !TryGetSimVarIdFromActionData(varName, out varId))) {
        _logger.LogError("Could not parse Variable Name parameter for Set SimVar from data: {data}", ActionDataToKVPairString(data));
        return false;
      }

      string unitName;
      if (newVersion) {
        // v1.3 added Unit and Index parameters
        if (!data.TryGetValue("Unit", out unitName) || string.IsNullOrWhiteSpace(unitName)) {
          _logger.LogError("Unit type missing or empty for Set SimVar '{varName}' in data: {data}", varName, ActionDataToKVPairString(data));
          return false;
        }
        if (data.TryGetValue("VarIndex", out var sIndex) && uint.TryParse(sIndex, out uint varIndex) && varIndex > 0) {
          varId += $":{varIndex}";
        }
      }
      else if (_simVarCollection.TryGet(varId, out var tmpSimVar)) {
        varId = tmpSimVar.SimVarName;
        unitName = tmpSimVar.Unit;
      }
      else {
        _logger.LogError("Could not find definition for settable SimVar '{varName}'", varName);
        return false;
      }
      return SetNamedVariable(PluginActions.SetSimulatorVar, 'A', varId, unitName, data, connValue);
    }

    // PluginActions.SetLocalVar
    // PluginActions.SetVariable
    bool SetNamedVariableFromActionData(PluginActions actId, char varType, ActionData data, int connValue)
    {
      if (!data.TryGetValue("VarName", out string varName) || string.IsNullOrWhiteSpace(varName)) {
        _logger.LogError("Could not parse Name parameter for {actId} from data: {data}", actId, ActionDataToKVPairString(data));
        return false;
      }
      if ((!data.TryGetValue("Unit", out string unitName) || string.IsNullOrWhiteSpace(unitName)) && SimVarItem.RequiresUnitType(varType)) {
        _logger.LogError("Unit type is required in {actId} for SimVar {varName}", actId, varName);
        return false;
      }
      return SetNamedVariable(actId, varType, varName, unitName, data, connValue);
    }

    // Shared method to set value of any variable type. Uses WASM if available, falls back to SimConnect for A vars otherwise.
    bool SetNamedVariable(PluginActions actId, char varType, string varName, string unitName, ActionData data, int connValue)
    {
      string sVal = null;
      double dVal = double.NaN;
      bool createLvar = false;
      bool isStringType = unitName?.ToLower() == "string";

      if (!_simConnectService.CanSetVariableType(varType)) {
        _logger.LogError("Cannot set {varType} type variable {varName} without WASM expansion add-on.", varType, varName);
        return false;
      }

      // Validate action value
      if (connValue < 0) {
        if (!data.TryGetValue("Value", out sVal)) {
          _logger.LogError("Could not find Value parameter for {actId} from data: {data}", actId, ActionDataToKVPairString(data));
          return false;
        }
        if (!isStringType && !TryEvaluateValue(sVal, out dVal, out var errMsg)) {
          _logger.LogError("Could not set numeric value '{value}' for {varType} variable: '{varName}'; Error: {errMsg}", sVal, varType, varName, errMsg);
          return false;
        }
        if (varType == 'L' && data.TryGetValue("Create", out var sCreate) && new BooleanString(sCreate))
          createLvar = true;
      }
      else if (isStringType) {
        _logger.LogError("Cannot set String type variable with a Connector value for {varName}", varName);
        return false;
      }
      // Validate and convert slider value into range
      else if (!ConvertSliderValueRange(connValue, data, out dVal)) {
        return false;
      }

      // Still needed?
      //if (data.TryGetValue("RelAi", out var relAi) && new BooleanString(relAi) && !_simConnectService.ReleaseAIControl(simVar.Def))
      //  return false;

      // If WASM is available, take the shorter and better route.
      // One exception is for L vars which have a custom Unit specified... WASM can't handle that.
      if (_simConnectService.WasmAvailable && !(varType == 'L' && (string.IsNullOrWhiteSpace(unitName) || unitName == "number"))) {
        if (isStringType) {
          string calcCode = $"'{sVal}' (>{varType}:{varName})";
          _logger.LogDebug("Sending code for String type var: {calcCode}", calcCode);
          return _simConnectService.ExecuteCalculatorCode(calcCode);
        }

        _logger.LogDebug("Setting {type} variable {varName} to {val}", varType, varName, dVal);
        return _simConnectService.SetVariable(varType, varName, dVal, unitName ?? "", createLvar);
      }

      if (!_simVarCollection.TryGetBySimName(varName, out SimVarItem simVar)) {
        simVar = PluginConfig.CreateDynamicSimVarItem(varType, varName, Groups.None, unitName, 0);
        if (simVar == null) {
          _logger.LogError("Could not create a valid variable from name: {varName}", varName);
          return false;
        }
        simVar.DefinitionSource = SimVarDefinitionSource.Temporary;
        simVar.UpdatePeriod = UpdatePeriod.Never;
        _simVarCollection.Add(simVar);
        _logger.LogDebug("Added temporary SimVar {simVarId} for {simVarName}", simVar.Id, simVar.SimVarName);
      }
      if (simVar.IsStringType) {
        if (!simVar.SetValue(new StringVal(sVal))) {
          _logger.LogError("Could not set string value '{value}' for SimVar Id: '{varId}' Name: '{varName}'", sVal, simVar.Id, varName);
          return false;
        }
      }
      else if (!simVar.SetValue(dVal)) {
        _logger.LogError("Could not set numeric value '{dVal}' for SimVar: '{varId}' Name: '{varName}'", dVal, simVar.Id, varName);
        return false;
      }

      _logger.LogDebug("Sending {simVar} with value {value}", simVar.ToString(), simVar.Value);
      return _simConnectService.SetDataOnSimObject(simVar);
    }

    // Variable Requests
    //

    //case PluginActions.AddSimulatorVar:
    //case PluginActions.AddLocalVar:
    //case PluginActions.AddNamedVariable:
    //case PluginActions.AddCalculatedValue:
    //case PluginActions.AddKnownSimVar:  // deprecated
    //case PluginActions.AddCustomSimVar:  // deprecated
    bool AddSimVarFromActionData(PluginActions actId, ActionData data)
    {
      if (!data.TryGetValue("VarName", out var varName) || string.IsNullOrWhiteSpace(varName) ||
          !data.TryGetValue("CatId", out var sCatId)    || !Categories.TryGetCategoryId(sCatId, out Groups catId)
      ) {
        _logger.LogError("Could not parse required action parameters for {actId} from data: {data}", actId, ActionDataToKVPairString(data));
        return false;
      }

      uint index = 0;
      char varType = 'A';
      string sCalcCode = null;
      var resType = WASimCommander.CLI.Enums.CalcResultType.None;
      switch (actId) {
        case PluginActions.AddLocalVar:
          varType = 'L';
          break;
        case PluginActions.AddNamedVariable:
          if (!data.TryGetValue("VarType", out var sVarType)) {
            _logger.LogError("Could not get variable type parameter for {actId} from data: {data}", actId, ActionDataToKVPairString(data));
            return false;
          }
          varType = sVarType[0];
          break;
        case PluginActions.AddCalculatedValue:
          if (!data.TryGetValue("CalcCode", out sCalcCode) || string.IsNullOrWhiteSpace(sCalcCode)) {
            _logger.LogError("Could not get valid CalcCode parameter for {actId} from data: {data}", actId, ActionDataToKVPairString(data));
            return false;
          }
          if (!data.TryGetValue("ResType", out var sResType) || !Enum.TryParse(sResType, out resType)) {
            _logger.LogError("Could not parse ResultType parameter for {actId} from {data}", actId, ActionDataToKVPairString(data));
            return false;
          }
          varType = 'Q';
          break;
        // legacy for < v1.3; replaced with AddLocalVar
        case PluginActions.AddKnownSimVar:  // deprecated
          if (data.TryGetValue("SimCatName", out var simCatName) && simCatName.Length > 1 && simCatName[0..2] == "L:")
            varType = 'L';
          break;
      }

      if (!_simConnectService.CanRequestVariableType(varType)) {
        _logger.LogError("Cannot request {varType} type variable {varName} without WASM expansion add-on.", varType, varName);
        return false;
      }

      if (!data.TryGetValue("Unit", out string sUnit) && SimVarItem.RequiresUnitType(varType)) {
        _logger.LogError("Unit Name is missing or empty for {actId} from data: {data}", actId, ActionDataToKVPairString(data));
        return false;
      }
      if (data.TryGetValue("VarIndex", out var sIndex) && !string.IsNullOrEmpty(sIndex))
        _ = uint.TryParse(sIndex, out index);

      // create the SimVarItem from collected data
      SimVarItem simVar = PluginConfig.CreateDynamicSimVarItem(varType, varName, catId, sUnit, index, _simVarCollection);
      if (simVar == null) {
        _logger.LogError("Failed to add Variable Request due to empty resulting variable name from action data: {data}", ActionDataToKVPairString(data));
        return false;
      }
      // check for optional properties
      if (data.TryGetValue("Format", out var sFormat))
        simVar.StringFormat = sFormat.Trim();
      if (data.TryGetValue("DfltVal", out var sDefault))
        simVar.DefaultValue = sDefault.Trim();
      if (data.TryGetValue("UpdPer", out var sPeriod) && Enum.TryParse(sPeriod, out UpdatePeriod period))
        simVar.UpdatePeriod = period;
      if (data.TryGetValue("UpdInt", out var sInterval) && uint.TryParse(sInterval, out uint interval))
        simVar.UpdateInterval = interval;
      if (data.TryGetValue("Epsilon", out var sEpsilon) && float.TryParse(sEpsilon, out float epsilon))
        simVar.DeltaEpsilon = epsilon;

      // Calculator result type
      if (varType == 'Q') {
        simVar.SimVarName = sCalcCode;     // replace simvar name with calc code; the Name used in state names/etc isn't changed.
        simVar.CalcResultType = resType;   // this also sets the Unit type and hence the data type (number/integer/string)
      }

      if (!simVar.Validate(out var validationError)) {
        _logger.LogError("Variable Request validation error \"{error}\"; for {actId} from data: {data}.", validationError, actId, ActionDataToKVPairString(data));
        return false;
      }
      else if (!string.IsNullOrEmpty(validationError)) {
        _logger.LogWarning("Variable Request validation warning \"{error}\"; for {actId} from data: {data}.", validationError, actId, ActionDataToKVPairString(data));
      }

      if (AddSimVar(simVar) is byte ret && ret > 0)
        _logger.LogInformation((int)EventIds.PluginInfo, "{action} Variable Request: {simVar}", (ret == 1 ? "Added" : "Replaced"), simVar.ToDebugString());
      else
        _logger.LogError("Failed to add Variable Request, check previous log messages.\n\tAction data: {data};\n\tVariable Request: {simVar}", ActionDataToKVPairString(data), simVar.ToDebugString());
      return ret > 0;
    }

    bool RemoveSimVarByActionDataName(ActionData data)
    {
      if (!data.TryGetValue("VarName", out var varName) || !TryGetSimVarIdFromActionData(varName, out string varId)) {
        _logger.LogError("Could not find valid Variable ID in action data: {data}", ActionDataToKVPairString(data));
        return false;
      }
      if ((_simVarCollection.TryGet(varId, out var simVar) && RemoveSimVar(simVar)) is bool ret && ret)
        _logger.LogInformation((int)EventIds.PluginInfo, "Removed SimVar '{simVar}'", simVar.SimVarName);
      else
        _logger.LogError("Could not find definition for settable SimVar Id: '{varId}' from Name: '{varName}'", varId, varName);
      return ret;
    }

    bool RequestValueUpdateFromActionData(ActionData data)
    {
      if (!data.TryGetValue("VarName", out var varName) || !TryGetSimVarIdFromActionData(varName, out string varId)) {
        _logger.LogError("Could not find valid Variable ID in action data: {data}", ActionDataToKVPairString(data));
        return false;
      }
      if (_simVarCollection.TryGet(varId, out var simVar) is bool ret && ret && simVar.RegistrationStatus == SimVarRegistrationStatus.Registered)
        _simConnectService.RequestVariableValueUpdate(simVar);
      else
        _logger.LogWarning("Variable with ID '{varId}' not found or not registered yet.", varId);
      return ret;
    }

    // Dynamic sim Events
    //

    //case PluginActions.SetCustomSimEvent:
    //case PluginActions.SetKnownSimEvent:
    //case PluginActions.SetHubHopEvent:
    bool ProcessSimEventFromActionData(PluginActions actId, ActionData data, int connValue = -1)
    {
      if (!data.TryGetValue("EvtId", out var eventName)) {
        _logger.LogError("Could not find Event Name parameter for {actId} from data: {data}", actId, ActionDataToKVPairString(data));
        return false;
      }
      eventName = eventName.Trim();

      // Collect array of event parameter value(s) from the action/connector data.
      uint[] values = new uint[5];
      bool isConn = connValue > -1;
      // how many ValueN action data parameters to expect, including connector value
      int maxVals = actId == PluginActions.SetHubHopEvent ? 1 : isConn || actId == PluginActions.SetKnownSimEvent ? 3 : 5;
      int connValIdx = -1; // position of connector value in parameter value data array
      if (isConn) {
        if (data.TryGetValue("ConnValIdx", out string sValIdx))
          _ = int.TryParse(sValIdx.AsSpan(0, 1), out connValIdx);  // default on failure is 0
        else
          connValIdx = 0;
      }
      double dVal;  // temp
      int valN = 1;  // starting "ValueN" action data parameter name
      for (int i = 0; i < maxVals; ++i) {
        if (connValIdx == i) {
          if (!ConvertSliderValueRange(connValue, data, out dVal))
            return false;
        }
        else {
          // For legacy/BC reasons, the first value doesn't have a digit after the name, and is also the only one expected to exist.
          string dataName = valN == 1 ? "Value" : "Value" + valN.ToString();
          string sVal = data.GetValueOrDefault(dataName, null);
          if (string.IsNullOrWhiteSpace(sVal)) {
            if (i == 0 && sVal == null)
              _logger.LogWarning("Could not find {dataName} parameter for {actId} in data: {data}", dataName, actId, ActionDataToKVPairString(data));
            break;  // exit on first empty value
          }
          if (!ConvertStringValue(sVal, DataType.Text, int.MinValue, uint.MaxValue, out dVal)) {
            _logger.LogError("Error evaluating {dataName} parameter for {actId} with data: {data}", dataName, actId, ActionDataToKVPairString(data));
            return false;
          }
          ++valN;
        }
        values[i] = (uint)Math.Round(dVal);
      }

      if (actId == PluginActions.SetHubHopEvent) {
        if (!data.TryGetValue("VendorAircraft", out var sVa) || !data.TryGetValue("System", out var sSystem)) {
          _logger.LogError("Could not find required action parameters for {actId} from data: {data}", actId, ActionDataToKVPairString(data));
          return false;
        }
        HubHopPreset p = _presets.PresetByName(eventName, HubHopType.AllInputs, sVa, sSystem);
        if (p == null) {
          _logger.LogError("Could not find Preset for action data: {data}", ActionDataToKVPairString(data));
          return false;
        }
        // Handle the preset action; first try use WASM module to execute the code directly.
        // MAYBE: save as registered event? Should be handled in SimConnectService probably.
        if (_simConnectService.WasmAvailable) {
          // HubHop actions have "@" placeholder where a value would go
          string eventCode = p.PresetType == HubHopType.InputPotentiometer ? p.Code.Replace("@", ((int)values[0]).ToString(CultureInfo.InvariantCulture)) : p.Code;
          return _simConnectService.ExecuteCalculatorCode(eventCode);
        }

        // No WASM module... try to use a named MobiFlight event... I guess. Then fall through and treat it as any other custom named action event.
        eventName = "MobiFlight." + p.Label;
      }
      // Check for known/imported type, which may have special formatting applied to the name
      else if (actId == PluginActions.SetKnownSimEvent && !TryGetSimEventIdFromActionData(eventName, out eventName)) {
        _logger.LogError("Could not find Event ID from action data: {data}", ActionDataToKVPairString(data));
        return false;
      }

      string actionKey = $"{Groups.Plugin}.{eventName}";
      // dynamically add an action if it doesn't already exist
      if (!actionsDictionary.TryGetValue(actionKey, out ActionEventType ev)) {
        // Dynamically added events only have one action mapping, which will be returned with ActionEventType.GetEventMapping().
        ev = new ActionEventType(eventName);
        actionsDictionary[actionKey] = ev;
      }

      return TransmitSimEvent(ev, ev.GetEventMapping(), values);
    }

    // PluginActions.ExecCalcCode
    bool ProcessCalculatorEventFromActionData(PluginActions actId, ActionData data, int connValue = -1)
    {
      if (!_simConnectService.CanSetVariableType('Q')) {
        _logger.LogError("Cannot execute Calculator Code without WASM expansion add-on.");
        return false;
      }

      if (!data.TryGetValue("Code", out var calcCode) || string.IsNullOrWhiteSpace(calcCode)) {
        _logger.LogError("Calculator Code parameter missing or empty for {actId} from data: {data}", actId, ActionDataToKVPairString(data));
        return false;
      }
      double dVal = double.NaN;
      // Validate action value
      if (connValue < 0) {
        if (data.TryGetValue("Value", out var sValue) && !TryEvaluateValue(sValue, out dVal, out var errMsg)) {
          _logger.LogError("Data conversion for string '{sValue}' failed for {actId} on with Error: {errMsg}.", sValue, actId, errMsg);
          return false;
        }
      }
      // Validate and convert slider value into range
      else if (!ConvertSliderValueRange(connValue, data, out dVal)) {
        return false;
      }
      if (!double.IsNaN(dVal))
        calcCode = calcCode.Replace("@", dVal.ToString("F6"));
      return _simConnectService.ExecuteCalculatorCode(calcCode);
    }


    // Main TP Action/Connector event handlers
    //

    bool ProcessEvent(DataContainerEventBase actionEvent)
    {
      var idParts = actionEvent.Id.Split('.', StringSplitOptions.RemoveEmptyEntries);
      if (idParts.Length < 4) {
        _logger.LogWarning("Malformed action ID '{actId}'", actionEvent.Id);
        return false;
      }
      string sActId = idParts[^3] + '.' + idParts[^1];
      if (!actionsDictionary.TryGetValue(sActId, out ActionEventType action)) {
        if (sActId != "Plugin.SetConnectorValue")  // exception for no-op event (hacky)
          _logger.LogWarning("Unknown action ID '{actId}'", sActId);
        return false;
      }

      int connValue = (actionEvent.GetType() == typeof(ConnectorChangeEvent)) ? ((ConnectorChangeEvent)actionEvent).Value : -1;
      if (action.InternalEvent)
        return ProcessInternalEvent(action, actionEvent.Data, connValue);

      return ProcessSimEvent(action, actionEvent.Data, connValue);
    }

    bool ProcessInternalEvent(ActionEventType action, ActionData data, int connValue = -1)
    {
      PluginActions pluginEventId = (PluginActions)action.Id;
      _logger.LogDebug("Firing Internal Event - action: {actId}; enum: {pluginEventId}; connVal {connVal}; data: {data}", action.ActionId, pluginEventId, connValue, ActionDataToKVPairString(data));
      switch (pluginEventId) {
        case PluginActions.Connection:
        case PluginActions.ActionRepeatInterval: {
          // preserve backwards compatibility with old actions which used indexed data IDs
          if ((data.TryGetValue("Action", out var actId) || data.TryGetValue("0", out actId)) && action.TryGetEventMapping(actId, out var evRecord))
            return ProcessPluginCommandAction((PluginActions)evRecord.EventId, data, connValue);
          _logger.LogError("Could not parse required action parameters for {actId} from data: {data}", action.ActionId, ActionDataToKVPairString(data));
          return false;
        }

        case PluginActions.SetSimulatorVar:
        case PluginActions.SetSimVar:  // deprecated
          return SetSimVarValueFromActionData(pluginEventId, data, connValue);

        case PluginActions.SetLocalVar:
          return SetNamedVariableFromActionData(pluginEventId, 'L', data, connValue);

        case PluginActions.SetVariable: {
          if (data.TryGetValue("VarType", out var varType))
            return SetNamedVariableFromActionData(PluginActions.SetVariable, varType[0], data, connValue);
          _logger.LogError("Could not parse Variable Type parameter for SetVariable from data: {data}", ActionDataToKVPairString(data));
          return false;
        }

        case PluginActions.SetCustomSimEvent:
        case PluginActions.SetKnownSimEvent:
        case PluginActions.SetHubHopEvent:
          return ProcessSimEventFromActionData(pluginEventId, data, connValue);

        case PluginActions.ExecCalcCode:
          return ProcessCalculatorEventFromActionData(pluginEventId, data, connValue);

        case PluginActions.AddSimulatorVar:
        case PluginActions.AddLocalVar:
        case PluginActions.AddNamedVariable:
        case PluginActions.AddCalculatedValue:
        case PluginActions.AddKnownSimVar:  // deprecated
        case PluginActions.AddCustomSimVar:  // deprecated
          return AddSimVarFromActionData(pluginEventId, data);

        case PluginActions.UpdateVarValue:
          return RequestValueUpdateFromActionData(data);

        case PluginActions.RemoveSimVar:
          return RemoveSimVarByActionDataName(data);

        case PluginActions.ClearSimVars: {
          if (data.TryGetValue("VarsSet", out var varSet) && action.TryGetEventMapping(varSet, out var evRecord)) {
            RemoveSimVars((PluginActions)evRecord.EventId == PluginActions.ClearCustomSimVars);
            return true;
          }
          return false;
        }

        case PluginActions.LoadSimVars: {
          if (data.TryGetValue("VarsFile", out var filepath) && !string.IsNullOrWhiteSpace(filepath)) {
            LoadCustomSimVarsFromFile(filepath.Trim());
            return true;
          }
          return false;
        }

        case PluginActions.SaveSimVars: {
          if (data.TryGetValue("VarsFile", out var filepath) && !string.IsNullOrWhiteSpace(filepath)) {
            // Revisit in future TP version.
            //if (data.TryGetValue("VarsDir", out var filedir) && !string.IsNullOrWhiteSpace(filedir)) {
            //  try {
            //    if (String.IsNullOrWhiteSpace(System.IO.Path.GetDirectoryName(filepath)))
            //      filepath = System.IO.Path.Join(filedir, filepath);
            //  }
            //  catch { /* ignore */ }
            //}
            bool customOnly = !action.TryGetEventMapping(data.GetValueOrDefault("VarsSet", "Custom"), out var evRecord) || (PluginActions)evRecord.EventId == PluginActions.SaveCustomSimVars;
            SaveSimVarsToFile(filepath.Trim(), customOnly);
            return true;
          }
          return false;
        }

        case PluginActions.SetConnectorValue:
          // ignore, has no action.
          return false;
        default:
          _logger.LogWarning("Unknown action ID '{eventId}'", pluginEventId.ToString());
          return false;
      }
    }

    // This method handles all "static" actions and connectors for Sim events where the event name to be triggered is mapped
    // to a plugin action and, possibly, its data values. Some of these event actions pass parameter value(s) as well, and of course
    // connectors only make sense with events which have a value. All these actions are defined in the "Objects" files, everything besides the Plugin category.
    bool ProcessSimEvent(ActionEventType action, ActionData data, int connValue)
    {
      if (!action.TryGetEventMapping(data.Values, out EventMappingRecord eventRecord)) {
        _logger.LogError("Could not find mapped Sim Event for action '{actId}' with data {data}.", action.ActionId, ActionDataToKVPairString(data));
        return false;
      }

      // Event values array to pass to the simulator (up to 5).
      uint[] values = new uint[5];
      int nextValIndex = 0;
      int recValsLen = eventRecord.Values?.Length ?? 0;
      // An event mapping record may have some static value(s) embedded. Prepend them if any user-provided values should come after them.
      if (recValsLen > 0 && action.OutputValueIndex < 0) {
        Array.Copy(eventRecord.Values, 0, destinationArray: values, destinationIndex: nextValIndex, length: Math.Min(recValsLen, 5));
        if (recValsLen >= 5) {
          // In the unlikely event that there are already 5 values, send them now and return.
          return TransmitSimEvent(action, eventRecord, values);
        }
        nextValIndex = recValsLen;
      }
      if (connValue < 0) {
        // Value from action data (only single value, at least for now).
        if (action.ValueIndex > -1 && action.ValueIndex < data.Values.Count) {
          if (!ConvertStringValue(data.Values.ElementAt(action.ValueIndex), action.ValueType, action.MinValue, action.MaxValue, out var dVal)) {
            _logger.LogError("Data conversion failed for action '{actId}' on sim event '{evName}'.", action.ActionId, eventRecord.EventName);
            return false;
          }
          values[nextValIndex++] = (uint)Math.Round(dVal, 0);
        }
      }
      else if (ConvertSliderValueRange(connValue, data, out var dVal)) {
        // Value from connector (only 1).
        values[nextValIndex++] = (uint)Math.Round(dVal, 0);
      }
      else {
        // no valid value
        _logger.LogError("Connector value conversion failed for action '{actId}' on sim event '{evName}'.", action.ActionId, eventRecord.EventName);
        return false;
      }
      // Append any static values
      if (recValsLen > 0 && action.OutputValueIndex > -1 && nextValIndex < 5)
        Array.Copy(eventRecord.Values, 0, destinationArray: values, destinationIndex: nextValIndex, length: Math.Min(eventRecord.Values.Length, 5 - nextValIndex));

      return TransmitSimEvent(action, eventRecord, values);
    }

    // Handles an array of `Setting` types sent from TP/API. This could come from either the
    // initial `OnInfoEvent` message, or the dedicated `OnSettingsEvent` message.
    void ProcessPluginSettings(IReadOnlyCollection<Setting> settings)
    {
      if (settings == null)
        return;
      // loop over incoming new settings
      foreach (var s in settings) {
        if (!pluginSettingsDictionary.TryGetValue(s.Name, out PluginSetting setting) || setting.Equals(s.Value))
          continue;
        _logger.LogDebug("{setName}; current: {oldVal}; new: {newVal};", setting.Name, setting.StringValue, s.Value);
        setting.SetValueFromString(s.Value);
        if (!string.IsNullOrWhiteSpace(setting.TouchPortalStateId))
          UpdateTpStateValue(setting.TouchPortalStateId, setting.StringValue);
      }

      // Check if number formatting rules have changed
      if (Settings.UseInvariantCulture.BoolValue != (_cultureInfo == CultureInfo.InvariantCulture)) {
        _cultureInfo = Settings.UseInvariantCulture.BoolValue ? CultureInfo.InvariantCulture : new CultureInfo(_defaultCultureId);
        _logger.LogInformation("Switching default formatting culture to {name} - {engName}", _cultureInfo.Name, _cultureInfo.EnglishName);
        CultureInfo.DefaultThreadCurrentCulture = _cultureInfo;
        Thread.CurrentThread.CurrentCulture = _cultureInfo;
        SendAllSimVarStates();
      }

      // change tracking for config files
      string[] p = new[] { _pluginConfig.UserConfigFolder, _pluginConfig.UserStateFiles };
      _pluginConfig.UserConfigFolder = Settings.UserConfigFilesPath.StringValue;  // will (re-)set to default if needed.
      _pluginConfig.UserStateFiles = Settings.UserStateFiles.StringValue;         // will (re-)set to default if needed.
      // compare with actual current config values (not Settings) because they may not have changed even if settings string did
      // states dict will be empty on initial startup
      if (_simVarCollection.IsEmpty || p[0] != _pluginConfig.UserConfigFolder || p[1] != _pluginConfig.UserStateFiles)
        SetupSimVars();
      // send state update with current user config files folder location
      UpdateTpStateValue("UserConfigFilesPath", _pluginConfig.UserConfigFolder);

      // Initiate or cancel Sim auto-connection as per setting and current connection status.
      if (Settings.ConnectSimOnStartup.BoolValue == _simAutoConnectDisable.IsSet) {
        if (Settings.ConnectSimOnStartup.BoolValue) {
          ConnectSimConnect();
        }
        else if (!_simConnectService.IsConnected) {
          _simAutoConnectDisable.Set();
          _simConnectionRequest.Reset();
          UpdateSimConnectState();
        }
      }
    }

    #endregion Plugin Action/Event Handlers

    #region TouchPortalSDK Events       ///////////////////////////////

    public void OnInfoEvent(InfoEvent message) {
      var runtimeVer = string.Format("{0:X}", VersionInfo.GetProductVersionNumber());
      _logger?.LogInformation(new EventId(1, "Connected"),
        "Touch Portal Connected with: TP v{tpV}, SDK v{sdkV}, {pluginId} entry.tp v{plugV}, {plugName} running v{prodV} ({runV})",
        message.TpVersionString, message.SdkVersion, PluginId, message.PluginVersion, VersionInfo.AssemblyName, VersionInfo.GetProductVersionString(), runtimeVer
      );

      ProcessPluginSettings(message.Settings);

      // Init HubHop database
      if (_presets.OpenDataFile() && Settings.UpdateHubHopOnStartup.BoolValue)
        UpdateHubHopData();

      // convert the entry.tp version back to the actual decimal value
      if (!uint.TryParse($"{message.PluginVersion}", NumberStyles.HexNumber, null, out uint tpVer))
        tpVer = VersionInfo.GetProductVersionNumber();
      // update version states
      UpdateTpStateValue("RunningVersion", runtimeVer);
      UpdateTpStateValue("EntryVersion", $"{tpVer:X}");
      // set a state for TP config home path (workaround for no env. var access in TP)
      UpdateTpStateValue("TouchPortalConfigPath", System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TouchPortal"));
      // update current repeat rate state
      UpdateTpStateValue(Settings.ActionRepeatInterval.SettingID, Settings.ActionRepeatInterval.StringValue);
      // update action data lists
      UpdateCategoryLists();
      UpdateUnitsLists();
      UpdateSimVarCategories();
      UpdateSimEventAircraft();
      UpdateSimEventCategories();
      UpdateDeprecatedOptions();
      // schedule update of connector values once TP has reported any of our Connectors with shortConnectorIdNotification events
      Task.Delay(800).ContinueWith(t => UpdateAllRelatedConnectors());
    }

    public void OnClosedEvent(string message)
    {
      _logger?.LogInformation("TouchPortal Disconnected with message: {message}", message);
      if (!_quitting)
        _hostApplicationLifetime.StopApplication();
    }

    public void OnSettingsEvent(SettingsEvent message) {
      ProcessPluginSettings(message.Values);
    }

    public void OnActionEvent(ActionEvent message)
    {
      // Actions used in TP "On Hold" events will send "down" and "up" events, in that order (usually).
      // "On Pressed" actions will have a "action" event (Tap) when it is _released_.
      switch (message.GetPressState())
      {
        case TouchPortalSDK.Messages.Models.Enums.Press.Down: {
          // "On Hold" activated ("down" event). Try to add this action to the repeating/scheduled actions queue, unless it already exists.
          var timer = new Timer(Settings.ActionRepeatInterval.IntValue);
          timer.Elapsed += delegate { ProcessEvent(message); };
          if (_repeatingActionTimers.TryAdd(message.ActionId, timer)) {
            if (ProcessEvent(message))  // fire the event first and see if it event succeeds
              timer.Start();
          }
          else {
            timer.Dispose();
          }
          break;
        }

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

    public void OnConnecterChangeEvent(ConnectorChangeEvent message)
    {
      // check connector status up/down (sort of like actions but harder to track); UpdateConnectorValue() returns false if connector is no longer moving.
      if (_connectorTracker.UpdateConnectorValue(message))
        ProcessEvent(message);
    }

    // This method handles tracking the "feedback" states that are used to set a slider position based on
    // a state's value. It examines the "long connector ID" to parse they data key=value pairs to determine
    // which state is to be used for feedback (if any), and the input range of that state's value.
    public void OnShortConnectorIdNotificationEvent(ShortConnectorIdNotificationEvent message)
    {
      // we only use the last part of the connectorId which is meaningful
      // note that ShortConnectorIdNotificationEvent.ActualConnectorId invokes the connectorId parser.
      string connectorId = message.ActualConnectorId?.Split('.').Last();
      if (!Enum.TryParse(connectorId, out PluginActions actionId))
        actionId = PluginActions.None;

      var data = message.Data;
      string varName;
      // The SetSimVar connector is special because we can get the feedback variable name from the one being set.
      if (actionId == PluginActions.SetSimulatorVar) {
        // New version of action
        if (!data.TryGetValue("FB", out var fbEnable) || !(new BooleanString(fbEnable)))
          return;
        if (data.TryGetValue("VarName", out var fbVarName) && TryGetSimVarNameFromActionData(fbVarName, out fbVarName)) {
          uint varIndex = 0;
          if (data.TryGetValue("VarIndex", out var sIndex) && uint.TryParse(sIndex, out varIndex) && varIndex > 0) {
            fbVarName += $":{varIndex}";
          }
          if (!_simVarCollection.TryGetBySimName(fbVarName, out var simVar)) {

            if ((!data.TryGetValue("Unit", out var unitName) || string.IsNullOrWhiteSpace(unitName))) {
              _logger.LogWarning("Unit type missing or empty for '{fbVarName}' in Connector '{cId}'", fbVarName, message.ConnectorId);
              return;
            }

            simVar = PluginConfig.CreateDynamicSimVarItem('A', fbVarName, Groups.None, unitName, varIndex);
            if (simVar == null) {
              _logger.LogWarning("Could not create valid variable from name '{varName}' in Connector '{cId}'", fbVarName, message.ConnectorId);
              return;
            }
            simVar.DefinitionSource = SimVarDefinitionSource.Temporary;
            _simVarCollection.Add(simVar);
            _logger.LogInformation("Added temporary SimVar {simVarId} ({simVarName}) for Connector Feedback.", simVar.Id, simVar.SimVarName);
          }
          varName = simVar.Id;
        }
        else {
          _logger.LogDebug("Not tracking connector w/out valid FB variable ({fbVarName}) in connector ID {cId}", fbVarName, message.ConnectorId);
          return;
        }
      }
      else if (actionId == PluginActions.SetSimVar) {  // deprecated
        // Old version of action
        if (!data.TryGetValue("VarName", out var fbVarName) || !TryGetSimVarIdFromActionData(fbVarName, out varName)) {
          _logger.LogWarning("Could not find Variable Name for Set Sim Var in connector ID '{cId}'", message.ConnectorId);
          return;
        }
      }
      // every other connector action type
      else {
        if (!data.TryGetValue("FbVarName", out var fbVarName) || string.IsNullOrWhiteSpace(fbVarName) || fbVarName == "null") {
          _logger.LogDebug("Not tracking connector w/out feedback variable in connector ID '{cId}'", message.ConnectorId);
          return;
        }
        if (!TryGetSimVarIdFromActionData(fbVarName, out varName)) {
          _logger.LogWarning("Could not parse feedback variable name for connector ID '{cId}'", message.ConnectorId);
          return;
        }
      }

      if (!data.TryGetValue("FbRangeMin", out var sRangeMin) || !float.TryParse(sRangeMin, out var rangeMin)) {
        if (!data.TryGetValue("RangeMin", out sRangeMin) || !float.TryParse(sRangeMin, out rangeMin)) {
          _logger.LogWarning("Could not parse minimum range value name for connector ID '{cId}'", message.ConnectorId);
          return;
        }
      }
      if (!data.TryGetValue("FbRangeMax", out var sRangeMax) || !float.TryParse(sRangeMax, out var rangeMax)) {
        if (!data.TryGetValue("RangeMax", out sRangeMax) || !float.TryParse(sRangeMax, out rangeMax)) {
          _logger.LogWarning("Could not parse maximum range value name for connector ID '{cId}'", message.ConnectorId);
          return;
        }
      }
      _logger.LogDebug("Got ShortId {shortId} for ActualId {longId}: State ID {stateId} range {rngMin}-{rngMax} for connector ID '{cId}'", message.ShortId, message.ActualConnectorId, varName, rangeMin, rangeMax, message.ConnectorId);
      _connectorTracker.SaveConnectorInstance(message, varName, rangeMin, rangeMax);
      //var trk = _connectorTracker.GetDataForEvent(message);
      //_logger.LogDebug("Connector: {trkConId} - {shortId} - {isDn} - {next} - {mapId}", trk.connectorId, trk.shortId, trk.isDown, trk.nextTimeout, trk.mappingId);
    }

    // Handle dynamic list updates based on value selected in another list, indicated by the message.ListId which is an action data ID.
    public void OnListChangedEvent(ListChangeEvent message)
    {
      // There's also a ListChangeEvent.ActionId property, but since our data IDs already contain the action ID, this is moot.
      if (string.IsNullOrWhiteSpace(message.InstanceId) || string.IsNullOrWhiteSpace(message.Value) || string.IsNullOrWhiteSpace(message.ListId))
        return;
      // get the last 5 parts of the data ID, which is in the form of: <Category>.<Type>.<ActionId>.Data.<DataId>
      var listParts = message.ListId.Split('.');
      if (listParts.Length < 5)
        return;
      listParts = listParts[^5..];
      string sCategoryId = listParts[0];
      bool isConnector = listParts[1] == "Conn";
      string sActId = listParts[2];
      string dataId = listParts[4];
      if (!Enum.TryParse(sCategoryId, true, out Groups catId))
        catId = Groups.None;
      if (isConnector && dataId == "FbCatId") {
        UpdateVariablesListPerCategory(catId, sActId, message.Value, message.InstanceId, isConnector);
        return;
      }
      if (!Enum.TryParse(sActId, true, out PluginActions actId))
        actId = PluginActions.None;
      switch (actId) {
        case PluginActions.AddSimulatorVar:
        case PluginActions.AddKnownSimVar:  // deprecated
          if (dataId == "SimCatName")
            UpdateKnownSimVars(actId, message.Value, message.InstanceId);
          else if (dataId == "VarName")
            UpdateUnitsListForKnownSimVar(actId, message.Value, message.InstanceId, false);
          break;
        case PluginActions.AddNamedVariable:
          if (dataId == "VarType")
            UpdateUnitsForVariableType(actId, message.Value, message.InstanceId, isConnector);
          break;
        case PluginActions.SetKnownSimEvent:
          if (dataId == "SimCatName")
            UpdateKnownSimEventsForCategory(message.Value, message.InstanceId, isConnector);
          break;
        case PluginActions.SetHubHopEvent:
          if (dataId == "VendorAircraft")
            UpdateHubHopEventSystems(message.Value, message.InstanceId, isConnector);
          else if (dataId == "System")
            UpdateHubHopEventPresets(message.Value, message.InstanceId, isConnector);
          break;
        case PluginActions.SetSimulatorVar:
          if (dataId == "CatId")
            UpdateRegisteredSimVarsList(message.Value, message.InstanceId, isConnector);
          else if (dataId == "VarName")
            UpdateUnitsListForKnownSimVar(actId, message.Value, message.InstanceId, isConnector);
          break;
        case PluginActions.SetVariable:
          if (dataId == "VarType") {
            UpdateUnitsForVariableType(actId, message.Value, message.InstanceId, isConnector);
            UpdateCreatableVariableType(message.Value, message.InstanceId, isConnector);
          }
          break;
        case PluginActions.UpdateVarValue:
        case PluginActions.RemoveSimVar:
          if (dataId == "CatId")
            UpdateVariablesListPerCategory(Groups.Plugin, sActId, message.Value, message.InstanceId, isConnector);
          break;

        default:
          break;
      }
    }

    public void OnNotificationOptionClickedEvent(NotificationOptionClickedEvent message) {
      // not implemented yet
    }

    public void OnBroadcastEvent(BroadcastEvent message)
    {
      if (message.Event == "pageChange")
        UpdateTpStateValue("CurrentTouchPortalPage", message.PageName.Replace(".tml", string.Empty, true, CultureInfo.InvariantCulture));
    }

    public void OnUnhandledEvent(string jsonMessage) {
      _logger?.LogDebug("Unhandled message: {jsonMessage}", jsonMessage);
    }

    #endregion TouchPortalSDK Events

    #region Utilities       ///////////////////////////////

    // used in numeric evaluator... because none of the math eval libs can parse hex notation
    static readonly System.Text.RegularExpressions.Regex _hexNumberRegex = new (@"\b0x([0-9A-Fa-f]{1,8})\b", System.Text.RegularExpressions.RegexOptions.None, TimeSpan.FromSeconds(0.05));

    private bool TryEvaluateValue(string strValue, out double value, out string errMsg) {
      value = double.NaN;
      errMsg = null;
      try {
        strValue = _hexNumberRegex.Replace(strValue, m => (Convert.ToUInt32(m.Groups[1].ToString(), 16).ToString()));
        if (_cultureInfo != CultureInfo.InvariantCulture && _cultureInfo.NumberFormat.NumberDecimalSeparator == ",")
          strValue = strValue.Replace(".", string.Empty).Replace(',', '.');  // hack for non-invariant cultures
        value = Convert.ToDouble(_expressionEvaluator.Compute(strValue, null));
      }
      catch (Exception e) {
        errMsg = e.Message;
        _logger.LogDebug(e, "Eval exception with '{strValue}'", strValue);
        return false;
      }
      return true;
    }

    bool ConvertStringValue(string value, DataType type, double minVal, double maxVal, out double result)
    {
      result = double.NaN;
      switch (type) {
        case DataType.Number:
        case DataType.Text:
          if (!TryEvaluateValue(value, out result, out var errMsg)) {
            _logger.LogError("Data conversion for string '{value}' failed with error: {errMsg}.", value, errMsg);
            return false;
          }
          break;
        case DataType.Switch:
          result = new BooleanString(value);
          break;
      }
      if (double.IsNaN(result))
        return false;
      if (!double.IsNaN(minVal))
        result = Math.Max(result, minVal);
      if (!double.IsNaN(maxVal))
        result = Math.Min(result, maxVal);
      return true;
    }

    static bool TryGetSimVarIdFromActionData(string varName, out string varId) {
      if (varName[^1] == ']' && (varName.IndexOf('[') is var brIdx && brIdx > -1)) {
        varId = varName[++brIdx..^1];
        return true;
      }
      varId = string.Empty;
      return false;
    }

    static bool TryGetSimVarNameFromActionData(string selector, out string varId)
    {
      varId = selector?.Split('\n', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[0].Replace(":N", string.Empty) ?? string.Empty;
      return !string.IsNullOrWhiteSpace(varId);
    }

    static bool TryGetSimEventIdFromActionData(string selector, out string eventId)
    {
      // old version of SimEvent names used '-' as separator for description, v1.3+ uses newline.
      eventId = selector?.Split(new[] { '\n', '-' }, 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[0] ?? string.Empty;
      return !string.IsNullOrWhiteSpace(eventId);
    }

    static string ActionDataToKVPairString(ActionData data) =>
      '{' + string.Join(", ", data.Select(d => $"{d.Key}={d.Value}")) + '}';

    bool ConvertSliderValueRange(int value, ActionData data, out double dValue)
    {
      // Validate connector value range
      if (!data.TryGetValue("RangeMin", out var sRangeMin) || !float.TryParse(sRangeMin, out var rangeMin) ||
          !data.TryGetValue("RangeMax", out var sRangeMax) || !float.TryParse(sRangeMax, out var rangeMax)) {
        _logger.LogError("Could not parse required Range connector parameters from data: {data}", ActionDataToKVPairString(data));
        dValue = 0.0;
        return false;
      }
      dValue = PercentOfRange(value, rangeMin, rangeMax);
      return true;
    }

    static double PercentOfRange(int value, float rangeMin, float rangeMax)
    {
      return ((rangeMax - rangeMin) / 100.0f * value) + rangeMin;
    }

    static int RangeValueToPercent(double value, float rangeMin, float rangeMax)
    {
      float dlta = rangeMax - rangeMin;
      float scale = dlta == 0.0f ? 100.0f : 100.0f / dlta;
      return Math.Clamp((int)Math.Round((value - rangeMin) * scale), 0, 100);
    }

    /*
    struct ActionValue
    {
      public string strValue;
      public uint uValue;
      public DataType type;
      public double minVal;
      public double maxVal;
      public ActionValue(string sVal = null, DataType t = DataType.Text, double min = double.NaN, double max = double.NaN, uint defaultVal = 0) {
        strValue = sVal; type = t; minVal = min; maxVal = max; uValue = defaultVal;
      }
    };

    uint[] ConvertStringValues(ActionValue[] values, out bool ok)
    {
      ok = true;
      uint[] d = new uint[5];
      for (int i = 0, e = Math.Min(values.Length, 5); i < e; ++i) {
        ActionValue v = values[i];
        if (string.IsNullOrWhiteSpace(v.strValue))
          break;  // exit on first empty value
        if (ConvertStringValue(v.strValue, v.type, v.minVal, v.maxVal, out double dVal))
          d[i] = (uint)Math.Round(dVal);
        else
          ok = false;
      }
      return d;
    }
    */

    #endregion Utilities

  }

}
