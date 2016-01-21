using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using MyNetSensors.Gateways;
using MyNetSensors.Gateways.MySensors.Serial;
using MyNetSensors.WebController.Controllers;


namespace MyNetSensors.WebController.Code
{
    public static class MySensorsSignalRServer
    {
        private static IHubContext hub;
        private static Gateway gateway;

        public static void Start(IConnectionManager connectionManager)
        {
            hub = connectionManager.GetHubContext<MySensorsHub>();

            SystemController.OnStarted += OnSystemControllerStarted;
        }
        

        private static void OnSystemControllerStarted(object sender, EventArgs e)
        {
            gateway = SystemController.gateway;

            gateway.OnMessageRecievedEvent += OnMessageRecievedEvent;
            gateway.OnMessageSendEvent += OnMessageSendEvent;
            gateway.OnConnectedEvent += OnConnectedEvent;
            gateway.OnDisconnectedEvent += OnDisconnectedEvent;
            gateway.OnRemoveAllNodesEvent += OnRemoveAllNodesEvent;
            gateway.OnNewNodeEvent += OnNewNodeEvent;
            gateway.OnNodeUpdatedEvent += OnNodeUpdatedEvent;
            gateway.OnNodeLastSeenUpdatedEvent += OnNodeLastSeenUpdatedEvent;
            gateway.OnNodeBatteryUpdatedEvent += OnNodeBatteryUpdatedEvent;
            gateway.OnSensorUpdatedEvent += OnSensorUpdatedEvent;
            gateway.OnNewSensorEvent += OnNewSensorEvent;
        }
        


        private static void OnNewSensorEvent(Sensor sensor)
        {
            hub.Clients.All.OnNewSensorEvent(sensor);
        }

        private static void OnSensorUpdatedEvent(Sensor sensor)
        {
            hub.Clients.All.OnSensorUpdatedEvent(sensor);
        }

        private static void OnNodeBatteryUpdatedEvent(Gateways.MySensors.Serial.Node node)
        {
            hub.Clients.All.OnNodeBatteryUpdatedEvent(node);
        }

        private static void OnNodeLastSeenUpdatedEvent(Gateways.MySensors.Serial.Node node)
        {
            hub.Clients.All.OnNodeLastSeenUpdatedEvent(node);
        }

        private static void OnNodeUpdatedEvent(Gateways.MySensors.Serial.Node node)
        {
            hub.Clients.All.OnNodeUpdatedEvent(node);
        }

        private static void OnNewNodeEvent(Gateways.MySensors.Serial.Node node)
        {
            hub.Clients.All.OnNewNodeEvent(node);
        }

        private static void OnRemoveAllNodesEvent()
        {
            hub.Clients.All.OnRemoveAllNodesEvent();
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
