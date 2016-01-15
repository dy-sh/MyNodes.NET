/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using MyNetSensors.LogicalNodes;

namespace MyNetSensors.LogicalNodesUI
{
  public class LogicalNodeUISlider : LogicalNodeUI
    {
      public int Value { get; set; }
      public int Min { get; set; }
      public int Max { get; set; }

        public LogicalNodeUISlider() : base(0, 1)
      {
            this.Title = "UI Slider";
            this.Type = "UI/Slider";
            this.Name = "Slider";

            Value = 0;
            Min = 0;
            Max = 100;

           Outputs[0].Value = Value.ToString();
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
        }

        public void SetValue(int value)
        {
            Value = value;
            LogInfo($"UI Slider [{Name}]: [{Value}]");
            Outputs[0].Value = Value.ToString();
        }
    }
}
