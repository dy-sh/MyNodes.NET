/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
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

        public double? State { get; set; }

        public DateTime LastUpdateDate { get; set; }


        private List<NodeState> NodeStates { get; set; }
        private string LastStateCached { get; set; }
        private bool LastStateUpdated { get; set; }


        public UiChartNode() : base("UI", "Chart")
        {
            AddInput();

            NodeStates = new List<NodeState>();
            LastUpdateDate = DateTime.Now;

            Settings.Add("WriteInDatabase", new NodeSetting(NodeSettingType.Checkbox, "Write In Database", "false"));
            Settings.Add("UpdateInterval", new NodeSetting(NodeSettingType.Number, "Update Interval", "500"));
        }

        public override void Loop()
        {
            if (!LastStateUpdated)
                return;

            int updateInteval = Int32.Parse(Settings["UpdateInterval"].Value);

            if ((DateTime.Now - LastUpdateDate).TotalMilliseconds < updateInteval)
                return;

            LastStateUpdated = false;
            LastUpdateDate = DateTime.Now;

            if (LastStateCached == null)
            {
                State = null;
                // UpdateMe();
                return;
            }

            try
            {
                double val = double.Parse(LastStateCached);
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

        public override string GetNodeDescription()
        {
            return "This is a UI node. It displays a chart on the dashboard. <br/>" +
                   "The chart shows the history of values, " +
                   "and updated in real time. You can view the chart in various styles. <br/>" +
                   "If you want to show someone some range of the history on the chart, " +
                   "then press the Share button and a link will be generated at exactly " +
                   "that moment that you see now. The chart style will be included. <br/>" +
                   "If you enable Autosroll, then the chart will automatically move " +
                   "to the current time. It is convenient to observe changes in real time. <br/>" +
                   "You can also share a link to real-time style of chart. <br/>" +
                   "The chart scale will also be included in the link. ";
        }
    }
}
