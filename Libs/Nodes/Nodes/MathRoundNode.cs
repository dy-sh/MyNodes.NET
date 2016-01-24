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
        /// <summary>
        /// Math Round (1 inputs, 1 output).
        /// </summary>
        public MathRoundNode() : base(1, 1)
        {
            this.Title = "Math Round";
            this.Type = "Math/Round";
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {


            try
            {
                if (Inputs[0].Value == null)
                {
                    LogInfo($"Math/Round: [NULL]");
                    Outputs[0].Value = null;
                }
                else
                {
                    Double a = Double.Parse(Inputs[0].Value);
                    int b = (int)Math.Round(a);

                    LogInfo($"Math/Round: [{a}] rounded to [{b}]");
                    Outputs[0].Value = b.ToString();
                }
            }
            catch
            {
                LogError($"Math/Round: Incorrect value in input");
                Outputs[0].Value = null;

            }
        }
    }
}