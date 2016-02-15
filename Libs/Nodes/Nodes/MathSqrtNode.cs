/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;

namespace MyNetSensors.Nodes
{
    public class MathSqrtNode : Node
    {
        public MathSqrtNode() : base("Math", "Sqrt", 1, 1)
        {
            Inputs[0].Type = DataType.Number;
            Outputs[0].Type = DataType.Number;

            options.ResetOutputsWhenAnyInputIsNull = true;
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
    }
}