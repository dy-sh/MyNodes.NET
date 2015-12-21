using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Hosting.Internal;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyNetSensors.Gateway;
using MyNetSensors.SerialController;

namespace MyNetSensors.WebServer.Code
{
    public static class SerialControllerInitializer
    {
        private static bool serialControllerStarted;

        private static IHubContext hub;

        public static async Task Start(ILoggerFactory loggerFactory, IConfigurationRoot Configuration,
            IConnectionManager connectionManager)
        {
            if (serialControllerStarted) return;
            serialControllerStarted = true;

            var logger = loggerFactory.CreateLogger("RequestInfoLogger");

            //configure
            string portName = null;
            try
            {
                SerialController.SerialController.serialPortDebugState = Boolean.Parse(Configuration["SerialPort:DebugState"]);
                SerialController.SerialController.serialPortDebugTxRx = Boolean.Parse(Configuration["SerialPort:DebugTxRx"]);
                SerialController.SerialController.enableAutoAssignId = Boolean.Parse(Configuration["Gateway:EnableAutoAssignId"]);
                SerialController.SerialController.gatewayDebugState = Boolean.Parse(Configuration["Gateway:DebugState"]);
                SerialController.SerialController.gatewayDebugTxRx = Boolean.Parse(Configuration["Gateway:DebugTxRx"]);

                SerialController.SerialController.dataBaseEnabled = Boolean.Parse(Configuration["DataBase:Enable"]);
                SerialController.SerialController.dataBaseConnectionString = Configuration["DataBase:ConnectionString"];
                SerialController.SerialController.dataBaseWriteInterval = Int32.Parse(Configuration["DataBase:WriteInterval"]);
                SerialController.SerialController.dataBaseDebugState = Boolean.Parse(Configuration["DataBase:DebugState"]);
                SerialController.SerialController.dataBaseWriteTxRxMessages = Boolean.Parse(Configuration["DataBase:WriteTxRxMessages"]);
                SerialController.SerialController.sensorsTasksEnabled = Boolean.Parse(Configuration["SensorsTasks:Enable"]);
                SerialController.SerialController.sensorsTasksUpdateInterval = Int32.Parse(Configuration["SensorsTasks:UpdateInterval"]);
                SerialController.SerialController.sensorsLinksEnabled = Boolean.Parse(Configuration["SensorsLinks:Enable"]);
                SerialController.SerialController.softNodesEnabled = Boolean.Parse(Configuration["SoftNodes:Enable"]);
                SerialController.SerialController.softNodesPort = Int32.Parse(Configuration["SoftNodes:Port"]);
                SerialController.SerialController.softNodesDebugTxRx = Boolean.Parse(Configuration["SoftNodes:DebugTxRx"]);
                SerialController.SerialController.softNodesDebugState = Boolean.Parse(Configuration["SoftNodes:DebugState"]);

                portName = Configuration["SerialPort:Name"];
            }
            catch
            {
                logger.LogInformation("ERROR: Bad configuration in appsettings.json file.");
                throw new Exception("Bad configuration in appsettings.json file.");
                
            }

            if (portName != null)
            {
                SerialController.SerialController.OnDebugStateMessage += logger.LogInformation;
                SerialController.SerialController.OnDebugTxRxMessage += logger.LogInformation;

                hub = connectionManager.GetHubContext<ClientsHub>();
                SerialController.SerialController.gateway.OnMessageRecievedEvent += OnMessageRecievedEvent;
                SerialController.SerialController.gateway.OnConnectedEvent += OnConnectedEvent;
                SerialController.SerialController.gateway.OnDisconnectedEvent += OnDisconnectedEvent;
                SerialController.SerialController.gateway.OnClearNodesListEvent += OnClearNodesListEvent;
                SerialController.SerialController.gateway.OnNewNodeEvent += OnNewNodeEvent;
                SerialController.SerialController.gateway.OnNodeUpdatedEvent += OnNodeUpdatedEvent;
                SerialController.SerialController.gateway.OnNodeLastSeenUpdatedEvent += OnNodeLastSeenUpdatedEvent;
                SerialController.SerialController.gateway.OnNodeBatteryUpdatedEvent += OnNodeBatteryUpdatedEvent;
                SerialController.SerialController.gateway.OnSensorUpdatedEvent += OnSensorUpdatedEvent;
                SerialController.SerialController.gateway.OnNewSensorEvent += OnNewSensorEvent;


                //start
                SerialController.SerialController.Start(portName);

                //while (true)
                //{
                //    //logger.LogInformation(DateTime.Now);
                //    await Task.Delay(5000);
                //}
            }
        }

        private static void OnNewSensorEvent(Sensor sensor)
        {
            hub.Clients.All.OnNewSensorEvent(sensor);
        }

        private static void OnSensorUpdatedEvent(Sensor sensor)
        {
            hub.Clients.All.OnSensorUpdatedEvent(sensor);
        }

        private static void OnNodeBatteryUpdatedEvent(Node node)
        {
            hub.Clients.All.OnNodeBatteryUpdatedEvent(node);
        }

        private static void OnNodeLastSeenUpdatedEvent(Node node)
        {
            hub.Clients.All.OnNodeLastSeenUpdatedEvent(node);
        }

        private static void OnNodeUpdatedEvent(Node node)
        {
            hub.Clients.All.OnNodeUpdatedEvent(node);
        }

        private static void OnNewNodeEvent(Node node)
        {
            hub.Clients.All.OnNewNodeEvent(node);
        }

        private static void OnClearNodesListEvent()
        {
            hub.Clients.All.OnClearNodesListEvent();
        }

        private static void OnDisconnectedEvent()
        {
            hub.Clients.All.OnDisconnectedEvent();
        }

        private static void OnConnectedEvent()
        {
            hub.Clients.All.OnConnectedEvent();
        }


        private static void OnMessageRecievedEvent(Message message)
        {
            hub.Clients.All.OnMessageRecievedEvent(message.ToString());
        }

    }
}
