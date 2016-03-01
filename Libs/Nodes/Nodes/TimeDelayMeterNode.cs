/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;

namespace MyNodes.Nodes
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

        public override string GetNodeDescription()
        {
            return "This node measures the delay between the incoming events. <br/>" +
                   "Any value sent to the input (including null) will be accepted.";
        }
    }
}