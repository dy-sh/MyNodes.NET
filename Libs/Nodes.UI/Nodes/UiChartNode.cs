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

        private List<NodeData> log;

        private NodeData LastRecordCached { get; set; }
        private bool LastRecordUpdated { get; set; }
        public DateTime LastUpdateDate { get; set; }

        public UiChartNode() : base("UI", "Chart")
        {
            AddInput(DataType.Number);

            log = new List<NodeData>();

            Settings.Add("WriteInDatabase", new NodeSetting(NodeSettingType.Checkbox, "Write in database", "false"));
            Settings.Add("MaxRecords", new NodeSetting(NodeSettingType.Number, "The maximum number of chart points", "100"));
            Settings.Add("UpdateInterval", new NodeSetting(NodeSettingType.Number, "Update Interval", "100"));
        }



        public override void Loop()
        {
            if (!LastRecordUpdated || LastRecordCached==null)
                return;

            int updateInteval = (int)double.Parse(Settings["UpdateInterval"].Value);
            if ((DateTime.Now - LastUpdateDate).TotalMilliseconds < updateInteval)
                return;

            LastRecordUpdated = false;
            LastUpdateDate = DateTime.Now;

            int max = (int)double.Parse(Settings["MaxRecords"].Value);
            if (max < 0)
                max = 0;

            LastRecord = new ChartData(LastRecordCached.DateTime, LastRecordCached.Value);
            log.Add(LastRecordCached);

            while (log.Count > max)
                log.Remove(log.First());

            UpdateMe();

            if (Settings["WriteInDatabase"].Value == "true")
                AddNodeData(LastRecordCached.Value, max);

        }

        public override void OnInputChange(Input input)
        {
            if (input.Value == null)
                return;

            LastRecordCached = new NodeData(Id, input.Value);
            LastRecordUpdated = true;
        }



        private void RemoveStates()
        {
            log.Clear();
            LastRecordCached = null;
            LastRecordUpdated = false;
            LastRecord = null;
            if (Settings["WriteInDatabase"].Value == "true")
                RemoveAllNodeData();
        }

        public override string GetValue(string name)
        {
            if (log == null || !log.Any())
                return null;

            //copy to array to prevent changing data error
            NodeData[] nodeDatasArray = new NodeData[log.Count];
            log.CopyTo(nodeDatasArray);

            List<ChartData> chartData = nodeDatasArray.Select(
                state => new ChartData(state.DateTime, state.Value)).ToList();

            return JsonConvert.SerializeObject(chartData);
        }


        public override bool OnAddToEngine(NodesEngine engine)
        {
            this.engine = engine;

            if (Settings["WriteInDatabase"].Value == "true")
                GetStatesFromRepository();

            return base.OnAddToEngine(engine);
        }

        private void GetStatesFromRepository()
        {
            List<NodeData> states = GetAllNodeData();
            log = states ?? new List<NodeData>();
        }

        public override void OnRemove()
        {
            RemoveAllNodeData();
            base.OnRemove();
        }

        public override bool SetValues(Dictionary<string, string> values)
        {
            if (values.ContainsKey("Clear"))
                RemoveStates();

            return true;
        }

        public override bool SetSettings(Dictionary<string, string> data)
        {
            if (data["WriteInDatabase"] == "false" && Settings["WriteInDatabase"].Value == "true")
            {
                log.Clear();
                LastRecordCached = null;
                LastRecordUpdated = false;
                LastRecord = null;
            }
            else if (data["WriteInDatabase"] == "true" && Settings["WriteInDatabase"].Value == "false")
                GetStatesFromRepository();

            return base.SetSettings(data);
        }


        public override string GetNodeDescription()
        {
            return "This is a UI node. It displays a chart on the dashboard. <br/>" +
                   "The chart shows the history of values, and updated in real time.  <br/><br/>" +

                   "You can enable writing chart data into the database in the settings of the node. <br/>" +
                   "You can also specify the maximum number of records in the database. " +
                   "Old records will be deleted automatically. <br/><br/>" +

                   "By default, the graph shows not all values. " +
                   "Too frequent update of the value is ignored. " +
                   "By default, the graph is updated 5 times per second. " +
                   "You can set the refresh rate in the settings of the node. " +
                   "Increase the refresh rate if you want to have a more detailed graph. " +
                   "Reduce the refresh rate, if your browser started to lag. <br/><br/>" +

                   "The chart can be displayed in different styles. <br/>" +
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
