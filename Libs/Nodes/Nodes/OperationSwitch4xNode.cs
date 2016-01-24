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
    public class OperationSwitch4xNode : Node
    {
        /// <summary>
        /// Switch4x (5 input, 1 output).
        /// </summary>
        public OperationSwitch4xNode() : base(5, 1)
        {
            this.Title = "Switch4x";
            this.Type = "Operation/Switch4x";

            Inputs[0].Name = "Value0";
            Inputs[1].Name = "Value1";
            Inputs[2].Name = "Value2";
            Inputs[3].Name = "Value3";
            Inputs[4].Name = "Switch value";
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {

            if (Inputs[4].Value == "0")
            {
                LogInfo($"Operation/Switch4x: switched to value0");
                Outputs[0].Value = Inputs[0].Value;
            }
            else if (Inputs[4].Value == "1")
            {
                LogInfo($"Operation/Switch4x: switched to value1");
                Outputs[0].Value = Inputs[1].Value;
            }
            else if (Inputs[4].Value == "2")
            {
                LogInfo($"Operation/Switch4x: switched to value2");
                Outputs[0].Value = Inputs[2].Value;
            }
            else if (Inputs[4].Value == "3")
            {
                LogInfo($"Operation/Switch4x: switched to value3");
                Outputs[0].Value = Inputs[3].Value;
            }
            else
            {
                LogInfo($"Operation/Switch4x: input value is incorrect");
                Outputs[0].Value = null;
            }
        }
    }
}
