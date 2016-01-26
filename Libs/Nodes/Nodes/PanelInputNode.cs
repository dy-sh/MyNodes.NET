/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;
using System.Linq;

namespace MyNetSensors.Nodes
{
    public class PanelInputNode : Node
    {
        //Id must be equal to panel input id
        public string Name { get; set; }

        public PanelInputNode() : base(0, 1)
        {
            this.Title = "Input";
            this.Type = "Main/Panel Input";
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
        }

        public override bool OnAddToEngine(NodesEngine engine)
        {
            if (PanelId == engine.MAIN_PANEL_ID)
            {
                LogError("Can`t create input for main panel.");
                return false;
            }

            PanelNode panel = engine.GetPanelNode(PanelId);
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
            PanelNode panel = engine.GetPanelNode(PanelId);
            panel?.RemoveInput(this);
        }

        public void UpdateName(string name)
        {
            Name = name;

            Input input = engine.GetInput(Id);
            input.Name = Name;

            Node panel = engine.GetPanelNode(PanelId);

            engine.UpdateNode(panel);
            engine.UpdateNode(this);

            engine.UpdateNodeInDb(panel);
            engine.UpdateNodeInDb(this);
        }
    }
}
