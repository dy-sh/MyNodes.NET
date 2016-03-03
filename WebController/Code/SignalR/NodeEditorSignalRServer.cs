/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using MyNodes.Nodes;
using MyNodes.WebController.Controllers;


namespace MyNodes.WebController.Code
{
    public static class NodeEditorSignalRServer
    {
        private static IHubContext hub;
        private static NodesEngine nodesEngine;

        public static void Start(IConnectionManager connectionManager)
        {
            hub = connectionManager.GetHubContext<NodeEditorHub>();

            SystemController.OnStarted += OnSystemControllerStarted;
            SystemController.OnGatewayConnected += OnGatewayConnected;
            SystemController.OnGatewayDisconnected += OnGatewayDisconnected;
        }

      


        private static void OnSystemControllerStarted()
        {
            nodesEngine = SystemController.nodesEngine;

            if (nodesEngine != null)
            {
                nodesEngine.OnNewNode += OnNewNode;
                nodesEngine.OnNodeUpdated += OnNodeUpdated;
                nodesEngine.OnRemoveNode += OnRemoveNode;
                nodesEngine.OnRemoveLink += OnRemoveLink;
                nodesEngine.OnNewLink += OnNewLink;
                nodesEngine.OnNodeActivity += OnNodeActivity;
                nodesEngine.OnRemoveAllNodesAndLinks += OnRemoveAllNodesAndLinks;
            }
        }

        

        private static void OnRemoveAllNodesAndLinks()
        {
            hub.Clients.All.OnRemoveAllNodesAndLinks();
        }

        
        private static void OnNodeActivity(Node node)
        {
            hub.Clients.All.OnNodeActivity(node.Id);
        }


        private static void OnNewLink(Link link)
        {
            NodeEditorAPIController nodeEditorApi = new NodeEditorAPIController();
            LiteGraph.Link liteGraphLink = nodeEditorApi.ConvertLinkToLiteGraphLink(link);
            hub.Clients.All.OnNewLink(liteGraphLink);
        }

        private static void OnRemoveLink(Link link)
        {
            NodeEditorAPIController nodeEditorApi = new NodeEditorAPIController();
            LiteGraph.Link liteGraphLink = nodeEditorApi.ConvertLinkToLiteGraphLink(link);
            hub.Clients.All.OnRemoveLink(liteGraphLink);
        }

        private static void OnRemoveNode(Node node)
        {
            NodeEditorAPIController nodeEditorApi = new NodeEditorAPIController();
            LiteGraph.Node liteGraphNode = nodeEditorApi.ConvertNodeToLiteGraphNode(node);
            hub.Clients.All.OnRemoveNode(liteGraphNode.id);
        }

        private static void OnNodeUpdated(Node node)
        {
            NodeEditorAPIController nodeEditorApi = new NodeEditorAPIController();
            LiteGraph.Node liteGraphNode = nodeEditorApi.ConvertNodeToLiteGraphNode(node);
            hub.Clients.All.OnNodeUpdated(liteGraphNode);
        }

        private static void OnNewNode(Node node)
        {
            NodeEditorAPIController nodeEditorApi = new NodeEditorAPIController();
            LiteGraph.Node liteGraphNode = nodeEditorApi.ConvertNodeToLiteGraphNode(node);
            hub.Clients.All.OnNewNode(liteGraphNode);
        }

        private static void OnGatewayConnected()
        {
            hub.Clients.All.OnGatewayConnected();
        }

        private static void OnGatewayDisconnected()
        {
            hub.Clients.All.OnGatewayDisconnected();
        }
    }
}
