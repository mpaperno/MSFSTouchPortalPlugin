using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Interfaces;
using MSFSTouchPortalPlugin.Objects.Plugin;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using TouchPortalApi.Interfaces;
using TouchPortalApi.Models;

namespace MSFSTouchPortalPlugin.Services {
  /// <inheritdoc cref="IPluginService" />
  internal class PluginService : IPluginService {
    private readonly IMessageProcessor _messageProcessor;
    private readonly ISimConnectService _simConnectService;

    private Dictionary<string, Enum> actionsDictionary = new Dictionary<string, Enum>();
    private Dictionary<Definition, SimVarItem> statesDictionary = new Dictionary<Definition, SimVarItem>();
    private Dictionary<string, Enum> internalEventsDictionary = new Dictionary<string, Enum>();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="messageProcessor">Message Processor Object</param>
    public PluginService(IMessageProcessor messageProcessor, ISimConnectService simConnectService) {
      _messageProcessor = messageProcessor ?? throw new ArgumentNullException(nameof(messageProcessor));
      _simConnectService = simConnectService ?? throw new ArgumentNullException(nameof(simConnectService));

      Initialize();
    }

    /// <summary>
    /// Initialized the Touch Portal Message Processor
    /// </summary>
    private void Initialize() {
      _messageProcessor.OnActionEvent += new ActionEventHandler(messageProcessor_OnActionEvent);
      _messageProcessor.OnListChangeEventHandler += new ListChangeEventHandler(messageProcessor_OnListChangeEventHandler);
      _messageProcessor.OnCloseEventHandler += () => {
        Console.WriteLine($"{DateTime.Now} TP Request Close, terminating...");
        Environment.Exit(0);
      };

      // On Data Update
      _simConnectService.OnDataUpdateEvent += ((Definition def, Request req, object data) => {
        // Lookup State Mapping
        if (statesDictionary.TryGetValue(def, out var value)) {
          var stringVal = data.ToString();

          // Only update state on changes
          if (value.Value != stringVal) {
            value.Value = stringVal;
            object valObj = stringVal;

            // Handle conversions
            switch (value.Unit) {
              case Units.degrees:
              case Units.knots:
              case Units.feet:
                valObj = float.Parse(stringVal);
                break;
              case Units.radians:
                // Convert to Degrees
                valObj = float.Parse(stringVal) * (180 / Math.PI);
                break;
            }

            // Update if known id.
            if (!string.IsNullOrWhiteSpace(value.TouchPortalStateId)) {
              _messageProcessor.UpdateState(new StateUpdate() { Id = value.TouchPortalStateId, Value = string.Format(value.StringFormat, valObj) });
            }
          }

          value.SetPending(false);
        }
      });

      _simConnectService.OnConnect += () => {
        // Map the touch portal action to the Sim Event
        //foreach (var a in actionsDictionary) {
        //  if (Enum.TryParse(a.ReflectedType, e.Name, out dynamic result)) {
        //    _simConnectService.MapClientEventToSimEvent(result, e.Name);
        //    _simConnectService.AddNotification(group, result);
        //  }
        //}

        // Register Actions
        foreach (var a in actionsDictionary) {
          _simConnectService.MapClientEventToSimEvent(a.Value, a.Value.ToString());
          _simConnectService.AddNotification(a.Value.GetType().GetCustomAttribute<SimNotificationGroupAttribute>().Group, a.Value);
        }


        // Register SimVars
        foreach (var s in statesDictionary) {
          _simConnectService.RegisterToSimConnect(s.Value);
        }
      };
    }

    public void TryConnect() {
      _simConnectService.Connect();
    }

    public void SetupEventLists(Dictionary<string, Enum> internalEvents, Dictionary<string, Enum> actionEvents, Dictionary<Definition, SimVarItem> states) {
      internalEventsDictionary = internalEvents;
      actionsDictionary = actionEvents;
      statesDictionary = states;
    }

    public async Task RunPluginServices() {
      // Run Data Polling
      var timer = new Timer(250) { AutoReset = false };

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
        _messageProcessor.Listen(),
        _messageProcessor.TryPairAsync(),
        _simConnectService.WaitForMessage()
      });
    }

    #region OnEvents

    private void messageProcessor_OnListChangeEventHandler(string actionId, string value) {
      Console.WriteLine($"{DateTime.Now} Choice Event Fired.");
    }

    private void messageProcessor_OnActionEvent(string actionId, List<ActionData> dataList) {
      if (dataList.Count > 0) {
        dataList.ForEach(a => {
          ProcessEvent(actionId, a.Value);
        });
      } else {
        ProcessEvent(actionId);
      }
    }

    private void ProcessEvent(string actionId, string value = default) {
      // Plugin Events
      if (internalEventsDictionary.TryGetValue($"{actionId}:{value}", out var internalEventResult)) {
        Console.WriteLine($"{DateTime.Now} {internalEventResult} - Firing Internal Event");

        // TODO: Modify Connect/Disconnect to re-setup events and notifications
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
        }

        return;
      }

      // Sim Events
      if (actionsDictionary.TryGetValue($"{actionId}:{value}", out var eventResult)) {
        Console.WriteLine($"{DateTime.Now} {eventResult} - Firing Event");
        var group = eventResult.GetType().GetCustomAttribute<SimNotificationGroupAttribute>().Group;
        _simConnectService.TransmitClientEvent(group, eventResult, 0);

        return;
      }
    }

    #endregion
  }
}
