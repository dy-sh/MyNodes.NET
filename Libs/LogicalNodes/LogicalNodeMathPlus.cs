/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.LogicalNodes
{
    public class LogicalNodeMathPlus : LogicalNode
    {
        /// <summary>
        /// Math Plus (2 inputs, 1 output).
        /// </summary>
        public LogicalNodeMathPlus() : base(2, 1)
        {
            this.Title = "Math Plus";
            this.Type = "Math/Plus";
        }

        public override void Loop()
        {
            //  Console.WriteLine( $"MATH LOOP {DateTime.Now} {Inputs[0].Value} {Inputs[1].Value}  {Outputs[0].Value}");
        }

        public override void OnInputChange(Input input)
        {
            int a = 0, b = 0;

            if (Inputs[0].Value != null)
                a = Int32.Parse(Inputs[0].Value);

            if (Inputs[1].Value != null)
                b = Int32.Parse(Inputs[1].Value);

            LogInfo($"MathPlus: {Inputs[0].Value} + {Inputs[1].Value}  = {Outputs[0].Value}");

            Outputs[0].Value = (a + b).ToString();

        }


    }
}
