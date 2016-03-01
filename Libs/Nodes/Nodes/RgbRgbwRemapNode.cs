/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Globalization;

namespace MyNodes.Nodes.Nodes
{
    public class RgbRgbwRemapNode : Node
    {
        public RgbRgbwRemapNode() : base("RGB", "RGBW Remap")
        {
            AddInput("RGBW Value");
            AddInput("RGBW InMin");
            AddInput("RGBW InMax");
            AddInput("RGBW OutMin");
            AddInput("RGBW OutMax");

            AddOutput("RGBW");

            options.ResetOutputsIfAnyInputIsNull = true;
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

                for (var i = 0; i < 4; i++)
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

        public override string GetNodeDescription()
        {
            return "This node works the same way as Math/Remap, " +
                   "but accepts and outputs RGBW color. <br/>" +
                   "Using this node, you can replace the white color to other (FFFFFFFF to AABBCCFF). " +
                   "Or, for example, to exclude red color (FFFFFFFF to 00FFFFFF).";
        }
    }
}