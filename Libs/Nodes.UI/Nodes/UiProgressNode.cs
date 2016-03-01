/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNodes.Nodes
{
    public class UiProgressNode : UiNode
    {
        public string Value { get; set; }

        public UiProgressNode() : base("UI", "Progress")
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
            return "This is a UI node. It displays a progress bar on the dashboard. <br/>" +
                   "The progress bar may display the progress of a certain event in percent. <br/>" +
                   "It takes a value from 0 to 100.";
        }
    }
}
