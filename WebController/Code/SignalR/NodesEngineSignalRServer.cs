using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using MyNetSensors.Nodes;
using MyNetSensors.WebController.Controllers;


namespace MyNetSensors.WebController.Code
{
    public static class NodesEngineSignalRServer
    {
        private static IHubContext hub;
        private static NodesEngine nodesEngine;
        private static UiNodesEngine uiNodesEngine;

        public static void Start(IConnectionManager connectionManager)
        {
            hub = connectionManager.GetHubContext<NodesEngineHub>();

            SystemController.OnStarted += OnSystemControllerStarted;
            SystemController.logs.OnGatewayLogInfo += OnLogRecord;
            SystemController.logs.OnGatewayLogError += OnLogRecord;
            SystemController.logs.OnHardwareNodeLogInfo += OnLogRecord;
            SystemController.logs.OnHardwareNodeLogError += OnLogRecord;
            SystemController.logs.OnDataBaseLogInfo += OnLogRecord;
            SystemController.logs.OnDataBaseLogError += OnLogRecord;
            SystemController.logs.OnNodesEngineLogInfo += OnLogRecord;
            SystemController.logs.OnNodesEngineLogError += OnLogRecord;
            SystemController.logs.OnNodeLogInfo += OnLogRecord;
            SystemController.logs.OnNodeLogError += OnLogRecord;
            SystemController.logs.OnSystemLogInfo += OnLogRecord;
            SystemController.logs.OnSystemLogError += OnLogRecord;
        }

      


        private static void OnSystemControllerStarted(object sender, EventArgs e)
        {
            nodesEngine = SystemController.nodesEngine;
            uiNodesEngine = SystemController.uiNodesEngine;

            if (nodesEngine != null)
            {
                nodesEngine.OnNewNodeEvent += OnNewNodeEvent;
                nodesEngine.OnNodeUpdatedEvent += OnNodeUpdatedEvent;
                nodesEngine.OnRemoveNodeEvent += OnRemoveNodeEvent;
                nodesEngine.OnRemoveLinkEvent += OnRemoveLinkEvent;
                nodesEngine.OnNewLinkEvent += OnNewLinkEvent;
                nodesEngine.OnInputUpdatedEvent += OnInputUpdatedEvent;
                nodesEngine.OnOutputUpdatedEvent += OnOutputUpdatedEvent;
                nodesEngine.OnRemoveAllNodesAndLinks += OnRemoveAllNodesAndLinks;
            }

            if (uiNodesEngine != null)
            {
                uiNodesEngine.OnUiNodeUpdatedEvent += OnNodeUpdatedEvent;
                uiNodesEngine.OnNewUiNodeEvent += OnNewNodeEvent;
                uiNodesEngine.OnRemoveUiNodeEvent += OnRemoveNodeEvent;
            }
        }

        private static void OnRemoveAllNodesAndLinks()
        {
            hub.Clients.All.OnRemoveAllNodesAndLinks();
        }

        private static void OnOutputUpdatedEvent(Output output)
        {
            Node node = nodesEngine.GetOutputOwner(output);
            hub.Clients.All.OnNodeActivity(node.Id);
        }

        private static void OnInputUpdatedEvent(Input input)
        {
            Node node = nodesEngine.GetInputOwner(input);
            hub.Clients.All.OnNodeActivity(node.Id);
        }

        private static void OnRemoveNodeEvent(UiNode uiNode)
        {
            hub.Clients.All.OnRemoveNodeEvent(uiNode);
        }

        private static void OnNewNodeEvent(UiNode uiNode)
        {
            hub.Clients.All.OnNewNodeEvent(uiNode);
        }

        private static void OnNodeUpdatedEvent(UiNode uiNode)
        {
            hub.Clients.All.OnNodeUpdatedEvent(uiNode);
        }


        private static void OnNewLinkEvent(Link link)
        {
            NodesEditorAPIController nodesEditorApi = new NodesEditorAPIController();
            LiteGraph.Link liteGraphLink = nodesEditorApi.ConvertLinkToLiteGraphLink(link);
            hub.Clients.All.OnNewLinkEvent(liteGraphLink);
        }

        private static void OnRemoveLinkEvent(Link link)
        {
            NodesEditorAPIController nodesEditorApi = new NodesEditorAPIController();
            LiteGraph.Link liteGraphLink = nodesEditorApi.ConvertLinkToLiteGraphLink(link);
            hub.Clients.All.OnRemoveLinkEvent(liteGraphLink);
        }

        private static void OnRemoveNodeEvent(Node node)
        {
            NodesEditorAPIController nodesEditorApi = new NodesEditorAPIController();
            LiteGraph.Node liteGraphNode = nodesEditorApi.ConvertNodeToLiteGraphNode(node);
            hub.Clients.All.OnRemoveNodeEvent(liteGraphNode.id);
        }

        private static void OnNodeUpdatedEvent(Node node)
        {
            NodesEditorAPIController nodesEditorApi = new NodesEditorAPIController();
            LiteGraph.Node liteGraphNode = nodesEditorApi.ConvertNodeToLiteGraphNode(node);
            hub.Clients.All.OnNodeUpdatedEvent(liteGraphNode);

        }

        private static void OnNewNodeEvent(Node node)
        {
            NodesEditorAPIController nodesEditorApi = new NodesEditorAPIController();
            LiteGraph.Node liteGraphNode = nodesEditorApi.ConvertNodeToLiteGraphNode(node);
            hub.Clients.All.OnNewNodeEvent(liteGraphNode);
        }

        


        private static void OnLogRecord(LogRecord record)
        {
            hub.Clients.All.OnLogRecord(record);
        }
        
    }
}
