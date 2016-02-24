/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Net.Http;

namespace MyNetSensors.Nodes
{
    public class ConnectionRemoteTransmitterNode : Node
    {
        private string address;
        private int channel;
        private string password;

        public ConnectionRemoteTransmitterNode() : base("Connection", "Remote Transmitter")
        {
            AddInput("Value", DataType.Number);
            AddInput("Address", DataType.Text);
            AddInput("Channel", DataType.Number, true);
            AddInput("Password", DataType.Text, true);
        }


        public override void OnInputChange(Input input)
        {
            if (input == Inputs[1])
            {
                address = input.Value;
                LogInfo($"Address changed: [{address ?? "NULL"}]");
            }

            if (input == Inputs[3])
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

            if (input == Inputs[2])
            {
                channel = input.Value == null ? 0 : int.Parse(input.Value);
                LogInfo($"Channel changed: {channel}");
            }

            if (input == Inputs[0] && address != null)
            {
                SendValue(input.Value);
            }
        }

        private async void SendValue(string value)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var url = address + "/NodeEditorApi/ReceiverSetValue/";

                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("value", value),
                        new KeyValuePair<string, string>("channel", channel.ToString()),
                        new KeyValuePair<string, string>("password", password)
                    });

                    LogInfo($"Send to [{address}] channel [{channel}]: [{value ?? "NULL"}]");

                    var result = await client.PostAsync(url, content);
                    var resultContent = result.Content.ReadAsStringAsync().Result;

                    if (resultContent == "0")
                    {
                        LogInfo($"[{address}] channel [{channel}] receiver: Received.");
                    }
                    else if (resultContent == "1")
                    {
                        LogError($"[{address}] channel [{channel}] receiver: Password is wrong.");
                    }
                    else if (resultContent == "2")
                    {
                        LogError($"[{address}]: No receivers with channel [{channel}].");
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
            }
        }

        public override string GetNodeDescription()
        {
            return "This node works in conjunction with the Remote Receiver, " +
                   "and provides a remote connection of nodes. <br/>" +
                   "The principle of operation of this node is the same as the local version, " +
                   "but this node can be used to link the nodes are located on different " +
                   "servers in a local network or in the Internet. <br/>" +
                   "With this node you can merge several systems MyNetSensors into one system. <br/>" +
                   "To link the transmitter and the receiver, " +
                   "you need to set the channel (like on the local version), " +
                   "address (and port) of the server and password. <br/>" +
                   "The server address (and port) - exactly the same, " +
                   "which it access in the browser (\"http://192.168.1.2:1312\" for example). <br/>" +
                   "The passwords in the transmitter and receiver must match. <br/>" +
                   "If you do not specify a channel, it will use channel 0 by default. <br/>" +
                   "If you do not specify a password, the password will not be used.";
        }
    }
}