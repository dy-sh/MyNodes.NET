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
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {

            try
            {
                if (Inputs[0].Value == null || Inputs[1].Value == null)
                {
                    LogInfo($"Operation/Compare NotEqual: [NULL]");
                    Outputs[0].Value = null;
                }
                else
                {
                    Double a = Double.Parse(Inputs[0].Value);
                    Double b = Double.Parse(Inputs[1].Value);

                    if (a != b)
                    {
                        LogInfo($"Operation/Compare NotEqual: [{a}] != [{b}]");
                        Outputs[0].Value = "1";
                    }
                    else
                    {
                        LogInfo($"Operation/Compare NotEqual: [{a}] = [{b}]");
                        Outputs[0].Value = "0";
                    }
                }
            }
            catch
            {
                if (Inputs[0].Value != Inputs[1].Value)
                {
                    LogInfo($"Operation/Compare NotEqual: [{Inputs[0].Value}] != [{Inputs[1].Value}]");
                    Outputs[0].Value = "1";
                }
                else
                {
                    LogInfo($"Operation/Compare NotEqual: [{Inputs[0].Value}] = [{Inputs[1].Value}]");
                    Outputs[0].Value = "0";
                }
            }
        }
    }
}