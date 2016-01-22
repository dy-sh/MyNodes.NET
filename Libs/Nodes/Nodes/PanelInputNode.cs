/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
    public class PanelInputNode:Node
    {
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
    }
}
