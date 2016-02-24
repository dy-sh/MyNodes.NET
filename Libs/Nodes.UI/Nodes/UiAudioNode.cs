/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
    public class UiAudioNode : UiNode
    {
        public string Address { get; set; }
        public bool Play { get; set; }

        public UiAudioNode() : base("UI", "Audio")
        {
            AddInput("Audio URL");
            AddInput("Play", DataType.Logical);
        }


        public override void OnInputChange(Input input)
        {
            Address = Inputs[0].Value;
            Play = Inputs[1].Value == "1";
            UpdateMe();
        }

        public override string GetNodeDescription()
        {
            return "This is a UI node. It can play any audio files on the dashboard.";
        }
    }
}
