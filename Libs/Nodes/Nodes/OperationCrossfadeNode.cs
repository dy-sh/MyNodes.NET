//planer-pro copyright 2015 GPL - license.

namespace MyNodes.Nodes
{
    public class OperationCrossfadeNode : Node
    {
        public OperationCrossfadeNode() : base("Operation", "Crossfade")
        {
            AddInput("Crossfade", DataType.Number);
            AddInput("A", DataType.Number);
            AddInput("B", DataType.Number);
            AddOutput("Out", DataType.Number);

            options.ResetOutputsIfAnyInputIsNull = true;
        }

        public override void OnInputChange(Input input)
        {
            var xf = double.Parse(Inputs[0].Value);
            var a = double.Parse(Inputs[1].Value);
            var b = double.Parse(Inputs[2].Value);

            var xout = a*(1 - xf/100) + b*xf/100;

            Outputs[0].Value = xout.ToString();
        }

        public override string GetNodeDescription()
        {
            return "This node makes the crossfade between two values. <br/>" +
                   "\"Crossfade\" input takes a value from 0 to 100. <br/>" +
                   "If Crossfade is 0, the output will be equal to A. <br/>" +
                   "If Crossfade is 100, then the output is equal to B. <br/>" +
                   "The intermediate value between 0 and 100 will give " +
                   "intermediate number between A and B. ";
        }
    }
}