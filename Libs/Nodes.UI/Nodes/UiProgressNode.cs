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
        private bool waitingToSend;

        public UiProgressNode() : base("UI", "Progress")
        {
            AddInput();

            Settings.Add("UpdateInterval", new NodeSetting(NodeSettingType.Number, "Update Interval", "100"));

        }


        public override void Loop()
        {
            if (interval == null)
                interval = double.Parse(Settings["UpdateInterval"].Value);

            if (!waitingToSend || (DateTime.Now - lastUpdateTime).TotalMilliseconds < interval)
                return;

            lastUpdateTime = DateTime.Now;
            waitingToSend = false;

            UpdateMe();
        }

        public override void OnInputChange(Input input)
        {
            Value = input.Value;

            if (interval == null)
                interval = double.Parse(Settings["UpdateInterval"].Value);

            if ((DateTime.Now - lastUpdateTime).TotalMilliseconds >= interval)
            {
                lastUpdateTime = DateTime.Now;
                UpdateMe();
            }
            else
            {
                waitingToSend = true;
            }
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
                   "You can set update interval in the settings of the node.";

        }
    }
}
