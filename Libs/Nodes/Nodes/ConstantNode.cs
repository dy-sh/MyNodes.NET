/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
    public class ConstantNode : Node
    {
        public string Value { get; set; }

        public ConstantNode() : base(0,1)
        {
            this.Title = "Constant";
            this.Type = "Basic/Constant";
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
        }

        public void SetValue(string value)
        {
            Value = value;
            LogInfo($"[{Value??"NULL"}]");
            Outputs[0].Value = Value;
            UpdateMeInDb();
        }
    }
}
