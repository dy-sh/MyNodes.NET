/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using MyNetSensors.LogicalNodes;

namespace MyNetSensors.LogicalNodesUI
{
    public class LogicalNodeUIChart : LogicalNodeUI
    {
        private List<NodeData> Log { get; set; }
        public int? State { get; set; }

        public LogicalNodeUIChart() : base(1, 0)
        {
            this.Title = "UI Chart";
            this.Type = "UI/Chart";
            this.Name = "Chart";
            Log = new List<NodeData>();
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            if (input.Value == null)
            {
                State = null;
                return;
            }

            try
            {
                int val = Int32.Parse(input.Value);

                NodeData nodeData = new NodeData(this.Id, val.ToString());
                Log.Add(nodeData);

                State = val;
            }
            catch (Exception)
            {
                LogError($"Incorrect input data in UI Chart [{Name}]");
            }
        }


        public void ClearLog()
        {
            Log.Clear();
            State = null;
        }
    }
}
