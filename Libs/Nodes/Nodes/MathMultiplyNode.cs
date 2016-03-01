//planer-pro copyright 2015 GPL - license.

namespace MyNodes.Nodes
{
    public class MathMultiplyNode : Node
    {
        public MathMultiplyNode() : base("Math", "Multiply")
        {
            AddInput(DataType.Number);
            AddInput(DataType.Number);
            AddOutput(DataType.Number);

            options.ResetOutputsIfAnyInputIsNull = true;
        }

        public override void OnInputChange(Input input)
        {
            var a = double.Parse(Inputs[0].Value);
            var b = double.Parse(Inputs[1].Value);
            var c = a*b;

            Outputs[0].Value = c.ToString();
        }

        public override string GetNodeDescription()
        {
            return "This node multiplies one number by another.";
        }
    }
}