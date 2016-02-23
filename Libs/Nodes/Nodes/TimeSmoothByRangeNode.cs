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
    public class TimeSmoothByRangeNode : Node
    {

        private readonly int DEFAULT_RANGE = 100;

        private bool enabled;

        private double range;

        private double? currentValue;
        private double endValue;

        private DateTime lastUpdateTime;

        public TimeSmoothByRangeNode() : base("Time", "Smooth By Range")
        {
            AddInput("Value", DataType.Number);
            AddInput("Max/s", DataType.Number, true);

            AddOutput("Value");

            range = DEFAULT_RANGE;

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

            double elapsed = (DateTime.Now - lastUpdateTime).TotalMilliseconds;
            lastUpdateTime = DateTime.Now;

            if (endValue >= currentValue)
            {
                currentValue += Remap(elapsed, 0, 1000, 0, range);

                if (currentValue >= endValue)
                {
                    currentValue = endValue;
                    enabled = false;
                }
            }
            else
            if (endValue < currentValue)
            {
                currentValue -= Remap(elapsed, 0, 1000, 0, range);

                if (currentValue <= endValue)
                {
                    currentValue = endValue;
                    enabled = false;
                }
            }

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

                    double updateInterval;
                    double.TryParse(Settings["UpdateInterval"].Value, out updateInterval);

                    if ((DateTime.Now- lastUpdateTime).TotalMilliseconds>= updateInterval)
                        lastUpdateTime = DateTime.Now;

                    enabled = true;
                }
            }


            if (input == Inputs[1])
            {
                if (input.Value == null)
                    range = DEFAULT_RANGE;
                else
                    double.TryParse(input.Value, out range);

                if (range < 0.0001)
                    range = 0.0001;

                LogInfo($"Range changed to {range}/s");
            }
        }

        private double Remap(double value, double inMin, double inMax, double outMin, double outMax)
        {
            return (value - inMin) / (inMax - inMin) * (outMax - outMin) + outMin;
        }
    }
}