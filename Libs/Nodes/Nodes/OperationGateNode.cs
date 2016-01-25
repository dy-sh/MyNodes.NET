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
    public class OperationGateNode : Node
    {
        /// <summary>
        /// Gate (2 input, 1 output).
        /// </summary>
        public OperationGateNode() : base(2, 1)
        {
            this.Title = "Gate";
            this.Type = "Operation/Gate";

            Inputs[0].Name = "Value";
            Inputs[1].Name = "Key";
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            if (Inputs[0].Value == null || Inputs[1].Value == null)
            {
                LogInfo($"Operation/Gate: [NULL]");
                Outputs[0].Value = null;
            }
            else if (Inputs[1].Value == "1")
            {
                LogInfo($"Operation/Gate: send [{Inputs[0].Value}] to output");
                Outputs[0].Value = Inputs[0].Value;
            }
            else if (Inputs[1].Value == "0")
            {
                Outputs[0].Value = null;
                LogInfo($"Operation/Gate: gate closed");
            }
            else
            {
                LogError($"Operation/Gate: Incorrect value in input");
                Outputs[0].Value = null;
            }
        }
    }
}
