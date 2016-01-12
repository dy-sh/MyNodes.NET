/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using MyNetSensors.LogicalNodes;

namespace MyNetSensors.LogicalNodesUI
{
  public class LogicalNodeUISwitchButton : LogicalNodeUI
    {
      public string Value { get; set; }

      public LogicalNodeUISwitchButton() : base(1, 0)
      {
            this.Title = "Switch Button";
            this.Type = "UI/Switch Button";
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            Value = input.Value;
        }


    }
}
