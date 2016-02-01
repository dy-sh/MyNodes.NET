/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
    public class UiSpeechNode : UiNode
    {
        public string Value { get; set; }

        public UiSpeechNode() : base(1, 0)
        {
            this.Title = "UI Speech";
            this.Type = "UI/Speech";
            this.DefaultName = "Speech";
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            Value = input.Value;
            UpdateMe();
        }


    }
}
