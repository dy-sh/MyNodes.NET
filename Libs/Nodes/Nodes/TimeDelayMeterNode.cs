//planer-pro copyright 2015 GPL - license.

using System;

namespace MyNetSensors.Nodes
{
    public class TimeDelayMeterNode : Node
    {
        private DateTime lasTime;

        public TimeDelayMeterNode() : base("Time", "Delay Meter")
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