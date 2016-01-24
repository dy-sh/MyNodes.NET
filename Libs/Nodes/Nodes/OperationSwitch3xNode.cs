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
    public class OperationSwitch3xNode : Node
    {
        /// <summary>
        /// Switch3x (4 input, 1 output).
        /// </summary>
        public OperationSwitch3xNode() : base(4, 1)
        {
            this.Title = "Switch3x";
            this.Type = "Operation/Switch3x";

            Inputs[0].Name = "Value0";
            Inputs[1].Name = "Value1";
            Inputs[2].Name = "Value2";
            Inputs[3].Name = "Switch value";
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {

            if (Inputs[3].Value == "0")
            {
                LogInfo($"Operation/Switch3x: switched to value0");
                Outputs[0].Value = Inputs[0].Value;
            }
            else if (Inputs[3].Value == "1")
            {
                LogInfo($"Operation/Switch3x: switched to value1");
                Outputs[0].Value = Inputs[1].Value;
            }
            else if (Inputs[3].Value == "2")
            {
                LogInfo($"Operation/Switch3x: switched to value2");
                Outputs[0].Value = Inputs[2].Value;
            }
            else
            {
                LogInfo($"Operation/Switch3x: input value is incorrect");
                Outputs[0].Value = null;
            }
        }
    }
}
