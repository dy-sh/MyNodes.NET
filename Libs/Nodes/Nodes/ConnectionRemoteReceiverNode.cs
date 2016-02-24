/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
    public class ConnectionRemoteReceiverNode : Node
    {
        private int channel;
        private string password;


        public ConnectionRemoteReceiverNode() : base("Connection", "Remote Receiver")
        {
            AddInput("Channel", DataType.Number, true);
            AddInput("Password", DataType.Text, true);
            AddOutput("Out");
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
                    for (var i = 0; i < password.Length; i++)
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

        public override string GetNodeDescription()
        {
            return "This node works in conjunction with Remote Trasmitter, " +
                    "and provides a remote connection of nodes. \n" +
                    "Read the description to Remote Trasmitter to understand how it works.";
        }
    }
}