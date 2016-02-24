/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
    public class FiltersOnlyFromRangeNode : Node
    {
        public FiltersOnlyFromRangeNode() : base("Filters", "Only From Range")
        {
            AddInput("Value",DataType.Number);
            AddInput("Min",DataType.Number);
            AddInput("Max",DataType.Number);
            AddOutput("Out",DataType.Number);

            options.ResetOutputsIfAnyInputIsNull = true;
        }


        public override void OnInputChange(Input input)
        {
            if (input == Inputs[0])
            {
                double val = double.Parse(Inputs[0].Value);
                double min = double.Parse(Inputs[1].Value);
                double max = double.Parse(Inputs[2].Value);

                if (val >= min && val <= max)
                    Outputs[0].Value = input.Value;
            }
        }

        public override string GetNodeDescription()
        {
            return "This node filters the input values. " +
                   "It transmits the value only if it is in the range from Min to Max.";
        }
    }
}