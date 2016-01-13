/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using MyNetSensors.LogicalNodes;

namespace MyNetSensors.LogicalNodesUI
{
  public class LogicalNodeUISwitch : LogicalNodeUI
    {
      public string Value { get; set; }

      public LogicalNodeUISwitch() : base(0, 1)
      {
            this.Title = "Switch";
            this.Type = "UI/Switch";
           Value = "0";
            Outputs[0].Value = Value;
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
        }

        public void Toggle()
        {
            Value = Value == "0" ? "1" : "0";
            LogInfo($"UI Switch [{Name}]: {Value}");
            Outputs[0].Value = Value;
        }
    }
}
