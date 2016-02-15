/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
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
    }
}