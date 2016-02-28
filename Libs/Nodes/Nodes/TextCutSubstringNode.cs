//planer-pro copyright 2015 GPL - license.

namespace MyNetSensors.Nodes
{
    public class TextCutSubstringNode : Node
    {
        public TextCutSubstringNode() : base("Text", "Cut Substring")
        {
            AddInput("Text");
            AddInput("Length", DataType.Number);
            AddInput("Start Index", DataType.Number, true);
            AddOutput("Text");

            options.ResetOutputsIfAnyInputIsNull = true;
        }


        public override void OnInputChange(Input input)
        {
            try
            {
                int startIndex = 0;
                if (Inputs[2].Value != null)
                    startIndex = (int)double.Parse(Inputs[2].Value);

                int length = (int)double.Parse(Inputs[1].Value);

                Outputs[0].Value = Inputs[0].Value.Substring(startIndex, length);
            }
            catch
            {
                ResetOutputs();
            }
        }

        public override string GetNodeDescription()
        {
            return "Cuts out a substring from a text string. <br/>" +
                   "You can specify a starting position from which to begin cutting, and length.";
        }
    }
}