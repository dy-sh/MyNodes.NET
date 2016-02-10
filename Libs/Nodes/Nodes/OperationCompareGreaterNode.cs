//planer-pro copyright 2015 GPL - license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.Nodes
{

    public class OperationCompareGreaterNode : Node
    {

        public OperationCompareGreaterNode() : base(2, 1)
        {
            this.Title = "Compare Greater";
            this.Type = "Operation/Compare Greater";

            Inputs[0].Type = DataType.Number;
            Inputs[1].Type = DataType.Number;
            Outputs[0].Type = DataType.Logical;
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

            Outputs[0].Value = a > b ? "1" : "0";
        }
    }
}