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

            return JsonConvert.SerializeObject(log);
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
                LastRecord = log.OrderBy(x => x.DateTime).Last();
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
            return "This is a UI node. It displays a log on the dashboard. <br/>" +
                   "The log displays all the values that it receives. <br/>" +
                   "It is very convenient to use for debugging your system or for the monitoring." +
                   "You can enable writing log into the database in the settings of the node. ";
        }
    }
}
