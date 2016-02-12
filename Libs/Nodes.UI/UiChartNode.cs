/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MyNetSensors.Nodes
{
    public class UiChartNode : UiNode
    {

        public int? State { get; set; }

        public DateTime WriteInDatabaseLastDate { get; set; }


        private List<NodeState> NodeStates { get; set; }
        private string LastStateCached { get; set; }
        private bool LastStateUpdated { get; set; }


        public UiChartNode() : base("Chart", 1, 0)
        {
            NodeStates = new List<NodeState>();
            WriteInDatabaseLastDate = DateTime.Now;

            Settings.Add("WriteInDatabase", new NodeSetting(NodeSettingType.Checkbox, "Write In Database", "false"));
            Settings.Add("UpdateInterval", new NodeSetting(NodeSettingType.Number, "Update Interval", "500"));
        }

        public override void Loop()
        {
            if (!LastStateUpdated)
                return;

            int updateInteval = Int32.Parse(Settings["UpdateInterval"].Value);

            if ((DateTime.Now - WriteInDatabaseLastDate).TotalMilliseconds < updateInteval)
                return;

            LastStateUpdated = false;
            WriteInDatabaseLastDate = DateTime.Now;

            if (LastStateCached == null)
            {
                State = null;
                // UpdateMe();
                return;
            }

            try
            {
                int val = Int32.Parse(LastStateCached);
                NodeStates.Add(new NodeState(Id, val.ToString()));
                State = val;
                UpdateMe();
            }
            catch (Exception)
            {
                State = null;
                LogError($"Incorrect input value.");
            }
        }

        public override void OnInputChange(Input input)
        {
            LastStateCached = input.Value;
            LastStateUpdated = true;
        }

        public List<NodeState> GetStates()
        {
            return NodeStates;
        }

        public void SetStates(List<NodeState> states)
        {
            NodeStates = states ?? new List<NodeState>();
            LastStateUpdated = false;
        }

        public void RemoveStates()
        {
            NodeStates.Clear();
            State = null;
            LastStateUpdated = false;
        }

        public override string GetValue(string name)
        {
            List<NodeState> nodeStates = GetStates();

            if (nodeStates == null || !nodeStates.Any())
                return null;

            //copy to array to prevent changing data error
            NodeState[] nodeStatesArray = new NodeState[nodeStates.Count];
            nodeStates.CopyTo(nodeStatesArray);

            List<ChartData> chartData = nodeStatesArray.Select(item => new ChartData
            {
                x = $"{item.DateTime:yyyy-MM-dd HH:mm:ss.fff}",
                y = item.State == "0" ? "-0.01" : item.State
            }).ToList();

            return JsonConvert.SerializeObject(chartData);
        }
    }
}
