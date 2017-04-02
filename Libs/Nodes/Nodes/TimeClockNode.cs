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
        private class OutputNames
        {
            public const string Millisecond = "Millisecond";
            public const string Second = "Second";
            public const string Minute = "Minute";
            public const string Hour = "Hour";
            public const string Day = "Day";
            public const string Month = "Month";
            public const string Year = "Year";
            public const string DayOfWeek = "DayOfWeek";
        }

        private const string YearMonthDaySettingKey = "YearMonthDay";
        private const string DayOfWeekSettingKey = "DayOfWeek";
        private const string UpdateIntervalSettingKey = "UpdateInterval";

        private const int DEFAULT_INTERVAL = 50;

        private DateTime lastUpdateTime;

        public TimeClockNode() : base("Time", "Clock")
        {
            AddOutput(OutputNames.Millisecond, DataType.Number);
            AddOutput(OutputNames.Second, DataType.Number);
            AddOutput(OutputNames.Minute, DataType.Number);
            AddOutput(OutputNames.Hour, DataType.Number);

            Settings.Add(UpdateIntervalSettingKey, new NodeSetting(NodeSettingType.Number, "Update interval", DEFAULT_INTERVAL.ToString()));
            Settings.Add(DayOfWeekSettingKey, new NodeSetting(NodeSettingType.Checkbox, "Day of week output", "false"));
            Settings.Add(YearMonthDaySettingKey, new NodeSetting(NodeSettingType.Checkbox, "Day, Month, Year outputs", "false"));

            options.LogOutputChanges = false;
        }

        public override void Loop()
        {
            double updateInterval;
            if (!double.TryParse(Settings[UpdateIntervalSettingKey].Value, out updateInterval))
            {
                return;
            }

            DateTime now = DateTime.Now;

            if ((now - lastUpdateTime).TotalMilliseconds < updateInterval)
            {
                return;
            }

            lastUpdateTime = now;

            SetOutputValue(OutputNames.Millisecond, now.Millisecond, true);
            SetOutputValue(OutputNames.Second, now.Second, true);
            SetOutputValue(OutputNames.Minute, now.Minute, true);
            SetOutputValue(OutputNames.Hour, now.Hour, true);

            if (Outputs.Count > 4)
            {
                SetOutputValue(OutputNames.Day, now.Day, true);
                SetOutputValue(OutputNames.Month, now.Month, true);
                SetOutputValue(OutputNames.Year, now.Year, true);
                SetOutputValue(OutputNames.DayOfWeek, (int)now.DayOfWeek, true);
            }
        }

        public override bool SetSettings(Dictionary<string, string> data)
        {
            if (data[YearMonthDaySettingKey] != Settings[YearMonthDaySettingKey].Value)
            {
                if (data[YearMonthDaySettingKey] == "true")
                {
                    AddYearMonthDayOutputs();
                }
                else
                {
                    RemoveYearMonthDayOutputs();
                }
            }

            if (data[DayOfWeekSettingKey] != Settings[DayOfWeekSettingKey].Value)
            {
                if (data[DayOfWeekSettingKey] == "true")
                {
                    AddDayOfWeekOutput();
                }
                else
                {
                    RemoveDayOfWeekOutput();
                }
            }

            return base.SetSettings(data);
        }

        public void AddYearMonthDayOutputs()
        {
            if (GetOutput(OutputNames.Day) == null)
                AddOutput(OutputNames.Day, DataType.Number);

            if (GetOutput(OutputNames.Month) == null)
                AddOutput(OutputNames.Month, DataType.Number);

            if (GetOutput(OutputNames.Year) == null)
                AddOutput(OutputNames.Year, DataType.Number);

            LogInfo($"Added YearMonthDay outputs");
            UpdateMe();
        }

        public void RemoveYearMonthDayOutputs()
        {
            if (GetOutput(OutputNames.Day) != null)
                RemoveOutput(OutputNames.Day);

            if (GetOutput(OutputNames.Month) != null)
                RemoveOutput(OutputNames.Month);

            if (GetOutput(OutputNames.Year) != null)
                RemoveOutput(OutputNames.Year);

            LogInfo($"Removed YearMonthDay outputs");
            UpdateMe();
        }

        public void AddDayOfWeekOutput()
        {
            if (GetOutput(OutputNames.DayOfWeek) == null)
                AddOutput(OutputNames.DayOfWeek, DataType.Number);

            LogInfo($"Added DayOfWeek output");
            UpdateMe();
        }

        public void RemoveDayOfWeekOutput()
        {
            if (GetOutput(OutputNames.DayOfWeek) != null)
                RemoveOutput(OutputNames.DayOfWeek);

            LogInfo($"Removed DayOfWeek output");
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