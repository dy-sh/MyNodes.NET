/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace MyNetSensors.Nodes
{
    public class UiSliderNode : UiNode
    {
        public int Value { get; set; }

        public UiSliderNode() : base("UI", "Slider")
        {
            AddOutput();

            Settings.Add("Min", new NodeSetting(NodeSettingType.Number, "Min Value", "0"));
            Settings.Add("Max", new NodeSetting(NodeSettingType.Number, "Max Value", "100"));

            Outputs[0].Value = Value.ToString();
        }


        public override bool SetSettings(Dictionary<string, string> data)
        {
            int min = Int32.Parse(data["Min"]);
            int max = Int32.Parse(data["Max"]);

            if (min >= max)
            {
                LogError($"Can`t set settings. Min must be > Max.");
                return false;
            }

            return base.SetSettings(data);
        }

        public override bool SetValues(Dictionary<string, string> values)
        {
            Value = Int32.Parse(values.FirstOrDefault().Value);
            Outputs[0].Value = Value.ToString();

            UpdateMe();
            UpdateMeInDb();

            return true;
        }

        public override string GetNodeDescription()
        {
            return "This is a UI node. It displays a slider on the dashboard. " +
                   "You can set the minimum and maximum value of the slider. " +
                   "By default it is 0-100.";
        }
    }
}
