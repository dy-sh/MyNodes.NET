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

        public override string GetNodeDescription()
        {
            return "This node makes a smooth transition of values. <br/>" +
                   "It avoids abrupt changes of the value on the output. <br/>" +
                   "The input named \"Max/s\" sets the limit range at which " +
                   "the output can change in 1 second. <br/><br/>" +

                   "For example, you set the Max/s to \"5\". <br/>" +
                   "Send \"10\" to the input. The node gradually changes the output value to 10. <br/>" +
                   "Then you send \"20\", and after 2 seconds a value " +
                   "of the output will be 20, but between 10 and 20 will " +
                   "be 11,12,13,14,15,16,17,18,19. <br/><br/>" +

                   "In the settings of the node you can increase the refresh rate " +
                   "to make the transition more smoother. " +
                   "Or, reduce the refresh rate to reduce CPU load.";
        }
    }
}