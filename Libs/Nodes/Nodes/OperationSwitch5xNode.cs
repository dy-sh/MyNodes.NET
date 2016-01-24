/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.Nodes
{
    public class OperationSwitch5xNode : Node
    {
        private static int channels = 5;

        /// <summary>
        /// Switch5x (6 input, 1 output).
        /// </summary>
        public OperationSwitch5xNode() : base(channels + 1, 1)
        {
            this.Title = $"Switch{channels}x";
            this.Type = $"Operation/Switch{channels}x";

            Inputs[0].Name = "Switch value";
            for (int i = 1; i <= channels; i++)
            {
                Inputs[i].Name = $"Value{i}";
            }
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            for (int i = 0; i < channels; i++)
            {
                if (Inputs[channels].Value == $"{i}")
                {
                    LogInfo($"Operation/Switch{channels}x: switched to value{i}");
                    Outputs[0].Value = Inputs[i].Value;

                    return;
                }

                LogInfo($"Operation/Switch{channels}x: input value is incorrect");
                Outputs[0].Value = null;

            }
        }
    }
}
