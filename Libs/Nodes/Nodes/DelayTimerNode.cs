/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;

namespace MyNetSensors.Nodes
{

    public class DelayTimerNode : Node
    {
        private int DEFAULT_VALUE = 5000;

        private double delay;
        private bool enabled = false;
        private DateTime lastTime;


        public DelayTimerNode() : base(2, 1)
        {
            this.Title = "Delay Timer";
            this.Type = "Delay/Delay Timer";

            Inputs[0].Name = "Delay";
            Inputs[1].Name = "Start";
            //Inputs[2].Name = "Reset";

            Inputs[0].Type = DataType.Number;
            Inputs[1].Type = DataType.Logical;
            //Inputs[2].Type = DataType.Logical;
            Outputs[0].Type = DataType.Number;

            lastTime = DateTime.Now;
            delay = DEFAULT_VALUE;
        }

        public override void Loop()
        {
            if (!enabled || delay <= 0)
                return;

            TimeSpan elapsed = DateTime.Now - lastTime;
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
                    Double.TryParse(input.Value, out delay);

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

                LogInfo(enabled ? "Started" : "Stopped, reseted" );
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
