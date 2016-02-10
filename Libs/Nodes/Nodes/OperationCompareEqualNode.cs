//planer-pro copyright 2015 GPL - license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.Nodes
{

    public class OperationCompareEqualNode : Node
    {

        public OperationCompareEqualNode() : base(2, 1)
        {
            this.Title = "Compare Equal";
            this.Type = "Operation/Compare Equal";

            Inputs[0].Type = DataType.Text;
            Inputs[1].Type = DataType.Text;
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

            try
            {
                Double a = Double.Parse(Inputs[0].Value);
                Double b = Double.Parse(Inputs[1].Value);

                Outputs[0].Value = a == b ? "1" : "0";
            }
            catch
            {
                Outputs[0].Value = Inputs[0].Value == Inputs[1].Value ? "1" : "0";
            }
        }
    }
}