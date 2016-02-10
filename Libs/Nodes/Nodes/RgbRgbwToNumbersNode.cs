using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.Nodes.Nodes
{
    public class RgbRgbwToNumbersNode : Node
    {

        public RgbRgbwToNumbersNode() : base(1, 4)
        {
            this.Title = "RGBW to Numbers";
            this.Type = "RGB/RGBW to Numbers";

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
        }
        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            if (input.Value == null)
            {
                ResetOutputs();
                return;
            }

            try
            {
                string hexString = input.Value;

                if (hexString[0] == '#')
                    hexString = hexString.Remove(0, 1);

                int r = int.Parse(hexString.Substring(0, 2), NumberStyles.HexNumber);
                int g = int.Parse(hexString.Substring(2, 2), NumberStyles.HexNumber);
                int b = int.Parse(hexString.Substring(4, 2), NumberStyles.HexNumber);
                int w = int.Parse(hexString.Substring(6, 2), NumberStyles.HexNumber);

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
