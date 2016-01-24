/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;

namespace MyNetSensors.Gateways.MySensors.Serial
{
    public class Message : ICloneable
    {
        public int Id { get; set; }


        public int nodeId { get; set; }
        public int sensorId { get; set; }
        public MessageType messageType { get; set; }
        public bool ack { get; set; }
        public int subType { get; set; }
        public string payload { get; set; }
        public bool incoming { get; set; } //or outgoing
        public DateTime dateTime { get; set; }



        public Message()
        {
            dateTime = DateTime.Now;
        }


        //parse message from string
        public Message(string message)
        {
            ParseFromString(message);
        }

        public override string ToString()
        {
            string inc = incoming ? "RX" : "TX";

            return $"{inc}: {nodeId}; {sensorId}; {messageType}; {ack}; {GetDecodedSubType()}; {payload}";
        }


        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public string GetDecodedSubType()
        {
            string subTypeString = subType.ToString();
            if (messageType == MessageType.C_PRESENTATION)
                subTypeString = ((SensorType) subType).ToString();
            else if (messageType == MessageType.C_SET || messageType == MessageType.C_REQ)
                subTypeString = ((SensorDataType) subType).ToString();
            else if (messageType == MessageType.C_INTERNAL)
                subTypeString = ((InternalDataType) subType).ToString();
            return subTypeString;
        }



        private void ParseFromString(string message)
        {
            dateTime = DateTime.Now;

            string[] arguments = message.Split(new char[] {';'}, 6);
            nodeId = Int32.Parse(arguments[0]);
            sensorId = Int32.Parse(arguments[1]);
            messageType = (MessageType) Int32.Parse(arguments[2]);
            ack = arguments[3] == "1";
            subType = Int32.Parse(arguments[4]);
            payload = arguments[5];
        }

        public string ParseToMySensorsMessage()
        {
            return $"{nodeId};{sensorId};{(int) messageType};{(ack ? "1" : "0")};{subType};{payload}\n";
        }
    }
}
