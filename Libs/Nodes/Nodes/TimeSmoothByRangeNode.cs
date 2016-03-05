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
    public class TimeSmoothByRangeNode : Node
    {

        private readonly int DEFAULT_RANGE = 100;

        private bool enabled;

        private double range;

        private double currentValue;
        private double endValue;

        private DateTime lastUpdateTime;

        public TimeSmoothByRangeNode() : base("Time", "Smooth By Range")
        {
            AddInput("Value", DataType.Number);
            AddInput("Max/s", DataType.Number, true);

            AddOutput("Value");
            AddOutput("Enabled", DataType.Logical);

            range = DEFAULT_RANGE;

            Settings.Add("UpdateInterval", new NodeSetting(NodeSettingType.Number, "Output update interval", "50"));
            Settings.Add("StopWhenDisconnected", new NodeSetting(NodeSettingType.Checkbox, "Stop when input value is null", "false"));
            Settings.Add("ResetWhenDisconnected", new NodeSetting(NodeSettingType.Checkbox, "Send null when input value is null", "false"));
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
                    currentValue = endValue;
            }
            else
            {
                currentValue -= Remap(elapsed, 0, 1000, 0, range);

                if (currentValue <= endValue)
                    currentValue = endValue;
            }

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
                    endValue = double.Parse(input.Value);

                    Start();
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
            double updateInterval;
            double.TryParse(Settings["UpdateInterval"].Value, out updateInterval);
            if ((DateTime.Now - lastUpdateTime).TotalMilliseconds >= updateInterval)
                lastUpdateTime = DateTime.Now;

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

                   "The input named \"Max/s\" sets the limit range at which " +
                   "the output can change in 1 second. <br/><br/>" +

                   "For example, you set the Max/s to \"5\". <br/>" +
                   "Send \"10\" to the input. The node gradually changes the output value to 10. <br/>" +
                   "Then you send \"20\", and after 2 seconds a value " +
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