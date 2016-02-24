/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
    public class UiVoiceYandexNode : UiNode
    {
        public string Value { get; set; }

        public UiVoiceYandexNode() : base("UI", "Voice Yandex")
        {
            AddInput();
            Settings.Add("APIKey",new NodeSetting(NodeSettingType.Text, "Yandex SpeechKit API-Key",""));
        }


        public override void OnInputChange(Input input)
        {
            Value = input.Value;
            UpdateMe();
        }

        public override string GetNodeDescription()
        {
            return "This is a UI node. It can generate speech from the incoming text. " +
                   "As the TTS engine is used Yandex SpeachKit TTS, " +
                   "so you need to enter Yandex SpeechKit API-Key in the settings of the node, " +
                   "which you can get free on the Yandex website, registering as a developer. " +
                   "You'll hear the voice if you have opened the panel on the dashboard, " +
                   "in which there is this node.";
        }
    }
}
