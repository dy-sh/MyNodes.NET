/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
    public class ConnectionLocalReceiverNode : Node
    {
        public ConnectionLocalReceiverNode() : base("Connection", "Local Receiver")
        {
            AddOutput();
            Settings.Add("Channel", new NodeSetting(NodeSettingType.Number, "Channel", "0"));
        }


        public void ReceiveValue(string value, string transmitPanelName)
        {
            LogInfo($"Received from [{transmitPanelName}: Transmitter]: [{value ?? "NULL"}]");
            Outputs[0].Value = value;
        }

        public override string GetNodeDescription()
        {
            return "This node works in conjunction with Local Trasmitter, " +
                   "and provides a connection of nodes without a graphical wires. \n" +
                   "Read the description to Local Trasmitter to understand how it works.";
        }
    }
}