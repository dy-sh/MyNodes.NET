using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.Nodes.Nodes
{
    public class OperationAccumulatorNode:Node
    {
        public int Value { get; set; }

        public OperationAccumulatorNode():base(2,1)
        {
            this.Title = "Accumulator";
            this.Type = "Operation/Accumulator";

            Inputs[0].Type = DataType.Number;
            Inputs[1].Type = DataType.Number;
            Outputs[0].Type = DataType.Number;

            Inputs[0].Name = "Set Value";
            Inputs[1].Name = "Add Value";
        }
        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            if (input == Inputs[0] && input.Value != null)
            {
                Value = Int32.Parse(input.Value);
                Outputs[0].Value = Value.ToString();
                UpdateMeInDb();
            }

            if (input == Inputs[1] && input.Value != null)
            {
                Value+= Int32.Parse(input.Value);
                Outputs[0].Value = Value.ToString();
                UpdateMeInDb();
            }
        }
    }
}
