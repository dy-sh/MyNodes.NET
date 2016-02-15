using System.Globalization;

namespace MyNetSensors.Nodes.Nodes
{
    public class RgbRgbwToNumbersNode : Node
    {
        public RgbRgbwToNumbersNode() : base("RGB", "RGBW to Numbers", 1, 4)
        {
            Inputs[0].Type = DataType.Text;
            Outputs[0].Type = DataType.Number;
            Outputs[1].Type = DataType.Number;
            Outputs[2].Type = DataType.Number;
            Outputs[3].Type = DataType.Number;

            Inputs[0].Name = "RGB";
            Outputs[0].Name = "R";
            Outputs[1].Name = "G";
            Outputs[2].Name = "B";
            Outputs[3].Name = "W";

            options.ResetOutputsWhenAnyInputIsNull = true;
        }

        public override void OnInputChange(Input input)
        {
            try
            {
                var hexString = input.Value;

                if (hexString[0] == '#')
                    hexString = hexString.Remove(0, 1);

                var r = int.Parse(hexString.Substring(0, 2), NumberStyles.HexNumber);
                var g = int.Parse(hexString.Substring(2, 2), NumberStyles.HexNumber);
                var b = int.Parse(hexString.Substring(4, 2), NumberStyles.HexNumber);
                var w = int.Parse(hexString.Substring(6, 2), NumberStyles.HexNumber);

                Outputs[0].Value = r.ToString();
                Outputs[1].Value = g.ToString();
                Outputs[2].Value = b.ToString();
                Outputs[3].Value = w.ToString();
            }
            catch
            {
                LogIncorrectInputValueError(input);
                ResetOutputs();
            }
        }
    }
}