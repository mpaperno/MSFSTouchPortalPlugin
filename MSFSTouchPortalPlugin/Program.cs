using Microsoft.Extensions.DependencyInjection;
using MSFSTouchPortalPlugin.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using TouchPortalApi;
using TouchPortalApi.Interfaces;
using TouchPortalApi.Models;
using static MSFSTouchPortalPlugin.SimConnectWrapper;

[assembly: InternalsVisibleTo("MSFSTouchPortalPlugin-Generator")]

namespace MSFSTouchPortalPlugin {
  class Program {
    static Dictionary<string, Enum> eventDict = new Dictionary<string, Enum>();
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

      // Configure SimConnect
      simConnect = new SimConnectWrapper();
      simConnect.Connect();

      messageProcessor.OnActionEvent += new ActionEventHandler(messageProcessor_OnActionEvent);
      messageProcessor.OnListChangeEventHandler += new ListChangeEventHandler(messageProcessor_OnListChangeEventHandler);

      // Setup Sim Connect
      try {
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
        
        //simconnect.AddToDataDefinition(Group.TouchPortal, "GROUND VELOCITY:1", "knots", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
        //simconnect.RegisterDataDefineStruct<double>(Group.TouchPortal);

        // Register all actions to Touch Portal



      } catch (COMException ex) {

      }

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
      if (eventDict.TryGetValue($"{actionId}:{value}", out var eventResult)) {
        Console.WriteLine($"{DateTime.Now} {eventResult} - Firing Event");
        var group = eventResult.GetType().GetCustomAttribute<SimNotificationGroupAttribute>().Group;
        simConnect.TransmitClientEvent(group, eventResult, 0);
      }
    }
  }
}
