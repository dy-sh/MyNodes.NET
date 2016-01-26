//planer-pro copyright 2015 GPL - license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.Nodes
{
    public class OperationRandomNode : Node
    {
        /// <summary>
        /// Random (3 inputs, 1 output).
        /// </summary>
        public OperationRandomNode() : base(3, 1)
        {
            this.Title = "Random";
            this.Type = "Operation/Random";

            Inputs[0].Name = "Start";
            Inputs[1].Name = "Min Value";
            Inputs[2].Name = "Max Value";
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            try
            {
                if (Inputs[0].Value == null || Inputs[1].Value == null || Inputs[2].Value == null)
                {
                    LogInfo($"Operation/Random: [NULL]");
                    Outputs[0].Value = null;
                }
                else
                {
                    if (Inputs[0].Value == "1")
                    {
                        Random rand = new Random(DateTime.Now.Millisecond);

                        int min = Int32.Parse(Inputs[1].Value);
                        int max = Int32.Parse(Inputs[2].Value);

                        int rnd = rand.Next(min, max);

                        LogInfo($"Operation/Random: random = [{rnd}]");
                        Outputs[0].Value = rnd.ToString();
                    }
                }
            }
            catch
            {
                LogError($"Operation/Random: Incorrect value in input");
                Outputs[0].Value = null;
            }
        }
    }
}