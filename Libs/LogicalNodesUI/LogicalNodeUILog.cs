/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using MyNetSensors.LogicalNodes;

namespace MyNetSensors.LogicalNodesUI
{
  public class LogicalNodeUILog : LogicalNodeUI
    {
      public string Log { get; set; }

      public LogicalNodeUILog() : base(1, 0)
      {
            this.Title = "UI Log";
            this.Type = "UI/Log";
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            Log += $"{DateTime.Now}: {input.Value}<br/>";
        }


      public void ClearLog()
      {
          Log = "";
      }
    }
}
