/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.UI.Xaml;


namespace SerialController_Windows.Code
{
    public delegate void MessageEventHandler(Message message);
    public delegate void NodeEventHandler(Node node);
    public delegate void SensorEventHandler(Sensor sensor);

    public class SerialController
    {
        private SerialPort serialPort;
        public bool enableLogging = true;
        public bool enableAutoAssignId = true;

        public event MessageEventHandler OnMessageRecievedEvent;
        public event MessageEventHandler OnMessageSendEvent;
        public event NodeEventHandler OnNewNodeEvent;
        public event NodeEventHandler OnNodeUpdatedEvent;
        public event NodeEventHandler OnNodeLastSeenUpdatedEvent;
        public event NodeEventHandler OnNodeBatteryUpdatedEvent;
        public event SensorEventHandler OnNewSensorEvent;
        public event SensorEventHandler OnSensorUpdatedEvent;
        public event Action OnClearNodesListEvent;

        public MessagesLog messagesLog = new MessagesLog();
        private List<Node> nodes = new List<Node>();

        public SerialController(SerialPort serialPort)
        {
            this.serialPort = serialPort;
            this.serialPort.ReceivedDataEvent += RecieveSerialMessage;

        }

        public bool IsConnected()
        {
            return serialPort.IsConnected();
        }


        private async void SendMessage(string message)
        {
            await serialPort.SendMessage(message);
        }

        public void SendMessage(Message message)
        {
            message.incoming = false;

            if (OnMessageSendEvent != null)
                OnMessageSendEvent(message);

            string mes = String.Format("{0};{1};{2};{3};{4};{5}\n",
                message.nodeId,
                message.sensorId,
                (int)message.messageType,
                (message.ack) ? "1" : "0",
                message.subType,
                message.payload);

            SendMessage(mes);

            if (enableLogging)
                messagesLog.AddNewMessage(message);
        }


        private void RecieveSerialMessage(string message)
        {

            Message mes = ParseMessageFromString(message);
            mes.incoming = true;

            if (enableLogging)
                messagesLog.AddNewMessage(mes);

            if (OnMessageRecievedEvent != null)
                OnMessageRecievedEvent(mes);

            if (mes.isValid)
            {
                //Gateway ready
                if (mes.messageType == MessageType.C_INTERNAL
                    && mes.subType == (int)InternalDataType.I_GATEWAY_READY)
                    return;


                //Gateway log message
                if (mes.messageType == MessageType.C_INTERNAL
                    && mes.subType == (int)InternalDataType.I_LOG_MESSAGE)
                    return;

                //New ID request
                if (mes.nodeId == 255)
                {
                    if (mes.messageType == MessageType.C_INTERNAL
                        && mes.subType == (int)InternalDataType.I_ID_REQUEST)
                        if (enableAutoAssignId)
                            SendNewIdResponse();

                    return;
                }

                //Metric system request
                if (mes.messageType == MessageType.C_INTERNAL
                    && mes.subType == (int)InternalDataType.I_CONFIG)
                    SendMetricResponse(mes.nodeId);

                //Sensor request
                if (mes.messageType == MessageType.C_REQ)
                    ProceedRequestMessage(mes);


                UpdateNodeFromMessage(mes);
                UpdateSensorFromMessage(mes);
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

            SensorDataType dataType = (SensorDataType)mes.subType;
            SensorData data = sensor.GetData(dataType);
            if (data == null) return;

            SendSensorState(mes.nodeId, mes.sensorId, data);
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
                }
                else if (mes.messageType == MessageType.C_INTERNAL)
                {
                    if (mes.subType == (int)InternalDataType.I_SKETCH_NAME)
                    {
                        node.name = mes.payload;

                        if (OnNodeUpdatedEvent != null)
                            OnNodeUpdatedEvent(node);
                    }
                    else if (mes.subType == (int)InternalDataType.I_SKETCH_VERSION)
                    {
                        node.version = mes.payload;

                        if (OnNodeUpdatedEvent != null)
                            OnNodeUpdatedEvent(node);
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
                SensorDataType dataType = (SensorDataType)mes.subType;
                sensor.AddOrUpdateData(dataType, mes.payload);
            }
            else if (mes.messageType == MessageType.C_PRESENTATION)
            {
                sensor.SetSensorType((SensorType)mes.subType);

                if (!String.IsNullOrEmpty(mes.payload))
                    sensor.description = mes.payload;
            }



            if (isNewSensor && OnNewSensorEvent != null)
                OnNewSensorEvent(sensor);
            else
            if (OnSensorUpdatedEvent != null)
                OnSensorUpdatedEvent(sensor);


        }


        public Node GetNode(int id)
        {
            Node node = nodes.FirstOrDefault(x => x.nodeId == id);
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
                mes.isValid = true;
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


        public void SendSensorState(int nodeId, int sensorId, SensorData data)
        {
            Sensor sensor = GetNode(nodeId).GetSensor(sensorId);
            sensor.AddOrUpdateData(data);

            Message message = new Message();
            message.ack = false;
            message.messageType = MessageType.C_SET;
            message.nodeId = nodeId;
            message.payload = data.state;
            message.sensorId = sensorId;
            message.subType = (int)data.dataType;
            message.isValid = true;
            SendMessage(message);

            if (OnSensorUpdatedEvent != null)
                OnSensorUpdatedEvent(sensor);
        }

        public int GetFreeNodeId()
        {
            for (int i = 1; i < 254; i++)
            {
                bool found = false;

                foreach (var node in nodes)
                {
                    if (node.nodeId == i)
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
            mess.isValid = true;
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
            mess.isValid = true;
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
            message.isValid = true;
            SendMessage(message);
        }
    }
}
