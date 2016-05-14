using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNodes.Nodes.Custom
{
    public class UiStateSwitchNode : UiNode
    {
        public string Value { get; set; }
        private DateTime lastUpdateTime;
        private double? interval;
        private string lastValue;

        public UiStateSwitchNode() : base("UI", "StateSwitch")
        {
            AddInput();
            Settings.Add("UpdateInterval", new NodeSetting(NodeSettingType.Number, "Update Interval", "100"));

            AddOutput();
            Value = "0";
            Outputs[0].Value = Value;
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

        public override bool SetValues(Dictionary<string, string> values)
        {
            Value = Value == "0" ? "1" : "0";
            Outputs[0].Value = Value;
            lastValue = Value;
            UpdateMeOnDashboard();
            UpdateMeInDb();

            return true;
        }

        public override string GetNodeDescription()
        {
            return "This is a UI node. It displays a switch on the dashboard. You can set the refresh rate in the settings of the node. ";
        }
    }
}
