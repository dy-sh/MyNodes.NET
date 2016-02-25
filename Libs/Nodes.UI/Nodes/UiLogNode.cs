/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace MyNetSensors.Nodes
{
    public class UiLogNode : UiNode
    {
        public NodeState LastRecord { get; set; }

        private List<NodeState> log;

        public UiLogNode() : base("UI", "Log")
        {
            AddInput();

            log = new List<NodeState>();

            Settings.Add("WriteInDatabase", new NodeSetting(NodeSettingType.Checkbox, "Write in database", "false"));
            Settings.Add("MaxRecords", new NodeSetting(NodeSettingType.Number, "The maximum number of log records", "100"));
        }


        public override void OnInputChange(Input input)
        {
            int max = (int)double.Parse(Settings["MaxRecords"].Value);
            if (max < 0)
                max = 0;

            LastRecord = new NodeState(Id, input.Value);
            log.Add(LastRecord);

            while (log.Count > max)
                log.Remove(log.First());

            UpdateMe();

            if (Settings["WriteInDatabase"].Value == "true")
                AddNodeData(input.Value, max);
        }


        
        private void RemoveStates()
        {
            log.Clear();
            RemoveAllNodeData();
        }

        public override string GetValue(string name)
        {
            if (log == null || !log.Any())
                return null;

            return JsonConvert.SerializeObject(log);
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
            List<NodeState> states = GetAllNodeData();
            log = states ?? new List<NodeState>();
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
            if (data["WriteInDatabase"] == "false"
                && Settings["WriteInDatabase"].Value == "true")
            {
                log = new List<NodeState>();
                LastRecord = null;
            }
            else if (data["WriteInDatabase"] == "true"
                && Settings["WriteInDatabase"].Value == "false")
            {
                GetStatesFromRepository();
            }

            return base.SetSettings(data);
        }


        public override string GetNodeDescription()
        {
            return "This is a UI node. It displays a log on the dashboard. <br/>" +
                   "The log displays all the values that it receives. <br/>" +
                   "It is very convenient to use for debugging your system or for the monitoring. <br/>" +
                   "You can enable writing log into the database in the settings of the node. <br/>" +
                   "You can also specify the maximum number of records in the database. " +
                   "Old records will be deleted automatically.";
        }
    }
}
