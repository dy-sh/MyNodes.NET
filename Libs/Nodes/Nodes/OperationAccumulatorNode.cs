/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes.Nodes
{
    public class OperationAccumulatorNode : Node
    {
        public double Value { get; set; }

        public OperationAccumulatorNode() : base("Operation", "Accumulator")
        {
            AddInput("Add Value", DataType.Number);
            AddInput("Set Value", DataType.Number,true);
            AddOutput("Out", DataType.Number);

            Outputs[0].Value = Value.ToString();
        }

        public override void OnInputChange(Input input)
        {
            var oldValue = Value;

            if (input == Inputs[0] && input.Value != null)
                Value += double.Parse(input.Value);

            if (input == Inputs[1] && input.Value != null)
                Value = double.Parse(input.Value);

            if (oldValue != Value)
            {
                Outputs[0].Value = Value.ToString();
                UpdateMeInDb();
            }
        }

        public override string GetNodeDescription()
        {
            return "This node adds the incoming value (Add Value) again and again. <br/>" +
                   "The output gives the result of all the additions. <br/>" +
                   "The internal counter can be overridden by the input Set Value.";
        }
    }
}