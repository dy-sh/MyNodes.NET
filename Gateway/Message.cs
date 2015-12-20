/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;



namespace MyNetSensors.Gateway
{
    public class Message:ICloneable
    {
        public int db_Id { get; set; }


        public int nodeId { get; set; }
        public int sensorId { get; set; }
        public MessageType messageType { get; set; }
        public bool ack { get; set; }
        public int subType { get; set; }
        public string payload { get; set; }
        public bool isValid { get; set; }
        public bool incoming { get; set; }//or outgoing
        public DateTime dateTime { get; set; }



        public Message()
        {
            dateTime = DateTime.Now;
            isValid = true;
        }

        public override string ToString()
        {
            string s;

            if (isValid)
            {
                s = string.Format("{0}: {1}: {2}; {3}; {4}; {5}; {6}; {7}",
                    dateTime,
                    incoming ? "RX" : "TX",
                    nodeId,
                    sensorId,
                    messageType,
                    ack,
                    GetDecodedSubType(),
                    payload
                    );
            }
            else
                s = string.Format("{0}: {1}: {2}",
                    dateTime,
                    incoming ? "RX" : "TX",
                    payload);

            return s;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public string GetDecodedSubType()
        {
            string subTypeString = subType.ToString();
            if (messageType == MessageType.C_PRESENTATION)
                subTypeString = ((SensorType)subType).ToString();
            else if (messageType == MessageType.C_SET || messageType == MessageType.C_REQ)
                subTypeString = ((SensorDataType)subType).ToString();
            else if (messageType == MessageType.C_INTERNAL)
                subTypeString = ((InternalDataType)subType).ToString();
            return subTypeString;
        }
    }
}
