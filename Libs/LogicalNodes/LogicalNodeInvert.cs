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
  public class LogicalNodeInvert : LogicalNode
    {
        /// <summary>
        /// Math Plus (2 inputs, 1 output).
        /// </summary>
        public LogicalNodeInvert() : base(1, 1){}

        public override void Loop()
        {
          //  Console.WriteLine( $"MATH LOOP {DateTime.Now} {Inputs[0].Value} {Inputs[1].Value}  {Outputs[0].Value}");
        }

        public override void OnInputChange(Input input)
        {
            if (Inputs[0].Value == "0")
                Outputs[0].Value = "1";
            else
                Outputs[0].Value = "0";


          //  Console.WriteLine($"MATH OUT {DateTime.Now} {Inputs[0].Value} {Inputs[1].Value}  {Outputs[0].Value}");
        }


    }
}
