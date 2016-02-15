/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
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


    }
}
