/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

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
        private Node node;

        public SoftNode(ISoftNodeClient client,string nodeName=null,string nodeVersion=null)
        {
            this.client = client;
            node = new Node();
            node.name = nodeName;
            node.version = nodeVersion;

            client.OnReceivedMessageEvent += OnReceivedSoftNodeMessage;
            client.OnConnected += OnConnected;

            if (client.IsConnected())
                OnConnected();
        }


        private void OnReceivedSoftNodeMessage(Message message)
        {
            if (message.nodeId== node.nodeId)
                 Console.WriteLine(message.ToString());
        }

        public void SendSensorData(int sensorId, SensorData data)
        {
            Message message = new Message
            {
                nodeId = node.nodeId,
                sensorId = sensorId,
                messageType = MessageType.C_SET,
                ack = false,
                subType = (int)data.dataType,
                payload = data.state
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

        private void OnConnected()
        {
            SendNodePresentation();

            if (node.name!=null)
                SendNodeName();

            if (node.version != null)
                SendNodeVersion();

            if (node.sensors!=null)
                foreach (var sensor in node.sensors)
                {
                    SendSensorPresentation(sensor);
                }
        }

        public void SendNodePresentation()
        {
            Message message = new Message
            {
                nodeId = node.nodeId,
                sensorId = 255,
                messageType = MessageType.C_PRESENTATION,
                ack = false,
                subType = (int)SensorType.S_ARDUINO_NODE,
                //payload = 
                dateTime = DateTime.Now
            };

            client.SendMessage(message);
        }


        public void SendNodeName()
        {
            Message message = new Message
            {
                nodeId = node.nodeId,
                sensorId = 255,
                messageType = MessageType.C_INTERNAL,
                ack = false,
                subType = (int)InternalDataType.I_SKETCH_NAME,
                payload = node.name,
            };

            client.SendMessage(message);
        }

        public void SendNodeVersion()
        {
            Message message = new Message
            {
                nodeId = node.nodeId,
                sensorId = 255,
                messageType = MessageType.C_INTERNAL,
                ack = false,
                subType = (int)InternalDataType.I_SKETCH_VERSION,
                payload = node.version,
            };

            client.SendMessage(message);
        }



        public void SetNodeName(string nodeName)
        {
            node.name = nodeName;
            if (IsConnected())
                SendNodeName();
        }

        public void SetNodeVersion(string nodeVersion)
        {
            node.version = nodeVersion;
            if (IsConnected())
                SendNodeName();
        }

        public void AddSensor(Sensor sensor)
        {
            node.sensors.Add(sensor);
            if (IsConnected())
                SendSensorPresentation(sensor);
        }

        private void SendSensorPresentation(Sensor sensor)
        {
            Message message = new Message
            {
                nodeId = node.nodeId,
                sensorId = sensor.sensorId,
                messageType = MessageType.C_PRESENTATION,
                ack = false,
                subType = (int)sensor.sensorType,
                payload = sensor.description
            };

            client.SendMessage(message);
        }

    }
}
