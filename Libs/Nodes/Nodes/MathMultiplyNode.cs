//planer-pro copyright 2015 GPL - license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.Nodes
{

    public class MathMultiplyNode : Node
    {

        public MathMultiplyNode() : base(2, 1)
        {
            this.Title = "Multiply";
            this.Type = "Math/Multiply";

            Inputs[0].Type = DataType.Number;
            Inputs[1].Type = DataType.Number;
            Outputs[0].Type = DataType.Number;
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

            Double a = Double.Parse(Inputs[0].Value);
            Double b = Double.Parse(Inputs[1].Value);
            Double c = a * b;

            Outputs[0].Value = c.ToString();
        }
    }
}