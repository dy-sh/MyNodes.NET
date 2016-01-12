/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using MyNetSensors.LogicalNodes;

namespace MyNetSensors.LogicalNodesUI
{
  public class LogicalNodeUILabel : LogicalNodeUI
    {
      public string Value { get; set; }

      /// <summary>
      /// Invert (1 input, 1 output).
      /// </summary>
      public LogicalNodeUILabel() : base(1, 0)
      {
            this.Title = "UI Label";
            this.Type = "UI/Label";
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
