/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
    public class PanelNode:Node
    {
        public string Name { get; set; }

        public PanelNode() : base(0, 0)
        {
            this.Title = "Panel";
            this.Type = "Main/Panel";
            this.Name = "Panel";
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
        }
    }
}
