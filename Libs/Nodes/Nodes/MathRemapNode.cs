/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
    public class MathRemapNode : Node
    {
        public MathRemapNode() : base("Math", "Remap", 5, 1)
        {
            Inputs[0].Name = "Value";
            Inputs[1].Name = "InMin";
            Inputs[2].Name = "InMax";
            Inputs[3].Name = "OutMin";
            Inputs[4].Name = "OutMax";

            Inputs[0].Type = DataType.Number;
            Inputs[1].Type = DataType.Number;
            Inputs[2].Type = DataType.Number;
            Inputs[3].Type = DataType.Number;
            Inputs[4].Type = DataType.Number;
            Outputs[0].Type = DataType.Number;

            options.ResetOutputsWhenAnyInputIsNull = true;
        }


        public override void OnInputChange(Input input)
        {
            try
            {
                var value = double.Parse(Inputs[0].Value);

                var inMin = double.Parse(Inputs[1].Value);
                var InMax = double.Parse(Inputs[2].Value);
                var outMin = double.Parse(Inputs[3].Value);
                var outMax = double.Parse(Inputs[4].Value);

                var result = (value - inMin)/(InMax - inMin)*(outMax - outMin) + outMin;
                Outputs[0].Value = result.ToString();
            }
            catch
            {
                ResetOutputs();
            }
        }
    }
}