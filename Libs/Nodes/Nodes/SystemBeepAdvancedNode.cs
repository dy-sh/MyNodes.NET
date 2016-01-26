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
    public class SystemBeepAdvancedNode : Node
    {
        /// <summary>
        /// Beep Advanced (3 inputs). Beep with frequency and duration.
        /// </summary>
        public SystemBeepAdvancedNode() : base(3, 0)
        {
            this.Title = "Beep Advanced";
            this.Type = "System/Beep Advanced";

            Inputs[0].Name = "Start";
            Inputs[1].Name = "Frequency";
            Inputs[2].Name = "Duration";
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            if (Inputs[0].Value == null || Inputs[1].Value == null || Inputs[2].Value == null)
            {
                LogInfo($"System/Beep Advanced: [NULL]");
            }
            else if (Inputs[0].Value == "1")
            {
                try
                {
                    int f = Int32.Parse(Inputs[1].Value);
                    int d = Int32.Parse(Inputs[2].Value);

                    Beep(f, d);
                    LogInfo($"Beep {f}Hz {d}mS");
                }
                catch
                {
                    LogError($"System/Beep Advanced: Incorrect value in input");
                }
            }
        }

        public async void Beep(int freq, int dur)
        {
            await Task.Run(() => Console.Beep(freq, dur));
        }
    }
}
