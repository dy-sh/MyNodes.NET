/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
    public class UiTextBoxNode : UiNode
    {
        public string Value { get; set; }

        public UiTextBoxNode() : base(0, 1)
        {
            this.Title = "UI TextBox";
            this.Type = "UI/TextBox";
            this.DefaultName = "TextBox";
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
            LogInfo($"UI TextBox [{Name}]: [{Value??"NULL"}]");
            Outputs[0].Value = Value;
        }
    }
}
