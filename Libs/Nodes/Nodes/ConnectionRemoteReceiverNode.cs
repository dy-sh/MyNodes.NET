/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
    public class ConnectionRemoteReceiverNode : Node
    {
        private string password;
        private int channel;


        public ConnectionRemoteReceiverNode() : base("Connection", "Remote Receiver",2, 1)
        {
            Inputs[0].Name = "Channel";
            Inputs[1].Name = "Password";

            Inputs[0].Type = DataType.Number;
            Inputs[1].Type = DataType.Text;
            Outputs[0].Type = DataType.Text;
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            if (input == Inputs[0])
            {
                channel = input.Value == null ? 0 : int.Parse(input.Value);
                LogInfo($"Channel changed: {channel}");
            }

            if (input == Inputs[1])
            {
                password = input.Value;
                string pass = null;

                if (password != null)
                    for (int i = 0; i < password.Length; i++)
                    {
                        pass += "*";
                    }

                LogInfo($"Password changed: {pass ?? "NULL"}");
            }
        }

        public bool ReceiveValue(string value, string channel, string password, string senderIp)
        {
            if (channel != this.channel.ToString())
                return false;

            if (password != this.password)
            {
                LogError($"Received value from [{senderIp}], but password is wrong");
                return false;
            }

            LogInfo($"Received from [{senderIp}]: [{value ?? "NULL"}]");
            Outputs[0].Value = value;

            return true;
        }

        public int GetChannel()
        {
            return channel;
        }
    }
}
