/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNodes.Nodes
{
    public class UiVoiceGoogleNode : UiNode
    {
        public string Value { get; set; }

        public UiVoiceGoogleNode() : base("UI", "Voice Google")
        {
            AddInput();
        }


        public override void OnInputChange(Input input)
        {
            Value = input.Value;
            UpdateMe();
        }

        public override string GetNodeDescription()
        {
            return "This is a UI node. It can generate speech from the incoming text. <br/>" +
                   "As the TTS engine is used built-in Google Chrome TTS, " +
                   "so this node will work in this browser only. <br/>" +
                   "You'll hear the voice if you have opened the panel on the dashboard, " +
                   "in which there is this node.";
        }
    }
}
