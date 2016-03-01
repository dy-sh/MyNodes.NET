/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNodes.Nodes
{
    public class FiltersOnlyLogicNode : Node
    {
        public FiltersOnlyLogicNode() : base("Filters", "Only Logic")
        {
            AddInput();
            AddOutput("Out", DataType.Logical);
        }


        public override void OnInputChange(Input input)
        {
            if (input.Value == "1" || input.Value == "0")
                Outputs[0].Value = input.Value;
        }

        public override string GetNodeDescription()
        {
            return "This node filters the input values. " +
                   "It transmits the value only if it is a logical value (0 or 1).";
        }
    }
}