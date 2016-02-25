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
        public ChartData LastRecord { get; set; }

        private List<NodeState> log;

        public UiChartNode() : base("UI", "Chart")
        {
            AddInput(DataType.Number);

            log = new List<NodeState>();

            Settings.Add("WriteInDatabase", new NodeSetting(NodeSettingType.Checkbox, "Write in database", "false"));
            Settings.Add("MaxRecords", new NodeSetting(NodeSettingType.Number, "The maximum number of chart points", "100"));
        }


        public override void OnInputChange(Input input)
        {
            if (input.Value == null)
                return;

            int max = (int)double.Parse(Settings["MaxRecords"].Value);
            if (max < 0)
                max = 0;

            LastRecord = new ChartData(DateTime.Now, input.Value);
            log.Add(new NodeState(Id, input.Value));

            while (log.Count > max)
                log.Remove(log.First());

            UpdateMe();

            if (Settings["WriteInDatabase"].Value == "true")
                uiEngine?.statesDb?.AddState(new NodeState(Id, input.Value), max);
        }



        private void RemoveStates()
        {
            log.Clear();
            uiEngine?.statesDb?.RemoveStatesForNode(Id);
        }

        public override string GetValue(string name)
        {
            if (log == null || !log.Any())
                return null;

            //copy to array to prevent changing data error
            NodeState[] nodeStatesArray = new NodeState[log.Count];
            log.CopyTo(nodeStatesArray);

            List<ChartData> chartData = nodeStatesArray.Select(
                state => new ChartData(state.DateTime, state.State)).ToList();

            return JsonConvert.SerializeObject(chartData);
        }


        public override void OnAddToUiEngine(UiNodesEngine uiEngine)
        {
            base.OnAddToUiEngine(uiEngine);
            GetStatesFromRepository();
        }

        private void GetStatesFromRepository()
        {
            if (uiEngine?.statesDb == null || Settings["WriteInDatabase"].Value != "true")
                return;

            List<NodeState> states = uiEngine?.statesDb?.GetStatesForNode(Id);
            log = states ?? new List<NodeState>();
            if (log != null && log.Any())
            {
                NodeState last = log.OrderBy(x => x.DateTime).Last();
                LastRecord = new ChartData(last.DateTime, last.State);
            }
        }

        public override void OnRemove()
        {
            base.OnRemove();
            uiEngine?.statesDb?.RemoveStatesForNode(Id);
        }

        public override bool SetValues(Dictionary<string, string> values)
        {
            if (values.ContainsKey("Clear"))
                RemoveStates();

            return true;
        }



        public override string GetNodeDescription()
        {
            return "This is a UI node. It displays a chart on the dashboard. <br/>" +
                   "The chart shows the history of values, and updated in real time.  <br/><br/>" +
                   "You can enable writing chart points into the database in the settings of the node. " +
                   "You can view the chart in various styles. <br/>" +
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
