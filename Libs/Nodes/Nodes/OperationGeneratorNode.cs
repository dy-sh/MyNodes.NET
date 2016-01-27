/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;

namespace MyNetSensors.Nodes
{

    public class OperationGeneratorNode : Node
    {
        private int DEFAULT_VALUE = 1000;

        public int Count { get; set; }

        private double frequency;
        private bool enabled = true;
        private DateTime lastTime;
        

        public OperationGeneratorNode() : base(3, 1)
        {
            this.Title = "Generator";
            this.Type = "Operation/Generator";

            Inputs[0].Name = "Frequency";
            Inputs[1].Name = "Start";
            Inputs[2].Name = "Reset";

            Inputs[0].Type = DataType.Number;
            Inputs[1].Type = DataType.Logical;
            Outputs[0].Type = DataType.Number;

            lastTime = DateTime.Now;
            frequency = DEFAULT_VALUE;
        }

        public override void Loop()
        {
            if (!enabled || frequency <= 0)
                return;

            TimeSpan elapsed = DateTime.Now - lastTime;
            if (elapsed.TotalMilliseconds >= frequency)
            {
                Count=1- Count;
                lastTime = DateTime.Now;

                LogInfo($"{Count}");

                Outputs[0].Value = Count.ToString();
            }
        }

        public override void OnInputChange(Input input)
        {
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
            

            if (input == Inputs[2])
            {
                if (input.Value != "1")
                    return;

                Count = 0;
                lastTime = DateTime.Now;
                LogInfo("Reset");
                Outputs[0].Value = "0";
            }
        }


    }
}
