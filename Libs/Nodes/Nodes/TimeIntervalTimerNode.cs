/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace MyNetSensors.Nodes
{
    public class TimeIntervalTimerNode : Node
    {
        private readonly int DEFAULT_INTERVAL = 1000;

        private bool enabled;

        private double interval;

        private readonly Timer timer = new Timer();

        private DateTime lastPercentUpdateTime;
        private DateTime startTime;


        public TimeIntervalTimerNode() : base("Time", "Interval Timer")
        {
            AddInput("Interval", DataType.Number, true);
            AddInput("Start/Stop", DataType.Logical, true);

            AddOutput("Trigger", DataType.Logical);
            AddOutput("Enabled", DataType.Logical);
            AddOutput("Progress", DataType.Number);

            Outputs[0].Value = "0";
            Outputs[1].Value = "0";
            Outputs[2].Value = "0";

            interval = DEFAULT_INTERVAL;

            timer = new Timer();
            timer.Interval = interval;
            timer.Elapsed += TimerElapsed;
            timer.Start();

            Settings.Add("PercentUpdateInterval", new NodeSetting(NodeSettingType.Number, "Percent Output Update Interval", "100"));
            lastPercentUpdateTime = DateTime.Now;
            options.LogOutputChanges = false;
        }

        public override void Loop()
        {
            if (!enabled)
                return;

            double updateInterval;
            double.TryParse(Settings["PercentUpdateInterval"].Value, out updateInterval);

            if ((DateTime.Now - lastPercentUpdateTime).TotalMilliseconds < updateInterval)
                return;

            lastPercentUpdateTime = DateTime.Now;

            double elapsed = (DateTime.Now - startTime).TotalMilliseconds;
            double percent = elapsed / interval * 100;
            if (percent > 100)
                percent = 100;
            String sperc = percent.ToString("0.##");
            if (sperc != Outputs[2].Value)
            {
                Outputs[2].Value = percent.ToString("0.##");
            }
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (!enabled)
                return;

            enabled = false;
            timer.Stop();

            LogInfo("Elapsed");

            Outputs[0].Value = "1";
            Outputs[1].Value = "0";
            Outputs[2].Value = "100";
        }


        public override void OnInputChange(Input input)
        {
            if (input == Inputs[0])
            {
                timer.Stop();

                if (input.Value == null)
                    interval = DEFAULT_INTERVAL;
                else
                    double.TryParse(input.Value, out interval);

                if (interval < 1)
                    interval = 1;

                LogInfo($"Interval changed to {interval} ms");

                if (enabled)
                    Start();
            }


            if (input == Inputs[1])
            {
                if (input.Value == "1")
                    Start();
                else
                    Stop();
            }
        }


        public void Start()
        {
            timer.Stop();

            startTime = DateTime.Now;
            enabled = true;

            timer.Interval = interval;
            timer.Start();

            LogInfo("Start");

            Outputs[0].Value = "0";
            Outputs[1].Value = "1";
            Outputs[2].Value = "0";
        }


        public void Stop()
        {
            enabled = false;

            timer.Stop();

            LogInfo("Stop");

            Outputs[0].Value = "0";
            Outputs[1].Value = "0";
            Outputs[2].Value = "0";
        }

        public override string GetNodeDescription()
        {
            return "This node represents a timer. <br/>" +
                   "You can set the time interval and activate the timer, " +
                   "giving \"1\" to the input named \"Start/Stop\". <br/>" +
                   "After specified time interval, the output named \"Trigger\" sends \"1\". <br/>" +
                   "The output named \"Enabled\" sends \"1\" " +
                   "when the timer is in the active state. <br/>" +
                   "The output named \"Progress\" sends " +
                   "the current state of the timer in percentage " +
                   "(what percentage of the time interval has expired). ";
        }
    }
}