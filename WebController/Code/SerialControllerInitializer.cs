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


            //start
            string portName = Configuration["SerialPort:Name"];
            SerialController.SerialController.Start(portName);

            //while (true)
            //{
            //    //logger.LogInformation(DateTime.Now);
            //    await Task.Delay(5000);
            //}
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
