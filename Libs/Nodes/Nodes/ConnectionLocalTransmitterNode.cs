/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;
using System.Linq;

namespace MyNodes.Nodes
{
    public class ConnectionLocalTransmitterNode : Node
    {
        public ConnectionLocalTransmitterNode() : base("Connection", "Local Transmitter")
        {
            AddInput();
            Settings.Add("Channel", new NodeSetting(NodeSettingType.Number, "Channel", "0"));
        }


        public override void OnInputChange(Input input)
        {
            List<ConnectionLocalReceiverNode> receivers;

            lock (engine.nodesLock)
                receivers = engine.GetNodes()
                    .OfType<ConnectionLocalReceiverNode>()
                    .Where(x => x.Settings["Channel"].Value == Settings["Channel"].Value)
                    .ToList();

            foreach (var receiver in receivers)
            {
                receiver.ReceiveValue(input.Value, PanelName);
                LogInfo($"Transmit to [{receiver.PanelName}: Receiver] : [{input.Value ?? "NULL"}]");
            }
        }

        public override string GetNodeDescription()
        {
            return "This node works in conjunction with Local Receiver, " +
                   "and provides a connection of nodes without a graphical wires. <br/>" +
                   "You can use this nodes to connect nodes that are far away " +
                   "from each other (for example on different panels) and you don't " +
                   "want to drag the \"wire\" so far. <br/>" +
                   "Set the same channel on transmitter and receiver to link them. " +
                   "If you do not specify a channel, it will use channel 0 by default. <br/>" +
                   "You can also use this node for broadcast. <br/>" +
                   "For example, you want a lot nodes on different panels " +
                   "heard the message from one node. " +
                   "Use a lot of receivers configured on the same channel. <br/>" +
                   "Or, you want to one node received messages " +
                   "on input from different nodes. " +
                   "Use many transmitters on the same channel. ";
        }
    }
}