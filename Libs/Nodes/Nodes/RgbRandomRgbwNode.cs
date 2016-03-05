//planer-pro copyright 2015 GPL - license.

using System;
using System.Globalization;

namespace MyNodes.Nodes
{
    public class RgbRandomRgbwNode : Node
    {
        private readonly string DEFAULT_MIN = "00000000";
        private readonly string DEFAULT_MAX = "FFFFFFFF";


        public RgbRandomRgbwNode() : base("RGB", "Random RGBW")
        {
            AddInput("Trigger", DataType.Logical);
            AddInput("Min RGBW", DataType.Text, true);
            AddInput("Max RGBW", DataType.Text, true);
            AddOutput("RGBW");

            options.ResetOutputsIfAnyInputIsNull = true;
        }


        public override void OnInputChange(Input input)
        {
            if (input == Inputs[0] && Inputs[0].Value == "1")
            {
                try
                {
                    var inMinRGB = Inputs[1].Value == null ? DEFAULT_MIN : Inputs[1].Value;
                    var inMaxRGB = Inputs[2].Value == null ? DEFAULT_MAX : Inputs[2].Value;

                    if (inMinRGB[0] == '#')
                        inMinRGB = inMinRGB.Remove(0, 1);

                    if (inMaxRGB[0] == '#')
                        inMaxRGB = inMaxRGB.Remove(0, 1);


                    var resultRGB = "";

                    var rand = new Random(DateTime.Now.Millisecond);

                    for (var i = 0; i < 4; i++)
                    {
                        int min = int.Parse(inMinRGB.Substring(i * 2, 2), NumberStyles.HexNumber);
                        int max = int.Parse(inMaxRGB.Substring(i * 2, 2), NumberStyles.HexNumber);

                        int rnd = rand.Next(min, max + 1);
                        rnd = Clamp(rnd, 0, 255);

                        resultRGB += rnd.ToString("X2");
                    }

                    Outputs[0].Value = resultRGB;
                }
                catch
                {
                    LogError($"Incorrect value in input.");
                    ResetOutputs();
                }
            }
        }


        private int Clamp(int value, int min, int max)
        {
            return value < min ? min : value > max ? max : value;
        }


        public override string GetNodeDescription()
        {
            return "This node generates random RGBW color. <br/>" +
                   "To generate color, send \"1\" to \"Trigger\" input. <br/>" +
                   "You can set the minimum and maximum color.";
        }


    }
}