using System.Globalization;

namespace MyNetSensors.Nodes.Nodes
{
    public class RgbRgbRemapNode : Node
    {
        public RgbRgbRemapNode() : base("RGB", "RGB Remap", 5, 1)
        {
            Inputs[0].Name = "RGB Value";
            Inputs[1].Name = "RGB InMin";
            Inputs[2].Name = "RGB InMax";
            Inputs[3].Name = "RGB OutMin";
            Inputs[4].Name = "RGB OutMax";

            Inputs[0].Type = DataType.Text;
            Inputs[1].Type = DataType.Text;
            Inputs[2].Type = DataType.Text;
            Inputs[3].Type = DataType.Text;
            Inputs[4].Type = DataType.Text;
            Outputs[0].Type = DataType.Text;

            options.ResetOutputsWhenAnyInputIsNull = true;
        }


        public override void OnInputChange(Input input)
        {
            try
            {
                var valueRGB = Inputs[0].Value;
                var inMinRGB = Inputs[1].Value;
                var inMaxRGB = Inputs[2].Value;
                var outMinRGB = Inputs[3].Value;
                var outMaxRGB = Inputs[4].Value;


                if (valueRGB[0] == '#')
                    valueRGB = valueRGB.Remove(0, 1);

                if (inMinRGB[0] == '#')
                    inMinRGB = inMinRGB.Remove(0, 1);

                if (inMaxRGB[0] == '#')
                    inMaxRGB = inMaxRGB.Remove(0, 1);

                if (outMinRGB[0] == '#')
                    outMinRGB = outMinRGB.Remove(0, 1);

                if (outMaxRGB[0] == '#')
                    outMaxRGB = outMaxRGB.Remove(0, 1);

                var resultRGB = "";

                for (var i = 0; i < 3; i++)
                {
                    var value = int.Parse(valueRGB.Substring(i*2, 2), NumberStyles.HexNumber);
                    var inMin = int.Parse(inMinRGB.Substring(i*2, 2), NumberStyles.HexNumber);
                    var inMax = int.Parse(inMaxRGB.Substring(i*2, 2), NumberStyles.HexNumber);
                    var outMin = int.Parse(outMinRGB.Substring(i*2, 2), NumberStyles.HexNumber);
                    var outMax = int.Parse(outMaxRGB.Substring(i*2, 2), NumberStyles.HexNumber);

                    var result = (int) Remap(value, inMin, inMax, outMin, outMax);
                    result = Clamp(result, 0, 255);

                    resultRGB += result.ToString("X2");
                }

                Outputs[0].Value = resultRGB;
            }
            catch
            {
                LogError($"Incorrect value in input.");
                ResetOutputs();
            }
        }

        private double Remap(double value, double inMin, double inMax, double outMin, double outMax)
        {
            return (value - inMin)/(inMax - inMin)*(outMax - outMin) + outMin;
        }

        private int Clamp(int value, int min, int max)
        {
            return value < min ? min : value > max ? max : value;
        }
    }
}