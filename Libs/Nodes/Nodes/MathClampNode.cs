/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.Nodes
{
    public class MathClampNode : Node
    {
        /// <summary>
        /// MathClampNode (3 inputs, 1 output).
        /// </summary>
        public MathClampNode() : base(3, 1)
        {
            this.Title = "Clamp";
            this.Type = "Math/Clamp";

            Inputs[0].Name = "Value";
            Inputs[1].Name = "Min";
            Inputs[2].Name = "Max";

            Inputs[0].Type = DataType.Number;
            Inputs[1].Type = DataType.Number;
            Inputs[2].Type = DataType.Number;
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

            Double value = Double.Parse(Inputs[0].Value);
            Double min = Double.Parse(Inputs[1].Value);
            Double max = Double.Parse(Inputs[2].Value);

            if (min > max)
            {
                LogError($"Min must be less than Max. Min is [{min}] Max is [{max}]");
                Outputs[0].Value = null;
                return;
            }

            Double result = (value < min) ? min : (value > max) ? max : value;

            LogInfo($"[{value}] clamped to [{result}]");
            Outputs[0].Value = result.ToString();
        }
    }
}
