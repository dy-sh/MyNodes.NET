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
            if (Inputs[0].Value != null)
            {
                try
                {
                    Random rand = new Random(DateTime.Now.Millisecond);

                    int min = Int32.Parse(Inputs[1].Value);
                    int max = Int32.Parse(Inputs[2].Value);


                    int rnd = rand.Next(min, max);

                    LogInfo($"Operation/Random: random = [{rnd}]");
                    Outputs[0].Value = rnd.ToString();
                }
                catch (Exception)
                {
                    LogInfo($"Operation/Random: input value is incorrect");
                    Outputs[0].Value = null;
                }
            }
        }
    }
}