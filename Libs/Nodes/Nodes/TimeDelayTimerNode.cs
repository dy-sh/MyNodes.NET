/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;

namespace MyNetSensors.Nodes
{
    public class TimeDelayTimerNode : Node
    {
        private readonly int DEFAULT_VALUE = 5000;

        private double delay;
        private bool enabled;
        private DateTime lastTime;


        public TimeDelayTimerNode() : base("Time", "Delay Timer")
        {
            AddInput("Delay", DataType.Number);
            AddInput("Start", DataType.Logical);
            AddOutput(DataType.Number);

            lastTime = DateTime.Now;
            delay = DEFAULT_VALUE;
        }

        public override void Loop()
        {
            if (!enabled || delay <= 0)
                return;

            var elapsed = DateTime.Now - lastTime;
            if (elapsed.TotalMilliseconds >= delay)
            {
                lastTime = DateTime.Now;

                LogInfo($"Time trigger");

                Outputs[0].Value = "1";
                enabled = false;
            }
        }

        public override void OnInputChange(Input input)
        {
            if (input == Inputs[0])
            {
                if (input.Value == null)
                    delay = DEFAULT_VALUE;
                else
                    double.TryParse(input.Value, out delay);

                if (delay < 0)
                    delay = 0;

                lastTime = DateTime.Now;

                LogInfo($"Delay changed to {delay} ms");
            }

            if (input == Inputs[1])
            {
                if (input.Value != "0")
                {
                    enabled = true;
                    lastTime = DateTime.Now;
                }
                else
                {
                    enabled = false;
                    Outputs[0].Value = "0";
                }

                LogInfo(enabled ? "Started" : "Stopped, reseted");
            }

            /*if (input == Inputs[2])
            {
                if (input.Value != "1")
                    return;

                lastTime = DateTime.Now;
                Outputs[0].Value = "0";

                LogInfo("Reset");
            }*/
        }
    }
}