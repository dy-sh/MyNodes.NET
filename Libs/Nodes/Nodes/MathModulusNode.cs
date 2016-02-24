/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
    public class MathModulusNode : Node
    {
        public MathModulusNode() : base("Math", "Modulus")
        {
            AddInput(DataType.Number);
            AddInput(DataType.Number);
            AddOutput(DataType.Number);

            options.ResetOutputsIfAnyInputIsNull = true;
        }

        public override void OnInputChange(Input input)
        {
            var a = double.Parse(Inputs[0].Value);
            var b = double.Parse(Inputs[1].Value);
            var c = a%b;

            Outputs[0].Value = c.ToString();
        }

        public override string GetNodeDescription()
        {
            return "This node performs modulo operation " +
                   "(finds the remainder after division of one number by another).";
        }
    }
}