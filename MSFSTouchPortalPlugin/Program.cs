using Microsoft.Extensions.DependencyInjection;
using MSFSTouchPortalPlugin.Attributes;
using MSFSTouchPortalPlugin.Constants;
using MSFSTouchPortalPlugin.Objects.Plugin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;
using TouchPortalApi;
using TouchPortalApi.Interfaces;
using TouchPortalApi.Models;
using TouchPortalExtension.Attributes;
using static MSFSTouchPortalPlugin.SimConnectWrapper;

[assembly: InternalsVisibleTo("MSFSTouchPortalPlugin-Generator")]

namespace MSFSTouchPortalPlugin {
  class Program {
    static Dictionary<string, Enum> eventDict = new Dictionary<string, Enum>();
    static Dictionary<Definition, SimVarItem> simVarsDict = new Dictionary<Definition, SimVarItem>();
    static Dictionary<string, Enum> internalEventDict = new Dictionary<string, Enum>();
    static SimConnectWrapper simConnect = null;

    static void Main(string[] args) {
      var rootName = Assembly.GetExecutingAssembly().GetName().Name;

      // Setup DI with configured options
      var serviceProvider = new ServiceCollection()
        .ConfigureTouchPointApi((opts) => {
          opts.PluginId = rootName;
          opts.ServerIp = "127.0.0.1";
          opts.ServerPort = 12136;
        }).BuildServiceProvider();

      // Our services, can be retrieved through DI in constructors
      var messageProcessor = serviceProvider.GetRequiredService<IMessageProcessor>();
      var stateService = serviceProvider.GetRequiredService<IStateService>();

      messageProcessor.OnActionEvent += new ActionEventHandler(messageProcessor_OnActionEvent);
      messageProcessor.OnListChangeEventHandler += new ListChangeEventHandler(messageProcessor_OnListChangeEventHandler);

      // Setup Sim Connect
      try {
        // Map internal events
        var internalEventList = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsEnum && t.GetCustomAttribute<InternalEventAttribute>() != null).ToList();
        internalEventList.ForEach(internalEvent => {
          string catName = internalEvent.GetCustomAttribute<TouchPortalCategoryMappingAttribute>().CategoryId;

          var list = internalEvent.GetFields().Where(f => f.GetCustomAttribute<TouchPortalActionMappingAttribute>() != null).ToList();

          list.ForEach(ie => {
            string actionName = ie.GetCustomAttribute<TouchPortalActionMappingAttribute>().ActionId;
            string actionValue = ie.GetCustomAttribute<TouchPortalActionMappingAttribute>().Value;

            if (Enum.TryParse(ie.ReflectedType, ie.Name, out dynamic result)) {
              // Put into collection
              internalEventDict.TryAdd($"{rootName}.{catName}.Action.{actionName}:{actionValue}", result);
            }
          });
        });


        // Configure SimConnect
        simConnect = new SimConnectWrapper();
        simConnect.Connect();

        // Map all Sim Events to the Sim
        var enumList = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsEnum && t.GetCustomAttribute<SimNotificationGroupAttribute>() != null).ToList();
        enumList.ForEach(enumValue => {
          // Get the notification group to register
          Groups group = enumValue.GetCustomAttribute<SimNotificationGroupAttribute>().Group;
          string catName = enumValue.GetCustomAttribute<TouchPortalCategoryMappingAttribute>().CategoryId;

          // Configure SimConnect action mappings
          var events = enumValue.GetMembers().Where(m => m.CustomAttributes.Any(att => att.AttributeType == typeof(SimActionEventAttribute))).ToList();

          events.ForEach(e => {
            // Map the touch portal action to the Sim Event
            if (Enum.TryParse(e.ReflectedType, e.Name, out dynamic result)) {
              simConnect.MapClientEventToSimEvent(result, e.Name);
              simConnect.AddNotification(group, result);
            }

            // Register to Touch Portal
            string actionName = e.GetCustomAttribute<TouchPortalActionMappingAttribute>().ActionId;
            string actionValue = e.GetCustomAttribute<TouchPortalActionMappingAttribute>().Value;

            // Put into collection
            eventDict.TryAdd($"{rootName}.{catName}.Action.{actionName}:{actionValue}", result);
          });
        });

