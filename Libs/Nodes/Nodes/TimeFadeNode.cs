/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace MyNetSensors.Nodes
{
    public class TimeFadeNode : Node
    {

        private readonly int DEFAULT_INTERVAL = 1000;

        private bool enabled;

        private double interval;

        private double? currentValue;
        private double startValue;
        private double endValue;

        private DateTime lastUpdateTime;
        private DateTime startTime;

        public TimeFadeNode() : base("Time", "Fade")
        {
            AddInput("From Value", DataType.Number);
            AddInput("To Value", DataType.Number);
            AddInput("Interval", DataType.Number, true);
            AddInput("Start/Stop", DataType.Logical, true);

            AddOutput("Value");
            AddOutput("Enabled", DataType.Logical);

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
            if (percent >= 100 || currentValue == endValue)
                currentValue = endValue;
            else
                currentValue = Remap(percent, 0, 100, startValue, endValue);

            Outputs[0].Value = currentValue.ToString();

            if (currentValue == endValue)
                Stop();
        }


        public override void OnInputChange(Input input)
        {
            if (input == Inputs[2])
            {
                if (input.Value == null)
                    interval = DEFAULT_INTERVAL;
                else
                    double.TryParse(input.Value, out interval);

                if (interval < 1)
                    interval = 1;

                LogInfo($"Interval changed to {interval} ms");
            }

            if (Inputs[0].Value == null || Inputs[1].Value == null)
            {
                Stop();
                return;
            }

            if (input == Inputs[3])
            {
                if (input.Value == "0")
                {
                    Stop();
                    return;
                }
                if (input.Value == "1")
                {
                    startValue = double.Parse(Inputs[0].Value);
                    endValue = double.Parse(Inputs[1].Value);

                    Start();
                }
            }

        }


        private void Stop()
        {
            if (enabled)
            {
                enabled = false;
                Outputs[1].Value = "0";
            }
        }

        private void Start()
        {
            currentValue = startValue;

            startTime = DateTime.Now;
            Outputs[1].Value = "1";
            enabled = true;
        }

        private double Remap(double value, double inMin, double inMax, double outMin, double outMax)
        {
            return (value - inMin) / (inMax - inMin) * (outMax - outMin) + outMin;
        }

        public override string GetNodeDescription()
        {
            return "This node makes a smooth transition from one value to another. " +
                   "You can specify the time interval for which the value must change. " +
                   "The output is named \"Enabled\" sends \"1\" " +
                   "when the node is in the active state (makes the transition)." +
                   "In the settings of the node you can increase the refresh rate " +
                   "to make the transition more smoother. " +
                   "Or, reduce the refresh rate to reduce CPU load.";
        }
    }
}