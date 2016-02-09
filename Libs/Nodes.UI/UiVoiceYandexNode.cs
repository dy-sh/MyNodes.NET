/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
    public class UiVoiceYandexNode : UiNode
    {
        public string Value { get; set; }

        public UiVoiceYandexNode() : base(1, 0)
        {
            this.Title = "UI Voice Yandex";
            this.Type = "UI/Voice Yandex";
            this.DefaultName = "Voice Yandex";

            Settings.Add("APIKey",new NodeSetting(NodeSettingType.Text, "Yandex SpeechKit API-Key",""));
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
