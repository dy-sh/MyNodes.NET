//planer-pro copyright 2015 GPL - license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.Nodes
{

    public class OperationxFader : Node
    {
        /// <summary>
        /// Math Minus (2 inputs, 1 output).
        /// </summary>
        public OperationxFader() : base(3, 1)
        {
            this.Title = "xFader";
            this.Type = "Operation/xFader";

            Inputs[0].Type = DataType.Number;
            Inputs[1].Type = DataType.Number;
            Inputs[2].Type = DataType.Number;
            Outputs[0].Type = DataType.Number;

            Inputs[0].Name = "xFader";
            Inputs[1].Name = "in 1";
            Inputs[2].Name = "in 2";

        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            if (Inputs.Any(i => i.Value == null))
            {
                LogInfo("[NULL]");
                Outputs[0].Value = null;
                return;
            }

            Double xf = Double.Parse(Inputs[0].Value);
            Double a = Double.Parse(Inputs[1].Value);
            Double b = Double.Parse(Inputs[2].Value);

            Double xout = a * (1 - xf / 100) + b * xf / 100;

            LogInfo($"[{xout}]");
            Outputs[0].Value = xout.ToString();
        }
    }
}