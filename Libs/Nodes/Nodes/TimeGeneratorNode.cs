/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Diagnostics;
using System.Timers;

namespace MyNetSensors.Nodes
{

    public class TimeGeneratorNode : Node
    {
        private int DEFAULT_VALUE = 1000;

        public int Count { get; set; }

        private double frequency;
        private bool enabled = true;

        private Timer timer = new Timer();

        public TimeGeneratorNode() : base("Time","Generator",3, 1)
        {
            Inputs[0].Name = "Frequency";
            Inputs[1].Name = "Start";
            Inputs[2].Name = "Reset";

            Inputs[0].Type = DataType.Number;
            Inputs[1].Type = DataType.Logical;
            Inputs[2].Type = DataType.Logical;
            Outputs[0].Type = DataType.Number;

            frequency = DEFAULT_VALUE;

            timer = new Timer();
            timer.Interval = frequency;
            timer.Elapsed += TimerElapsed;
            timer.Start();
        }


        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (!enabled || frequency <= 0)
                return;

            Count = 1 - Count;

            Outputs[0].Value = Count.ToString();
        }

        public override void Loop()
        {

        }

        public override void OnInputChange(Input input)
        {
            if (input == Inputs[0])
            {
                if (input.Value == null)
                    frequency = DEFAULT_VALUE;
                else
                    Double.TryParse(input.Value, out frequency);

                if (frequency < 1)
                    frequency = 1;

                timer.Stop();
                timer.Interval = frequency;
                timer.Start();

                LogInfo($"Frequency changed to {frequency} ms");
            }


            if (input == Inputs[1])
            {
                enabled = input.Value != "0";
                timer.Enabled = enabled;

                LogInfo(enabled ? "Started" : "Stopped");
            }


            if (input == Inputs[2])
            {
                if (input.Value != "1")
                    return;

                timer.Stop();
                timer.Start();

                Count = 0;
                LogInfo("Reset");
                Outputs[0].Value = "0";
            }
        }


    }
}
