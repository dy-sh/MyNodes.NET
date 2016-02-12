/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
    public class UiTextBoxNode : UiNode
    {
        public string Value { get; set; }

        public UiTextBoxNode() : base("TextBox",0, 1)
        {
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
            Outputs[0].Value = Value;

            UpdateMe();
            UpdateMeInDb();
        }
    }
}
