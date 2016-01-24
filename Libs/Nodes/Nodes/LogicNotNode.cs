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
  public class LogicNotNode : Node
    {
        /// <summary>
        /// Not (1 input, 1 output).
        /// </summary>
        public LogicNotNode() : base(1, 1)
      {
            this.Title = "Logic NOT";
            this.Type = "Logic/NOT";
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            if (Inputs[0].Value == null)
            {
                LogInfo($"Logic/NOT: [NULL]");
                Outputs[0].Value = null;

                return;
            }

            if (Inputs[0].Value != "0" && Inputs[0].Value != "1")
            {
                LogError($"Logic/NOT: Incorrect value in input");
                Outputs[0].Value = null;

                return;
            }
            string result;

            if (Inputs[0].Value == "0")
                result = "1";
            else
                result = "0";

            LogInfo($"Logic/NOT: NOT [{Inputs[0].Value}] = [{result}]");

            Outputs[0].Value = result;
        }
    }
}
