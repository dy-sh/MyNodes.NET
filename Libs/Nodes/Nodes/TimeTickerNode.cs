/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;
using System.Timers;

namespace MyNetSensors.Nodes
{
    public class TimeTickerNode : Node
    {
        private readonly int DEFAULT_INTERVAL = 1000;

        private bool state;

        private bool enabled = true;

        private double interval;

        private readonly Timer timer = new Timer();

        public TimeTickerNode() : base("Time", "Ticker")
        {
            AddInput("Interval", DataType.Number, true);
            AddInput("Start/Stop", DataType.Logical, true);

            AddOutput("Out", DataType.Logical);

            interval = DEFAULT_INTERVAL;

            Settings.Add("zero", new NodeSetting(NodeSettingType.Checkbox, "Generate Zero", "true"));

            timer = new Timer();
            timer.Interval = interval / 2;
            timer.Elapsed += TimerElapsed;
            timer.Start();
        }



        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (!enabled)
                return;

            state = !state;

            if (Settings["zero"].Value == "true" )
                Outputs[0].Value = state ? "1" : "0";
            else if (state)
                Outputs[0].Value = "1";
        }

        public override void OnInputChange(Input input)
        {
            if (input == Inputs[0])
            {
                if (input.Value == null)
                    interval = DEFAULT_INTERVAL;
                else
                    double.TryParse(input.Value, out interval);

                if (interval < 1)
                    interval = 1;

                timer.Stop();
                timer.Interval = interval / 2;
                timer.Start();

                LogInfo($"Interval changed to {interval} ms");
            }


            if (input == Inputs[1])
            {
                enabled = input.Value == "1" || input.Value == null;
                timer.Enabled = enabled;
                state = enabled;
                LogInfo(enabled ? "Start" : "Stop");
                Outputs[0].Value = state ? "1" : "0";

            }
        }
    }
}