//planer-pro copyright 2015 GPL - license.

namespace MyNetSensors.Nodes
{
    public class TextASCIICharNode : Node
    {
        public TextASCIICharNode() : base("Text", "ASCII Char")
        {
            AddInput("ASCII Code",DataType.Number);
            AddOutput("Char");

            options.ResetOutputsIfAnyInputIsNull = true;
        }


        public override void OnInputChange(Input input)
        {
            int code = (int)double.Parse(Inputs[0].Value);
            Outputs[0].Value = ((char)code).ToString();
        }

        public override string GetNodeDescription()
        {
            return "This node generates symbol from the ASCII code.";
        }
    }
}