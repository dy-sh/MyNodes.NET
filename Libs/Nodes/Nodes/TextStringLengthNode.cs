//planer-pro copyright 2015 GPL - license.

namespace MyNetSensors.Nodes
{
    public class TextStringLengthNode : Node
    {
        public TextStringLengthNode() : base("Text", "String Length")
        {
            AddInput("Text");
            AddOutput("Length", DataType.Number);

            options.ResetOutputsIfAnyInputIsNull = true;
        }


        public override void OnInputChange(Input input)
        {
            Outputs[0].Value = Inputs[0].Value.Length.ToString();
        }

        public override string GetNodeDescription()
        {
            return "This node counts the number of characters in the given text string.";
        }
    }
}