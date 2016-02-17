﻿using System.Globalization;

namespace MyNetSensors.Nodes.Nodes
{
    public class RgbRgbwToNumbersNode : Node
    {
        public RgbRgbwToNumbersNode() : base("RGB", "RGBW to Numbers")
        {
            AddInput("RGB");

            AddOutput("R", DataType.Number);
            AddOutput("G", DataType.Number);
            AddOutput("B", DataType.Number);
            AddOutput("W", DataType.Number);
         
            options.ResetOutputsIfAnyInputIsNull = true;
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