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
    public class LogicOrNode : Node
    {
        /// <summary>
        /// Or (2 input, 1 output).
        /// </summary>
        public LogicOrNode() : base(2, 1)
        {
            this.Title = "Logic OR";
            this.Type = "Logic/OR";
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            if (Inputs[0].Value == null || Inputs[1].Value == null)
            {
                LogInfo($"Logic/OR: [NULL]");
                Outputs[0].Value = null;

                return;
            }

            if ((Inputs[0].Value != "0" && Inputs[0].Value != "1") ||
                (Inputs[1].Value != "0" && Inputs[1].Value != "1"))
            {
                LogError($"Logic/OR: Incorrect value in input");
                Outputs[0].Value = null;

                return;
            }

            string result = "1";

            if (Inputs[0].Value == "0" && Inputs[1].Value == "0")
                result = "0";

            LogInfo($"Logic/OR: [{Inputs[0].Value}] OR [{Inputs[1].Value}] = [{result}]");

            Outputs[0].Value = result;
        }
    }
}
