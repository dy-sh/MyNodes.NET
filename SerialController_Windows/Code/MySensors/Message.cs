using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialController_Windows.Code
{
    public class Message
    {
        public int nodeId;
        public int sensorId;
        public MessageType messageType;
        public bool ack;
        public int subType;
        public string payload;
        public bool isValid;
        public DateTime dateTime;

        public override string ToString()
        {
            string s;

            if (isValid)
            {



                s = string.Format("{0}: {1}; {2}; {3}; {4}; {5}; {6}",
                    dateTime,
                    nodeId,
                    sensorId,
                    messageType,
                    ack,
                    GetDecodedSubType(),
                    payload
                    );
            }
            else
                s = string.Format("{0}: {1}",
                    dateTime,
                    payload);

            return s;
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
