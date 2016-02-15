//planer-pro copyright 2015 GPL - license.

using System;

namespace MyNetSensors.Nodes
{
    public class MathRoundNode : Node
    {
        public MathRoundNode() : base("Math", "Round")
        {
            AddInput(DataType.Number);
            AddOutput(DataType.Number);

            options.ResetOutputsIfAnyInputIsNull = true;
        }


        public override void OnInputChange(Input input)
        {
            var a = double.Parse(Inputs[0].Value);
            var b = (int) Math.Round(a);

            Outputs[0].Value = b.ToString();
        }
    }
}