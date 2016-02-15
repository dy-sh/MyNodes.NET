namespace MyNetSensors.Nodes.Nodes
{
    public class RgbNumbersToRgbwNode : Node
    {
        public RgbNumbersToRgbwNode() : base("RGB", "Numbers to RGBW", 4, 1)
        {
            Inputs[0].Type = DataType.Number;
            Inputs[1].Type = DataType.Number;
            Inputs[2].Type = DataType.Number;
            Inputs[3].Type = DataType.Number;
            Outputs[0].Type = DataType.Text;

            Inputs[0].Name = "R";
            Inputs[1].Name = "G";
            Inputs[2].Name = "B";
            Inputs[3].Name = "W";
            Outputs[0].Name = "RGB";

            options.ResetOutputsWhenAnyInputIsNull = true;
        }

        public override void OnInputChange(Input input)
        {
            var r = (int) double.Parse(Inputs[0].Value);
            var g = (int) double.Parse(Inputs[1].Value);
            var b = (int) double.Parse(Inputs[2].Value);
            var w = (int) double.Parse(Inputs[3].Value);

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
            if (w < 0 || w > 255)
            {
                LogIncorrectInputValueError(Inputs[3]);
                ResetOutputs();
                return;
            }

            var result = r.ToString("X2")
                         + g.ToString("X2")
                         + b.ToString("X2")
                         + w.ToString("X2");

            Outputs[0].Value = result;
        }
    }
}