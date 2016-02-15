/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
    public class MathClampNode : Node
    {
        public MathClampNode() : base("Math", "Clamp", 3, 1)
        {
            Inputs[0].Name = "Value";
            Inputs[1].Name = "Min";
            Inputs[2].Name = "Max";

            Inputs[0].Type = DataType.Number;
            Inputs[1].Type = DataType.Number;
            Inputs[2].Type = DataType.Number;
            Outputs[0].Type = DataType.Number;

            options.ResetOutputsWhenAnyInputIsNull = true;
        }


        public override void OnInputChange(Input input)
        {
            var value = double.Parse(Inputs[0].Value);
            var min = double.Parse(Inputs[1].Value);
            var max = double.Parse(Inputs[2].Value);

            if (min > max)
            {
                LogError($"Min must be less than Max. Min is [{min}] Max is [{max}]");
                Outputs[0].Value = null;
                return;
            }

            var result = value < min ? min : value > max ? max : value;

            Outputs[0].Value = result.ToString();
        }
    }
}