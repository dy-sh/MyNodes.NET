/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace MyNodes.Nodes
{
    public class TimeSmoothByTimeNode : Node
    {

        private readonly int DEFAULT_INTERVAL = 1000;

        private bool enabled;

        private double interval;

        private double currentValue;
        private double endValue;
        private double startValue;

        private DateTime lastUpdateTime;
        private DateTime startTime;

        public TimeSmoothByTimeNode() : base("Time", "Smooth By Time")
        {
            AddInput("Value", DataType.Number);
            AddInput("Interval", DataType.Number, true);

            AddOutput("Value");
            AddOutput("Enabled", DataType.Logical);

            interval = DEFAULT_INTERVAL;

            Settings.Add("UpdateInterval", new NodeSetting(NodeSettingType.Number, "Output update interval", "50"));
            Settings.Add("StopWhenDisconnected", new NodeSetting(NodeSettingType.Checkbox, "Stop when input value is null", "false"));
            Settings.Add("ResetWhenDisconnected", new NodeSetting(NodeSettingType.Checkbox, "Reset and send null when input value is null", "false"));
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
            if (input == Inputs[0])
            {
                if (input.Value == null)
                {
                    if (Settings["StopWhenDisconnected"].Value == "true")
                        Stop();

                    if (Settings["ResetWhenDisconnected"].Value == "true")
                        Reset();
                }
                else
                {
                    startValue = currentValue;
                    endValue = double.Parse(input.Value);

                    Start();
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


        private void Stop()
        {
            if (enabled)
            {
                enabled = false;
                Outputs[1].Value = "0";
            }
        }

        private void Reset()
        {
            Outputs[0].Value = null;
            currentValue = 0;
        }

        private void Start()
        {
            startTime = DateTime.Now;
            lastUpdateTime = startTime;

            Outputs[1].Value = "1";
            enabled = true;
        }


        private double Remap(double value, double inMin, double inMax, double outMin, double outMax)
        {
            return (value - inMin) / (inMax - inMin) * (outMax - outMin) + outMin;
        }

        public override string GetNodeDescription()
        {
            return "This node makes a smooth transition of values. <br/>" +
                   "It avoids abrupt changes of the value on the output. <br/><br/>" +

                   "The input named \"Interval\" specifies the time " +
                   "for which the output should change completely. <br/><br/>" +

                   "For example, you set the interval to 1000 MS. <br/>" +
                   "Send \"10\" to the input. The node gradually changes the output value, " +
                   "and after 1000 MS it will be equal to \"10\". <br/>" +
                   "Then you send \"20\", and after 1000 MS a value " +
                   "of the output will be 20, but between 10 and 20 will " +
                   "be 11,12,13,14,15,16,17,18,19. <br/><br/>" +

                   "In the settings of the node you can increase the refresh rate " +
                   "to make the transition more smoother. " +
                   "Or, reduce the refresh rate to reduce CPU load.<br/><br/>" +

                   "Also, you can specify in the settings, " +
                   "what should be done when the input color is null.";
        }
    }
}