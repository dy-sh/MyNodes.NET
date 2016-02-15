//planer-pro copyright 2015 GPL - license.

namespace MyNetSensors.Nodes
{
    public class OperationMixerNode : Node
    {
        public OperationMixerNode() : base("Operation", "Mixer")
        {
            AddInput();
            AddInput();
            AddOutput();
        }


        public override void OnInputChange(Input input)
        {
            var mixOut = "";

            if (Inputs[0].Value == null && Inputs[1].Value == null) mixOut = null;
            if (Inputs[0].Value != null) mixOut = mixOut + Inputs[0].Value;
            if (Inputs[1].Value != null) mixOut = mixOut + Inputs[1].Value;

            Outputs[0].Value = mixOut;
        }
    }
}