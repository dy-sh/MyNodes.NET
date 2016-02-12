//planer-pro copyright 2015 GPL - license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.Nodes
{

    public class MathRoundNode : Node
    {

        public MathRoundNode() : base("Math","Round",1, 1)
        {
            Inputs[0].Type = DataType.Number;
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
            int b = (int)Math.Round(a);

            Outputs[0].Value = b.ToString();
        }
    }
}