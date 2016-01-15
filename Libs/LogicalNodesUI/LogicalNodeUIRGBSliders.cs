/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using MyNetSensors.LogicalNodes;

namespace MyNetSensors.LogicalNodesUI
{
  public class LogicalNodeUIRGBSliders : LogicalNodeUI
    {
      public string Value { get; set; }

      public LogicalNodeUIRGBSliders() : base(0, 1)
      {
            this.Title = "UI RGB Sliders";
            this.Type = "UI/RGB Sliders";
            this.Name = "RGB";
            Value = "000000";
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
            LogInfo($"UI RGB Sliders [{Name}]: [{Value}]");
            Outputs[0].Value = Value;
        }
    }
}
