using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.Nodes.Nodes
{
    public class OperationAccumulatorNode : Node
    {
        public double Value { get; set; }

        public OperationAccumulatorNode() : base(2, 1)
        {
            this.Title = "Accumulator";
            this.Type = "Operation/Accumulator";

            Inputs[0].Type = DataType.Number;
            Inputs[1].Type = DataType.Number;
            Outputs[0].Type = DataType.Number;

            Inputs[0].Name = "Set Value";
            Inputs[1].Name = "Add Value";
            Outputs[0].Value = Value.ToString();
        }
        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            double oldValue = Value;

            if (input == Inputs[0] && input.Value != null)
                Value = Int32.Parse(input.Value);

            if (input == Inputs[1] && input.Value != null)
                Value += Int32.Parse(input.Value);

            if (oldValue != Value)
            {
                Outputs[0].Value = Value.ToString();
                UpdateMeInDb();
            }
        }
    }
}
