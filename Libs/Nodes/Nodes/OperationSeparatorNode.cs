using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.Nodes.Nodes
{
    public class OperationSeparatorNode : Node
    {

        public OperationSeparatorNode() : base(2, 2)
        {
            this.Title = "Separator";
            this.Type = "Operation/Separator";

            Inputs[0].Type = DataType.Number;
            Inputs[1].Type = DataType.Number;
            Outputs[0].Type = DataType.Number;
            Outputs[0].Type = DataType.Number;

            Inputs[0].Name = "Treshold";
            Inputs[1].Name = "Value";
            Outputs[0].Name = "Hi";
            Outputs[1].Name = "Lo";
        }
        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            if (Inputs.Any(i => i.Value == null))
            {
                ResetOutputs();
                return;
            }

            if (input == Inputs[1])
            {
                double threshold = double.Parse(Inputs[0].Value);
                double val = double.Parse(Inputs[1].Value);

                if (val >= threshold)
                    Outputs[0].Value = val.ToString();
                else
                    Outputs[1].Value = val.ToString();
            }
        }
    }
}
