/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MyNodes.Nodes
{
    public class UiLabelNode : UiNode
    {
        public string Value { get; set; }

        private DateTime lastUpdateTime;
        private double? interval;
        private bool waitingToSend;
        private string lastValue;

        public UiLabelNode() : base("UI", "Label")
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
            UpdateMe();
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
            return "This is a UI node. It displays a label on the dashboard. <br/>" +
                   "Label can display any text. <br/>" +
                   "You can set the refresh rate in the settings of the node. ";

        }
    }
}
