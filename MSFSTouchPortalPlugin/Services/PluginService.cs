using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Interfaces;
using MSFSTouchPortalPlugin.Objects.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TouchPortalApi.Interfaces;
using TouchPortalApi.Models;
using Timer = System.Timers.Timer;

namespace MSFSTouchPortalPlugin.Services {
  /// <inheritdoc cref="IPluginService" />
  internal class PluginService : IPluginService, IDisposable {
    private CancellationToken _cancellationToken;
    private CancellationTokenSource _simConnectCancellationTokenSource;

    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly ILogger<PluginService> _logger;
    private readonly IMessageProcessor _messageProcessor;
    private readonly ISimConnectService _simConnectService;
    private readonly IReflectionService _reflectionService;

    private Dictionary<string, Enum> actionsDictionary = new Dictionary<string, Enum>();
    private Dictionary<Definition, SimVarItem> statesDictionary = new Dictionary<Definition, SimVarItem>();
    private Dictionary<string, Enum> internalEventsDictionary = new Dictionary<string, Enum>();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="messageProcessor">Message Processor Object</param>
    public PluginService(IHostApplicationLifetime hostApplicationLifetime, ILogger<PluginService> logger,
      IMessageProcessor messageProcessor, ISimConnectService simConnectService, IReflectionService reflectionService) {
      _hostApplicationLifetime = hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _messageProcessor = messageProcessor ?? throw new ArgumentNullException(nameof(messageProcessor));
      _simConnectService = simConnectService ?? throw new ArgumentNullException(nameof(simConnectService));
      _reflectionService = reflectionService ?? throw new ArgumentNullException(nameof(reflectionService));
    }

    /// <summary>
    /// Initialized the Touch Portal Message Processor
    /// </summary>
    private void Initialize() {
      _messageProcessor.OnActionEvent += new ActionEventHandler(MessageProcessor_OnActionEvent);
      _messageProcessor.OnListChangeEventHandler += new ListChangeEventHandler(MessageProcessor_OnListChangeEventHandler);
      _messageProcessor.OnCloseEventHandler += () => {
        _logger.LogInformation($"{DateTime.Now} TP Request Close, terminating...");
        Environment.Exit(0);
      };

      Task.Run(_messageProcessor.Listen);
      Task.Run(_messageProcessor.TryPairAsync);

      // On Data Update
      _simConnectService.OnDataUpdateEvent += ((Definition def, Definition req, object data) => {
        // Lookup State Mapping
        if (statesDictionary.TryGetValue(def, out var value)) {
          var stringVal = data.ToString();

          // Only update state on changes
          if (value.Value != stringVal) {
            value.Value = stringVal;
            object valObj = stringVal;

            // Handle conversions
            if (ShouldConvertToFload(value.Unit)) {
              valObj = float.Parse(stringVal);
            } else if (value.Unit == Units.radians) {
              // Convert to Degrees
              valObj = float.Parse(stringVal) * (180 / Math.PI);
            }

            // Update if known id.
            if (!string.IsNullOrWhiteSpace(value.TouchPortalStateId)) {
              _messageProcessor.UpdateState(new StateUpdate { Id = value.TouchPortalStateId, Value = string.Format(value.StringFormat, valObj) });
            }
          }

          value.SetPending(false);
        }
      });

      _simConnectService.OnConnect += () => {
        _simConnectCancellationTokenSource = new CancellationTokenSource();

        _messageProcessor.UpdateState(new StateUpdate { Id = "MSFSTouchPortalPlugin.Plugin.State.Connected", Value = _simConnectService.IsConnected().ToString().ToLower() });

        // Register Actions
        foreach (var a in actionsDictionary) {
          _simConnectService.MapClientEventToSimEvent(a.Value, a.Value.ToString());
          _simConnectService.AddNotification(a.Value.GetType().GetCustomAttribute<SimNotificationGroupAttribute>().Group, a.Value);
        }

        // Register SimVars
        foreach (var s in statesDictionary) {
          _simConnectService.RegisterToSimConnect(s.Value);
        }

        Task.WhenAll(RunPluginServices(_simConnectCancellationTokenSource.Token));
      };

      _simConnectService.OnDisconnect += () => {
        _simConnectCancellationTokenSource.Cancel();
        _messageProcessor.UpdateState(new StateUpdate { Id = "MSFSTouchPortalPlugin.Plugin.State.Connected", Value = _simConnectService.IsConnected().ToString().ToLower() });
      };
    }

