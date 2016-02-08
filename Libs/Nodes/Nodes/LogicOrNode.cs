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

        public LogicOrNode() : base(2, 1)
        {
            this.Title = "OR";
            this.Type = "Logic/OR";

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

            string result = "1";

            if (Inputs[0].Value == "0" && Inputs[1].Value == "0")
                result = "0";

            LogInfo($"[{Inputs[0].Value}] OR [{Inputs[1].Value}] = [{result}]");

            Outputs[0].Value = result;
        }
    }
}
