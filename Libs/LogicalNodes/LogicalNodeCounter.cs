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
        private int count = 0;
        private int freq = 1000;
        private DateTime lastTime;

        /// <summary>
        /// Counter (1 input, 1 output). Input[0] - Frequency (ms). Default=1000.
        /// </summary>
        public LogicalNodeCounter() : base(1, 2)
        {
            Inputs[0].Name = "Frequency";
            lastTime = DateTime.Now;
        }

        public override void Loop()
        {
            if (freq <= 0) return;

            TimeSpan elapsed = DateTime.Now - lastTime;
            if (elapsed.TotalMilliseconds >= freq)
            {
                count++;

                Debug($"Counter: {count}");

                Outputs[0].Value = count.ToString();
                lastTime = DateTime.Now;

            }
        }

        public override void OnInputChange(Input input)
        {
            freq = Int32.Parse(input.Value);

            Debug($"Counter: frequency changed to {freq}");
        }


    }
}
