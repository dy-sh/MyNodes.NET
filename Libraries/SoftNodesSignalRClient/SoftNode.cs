using System;
using Microsoft.AspNet.SignalR.Client;
using MyNetSensors.Gateway;
using MyNetSensors.SoftNodes;

namespace MyNetSensors.SoftNodesSignalRClient
{
    public class SoftNode:ISoftNode
    {
        IHubProxy hub;
        string url;
        public int nodeId=100;//todo get nodeid from controller

        public SoftNode(string url = "http://localhost:8080/")
        {
            this.url = url;

            var connection = new HubConnection(url);
            hub = connection.CreateHubProxy("SoftNodesHub");
            connection.Start().Wait();

            hub.On<Message>("SendMessage", OnReceivedMessage);
        }

        public void SendMessage(Message message)
        {
            hub.Invoke("SendMessage", message).Wait();
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
