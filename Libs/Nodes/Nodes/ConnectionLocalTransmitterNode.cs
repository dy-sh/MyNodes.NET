/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Linq;

namespace MyNetSensors.Nodes
{
    public class ConnectionLocalTransmitterNode : Node
    {
        public ConnectionLocalTransmitterNode() : base("Connection", "Local Transmitter", 1, 0)
        {
            Settings.Add("Channel", new NodeSetting(NodeSettingType.Number, "Channel", "0"));
        }


        public override void OnInputChange(Input input)
        {
            var receivers = engine.GetNodes()
                .OfType<ConnectionLocalReceiverNode>()
                .Where(x => x.Settings["Channel"].Value == Settings["Channel"].Value)
                .ToList();

            foreach (var receiver in receivers)
            {
                receiver.ReceiveValue(input.Value, PanelName);
                LogInfo($"Transmit to [{receiver.PanelName}: Receiver] : [{input.Value ?? "NULL"}]");
            }
        }
    }
}