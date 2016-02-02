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

            SystemController.OnGatewayConnected += OnGatewayConnected;
        }
        

        private static void OnGatewayConnected()
        {
            gateway = SystemController.gateway;

            gateway.OnMessageRecieved += OnMessageRecieved;
            gateway.OnMessageSend += OnMessageSend;
            gateway.OnConnected += OnConnected;
            gateway.OnDisconnected += OnDisconnected;
            gateway.OnNewNode += OnNewNode;
            gateway.OnNodeUpdated += OnNodeUpdated;
            gateway.OnNodeLastSeenUpdated += OnNodeLastSeenUpdated;
            gateway.OnNodeBatteryUpdated += OnNodeBatteryUpdated;
            gateway.OnNewSensor += OnNewSensor;
            gateway.OnSensorUpdated += OnSensorUpdated;
            gateway.OnRemoveAllNodes += OnRemoveAllNodes;
            gateway.OnRemoveNode += OnRemoveNode;
        }

        private static void OnRemoveNode(Node node)
        {
            hub.Clients.All.OnRemoveNode(node.Id);
        }


        private static void OnNewSensor(Sensor sensor)
        {
            hub.Clients.All.OnNewSensor(sensor);
        }

        private static void OnSensorUpdated(Sensor sensor)
        {
            hub.Clients.All.OnSensorUpdated(sensor);
        }

        private static void OnNodeBatteryUpdated(Gateways.MySensors.Serial.Node node)
        {
            hub.Clients.All.OnNodeBatteryUpdated(node);
        }

        private static void OnNodeLastSeenUpdated(Gateways.MySensors.Serial.Node node)
        {
            hub.Clients.All.OnNodeLastSeenUpdated(node);
        }

        private static void OnNodeUpdated(Gateways.MySensors.Serial.Node node)
        {
            hub.Clients.All.OnNodeUpdated(node);
        }

        private static void OnNewNode(Gateways.MySensors.Serial.Node node)
        {
            hub.Clients.All.OnNewNode(node);
        }

        private static void OnRemoveAllNodes()
        {
            hub.Clients.All.OnRemoveAllNodes();
        }

        private static void OnDisconnected()
        {
            hub.Clients.All.OnDisconnected();
        }

        private static void OnConnected()
        {
            hub.Clients.All.OnConnected();
        }



        private static void OnMessageRecieved(Message message)
        {
            hub.Clients.All.OnMessageRecieved(message.ToString());
        }

        private static void OnMessageSend(Message message)
        {
            hub.Clients.All.OnMessageSend(message.ToString());

        }
       
    }
}
