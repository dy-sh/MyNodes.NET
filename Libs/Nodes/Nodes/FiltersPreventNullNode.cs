/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
    public class FiltersPreventNullNode : Node
    {
        public FiltersPreventNullNode() : base("Filters", "Prevent Null")
        {
            AddInput();
            AddOutput();
        }


        public override void OnInputChange(Input input)
        {
            if (input.Value != null)
                Outputs[0].Value = input.Value;
        }

        public override string GetNodeDescription()
        {
            return "This node filters the input values. " +
                   "It transmits the value only if it is not a null.";
        }
    }
}