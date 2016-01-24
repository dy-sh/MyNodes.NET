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
    public class MathModulusNode : Node
    {
        /// <summary>
        /// Math Modulus (2 inputs, 1 output).
        /// </summary>
        public MathModulusNode() : base(2, 1)
        {
            this.Title = "Math Modulus";
            this.Type = "Math/Modulus";
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
                    LogInfo($"Math/Modulus: [NULL]");
                    Outputs[0].Value = null;
                }
                else
                {
                    Double a = Double.Parse(Inputs[0].Value);
                    Double b = Double.Parse(Inputs[1].Value);
                    Double c = a % b;

                    LogInfo($"Math/Modulus: [{a}] modulus [{b}] = [{c}]");
                    Outputs[0].Value = c.ToString();
                }
            }
            catch
            {
                LogError($"Math/Modulus: Incorrect value in input");
                Outputs[0].Value = null;
            }
        }
    }
}
