/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
    public class UiButtonNode : UiNode
    {
        public UiButtonNode() : base(0, 1)
        {
            this.Title = "UI Button";
            this.Type = "UI/Button";
            this.Name = "Button";
            Outputs[0].Value = "0";
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
        }

        public void Click()
        {
            LogInfo($"UI Button [{Name}]: [1]");
            Outputs[0].Value = "1";
            LogInfo($"UI Button [{Name}]: [0]");
            Outputs[0].Value = "0";
        }
    }
}
