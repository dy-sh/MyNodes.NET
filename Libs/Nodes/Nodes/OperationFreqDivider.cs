//planer-pro copyright 2015 GPL - license.

using System;

namespace MyNetSensors.Nodes
{
    public class OperationFreqDivider : Node
    {
        public int Counter { get; set; } = -1;

        public OperationFreqDivider() : base("Operation", "Freq Divider", 4, 1)
        {
            Inputs[0].Name = "Divide by";
            Inputs[1].Name = "Width %";
            Inputs[2].Name = "Trigger";
            Inputs[3].Name = "Reset";

            Inputs[0].Type = DataType.Number;
            Inputs[1].Type = DataType.Number;
            Inputs[2].Type = DataType.Logical;
            Inputs[3].Type = DataType.Logical;
            Outputs[0].Type = DataType.Logical;
        }


        public override void OnInputChange(Input input)
        {
            if (Inputs[0].Value == null || Inputs[2].Value == null)
            {
                ResetOutputs();
                return;
            }

            if (input == Inputs[3] && input.Value == "1")
            {
                Counter = -1;
                Outputs[0].Value = "0";
                return;
            }

            if (input == Inputs[2] && input.Value == "1")
            {
                Counter++;

                double divideBy = double.Parse(Inputs[0].Value);

                double width = 50;
                if (Inputs[1].Value != null)
                    width = double.Parse(Inputs[1].Value);


                if (Counter >= divideBy)
                {
                    Counter = 0;
                }

                Outputs[0].Value = divideBy * (width / 100) > Counter ? "1" : "0";
            }
        }
    }
}