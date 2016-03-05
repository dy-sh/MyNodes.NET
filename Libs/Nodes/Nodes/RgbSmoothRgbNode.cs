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
    public class RgbSmoothRgbNode : Node
    {

        private readonly int DEFAULT_INTERVAL = 1000;

        private bool enabled;

        private double interval;

        private int[] currentValue = { 0, 0, 0 };
        private int[] startValue;
        private int[] endValue;

        private DateTime lastUpdateTime;
        private DateTime startTime;

        public RgbSmoothRgbNode() : base("RGB", "Smooth RGB")
        {
            AddInput("RGB", DataType.Text);
            AddInput("Interval", DataType.Number, true);

            AddOutput("RGB");
            AddOutput("Enabled", DataType.Logical);

            Outputs[1].Value = "0";
            Outputs[0].Value = "000000";

            interval = DEFAULT_INTERVAL;

            Settings.Add("UpdateInterval", new NodeSetting(NodeSettingType.Number, "Output update interval", "30"));
            Settings.Add("PreventDuplication", new NodeSetting(NodeSettingType.Checkbox, "Prevent duplication", "true"));
            Settings.Add("StopWhenDisconnected", new NodeSetting(NodeSettingType.Checkbox, "Stop when input color is null", "false"));
            Settings.Add("ResetWhenDisconnected", new NodeSetting(NodeSettingType.Checkbox, "Send null when input color is null", "false"));
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

            string newVal = ConvertIntsToHexString(currentValue);

            if (Settings["PreventDuplication"].Value != "true" || Outputs[0].Value != newVal)
                Outputs[0].Value = newVal;

            if (currentValue == endValue)
                Stop();
        }


        public override void OnInputChange(Input input)
        {
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
                    try
                    {
                        startValue = (int[])currentValue.Clone();
                        endValue = ConvertHexStringToIntArray(Inputs[0].Value);

                        if (startValue.Length != 3 || endValue.Length != 3)
                            throw new Exception("Incorrect value in input.");

                        Start();
                    }
                    catch
                    {
                        LogError("Incorrect value in input.");

                        if (Settings["StopWhenDisconnected"].Value == "true")
                            Stop();

                        if (Settings["ResetWhenDisconnected"].Value == "true")
                            Reset();
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

        private void Reset()
        {
            Outputs[0].Value = null;
            currentValue = new int[] { 0, 0, 0 };
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
            return "This node makes a smooth transition of the RGB color. <br/>" +
                   "It avoids abrupt changes of the color on the output. <br/>" +
                   "The input named \"Interval\" specifies the time " +
                   "for which the color should change completely. <br/><br/>" +

                   "The output is named \"Enabled\" sends \"1\" " +
                   "when the node is in the active state (makes the transition). <br/><br/>" +
                   "In the settings of the node you can increase the refresh rate " +
                   "to make the transition more smoother. " +
                   "Or, reduce the refresh rate to reduce CPU load.<br/><br/>" +
                   "Also, you can specify in the settings, " +
                   "what should be done when the input color is null.";
        }
    }
}