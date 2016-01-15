/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using MyNetSensors.LogicalNodes;

namespace MyNetSensors.LogicalNodesUI
{
  public class LogicalNodeUIProgress : LogicalNodeUI
    {
      public string Value { get; set; }

      public LogicalNodeUIProgress() : base(1, 0)
      {
            this.Title = "UI Progress";
            this.Type = "UI/Progress";
            this.Name = "Progress";
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
