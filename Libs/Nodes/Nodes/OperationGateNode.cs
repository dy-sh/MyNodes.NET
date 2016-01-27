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

            Inputs[0].Type = DataType.Text;
            Inputs[1].Type = DataType.Logical;
            Outputs[0].Type = DataType.Text;
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            if (Inputs.Any(i => i.Value == null))
            {
                LogInfo("[NULL]");
                Outputs[0].Value = null;
                return;
            }

            if (Inputs[1].Value == "1")
            {
                LogInfo($"[{Inputs[0].Value}]");
                Outputs[0].Value = Inputs[0].Value;
            }
            else
            {
                Outputs[0].Value = null;
                LogInfo($"[NULL]");
            }
        }
    }
}
