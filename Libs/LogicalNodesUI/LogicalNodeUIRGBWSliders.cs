/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using MyNetSensors.LogicalNodes;

namespace MyNetSensors.LogicalNodesUI
{
  public class LogicalNodeUIRGBWSliders : LogicalNodeUI
    {
      public string Value { get; set; }

      public LogicalNodeUIRGBWSliders() : base(0, 1)
      {
            this.Title = "UI RGBW Sliders";
            this.Type = "UI/RGBW Sliders";
            this.Name = "RGBW";
            Value = "00000000";
           Outputs[0].Value = Value.ToString();
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
            LogInfo($"UI RGBW Sliders [{Name}]: [{Value}]");
            Outputs[0].Value = Value;
        }
    }
}
