//planer-pro copyright 2015 GPL - license.

namespace MyNetSensors.Nodes
{
    public class OperationConcatenationNode : Node
    {
        public OperationConcatenationNode() : base("Operation", "Concatenation")
        {
            AddInput();
            AddInput();
            AddOutput();
        }


        public override void OnInputChange(Input input)
        {
            var value = "";

            if (Inputs[0].Value == null && Inputs[1].Value == null) value = null;
            if (Inputs[0].Value != null) value += Inputs[0].Value;
            if (Inputs[1].Value != null) value += Inputs[1].Value;

            Outputs[0].Value = value;
        }
    }
}