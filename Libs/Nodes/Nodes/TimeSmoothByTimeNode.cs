/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace MyNetSensors.Nodes
{
    public class TimeSmoothByTimeNode : Node
    {

        private readonly int DEFAULT_INTERVAL = 1000;

        private bool enabled;

        private double interval;

        private double? currentValue;
        private double endValue;
        private double startValue;

        private DateTime lastUpdateTime;
        private DateTime startTime;

        public TimeSmoothByTimeNode() : base("Time", "Smooth By Time")
        {
            AddInput("Value", DataType.Number);
            AddInput("Interval", DataType.Number, true);

            AddOutput("Value");

            interval = DEFAULT_INTERVAL;

            Settings.Add("UpdateInterval", new NodeSetting(NodeSettingType.Number, "Output Update Interval", "50"));
        }

        public override void Loop()
        {
            if (!enabled)
                return;

            double updateInterval;
            double.TryParse(Settings["UpdateInterval"].Value, out updateInterval);

            if ((DateTime.Now - lastUpdateTime).TotalMilliseconds < updateInterval)
                return;

            lastUpdateTime = DateTime.Now;

            double elapsed = (DateTime.Now - startTime).TotalMilliseconds;
            double percent = elapsed / interval * 100;
            if (percent >= 100 || currentValue== endValue)
            {
                currentValue = endValue;
                enabled = false;
            }
            else
                currentValue = Remap(percent, 0, 100, startValue, endValue);

            Outputs[0].Value = currentValue.ToString();
        }


        public override void OnInputChange(Input input)
        {
            if (input == Inputs[0])
            {
                if (input.Value == null)
                {
                    enabled = false;
                    currentValue = null;
                    ResetOutputs();
                }
                else
                {
                    double val= double.Parse(input.Value);
                    if (val==endValue && val == currentValue)
                        return;

                    if (currentValue == null)
                        currentValue = val;

                    endValue = val;
                    startValue = currentValue.Value;
                    startTime = DateTime.Now;
                    enabled = true;
                }
            }


            if (input == Inputs[1])
            {
                if (input.Value == null)
                    interval = DEFAULT_INTERVAL;
                else
                    double.TryParse(input.Value, out interval);

                if (interval < 1)
                    interval = 1;

                LogInfo($"Interval changed to {interval} ms");
            }
        }

        private double Remap(double value, double inMin, double inMax, double outMin, double outMax)
        {
            return (value - inMin) / (inMax - inMin) * (outMax - outMin) + outMin;
        }
    }
}