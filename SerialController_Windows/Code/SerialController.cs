using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.UI.Xaml;

namespace SerialController_Windows.Code
{
    public delegate void OnMessageRecievedEventHandler(Message message);
    public delegate void OnNewNodeEventHandler(Node node);
    public delegate void OnNodeUpdatedEventHandler(Node node);
    public delegate void OnNewSensorEventHandler(Sensor sensor);
    public delegate void OnSensorUpdatedEventHandler(Sensor sensor);

    public class SerialController
    {
        private SerialPort serialPort;
        public bool enableLogging = true;

        public event OnMessageRecievedEventHandler OnMessageRecievedEvent;
        public event OnNewNodeEventHandler OnNewNodeEvent;
        public event OnNodeUpdatedEventHandler OnNodeUpdatedEvent;
        public event OnNewSensorEventHandler OnNewSensorEvent;
        public event OnSensorUpdatedEventHandler OnSensorUpdatedEvent;

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
            string mes=String.Format("{0};{1};{2};{3};{4};{5};\n",
                message.nodeId,
                message.sensorId,
                (int)message.messageType,
                (message.ack)?"1":"0",
                message.subType,
                message.payload);
            SendMessage(mes);
        }


        private void RecieveSerialMessage(string message)
        {

            Message mes = ParseMessageFromString(message);

            if (enableLogging)
                messagesLog.AddNewMessage(mes);

            if (OnMessageRecievedEvent != null)
                OnMessageRecievedEvent(mes);

            if (mes.isValid)
            {
                if (mes.messageType==MessageType.C_INTERNAL
                    && mes.subType==(int)InternalDataType.I_LOG_MESSAGE)
                    return;

                UpdateNodeFromMessage(mes);
                UpdateSensorFromMessage(mes);
            }
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
            else
            {
                node.UpdateLastSeenNow();

                if (OnNodeUpdatedEvent != null)
                    OnNodeUpdatedEvent(node);
            }
        }

        public void UpdateSensorFromMessage(Message mes)
        {
            if (mes.messageType != MessageType.C_PRESENTATION
                 && mes.messageType != MessageType.C_SET)
                return;

            Node node = GetNode(mes.nodeId);
            Sensor sensor = node.GetSensor(mes.sensorId);

            if (sensor == null)
            {
                sensor = node.AddSensor(mes.sensorId);

                if (OnNewSensorEvent != null)
                    OnNewSensorEvent(sensor);
            }

            if (mes.messageType == MessageType.C_SET)
            {
                SensorDataType dataType = (SensorDataType)mes.subType;
                sensor.AddOrUpdateData(dataType, mes.payload);
            }
            else if (mes.messageType == MessageType.C_PRESENTATION)
            {
                sensor.SetSensorType((SensorType)mes.subType);
            }

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
                mes.dateTime = DateTime.Now;
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
                mes.dateTime = DateTime.Now;
                mes.isValid = false;
                mes.payload = message;
            }
            return mes;
        }


        public List<Node> GetNodes()
        {
            return nodes;
        }


        public void ChangeSensorState(int nodeId, int sensorId, SensorData data)
        {
            GetNode(nodeId).GetSensor(sensorId).AddOrUpdateData(data);

            Message message = new Message();
            message.ack = false;
            message.dateTime = DateTime.Now;
            message.isValid = true;
            message.messageType = MessageType.C_SET;
            message.nodeId = nodeId;
            message.payload = data.state;
            message.sensorId = sensorId;
            message.subType = (int)data.dataType;
            SendMessage(message);
        }
    }
}