    /// <summary>
    /// All the unit types that should convert to Float
    /// </summary>
    /// <param name="unitType">The unit type</param>
    /// <returns>True if it should be a float</returns>
    private bool ShouldConvertToFload(string unitType) {
      return unitType == Units.degrees ||
        unitType == Units.knots ||
        unitType == Units.feet ||
        unitType == Units.MHz ||
        unitType == Units.percent ||
        unitType == Units.rpm ||
        unitType == Units.mach ||
        unitType == Units.feetminute;
    }

    private void SetupEventLists() {
      internalEventsDictionary = _reflectionService.GetInternalEvents();
      actionsDictionary = _reflectionService.GetActionEvents();
      statesDictionary = _reflectionService.GetStates();
    }

    private Task TryConnect() {
      int i = 0;

      while (!_cancellationToken.IsCancellationRequested) {
        if (i == 0 && !_simConnectService.IsConnected()) {
          _simConnectService.Connect();
          i = 10;
        }

        // SimConnect is typically available even before loading into a flight. This should connect and be ready by the time a flight is started.
        i--;
        Thread.Sleep(1000);
      }

      return Task.CompletedTask;
    }

    public async Task RunPluginServices(CancellationToken simConnectCancelToken) {
      // Run Data Polling
      var timer = new Timer(250) { AutoReset = false };

      simConnectCancelToken.Register(() => {
        timer.Stop();
        timer.Dispose();
      });

      timer.Elapsed += (obj, args) => {
        foreach (var s in statesDictionary) {
          // Expire pending if more than 30 seconds
          s.Value.PendingTimeout();

          // Check if Pending data request in paly
          if (!s.Value.PendingRequest) {
            _simConnectService.RequestDataOnSimObjectType(s.Value);
            s.Value.SetPending(true);
          }
        }
        timer.Start();
      };

      timer.Start();

      // Run Listen and pairing
      await Task.WhenAll(new Task[] {
        _simConnectService.WaitForMessage(simConnectCancelToken)
      }).ConfigureAwait(false);
    }

    #region OnEvents

    private void MessageProcessor_OnListChangeEventHandler(string actionId, string value) {
      _logger.LogInformation($"{DateTime.Now} Choice Event Fired. ActionId: {actionId} Value: {value}");
    }

    private void MessageProcessor_OnActionEvent(string actionId, List<ActionData> dataList) {
      if (dataList.Count > 0) {
        var values = string.Join(",", dataList.Select(x => x.Value));
        ProcessEvent(actionId, values);
      } else {
        ProcessEvent(actionId);
      }
    }

    private void ProcessEvent(string actionId, string value = default) {
      // Plugin Events
      if (internalEventsDictionary.TryGetValue($"{actionId}:{value}", out var internalEventResult)) {
        _logger.LogInformation($"{DateTime.Now} {internalEventResult} - Firing Internal Event");

        switch (internalEventResult) {
          case Plugin.ToggleConnection:
            if (_simConnectService.IsConnected()) {
              _simConnectService.Disconnect();
            } else {
              _simConnectService.Connect();
            }
            break;
          case Plugin.Connect:
            if (!_simConnectService.IsConnected()) {
              _simConnectService.Connect();
            }
            break;
          case Plugin.Disconnect:
            if (_simConnectService.IsConnected()) {
              _simConnectService.Disconnect();
            }
            break;
          default:
            // No other types of events supported right now.
            break;
        }

        return;
      }

      // Sim Events
      if (actionsDictionary.TryGetValue($"{actionId}:{value}", out var eventResult)) {
        _logger.LogInformation($"{DateTime.Now} {eventResult} - Firing Event");
        var group = eventResult.GetType().GetCustomAttribute<SimNotificationGroupAttribute>().Group;
        _simConnectService.TransmitClientEvent(group, eventResult, 0);
      }
    }

    public Task StartAsync(CancellationToken cancellationToken) {
      _cancellationToken = cancellationToken;

      _hostApplicationLifetime.ApplicationStarted.Register(() => {
        Initialize();
        SetupEventLists();

        Task.WhenAll(TryConnect());
      });

      _hostApplicationLifetime.ApplicationStopping.Register(() => {
        // Disconnect from SimConnect
        _simConnectService.Disconnect();

        // Stop app
        _hostApplicationLifetime.StopApplication();
      });

      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) {
      return Task.CompletedTask;
    }

    #region IDisposable Support
    private bool disposedValue; // To detect redundant calls

    protected virtual void Dispose(bool disposing) {
      if (!disposedValue) {
        if (disposing) {
          // Dispose managed state (managed objects).
          _simConnectCancellationTokenSource.Dispose();
        }

        disposedValue = true;
      }
    }

    // This code added to correctly implement the disposable pattern.
    public void Dispose() {
      // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
      Dispose(true);
    }
    #endregion

    #endregion
  }
}
