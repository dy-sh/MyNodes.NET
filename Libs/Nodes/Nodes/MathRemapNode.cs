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
    public class MathRemapNode : Node
    {

        public MathRemapNode() : base(5, 1)
        {
            this.Title = "Remap";
            this.Type = "Math/Remap";

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

                Double value = Double.Parse(Inputs[0].Value);

                Double inMin = Double.Parse(Inputs[1].Value);
                Double InMax = Double.Parse(Inputs[2].Value);
                Double outMin = Double.Parse(Inputs[3].Value);
                Double outMax = Double.Parse(Inputs[4].Value);

                Double result = (value - inMin) / (InMax - inMin) * (outMax - outMin) + outMin;
                Outputs[0].Value = result.ToString();
            }
            catch
            {
                ResetOutputs();
            }
        }
    }
}
