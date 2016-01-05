/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;


namespace MyNetSensors.LogicalNodes
{

    public class LogicalNodeCounter : LogicalNode
    {
        private int DEFAULT_VALUE = 1000;

        public int count = 0;
        private int? freqInput;
        private DateTime lastTime;

        /// <summary>
        /// Counter (1 input, 1 output). Input[0] - Frequency (ms). Default=1000.
        /// </summary>
        public LogicalNodeCounter() : base(1, 1)
        {
            this.Title = "Logic Counter";
            this.Type = "Logic/Counter";

            Inputs[0].Name = "Frequency";
            lastTime = DateTime.Now;
        }

        public override void Loop()
        {
            int freq = DEFAULT_VALUE;
            if (freqInput.HasValue)
                freq = freqInput.Value;

            if (freq <= 0) return;

            TimeSpan elapsed = DateTime.Now - lastTime;
            if (elapsed.TotalMilliseconds >= freq)
            {
                count++;

                Log($"Counter: {count}");

                Outputs[0].Value = count.ToString();
                lastTime = DateTime.Now;

            }
        }

        public override void OnInputChange(Input input)
        {
            try
            {
                freqInput = Int32.Parse(input.Value);
                Log($"Counter: frequency changed to {freqInput.Value} ms");
            }
            catch
            {
                freqInput = null;
                Log($"Counter: frequency changed to default value: {DEFAULT_VALUE} ms");
            }

        }


    }
}
