using System.Collections.Generic;
using System.Linq;

namespace MyNetSensors.Nodes
{
    public delegate void UiNodeEventHandler(UiNode uiNode);

    public class UiNodesEngine
    {
        public event UiNodeEventHandler OnNewUiNode;
        public event UiNodeEventHandler OnRemoveUiNode;
        public event UiNodeEventHandler OnUiNodeUpdated;

        private static NodesEngine engine;

        private INodesStatesRepository statesDb;



        public UiNodesEngine(NodesEngine engine, INodesStatesRepository statesDb = null)
        {
            this.statesDb = statesDb;

            UiNodesEngine.engine = engine;
            engine.OnNewNode += OnNewNode;
            engine.OnRemoveNode += OnRemoveNode;
            engine.OnNodeUpdated += OnNodeUpdated;
            engine.OnRemoveAllNodesAndLinks += OnRemoveAllNodesAndLinks;

            GetStatesFromRepository();
        }

        private void OnRemoveAllNodesAndLinks()
        {
            statesDb?.RemoveAllStates();
        }

        private void GetStatesFromRepository()
        {
            if (statesDb == null)
                return;

            List<UiChartNode> charts = engine.GetNodes()
                .Where(n => n is UiChartNode)
                .Cast<UiChartNode>()
                .ToList();

            foreach (var chart in charts)
            {
                if (chart.Settings["WriteInDatabase"].Value!="true") continue;

                List<NodeState> states = statesDb.GetStatesForNode(chart.Id);
                chart.SetStates(states);
            }
        }



        private void OnNodeUpdated(Node node)
        {
            if (node is UiNode)
                OnUiNodeUpdated?.Invoke((UiNode)node);


            if (node is UiChartNode)
            {
                UiChartNode chart = (UiChartNode)node;
                if (chart.Settings["WriteInDatabase"].Value == "true" && chart.State != null)
                {
                    statesDb?.AddState(new NodeState(chart.Id, chart.State.ToString()));
                }
            }
        }

        private void OnRemoveNode(Node node)
        {
            if (node is UiNode)
                OnRemoveUiNode?.Invoke((UiNode)node);

            if (node is UiChartNode)
                statesDb?.RemoveStatesForNode(node.Id);
        }

        private void OnNewNode(Node node)
        {
            if (!(node is UiNode)) return;

            UiNode n = (UiNode)node;

            OnNewUiNode?.Invoke(n);

            if (string.IsNullOrEmpty(n.Settings["Name"].Value))
                n.Settings["Name"].Value = GenerateName(n);

            engine.UpdateNode(n);
            engine.UpdateNodeInDb(n);
        }

        private string GenerateName(UiNode uiNode)
        {
            //auto naming
            List<UiNode> nodes = GetUINodesForPanel(uiNode.PanelId);
            List<string> names = nodes.Select(x => x.Settings["Name"].Value).ToList();
            for (int i = 1; i <= names.Count + 1; i++)
            {
                if (!names.Contains($"{uiNode.DefaultName} {i}"))
                    return $"{uiNode.DefaultName} {i}";
            }
            return null;
        }


        public List<PanelNode> GetPanels()
        {
            return engine.GetNodes()
                .Where(n => n is PanelNode)
                .Cast<PanelNode>()
                .ToList();
        }



        public PanelNode GetPanel(string id)
        {
            Node node = engine.GetNode(id);

            return node as PanelNode;
        }

        public UiNode GetUINode(string id)
        {
            return engine.GetNode(id) as UiNode;
        }


        public List<UiNode> GetUINodes()
        {
            return engine.GetNodes()
                .Where(n => n is UiNode)
                .Cast<UiNode>()
                .ToList();
        }

        public List<UiNode> GetUINodesForMainPage()
        {
            return engine.GetNodes()
                .Where(n => n is UiNode && ((UiNode)n).Settings["ShowOnMainPage"].Value=="true")
                .Cast<UiNode>()
                .ToList();
        }

        public List<UiNode> GetUINodesForPanel(string panelId)
        {
            return engine.GetNodes()
                .Where(n => n is UiNode && n.PanelId == panelId)
                .Cast<UiNode>()
                .ToList();
        }
        


        public void ClearChart(string nodeId)
        {
            UiChartNode uiNode = engine.GetNode(nodeId) as UiChartNode;
            if (uiNode == null)
                return;

            uiNode.RemoveStates();
            statesDb?.RemoveStatesForNode(nodeId);

            //send update ivent
            engine.UpdateNode(uiNode);
            engine.UpdateNodeInDb(uiNode);
        }


    }
}
