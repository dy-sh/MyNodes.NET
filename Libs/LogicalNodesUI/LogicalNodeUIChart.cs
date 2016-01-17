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
        private List<NodeData> NodeData { get; set; }
        public int? State { get; set; }


        public bool WriteInDatabase { get; set; }
        public int WriteInDatabaseMinInterval { get; set; }
        public DateTime WriteInDatabaseLastDate { get; set; }


        public LogicalNodeUIChart() : base(1, 0)
        {
            this.Title = "UI Chart";
            this.Type = "UI/Chart";
            this.Name = "Chart";

            NodeData = new List<NodeData>();
            WriteInDatabase = false;
            WriteInDatabaseMinInterval = 1000;
            WriteInDatabaseLastDate = DateTime.Now;
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
                NodeData.Add(nodeData);

                State = val;
            }
            catch (Exception)
            {
                LogError($"Incorrect input data in UI Chart [{Name}]");
            }
        }

        public List<NodeData> GetChartData()
        {
            return NodeData;
        }

        public void ClearChart()
        {
            NodeData.Clear();
            State = null;
        }
    }
}
