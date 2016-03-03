/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;

namespace MyNodes.Nodes
{
    public class PanelInputNode : Node
    {
        //Id must be equal to panel input id

        public PanelInputNode() : base("Main", "Panel Input")
        {
            AddOutput();
            Settings.Add("Name", new NodeSetting(NodeSettingType.Text, "Name", ""));
        }


        public override bool OnAddToEngine(NodesEngine engine)
        {
            if (PanelId == engine.MAIN_PANEL_ID)
            {
                LogError("Can`t create input for main panel.");
                return false;
            }

            var panel = engine.GetPanelNode(PanelId);
            if (panel == null)
            {
                LogError($"Can`t create panel input. Panel [{PanelId}] does not exist.");
                return false;
            }

            panel.AddInputNode(this);

            base.OnAddToEngine(engine);
            return true;
        }


        public override void OnRemove()
        {
            var panel = engine.GetPanelNode(PanelId);
            panel?.RemovePanelInput(this);
        }

        public void UpdateName(string name)
        {
            Settings["Name"].Value = name;

            var input = engine.GetInput(Id);
            input.Name = name;

            Node panel = engine.GetPanelNode(PanelId);

            engine.UpdateNodeInEditor(panel);
            engine.UpdateNodeInDb(panel);
        }

        public override bool SetSettings(Dictionary<string, string> data)
        {
            UpdateName(data["Name"]);
            return base.SetSettings(data);
        }

        public override string GetNodeDescription()
        {
            return "This node adds the Input to the panel.<br/>" +
                   "The value that comes on the outside of the panel, " +
                   "becomes available inside the panel.";
        }
    }
}