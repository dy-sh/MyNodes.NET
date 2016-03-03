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
    public static class DashboardSignalRServer
    {
        private static string MAIN_PAGE_ID = "Home";

        private static IHubContext hub;
        private static NodesEngine nodesEngine;
        private static UiNodesEngine uiNodesEngine;


        public static void Start(IConnectionManager connectionManager)
        {
            hub = connectionManager.GetHubContext<DashboardHub>();

            SystemController.OnStarted += OnSystemControllerStarted;
            SystemController.OnGatewayConnected += OnGatewayConnected;
            SystemController.OnGatewayDisconnected += OnGatewayDisconnected;
        }


        private static void OnSystemControllerStarted()
        {
            nodesEngine = SystemController.nodesEngine;
            uiNodesEngine = SystemController.uiNodesEngine;

            if (nodesEngine != null)
            {
                nodesEngine.OnRemoveAllNodesAndLinks += OnRemoveAllNodesAndLinks;
            }

            if (uiNodesEngine != null)
            {
                uiNodesEngine.OnUiNodeUpdated += OnUiNodeUpdated;
                uiNodesEngine.OnNewUiNode += OnNewUiNode;
                uiNodesEngine.OnRemoveUiNode += OnRemoveUiNode;
                uiNodesEngine.OnPanelNodeUpdated += OnPanelNodeUpdated;
                uiNodesEngine.OnHideFromHomePage += OnHideFromHomePage;
            }
        }

        private static void OnHideFromHomePage(UiNode uinode)
        {
            hub.Clients.Group(MAIN_PAGE_ID).OnHideFromHomePage(uinode);
        }


        private static void OnRemoveUiNode(UiNode uinode)
        {
            hub.Clients.Group(uinode.PanelId).OnRemoveUiNode(uinode);
            if (uinode.Settings["ShowOnHomePage"].Value == "true")
                hub.Clients.Group(MAIN_PAGE_ID).OnRemoveUiNode(uinode);
        }

        private static void OnNewUiNode(UiNode uinode)
        {
            hub.Clients.Group(uinode.PanelId).OnNewUiNode(uinode);
            if (uinode.Settings["ShowOnHomePage"].Value == "true")
                hub.Clients.Group(MAIN_PAGE_ID).OnNewUiNode(uinode);
        }

        private static void OnUiNodeUpdated(UiNode uinode)
        {
            hub.Clients.Group(uinode.PanelId).OnUiNodeUpdated(uinode);
            if (uinode.Settings["ShowOnHomePage"].Value == "true")
                hub.Clients.Group(MAIN_PAGE_ID).OnUiNodeUpdated(uinode);
        }

        private static void OnRemoveAllNodesAndLinks()
        {
            hub.Clients.All.OnRemoveAllNodesAndLinks();
        }

        private static void OnPanelNodeUpdated(Node node)
        {
            NodeEditorAPIController nodeEditorApi = new NodeEditorAPIController();
            LiteGraph.Node liteGraphNode = nodeEditorApi.ConvertNodeToLiteGraphNode(node);
            hub.Clients.All.OnPanelNodeUpdated(liteGraphNode);
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
