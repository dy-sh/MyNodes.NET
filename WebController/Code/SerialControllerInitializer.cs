/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Hosting.Internal;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyNetSensors.Gateway;
using MyNetSensors.SerialControl;

namespace MyNetSensors.WebController.Code
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
                SerialController.serialPortDebugState = Boolean.Parse(Configuration["SerialPort:DebugState"]);
                SerialController.serialPortDebugTxRx = Boolean.Parse(Configuration["SerialPort:DebugTxRx"]);
                SerialController.enableAutoAssignId = Boolean.Parse(Configuration["Gateway:EnableAutoAssignId"]);
                SerialController.gatewayDebugState = Boolean.Parse(Configuration["Gateway:DebugState"]);
                SerialController.gatewayDebugTxRx = Boolean.Parse(Configuration["Gateway:DebugTxRx"]);

                SerialController.dataBaseEnabled = Boolean.Parse(Configuration["DataBase:Enable"]);
                SerialController.dataBaseConnectionString = Configuration["DataBase:ConnectionString"];
                SerialController.dataBaseWriteInterval = Int32.Parse(Configuration["DataBase:WriteInterval"]);
                SerialController.dataBaseDebugState = Boolean.Parse(Configuration["DataBase:DebugState"]);
                SerialController.dataBaseWriteTxRxMessages = Boolean.Parse(Configuration["DataBase:WriteTxRxMessages"]);
                SerialController.sensorsTasksEnabled = Boolean.Parse(Configuration["SensorsTasks:Enable"]);
                SerialController.sensorsTasksUpdateInterval = Int32.Parse(Configuration["SensorsTasks:UpdateInterval"]);
                SerialController.sensorsLinksEnabled = Boolean.Parse(Configuration["SensorsLinks:Enable"]);
                SerialController.softNodesEnabled = Boolean.Parse(Configuration["SoftNodes:Enable"]);
                SerialController.softNodesPort = Int32.Parse(Configuration["SoftNodes:Port"]);
                SerialController.softNodesDebugTxRx = Boolean.Parse(Configuration["SoftNodes:DebugTxRx"]);
                SerialController.softNodesDebugState = Boolean.Parse(Configuration["SoftNodes:DebugState"]);

                portName = Configuration["SerialPort:Name"];
            }
            catch
            {
                Log("ERROR: Bad configuration in appsettings.json file.");
                throw new Exception("Bad configuration in appsettings.json file.");
            }

            if (portName != null)
            {
                SerialController.OnDebugStateMessage += Log;
                SerialController.OnDebugTxRxMessage += Log;

                hub = connectionManager.GetHubContext<ClientsHub>();
                SerialController.gateway.OnMessageRecievedEvent += OnMessageRecievedEvent;
                SerialController.gateway.OnMessageSendEvent += OnMessageSendEvent;
                SerialController.gateway.OnConnectedEvent += OnConnectedEvent;
                SerialController.gateway.OnDisconnectedEvent += OnDisconnectedEvent;
                SerialController.gateway.OnClearNodesListEvent += OnClearNodesListEvent;
                SerialController.gateway.OnNewNodeEvent += OnNewNodeEvent;
                SerialController.gateway.OnNodeUpdatedEvent += OnNodeUpdatedEvent;
                SerialController.gateway.OnNodeLastSeenUpdatedEvent += OnNodeLastSeenUpdatedEvent;
                SerialController.gateway.OnNodeBatteryUpdatedEvent += OnNodeBatteryUpdatedEvent;
                SerialController.gateway.OnSensorUpdatedEvent += OnSensorUpdatedEvent;
                SerialController.gateway.OnNewSensorEvent += OnNewSensorEvent;


                //start
                SerialController.Start(portName);
            }
        }



        public static void Log(string message)
        {
            Console.WriteLine(message);
           // logger.LogInformation(message);
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

        private static void OnMessageSendEvent(Message message)
        {
            hub.Clients.All.OnMessageSendEvent(message.ToString());

        }
    }
}
