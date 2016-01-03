/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyNetSensors.Gateways
{
    public delegate void MessageEventHandler(Message message);
    public delegate void NodeEventHandler(Node node);
    public delegate void SensorEventHandler(Sensor sensor);
    public delegate void DebugMessageEventHandler(string message);
    public delegate void ExceptionEventHandler(Exception exception);

    public class Gateway
    {
        private IComPort serialPort;
        public bool storeMessages = true;
        public bool enableAutoAssignId = true;

        public event MessageEventHandler OnMessageRecievedEvent;
        public event MessageEventHandler OnMessageSendEvent;
        public event NodeEventHandler OnDeleteNodeEvent;
        public event NodeEventHandler OnNewNodeEvent;
        public event NodeEventHandler OnNodeUpdatedEvent;
        public event NodeEventHandler OnNodeLastSeenUpdatedEvent;
        public event NodeEventHandler OnNodeBatteryUpdatedEvent;
        public event SensorEventHandler OnNewSensorEvent;
        public event SensorEventHandler OnSensorUpdatedEvent;
        public event Action OnClearNodesListEvent;
        public event Action OnDisconnectedEvent;
        public event Action OnConnectedEvent;
        public event DebugMessageEventHandler OnDebugTxRxMessage;
        public event DebugMessageEventHandler OnDebugGatewayStateMessage;

        public MessagesLog messagesLog = new MessagesLog();
        private List<Node> nodes = new List<Node>();
        private bool isConnected;

        private void DebugTxRx(string message)
        {
            if (OnDebugTxRxMessage != null)
                OnDebugTxRxMessage(message);
        }

        private void DebugGatewayState(string message)
        {
            if (OnDebugGatewayStateMessage != null)
                OnDebugGatewayStateMessage(message);
        }

        public void Connect(IComPort serialPort)
        {
            if (isConnected)
                Disconnect();

            this.serialPort = serialPort;
            this.serialPort.OnDataReceivedEvent += RecieveMessage;
            this.serialPort.OnDisconnectedEvent += OnSerialPortDisconnectedEvent;
            isConnected = true;

            DebugGatewayState("Gateway connected.");

            if (OnConnectedEvent != null)
                OnConnectedEvent();
        }

        public void Disconnect()
        {
            isConnected = false;
            if (serialPort != null)
            {
                serialPort.OnDataReceivedEvent -= RecieveMessage;
                serialPort.OnDisconnectedEvent -= OnSerialPortDisconnectedEvent;
                serialPort = null;
            }

            DebugGatewayState("Gateway disconnected.");


            if (OnDisconnectedEvent != null)
                OnDisconnectedEvent();
        }

        public bool IsConnected()
        {
            return (isConnected && serialPort.IsConnected());
        }

        private void OnSerialPortDisconnectedEvent()
        {
            Disconnect();
        }


        private void SendToSerial(string message)
        {
            if (!isConnected)
            {
                throw new Exception("Failed to send message. Serial port is not connected.");
                return;
            }

            serialPort.SendMessage(message);
        }



        private void SendMessage(Message message)
        {
            message.incoming = false;

            if (OnMessageSendEvent != null)
                OnMessageSendEvent(message);

            UpdateSensorFromMessage(message);

            //todo if (message.messageType == MessageType.C_SET)
                //message = DeRemapMessage(message);

            DebugTxRx($"TX: {message.ToString()}");

            string ack = (message.ack) ? "1" : "0";
            string mes = $"{message.nodeId};{message.sensorId};{(int)message.messageType};{ack};{message.subType};{message.payload}\n";


            SendToSerial(mes);

            if (storeMessages)
                messagesLog.AddNewMessage(message);
        }


        public void RecieveMessage(string message)
        {
            Message mes = ParseMessageFromString(message);
            RecieveMessage(mes);
        }

        public void RecieveMessage(Message message)
        {
            message.incoming = true;

            if (storeMessages)
                messagesLog.AddNewMessage(message);

            DebugTxRx($"RX: { message.ToString()}");

            if (OnMessageRecievedEvent != null)
                OnMessageRecievedEvent(message);


            if (message.isValid)
            {
                //Gateway ready
                if (message.messageType == MessageType.C_INTERNAL
                    && message.subType == (int)InternalDataType.I_GATEWAY_READY)
                    return;


                //Gateway log message
                if (message.messageType == MessageType.C_INTERNAL
                    && message.subType == (int)InternalDataType.I_LOG_MESSAGE)
                    return;

                //New ID request
                if (message.nodeId == 255)
                {
                    if (message.messageType == MessageType.C_INTERNAL
                        && message.subType == (int)InternalDataType.I_ID_REQUEST)
                        if (enableAutoAssignId)
                            SendNewIdResponse();

                    return;
                }

                //Metric system request
                if (message.messageType == MessageType.C_INTERNAL
                    && message.subType == (int)InternalDataType.I_CONFIG)
                    SendMetricResponse(message.nodeId);

                //Sensor request
                if (message.messageType == MessageType.C_REQ)
                    ProceedRequestMessage(message);


                UpdateNodeFromMessage(message);
                UpdateSensorFromMessage(message);
            }
        }

        private void ProceedRequestMessage(Message mes)
        {
            if (mes.messageType != MessageType.C_REQ)
                return;

            Node node = GetNode(mes.nodeId);
            if (node == null) return;

            Sensor sensor = node.GetSensor(mes.sensorId);
            if (sensor == null) return;

            sensor.dataType = (SensorDataType)mes.subType;
            sensor.state = mes.payload;

            SendSensorState(sensor);
        }


        private void UpdateNodeFromMessage(Message mes)
        {
            Node node = GetNode(mes.nodeId);

            if (node == null)
            {
                node = new Node(mes.nodeId);
                nodes.Add(node);

                if (OnNewNodeEvent != null)
                    OnNewNodeEvent(node);

                DebugGatewayState($"New node (id: {node.Id}) registered");
            }

            node.UpdateLastSeenNow();
            if (OnNodeLastSeenUpdatedEvent != null)
                OnNodeLastSeenUpdatedEvent(node);


            if (mes.sensorId == 255)
            {
                if (mes.messageType == MessageType.C_PRESENTATION)
                {
                    if (mes.subType == (int)SensorType.S_ARDUINO_NODE)
                    {
                        node.isRepeatingNode = false;
                    }
                    else if (mes.subType == (int)SensorType.S_ARDUINO_REPEATER_NODE)
                    {
                        node.isRepeatingNode = true;
                    }


                    if (OnNodeUpdatedEvent != null)
                        OnNodeUpdatedEvent(node);

                    DebugGatewayState($"Node {node.Id} updated");
                }
                else if (mes.messageType == MessageType.C_INTERNAL)
                {
                    if (mes.subType == (int)InternalDataType.I_SKETCH_NAME)
                    {
                        node.name = mes.payload;

                        if (OnNodeUpdatedEvent != null)
                            OnNodeUpdatedEvent(node);

                        DebugGatewayState($"Node {node.Id} updated");
                    }
                    else if (mes.subType == (int)InternalDataType.I_SKETCH_VERSION)
                    {
                        node.version = mes.payload;

                        if (OnNodeUpdatedEvent != null)
                            OnNodeUpdatedEvent(node);

                        DebugGatewayState($"Node {node.Id} updated");
                    }
                    else if (mes.subType == (int)InternalDataType.I_BATTERY_LEVEL)
                    {
                        node.batteryLevel = Int32.Parse(mes.payload);
                        if (OnNodeBatteryUpdatedEvent != null)
                            OnNodeBatteryUpdatedEvent(node);
                        return;
                    }
                }
            }

        }

        public void UpdateSensorFromMessage(Message mes)
        {
            //if internal node message
            if (mes.sensorId == 255)
                return;

            if (mes.messageType != MessageType.C_PRESENTATION
                && mes.messageType != MessageType.C_SET)
                return;

            Node node = GetNode(mes.nodeId);

            Sensor sensor = node.GetSensor(mes.sensorId);
            bool isNewSensor = false;

            if (sensor == null)
            {
                sensor = node.AddSensor(mes.sensorId);
                isNewSensor = true;
            }

            if (mes.messageType == MessageType.C_SET)
            {
                sensor.dataType= (SensorDataType)mes.subType;
                sensor.state = mes.payload;
                sensor.RemapSensorData();
            }
            else if (mes.messageType == MessageType.C_PRESENTATION)
            {
                if (mes.subType < 0 || mes.subType > (int)Enum.GetValues(typeof(SensorType)).Cast<SensorType>().Max())
                {
                    DebugGatewayState("Exception occurs when the serial port does not have time to write the data");

                    throw new ArgumentOutOfRangeException(
                        "Exception occurs when the serial port does not have time to write the data");
                }

                sensor.SetSensorType((SensorType)mes.subType);

                if (!String.IsNullOrEmpty(mes.payload))
                    sensor.description = mes.payload;
            }



            if (isNewSensor)
            {
                if (OnNewSensorEvent != null)
                    OnNewSensorEvent(sensor);

                DebugGatewayState($"New sensor (node id {sensor.nodeId}, sensor id: {sensor.sensorId}) registered");
            }
            else
            {
                if (OnSensorUpdatedEvent != null)
                    OnSensorUpdatedEvent(sensor);
            }

        }


        public Node GetNode(int nodeId)
        {
            Node node = nodes.FirstOrDefault(x => x.Id == nodeId);
            return node;
        }



        public Message ParseMessageFromString(string message)
        {
            var mes = new Message();

            try
            {
                string[] arguments = message.Split(new char[] { ';' }, 6);
                mes.nodeId = Int32.Parse(arguments[0]);
                mes.sensorId = Int32.Parse(arguments[1]);
                mes.messageType = (MessageType)Int32.Parse(arguments[2]);
                mes.ack = arguments[3] == "1";
                mes.subType = Int32.Parse(arguments[4]);
                mes.payload = arguments[5];
            }
            catch
            {
                mes = new Message();
                mes.isValid = false;
                mes.payload = message;
            }
            return mes;
        }


        public List<Node> GetNodes()
        {
            return nodes;
        }

        public void AddNode(Node node)
        {
            nodes.Add(node);
        }


        public void SendSensorState(int nodeId,int sensorId,string state)
        {
            Sensor sensor = GetNode(nodeId).GetSensor(sensorId);
            sensor.state = state;
            sensor.UnRemapSensorData();
            SendSensorState(sensor);
        }

        private void SendSensorState(Sensor sensor)
        {
            Message message = new Message();
            message.ack = false;
            message.messageType = MessageType.C_SET;
            message.nodeId = sensor.nodeId;
            message.payload = sensor.state;
            message.sensorId = sensor.sensorId;
            message.subType = (int)sensor.dataType;
            SendMessage(message);
        }

        public int GetFreeNodeId()
        {
            for (int i = 1; i < 254; i++)
            {
                bool found = false;

                foreach (var node in nodes)
                {
                    if (node.Id == i)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    return i;
                }
            }

            return 255;
        }

        public void SendNewIdResponse()
        {
            int freeId = GetFreeNodeId();

            Message mess = new Message();
            mess.nodeId = 255;
            mess.sensorId = 255;
            mess.messageType = MessageType.C_INTERNAL;
            mess.ack = false;
            mess.subType = (int)InternalDataType.I_ID_RESPONSE;
            mess.payload = freeId.ToString();
            SendMessage(mess);
        }

        private void SendMetricResponse(int nodeId)
        {
            Message mess = new Message();
            mess.nodeId = nodeId;
            mess.sensorId = 255;
            mess.messageType = MessageType.C_INTERNAL;
            mess.ack = false;
            mess.subType = (int)InternalDataType.I_CONFIG;
            mess.payload = "M";
            SendMessage(mess);
        }

        public void ClearNodesList()
        {
            nodes.Clear();

            if (OnClearNodesListEvent != null)
                OnClearNodesListEvent();
        }

        public async Task SendRebootToAllNodes()
        {
            for (int i = 1; i <= 254; i++)
            {
                SendReboot(i);
                await Task.Delay(10);
            }
        }

        public void SendReboot(int nodeId)
        {
            Message message = new Message();
            message.ack = false;
            message.messageType = MessageType.C_INTERNAL;
            message.nodeId = nodeId;
            message.payload = "0";
            message.sensorId = 0;
            message.subType = (int)InternalDataType.I_REBOOT;
            SendMessage(message);
        }

        public GatewayInfo GetGatewayInfo()
        {
            GatewayInfo info =new GatewayInfo();

            info.isGatewayConnected = IsConnected();

            info.gatewayNodesRegistered = nodes.Count;

            int sensors=0;
            foreach (var node in nodes)
                sensors += node.sensors.Count;
            info.gatewaySensorsRegistered = sensors;

            return info;
        }

        public void SetSensorDbId(int nodeId,int sensorId, int dbId)
        {
            Node node = GetNode(nodeId);
            Sensor sensor = node.GetSensor(sensorId);
            sensor.Id = dbId;
        }

        public void UpdateNodeSettings(Node node)
        {
            Node oldNode = GetNode(node.Id);
            oldNode.name = node.name;
            foreach (var sensor in node.sensors)
            {
                Sensor oldSensor = oldNode.GetSensor(sensor.sensorId);
                oldSensor.description = sensor.description;
                oldSensor.storeHistoryEnabled = sensor.storeHistoryEnabled;
                oldSensor.storeHistoryEveryChange = sensor.storeHistoryEveryChange;
                oldSensor.storeHistoryWithInterval = sensor.storeHistoryWithInterval;
                oldSensor.invertData = sensor.invertData;
                oldSensor.remapEnabled = sensor.remapEnabled;
                oldSensor.remapFromMin = sensor.remapFromMin;
                oldSensor.remapFromMax = sensor.remapFromMax;
                oldSensor.remapToMin = sensor.remapToMin;
                oldSensor.remapToMax = sensor.remapToMax;
            }
        }

        public void DeleteNode(int nodeId)
        {
            Node oldNode = GetNode(nodeId);

            OnDeleteNodeEvent?.Invoke(oldNode);

            if (oldNode!=null) 
                nodes.Remove(oldNode);
        }
    }
}
