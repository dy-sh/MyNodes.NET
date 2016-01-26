/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.Nodes
{
    public class MathClamp : Node
    {
        /// <summary>
        /// MathClamp (3 inputs, 1 output).
        /// </summary>
        public MathClamp() : base(3, 1)
        {
            this.Title = "Clamp";
            this.Type = "Math/Clamp";

            Inputs[0].Name = "In";
            Inputs[1].Name = "InMin";
            Inputs[2].Name = "InMax";  
        }

        public override void Loop()
        {
            //  Console.WriteLine( $"MATH LOOP {DateTime.Now} {Inputs[0].Value} {Inputs[1].Value}  {Outputs[0].Value}");
        }

        public override void OnInputChange(Input input)
        {
            try
            {
                if (Inputs[0].Value == null || Inputs[1].Value == null || Inputs[2].Value == null)
                {
                    LogInfo($"Math/Clamp: [NULL]");
                    Outputs[0].Value = null;
                }
                else
                {
                    Double signal = Double.Parse(Inputs[0].Value);
                    Double aIn = Double.Parse(Inputs[1].Value);
                    Double bIn = Double.Parse(Inputs[2].Value);

                    Double result;

                    if (signal >= aIn && signal <= bIn)
                    {
                        result = signal;
                        LogInfo($"Math/Clamp: in range");
                        Outputs[0].Value = result.ToString();
                    }
                    else
                    {
                        LogInfo($"Math/Clamp: no in range");
                        Outputs[0].Value = null;
                    }
                    //Double result = (signal < aIn) ? aIn : (signal > bIn) ? bIn : signal; 
                }
            }
            catch
            {
                LogError($"Math/Clamp: Incorrect value in input");
                Outputs[0].Value = null;
            }
        }
    }
}
