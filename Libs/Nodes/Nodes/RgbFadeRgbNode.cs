/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace MyNodes.Nodes
{
    public class RgbFadeRgbNode : Node
    {

        private readonly int DEFAULT_INTERVAL = 1000;

        private bool enabled;

        private double interval;

        private int[] currentValue;
        private int[] startValue;
        private int[] endValue;

        private DateTime lastUpdateTime;
        private DateTime startTime;

        public RgbFadeRgbNode() : base("RGB", "Fade RGB")
        {
            AddInput("From RGB", DataType.Text);
            AddInput("To RGB", DataType.Text);
            AddInput("Interval", DataType.Number, true);
            AddInput("Start/Stop", DataType.Logical, true);

            AddOutput("RGB");
            AddOutput("Enabled", DataType.Logical);

            Outputs[1].Value = "0";

            interval = DEFAULT_INTERVAL;

            Settings.Add("UpdateInterval", new NodeSetting(NodeSettingType.Number, "Output update interval", "30"));
            Settings.Add("PreventDuplication", new NodeSetting(NodeSettingType.Checkbox, "Prevent duplication", "true"));
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
            {
                for (int i = 0; i < startValue.Length; i++)
                    currentValue[i] = (int)Remap(percent, 0, 100, startValue[i], endValue[i]);
            }

            string newVal= ConvertIntsToHexString(currentValue);

            if (Settings["PreventDuplication"].Value != "true" || Outputs[0].Value != newVal)
                Outputs[0].Value = newVal;

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

                if (Outputs[0].Value != null)
                    Outputs[0].Value = null;

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
                    try
                    {
                        startValue = ConvertHexStringToIntArray(Inputs[0].Value);
                        endValue = ConvertHexStringToIntArray(Inputs[1].Value);

                        if (startValue.Length != 3 || endValue.Length != 3)
                            throw new Exception("Incorrect value in input.");

                        Start();
                    }
                    catch
                    {
                        LogError("Incorrect value in input.");
                        Stop();

                        if (Outputs[0].Value != null)
                            Outputs[0].Value = null;
                    }

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
            currentValue = (int[]) startValue.Clone();

            startTime = DateTime.Now;
            lastUpdateTime = startTime;
            Outputs[0].Value = ConvertIntsToHexString(currentValue);

            Outputs[1].Value = "1";
            enabled = true;
        }

        private double Remap(double value, double inMin, double inMax, double outMin, double outMax)
        {
            return (value - inMin) / (inMax - inMin) * (outMax - outMin) + outMin;
        }

        public string ConvertIntsToHexString(int[] rgb)
        {
            return rgb.Aggregate("", (c, t) => c + t.ToString("X2"));
        }

        public static int[] ConvertHexStringToIntArray(string hexString)
        {

            if (hexString[0] == '#')
                hexString = hexString.Remove(0, 1);

            int count = hexString.Length / 2;
            int[] result = new int[count];

            for (int i = 0; i < count; i++)
                result[i] = int.Parse(hexString.Substring(i * 2, 2), NumberStyles.HexNumber);

            return result;
        }



        public override string GetNodeDescription()
        {
            return "This node makes a smooth transition from one RGB color to another. <br/>" +
                   "You can specify the time interval for which color must change. <br/>" +
                   "The output is named \"Enabled\" sends \"1\" " +
                   "when the node is in the active state (makes the transition). <br/>" +
                   "In the settings of the node you can increase the refresh rate " +
                   "to make the transition more smoother. " +
                   "Or, reduce the refresh rate to reduce CPU load.";
        }
    }
}