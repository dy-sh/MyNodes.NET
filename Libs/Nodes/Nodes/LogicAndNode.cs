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
    public class LogicAndNode : Node
    {
        /// <summary>
        /// Not (2 input, 1 output).
        /// </summary>
        public LogicAndNode() : base(2, 1)
        {
            this.Title = "Logic AND";
            this.Type = "Logic/AND";

            Inputs[0].Type = DataType.Logical;
            Inputs[1].Type = DataType.Logical;
            Outputs[0].Type = DataType.Logical;
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

            string result = "0";

            if (Inputs[0].Value == "1" && Inputs[1].Value == "1")
                result = "1";

            LogInfo($"[{Inputs[0].Value}] AND [{Inputs[1].Value}] = [{result}]");

            Outputs[0].Value = result;
        }
    }
}
