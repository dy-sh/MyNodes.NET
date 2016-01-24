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
    public class OperationRemap : Node
    {
        /// <summary>
        /// OperationRemap (2 inputs, 1 output).
        /// </summary>
        public OperationRemap() : base(5, 1)
        {
            this.Title = "Remap";
            this.Type = "Operation/Remap";

            Inputs[0].Name = "InMin";
            Inputs[1].Name = "InMax";
            Inputs[2].Name = "Input";
            Inputs[3].Name = "OutMin";
            Inputs[4].Name = "OutMax";
        }

        public override void Loop()
        {
            //  Console.WriteLine( $"MATH LOOP {DateTime.Now} {Inputs[0].Value} {Inputs[1].Value}  {Outputs[0].Value}");
        }

        public override void OnInputChange(Input input)
        {
            try
            {
                if (Inputs[0].Value == null || Inputs[1].Value == null || Inputs[2].Value == null || Inputs[3].Value == null || Inputs[4].Value == null)
                {
                    LogInfo($"Operation/Remap: [NULL]");
                    Outputs[0].Value = null;
                }
                else
                {
                    
                    Double aIn = Double.Parse(Inputs[0].Value);
                    Double bIn = Double.Parse(Inputs[1].Value);
                    Double cOut = Double.Parse(Inputs[3].Value);
                    Double dOut = Double.Parse(Inputs[4].Value);

                    Double signal = Double.Parse(Inputs[2].Value);

                    Double result = (signal - aIn) / (bIn - aIn) * (dOut - cOut) + cOut;
                    LogInfo($"Operation/Remap: {result}");
                    Outputs[0].Value = result.ToString();
                }
            }
            catch
            {
                LogError($"Operation/Remap: Incorrect value in input");
                Outputs[0].Value = null;
            }
        }
    }
}
