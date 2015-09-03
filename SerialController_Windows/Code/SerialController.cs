using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace SerialController_Windows.Code
{
    public delegate void OnMessageRecievedEventHandler(Message message);
    public delegate void OnNewNodeEventHandler(Node node);
    public delegate void OnNodeUpdatedEventHandler(Node node);
    public delegate void OnNewSensorEventHandler(Sensor sensor);
    public delegate void OnSensorUpdatedEventHandler(Sensor sensor);

    public class SerialMySensorsController
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

        public SerialMySensorsController(SerialPort serialPort)
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
            serialPort.SendMessage(message);
        }



        private void RecieveSerialMessage(string message)
        {

            Message mes = ParseMessageFromString(message);

            if (enableLogging)
                messagesLog.AddNewMessage(mes);

            if (OnMessageRecievedEvent != null)
                OnMessageRecievedEvent(mes);

            UpdateNodeFromMessage(mes);
            UpdateSensorFromMessage(mes);
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
            Sensor sensor = GetSensor(node, mes.sensorId);

            if (sensor == null)
            {
                sensor = new Sensor(mes.sensorId, node);
                sensor.state = mes.payload;

                if (mes.messageType==MessageType.C_SET)
                    sensor.dataType = (SensorDataType)mes.subType;

                node.sensors.Add(sensor);

                if (OnNewSensorEvent != null)
                    OnNewSensorEvent(sensor);
            }
            else
            {
                sensor.state = mes.payload;

                if (OnSensorUpdatedEvent != null)
                    OnSensorUpdatedEvent(sensor);
            }
        }


        public Node GetNode(int id)
        {
            Node node = nodes.FirstOrDefault(x => x.nodeId == id);
            return node;
        }

        public Sensor GetSensor(int nodeId,int sensorId)
        {
            Node node = GetNode(nodeId);
            Sensor sensor = GetSensor(node,sensorId);
            return sensor;
        }

        public Sensor GetSensor(Node node, int sensorId)
        {
            Sensor sensor = node.sensors.FirstOrDefault(x => x.sensorId == sensorId);
            return sensor;
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


    }
}
