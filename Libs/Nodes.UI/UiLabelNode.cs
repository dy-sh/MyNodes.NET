/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
    public class UiLabelNode : UiNode
    {
        public string Value { get; set; }

        public UiLabelNode() : base(1, 0)
        {
            this.Title = "UI Label";
            this.Type = "UI/Label";
            this.DefaultName = "Label";
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            Value = input.Value;
            UpdateNode(false);
        }


    }
}
