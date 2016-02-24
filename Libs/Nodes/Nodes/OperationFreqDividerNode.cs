/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;

namespace MyNetSensors.Nodes
{
    public class OperationFreqDividerNode : Node
    {
        public int Counter { get; set; } = -1;

        public OperationFreqDividerNode() : base("Operation", "Freq Divider")
        {
            AddInput("Divide by", DataType.Number);
            AddInput("Trigger", DataType.Logical);
            AddInput("Width %", DataType.Number,true);
            AddInput("Reset", DataType.Logical, true);
            AddOutput("Out");

            options.ResetOutputsIfAnyInputIsNull = true;
        }


        public override void OnInputChange(Input input)
        {

            if (input == Inputs[3] && input.Value == "1")
            {
                Counter = -1;
                Outputs[0].Value = "0";
                return;
            }

            if (input == Inputs[1] && input.Value == "1")
            {
                Counter++;

                double divideBy = double.Parse(Inputs[0].Value);

                double width = 50;
                if (Inputs[2].Value != null)
                    width = double.Parse(Inputs[2].Value);


                if (Counter >= divideBy)
                {
                    Counter = 0;
                }

                Outputs[0].Value = divideBy * (width / 100) > Counter ? "1" : "0";
            }
        }

        public override string GetNodeDescription()
        {
            return "This node divides the frequency. <br/>" +
                   "Input \"Devide by\" specifies the number of clock cycles. <br/>" +
                   "\"Width %\" input specifies the percentage width of the positive portion " +
                   "of the cycle (if not set it is 50%). <br/>" +
                   "Input \"Trigger\" toggles the clock cycles. <br/><br/>" +

                   "For example, \"Devide by\"=4. The \"Width %\" is not connected (50). <br/>" +
                   "Sending \"1\" constantly to the Trigger input, " +
                   "you will get the following sequence on output: 1100 1100 1100... <br/><br/>" +

                   "Or, for example, \"Devide by\"=10. The \"Width %\"=80. <br/>" +
                   "Switching Trigger you will get: 1111111100 1111111100 1111111100... " +
                   "(80% of 1, 20% of 0).";
        }
    }
}