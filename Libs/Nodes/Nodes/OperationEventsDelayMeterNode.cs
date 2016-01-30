//planer-pro copyright 2015 GPL - license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MyNetSensors.Nodes
{

    public class OperationEventsDelayMeterNode : Node
    {
        DateTime lasTime;

        public OperationEventsDelayMeterNode() : base(1, 1)
        {
            this.Title = "Events Delay Meter";
            this.Type = "Operation/Events Delay Meter";

            Outputs[0].Name = "Delay (ms)";

            Inputs[0].Type = DataType.Text;
            Outputs[0].Type = DataType.Number;

            lasTime = DateTime.Now;
        }

        public override void Loop()
        {
        }


        public override void OnInputChange(Input input)
        {
            double delay = (DateTime.Now - lasTime).TotalMilliseconds;
            lasTime = DateTime.Now;

            LogInfo($"[{delay}] ms");
            Outputs[0].Value = delay.ToString();
        }
    }
}