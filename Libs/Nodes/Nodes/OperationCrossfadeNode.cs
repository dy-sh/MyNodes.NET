//planer-pro copyright 2015 GPL - license.

namespace MyNetSensors.Nodes
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
    }
}