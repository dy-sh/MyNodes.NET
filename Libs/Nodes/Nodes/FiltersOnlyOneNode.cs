/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNodes.Nodes
{
    public class FiltersOnlyOneNode : Node
    {
        public FiltersOnlyOneNode() : base("Filters", "Only One")
        {
            AddInput();
            AddOutput("Out", DataType.Logical);
        }


        public override void OnInputChange(Input input)
        {
            if (input.Value== "1")
                Outputs[0].Value = "1";
        }

        public override string GetNodeDescription()
        {
            return "This node filters the input values. " +
                   "It transmits the value only if it is a \"1\".";
        }
    }
}