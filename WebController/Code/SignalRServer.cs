using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using MyNetSensors.Gateways;
using MyNetSensors.LogicalNodes;
using MyNetSensors.SerialControllers;
using MyNetSensors.WebController.Controllers;

namespace MyNetSensors.WebController.Code
{
    public static class SignalRServer
    {
        private static IHubContext hub;
        private static Gateway gateway;
        private static LogicalNodesEngine logicalNodesEngine;

        public static void Start(IConnectionManager connectionManager)
        {
            hub = connectionManager.GetHubContext<ClientsHub>();

            SerialController.OnStarted += OnSerialControllerStarted;
            SerialController.logs.OnGatewayLogInfo += OnGatewayLogInfo;
            SerialController.logs.OnNodeLogInfo += OnNodeLogInfo;
            SerialController.logs.OnDataBaseLogInfo += OnDataBaseLogInfo;
            SerialController.logs.OnLogicalNodesEngineLogInfo += OnLogicalNodesEngineLogInfo;
            SerialController.logs.OnLogicalNodeLogInfo += OnLogicalNodeLogInfo;
            SerialController.logs.OnSerialControllerLogInfo += OnSerialControllerLogInfo;
        }

      


        private static void OnSerialControllerStarted(object sender, EventArgs e)
        {
            gateway = SerialController.gateway;
            logicalNodesEngine = SerialController.logicalNodesEngine;

            gateway.OnMessageRecievedEvent += OnMessageRecievedEvent;
            gateway.OnMessageSendEvent += OnMessageSendEvent;
            gateway.OnConnectedEvent += OnConnectedEvent;
            gateway.OnDisconnectedEvent += OnDisconnectedEvent;
            gateway.OnClearNodesListEvent += OnClearNodesListEvent;
            gateway.OnNewNodeEvent += OnNewNodeEvent;
            gateway.OnNodeUpdatedEvent += OnNodeUpdatedEvent;
            gateway.OnNodeLastSeenUpdatedEvent += OnNodeLastSeenUpdatedEvent;
            gateway.OnNodeBatteryUpdatedEvent += OnNodeBatteryUpdatedEvent;
            gateway.OnSensorUpdatedEvent += OnSensorUpdatedEvent;
            gateway.OnNewSensorEvent += OnNewSensorEvent;

            if (logicalNodesEngine != null)
            {
                logicalNodesEngine.OnNewNodeEvent += OnNewLogicalNodeEvent;
                logicalNodesEngine.OnNodeUpdatedEvent += OnLogicalNodeUpdatedEvent;
                logicalNodesEngine.OnNodeDeleteEvent += OnLogicalNodeDeleteEvent;
                logicalNodesEngine.OnLinksUpdatedEvent += OnLinksUpdatedEvent;
                logicalNodesEngine.OnLinkDeleteEvent += OnLinkDeleteEvent;
                logicalNodesEngine.OnNewLinkEvent += OnNewLinkEvent;
            }
        }


        private static void OnNewLinkEvent(LogicalLink link)
        {
            NodesEditorAPIController nodesEditorApi = new NodesEditorAPIController();
            LiteGraph.Link liteGraphLink = nodesEditorApi.ConvertLogicalNodeToLitegraphLink(link);
            hub.Clients.All.OnNewLinkEvent(liteGraphLink);
        }

        private static void OnLinkDeleteEvent(LogicalLink link)
        {
            NodesEditorAPIController nodesEditorApi = new NodesEditorAPIController();
            LiteGraph.Link liteGraphLink = nodesEditorApi.ConvertLogicalNodeToLitegraphLink(link);
            hub.Clients.All.OnDeleteLinkEvent(liteGraphLink);
        }

        private static void OnLinksUpdatedEvent(List<LogicalLink> link)
        {

        }

        private static void OnLogicalNodeDeleteEvent(LogicalNode node)
        {
            NodesEditorAPIController nodesEditorApi = new NodesEditorAPIController();
            LiteGraph.Node liteGraphNode = nodesEditorApi.ConvertLogicalNodeToLitegraphNode(node);
            hub.Clients.All.OnLogicalNodeDeleteEvent(liteGraphNode);
        }

        private static void OnLogicalNodeUpdatedEvent(LogicalNode node)
        {
            NodesEditorAPIController nodesEditorApi = new NodesEditorAPIController();
            LiteGraph.Node liteGraphNode = nodesEditorApi.ConvertLogicalNodeToLitegraphNode(node);
            hub.Clients.All.OnLogicalNodeUpdatedEvent(liteGraphNode);

        }

        private static void OnNewLogicalNodeEvent(LogicalNode node)
        {
            NodesEditorAPIController nodesEditorApi = new NodesEditorAPIController();
            LiteGraph.Node liteGraphNode = nodesEditorApi.ConvertLogicalNodeToLitegraphNode(node);
            hub.Clients.All.OnNewLogicalNodeEvent(liteGraphNode);
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




        private static void OnSerialControllerLogInfo(LogRecord record)
        {
            hub.Clients.All.OnSerialControllerLog(record.ToString());
            hub.Clients.All.OnLog(record.ToStringWithType());
        }

        private static void OnLogicalNodesEngineLogInfo(LogRecord record)
        {
            hub.Clients.All.OnLogicalNodesEngineLog(record.ToString());
            hub.Clients.All.OnLog(record.ToStringWithType());
        }

        private static void OnLogicalNodeLogInfo(LogRecord record)
        {
            hub.Clients.All.OnLogicalNodesLog(record.ToString());
            hub.Clients.All.OnLog(record.ToStringWithType());
        }

        private static void OnDataBaseLogInfo(LogRecord record)
        {
            hub.Clients.All.OnDataBaseStateLog(record.ToString());
            hub.Clients.All.OnLog(record.ToStringWithType());
        }


        private static void OnNodeLogInfo(LogRecord record)
        {
            hub.Clients.All.OnGatewayMessagesLog(record.Message);
            hub.Clients.All.OnLog(record.ToStringWithType());
        }

        private static void OnGatewayLogInfo(LogRecord record)
        {
            hub.Clients.All.OnGatewayStateLog(record.ToString());
            hub.Clients.All.OnLog(record.ToStringWithType());
        }
    }
}
