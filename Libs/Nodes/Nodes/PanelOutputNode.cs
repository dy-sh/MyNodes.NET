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
        public string Name { get; set; }

        public PanelOutputNode() : base(1, 0)
        {
            this.Title = "Output";
            this.Type = "Main/Panel Output";
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
            Name = name;

            Output output = engine.GetOutput(Id);
            output.Name = Name;

            Node panel = engine.GetPanelNode(PanelId);

            engine.UpdateNode(panel);
            engine.UpdateNode(this);

            engine.UpdateNodeInDb(panel);
            engine.UpdateNodeInDb(this);
        }
    }
}
