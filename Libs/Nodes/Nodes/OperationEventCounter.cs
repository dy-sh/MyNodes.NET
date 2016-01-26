//planer-pro copyright 2015 GPL - license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.Nodes
{

    public class OperationEventCounter : Node
    {
        /// <summary>
        /// Event Counter (1 inputs, 1 output).
        /// </summary>
        public OperationEventCounter() : base(2, 1)
        {
            this.Title = "Event Counter";
            this.Type = "Operation/Event Counter";

            Inputs[0].Name = "input";
            Inputs[1].Name = "clear";
        }

        public override void Loop()
        {
        }

        private int count = 0;
        private string oldValue = "";
        public override void OnInputChange(Input input)
        {
            if (Inputs[0].Value == null)
            {
                LogInfo($"Operation/Event Counter: [NULL]");
                Outputs[0].Value = null;
            }
            else
            {
                if (Inputs[1].Value == "1")
                {
                    count = 0;
                    LogInfo($"Operation/Event Counter: cleared");
                }
                else
                {
                    if (oldValue != Inputs[0].Value)
                    {
                        count++;
                        oldValue = Inputs[0].Value;
                    }
                    LogInfo($"Operation/Event Counter: {count}");
                    Outputs[0].Value = count.ToString();
                }
            }
        }
    }
}