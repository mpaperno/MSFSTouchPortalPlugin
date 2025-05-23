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
using SIMCONNECT_EXCEPTION = Microsoft.FlightSimulator.SimConnect.SIMCONNECT_EXCEPTION;

namespace MSFSTouchPortalPlugin.Services
{
  /// <inheritdoc cref="IPluginService" />
  internal class PluginService : IPluginService, ITouchPortalEventHandler
  {
    public string PluginId => PluginConfig.PLUGIN_ID;  // for ITouchPortalEventHandler

    const int SIM_RECONNECT_DELAY_SEC = 30;   // SimConnect connection attempts delay on failure

    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly ILogger<PluginService> _logger;
    private readonly ITouchPortalClient _client;
    private readonly ISimConnectService _simConnectService;
    private readonly IReflectionService _reflectionService;
    private readonly PluginConfig _pluginConfig;
    private readonly SimVarCollection _simVarCollection;
    private readonly HubHopPresetsCollection _presets = null;
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
    private readonly ConcurrentDictionary<string, Timer> _repeatingActionTimers = new();  // storage for temporary repeating (held) action timers, index by action ID
    private readonly ConcurrentQueue<string> _logMessages = new();  // stores the last log messages for the LogMessages state value, used in PluginLogger callback
    private static readonly System.Data.DataTable _expressionEvaluator = new();  // used to evaluate basic math in action data
    private CultureInfo _cultureInfo = CultureInfo.CurrentCulture;
    private readonly string _defaultCultureId = CultureInfo.CurrentCulture.Name;
#if WASIM
    private IReadOnlyDictionary<int, string> _localVariablesList = null;
#endif
#if !FSX
    SimPauseStates _simPauseState = SimPauseStates.OFF;  // Tracks pause state reported from last Pause_EX1 sim event.
#endif

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

#if !FSX
      _presets = new();
      Configuration.HubHop.Common.Logger = _logger;
      _presets.OnDataUpdateEvent += HubHop_OnDataUpdate;
      _presets.OnDataErrorEvent += Collections_OnDataError;
#endif

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

      _logger.LogInformation("======= {pluginId} Starting =======", PluginConfig.PLUGIN_ID);

      // register ctrl-c exit handler
      Console.CancelKeyPress += (_, _) => {
        _logger.LogInformation("Quitting due to keyboard interrupt.");
        _hostApplicationLifetime.StopApplication();
      };

      if (!Initialize()) {
        _hostApplicationLifetime.StopApplication();
        return Task.CompletedTask;
      }
      Task.Delay(500).ContinueWith(t => UpdateTpStateValue("RunningState", "started"));

      //StartPluginEventsTask();  // useful for testing repeating actions w/out a sim connection
      return SimConnectionMonitor();
    }

    /// <summary>
    /// Stops the plugin service
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    public Task StopAsync(CancellationToken cancellationToken) {
      if (_quitting) {
        _logger.LogWarning((int)EventIds.Ignore, "Already Quitting!");
        return Task.CompletedTask;
      }
      // Shut down
      _quitting = true;
      _logger.LogDebug("Shutting down...");
      UpdateTpStateValue("RunningState", "stopped");
      StopPluginEventsTask();
      DisconnectSimConnect();
      if (_pluginConfig.SaveSettings())
        _logger.LogInformation("Saved Settings to file {file}", PluginConfig.SettingsConfigFile);
      if (_client.IsConnected) {
        try { _client.Close(); }
        catch (Exception ex) {
          _logger.LogWarning((int)EventIds.Ignore, ex, "TouchPortalClient exception");
        }
      }
      _simConnectService?.Dispose();
      _simConnectionRequest?.Dispose();
      _simAutoConnectDisable?.Dispose();
      _presets?.Dispose();

      _logger.LogInformation("======= {pluginId} Stopped =======", PluginConfig.PLUGIN_ID);
      return Task.CompletedTask;
    }

