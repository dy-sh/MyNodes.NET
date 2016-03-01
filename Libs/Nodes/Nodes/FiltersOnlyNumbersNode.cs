/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNodes.Nodes
{
    public class FiltersOnlyNumbersNode : Node
    {
        public FiltersOnlyNumbersNode() : base("Filters", "Only Numbers")
        {
            AddInput();
            AddOutput("Out",DataType.Number);
        }


        public override void OnInputChange(Input input)
        {
            double val;

            if (double.TryParse(input.Value, out val))
                Outputs[0].Value = val.ToString();
        }

        public override string GetNodeDescription()
        {
            return "This node filters the input values. " +
                   "It transmits the value only if it is a number.";
        }
    }
}