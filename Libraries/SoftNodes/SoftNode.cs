using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyNetSensors.Gateway;

namespace MyNetSensors.SoftNodes
{
    public class SoftNode
    {
        private ISoftNodeClient client;

        public int nodeId = 100;//todo get nodeid from controller


        public SoftNode(ISoftNodeClient client)
        {
            this.client = client;
            client.OnReceivedMessageEvent += OnReceivedSoftNodeMessage;
        }


        private void OnReceivedSoftNodeMessage(Message message)
        {
            if (message.nodeId== nodeId)
                 Console.WriteLine(message.ToString());
        }

        public void SendSensorData(int sensorId, SensorData data)
        {
            Message message = new Message
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

            client.SendMessage(message);
        }

        public void ConnectToServer(string url= "http://localhost:13122/")
        {
            client.ConnectToServer(url);
        }

        public void Disconnect()
        {
            client.Disconnect();
        }

        public bool IsConnected()
        {
            return client.IsConnected();
        }
    }
}