    bool Initialize() {
      // set up data which may be sent to TP upon initial connection
      _pluginConfig.Init();
      actionsDictionary = _reflectionService.GetActionEvents();
      pluginSettingsDictionary = _reflectionService.GetSettings();
      _reflectionService.InitEvents();

      if (!_client.Connect()) {
        _logger.LogCritical("Failed to connect to Touch Portal! Quitting.");
        return false;
      }

      // Set up Sim connection service and events
      _simConnectService.Init();
      _simConnectService.OnDataUpdateEvent += SimConnectEvent_OnDataUpdateEvent;
      _simConnectService.OnConnect += SimConnectEvent_OnConnect;
      _simConnectService.OnDisconnect += SimConnectEvent_OnDisconnect;
      //_simConnectService.OnException += SimConnectEvent_OnException;
      _simConnectService.OnSimVarError += SimConnectEvent_OnSimVarError;
      _simConnectService.OnEventReceived += SimConnectEvent_OnEventReceived;
      _simConnectService.OnInputEventsUpdated += SimConnect_OnInputEventsListUpdated;
#if WASIM
      _simConnectService.OnWasmStatusChanged += SimConnectEvent_OnWasmStatusChanged;
      _simConnectService.OnLVarsListUpdated += SimConnect_OnLVarsListUpdated;
#endif

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

    // start checking timer events
    void StartPluginEventsTask() {
      if (_simTasksCTS != null)
          return;
      _simTasksCTS = new CancellationTokenSource();
      _simTasksCancelToken = _simTasksCTS.Token;
      _pluginEventsTask = Task.Run(PluginEventsTask);
    }

    void StopPluginEventsTask() {
      if (_simTasksCTS == null)
          return;
      _simTasksCTS.Cancel();
      if (_pluginEventsTask != null) {
        if (_pluginEventsTask.Status < TaskStatus.RanToCompletion && !_pluginEventsTask.Wait(5000))
          _logger.LogWarning((int)EventIds.Ignore, "PluginEventsTask timed out while stopping.");
        try { _pluginEventsTask.Dispose(); }
        catch (Exception ex) {
          _logger.LogWarning((int)EventIds.Ignore, ex, "PluginEventsTask shutdown exception");
        }
      }
      _pluginEventsTask = null;
      _simTasksCTS.Dispose();
      _simTasksCTS = null;

      ClearRepeatingActions();
    }

    /// <summary>
    /// Task for running various timed SimConnect events on one thread.
    /// This is primarily used for data polling and repeating (held) actions.
    /// </summary>
    async Task PluginEventsTask()
    {
      _logger.LogDebug("PluginEventsTask task started.");
      using PeriodicTimer timer = new(TimeSpan.FromMilliseconds(25));
      try {
        while (!_simTasksCancelToken.IsCancellationRequested) {
          foreach (Timer tim in (IReadOnlyCollection<Timer>)_repeatingActionTimers.Values)
            tim.Tick();
          CheckPendingRequests();
          await timer.WaitForNextTickAsync(_simTasksCancelToken).ConfigureAwait(false);
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
        if (simVar.UpdateRequired)
          simVar.SetPending(_simConnectService.RequestVariableValueUpdate(simVar));
        else if (!simVar.NeedsScheduledRequest)
          _simVarCollection.RemoveFromPolled(simVar);
      }
    }

    // Cancels any currently repeating actions, eg. when disconnected from sim.
    void ClearRepeatingActions() {
      foreach (var act in _repeatingActionTimers) {
        if (_repeatingActionTimers.TryRemove(act.Key, out var tim)) {
          tim.Dispose();
        }
      }
    }

    // Used when disconnecting from SimConnect to clear mapped Key Event IDs which will not be valid in future sessions.
    void ResetMappedEventIds() {
      foreach (var ev in actionsDictionary.Values)
        ev.ClearMappedEventIds(autoAssignedOnly: true);
    }

    // PluginLogger callback
    private void OnPluginLoggerMessage(string message, LogLevel logLevel = LogLevel.Information, EventId eventId = default) {
      //_logger.LogDebug($"Got message from our logger! {logLevel}: {message}");
      var evId = (EventIds)eventId.Id;
      if (evId == EventIds.Ignore)
        return;
      while (_logMessages.Count >= Settings.LogMessagesStateMaxLines.IntValue && _logMessages.Count > 0)
        _logMessages.TryDequeue(out _);
      if (Settings.LogMessagesStateMaxLines.IntValue > 0)
        _logMessages.Enqueue(message);
      if (_client.IsConnected) {
        if (evId == EventIds.None && logLevel > LogLevel.Information)
          evId = EventIds.PluginError;
        if (evId != EventIds.None) {
          UpdateSimSystemEventState(evId, message);
          TriggerTpEvent(PluginMapping.MessageEvent, [
            [ "Type",    evId.ToString() ],
            [ "Message", message ]
          ]);
        }
        UpdateTpStateValue("LogMessages", string.Join('\n', _logMessages.ToArray()));
      }
    }

#if !FSX
    // HubHopPresetsCollection data update callback
    void HubHop_OnDataUpdate(bool updated)
    {
      if (updated)
        UpdateSimEventAircraft();
      string logMsg = updated ? "HubHop Data Updated" : "No HubHop Updates Detected";
      _logger.LogInformation((int)EventIds.PluginInfo, "{logMsg}; Latest entry date: {time:u}", logMsg, _presets.LatestUpdateTime);
    }
#endif

    void Collections_OnDataError(LogLevel severity, string message) {
      _logger.Log(severity, (int)EventIds.PluginError, "{message}", message);
    }

    #endregion Startup, Shutdown and Processing Tasks

    #region SimConnect Events   /////////////////////////////////////

    void SimConnectEvent_OnConnect(SimulatorInfo simInfo)
    {
      _logger.LogDebug("SimConnectService connected, starting events task...");

      _simConnectionRequest.Reset();
      StartPluginEventsTask();
      UpdateSimConnectState();
      UpdateTpStateValue("SimVersion", simInfo.AppVersionMaj.ToString(), Groups.SimSystem);

      if (DocImportsCollection.SetNewSimulatorVersion(simInfo.AppVersionMaj)) {
        UpdateSimVarCategories();
        UpdateSimEventCategories();
        _logger.LogDebug("Reloaded doc imports for simulator version {SimVersionTag}", DocImportsCollection.SimulatorVersion);
      }
    }

    void SimConnectEvent_OnWasmStatusChanged(WasmModuleStatus status)
    {
      string state = status switch {
        WasmModuleStatus.NotFound  => "undetected",
        WasmModuleStatus.Found     => "disconnected",
        WasmModuleStatus.Connected => "connected",
        _                          => "unknown",
      };
      UpdateTpStateValue("WasmStatus", state);
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
        _logger.LogTrace("Got value '{data}' for Sim Var {var} and set to '{value}' (formatted: '{format}')", data, simVar.ToDebugString(), simVar.Value, simVar.FormattedValue);
        if (simVar.CategoryId != Groups.None)  // assumes anything for an actual category will have a State ID
          _client.StateUpdate(simVar.TouchPortalStateId, simVar.FormattedValue);
        // Check for any Connectors (sliders) which use this state as feedback mechanism.
        if (!simVar.IsStringType)
          UpdateRelatedConnectors(simVar.Id, (double)simVar);
      }
    }

    private void SimConnectEvent_OnDisconnect() {
      _logger.LogInformation("SimConnect Disconnected");

      StopPluginEventsTask();
      ResetMappedEventIds();

      if (!_simAutoConnectDisable.IsSet && !_quitting)
        _simConnectionRequest.Set();  // re-enable connection attempts

      UpdateSimConnectState();
    }

    private void SimConnectEvent_OnEventReceived(EventIds eventId, Groups categoryId, object data) {
      if (eventId <= EventIds.SimEventNone || eventId >= EventIds.SimEventLast)
        return;
      switch (eventId) {
        case EventIds.View:
          eventId = (uint)data == SimConnectService.VIEW_EVENT_DATA_COCKPIT_3D ? EventIds.ViewCockpit : EventIds.ViewExternal;
          break;
#if !FSX
        case EventIds.Pause_EX1: {
          SimPauseStates lastState = _simPauseState;
          _simPauseState = (SimPauseStates)Convert.ToByte(data);
          if (lastState == _simPauseState)
            return;

          UpdateTpStateValue("SimPauseState", _simPauseState.ToString("G"), Groups.SimSystem);
          TriggerTpEvent(SimSystemMapping.SimPauseEvent, [
              [ "NewState",     _simPauseState.ToString("d") ],
              [ "NewStateStr",  _simPauseState.ToString("G") ],
              [ "PrevState",    lastState.ToString("d")      ],
              [ "PrevStateStr", lastState.ToString("G")      ],
          ]);

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
          break;
        }
#endif
      }

      UpdateSimSystemEventState(eventId, data);
    }

#nullable enable
    private void SimConnectEvent_OnSimVarError(Definition def, SimVarErrorType errType, object? data = null)
    {
      if (_simVarCollection.TryGet(def, out var simVar)) {
        string? reason = null;
        switch (errType) {
          case SimVarErrorType.SimVersion:
            reason = $"required Sim version does not match (SimVar.SimVersion: '{simVar.SimVersion}', Sim: '{data}')";
            break;

          case SimVarErrorType.VarType:
          case SimVarErrorType.Registration:
            reason = data?.ToString() ?? "Could not register variable due to Simulator issue, see previous messages for details";
            break;

          case SimVarErrorType.SimConnectError:
            if ((data as RequestTrackingData is var rtd) && rtd != null) {
              reason = rtd.eException switch {
                SIMCONNECT_EXCEPTION.NAME_UNRECOGNIZED => $"SimConnect says variable '{rtd.aArguments[1]}' is unknown",
                _ => $"SimConnect error {rtd.eException}",
              };
            }
            break;
        }

        reason ??= "Unknown reason";
        bool remove = simVar.DefinitionSource != SimVarDefinitionSource.Temporary;
        LogLevel level = simVar.RegistrationStatus == SimVarRegistrationStatus.Registered ? LogLevel.Warning : LogLevel.Information;

        _logger.Log(level, (int)EventIds.SimError,
          "{action} variable request '{id}' due to: {reason}.", (remove ? "Removing" : "Suspending"), simVar.Id, reason);

        simVar.RegistrationStatus = SimVarRegistrationStatus.Error;
        if (remove)
          RemoveSimVar(simVar);
      }
    }
#nullable restore

    void SimConnect_OnInputEventsListUpdated()
    {
      // Send TP selector choice update.
      UpdateSimInputEvents();
      // Try re-registering any InputEvent requests that may have failed previously due to non-existence.
      if (_simConnectService.SimInputEvents.Any())
        _simConnectService.RetryRegisterVarRequests('B');
    }

#if WASIM
    void SimConnect_OnLVarsListUpdated(IReadOnlyDictionary<int, string> list) {
      _localVariablesList = list;
      UpdateLocalVarsLists();
    }
#endif

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

      UpdateLoadedStateFilesList();
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

      _logger.LogTrace("{action} Request: {simVar}", (ret == 1 ? "Added" : ret == 2 ? "Replaced" : "Error"), simVar.ToDebugString());
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
    void RemoveSimVars(PluginActions action, string spec)
    {
      var list = action switch {
        PluginActions.ClearCustomSimVars => _simVarCollection.CustomVariables,
        PluginActions.ClearAllSimVars => _simVarCollection.RequestedVariables,
        PluginActions.ClearSimVarsFromFile => _simVarCollection.VariablesFromFile(spec),
        PluginActions.ClearSimVarsOfType => _simVarCollection.VariablesOfType(spec[0]),
        _ => Array.Empty<SimVarItem>()
      };
      if (list.Any()) {
        foreach (SimVarItem simVar in list)
          RemoveSimVar(simVar);
        _logger.LogInformation((int)EventIds.PluginInfo, "Removed {count} variable requests for action {action} with data (if any) '{spec}'.", list.Count(), action.ToString(), spec);
      }
      else {
        _logger.LogInformation("Did not find any variables to remove for {action} with data (if any) '{spec}'.", action.ToString(), spec);
      }
      // Adjust files list even if no variables were removed (perhaps they were all removed individually or by type).
      switch (action) {
        case PluginActions.ClearAllSimVars:
          _pluginConfig.LoadedStateConfigFiles.Clear();
          break;

        case PluginActions.ClearSimVarsFromFile:
          _pluginConfig.LoadedStateConfigFiles.Remove(spec);
          break;

        case PluginActions.ClearCustomSimVars:
          foreach (var file in _pluginConfig.LoadedStateConfigFiles.ToArray())
            if (file != PluginConfig.StatesConfigFile)
              _pluginConfig.LoadedStateConfigFiles.Remove(file);
          break;

        default:
          return;
      }
      UpdateLoadedStateFilesList();
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
        UpdateLoadedStateFilesList();
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

    [Flags]
    enum DataListUpdtType { None = 0, Action = 0x01, Connector = 0x02, All = Action | Connector }

    // common handler for other action list updaters
    void UpdateActionDataList(PluginActions actionId, string dataId, IEnumerable<string> data, string instanceId, bool isConnector) {
      UpdateActionDataList(actionId, dataId, data, instanceId, (isConnector ? DataListUpdtType.Connector : DataListUpdtType.Action));
    }

    void UpdateActionDataList(PluginActions actionId, string dataId, IEnumerable<string> data, string instanceId = null, DataListUpdtType type = DataListUpdtType.Action) {
      UpdateActionDataList(Groups.Plugin, actionId.ToString(), dataId, data, instanceId, type);
    }

    void UpdateActionDataList(PluginActions actionId, string dataId, IEnumerable<string> data, DataListUpdtType type) {
      UpdateActionDataList(Groups.Plugin, actionId.ToString(), dataId, data, null, type);
    }

    void UpdateActionDataList(Groups categoryId, string actionId, string dataId, IEnumerable<string> data, string instanceId = null, DataListUpdtType type = DataListUpdtType.Action)
    {
      if (data == null)
        return;
      var dataArry = data.ToArray();
      if (type.HasFlag(DataListUpdtType.Action))
        _client.ChoiceUpdate($"{PluginConfig.PLUGIN_ID}.{categoryId}.Action.{actionId}.Data.{dataId}", dataArry, instanceId);
      if (type.HasFlag(DataListUpdtType.Connector))
        _client.ChoiceUpdate($"{PluginConfig.PLUGIN_ID}.{categoryId}.Conn.{actionId}.Data.{dataId}", dataArry, instanceId);
    }

    // Variables, setters and requesters  ----------------------------

#if WASIM
    // List of Local vars for all instances
    void UpdateLocalVarsLists()
    {
      UpdateLocalVarsList(PluginActions.SetLocalVar, null, DataListUpdtType.All);
      UpdateLocalVarsList(PluginActions.AddLocalVar, null, DataListUpdtType.Action);
    }
#endif

    // Unit lists
    void UpdateUnitsLists()
    {
      var units = _imports.UnitNames();
      UpdateActionDataList(PluginActions.AddNamedVariable, "Unit", units);
      UpdateActionDataList(PluginActions.SetVariable, "Unit", units, DataListUpdtType.All);
      // L var unit type is usually "number", except for a few special cases, so put that on top.
      var lunits = units.Prepend("number");
      UpdateActionDataList(PluginActions.AddLocalVar, "Unit", lunits);
      UpdateActionDataList(PluginActions.SetLocalVar, "Unit", lunits, DataListUpdtType.All);
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
          UpdateActionDataList(cat.Id, conn.Id, "FbCatId", Categories.ListUsable, null, DataListUpdtType.Connector);
    }

    // List of imported SimVariable categories
    void UpdateSimVarCategories() {
      // settable
      UpdateActionDataList(PluginActions.SetSimulatorVar, "CatId", _imports.SimVarSystemCategories(true), DataListUpdtType.All);
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
      UpdateActionDataList(PluginActions.SetSimVar, "CatId", list, DataListUpdtType.All);
      UpdateActionDataList(PluginActions.SetSimVar, "VarName", list, DataListUpdtType.All);
    }


    // Action instance-specific updaters
    //

    // List of all current variables per category
    private void UpdateVariablesListPerCategory(Groups categoryId, string actId, string category, string instanceId, bool isConnector) {
      if (Categories.TryGetCategoryId(category, out Groups catId))
        UpdateActionDataList(categoryId, actId, isConnector ? "FbVarName" : "VarName", _simVarCollection.GetSimVarSelectorList(catId), instanceId, isConnector ? DataListUpdtType.Connector : DataListUpdtType.Action);
    }

#if WASIM
    // List of Local vars per action instance
    private void UpdateLocalVarsList(PluginActions action, string instanceId, DataListUpdtType type) {
      if (_localVariablesList == null || !_localVariablesList.Any())
        UpdateActionDataList(action, "VarName", new[] { "<local variables list not available>" }, instanceId, type);
      else if (Settings.SortLVarsAlpha.BoolValue)
        UpdateActionDataList(action, "VarName", _localVariablesList.Values.OrderBy(m => m), instanceId, type);
      else
        UpdateActionDataList(action, "VarName", _localVariablesList.Values, instanceId, type);
    }
#endif

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
#if WASIM
        if (categoryName.StartsWith("L:")) {
          UpdateLocalVarsList(action, instanceId, DataListUpdtType.Action);
          return;
        }
#endif
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
      else if (varType[0] == 'B')
        UpdateActionDataList(action, "Unit", new [] { "number", "string" }, instanceId, isConnector);
      else
        UpdateActionDataList(action, "Unit", __strArry_NA, instanceId, isConnector);
    }

    void UpdateClearSimVarsSpecSelector(string varsSet, string instanceId)
    {
      if (varsSet.EndsWith("File"))
        UpdateActionDataList(PluginActions.ClearSimVars, "VarsSpec", _pluginConfig.LoadedStateConfigFiles, instanceId);
      else if (varsSet.EndsWith("Type"))
        UpdateActionDataList(PluginActions.ClearSimVars, "VarsSpec", SimVarItem.ReadableVariableTypes, instanceId);
      else
        UpdateActionDataList(PluginActions.ClearSimVars, "VarsSpec", __strArry_NA, instanceId);
    }

    // HubHop Event action updaters -------------------------
#if !FSX
    // these are to cache the last user selections for HubHop presets; works since user can only edit one action at a time in TP
    string _lastSelectedVendor = string.Empty;
    string _lastSelectedAircraft = string.Empty;
    List<string> _lastLoadedSystems = new();

    // List of HubHop events vendor - aircraft
    void UpdateSimEventAircraft() {
      UpdateActionDataList(PluginActions.SetHubHopEvent, "VendorAircraft", _presets.VendorAircraft(HubHopType.AllInputs));
      UpdateActionDataList(PluginActions.SetHubHopEvent, "VendorAircraft", _presets.VendorAircraft(HubHopType.InputPotentiometer), DataListUpdtType.Connector);
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
        UpdateHubHopEventPresets(list[0], instanceId, isConnector, new ActionData{ {"VendorAircraft", vendorAircraft} });
    }

    // List of HubHop events for vendor/aircraft/system
    void UpdateHubHopEventPresets(string system, string instanceId, bool isConnector, ActionData values = null) {
      HubHopType presetType = isConnector ? HubHopType.InputPotentiometer : HubHopType.AllInputs;
      string vendor;
      string craft;
      if (values != null && values.TryGetValue("VendorAircraft", out string va)) {
        var vaa = HubHopPresetsCollection.SplitVendorAircraft(va);
        vendor = vaa.Item1;
        craft = vaa.Item2;
        //_logger.LogDebug("Using airplane/vendor from action data {vendor} {craft}", vendor, craft);
      }
      else {
        vendor = _lastSelectedVendor;
        craft = _lastSelectedAircraft;
      }
      UpdateActionDataList(PluginActions.SetHubHopEvent, "EvtId", _presets.EventsForSelector(presetType, craft, system, vendor), instanceId, isConnector);
    }

    void UpdateHubHopData()
    {
      _logger.LogInformation((int)EventIds.PluginInfo, "HubHop Data Update Requested...");
      _presets.UpdateIfNeededAsync(Settings.HubHopUpdateTimeout.IntValue).ConfigureAwait(false);
    }
#endif

    // Sim Event action updaters -------------------------

    // List of imported Sim Event IDs categories; for PluginActions.SetKnownSimEvent
    void UpdateSimEventCategories() {
      UpdateActionDataList(PluginActions.SetKnownSimEvent, "SimCatName", _imports.EventSystemCategories(), DataListUpdtType.All);
    }

    // List of imported SimEvents per imported category; for PluginActions.SetKnownSimEvent
    void UpdateKnownSimEventsForCategory(string categoryName, string instanceId, bool isConnector) {
      UpdateActionDataList(PluginActions.SetKnownSimEvent, "EvtId", _imports.EventNamesForSelector(categoryName), instanceId, isConnector);
    }

    // Input Events action updaters -------------------------

    // List of input events
    void UpdateSimInputEvents()
    {
      IEnumerable<string> list;
      if (_simConnectService.SimInputEvents.Any())
        list = _simConnectService.SimInputEvents.Keys.OrderBy(n => n);
      else
        list = new[] { "<no input events loaded>" };

      UpdateActionDataList(PluginActions.SetInputEvent, "VarName", list, null, DataListUpdtType.All);
      UpdateActionDataList(PluginActions.AddInputEventVar, "VarName", list, null, DataListUpdtType.Action);
    }

    // Misc. data update/clear  -------------------------------

    void SendAllSimVarStates()
    {
      foreach (SimVarItem simVar in _simVarCollection.RequestedVariables)
        _client.StateUpdate(simVar.TouchPortalStateId, simVar.FormattedValue);
    }

    // common handler for state updates
    void UpdateTpStateValue(string stateId, string value, Groups catId = Groups.Plugin) {
      _client.StateUpdate($"{PluginId}.{catId}.State.{stateId}", value);
    }

    // trigger a TPv4 event with local states feature
    void TriggerTpEvent(TouchPortalEvent tpEv, params string[][] states) {
      //_logger.LogDebug("Triggering Event '{eventId}' with states: {states}", tpEv.TpEventId, tpEv.States.ValueStates(states).ToString());
      _client.TriggerEvent(tpEv.TpEventId, tpEv.States.ValueStates(states) );
    }

    // update SimSystemEvent and (maybe) SimSystemEventData states in SimSystem group
    private void UpdateSimSystemEventState(EventIds eventId, object data = null)
    {
      if (!SimSystemMapping.SimSystemEvent.ChoiceMappings.TryGetValue(eventId, out var eventName))
        return;

      if (data is string)
        UpdateTpStateValue("SimSystemEventData", data.ToString(), Groups.SimSystem);
      UpdateTpStateValue("SimSystemEvent", eventName, Groups.SimSystem);

      if (eventId > EventIds.SimEventNone && eventId < EventIds.SimEventLast && data is string) {
        TriggerTpEvent(SimSystemMapping.FilenameEvent, [
          [ "Type",     eventId.ToString() ],
          [ "Filename", data.ToString() ]
        ]);
      }
      _logger.LogTrace("Updated SimSystemEvent state value to '{eventName}'", eventName);
    }

    // update Connected state and trigger corresponding UpdateSimSystemEventState update
    private void UpdateSimConnectState() {
      EventIds evtId = _simConnectService.IsConnected ? EventIds.SimConnected : _simConnectionRequest.IsSet ? EventIds.SimConnecting : EventIds.SimDisconnected;
      string evtStr = evtId switch { EventIds.SimConnected => "true", EventIds.SimDisconnected => "false", _ => "connecting" };
      UpdateTpStateValue("Connected", evtStr);
      UpdateSimSystemEventState(evtId);
      TriggerTpEvent(PluginMapping.SimConnectionEvent, [[ "Status", evtId.ToString().ToLower().Replace("sim", "") ]]);
    }

    void UpdateLoadedStateFilesList() {
      UpdateTpStateValue("LoadedStateConfigFiles", string.Join(',', _pluginConfig.LoadedStateConfigFiles));
    }

    void UpdateRelatedConnectors(string varName, double value)
    {
      if (_connectorTracker.GetInstancesForStateId(varName) is var list && list != null) {
        foreach (var instance in list) {
          //_logger.LogDebug("{conntId} - {isDn} - v: '{value}'", instance.connectorId, instance.IsStillDown, value);
          if (instance.IsStillDown || string.IsNullOrEmpty(instance.shortId) || instance.fbRangeMin == instance.fbRangeMax)
            continue;
          int iValue = RangeValueToPercent(value, instance.fbRangeMin, instance.fbRangeMax);
          if (iValue != instance.lastUpdateValue) {
            instance.lastUpdateValue = iValue;
            _logger.LogTrace("Sending connector update to {shortId} value {value}", instance.shortId, iValue);
            _client.ConnectorUpdateShort(instance.shortId, iValue);
          }
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
      bool wasConn = _simConnectService.IsConnected;
      bool wasSet = _simConnectionRequest.IsSet;
      _simAutoConnectDisable.Set();
      _simConnectionRequest.Reset();
      if (wasConn)
        _simConnectService.Disconnect();
      else if (wasSet)
        _logger.LogInformation((int)EventIds.PluginInfo, "Connection attempts to Simulator were canceled.");
      if (wasConn || wasSet)
        UpdateSimConnectState();
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
#if !FSX
        case PluginActions.UpdateHubHopPresets:
          UpdateHubHopData();
          break;

        case PluginActions.UpdateLocalVarsList:
          return _simConnectService.RequestLocalVariablesList();

        case PluginActions.UpdateInputEventsList:
          return _simConnectService.UpdateInputEventsList();

        case PluginActions.ReRegisterInputEventVars:
          _simConnectService.RetryRegisterVarRequests('B');
          break;
#endif
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
              return false;
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

    bool TryGetVarName(PluginActions actId, ActionData data, out string varName)
    {
      if (TryGetSomeActionData(data, "VarName", out varName))
        return true;
      _logger.LogError("Could not parse Variable Name parameter for {actId} from data: {data}", actId, ActionDataToKVPairString(data));
      return false;
    }

    // PluginActions.SetSimulatorVar
    // PluginActions.SetSimVar  // deprecated
    bool SetSimVarValueFromActionData(PluginActions action, ActionData data, int connValue = -1)
    {
      if (!_simConnectService.IsConnected || !TryGetVarName(action, data, out string varName))
        return false;

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
        if (!TryGetSomeActionData(data, "Unit", out unitName)) {
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
      if (!_simConnectService.IsConnected || !TryGetVarName(actId, data, out string varName))
        return false;
      if (!TryGetSomeActionData(data, "Unit", out string unitName) && SimVarItem.RequiresUnitType(varType)) {
        _logger.LogError("Unit type is required in {actId} for SimVar {varName}", actId, varName);
        return false;
      }
      return SetNamedVariable(actId, varType, varName, unitName, data, connValue);
    }

#if !FSX
    // PluginActions.SetInputEvent
    bool SetInputEventFromActionData(PluginActions actId, ActionData data, int connValue)
    {
      if (!_simConnectService.IsConnected || !TryGetVarName(actId, data, out string varName))
        return false;
      if (!_simConnectService.SimInputEvents.TryGetValue(varName, out var simEvent)) {
        _logger.LogError("Cannot find Simulator Input Event named {varName} for {action}.", varName, actId);
        return false;
      }
      var unitName = simEvent.Type switch {
        Microsoft.FlightSimulator.SimConnect.SIMCONNECT_INPUT_EVENT_TYPE.STRING => "string",
        _ => "number"
      };
      return SetNamedVariable(actId, 'B', varName, unitName, data, connValue);
    }
#endif

    // Shared method to set value of any variable type. Uses WASM if available, falls back to SimConnect for A vars otherwise.
    bool SetNamedVariable(PluginActions actId, char varType, string varName, string unitName, ActionData data, int connValue)
    {
      string sVal = null;
      double dVal = double.NaN;
      bool createLvar = false;
      bool isStringType = unitName?.ToLower() == "string";

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

      return _simConnectService.SetVariable(varType, varName, isStringType ? sVal : dVal, unitName ?? "", createLvar);
    }

    // Variable Requests
    //

    // PluginActions.AddSimulatorVar:
    // PluginActions.AddLocalVar:
    // PluginActions.AddInputEventVar:
    // PluginActions.AddNamedVariable:
    // PluginActions.AddCalculatedValue:
    // PluginActions.AddKnownSimVar:  // deprecated
    // PluginActions.AddCustomSimVar:  // deprecated
    bool AddSimVarFromActionData(PluginActions actId, ActionData data)
    {
      if (!TryGetSomeActionData(data, "VarName", out var varName) ||
          !TryGetSomeActionData(data, "CatId", out var sCatId) ||
          !Categories.TryGetCategoryId(sCatId, out Groups catId)
      ) {
        _logger.LogError("Could not parse required action parameters for {actId} from data: {data}", actId, ActionDataToKVPairString(data));
        return false;
      }

      uint index = 0;
      char varType = 'A';
#if WASIM
      string sCalcCode = null;
      var resType = WASimCommander.CLI.Enums.CalcResultType.None;
#endif
      switch (actId) {
        case PluginActions.AddLocalVar:
          varType = 'L';
          break;
        case PluginActions.AddInputEventVar:
          varType = 'B';
          break;
        case PluginActions.AddNamedVariable:
          if (!TryGetSomeActionData(data, "VarType", out var sVarType)) {
            _logger.LogError("Could not get variable type parameter for {actId} from data: {data}", actId, ActionDataToKVPairString(data));
            return false;
          }
          varType = sVarType[0];
          break;
#if WASIM
        case PluginActions.AddCalculatedValue:
          if (!TryGetSomeActionData(data, "CalcCode", out sCalcCode)) {
            _logger.LogError("Could not get valid CalcCode parameter for {actId} from data: {data}", actId, ActionDataToKVPairString(data));
            return false;
          }
          if (!data.TryGetValue("ResType", out var sResType) || !Enum.TryParse(sResType, out resType)) {
            _logger.LogError("Could not parse ResultType parameter for {actId} from {data}", actId, ActionDataToKVPairString(data));
            return false;
          }
          varType = 'Q';
          break;
#endif
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

#if WASIM
      // Calculator result type
      if (varType == 'Q') {
        simVar.SimVarName = sCalcCode;     // replace simvar name with calc code; the Name used in state names/etc isn't changed.
        simVar.CalcResultType = resType;   // this also sets the Unit type and hence the data type (number/integer/string)
      }
#endif

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
          if (string.IsNullOrWhiteSpace(sVal))
            break;  // exit on first empty value
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
        HubHopPreset p = _presets?.PresetBySelectorName(eventName, sVa, sSystem);
        if (p == null) {
          _logger.LogError("Could not find Preset for action data: {data}", ActionDataToKVPairString(data));
          return false;
        }
        // Handle the preset action; first try use WASM module to execute the code directly.
        // MAYBE: save as registered event? Should be handled in SimConnectService probably.
        if (_simConnectService.CanSetVariableType('Q')) {
          // HubHop actions have "@" placeholder where a value would go
          string eventCode = p.PresetType == HubHopType.InputPotentiometer ? p.Code.Replace("@", ((int)values[0]).ToString(CultureInfo.InvariantCulture)) : p.Code;
          //return _simConnectService.ExecuteCalculatorCode(eventCode);
          return _simConnectService.SetVariable('Q', "HubHopEvent_Code", eventCode);
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
      if (!_simConnectService.IsConnected)
        return false;

      if (!TryGetSomeActionData(data, "Code", out var calcCode)) {
        _logger.LogError("Calculator Code parameter missing or empty for {actId} from data: {data}", actId, ActionDataToKVPairString(data));
        return false;
      }

      // Calc code may contain an "@" placeholder to be replaced with a connector or action value.
      if (calcCode.Contains('@', StringComparison.InvariantCulture)) {
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
      }
      //return _simConnectService.ExecuteCalculatorCode(calcCode);
      return _simConnectService.SetVariable('Q', "Calculator_Code", calcCode);
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
          if (TryGetSomeActionData(data, "VarType", out var varType))
            return SetNamedVariableFromActionData(PluginActions.SetVariable, varType[0], data, connValue);
          _logger.LogError("Could not parse Variable Type parameter for SetVariable from data: {data}", ActionDataToKVPairString(data));
          return false;
        }

#if !FSX
        case PluginActions.SetInputEvent:
          return SetInputEventFromActionData(PluginActions.SetInputEvent, data, connValue);
#endif

        case PluginActions.SetCustomSimEvent:
        case PluginActions.SetKnownSimEvent:
        case PluginActions.SetHubHopEvent:
          return ProcessSimEventFromActionData(pluginEventId, data, connValue);

        case PluginActions.ExecCalcCode:
          return ProcessCalculatorEventFromActionData(pluginEventId, data, connValue);

        case PluginActions.AddSimulatorVar:
        case PluginActions.AddLocalVar:
        case PluginActions.AddInputEventVar:
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
            data.TryGetValue("VarsSpec", out var spec);
            _logger.LogDebug("Firing Internal Event - action: {actId}; enum: {pluginEventId}; data: {varSet} {spec}", action.ActionId, (PluginActions)evRecord.EventId, varSet, spec);
            RemoveSimVars((PluginActions)evRecord.EventId, spec);
            return true;
          }
          _logger.LogDebug("Could not get event mapping from {action}; with key format {format}", action.TpActionToEventMap, action.KeyFormatStr);
          return false;
        }

        case PluginActions.LoadSimVars: {
          if (TryGetSomeActionData(data, "VarsFile", out var filepath)) {
            LoadCustomSimVarsFromFile(filepath.Trim());
            return true;
          }
          return false;
        }

        case PluginActions.SaveSimVars: {
          if (TryGetSomeActionData(data, "VarsFile", out var filepath)) {
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
        var lastVal = setting.StringValue;
        setting.SetValueFromString(s.Value);
        _logger.LogDebug("{setName}; previous: '{oldVal}'; sent: '{sentVal}'; new: '{newVal}'", setting.Name, lastVal, s.Value, setting.StringValue);
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

      // do check for updates; this tracks last update time and won't spam
      if (Settings.CheckVersionOnStartup.BoolValue)
        NewVersionCheck();
    }

    #endregion Plugin Action/Event Handlers

    #region TouchPortalSDK Events       ///////////////////////////////

    public void OnInfoEvent(InfoEvent message) {
      var runtimeVer = string.Format("{0:X}", VersionInfo.GetProductVersionNumber());
      _logger?.LogInformation(new EventId(1, "Connected"),
        "Touch Portal Connected with: TP v{tpV}, SDK v{sdkV}, {pluginId} entry.tp v{plugV}, {plugName} running v{prodV} ({runV})",
        message.TpVersionString, message.SdkVersion, PluginId, message.PluginVersion, VersionInfo.AssemblyName, VersionInfo.GetProductVersionString(), runtimeVer
      );
      UpdateTpStateValue("RunningState", "starting");

      ProcessPluginSettings(message.Settings);
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
      UpdateSimEventCategories();
      UpdateDeprecatedOptions();
#if !FSX
      // Init HubHop database
      if (_presets.OpenDataFile()) {
        if (Settings.UpdateHubHopOnStartup.BoolValue)
          UpdateHubHopData();
        UpdateSimEventAircraft();
      }
#endif
      // schedule update of connector values once TP has reported any of our Connectors with shortConnectorIdNotification events
      Task.Delay(1000).ContinueWith(t => UpdateAllRelatedConnectors());
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

    enum OnHoldActionType {
      NONE    = 0,
      PRESS   = 0x01,
      RELEASE = 0x02,
      REPEAT  = 0x04,
    }

    public void OnActionEvent(ActionEvent message)
    {
      // Actions used in TP "On Hold" events will send "down" and "up" events, in that order (usually).
      // "On Pressed" actions will have an "action" event (Tap) when it is _released_.
      var pressState = message.GetPressState();
      if (pressState == TouchPortalSDK.Messages.Models.Enums.Press.Tap) {
        ProcessEvent(message);
        return;
      }

      // These defaults preserve BC with older action versions w/out "On Hold" data attributes.
      var action = OnHoldActionType.PRESS | OnHoldActionType.REPEAT;
      var rate = Settings.ActionRepeatInterval.IntValue;
      var delay = Settings.ActionRepeatDelay.IntValue;

      // Look for new version of "On Hold" actions with data values specific to this use.
      if (message.Data.TryGetValue("OnHoldAction", out var actionName)) {
        action = OnHoldActionType.NONE;
        if (actionName.Contains("Press"))
          action |= OnHoldActionType.PRESS;
        if (actionName.Contains("Rel"))
          action |= OnHoldActionType.RELEASE;

        if (BooleanString.StringToBool(message.Data.GetValueOrDefault("OnHoldRepeat", "On")))
          action |= OnHoldActionType.REPEAT;

        if (action == OnHoldActionType.NONE)
          return;

        if (ConvertStringValue(message.Data.GetValueOrDefault("OnHoldRate"), DataType.Number, 0.0, double.NaN, out var fRate) && fRate > 0.0)
          rate = Math.Max((int)fRate, PluginConfig.ACTION_REPEAT_RATE_MIN_MS);
        if (ConvertStringValue(message.Data.GetValueOrDefault("OnHoldDelay"), DataType.Number, 0.0, double.NaN, out var fDelay) && fDelay > 0.0)
          delay = Math.Max((int)fDelay, PluginConfig.ACTION_REPEAT_RATE_MIN_MS);
      }
      // The UnthreadedTimer uses -1 to signal no delay, whereas here 0 means to use no delay
      if (delay == 0)
        delay = -1;
      //_logger.LogDebug("OnActionEvent - {actId}; pressState: {pressState}; timers: {TimerCount}; data: {data}", message.Id, pressState, _repeatingActionTimers.Count, ActionDataToKVPairString(data));

      if (pressState == TouchPortalSDK.Messages.Models.Enums.Press.Down) {
        // "On Hold" activated ("down" event).
        // Fire the event first and see if it succeeds if it has a PRESS flag
        if ((action & OnHoldActionType.PRESS) > 0 && !ProcessEvent(message))
          return;

        // If REPEAT flag is set, try to add this action to the repeating/scheduled actions queue, unless it already exists.
        if ((action & OnHoldActionType.REPEAT) > 0 && !_repeatingActionTimers.ContainsKey(message.ActionId)) {
          var timer = new Timer(rate, delay);
          timer.Elapsed += delegate { ProcessEvent(message); };
          if (_repeatingActionTimers.TryAdd(message.ActionId, timer))
            timer.Start();
          else
            timer.Dispose();
        }
      }
      else /*if (pressState == TouchPortalSDK.Messages.Models.Enums.Press.Up)*/ {
        // "On Hold" released ("up" event). Mark action for removal from repeating queue.
        if (_repeatingActionTimers.TryRemove(message.ActionId, out var tim))
          tim.Dispose();
        // Fire this action again only if the RELEASE flag was set.
        if ((action & OnHoldActionType.RELEASE) > 0)
          ProcessEvent(message);
      }

    }

    public void OnConnecterChangeEvent(ConnectorChangeEvent message)
    {
      // check connector status up/down (sort of like actions but harder to track); UpdateConnectorValue() returns false if connector is no longer moving.
      if (!string.IsNullOrEmpty(message.ConnectorId) && _connectorTracker.UpdateConnectorValue(message))
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

            if (!TryGetSomeActionData(data, "Unit", out var unitName)) {
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
          else if (simVar.RegistrationStatus == SimVarRegistrationStatus.Error) {
            _logger.LogWarning("Variable from name '{varName}' in Connector '{cId}' is invalid due to registration error.", fbVarName, message.ConnectorId);
            return;
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
        if (!TryGetSomeActionData(data, "FbVarName", out var fbVarName) || fbVarName == "null") {
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
        return;

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
#if !FSX
        case PluginActions.SetHubHopEvent:
          if (dataId == "VendorAircraft")
            UpdateHubHopEventSystems(message.Value, message.InstanceId, isConnector);
          else if (dataId == "System")
            UpdateHubHopEventPresets(message.Value, message.InstanceId, isConnector /*, message.Values*/);
          break;
#endif
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
        case PluginActions.ClearSimVars:
          if (dataId == "VarsSet")
            UpdateClearSimVarsSpecSelector(message.Value, message.InstanceId);
          break;

        default:
          break;
      }
    }

    public void OnBroadcastEvent(BroadcastEvent message)
    {
      if (message.Event != "pageChange")
        return;

      string pgName = cleanPageName(message.PageName);
      UpdateTpStateValue("CurrentTouchPortalPage", pgName);
      if (message.DeviceName != default) {
        TriggerTpEvent(PluginMapping.PageChange, [
          [ "PageName",     pgName ],
          [ "PreviousPage", cleanPageName(message.PreviousPageName) ],
          [ "DeviceName",   message.DeviceName ],
          [ "DeviceId",     message.DeviceId ],
          [ "DeviceIP",     message.DeviceIP ],
        ]);
      }
    }

    public void OnNotificationOptionClickedEvent(NotificationOptionClickedEvent message)
    {
      //_logger.LogDebug("{@Message}", message);
      if (message.NotificationId.Contains("NewVersion") && !string.IsNullOrWhiteSpace(message.OptionId) && message.OptionId.StartsWith("http")) {
        // Try launch default browser with URL
        try {
          System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo{ FileName = message.OptionId, UseShellExecute = true });
        }
        catch (Exception e) {
          _logger.LogError(e, "Failed to launch system browser for URL {URL}", message.OptionId);
        }
      }
    }

    #endregion TouchPortalSDK Events

    #region Utilities       ///////////////////////////////

    // used in numeric evaluator... because none of the math eval libs can parse hex notation
    static readonly System.Text.RegularExpressions.Regex _hexNumberRegex = new (@"\b0x([0-9A-Fa-f]{1,8})\b", System.Text.RegularExpressions.RegexOptions.None, TimeSpan.FromSeconds(0.05));

    private bool TryEvaluateValue(string strValue, out double value, out string errMsg) {
      value = double.NaN;
      errMsg = null;
      if (string.IsNullOrWhiteSpace(strValue)) {
        value = 0.0;
        errMsg = "Value is empty or null";
        return true;
      }
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

    static bool TryGetSomeActionData(ActionData data, string key, out string value) {
      return data.TryGetValue(key, out value) && !string.IsNullOrWhiteSpace(value);
    }

    static bool TryGetSimVarIdFromActionData(string varName, out string varId) {
      if (!string.IsNullOrEmpty(varName) && varName[^1] == ']' && (varName.IndexOf('[') is var brIdx && brIdx > -1)) {
        varId = varName[++brIdx..^1];
        return true;
      }
      varId = string.Empty;
      return false;
    }

    static bool TryGetSimVarNameFromActionData(string selector, out string varId)
    {
      varId = selector?.Split('\n', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ElementAtOrDefault(0).Replace(":N", string.Empty) ?? string.Empty;
      return !string.IsNullOrWhiteSpace(varId);
    }

    static bool TryGetSimEventIdFromActionData(string selector, out string eventId)
    {
      // old version of SimEvent names used '-' as separator for description, v1.3+ uses newline.
      eventId = selector?.Split(new[] { '\n', '-' }, 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ElementAtOrDefault(0) as string ?? string.Empty;
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

    static string cleanPageName(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
        return name;
      return name
        .TrimStart(['/','\\'])
        .Replace(".tml", string.Empty, true, CultureInfo.InvariantCulture)
        .Replace('\\', '/');
    }

    async void NewVersionCheck()
    {
      if (DateTimeOffset.Now.ToUnixTimeSeconds() is var ts && (ts - Settings.LastVersionCheckTime.LongValue < 6 * 60 * 60)) {
        _logger.LogDebug(
          "Skipping update check because last one was less than 6 hours ago. {now} - {last} = {delta}s",
          ts, Settings.LastVersionCheckTime.LongValue, (ts - Settings.LastVersionCheckTime.LongValue)
        );
        return;
      }
      Settings.LastVersionCheckTime.Value = ts;
      VersionCheckResult result = await GitHubVersionCheck.CheckForUpdates(PluginConfig.PLUGIN_REPO_URL_PATH, VersionInfo.GetProductVersion()).ConfigureAwait(false);
      if (result.ReleaseIsNewer) {
        _client.ShowNotification(
          $"{PluginId}.Notice.NewVersion-{result.ReleaseVersion}.5",
          $"{PluginConfig.PLUGIN_NAME} Update Avalable!",
          $"A new release has been published on {result.ReleaseDate.ToShortDateString()}:\n\n {result.ReleaseName}\n\n" +
            "Click one of the links for release notes and download!",
          new[] {
            new NotificationOptions { Id = result.ReleaseUrl,                   Title = "Go To GitHub Release" },
            new NotificationOptions { Id = PluginConfig.PLUGIN_URL_FLIGHTSIMIO, Title = "Go To Flightsim.io" },
          }
        );
      }
      if (!string.IsNullOrEmpty(result.ErrorMessage))
        _logger.LogWarning("Version update check error: {Message}", result.ErrorMessage);
      else
        _logger.LogDebug("Version check completed with result: {Result}", result);
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
