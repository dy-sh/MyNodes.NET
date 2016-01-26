/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;

namespace MyNetSensors.Nodes
{

    public class CounterNode : Node
    {
        private int DEFAULT_VALUE = 1000;

        public int Count { get; set; }

        private double frequency;
        private bool enabled = true;
        private DateTime lastTime;

        /// <summary>
        /// Counter (1 input, 1 output). Input[0] - Frequency (ms). Default=1000.
        /// </summary>
        public CounterNode() : base(3, 1)
        {
            this.Title = "Counter";
            this.Type = "Operation/Counter";

            Inputs[0].Name = "Frequency";
            Inputs[1].Name = "Enable";
            Inputs[2].Name = "Reset";
            lastTime = DateTime.Now;
            frequency = DEFAULT_VALUE;
        }

        public override void Loop()
        {
            if (!enabled || frequency<=0)
                return;

            TimeSpan elapsed = DateTime.Now - lastTime;
            if (elapsed.TotalMilliseconds >= frequency)
            {
                Count++;
                lastTime = DateTime.Now;

                LogInfo($"{Count}");

                Outputs[0].Value = Count.ToString();
            }
        }

        public override void OnInputChange(Input input)
        {
            if (!CheckIsDoubleOrNull(Inputs[0].Value))
            {
                LogIncorrectInputValueError(input);
                return;
            }

            if (!CheckIsBoolOrNull(Inputs[1].Value))
            {
                LogIncorrectInputValueError(input);
                return;
            }

            if (!CheckIsBoolOrNull(Inputs[2].Value))
            {
                LogIncorrectInputValueError(input);
                return;
            }

            if (input == Inputs[0])
            {
                if (input.Value == null)
                    frequency = DEFAULT_VALUE;
                else
                    Double.TryParse(input.Value, out frequency);

                if (frequency < 0)
                    frequency = 0;

                LogInfo($"Frequency changed to {frequency} ms");

            }

            if (input == Inputs[1])
            {
                enabled = input.Value != "0";

                LogInfo(enabled ? "Started" : "Stopped");
            }

            if (input == Inputs[2] && input.Value == "1")
            {
                Count = 0;
                lastTime = DateTime.Now;
                LogInfo("Reset");
            }
        }


    }
}
