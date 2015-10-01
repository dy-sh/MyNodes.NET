/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using Microsoft.AspNet.SignalR.Client;
using MyNetSensors.Gateway;
using MyNetSensors.SoftNodes;
using DebugMessageEventHandler = MyNetSensors.SoftNodes.DebugMessageEventHandler;

namespace MyNetSensors.SoftNodesSignalRClient
{
    public class SoftNodeClient:ISoftNodeClient
    {
        private IHubProxy hubProxy;
        private HubConnection hubConnection;
        private string url;

        public event OnReceivedMessageHandler OnReceivedMessage;
        public event Action OnConnected;
        public event Action OnDisconnected;
        public event DebugMessageEventHandler OnConnectionFailed;
        public event DebugMessageEventHandler OnSendingMessageFailed;


        public void ConnectToServer(string url)
        {
            this.url = url;

            hubConnection = new HubConnection(url);
            hubProxy = hubConnection.CreateHubProxy("SoftNodesHub");
            bool isConnected=false;
            while (!isConnected)
            {
                try
                {
                    hubConnection.Start().Wait();
                    hubConnection.Closed += OnHubConnectionClosed;
                    hubProxy.On<Message>("ReceiveMessage", ReceiveMessage);

                    isConnected = true;
                    if (OnConnected != null)
                        OnConnected();
                }
                catch (Exception e)
                {
                    if (OnConnectionFailed != null)
                        OnConnectionFailed(e.Message);
                }
            }
        }

        private void OnHubConnectionClosed()
        {
            if (OnDisconnected != null)
                OnDisconnected();
        }


        public bool IsConnected()
        {
            return hubConnection != null && hubConnection.State == ConnectionState.Connected;
        }

        public void Disconnect()
        {
            hubConnection.Stop();
            if (OnDisconnected != null)
                OnDisconnected();
        }


        public void SendMessage(Message message)
        {
            if (IsConnected())
            {
                try
                {
                    hubProxy.Invoke("ReceiveMessage", message);
                }
                catch (Exception e)
                {
                    if (OnConnectionFailed != null)
                        OnConnectionFailed(e.Message);
                }
            }
        }

        public void ReceiveMessage(Message message)
        {
            message.incoming = true;

            //Console.WriteLine(message.ToString());
            if (OnReceivedMessage != null)
                OnReceivedMessage(message);
        }
    }
}
