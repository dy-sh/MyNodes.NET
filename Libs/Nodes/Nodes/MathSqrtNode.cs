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
    public class MathSqrtNode : Node
    {

        public MathSqrtNode() : base("Math","Sqrt",1, 1)
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

            try
            {
                Double a = Double.Parse(Inputs[0].Value);
                Double b = Math.Sqrt(a);

                Outputs[0].Value = b.ToString();
            }
            catch
            {
                Outputs[0].Value = null;
            }
        }
    }
}
