/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Net.Http;

namespace MyNodes.SoftNodes
{
    public delegate void LogMessageEventHandler(string message);

    public class SoftNodeTransmitter
    {
        public string serverAddress = "http://localhost:1312";
        public int receiverChannel = 0;
        public string receiverPassword;

        public event LogMessageEventHandler OnLogInfo;
        public event LogMessageEventHandler OnLogError;

        public SoftNodeTransmitter(string serverAddress, int receiverChannel, string receiverPassword)
        {
            this.serverAddress = serverAddress;
            this.receiverChannel = receiverChannel;
            this.receiverPassword = receiverPassword;
        }

        public async void Send(string value)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string url = serverAddress + "/NodeEditorApi/ReceiverSetValue/";

                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("value", value),
                        new KeyValuePair<string, string>("channel", receiverChannel.ToString()),
                        new KeyValuePair<string, string>("password", receiverPassword)
                    });

                    LogInfo($"Send to [{serverAddress}] channel [{receiverChannel}]: [{value ?? "NULL"}]");

                    var result = await client.PostAsync(url, content);
                    string resultContent = result.Content.ReadAsStringAsync().Result;

                    if (resultContent == "0")
                    {
                        LogInfo($"[{serverAddress}] channel [{receiverChannel}] receiver: Received.");
                    }
                    else if (resultContent == "1")
                    {
                        LogError($"[{serverAddress}] channel [{receiverChannel}] receiver: Password is wrong.");
                    }
                    else if (resultContent == "2")
                    {
                        LogError($"[{serverAddress}]: No receivers with channel [{receiverChannel}].");
                    }

                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
            }
        }

        private void LogInfo(string message)
        {
            OnLogInfo?.Invoke(message);
        }

        private void LogError(string message)
        {
            OnLogError?.Invoke(message);
        }
    }
}
