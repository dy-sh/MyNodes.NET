using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using MyNetSensors.Gateways;
using MyNetSensors.LogicalNodes;
using MyNetSensors.LogicalNodesUI;
using MyNetSensors.SerialControllers;
using MyNetSensors.WebController.Controllers;

namespace MyNetSensors.WebController.Code
{
    public static class SignalRServer
    {
        private static IHubContext hub;
        private static Gateway gateway;
        private static LogicalNodesEngine logicalNodesEngine;
        private static LogicalNodesUIEngine logicalNodesUiEngine;

        public static void Start(IConnectionManager connectionManager)
        {
            hub = connectionManager.GetHubContext<ClientsHub>();

            SerialController.OnStarted += OnSerialControllerStarted;
            SerialController.logs.OnGatewayLogInfo += OnLogRecord;
            SerialController.logs.OnGatewayLogError += OnLogRecord;
            SerialController.logs.OnNodeLogInfo += OnLogRecord;
            SerialController.logs.OnNodeLogError += OnLogRecord;
            SerialController.logs.OnDataBaseLogInfo += OnLogRecord;
            SerialController.logs.OnDataBaseLogError += OnLogRecord;
            SerialController.logs.OnLogicalNodesEngineLogInfo += OnLogRecord;
            SerialController.logs.OnLogicalNodesEngineLogError += OnLogRecord;
            SerialController.logs.OnLogicalNodeLogInfo += OnLogRecord;
            SerialController.logs.OnLogicalNodeLogError += OnLogRecord;
            SerialController.logs.OnSerialControllerLogInfo += OnLogRecord;
            SerialController.logs.OnSerialControllerLogError += OnLogRecord;
        }

      


        private static void OnSerialControllerStarted(object sender, EventArgs e)
        {
            gateway = SerialController.gateway;
            logicalNodesEngine = SerialController.logicalNodesEngine;
            logicalNodesUiEngine = SerialController.logicalNodesUIEngine;

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

            if (logicalNodesEngine != null)
            {
                logicalNodesEngine.OnNewNodeEvent += OnNewLogicalNodeEvent;
                logicalNodesEngine.OnNodeUpdatedEvent += OnLogicalNodeUpdatedEvent;
                logicalNodesEngine.OnRemoveNodeEvent += OnRemoveLogicalNodeEvent;
                logicalNodesEngine.OnLinksUpdatedEvent += OnLinksUpdatedEvent;
                logicalNodesEngine.OnRemoveLinkEvent += OnRemoveLinkEvent;
                logicalNodesEngine.OnNewLinkEvent += OnNewLinkEvent;
                logicalNodesEngine.OnInputUpdatedEvent += OnInputUpdatedEvent;
                logicalNodesEngine.OnOutputUpdatedEvent += OnOutputUpdatedEvent;
            }

            if (logicalNodesUiEngine != null)
            {
                logicalNodesUiEngine.OnUINodeUpdatedEvent += OnUINodeUpdatedEvent;
                logicalNodesUiEngine.OnNewUINodeEvent += OnNewUINodeEvent;
                logicalNodesUiEngine.OnRemoveUINodeEvent += OnRemoveUiNodeEvent;
            }
        }

        private static void OnOutputUpdatedEvent(Output output)
        {
            LogicalNode node = logicalNodesEngine.GetOutputOwner(output);
            hub.Clients.All.OnNodeActivity(node.Id);
        }

        private static void OnInputUpdatedEvent(Input input)
        {
            LogicalNode node = logicalNodesEngine.GetInputOwner(input);
            hub.Clients.All.OnNodeActivity(node.Id);
        }

        private static void OnRemoveUiNodeEvent(LogicalNodeUI node)
        {
            hub.Clients.All.OnUINodeRemoveEvent(node);
        }

        private static void OnNewUINodeEvent(LogicalNodeUI node)
        {
            hub.Clients.All.OnNewUINodeEvent(node);
        }

        private static void OnUINodeUpdatedEvent(LogicalNodeUI node)
        {
            hub.Clients.All.OnUINodeUpdatedEvent(node);
        }


        private static void OnNewLinkEvent(LogicalLink link)
        {
            NodesEditorAPIController nodesEditorApi = new NodesEditorAPIController();
            LiteGraph.Link liteGraphLink = nodesEditorApi.ConvertLogicalNodeToLitegraphLink(link);
            hub.Clients.All.OnNewLinkEvent(liteGraphLink);
        }

        private static void OnRemoveLinkEvent(LogicalLink link)
        {
            NodesEditorAPIController nodesEditorApi = new NodesEditorAPIController();
            LiteGraph.Link liteGraphLink = nodesEditorApi.ConvertLogicalNodeToLitegraphLink(link);
            hub.Clients.All.OnRemoveLinkEvent(liteGraphLink);
        }

        private static void OnLinksUpdatedEvent(List<LogicalLink> link)
        {

        }

        private static void OnRemoveLogicalNodeEvent(LogicalNode node)
        {
            NodesEditorAPIController nodesEditorApi = new NodesEditorAPIController();
            LiteGraph.Node liteGraphNode = nodesEditorApi.ConvertLogicalNodeToLitegraphNode(node);
            hub.Clients.All.OnLogicalNodeRemoveEvent(liteGraphNode.id);
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




        private static void OnLogRecord(LogRecord record)
        {
            hub.Clients.All.OnLogRecord(record);
        }
        
    }
}
