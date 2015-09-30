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

        private int nodeId = 100;//todo get nodeid from controller
        private int sensorId;
        private string nodeName;
        private string nodeVersion;
        private SensorType sensorType;


        public SoftNode(ISoftNodeClient client,string nodeName=null,string nodeVersion=null)
        {
            this.client = client;
            this.nodeName = nodeName;
            this.nodeVersion = nodeVersion;

            client.OnReceivedMessageEvent += OnReceivedSoftNodeMessage;
            client.OnConnected += OnConnected;

            if (client.IsConnected())
                OnConnected();
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

        public void OnConnected()
        {
            SendNodePresentation();

            if (nodeName!=null)
                SendNodeName();

            if (nodeVersion!=null)
                SendNodeVersion();

            //SendSensorsPresentation();
        }

        public void SendNodePresentation()
        {
            Message message = new Message
            {
                nodeId = nodeId,
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
                nodeId = nodeId,
                sensorId = 255,
                messageType = MessageType.C_INTERNAL,
                ack = false,
                subType = (int)InternalDataType.I_SKETCH_NAME,
                payload = nodeName,
            };

            client.SendMessage(message);
        }

        public void SendNodeVersion()
        {
            Message message = new Message
            {
                nodeId = nodeId,
                sensorId = 255,
                messageType = MessageType.C_INTERNAL,
                ack = false,
                subType = (int)InternalDataType.I_SKETCH_VERSION,
                payload = nodeVersion,
            };

            client.SendMessage(message);
        }

        public void SendSensorsPresentation()
        {
            Message message = new Message
            {
                nodeId = nodeId,
                sensorId = sensorId,
                messageType = MessageType.C_PRESENTATION,
                ack = false,
                subType = (int)sensorType,
                //payload = 
            };

            client.SendMessage(message);
        }

        public void SetNodeName(string nodeName)
        {
            this.nodeName = nodeName;
            if (IsConnected())
                SendNodeName();
        }

        public void SetNodeVersion(string nodeVersion)
        {
            this.nodeVersion = nodeVersion;
            if (IsConnected())
                SendNodeName();
        }
    }
}
