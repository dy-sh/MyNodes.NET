using System;
using Microsoft.AspNet.SignalR.Client;
using MyNetSensors.Gateway;
using MyNetSensors.SoftNodes;

namespace MyNetSensors.SoftNodesSignalRClient
{
    public class SoftNode:ISoftNode
    {
        private IHubProxy hubProxy;
        private HubConnection hubConnection;
        private string url;

        public int nodeId=100;//todo get nodeid from controller

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
                    hubProxy.On<Message>("ReceiveMessage", OnReceivedMessage);
                    Console.WriteLine("Connection established");
                    isConnected = true;
                }
                catch
                {
                    Console.WriteLine("Connection failed");
                }
            }
        }




        public bool IsConnected()
        {
            return hubConnection != null && hubConnection.State == ConnectionState.Connected;
        }

        public void Disconnect()
        {
            hubConnection.Stop();
        }


        public void SendMessage(Message message)
        {
            if (IsConnected())
            hubProxy.Invoke("ReceiveMessage", message).Wait();
        }

        public void SendSensorData(int sensorId, SensorData data)
        {
            Message message=new Message
            {
                nodeId = nodeId,
                sensorId = sensorId,
                messageType = MessageType.C_SET,
                subType = (int)data.dataType,
                payload = data.state,
                dateTime = DateTime.Now,
                incoming = true,
                ack = false,
                isValid = true
            };

            SendMessage(message);
        }

        public void OnReceivedMessage(Message message)
        {
            Console.WriteLine(message.ToString());
        }

 
    }
}
