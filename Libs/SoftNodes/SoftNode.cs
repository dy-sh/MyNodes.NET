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
        private bool isPresentationCompleted;

        public event OnIdResponseReceivedHandler OnIdResponseReceived;
        public event OnReceivedMessageHandler OnReceivedMessage;
        public event DebugMessageEventHandler OnDebugTxRxMessage;
        public event DebugMessageEventHandler OnDebugNodeStateMessage;
        public event DebugMessageEventHandler OnConnectionFailed;
        public event DebugMessageEventHandler OnSendingMessageFailed;
        public event Action OnConnected;
        public event Action OnDisconnected;

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

            client.OnReceivedMessage += OnClientReceivedMessage;
            client.OnConnected += OnClientConnected;
            client.OnDisconnected += OnClientDisconnected;
            client.OnConnectionFailed += OnClientConnectionFailed;
            client.OnSendingMessageFailed += OnClientSendingMessageFailed;

            if (client.IsConnected())
                OnClientConnected();
        }

        public bool IsPresentationCompleted()
        {
            return isPresentationCompleted;
        }

        private void OnClientSendingMessageFailed(string message)
        {
            if (OnDebugNodeStateMessage != null)
                OnDebugNodeStateMessage("Sending message failed: " + message);

            if (OnSendingMessageFailed != null)
                OnSendingMessageFailed(message);
        }

        private void OnClientConnectionFailed(string message)
        {
            if (OnDebugNodeStateMessage != null)
                OnDebugNodeStateMessage("Connection failed: " + message);

            if (OnConnectionFailed != null)
                OnConnectionFailed(message);
        }

        private void OnClientDisconnected()
        {
            if (OnDebugNodeStateMessage != null)
                OnDebugNodeStateMessage("Disconnected");

            if (OnDisconnected != null)
                OnDisconnected();
        }


        private void OnClientReceivedMessage(Message message)
        {
            if (message.nodeId == node.nodeId)
            {
                if (OnDebugTxRxMessage != null)
                    OnDebugTxRxMessage(message.ToString());


                //if ID_RESPONSE
                if (message.messageType == MessageType.C_INTERNAL
                    && message.subType == (int)InternalDataType.I_ID_RESPONSE)
                {
                    node.nodeId = Int32.Parse(message.payload);

                    if (OnDebugNodeStateMessage != null)
                        OnDebugNodeStateMessage("Received new ID: "+ node.nodeId);

                    if (OnIdResponseReceived != null)
                        OnIdResponseReceived(node.nodeId);

                    SendPresentation();
                }

                if (OnReceivedMessage != null)
                    OnReceivedMessage(message);
            }
        }

        public void SendSensorData(int sensorId, SensorData data)
        {
            if (!IsConnected() || !IsPresentationCompleted()) return;

            Message message = new Message
            {
                nodeId = node.nodeId,
                sensorId = sensorId,
                messageType = MessageType.C_SET,
                ack = false,
                subType = (int)data.dataType,
                payload = data.state
            };

            if (OnDebugTxRxMessage != null)
                OnDebugTxRxMessage(message.ToString());

            client.SendMessage(message);
        }

        public void ConnectToServer(string url = "http://localhost:13122/")
        {
            if (OnDebugNodeStateMessage != null)
                OnDebugNodeStateMessage("Connecting to server: " + url);

            client.ConnectToServer(url);
        }

        public void Disconnect()
        {
            if (OnDebugNodeStateMessage != null)
                OnDebugNodeStateMessage("Disconnected");

            client.Disconnect();
        }

        public bool IsConnected()
        {
            return client.IsConnected() && node.nodeId != 255;
        }

        private void OnClientConnected()
        {
            if (OnDebugNodeStateMessage != null)
                OnDebugNodeStateMessage("Connected");

            if (node.nodeId == 255)
                SendIdRequest();
            else
            {
                if (OnDebugNodeStateMessage != null)
                    OnDebugNodeStateMessage("Node ID: "+node.nodeId);

                SendPresentation();
            }

            if (OnConnected != null)
                OnConnected();
        }

        private void SendPresentation()
        {
            if (!IsConnected()) return;

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

            isPresentationCompleted = true;
        }

        private void SendNodePresentation()
        {
            if (!IsConnected()) return;

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

            if (OnDebugNodeStateMessage != null)
                OnDebugNodeStateMessage("Sending node presentation");

            if (OnDebugTxRxMessage != null)
                OnDebugTxRxMessage(message.ToString());

            client.SendMessage(message);
        }


        private void SendNodeName()
        {
            if (!IsConnected()) return;

            Message message = new Message
            {
                nodeId = node.nodeId,
                sensorId = 255,
                messageType = MessageType.C_INTERNAL,
                ack = false,
                subType = (int)InternalDataType.I_SKETCH_NAME,
                payload = node.name,
            };

            if (OnDebugNodeStateMessage != null)
                OnDebugNodeStateMessage("Sending node name");

            if (OnDebugTxRxMessage != null)
                OnDebugTxRxMessage(message.ToString());

            client.SendMessage(message);
        }

        private void SendNodeVersion()
        {
            if (!IsConnected()) return;

            Message message = new Message
            {
                nodeId = node.nodeId,
                sensorId = 255,
                messageType = MessageType.C_INTERNAL,
                ack = false,
                subType = (int)InternalDataType.I_SKETCH_VERSION,
                payload = node.version,
            };

            if (OnDebugNodeStateMessage != null)
                OnDebugNodeStateMessage("Sending node version");

            if (OnDebugTxRxMessage != null)
                OnDebugTxRxMessage(message.ToString());

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
            if (!IsConnected()) return;

            Message message = new Message
            {
                nodeId = node.nodeId,
                sensorId = sensor.sensorId,
                messageType = MessageType.C_PRESENTATION,
                ack = false,
                subType = (int)sensor.type,
                payload = sensor.description
            };

            if (OnDebugNodeStateMessage != null)
                OnDebugNodeStateMessage("Sending sensor "+sensor.sensorId+" presentation");

            if (OnDebugTxRxMessage != null)
                OnDebugTxRxMessage(message.ToString());

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
                subType = (int)InternalDataType.I_ID_REQUEST
            };

            if (OnDebugNodeStateMessage != null)
                OnDebugNodeStateMessage("Sending ID request");

            if (OnDebugTxRxMessage != null)
                OnDebugTxRxMessage(message.ToString());

            client.SendMessage(message);
        }

    }
}
