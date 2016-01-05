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
    public delegate void LogEventHandler(string message);
    public delegate void ExceptionEventHandler(Exception exception);

    public class Gateway
    {
        public IComPort serialPort;
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
        public event Action OnUnexpectedlyDisconnectedEvent;
        public event Action OnConnectedEvent;
        public event LogEventHandler OnLogMessage;
        public event LogEventHandler OnLogStateMessage;

        public MessagesLog messagesLog = new MessagesLog();
        private List<Node> nodes = new List<Node>();
        private bool isConnected;


        public Gateway(IComPort serialPort)
        {
            this.serialPort = serialPort;
            this.serialPort.OnDataReceivedEvent += RecieveMessage;
            this.serialPort.OnDisconnectedEvent += OnSerialPortDisconnectedEvent;
            this.serialPort.OnConnectedEvent += OnSerialPortConnectedEvent;
        }


        private void LogMessage(string message)
        {
            OnLogMessage?.Invoke(message);
        }

        private void LogState(string message)
        {
            OnLogStateMessage?.Invoke(message);
        }

        public void Connect(string serialPortName)
        {
            if (isConnected)
                Disconnect();

            serialPort.Connect(serialPortName);
        }

        public void Disconnect()
        {
            isConnected = false;

            if(serialPort.IsConnected())
                serialPort.Disconnect();

            LogState("Gateway disconnected.");

            OnDisconnectedEvent?.Invoke();
        }

        private void OnSerialPortDisconnectedEvent()
        {
            if (isConnected)
            {
                isConnected = false;
                LogState("Gateway unexpectedly disconnected.");
                OnUnexpectedlyDisconnectedEvent?.Invoke();
            }
        }

        public bool IsConnected()
        {
            return isConnected;
        }



        private void OnSerialPortConnectedEvent()
        {
            isConnected = true;

            //LogState("Gateway connected.");

            OnConnectedEvent?.Invoke();
        }

        

        private void SendMessage(Message message)
        {
            if (!isConnected)
            {
                LogState("Failed to send message. Gateway is not connected.");
                return;
            }

            message.incoming = false;

            OnMessageSendEvent?.Invoke(message);

            UpdateSensorFromMessage(message);

            LogMessage(message.ToString());

            string mes = $"{message.nodeId};" +
                         $"{message.sensorId};" +
                         $"{(int)message.messageType};" +
                         $"{((message.ack) ? "1" : "0")};" +
                         $"{message.subType};" +
                         $"{message.payload}\n";


            serialPort.SendMessage(mes);

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

            LogMessage( message.ToString());

            OnMessageRecievedEvent?.Invoke(message);


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

                OnNewNodeEvent?.Invoke(node);

                LogState($"New node (id: {node.Id}) registered");
            }

            node.UpdateLastSeenNow();
            OnNodeLastSeenUpdatedEvent?.Invoke(node);


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


                    OnNodeUpdatedEvent?.Invoke(node);

                    LogState($"Node {node.Id} updated");
                }
                else if (mes.messageType == MessageType.C_INTERNAL)
                {
                    if (mes.subType == (int)InternalDataType.I_SKETCH_NAME)
                    {
                        node.name = mes.payload;

                        OnNodeUpdatedEvent?.Invoke(node);

                        LogState($"Node {node.Id} updated");
                    }
                    else if (mes.subType == (int)InternalDataType.I_SKETCH_VERSION)
                    {
                        node.version = mes.payload;

                        OnNodeUpdatedEvent?.Invoke(node);

                        LogState($"Node {node.Id} updated");
                    }
                    else if (mes.subType == (int)InternalDataType.I_BATTERY_LEVEL)
                    {
                        node.batteryLevel = Int32.Parse(mes.payload);
                        OnNodeBatteryUpdatedEvent?.Invoke(node);
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
                    LogState("Exception occurs when the serial port does not have time to write the data");

                    return;
                }

                sensor.SetSensorType((SensorType)mes.subType);

                if (!String.IsNullOrEmpty(mes.payload))
                    sensor.description = mes.payload;
            }



            if (isNewSensor)
            {
                OnNewSensorEvent?.Invoke(sensor);

                LogState($"New sensor (node id {sensor.nodeId}, sensor id: {sensor.sensorId}) registered");
            }
            else
            {
                OnSensorUpdatedEvent?.Invoke(sensor);
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
                mes = new Message
                {
                    isValid = false,
                    payload = message
                };
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
            Node node = GetNode(nodeId);
            if (node == null)
            {
                LogState($"Can`t send message. Node{nodeId} does not exist.");
                return;
            }
            Sensor sensor = node.GetSensor(sensorId);
            if (sensor == null)
            {
                LogState($"Can`t send message. Node{nodeId} Sensor{sensorId} does not exist.");
                return;
            }
            sensor.state = state;
            sensor.UnRemapSensorData();
            SendSensorState(sensor);
        }

        private void SendSensorState(Sensor sensor)
        {
            Message message = new Message
            {
                ack = false,
                messageType = MessageType.C_SET,
                nodeId = sensor.nodeId,
                payload = sensor.state,
                sensorId = sensor.sensorId,
                subType = (int) sensor.dataType
            };
            SendMessage(message);
        }

        public int GetFreeNodeId()
        {
            for (int i = 1; i < 254; i++)
            {
                bool found = nodes.Any(node => node.Id == i);

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

            Message mess = new Message
            {
                nodeId = 255,
                sensorId = 255,
                messageType = MessageType.C_INTERNAL,
                ack = false,
                subType = (int) InternalDataType.I_ID_RESPONSE,
                payload = freeId.ToString()
            };
            SendMessage(mess);
        }

        private void SendMetricResponse(int nodeId)
        {
            Message mess = new Message
            {
                nodeId = nodeId,
                sensorId = 255,
                messageType = MessageType.C_INTERNAL,
                ack = false,
                subType = (int) InternalDataType.I_CONFIG,
                payload = "M"
            };
            SendMessage(mess);
        }

        public void ClearNodesList()
        {
            nodes.Clear();

            OnClearNodesListEvent?.Invoke();
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
            Message message = new Message
            {
                ack = false,
                messageType = MessageType.C_INTERNAL,
                nodeId = nodeId,
                payload = "0",
                sensorId = 0,
                subType = (int) InternalDataType.I_REBOOT
            };
            SendMessage(message);
        }

        public GatewayInfo GetGatewayInfo()
        {
            GatewayInfo info = new GatewayInfo
            {
                isGatewayConnected = IsConnected(),
                gatewayNodesRegistered = nodes.Count
            };


            int sensors= nodes.Sum(node => node.sensors.Count);
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
