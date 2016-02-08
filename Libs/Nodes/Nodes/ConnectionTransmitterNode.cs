/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;
using System.Linq;

namespace MyNetSensors.Nodes
{
    public class ConnectionTransmitterNode : Node
    {

        public ConnectionTransmitterNode() : base(1,0)
        {
            this.Title = "Transmitter";
            this.Type = "Connection/Transmitter";

            Settings.Add("Channel", new NodeSetting(NodeSettingType.Number, "Channel", "0"));

        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            List<ConnectionReceiverNode> receivers=  engine.GetNodes()
                .OfType<ConnectionReceiverNode>()
                .Where(x => x.Settings["Channel"].Value == Settings["Channel"].Value)
                .ToList();

            foreach (var receiver in receivers)
            {
                receiver.ReceiveValue(input.Value,PanelName);
                LogInfo($"Transmit to [{receiver.PanelName}: Receiver] : [{input.Value??"NULL"}]");
            }
        }
    }
}
