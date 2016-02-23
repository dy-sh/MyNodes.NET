/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNet.SignalR.Client;

namespace MyNetSensors.SoftNodes
{
    public class SoftNodeSignalRTransmitter
    {
        public string serverAddress = "http://localhost:1312";
        public int receiverChannel = 0;
        public string receiverPassword;

        public event LogMessageEventHandler OnLogInfo;
        public event LogMessageEventHandler OnLogError;
        public event Action OnConnected;
        public event Action OnDisconnected;
        public event LogMessageEventHandler OnConnectionFailed;

        public SoftNodeSignalRTransmitter(string serverAddress, int receiverChannel, string receiverPassword)
        {
            this.serverAddress = serverAddress;
            this.receiverChannel = receiverChannel;
            this.receiverPassword = receiverPassword;

            ConnectToServer();
        }


        private void LogInfo(string message)
        {
            OnLogInfo?.Invoke(message);
        }

        private void LogError(string message)
        {
            OnLogError?.Invoke(message);
        }




        private IHubProxy hubProxy;
        private HubConnection hubConnection;

        public void ConnectToServer()
        {

            hubConnection = new HubConnection(serverAddress);
            hubProxy = hubConnection.CreateHubProxy("SoftNodesHub");
            bool isConnected = false;
            while (!isConnected)
            {
                try
                {
                    hubConnection.Start().Wait();
                    hubConnection.Closed += OnHubConnectionClosed;
                    //hubProxy.On<Message>("ReceiveMessage", ReceiveMessage);

                    isConnected = true;
                    LogInfo("Connected to server");
                    OnConnected?.Invoke();
                }
                catch (Exception e)
                {
                    LogError("Connection to server failed: " + e.Message);
                    OnConnectionFailed?.Invoke(e.Message);
                }
            }
        }

        private void OnHubConnectionClosed()
        {
            LogInfo("Disconnected from server");
            OnDisconnected?.Invoke();
        }


        public bool IsConnected()
        {
            return hubConnection != null && hubConnection.State == ConnectionState.Connected;
        }

        public void Disconnect()
        {
            hubConnection.Stop();
            LogInfo("Disconnected from server");
            OnDisconnected?.Invoke();
        }


        public void Send(string value)
        {
            if (!IsConnected())
                return;

            try
            {
                int result = hubProxy.Invoke<int>("SetValue", value,receiverChannel,receiverPassword).Result;

                LogInfo($"Send to [{serverAddress}] channel [{receiverChannel}]: [{value ?? "NULL"}]");

                if (result == 0)
                {
                    LogInfo($"[{serverAddress}] channel [{receiverChannel}] receiver: Received.");
                }
                else if (result == 1)
                {
                    LogError($"[{serverAddress}] channel [{receiverChannel}] receiver: Password is wrong.");
                }
                else if (result == 2)
                {
                    LogError($"[{serverAddress}]: No receivers with channel [{receiverChannel}].");
                }
            }
            catch (Exception e)
            {
                OnConnectionFailed?.Invoke(e.Message);
            }

        }


        //public async void Send(string value)
        //{
        //    try
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            string url = serverAddress + "/NodeEditorApi/ReceiverSetValue/";

        //            var content = new FormUrlEncodedContent(new[]
        //            {
        //                new KeyValuePair<string, string>("value", value),
        //                new KeyValuePair<string, string>("channel", receiverChannel.ToString()),
        //                new KeyValuePair<string, string>("password", receiverPassword)
        //            });

        //            LogInfo($"Send to [{serverAddress}] channel [{receiverChannel}]: [{value ?? "NULL"}]");

        //            var result = await client.PostAsync(url, content);
        //            string resultContent = result.Content.ReadAsStringAsync().Result;

        //            if (resultContent == "0")
        //            {
        //                LogInfo($"[{serverAddress}] channel [{receiverChannel}] receiver: Received.");
        //            }
        //            else if (resultContent == "1")
        //            {
        //                LogError($"[{serverAddress}] channel [{receiverChannel}] receiver: Password is wrong.");
        //            }
        //            else if (resultContent == "2")
        //            {
        //                LogError($"[{serverAddress}]: No receivers with channel [{receiverChannel}].");
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogError(ex.Message);
        //    }
        //}

        private void Receive(string value)
        {

        }
    }
}
