/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Diagnostics;
using System.Timers;

namespace MyNetSensors.Nodes
{
    public class TimeIteratorNode : Node
    {
        public int CompletedCount { get; set; }

        private readonly int DEFAULT_INTERVAL = 1000;

        private bool state;

        private bool enabled = true;

        private double interval;

        private readonly Timer timer;
        private int count;


        public TimeIteratorNode() : base("Time", "Iterator")
        {
            AddInput("Enents Count", DataType.Number);
            AddInput("Interval", DataType.Number, true);
            AddInput("Start", DataType.Logical, true);
            AddInput("Stop", DataType.Logical, true);

            AddOutput("Trigger", DataType.Logical);
            AddOutput("Enabled", DataType.Logical);

            interval = DEFAULT_INTERVAL;

            Settings.Add("zero", new NodeSetting(NodeSettingType.Checkbox, "Generate Zero", "true"));

            timer = new Timer();
            timer.Interval = interval / 2;
            timer.Elapsed += TimerElapsed;
        }



        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (!enabled)
                return;

            state = !state;

            if (state)
                CompletedCount++;

            if (CompletedCount >= count)
            {
                LogInfo("Completed");
                Stop();
                return;
            }

            if (Settings["zero"].Value == "true")
                Outputs[0].Value = state ? "1" : "0";
            else if (state)
                Outputs[0].Value = "1";
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

                timer.Stop();
                timer.Interval = interval / 2;
                timer.Start();

                LogInfo($"Interval changed to {interval} ms");
            }

            if (input == Inputs[0])
            {
                if (input.Value == null)
                    Stop();
                else
                {
                    count = (int)double.Parse(input.Value);
                    LogInfo($"Count changed to [{count}]");
                }
            }


            if (input == Inputs[2] && input.Value == "1")
                Start();

            if (input == Inputs[3] && input.Value == "1")
                Stop();
        }

        public void Start()
        {
            if (Inputs[0].Value == null)
            {
                LogError("Cant start. Events count is not set.");
                return;
            }

            Stop();

            state = true;
            enabled = true;
            CompletedCount = 0;
            LogInfo("Start");

            Outputs[1].Value = "1";

            Outputs[0].Value = "1";
            timer.Start();
        }

        public void Stop()
        {
            if (!enabled)
                return;

            enabled = false;
            timer.Stop();

            LogInfo("Stop");

            Outputs[1].Value = "0";


            if (Settings["zero"].Value == "true")
                Outputs[0].Value = "0";
        }
    }
}