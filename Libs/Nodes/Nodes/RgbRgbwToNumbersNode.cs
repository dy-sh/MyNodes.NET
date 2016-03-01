/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Globalization;

namespace MyNodes.Nodes.Nodes
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

        public override string GetNodeDescription()
        {
            return "This node converts RGBW color to four numbers. <br/>" +
                   "For example: \"FFAA00FF\" will be converted to 255, 170, 0, 255. <br/>" +
                   "Node takes color with a # sign at the beginning or without it.";
        }
    }
}