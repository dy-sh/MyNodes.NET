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
        /// <summary>
        /// Math Multiply (2 inputs, 1 output).
        /// </summary>
        public MathMultiplyNode() : base(2, 1)
        {
            this.Title = "Math Multiply";
            this.Type = "Math/Multiply";
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
                    LogInfo($"Math/Multiply: [NULL]");
                    Outputs[0].Value = null;
                }
                else
                {
                    Double a = Double.Parse(Inputs[0].Value);
                    Double b = Double.Parse(Inputs[1].Value);
                    Double c = a * b;

                    LogInfo($"Math/Multiply: [{a}] * [{b}]  = [{c}]");
                    Outputs[0].Value = c.ToString();
                }
            }
            catch
            {
                LogError($"Math/Multiply: Incorrect value in input");
                Outputs[0].Value = null;
            }
        }
    }
}