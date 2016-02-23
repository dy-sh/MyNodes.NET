/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;

namespace MyNetSensors.Nodes
{
    public class PanelOutputNode : Node
    {
        //Id must be equal to panel output id

        public PanelOutputNode() : base("Main", "Panel Output")
        {
            AddInput();
            Settings.Add("Name", new NodeSetting(NodeSettingType.Text, "Name", ""));
        }


        public override void OnInputChange(Input input)
        {
            if (engine != null)
                engine.GetOutput(Id).Value = input.Value;
        }

        public override bool OnAddToEngine(NodesEngine engine)
        {
            if (PanelId == engine.MAIN_PANEL_ID)
            {
                LogError("Can`t create output for main panel.");
                return false;
            }

            var panel = engine.GetPanelNode(PanelId);
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
            var panel = engine.GetPanelNode(PanelId);
            panel?.RemovePanelOutput(this);
        }

        public void UpdateName(string name)
        {
            Settings["Name"].Value = name;

            var output = engine.GetOutput(Id);
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

        public override string GetNodeDescription()
        {
            return "This node adds the Output to the panel." +
                   "The value that comes to this input inside the panel, " +
                   "becomes accessible from outside the panel.";
        }
    }
}