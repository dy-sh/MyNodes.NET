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
    public delegate void OnIdResponseReceivedHandler (int nodeId);
    public class SoftNode
    {
        private ISoftNodeClient client;
        private Node node;

        public event OnIdResponseReceivedHandler OnIdResponseReceived;
        public event OnReceivedMessageHandler OnReceivedMessageEvent;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client">client for connection</param>
        /// <param name="nodeId">if nodeId==255, node will ask id from gateway</param>
        /// <param name="nodeName">node name for node presentation</param>
        /// <param name="nodeVersion">node version for node presentation</param>
        public SoftNode(ISoftNodeClient client,int nodeId=255,string nodeName = null, string nodeVersion = null)
        {
            this.client = client;
            node = new Node();
            node.name = nodeName;
            node.version = nodeVersion;
            node.nodeId = nodeId;

            client.OnReceivedMessageEvent += OnReceivedSoftNodeMessage;
            client.OnConnected += OnConnected;

            if (client.IsConnected())
                OnConnected();
        }


        private void OnReceivedSoftNodeMessage(Message message)
        {
            if (message.nodeId == node.nodeId)
            {
                Console.WriteLine(message.ToString());


                //if ID_RESPONSE
                if (message.messageType == MessageType.C_INTERNAL
                    && message.subType == (int)InternalDataType.I_ID_RESPONSE)
                {
                    node.nodeId = Int32.Parse(message.payload);

                    if (OnIdResponseReceived != null)
                        OnIdResponseReceived(node.nodeId);

                    SendPresentation();
                }

                if (OnReceivedMessageEvent != null)
                    OnReceivedMessageEvent(message);
            }
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

        public void ConnectToServer(string url = "http://localhost:13122/")
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
            if (node.nodeId == 255)
                SendIdRequest();
            else
                SendPresentation();
        }

        private void SendPresentation()
        {
            SendNodePresentation();

            if (node.name != null)
                SendNodeName();

            if (node.version != null)
                SendNodeVersion();

            if (node.sensors != null)
                foreach (var sensor in node.sensors)
                {
                    SendSensorPresentation(sensor);
                }
        }

        private void SendNodePresentation()
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


        private void SendNodeName()
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

        private void SendNodeVersion()
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



        private void SetNodeName(string nodeName)
        {
            node.name = nodeName;
            if (IsConnected())
                SendNodeName();
        }

        private void SetNodeVersion(string nodeVersion)
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

        private void SendIdRequest()
        {
            Message message = new Message
            {
                nodeId = 255,
                sensorId = 255,
                messageType = MessageType.C_INTERNAL,
                ack = false,
                subType = (int)InternalDataType.I_ID_REQUEST,
                payload = node.version,
            };

            client.SendMessage(message);
        }

    }
}
