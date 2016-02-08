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

        public LogicNotNode() : base(1, 1)
        {
            this.Title = "NOT";
            this.Type = "Logic/NOT";

            Inputs[0].Type = DataType.Logical;
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

            string result;

            if (Inputs[0].Value == "0")
                result = "1";
            else
                result = "0";

            LogInfo($"NOT [{Inputs[0].Value}] = [{result}]");

            Outputs[0].Value = result;
        }
    }
}
