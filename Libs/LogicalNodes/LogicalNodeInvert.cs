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
        /// Invert (1 input, 1 output).
        /// </summary>
        public LogicalNodeInvert() : base(1, 1){}

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {

            string result;

            if (Inputs[0].Value == "0")
                result = "1";
            else
                result = "0";

            Debug($"Invert: from {Inputs[0].Value} to {result}");

            Outputs[0].Value = result;
        }


    }
}
