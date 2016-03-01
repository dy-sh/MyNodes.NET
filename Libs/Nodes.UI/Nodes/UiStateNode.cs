/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNodes.Nodes
{
    public class UiStateNode : UiNode
    {
        public string Value { get; set; }

        public UiStateNode() : base("UI", "State")
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
            return "This is a UI node. It displays a switch on the dashboard " +
                   "that can be used to monitor the status of any node. <br/>" +
                   "It takes logical 0 or 1. When another value, the node will be red.";
        }
    }
}
