//planer-pro copyright 2015 GPL - license.

namespace MyNodes.Nodes
{
    public class TextASCIICodeNode : Node
    {
        public TextASCIICodeNode() : base("Text", "ASCII Code")
        {
            AddInput("ASCII Char", DataType.Text);
            AddOutput("Code",DataType.Number);

            options.ResetOutputsIfAnyInputIsNull = true;
        }


        public override void OnInputChange(Input input)
        {
            if (Inputs[0].Value.Length < 1)
            {
                ResetOutputs();
                return;
            }

            char c = Inputs[0].Value[0];
            Outputs[0].Value = ((int)c).ToString();
        }

        public override string GetNodeDescription()
        {
            return "This node generates ASCII code from the symbol.";
        }
    }
}