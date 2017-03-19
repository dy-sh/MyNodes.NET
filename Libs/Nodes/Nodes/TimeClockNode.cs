/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;

namespace MyNodes.Nodes
{
    public class TimeClockNode : Node
    {
        private const string YearMonthDaySettingKey = "YearMonthDay";
        private const string UpdateIntervalSettingKey = "UpdateInterval";

        private const int DEFAULT_INTERVAL = 50;

        private DateTime lastUpdateTime;

        public TimeClockNode() : base("Time", "Clock")
        {
            AddOutput("Millisecond", DataType.Number);
            AddOutput("Second", DataType.Number);
            AddOutput("Minute", DataType.Number);
            AddOutput("Hour", DataType.Number);

            Settings.Add(UpdateIntervalSettingKey, new NodeSetting(NodeSettingType.Number, "Update Interval", DEFAULT_INTERVAL.ToString()));
            Settings.Add(YearMonthDaySettingKey, new NodeSetting(NodeSettingType.Checkbox, "Day, Month, Year  Outputs", "false"));

            options.LogOutputChanges = false;
        }

        public override void Loop()
        {
            double updateInterval;
            if (!double.TryParse(Settings[UpdateIntervalSettingKey].Value, out updateInterval))
            {
                return;
            }

            if ((DateTime.Now - lastUpdateTime).TotalMilliseconds < updateInterval)
            {
                return;
            }

            lastUpdateTime = DateTime.Now;

            string ms = DateTime.Now.Millisecond.ToString();
            string s = DateTime.Now.Second.ToString();
            string mi = DateTime.Now.Minute.ToString();
            string h = DateTime.Now.Hour.ToString();

            if (Outputs[0].Value != ms) Outputs[0].Value = ms;
            if (Outputs[1].Value != s) Outputs[1].Value = s;
            if (Outputs[2].Value != mi) Outputs[2].Value = mi;
            if (Outputs[3].Value != h) Outputs[3].Value = h;

            if (Outputs.Count > 4)
            {
                string d = DateTime.Now.Day.ToString();
                string mo = DateTime.Now.Month.ToString();
                string y = DateTime.Now.Year.ToString();
                string dw = ((int)DateTime.Now.DayOfWeek).ToString();

                if (Outputs[4].Value != d) Outputs[4].Value = d;
                if (Outputs[5].Value != mo) Outputs[5].Value = mo;
                if (Outputs[6].Value != y) Outputs[6].Value = y;

                // If the node data is from an older version there wasn't the DayOfWeek column
                if (Outputs.Count == 8 && Outputs[7].Value != dw) Outputs[7].Value = dw;
            }
        }

        public override bool SetSettings(Dictionary<string, string> data)
        {
            if (data[YearMonthDaySettingKey] == "true" && Settings[YearMonthDaySettingKey].Value == "false")
            {
                AddAdditionOutputs();
            }
            else if (data[YearMonthDaySettingKey] == "false" && Settings[YearMonthDaySettingKey].Value == "true")
            {
                RemoveAdditionOutputs();
            }

            return base.SetSettings(data);
        }

        public void AddAdditionOutputs()
        {
            if (GetOutput("Day") == null)
                AddOutput("Day", DataType.Number);

            if (GetOutput("Month") == null)
                AddOutput("Month", DataType.Number);

            if (GetOutput("Year") == null)
                AddOutput("Year", DataType.Number);

            if (GetOutput("DayOfWeek") == null)
                AddOutput("DayOfWeek", DataType.Number);

            LogInfo($"Added additional outputs");
            UpdateMe();
        }

        public void RemoveAdditionOutputs()
        {
            if (GetOutput("Day") != null)
                RemoveOutput("Day");

            if (GetOutput("Month") != null)
                RemoveOutput("Month");

            if (GetOutput("Year") != null)
                RemoveOutput("Year");

            if (GetOutput("DayOfWeek") != null)
                RemoveOutput("DayOfWeek");

            LogInfo($"Removed additional outputs");
            UpdateMe();
        }

        public override string GetNodeDescription()
        {
            return "This is the system clock. <br/>" +
                   "In the settings you can enable additional outputs " +
                   "and configure the refresh rate of the outputs.";
        }

        private void UpdateMe()
        {
            UpdateMeInEditor();
            UpdateMeInDb();
        }
    }
}