/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNodes.Nodes
{
    public class UiLabelNode : UiNode
    {
        public string Value { get; set; }

        public UiLabelNode() : base("UI", "Label")
        {
            AddInput();
        }


        public override void OnInputChange(Input input)
        {
            Value = input.Value;
            UpdateMe();
        }

        public override string GetNodeDescription()
        {
            return "This is a UI node. It displays a label on the dashboard. <br/>" +
                   "Label can display any text.";
        }
    }
}
