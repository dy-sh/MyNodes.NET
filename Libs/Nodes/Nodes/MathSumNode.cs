/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes.Nodes
{
    public class MathSumNode : Node
    {
        public double Value { get; set; }

        public MathSumNode() : base("Math", "Sum")
        {
            AddInput("Add Value", DataType.Number);
            AddInput("Set Value", DataType.Number, true);
            AddInput("Reset", DataType.Logical, true);
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

            if (input == Inputs[2] && input.Value == "1")
                Value = 0;

            if (oldValue != Value)
            {
                Outputs[0].Value = Value.ToString();
                UpdateMeInDb();
            }
        }

        public override string GetNodeDescription()
        {
            return "This node calculates sum of all incoming values. <br/>" +
                   "The internal counter can be overridden by the input named \"Set Value\". <br/>" +
                    "To reset node, send logical \"1\" to input named \"Reset\"";
        }
    }
}