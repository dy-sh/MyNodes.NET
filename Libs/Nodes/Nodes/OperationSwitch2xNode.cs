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
    public class OperationSwitch2xNode : Node
    {
        /// <summary>
        /// Switch2x (3 input, 1 output).
        /// </summary>
        public OperationSwitch2xNode() : base(3, 1)
        {
            this.Title = "Switch2x";
            this.Type = "Operation/Switch2x";

            Inputs[0].Name = "Value0";
            Inputs[1].Name = "Value1";
            Inputs[2].Name = "Switch value";
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {

            if (Inputs[2].Value == "0")
            {
                LogInfo($"Switched to value0");
                Outputs[0].Value = Inputs[0].Value;
            }
            else if (Inputs[2].Value == "1")
            {
                LogInfo($"Switched to value1");
                Outputs[0].Value = Inputs[1].Value;
            }
            else
            {
                LogInfo($"Input value is incorrect");
                Outputs[0].Value = null;
            }
        }
    }
}
