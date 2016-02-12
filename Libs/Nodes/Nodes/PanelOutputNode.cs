/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;
using System.Linq;

namespace MyNetSensors.Nodes
{
    public class PanelOutputNode : Node
    {
        //Id must be equal to panel output id

        public PanelOutputNode() : base("Main","Panel Output",1, 0)
        {
            Settings.Add("Name", new NodeSetting(NodeSettingType.Text, "Name", ""));
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            if (engine!=null)
            engine.GetOutput(Id).Value = input.Value;
        }

        public override bool OnAddToEngine(NodesEngine engine)
        {
            if (PanelId == engine.MAIN_PANEL_ID)
            {
                LogError("Can`t create output for main panel.");
                return false;
            }

            PanelNode panel = engine.GetPanelNode(PanelId);
            if (panel == null)
            {
                LogError($"Can`t create panel output. Panel [{PanelId}] does not exist.");
                return false;
            }

            panel.AddOutputNode(this);

            base.OnAddToEngine(engine);
            return true;
        }

    

        public override void OnRemove()
        {
            PanelNode panel = engine.GetPanelNode(PanelId);
            panel?.RemoveOutput(this);
        }

        public void UpdateName(string name)
        {
            Settings["Name"].Value = name;

            Output output = engine.GetOutput(Id);
            output.Name = name;

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
            function PanelOutputNode() {
                this.properties = {
                    ObjectType: 'MyNetSensors.Nodes.PanelOutputNode',
                    'Assembly': 'Nodes'
                };
                this.bgcolor = '#151515';
            }
            PanelOutputNode.title = 'Panel Output';
            LiteGraph.registerNodeType('Main/Panel Output', PanelOutputNode);

            ";
        }
    }
}
