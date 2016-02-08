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
    public class OperationFlipflopNode : Node
    {
        private int part = 0;
        string result = null;

        public OperationFlipflopNode() : base(1, 1)
        {
            this.Title = "Flip-Flop";
            this.Type = "Operation/Flip-Flop";

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

            switch (part)
            {
                case 0:
                    if (Inputs[0].Value == "1")
                    {
                        result = "1";
                        part++;
                    }
                    break;

                case 1:
                    if (Inputs[0].Value == "0")
                    {
                        result = "1";
                        part++;
                    }
                    break;

                case 2:
                    if (Inputs[0].Value == "1")
                    {
                        result = "0";
                        part++;
                    }
                    break;

                case 3:
                    if (Inputs[0].Value == "0")
                    {
                        result = "0";
                        part = 0;
                    }
                    break;
            }

            LogInfo($"[{Inputs[0].Value}] Flip-Flop to [{result}]");
            Outputs[0].Value = result;
        }
    }
}
