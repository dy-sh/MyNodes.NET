//planer-pro copyright 2015 GPL - license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.Nodes
{

    public class OperationMixerNode : Node
    {

        public OperationMixerNode() : base(2, 1)
        {
            this.Title = "Mixer";
            this.Type = "Operation/Mixer";
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
        }
    }
}