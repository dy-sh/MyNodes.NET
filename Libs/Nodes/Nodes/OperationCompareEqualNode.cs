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
        /// <summary>
        /// Compare Equal (2 inputs, 1 output).
        /// </summary>
        public OperationCompareEqualNode() : base(2, 1)
        {
            this.Title = "Compare Equal";
            this.Type = "Operation/Compare Equal";
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {

            try
            {
                Double a = Double.Parse(Inputs[0].Value);
                Double b = Double.Parse(Inputs[1].Value);

                if (a == b)
                {
                    LogInfo($"Operation/Compare Equal: [{a}] = [{b}]");
                    Outputs[0].Value = "1";
                }
                else
                {
                    LogInfo($"Operation/Compare Equal: [{a}] != [{b}]");
                    Outputs[0].Value = "0";
                }
            }
            catch
            {
                if (Inputs[0].Value == Inputs[1].Value)
                {
                    LogInfo($"Operation/Compare Equal: [{Inputs[0].Value}] = [{Inputs[1].Value}]");
                    Outputs[0].Value = "1";
                }
                else
                {
                    LogInfo($"Operation/Compare Equal: [{Inputs[0].Value}] != [{Inputs[1].Value}]");
                    Outputs[0].Value = "0";
                }
            }
        }
    }
}