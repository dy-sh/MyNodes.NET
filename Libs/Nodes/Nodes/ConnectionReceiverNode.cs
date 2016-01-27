/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
    public class ConnectionReceiverNode : Node
    {
        public int Channel { get; set; }

        public ConnectionReceiverNode() : base(0,1)
        {
            this.Title = "Receiver";
            this.Type = "Connection/Receiver";
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
        }

        public void SetChannel(int channel)
        {
            Channel = channel;
            LogInfo($"Channel changed to [{Channel}]");
            UpdateMeInDb();
        }

        public void ReceiveValue(string value, string transmitPanelName)
        {
            LogInfo($"Received from [{transmitPanelName}: Transmitter]: [{value??"NULL"}]");
            Outputs[0].Value = value;
        }
    }
}
