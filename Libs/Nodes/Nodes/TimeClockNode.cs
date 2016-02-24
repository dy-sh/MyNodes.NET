/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace MyNetSensors.Nodes
{
    public class TimeClockNode : Node
    {

        private readonly int DEFAULT_INTERVAL = 1000;

        private double interval;

        private DateTime lastUpdateTime;

        public TimeClockNode() : base("Time", "Clock")
        {
            AddOutput("Millisecond", DataType.Number);
            AddOutput("Second", DataType.Number);
            AddOutput("Minute", DataType.Number);
            AddOutput("Hour", DataType.Number);

            interval = DEFAULT_INTERVAL;

            Settings.Add("UpdateInterval", new NodeSetting(NodeSettingType.Number, "Update Interval", "50"));
            Settings.Add("YearMonthDay", new NodeSetting(NodeSettingType.Checkbox, "Day, Month, Year  Outputs", "false"));

            options.LogOutputChanges = false;
        }

        public override void Loop()
        {
            double updateInterval;
            double.TryParse(Settings["UpdateInterval"].Value, out updateInterval);

            if ((DateTime.Now - lastUpdateTime).TotalMilliseconds < updateInterval)
                return;

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

                if (Outputs[4].Value != d) Outputs[4].Value = d;
                if (Outputs[5].Value != mo) Outputs[5].Value = mo;
                if (Outputs[6].Value != y) Outputs[6].Value = y;
            }
        }

        public override bool SetSettings(Dictionary<string, string> data)
        {
            if (data["YearMonthDay"]=="true" && Settings["YearMonthDay"].Value=="false")
                AddAdditionOutputs();

            else if (data["YearMonthDay"] == "false" && Settings["YearMonthDay"].Value == "true")
                RemoveAdditionOutputs();

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

            LogInfo($"Added additional outputs");

            UpdateMe();
            UpdateMeInDb();
        }

        public void RemoveAdditionOutputs()
        {
            if (GetOutput("Day") != null)
                RemoveOutput("Day");

            if (GetOutput("Month") != null)
                RemoveOutput("Month");

            if (GetOutput("Year") != null)
                RemoveOutput("Year");

            LogInfo($"Removed additional outputs");

            UpdateMe();
            UpdateMeInDb();
        }

        public override string GetNodeDescription()
        {
            return "This is the system clock. " +
                   "In the settings you can enable additional outputs " +
                   "and configure the refresh rate of the outputs.";
        }
    }
}