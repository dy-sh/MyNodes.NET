/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Timers;

namespace MyNodes.Nodes
{
    public class TimeTickerNode : Node
    {
        private readonly int DEFAULT_INTERVAL = 1000;

        private bool state;

        private bool enabled = true;

        private double interval;

        private DateTime lastTime;

        public TimeTickerNode() : base("Time", "Ticker")
        {
            AddInput("Interval", DataType.Number, true);
            AddInput("Start/Stop", DataType.Logical, true);

            AddOutput("Out", DataType.Logical);

            interval = DEFAULT_INTERVAL;

            Settings.Add("zero", new NodeSetting(NodeSettingType.Checkbox, "Generate Zero", "true"));

            lastTime = DateTime.Now;
        }


        public override void Loop()
        {
            if (!enabled)
                return;

            if (Settings["zero"].Value == "true")
            {
                if ((DateTime.Now - lastTime).TotalMilliseconds < interval / 2)
                    return;

                state = !state;
                lastTime = DateTime.Now;

                Outputs[0].Value = state ? "1" : "0";
            }
            else
            {
                if ((DateTime.Now - lastTime).TotalMilliseconds < interval)
                    return;

                lastTime = DateTime.Now;

                Outputs[0].Value = "1";
            }
        }



        public override void OnInputChange(Input input)
        {
            if (input == Inputs[0])
            {
                if (input.Value == null)
                    interval = DEFAULT_INTERVAL;
                else
                    double.TryParse(input.Value, out interval);

                LogInfo($"Interval changed to {interval} ms");
            }


            if (input == Inputs[1])
            {
                enabled = input.Value == "1" || input.Value == null;
                state = enabled;
                LogInfo(enabled ? "Start" : "Stop");
                Outputs[0].Value = state ? "1" : "0";
            }
        }

        public override string GetNodeDescription()
        {
            return "This node generates a sequence like 101010... with specified time interval. <br/>" +
                   "You can set the time interval and activate the timer, " +
                   "giving \"1\" to the input named \"Start/Stop\". <br/>" +
                   "If \"Generate Zero\" option is enabled in the settings of the node, " +
                   "node will generate a sequence like 101010... " +
                   "If disabled, the output will be 111111...";
        }
    }
}