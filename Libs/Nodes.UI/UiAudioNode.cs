/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
    public class UiAudioNode : UiNode
    {
        public string Address { get; set; }
        public bool Play { get; set; }

        public UiAudioNode() : base(2, 0)
        {
            this.Title = "UI Audio";
            this.Type = "UI/Audio";
            this.DefaultName = "Audio";

            Inputs[0].Name = "Audio URL";
            Inputs[1].Name = "Play";
            Inputs[0].Type = DataType.Text;
            Inputs[1].Type = DataType.Logical;
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            Address = Inputs[0].Value;
            Play = Inputs[1].Value=="1";
            UpdateMe();
        }


    }
}
