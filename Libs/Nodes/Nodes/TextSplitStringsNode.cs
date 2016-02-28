//planer-pro copyright 2015 GPL - license.

namespace MyNetSensors.Nodes
{
    public class TextSplitStringsNode : Node
    {
        private char separator;
        private int currentIndex;
        private string[] splittedText;

        public TextSplitStringsNode() : base("Text", "Split Strings")
        {
            AddInput("Text");
            AddInput("Separator");
            AddInput("Start",DataType.Logical,true);
            AddInput("Next",DataType.Logical, true);
            AddInput("Reset",DataType.Logical, true);
            AddOutput("Text");
            AddOutput("Left", DataType.Number);

            Outputs[1].Value = "0";

        }


        public override void OnInputChange(Input input)
        {
            if (input == Inputs[2] && input.Value == "1")
            {
                if (Inputs[0].Value ==null || Inputs[1].Value == null || Inputs[1].Value.Length < 1)
                {
                    Reset();
                    return;
                }

                separator = Inputs[1].Value[0];
                currentIndex = 0;
                splittedText = Inputs[0].Value.Split(separator);

                SplitNext();
            }

            if (input == Inputs[3] && input.Value == "1")
                SplitNext();

            if (input == Inputs[4] && input.Value == "1")
                Reset();

        }


        private void SplitNext()
        {
            if (splittedText == null || currentIndex >= splittedText.Length)
            {
                Reset();
                return;
            }

            Outputs[0].Value = splittedText[currentIndex];
            Outputs[1].Value = (splittedText.Length- currentIndex-1).ToString();

            currentIndex++;
        }


        private void Reset()
        {
            currentIndex = 0;
            splittedText = null;
            Outputs[0].Value = null;
            Outputs[1].Value = "0";
        }


        public override string GetNodeDescription()
        {
            return "This node splits the text into several strings using the separator. <br/>" +
                   "Send \"1\" to \"Start\" to begin the separation. The incoming string is stored. " +
                   "Next send \"1\" to \"Next\" to get the next line. <br/>" +
                   "The result strings are sequentially sent to the output named \"Text\". " +
                   "The output named \"Left\" reports how many strings are left.";
        }
    }
}