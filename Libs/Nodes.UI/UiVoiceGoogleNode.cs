/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
    public class UiVoiceGoogleNode : UiNode
    {
        public string Value { get; set; }

        public UiVoiceGoogleNode() : base(1, 0)
        {
            this.Title = "UI Voice Google";
            this.Type = "UI/Voice Google";
            this.DefaultName = "Voice Google";
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
