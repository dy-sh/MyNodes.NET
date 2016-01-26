//planer-pro copyright 2015 GPL - license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.Nodes
{

    public class OperationMixer : Node
    {
        /// <summary>
        /// OperationMixer (2 inputs, 1 output).
        /// </summary>
        public OperationMixer() : base(2, 1)
        {
            this.Title = "Mixer";
            this.Type = "Operation/Mixer";

            Inputs[0].Name = "in 1";
            Inputs[1].Name = "in 2";
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            string mixOut = "";

            if (Inputs[0].Value == null && Inputs[1].Value == null) mixOut = null;
            if (Inputs[0].Value != null) mixOut = mixOut + Inputs[0].Value;
            if (Inputs[1].Value != null) mixOut = mixOut + Inputs[1].Value;

            Outputs[0].Value = mixOut;
            LogInfo($"Operation/Mixer: {mixOut}");
        }
    }
}