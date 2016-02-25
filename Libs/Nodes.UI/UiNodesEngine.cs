/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

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
        


        public UiNodesEngine(NodesEngine engine)
        {
            UiNodesEngine.engine = engine;
            engine.OnNewNode += OnNewNode;
            engine.OnRemoveNode += OnRemoveNode;
            engine.OnNodeUpdated += OnNodeUpdated;

            List<UiNode> nodes = engine.GetNodes()
                .OfType<UiNode>()
                .ToList();

            foreach (var node in nodes)
                node.OnAddToUiEngine(this);
        }
        
        
        private void OnNodeUpdated(Node node)
        {
            if (node is UiNode)
                OnUiNodeUpdated?.Invoke((UiNode)node);
        }

        private void OnRemoveNode(Node node)
        {
            if (node is UiNode)
                OnRemoveUiNode?.Invoke((UiNode)node);
        }

        private void OnNewNode(Node node)
        {
            if (!(node is UiNode)) return;

            UiNode n = (UiNode)node;

            n.OnAddToUiEngine(this);

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

    }
}
