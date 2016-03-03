/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;

namespace MyNodes.Nodes
{
    public class UiProgressNode : UiNode
    {
        public string Value { get; set; }

        private DateTime lastUpdateTime;
        private double? interval;
        private string lastValue;

        public UiProgressNode() : base("UI", "Progress")
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
            return "This is a UI node. It displays a progress bar on the dashboard. <br/>" +
                   "The progress bar may display the progress of a certain event in percent. <br/>" +
                   "It takes a value from 0 to 100.<br/>" +
                   "You can set the refresh rate in the settings of the node. ";
        }
    }
}
