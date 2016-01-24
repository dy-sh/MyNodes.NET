/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.Nodes
{
    public class MathPowNode : Node
    {
        /// <summary>
        /// Math Pow (2 inputs, 1 output).
        /// </summary>
        public MathPowNode() : base(2, 1)
        {
            this.Title = "Math Pow";
            this.Type = "Math/Pow";
        }

        public override void Loop()
        {
            //  Console.WriteLine( $"MATH LOOP {DateTime.Now} {Inputs[0].Value} {Inputs[1].Value}  {Outputs[0].Value}");
        }

        public override void OnInputChange(Input input)
        {
            try
            {
                if (Inputs[0].Value == null || Inputs[1].Value == null)
                {
                    LogInfo($"Math/Pow: [NULL]");
                    Outputs[0].Value = null;
                }
                else
                {
                    Double a = Double.Parse(Inputs[0].Value);
                    Double b = Double.Parse(Inputs[1].Value);
                    Double c = Math.Pow(a, b);

                    LogInfo($"Math/Pow: Pow [{a}] to [{b}]");
                    Outputs[0].Value = c.ToString();
                }
            }
            catch
            {
                LogError($"Math/Pow: Incorrect value in input");
                Outputs[0].Value = null;
            }
        }
    }
}
