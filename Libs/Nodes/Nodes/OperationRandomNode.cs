//planer-pro copyright 2015 GPL - license.

using System;

namespace MyNetSensors.Nodes
{
    public class OperationRandomNode : Node
    {
        private readonly int DEFAULT_MAX = 100;
        private readonly int DEFAULT_MIN = 0;


        public OperationRandomNode() : base("Operation", "Random", 3, 1)
        {
            Inputs[0].Name = "Start";
            Inputs[1].Name = "Min Value";
            Inputs[2].Name = "Max Value";

            Inputs[0].Type = DataType.Logical;
            Inputs[1].Type = DataType.Number;
            Inputs[2].Type = DataType.Number;
            Outputs[0].Type = DataType.Number;
        }


        public override void OnInputChange(Input input)
        {
            if (input == Inputs[0] && Inputs[0].Value == "1")
            {
                var rand = new Random(DateTime.Now.Millisecond);

                var min = Inputs[1].Value == null ? DEFAULT_MIN*100 : (int) (double.Parse(Inputs[1].Value)*100);
                var max = Inputs[2].Value == null ? DEFAULT_MAX*100 : (int) (double.Parse(Inputs[2].Value)*100);

                double rnd = rand.Next(min, max);
                rnd = rnd/100;

                Outputs[0].Value = rnd.ToString();
            }
        }
    }
}