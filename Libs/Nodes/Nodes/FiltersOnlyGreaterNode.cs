/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes.Nodes
{
    public class FiltersOnlyGreaterNode : Node
    {
        public FiltersOnlyGreaterNode() : base("Filters", "Only Greater")
        {
            AddInput("Treshold", DataType.Number);
            AddInput("Value", DataType.Number);

            AddOutput("Out", DataType.Number);

            options.ResetOutputsIfAnyInputIsNull = true;
        }


        public override void OnInputChange(Input input)
        {
            if (input == Inputs[1])
            {
                var threshold = double.Parse(Inputs[0].Value);
                var val = double.Parse(Inputs[1].Value);

                if (val > threshold)
                    Outputs[0].Value = val.ToString();
            }
        }

        public override string GetNodeDescription()
        {
            return "This node filters the input values. " + 
                    "It passes only those values that are greater than a Treshold.";
        }
    }
}