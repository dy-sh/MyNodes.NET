/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;

namespace MyNetSensors.Nodes
{
    public class MathSinNode : Node
    {
        public MathSinNode() : base("Math", "Sin", 1, 1)
        {
            Inputs[0].Type = DataType.Number;
            Outputs[0].Type = DataType.Number;

            options.ResetOutputsWhenAnyInputIsNull = true;
        }


        public override void OnInputChange(Input input)
        {
            var a = double.Parse(Inputs[0].Value);
            var b = Math.Sin(a);

            Outputs[0].Value = b.ToString();
        }
    }
}