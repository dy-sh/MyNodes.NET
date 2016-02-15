//planer-pro copyright 2015 GPL - license.

using System;

namespace MyNetSensors.Nodes
{
    public class OperationEventsDelayMeterNode : Node
    {
        private DateTime lasTime;

        public OperationEventsDelayMeterNode() : base("Operation", "Events Delay Meter")
        {
            AddInput();
            AddOutput("Delay (ms)",DataType.Number);

            lasTime = DateTime.Now;
        }


        public override void OnInputChange(Input input)
        {
            var delay = (DateTime.Now - lasTime).TotalMilliseconds;
            lasTime = DateTime.Now;

            Outputs[0].Value = delay.ToString();
        }
    }
}