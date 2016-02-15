/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;

namespace MyNetSensors.Nodes
{
    public class PanelInputNode : Node
    {
        //Id must be equal to panel input id

        public PanelInputNode() : base("Main", "Panel Input", 0, 1)
        {
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
            panel?.RemoveInput(this);
        }

        public void UpdateName(string name)
        {
            Settings["Name"].Value = name;

            var input = engine.GetInput(Id);
            input.Name = name;

            Node panel = engine.GetPanelNode(PanelId);

            engine.UpdateNode(panel);
            engine.UpdateNodeInDb(panel);
        }

        public override bool SetSettings(Dictionary<string, string> data)
        {
            UpdateName(data["Name"]);
            return base.SetSettings(data);
        }

        public override string GetJsListGenerationScript()
        {
            return @"

            //PanelOutputNode
            function PanelInputNode() {
                this.properties = {
                    ObjectType: 'MyNetSensors.Nodes.PanelInputNode',
                    'Assembly': 'Nodes'
                };
                this.bgcolor = '#151515';

            }
            PanelInputNode.title = 'Panel Input';
            LiteGraph.registerNodeType('Main/Panel Input', PanelInputNode);

            ";
        }
    }
}