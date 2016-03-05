/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace MyNodes.Nodes
{
    public class RgbCrossfadeRgbNode : Node
    {
        public RgbCrossfadeRgbNode() : base("RGB", "Crossfade RGB")
        {
            AddInput("Crossfade", DataType.Number);
            AddInput("A", DataType.Text);
            AddInput("B", DataType.Text);
            AddOutput("RGB", DataType.Text);

            options.ResetOutputsIfAnyInputIsNull = true;
        }

     

        public override void OnInputChange(Input input)
        {
            try
            {
                double value = double.Parse(Inputs[0].Value);
                var outMinRGB = Inputs[1].Value;
                var outMaxRGB = Inputs[2].Value;
                
                if (outMinRGB[0] == '#')
                    outMinRGB = outMinRGB.Remove(0, 1);

                if (outMaxRGB[0] == '#')
                    outMaxRGB = outMaxRGB.Remove(0, 1);

                var resultRGB = "";

                for (var i = 0; i < 3; i++)
                {
                    var outMin = int.Parse(outMinRGB.Substring(i * 2, 2), NumberStyles.HexNumber);
                    var outMax = int.Parse(outMaxRGB.Substring(i * 2, 2), NumberStyles.HexNumber);

                    var result = (int)Remap(value, 0, 100, outMin, outMax);
                    result = Clamp(result, 0, 255);

                    resultRGB += result.ToString("X2");
                }

                Outputs[0].Value = resultRGB;
            }
            catch
            {
                LogError($"Incorrect value in input.");
                ResetOutputs();
            }
        }

        

        private double Remap(double value, double inMin, double inMax, double outMin, double outMax)
        {
            return (value - inMin) / (inMax - inMin) * (outMax - outMin) + outMin;
        }

        private int Clamp(int value, int min, int max)
        {
            return value < min ? min : value > max ? max : value;
        }


        public override string GetNodeDescription()
        {
            return "This node makes the crossfade between two RGB colors. <br/>" +
                   "\"Crossfade\" input takes a value from 0 to 100. <br/>" +
                   "If Crossfade is 0, the output will be equal to A. <br/>" +
                   "If Crossfade is 100, then the output is equal to B. <br/>" +
                   "The intermediate value between 0 and 100 will give " +
                   "intermediate number between A and B. ";
        }
    }
}