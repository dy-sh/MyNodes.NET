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
      public LogicalNodeInvert() : base(1, 1)
      {
            this.Title = "Logic Invert";
            this.Type = "Logic/Invert";
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {

            string result;

            if (Inputs[0].Value == "0")
                result = "1";
            else if (Inputs[0].Value == "1")
                result = "0";
            else 
                result = null;

            LogInfo($"Invert: from [{Inputs[0].Value ?? "NULL"}] to [{result ?? "NULL"}]");


            Outputs[0].Value = result;
        }


    }
}
