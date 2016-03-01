/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;

namespace MyNodes.Nodes
{
    public class FiltersReduceEventsNode : Node
    {
        private DateTime lastTime;
        private double interval;
        private string lastValue;
        private bool waitingToSend;

        public FiltersReduceEventsNode() : base("Filters", "Reduce Events")
        {
            AddInput("Value");
            AddInput("Interval", DataType.Number);
            AddOutput();

            lastTime = DateTime.Now;

            Settings.Add("sendlast",new NodeSetting(NodeSettingType.Checkbox, "Store and send last value","true"));
        }

        public override void Loop()
        {
            if (!waitingToSend || IsBlocked())
                return;

            lastTime = DateTime.Now;
            Outputs[0].Value = lastValue;
            waitingToSend = false;
        }

        public override void OnInputChange(Input input)
        {
            if (input == Inputs[1] && input.Value != null)
            {
                interval = double.Parse(input.Value);
                return;
            }

            if (input == Inputs[0])
            {
                if (!IsBlocked())
                {
                    lastTime = DateTime.Now;
                    Outputs[0].Value = input.Value;
                }
                else if (Settings["sendlast"].Value == "true")
                {
                    lastValue = input.Value;
                    waitingToSend = true;
                }
            }
        }

        public bool IsBlocked()
        {
            return (DateTime.Now - lastTime).TotalMilliseconds < interval;
        }

        public override string GetNodeDescription()
        {
            return "This node reduces the number of transmitted values. <br/><br/>" +
                   "When the value on the input is changed, " +
                   "the node sends it to the output and stops " +
                   "receiving at a specified time interval. <br/>" +
                   "The values that come at this time are simply ignored, " +
                   "but the last value will be stored. <br/>" +
                   "When the interval passes, the last value is sent to the output. " +
                   "This reduces the number of events, if they are sent too often, but" +
                   "ensures that the node will always send the last actual value. <br/><br/>" +
                   "You can disable the sending of the last value in the settings of the node. " +
                   "This may be necessary if you do not want to receive messages late, " +
                   "but then it is not guaranteed that the nodes connected to this node " +
                   "will have the last actual value.";
        }
    }
}