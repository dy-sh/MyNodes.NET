//planer-pro copyright 2015 GPL - license.

namespace MyNetSensors.Nodes
{
    public class OperationCrossfadeNode : Node
    {
        public OperationCrossfadeNode() : base("Operation", "Crossfade", 3, 1)
        {
            Inputs[0].Type = DataType.Number;
            Inputs[1].Type = DataType.Number;
            Inputs[2].Type = DataType.Number;
            Outputs[0].Type = DataType.Number;

            Inputs[0].Name = "Crossfade";
            Inputs[1].Name = "A";
            Inputs[2].Name = "B";

            options.ResetOutputsWhenAnyInputIsNull = true;
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