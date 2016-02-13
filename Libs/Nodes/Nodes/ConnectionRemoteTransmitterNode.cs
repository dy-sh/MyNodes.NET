/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;


namespace MyNetSensors.Nodes
{
    public class ConnectionRemoteTransmitterNode : Node
    {
        private string address;
        private string password;
        private int channel;

        public ConnectionRemoteTransmitterNode() : base("Connection", "Remote Transmitter",4, 0)
        {
            Inputs[0].Name = "Value";
            Inputs[1].Name = "Address";
            Inputs[2].Name = "Channel";
            Inputs[3].Name = "Password";

            Inputs[0].Type = DataType.Text;
            Inputs[1].Type = DataType.Text;
            Inputs[2].Type = DataType.Number;
            Inputs[3].Type = DataType.Text;
        }

        public override void Loop()
        {
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
                    for (int i = 0; i < password.Length; i++)
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
                    string url = address + "/NodeEditorApi/ReceiverSetValue/";

                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("value", value),
                        new KeyValuePair<string, string>("channel", channel.ToString()),
                        new KeyValuePair<string, string>("password", password)
                    });

                    LogInfo($"Send to [{address}] channel [{channel}]: [{value ?? "NULL"}]");

                    var result = await client.PostAsync(url, content);
                    string resultContent = result.Content.ReadAsStringAsync().Result;

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
    }
}
