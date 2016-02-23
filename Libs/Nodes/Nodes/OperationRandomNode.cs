//planer-pro copyright 2015 GPL - license.

using System;

namespace MyNetSensors.Nodes
{
    public class OperationRandomNode : Node
    {
        private readonly int DEFAULT_MAX = 100;
        private readonly int DEFAULT_MIN = 0;


        public OperationRandomNode() : base("Operation", "Random")
        {
            AddInput("Trigger", DataType.Logical);
            AddInput("Min Value", DataType.Number,true);
            AddInput("Max Value", DataType.Number, true);
            AddOutput("Out");
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

        public override string GetNodeDescription()
        {
            return "This node generates random values. " +
                   "You can set the minimum and maximum limit.";
        }


    }
}