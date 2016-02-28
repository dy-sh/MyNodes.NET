//planer-pro copyright 2015 GPL - license.

namespace MyNetSensors.Nodes
{
    public class TextCharAtIndexNode : Node
    {
        public TextCharAtIndexNode() : base("Text", "Char At Index")
        {
            AddInput("Text");
            AddInput("Index", DataType.Number);
            AddOutput("Char");

            options.ResetOutputsIfAnyInputIsNull = true;
        }


        public override void OnInputChange(Input input)
        {
            try
            {
                int index = (int)double.Parse(Inputs[1].Value);

                char c = Inputs[0].Value[index];
                Outputs[0].Value = c.ToString();
            }
            catch
            {
                ResetOutputs();
            }
        }

        public override string GetNodeDescription()
        {
            return "This node takes the character from a text string at the specified index (position).";
        }
    }
}