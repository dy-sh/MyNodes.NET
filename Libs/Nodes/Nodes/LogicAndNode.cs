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

        public LogicAndNode() : base("Logic", "AND", 2, 1)
        {
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
                ResetOutputs();
                return;
            }

            string result = "0";

            if (Inputs[0].Value == "1" && Inputs[1].Value == "1")
                result = "1";

            Outputs[0].Value = result;
        }
    }
}
