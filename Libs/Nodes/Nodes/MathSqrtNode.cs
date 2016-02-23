/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;

namespace MyNetSensors.Nodes
{
    public class MathSqrtNode : Node
    {
        public MathSqrtNode() : base("Math", "Sqrt")
        {
            AddInput(DataType.Number);
            AddOutput(DataType.Number);

            options.ResetOutputsIfAnyInputIsNull = true;
        }

        public override void OnInputChange(Input input)
        {
            try
            {
                var a = double.Parse(Inputs[0].Value);
                var b = Math.Sqrt(a);

                Outputs[0].Value = b.ToString();
            }
            catch
            {
                Outputs[0].Value = null;
            }
        }

        public override string GetNodeDescription()
        {
            return "This node produces the square root of a number.";
        }
    }
}