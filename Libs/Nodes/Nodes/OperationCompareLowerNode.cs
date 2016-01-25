//planer-pro copyright 2015 GPL - license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.Nodes
{

    public class OperationCompareLowerNode : Node
    {
        /// <summary>
        /// Compare Lower (2 inputs, 1 output).
        /// </summary>
        public OperationCompareLowerNode() : base(2, 1)
        {
            this.Title = "Compare Lower";
            this.Type = "Operation/Compare Lower";
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
                    LogInfo($"Operation/Compare Lower: [NULL]");
                    Outputs[0].Value = null;
                }
                else
                {
                    Double a = Double.Parse(Inputs[0].Value);
                    Double b = Double.Parse(Inputs[1].Value);

                    if (a < b)
                    {
                        LogInfo($"Operation/Compare Lower: [{a}] < [{b}]");
                        Outputs[0].Value = "1";
                    }
                    else
                    {
                        LogInfo($"Operation/Compare Lower: [{a}] > [{b}]");
                        Outputs[0].Value = "0";
                    }
                }
            }
            catch
            {
                LogError($"Operation/Compare Lower: Incorrect value in input");
                Outputs[0].Value = null;
            }
        }
    }
}