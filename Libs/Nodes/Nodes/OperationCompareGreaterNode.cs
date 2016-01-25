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
        /// <summary>
        /// Compare Greater (2 inputs, 1 output).
        /// </summary>
        public OperationCompareGreaterNode() : base(2, 1)
        {
            this.Title = "Compare Greater";
            this.Type = "Operation/Compare Greater";
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
                    LogInfo($"Operation/Compare Greater: [NULL]");
                    Outputs[0].Value = null;
                }
                else
                {
                    Double a = Double.Parse(Inputs[0].Value);
                    Double b = Double.Parse(Inputs[1].Value);

                    if (a > b)
                    {
                        LogInfo($"Operation/Compare Greater: [{a}] > [{b}]");
                        Outputs[0].Value = "1";
                    }
                    else
                    {
                        LogInfo($"Operation/Compare Greater: [{a}] < [{b}]");
                        Outputs[0].Value = "0";
                    }
                }
            }
            catch
            {
                LogError($"Operation/Compare Greater: Incorrect value in input");
                Outputs[0].Value = null;
            }
        }
    }
}