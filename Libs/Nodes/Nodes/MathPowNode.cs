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
    public class MathPowNode : Node
    {
        /// <summary>
        /// Math Pow (2 inputs, 1 output).
        /// </summary>
        public MathPowNode() : base(2, 1)
        {
            this.Title = "Math Pow";
            this.Type = "Math/Pow";

            Inputs[0].Type = DataType.Number;
            Inputs[1].Type = DataType.Number;
            Outputs[0].Type = DataType.Number;
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            if (Inputs.Any(i => i.Value == null))
            {
                LogInfo("[NULL]");
                Outputs[0].Value = null;
                return;
            }

            Double a = Double.Parse(Inputs[0].Value);
            Double b = Double.Parse(Inputs[1].Value);
            Double c = Math.Pow(a, b);

            LogInfo($"Pow [{a}] to [{b}]");
            Outputs[0].Value = c.ToString();
        }
    }
}
