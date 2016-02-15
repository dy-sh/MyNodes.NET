namespace MyNetSensors.Nodes.Nodes
{
    public class RgbNumbersToRgbNode : Node
    {
        public RgbNumbersToRgbNode() : base("RGB", "Numbers to RGB", 3, 1)
        {
            Inputs[0].Type = DataType.Number;
            Inputs[1].Type = DataType.Number;
            Inputs[2].Type = DataType.Number;
            Outputs[0].Type = DataType.Text;

            Inputs[0].Name = "R";
            Inputs[1].Name = "G";
            Inputs[2].Name = "B";
            Outputs[0].Name = "RGB";

            options.ResetOutputsWhenAnyInputIsNull = true;
        }

        public override void OnInputChange(Input input)
        {
            var r = (int) double.Parse(Inputs[0].Value);
            var g = (int) double.Parse(Inputs[1].Value);
            var b = (int) double.Parse(Inputs[2].Value);

            if (r < 0 || r > 255)
            {
                LogIncorrectInputValueError(Inputs[0]);
                ResetOutputs();
                return;
            }
            if (g < 0 || g > 255)
            {
                LogIncorrectInputValueError(Inputs[1]);
                ResetOutputs();
                return;
            }
            if (b < 0 || b > 255)
            {
                LogIncorrectInputValueError(Inputs[2]);
                ResetOutputs();
                return;
            }

            var result = r.ToString("X2")
                         + g.ToString("X2")
                         + b.ToString("X2");

            Outputs[0].Value = result;
        }
    }
}