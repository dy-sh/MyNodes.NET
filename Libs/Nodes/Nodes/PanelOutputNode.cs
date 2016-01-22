/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

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
        }
    }
}