        var stateFieldList = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass && t.GetCustomAttribute<SimVarDataRequestGroupAttribute>() != null).ToList();
        stateFieldList.ForEach(stateFieldClass => {
          string catName = stateFieldClass.GetCustomAttribute<TouchPortalCategoryAttribute>().Name;

          // Get all States and register to SimConnect
          var states = stateFieldClass.GetFields().Where(m => m.CustomAttributes.Any(att => att.AttributeType == typeof(SimVarDataRequestAttribute))).ToList();
          states.ForEach(s => {
            // Evaluate and setup the Touch Portal State ID
            string catId = stateFieldClass.GetCustomAttribute<TouchPortalCategoryAttribute>().Id;
            var item = (SimVarItem)s.GetValue(null);
            item.TouchPortalStateId = $"{rootName}.{catId}.State.{s.Name}";

            //simVarsDict.TryAdd((Definition)Enum.Parse(typeof(Definition), s.Name), (SimVarItem)s.GetValue(null));
            simVarsDict.TryAdd(item.def, item);
          });
        });

        // On Data Update
        simConnect.OnDataUpdateEvent += ((Definition def, Request req, object data) => {
          if (simVarsDict.TryGetValue(def, out var value)) {
            var stringVal = data.ToString();

            if (value.Value != stringVal) {
              value.Value = stringVal;
              object valObj = stringVal;

              switch (value.Unit) {
                case Units.degrees:
                case Units.knots:
                case Units.feet:
                  valObj = float.Parse(stringVal);
                  break;
                case Units.radians:
                  // Convert to Degrees
                  valObj = float.Parse(stringVal) * (180/Math.PI);
                  break;
              }

              // Update if known id.
              if (!string.IsNullOrWhiteSpace(value.TouchPortalStateId)) {
                stateService.UpdateState(new StateUpdate() { Id = value.TouchPortalStateId, Value = string.Format(value.StringFormat, valObj) });
              }
            }

            value.SetPending(false);
          }
        });

        // Register SimVars
        foreach (var s in simVarsDict) {
          simConnect.RegisterToSimConnect(s.Value);
        }

        var timer = new Timer(250) { AutoReset = false };

        timer.Elapsed += (obj, args) => {
          foreach (var s in simVarsDict) {
            // Expire pending if more than 30 seconds
            s.Value.PendingTimeout();

            // Check if Pending data request in paly
            if (!s.Value.PendingRequest) {
              simConnect.RequestDataOnSimObjectType(s.Value);
              s.Value.SetPending(true);
            }
          }
          timer.Start();
        };

        timer.Start();
      } catch (COMException ex) {

      }

      //Task.Run(RunTimer);

      // Run Listen and pairing
      Task.WhenAll(new Task[] {
        messageProcessor.Listen(),
        messageProcessor.TryPairAsync(),
        simConnect.WaitForMessage()
      });

      Console.ReadLine();

      // Dispose
      simConnect.Disconnect();
    }

    private static void messageProcessor_OnListChangeEventHandler(string actionId, string value) {
      Console.WriteLine($"{DateTime.Now} Choice Event Fired.");
    }

    private static void messageProcessor_OnActionEvent(string actionId, List<ActionData> dataList) {
      if (dataList.Count > 0) {
        dataList.ForEach(a => {
          ProcessEvent(actionId, a.Value);
        });
      } else {
        ProcessEvent(actionId);
      }
    }

    private static void ProcessEvent(string actionId, string value = default) {
      // Plugin Events
      if (internalEventDict.TryGetValue($"{actionId}:{value}", out var internalEventResult)) {
        Console.WriteLine($"{DateTime.Now} {internalEventResult} - Firing Internal Event");

        // TODO: Modify Connect/Disconnect to re-setup events and notifications
        switch (internalEventResult) {
          case Plugin.ToggleConnection:
            if (simConnect.Connected) {
              simConnect.Disconnect();
            } else {
              simConnect.Connect();
            }
            break;
          case Plugin.Connect:
            if (!simConnect.Connected) {
              simConnect.Connect();
            }
            break;
          case Plugin.Disconnect:
            if (simConnect.Connected) {
              simConnect.Disconnect();
            }
            break;
        }

        return;
      }
      // Sim Events
      if (eventDict.TryGetValue($"{actionId}:{value}", out var eventResult)) {
        Console.WriteLine($"{DateTime.Now} {eventResult} - Firing Event");
        var group = eventResult.GetType().GetCustomAttribute<SimNotificationGroupAttribute>().Group;
        simConnect.TransmitClientEvent(group, eventResult, 0);

        return;
      }
    }
  }
}
