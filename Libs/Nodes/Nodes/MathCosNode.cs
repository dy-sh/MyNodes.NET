/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.Nodes
{
    public class MathCosNode : Node
    {

        public MathCosNode() : base("Math","Cos",1, 1)
        {

            Inputs[0].Type = DataType.Number;
            Outputs[0].Type = DataType.Number;
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            if (Inputs.Any(i => i.Value == null))
            {
                ResetOutputs();
                return;
            }

            Double a = Double.Parse(Inputs[0].Value);
            Double b = Math.Cos(a);

            Outputs[0].Value = b.ToString();
        }
    }
}
