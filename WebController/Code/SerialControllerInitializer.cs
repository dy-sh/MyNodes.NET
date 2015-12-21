using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
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

        public static async Task Start(ILoggerFactory loggerFactory, IConfigurationRoot Configuration, IConnectionManager connectionManager)
        {
            if (serialControllerStarted) return;
            serialControllerStarted = true;

            var logger = loggerFactory.CreateLogger("RequestInfoLogger");

            //configure
            SerialController.SerialController.serialPortDebugState = Boolean.Parse(Configuration["SerialPort:DebugState"]);
            SerialController.SerialController.serialPortDebugTxRx = Boolean.Parse(Configuration["SerialPort:DebugTxRx"]);
            SerialController.SerialController.enableAutoAssignId = Boolean.Parse(Configuration["Gateway:EnableAutoAssignId"]);
            SerialController.SerialController.gatewayDebugState = Boolean.Parse(Configuration["Gateway:DebugState"]);
            SerialController.SerialController.gatewayDebugTxRx = Boolean.Parse(Configuration["Gateway:DebugTxRx"]);

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
            string portName = Configuration["SerialPort:Name"];
            SerialController.SerialController.Start(portName);

            //while (true)
            //{
            //    //logger.LogInformation(DateTime.Now);
            //    await Task.Delay(5000);
            //}
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
