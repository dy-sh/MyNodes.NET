/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes.Nodes
{
    public class RgbNumbersToRgbwNode : Node
    {
        public RgbNumbersToRgbwNode() : base("RGB", "Numbers to RGBW")
        {
            AddInput("R", DataType.Number);
            AddInput("G", DataType.Number);
            AddInput("B", DataType.Number);
            AddInput("W", DataType.Number);

            AddOutput("RGB", DataType.Text);

            options.ResetOutputsIfAnyInputIsNull = true;
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