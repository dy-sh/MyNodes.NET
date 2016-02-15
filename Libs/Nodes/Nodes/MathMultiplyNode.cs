//planer-pro copyright 2015 GPL - license.

namespace MyNetSensors.Nodes
{
    public class MathMultiplyNode : Node
    {
        public MathMultiplyNode() : base("Math", "Multiply", 2, 1)
        {
            Inputs[0].Type = DataType.Number;
            Inputs[1].Type = DataType.Number;
            Outputs[0].Type = DataType.Number;

            options.ResetOutputsWhenAnyInputIsNull = true;
        }

        public override void OnInputChange(Input input)
        {
            var a = double.Parse(Inputs[0].Value);
            var b = double.Parse(Inputs[1].Value);
            var c = a*b;

            Outputs[0].Value = c.ToString();
        }
    }
}