/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using MyNetSensors.LogicalNodes;

namespace MyNetSensors.LogicalNodesUI
{
    public class LogicalNodeUITextBox : LogicalNodeUI
    {
        public string Value { get; set; }

        public LogicalNodeUITextBox() : base(0, 1)
        {
            this.Title = "UI TextBox";
            this.Type = "UI/TextBox";
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
        }

        public void Send(string value)
        {
            Value = value;
            LogInfo($"UI TextBox [{Name}]: {Value}");
            Outputs[0].Value = Value;
        }
    }
}
