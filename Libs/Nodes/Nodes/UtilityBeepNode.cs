/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyNetSensors.Nodes
{
    public class UtilityBeepNode : Node
    {
        private readonly int DEFAULT_DURATION = 200;
        private readonly int DEFAULT_FREQUENCY = 2000;

        private int duration;
        private int frequency;

        public UtilityBeepNode() : base("Utility", "Beep")
        {
            AddInput("Start", DataType.Logical);
            AddInput("Frequency", DataType.Number, true);
            AddInput("Duration", DataType.Number, true);

            duration = DEFAULT_DURATION;
            frequency = DEFAULT_FREQUENCY;
        }

        public override void OnInputChange(Input input)
        {
            if (input == Inputs[1])
            {
                if (input.Value == null)
                    frequency = DEFAULT_FREQUENCY;
                else
                {
                    double f;
                    double.TryParse(input.Value, out f);
                    frequency = (int)(f < 37 ? 37 : f > 32767 ? 32767 : f);
                }
            }

            if (input == Inputs[2])
            {
                if (input.Value == null)
                    duration = DEFAULT_DURATION;
                else
                {
                    double d;
                    double.TryParse(input.Value, out d);
                    duration = (int)(d < 1 ? 1 : d > 10000 ? 10000 : d);
                }
            }

            if (input == Inputs[0] && input.Value == "1")
            {
                try
                {
                    Beep(frequency, duration);
                    LogInfo($"Beep");
                }
                catch
                {
                    LogError($"Incorrect value in input");
                }
            }
        }

        public async void Beep(int freq, int dur)
        {
            await Task.Run(() => Console.Beep(freq, dur));
        }

        public override string GetNodeDescription()
        {
            return "This node plays a sound on the server (not in the browser) " +
                   "of specified frequency and duration. " +
                   "If you do not specify the frequency and duration, " +
                   "will be used default: 2000Hz, 200ms.";
        }
    }
}