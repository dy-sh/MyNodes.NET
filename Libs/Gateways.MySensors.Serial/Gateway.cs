/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyNetSensors.Gateways.MySensors.Serial
{
    public delegate void MessageEventHandler(Message message);
    public delegate void NodeEventHandler(Node node);
    public delegate void SensorEventHandler(Sensor sensor);
    public delegate void LogEventHandler(string message);
    public delegate void ExceptionEventHandler(Exception exception);
    public delegate void GatewayStateEventHandler(GatewayState gatewayState);

    public enum GatewayState
    {
        Disconnected,
        ConnectingToPort,
        ConnectingToGateway,
        Connected
    }

    public class Gateway
    {
        public IComPort serialPort;
        public bool enableAutoAssignId = true;
        public bool messagesLogEnabled = true;
        public bool endlessConnectionAttempts = true;
        public bool reconnectIfDisconnected = true;
        public int ATTEMPTS_TO_COMMUNICATE = 5;

        public event MessageEventHandler OnMessageRecievedEvent;
        public event MessageEventHandler OnMessageSendEvent;
        public event NodeEventHandler OnRemoveNodeEvent;
        public event NodeEventHandler OnNewNodeEvent;
        public event NodeEventHandler OnNodeUpdatedEvent;
        public event NodeEventHandler OnNodeLastSeenUpdatedEvent;
        public event NodeEventHandler OnNodeBatteryUpdatedEvent;
        public event SensorEventHandler OnNewSensorEvent;
        public event SensorEventHandler OnSensorUpdatedEvent;
        public event Action OnRemoveAllNodesEvent;
        public event Action OnDisconnectedEvent;
        public event Action OnUnexpectedlyDisconnectedEvent;
        public event Action OnConnectedEvent;
        public event GatewayStateEventHandler OnGatewayStateChangedEvent;
        public event LogEventHandler OnLogMessage;
        public event LogEventHandler OnLogInfo;
        public event LogEventHandler OnLogError;

        private List<Message> messagesLog = new List<Message>();
        private List<Node> nodes = new List<Node>();

        GatewayAliveChecker gatewayAliveChecker;

        private GatewayState gatewayState = GatewayState.Disconnected;

        private IMySensorsRepository db;
        private IMySensorsMessagesRepository hisotryDb;

        public Gateway(IComPort serialPort, IMySensorsRepository db = null, IMySensorsMessagesRepository hisotryDb = null)
        {
            this.db = db;
            this.hisotryDb = hisotryDb;

            messagesLog = hisotryDb?.GetMessages();
            nodes = db?.GetNodes();

            gatewayAliveChecker = new GatewayAliveChecker(this);

            this.serialPort = serialPort;
            this.serialPort.OnDataReceivedEvent += RecieveMessage;
            this.serialPort.OnDisconnectedEvent += OnSerialPortDisconnectedEvent;
            this.serialPort.OnConnectedEvent += TryToCommunicateWithGateway;
            this.serialPort.OnLogError += LogError;
            this.serialPort.OnLogInfo += OnLogInfo;
        }


        public List<Message> GetMessagesLog()
        {
            return messagesLog;
        }

        public void ClearMessagesLog()
        {
            messagesLog.Clear();
            hisotryDb?.RemoveAllMessages();
        }


        internal void LogInfo(string message)
        {
            OnLogInfo?.Invoke(message);
        }

        internal void LogError(string message)
        {
            OnLogError?.Invoke(message);
        }


        public GatewayState GetGatewayState()
        {
            return gatewayState;
        }

        private void SetGatewayState(GatewayState newState)
        {

            switch (newState)
            {
                case GatewayState.Disconnected:
                    LogInfo("Disconnected.");
                    break;
                case GatewayState.ConnectingToPort:
                    LogInfo("Trying to connect...");
                    break;
                case GatewayState.ConnectingToGateway:
                    LogInfo("Trying to communicate...");
                    break;
                case GatewayState.Connected:
                    LogInfo("Gateway connected.");
                    break;
            }

            if (gatewayState == GatewayState.Connected && newState != GatewayState.Connected)
                OnDisconnectedEvent?.Invoke();
            else if (gatewayState != GatewayState.Connected && newState == GatewayState.Connected)
                OnConnectedEvent?.Invoke();

            gatewayState = newState;
            OnGatewayStateChangedEvent?.Invoke(gatewayState);
        }

        public async Task Connect(string serialPortName)
        {
            if (gatewayState != GatewayState.Disconnected)
                Disconnect();

            SetGatewayState(GatewayState.ConnectingToPort);

            if (endlessConnectionAttempts)
            {
                while (gatewayState == GatewayState.ConnectingToPort)
                {
                    serialPort.Connect(serialPortName);
                    await Task.Delay(1000);
                }
                while (gatewayState == GatewayState.ConnectingToGateway)
                {
                    await Task.Delay(1000);
                }
            }
            else
            {
                serialPort.Connect(serialPortName);
            }
        }

        public void Disconnect()
        {
            SetGatewayState(GatewayState.Disconnected);

            if (serialPort.IsConnected())
                serialPort.Disconnect();
        }

        internal void OnSerialPortDisconnectedEvent()
        {
            if (gatewayState == GatewayState.Connected)
            {
                LogError("Port unexpectedly disconnected.");

                SetGatewayState(GatewayState.Disconnected);

                if (serialPort.IsConnected())
                    serialPort.Disconnect();

                OnUnexpectedlyDisconnectedEvent?.Invoke();
                if (reconnectIfDisconnected)
                    Connect(serialPort.GetPortName());
            }
        }

        public bool IsConnected()
        {
            return gatewayState == GatewayState.Connected;
        }


        private async void TryToCommunicateWithGateway()
        {
            SetGatewayState(GatewayState.ConnectingToGateway);

            if (endlessConnectionAttempts)
            {
                while (gatewayState == GatewayState.ConnectingToGateway)
                {
                    for (int i = 0; i < ATTEMPTS_TO_COMMUNICATE; i++)
                    {
                        if (gatewayState != GatewayState.ConnectingToGateway)
                            return;

                        SendGetwayVersionRequest();
                        await Task.Delay(1000);
                    }
                    LogError("Gateway is not responding.");
                }
            }
            else
            {
                for (int i = 0; i < ATTEMPTS_TO_COMMUNICATE; i++)
                {
                    if (gatewayState != GatewayState.ConnectingToGateway)
                        return;

                    SendGetwayVersionRequest();
                    await Task.Delay(1000);
                }
                LogError("Gateway is not responding.");
            }


        }


        private void SendMessage(Message message)
        {
            if (gatewayState != GatewayState.Connected
                && gatewayState != GatewayState.ConnectingToGateway)
            {
                LogError("Failed to send message. Gateway is not connected.");
                return;
            }

            message.incoming = false;

            OnMessageSendEvent?.Invoke(message);

            UpdateSensorFromMessage(message);

            string mes = message.ParseToMySensorsMessage();
            serialPort.SendMessage(mes);

            AddMessageToLog(message);
        }


        public void AddMessageToLog(Message message)
        {
            if (!messagesLogEnabled)
                return;

            //ignore check alive message
            if (message.nodeId == 0
                && message.messageType == MessageType.C_INTERNAL
                && message.subType == (int)InternalDataType.I_VERSION)
                return;

            messagesLog.Add(message);
            hisotryDb?.AddMessage(message);

            OnLogMessage?.Invoke(message.ToString());
        }

        public void RecieveMessage(string message)
        {
            Message mes = null;
            try
            {
                mes = new Message(message);
            }
            catch
            {
                LogError($"Failed to process incoming message: [{message}]");
            }
            RecieveMessage(mes);
        }

        public void RecieveMessage(Message message)
        {
            message.incoming = true;

            AddMessageToLog(message);

            OnMessageRecievedEvent?.Invoke(message);


            //Gateway ready
            if (message.messageType == MessageType.C_INTERNAL && message.subType == (int)InternalDataType.I_GATEWAY_READY)
                return;


            //Gateway log message
            if (message.messageType == MessageType.C_INTERNAL && message.subType == (int)InternalDataType.I_LOG_MESSAGE)
                return;

            //New ID request
            if (message.nodeId == 255)
            {
                if (message.messageType == MessageType.C_INTERNAL && message.subType == (int)InternalDataType.I_ID_REQUEST)
                    if (enableAutoAssignId)
                        SendNewIdResponse();

                return;
            }

            //Metric system request
            if (message.messageType == MessageType.C_INTERNAL && message.subType == (int)InternalDataType.I_CONFIG)
                SendMetricResponse(message.nodeId);

            //Sensor request
            if (message.messageType == MessageType.C_REQ)
                ProceedRequestMessage(message);

            //Gateway vesrion (alive) response
            if (message.nodeId == 0
                && message.messageType == MessageType.C_INTERNAL
                && message.subType == (int)InternalDataType.I_VERSION)
            {
                if (gatewayState != GatewayState.Connected)
                    SetGatewayState(GatewayState.Connected);
            }

            //request to node
            if (message.nodeId == 0)
                return;

            UpdateNodeFromMessage(message);
            UpdateSensorFromMessage(message);
        }




        private void ProceedRequestMessage(Message mes)
        {
            if (mes.messageType != MessageType.C_REQ)
                return;

            Node node = GetNode(mes.nodeId);
            Sensor sensor = node?.GetSensor(mes.sensorId);
            if (sensor == null)
            {
                LogInfo($"Can't answer to REQ message. Node{mes.nodeId} Sensor{mes.sensorId} does not registered.");
                return;
            }

            //update sensor data type
            if (sensor.dataType != (SensorDataType)mes.subType)
            {
                sensor.dataType = (SensorDataType)mes.subType;
                db?.UpdateSensor(sensor);
            }

            SendSensorState(sensor);
        }


        private void UpdateNodeFromMessage(Message mes)
        {
            Node node = GetNode(mes.nodeId);

            if (node == null)
            {
                node = new Node(mes.nodeId);
                nodes.Add(node);
                db?.AddNode(node);

                OnNewNodeEvent?.Invoke(node);

                LogInfo($"Node{node.Id} registered.");
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
                        LogInfo($"Node{node.Id} is not repeating");

                        OnNodeUpdatedEvent?.Invoke(node);
                        db?.UpdateNode(node);

                    }
                    else if (mes.subType == (int)SensorType.S_ARDUINO_REPEATER_NODE)
                    {
                        node.isRepeatingNode = true;
                        LogInfo($"Node{node.Id} is repeating");

                        OnNodeUpdatedEvent?.Invoke(node);
                        db?.UpdateNode(node);
                    }
                }
                else if (mes.messageType == MessageType.C_INTERNAL)
                {
                    if (mes.subType == (int)InternalDataType.I_SKETCH_NAME)
                    {
                        node.name = mes.payload;
                        LogInfo($"Node{node.Id} name: [{node.name}]");

                        OnNodeUpdatedEvent?.Invoke(node);
                        db?.UpdateNode(node);
                    }
                    else if (mes.subType == (int)InternalDataType.I_SKETCH_VERSION)
                    {
                        node.version = mes.payload;
                        LogInfo($"Node{node.Id} version: [{node.version}]");

                        OnNodeUpdatedEvent?.Invoke(node);
                        db?.UpdateNode(node);
                    }
                    else if (mes.subType == (int)InternalDataType.I_BATTERY_LEVEL)
                    {
                        node.batteryLevel = Int32.Parse(mes.payload);
                        LogInfo($"Node{node.Id} battery level: [{node.batteryLevel}]");

                        OnNodeBatteryUpdatedEvent?.Invoke(node);
                        db?.UpdateNode(node);
                    }
                }
            }
        }

        public void UpdateSensorFromMessage(Message mes)
        {
            //if internal node message
            if (mes.sensorId == 255)
                return;

            if (mes.messageType != MessageType.C_PRESENTATION && mes.messageType != MessageType.C_SET)
                return;

            Node node = GetNode(mes.nodeId);

            Sensor sensor = node.GetSensor(mes.sensorId);
            bool isNewSensor = false;

            if (sensor == null)
            {
                sensor = node.AddSensor(mes.sensorId);
                db?.AddSensor(sensor);
                isNewSensor = true;
            }

            if (mes.messageType == MessageType.C_SET)
            {
                sensor.dataType = (SensorDataType)mes.subType;
                sensor.state = mes.payload;
            }
            else if (mes.messageType == MessageType.C_PRESENTATION)
            {
                sensor.SetDefaultSensorType((SensorType)mes.subType);

                if (!String.IsNullOrEmpty(mes.payload))
                    sensor.description = mes.payload;
            }


            if (isNewSensor)
            {
                OnNewSensorEvent?.Invoke(sensor);

                LogInfo($"Node{sensor.nodeId} Sensor{sensor.sensorId} registered");
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


        public List<Node> GetNodes()
        {
            return nodes;
        }




        public void SendSensorState(int nodeId, int sensorId, string state)
        {
            Node node = GetNode(nodeId);
            if (node == null)
            {
                LogError($"Can`t send message. Node{nodeId} does not exist.");
                return;
            }
            Sensor sensor = node.GetSensor(sensorId);
            if (sensor == null)
            {
                LogError($"Can`t send message. Node{nodeId} Sensor{sensorId} does not exist.");
                return;
            }
            sensor.state = state;


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
                subType = (int)sensor.dataType
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
                subType = (int)InternalDataType.I_ID_RESPONSE,
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
                subType = (int)InternalDataType.I_CONFIG,
                payload = "M"
            };
            SendMessage(mess);
        }


        public void SendGetwayVersionRequest()
        {
            Message mess = new Message
            {
                nodeId = 0,
                sensorId = 0,
                messageType = MessageType.C_INTERNAL,
                ack = false,
                subType = (int)InternalDataType.I_VERSION,
                payload = ""
            };
            SendMessage(mess);
        }


        public void RemoveAllNodes()
        {
            nodes.Clear();

            OnRemoveAllNodesEvent?.Invoke();
            db?.RemoveAllNodesAndSensors();
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
                subType = (int)InternalDataType.I_REBOOT
            };
            SendMessage(message);
        }

        public GatewayInfo GetGatewayInfo()
        {
            GatewayInfo info = new GatewayInfo
            {
                state = gatewayState,
                isGatewayConnected = IsConnected(),
                gatewayNodesRegistered = nodes.Count
            };


            int sensors = nodes.Sum(node => node.sensors.Count);
            info.gatewaySensorsRegistered = sensors;

            return info;
        }


 
        public void RemoveNode(int nodeId)
        {
            Node oldNode = GetNode(nodeId);
            if (oldNode == null)
                return;

            OnRemoveNodeEvent?.Invoke(oldNode);

            db?.RemoveNode(oldNode.Id);
            nodes.Remove(oldNode);
        }
    }
}
