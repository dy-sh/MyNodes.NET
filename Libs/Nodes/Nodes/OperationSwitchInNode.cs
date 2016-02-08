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
    public class OperationSwitchInNode : Node
    {

        public OperationSwitchInNode() : base(11, 1)
        {
            this.Title = "Switch in";
            this.Type = "Operation/Switch in";

            Inputs[0].Name = "Switch";
            Inputs[1].Name = "Value1";
            Inputs[2].Name = "Value2";
            Inputs[3].Name = "Value3";
            Inputs[4].Name = "Value4";
            Inputs[5].Name = "Value5";
            Inputs[6].Name = "Value6";
            Inputs[7].Name = "Value7";
            Inputs[8].Name = "Value8";
            Inputs[9].Name = "Value9";
            Inputs[10].Name = "Value10";

            Inputs[0].Type = DataType.Number;
            Inputs[1].Type = DataType.Text;
            Inputs[2].Type = DataType.Text;
            Inputs[3].Type = DataType.Text;
            Inputs[4].Type = DataType.Text;
            Inputs[5].Type = DataType.Text;
            Inputs[6].Type = DataType.Text;
            Inputs[7].Type = DataType.Text;
            Inputs[8].Type = DataType.Text;
            Inputs[9].Type = DataType.Text;
            Inputs[10].Type = DataType.Text;

            Outputs[0].Type = DataType.Text;
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
                    Outputs[0].Value = Inputs[sw].Value;
                    LogInfo($"Switched to [{sw}]");
                }
                else
                {
                    Outputs[0].Value = null;
                    LogInfo($"Out of range switch value");
                }
            }
            else
            {
                Outputs[0].Value = null;
                LogInfo($"No switch control");
            }
        }
    }
}
