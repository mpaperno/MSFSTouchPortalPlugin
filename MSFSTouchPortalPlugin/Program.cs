using Microsoft.Extensions.DependencyInjection;
using Microsoft.FlightSimulator.SimConnect;
using MSFSTouchPortalPlugin.Attributes;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using TouchPortalApi;
using TouchPortalApi.Interfaces;

[assembly: InternalsVisibleTo("MSFSTouchPortalPlugin-Generator")]

namespace MSFSTouchPortalPlugin {
  class Program {
    const int WM_USER_SIMCONNECT = 0x0402;
    static SimConnect simconnect = null;

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
      var actionService = serviceProvider.GetRequiredService<IActionService>();
      var choiceService = serviceProvider.GetRequiredService<IChoiceService>();

      // Setup Sim Connect
      try {
        simconnect = new SimConnect("Managed Data Request", Process.GetCurrentProcess().MainWindowHandle, WM_USER_SIMCONNECT, null, 0);

        // TODO: Map all Enums to Sim
        var a = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsEnum).ToList();
        a.ForEach(assembly => {
          var events = assembly.GetMembers().Where(m => m.CustomAttributes.Any(att => att.AttributeType == typeof(SimActionEventAttribute))).ToList();

          events.ForEach(e => {
            Type t = e.ReflectedType;
            if (Enum.TryParse(e.ReflectedType, e.Name, out dynamic result)) {
              simconnect.MapClientEventToSimEvent(result, e.Name);

              // Register action event
              actionService.RegisterActionEvent($"{e.DeclaringType.FullName}.{e.Name}", (obj) => {
                Console.WriteLine($"{DateTime.Now} {result} - Firing Event");
                simconnect.TransmitClientEvent((uint)SimConnect.SIMCONNECT_OBJECT_ID_USER, result, 0, Group.Test, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
                foreach (var o in obj) {
                  Console.WriteLine($"Id: {o.Id} Value: {o.Value}");
                }
              });
            }
          });
        });

        simconnect.OnRecvClientData += (sender, data) => {
          Console.WriteLine("Received");
        };

        simconnect.OnRecvEvent += new SimConnect.RecvEventEventHandler(simconnect_OnRecvEvent);

        //simconnect.OnRecvEvent += (sender, data) => {
        //  Console.WriteLine("Received");
        //};

        simconnect.OnRecvSimobjectDataBytype += (sender, data) => {
          Console.WriteLine("Received");
        };

        simconnect.OnRecvSimobjectData += new SimConnect.RecvSimobjectDataEventHandler(simconnect_OnRecvSimObjectData);

        simconnect.AddClientEventToNotificationGroup(Group.Test, AutoPilot.AP_HDG_HOLD_ON, false);
        simconnect.SetNotificationGroupPriority(Group.Test, 10000000);

        simconnect.ReceiveMessage();

        simconnect.Text(SIMCONNECT_TEXT_TYPE.PRINT_BLACK, 5, Events.Test, "Test Message");
      } catch (COMException ex) {

      }

      // Register event callbacks with ID of the button or choice id from your plugin, returned data is a list of action IDs and values from your plugin
      //actionService.RegisterActionEvent("MSFS-TouchPortal.Plugin.InstrumentsSystems.Fuel.AddFuel", (obj) => {
      //  Console.WriteLine($"{DateTime.Now} MSFS Action Event Fired.");
      //  simconnect.TransmitClientEvent((uint)SimConnect.SIMCONNECT_OBJECT_ID_USER, Fuel.ADD_FUEL_QUANTITY, 0, Group.Test, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
      //  foreach (var o in obj) {
      //    Console.WriteLine($"Id: {o.Id} Value: {o.Value}");
      //  }
      //});

      // Register Choice Events - Returned data is the new value
      choiceService.RegisterChoiceEvent("choice test", (obj) => {
        Console.WriteLine($"{DateTime.Now} Choice Event Fired.");
      });

      // Run Listen and pairing
      Task.WhenAll(new Task[] {
        messageProcessor.Listen(),
        messageProcessor.TryPairAsync(),
        new Task(() => {
          while(1==1) simconnect.ReceiveDispatch(null);
        })
      });

      Console.ReadLine();

      if (simconnect != null) {
        simconnect.Dispose();
        simconnect = null;
      }
    }

    private static void simconnect_OnRecvEvent(SimConnect sender, SIMCONNECT_RECV_EVENT data) {
      Console.WriteLine("Recieved");
    }

    private static void simconnect_OnRecvSimObjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data) {
      Console.WriteLine("Recieved");
      //throw new NotImplementedException();
    }
  }

  public enum Events {
    Test = 0
  }

  public enum Group {
    Test = 0
  }
}
