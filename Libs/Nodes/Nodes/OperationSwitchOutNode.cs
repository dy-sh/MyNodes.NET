/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.Nodes
{
    public class OperationSwitchOutNode : Node
    {

        public OperationSwitchOutNode() : base(2, 10)
        {
            this.Title = "Switch out";
            this.Type = "Operation/Switch out";

            Inputs[0].Name = "Switch";
            Inputs[1].Name = "Value";

            Outputs[0].Name = "Out 1";
            Outputs[1].Name = "Out 2";
            Outputs[2].Name = "Out 3";
            Outputs[3].Name = "Out 4";
            Outputs[4].Name = "Out 5";
            Outputs[5].Name = "Out 6";
            Outputs[6].Name = "Out 7";
            Outputs[7].Name = "Out 8";
            Outputs[8].Name = "Out 9";
            Outputs[9].Name = "Out 10";

            Inputs[0].Type = DataType.Number;
            Inputs[1].Type = DataType.Text;

            Outputs[0].Type = DataType.Text;
            Outputs[1].Type = DataType.Text;
            Outputs[2].Type = DataType.Text;
            Outputs[3].Type = DataType.Text;
            Outputs[4].Type = DataType.Text;
            Outputs[5].Type = DataType.Text;
            Outputs[6].Type = DataType.Text;
            Outputs[7].Type = DataType.Text;
            Outputs[8].Type = DataType.Text;
            Outputs[9].Type = DataType.Text;
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            if (Inputs[0].Value != null)
            {
                int sw = Int32.Parse(Inputs[0].Value);

                if (sw > 0 && sw <= 10)
                {
                    foreach (var output in Outputs)
                    {
                        output.Value = null;
                    }

                    Outputs[sw - 1].Value = Inputs[1].Value;
                    LogInfo($"Switched to [{sw}]");
                }
                else
                {
                    foreach (var output in Outputs)
                    {
                        output.Value = null;
                    }
                    LogInfo($"Out of range switch value");
                }
            }
            else
            {
                foreach (var output in Outputs)
                {
                    output.Value = null;
                }
                LogInfo($"No switch control");
            }
        }
    }
}
