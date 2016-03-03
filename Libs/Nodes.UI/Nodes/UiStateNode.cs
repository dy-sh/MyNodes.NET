/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;

namespace MyNodes.Nodes
{
    public class UiStateNode : UiNode
    {
        public string Value { get; set; }

        private DateTime lastUpdateTime;
        private double? interval;
        private string lastValue;

        public UiStateNode() : base("UI", "State")
        {
            AddInput();
            Settings.Add("UpdateInterval", new NodeSetting(NodeSettingType.Number, "Update Interval", "100"));
        }

        public override void Loop()
        {
            if (interval == null)
                interval = double.Parse(Settings["UpdateInterval"].Value);

            if (lastValue == Value || (DateTime.Now - lastUpdateTime).TotalMilliseconds < interval)
                return;
            lastUpdateTime = DateTime.Now;

            Value = lastValue;
            UpdateMeOnDashboard();
        }

        public override void OnInputChange(Input input)
        {
            lastValue = input.Value;
        }

        public override bool SetSettings(Dictionary<string, string> data)
        {
            interval = double.Parse(data["UpdateInterval"]);
            return base.SetSettings(data);
        }

        public override string GetNodeDescription()
        {
            return "This is a UI node. It displays a switch on the dashboard " +
                   "that can be used to monitor the status of any node. <br/>" +
                   "It takes logical 0 or 1. When another value, the node will be red.<br/>" +
                   "You can set the refresh rate in the settings of the node. ";
        }
    }
}
