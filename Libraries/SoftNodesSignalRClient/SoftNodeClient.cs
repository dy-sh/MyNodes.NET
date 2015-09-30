/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using Microsoft.AspNet.SignalR.Client;
using MyNetSensors.Gateway;
using MyNetSensors.SoftNodes;

namespace MyNetSensors.SoftNodesSignalRClient
{
    public class SoftNodeClient:ISoftNodeClient
    {
        private IHubProxy hubProxy;
        private HubConnection hubConnection;
        private string url;

        public event OnReceivedMessageHandler OnReceivedMessageEvent;
        public event Action OnConnected;
        public event Action OnDisconnected;


        public void ConnectToServer(string url)
        {
            this.url = url;

            Console.WriteLine("Connecting to server " + url);

            hubConnection = new HubConnection(url);
            hubProxy = hubConnection.CreateHubProxy("SoftNodesHub");
            bool isConnected=false;
            while (!isConnected)
            {
                try
                {
                    hubConnection.Start().Wait();
                    hubConnection.Closed += OnHubConnectionClosed;
                    hubProxy.On<Message>("ReceiveMessage", OnReceivedMessage);
                    Console.WriteLine("Connection established");
                    isConnected = true;
                    if (OnConnected != null)
                        OnConnected();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Connection failed: "+e.Message);
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
                    Console.WriteLine("Sending message failed: "+e);
                }
            }
        }

        public void OnReceivedMessage(Message message)
        {
            //Console.WriteLine(message.ToString());
            if (OnReceivedMessageEvent != null)
                OnReceivedMessageEvent(message);
        }
    }
}
