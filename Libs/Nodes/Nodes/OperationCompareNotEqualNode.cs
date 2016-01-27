//planer-pro copyright 2015 GPL - license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.Nodes
{

    public class OperationCompareNotEqualNode : Node
    {
        /// <summary>
        /// Compare NotEqual (2 inputs, 1 output).
        /// </summary>
        public OperationCompareNotEqualNode() : base(2, 1)
        {
            this.Title = "Compare NotEqual";
            this.Type = "Operation/Compare NotEqual";

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
                LogInfo("[NULL]");
                Outputs[0].Value = null;
                return;
            }

            try
            {
                Double a = Double.Parse(Inputs[0].Value);
                Double b = Double.Parse(Inputs[1].Value);

                if (a != b)
                {
                    LogInfo($"[{a}] != [{b}]");
                    Outputs[0].Value = "1";
                }
                else
                {
                    LogInfo($"[{a}] = [{b}]");
                    Outputs[0].Value = "0";
                }
            }
            catch
            {
                if (Inputs[0].Value != Inputs[1].Value)
                {
                    LogInfo($"[{Inputs[0].Value}] != [{Inputs[1].Value}]");
                    Outputs[0].Value = "1";
                }
                else
                {
                    LogInfo($"[{Inputs[0].Value}] = [{Inputs[1].Value}]");
                    Outputs[0].Value = "0";
                }
            }
        }
    }
}