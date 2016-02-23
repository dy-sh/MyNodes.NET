/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
    public class MathRemapNode : Node
    {
        public MathRemapNode() : base("Math", "Remap")
        {
            AddInput("Value", DataType.Number);
            AddInput("InMin", DataType.Number);
            AddInput("InMax", DataType.Number);
            AddInput("OutMin", DataType.Number);
            AddInput("OutMax", DataType.Number);
            AddOutput("Out", DataType.Number);

            options.ResetOutputsIfAnyInputIsNull = true;
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