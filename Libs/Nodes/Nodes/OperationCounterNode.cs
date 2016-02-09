/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;

namespace MyNetSensors.Nodes
{

    public class OperationCounterNode : Node
    {

        private int count = 0;
        private int result = 0;


        public OperationCounterNode() : base(3, 1)
        {
            this.Title = "Counter";
            this.Type = "Operation/Counter";

            Inputs[0].Name = "Set value";
            Inputs[1].Name = "Count Up";
            Inputs[2].Name = "Count Down";

            Inputs[0].Type = DataType.Number;
            Inputs[1].Type = DataType.Logical;
            Inputs[2].Type = DataType.Logical;
            Outputs[0].Type = DataType.Text;
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            if (input == Inputs[0])
            {
                if (Inputs[0].Value != null)
                {
                    count = 0;
                    result = Int32.Parse(Inputs[0].Value);

                    Outputs[0].Value = result.ToString();
                    LogInfo($"Count set at {result}");

                    return;
                }
                else
                {
                    count = 0;

                    Outputs[0].Value = null;
                    LogInfo($"Invalid set value");

                    return;
                }
            }

            if (input == Inputs[1])
            {
                if (input.Value == "1")
                {
                    if (Inputs[0].Value != null)
                    {
                        count++;
                        int faderValue = Int32.Parse(Inputs[0].Value);
                        result = faderValue + count;

                        Outputs[0].Value = result.ToString();
                        LogInfo($"Count = {result}");
                    }
                    else
                    {
                        count++;

                        Outputs[0].Value = count.ToString();
                        LogInfo($"Count = {count}");
                    }
                }
            }

            if (input == Inputs[2])
            {
                if (input.Value == "1")
                {
                    if (Inputs[0].Value != null)
                    {
                        count--;
                        int faderValue = Int32.Parse(Inputs[0].Value);
                        result = faderValue + count;

                        Outputs[0].Value = result.ToString();
                        LogInfo($"Count = {result}");
                    }

                    else
                    {
                        count--;

                        Outputs[0].Value = count.ToString();
                        LogInfo($"Count = {count}");
                    }
                }
            }
        }
    }
}