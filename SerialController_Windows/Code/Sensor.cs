using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialController_Windows.Code
{
    public class Sensor
    {
        public Node ownerNode;
        public int sensorId;
        public SensorType? sensorType;
        public SensorDataType? dataType;
        public string state;

        public Sensor(int sensorId, Node ownerNode)
        {
            this.sensorId = sensorId;
        }

        public override string ToString()
        {
            string s = String.Format("Sensor ID: {0}\r\n", sensorId);

            if (sensorType != null)
                s += String.Format("Type: {0}\r\n", sensorType.ToString());
            else
                s += String.Format("Type: unknown\r\n");

            if (dataType != null)
                s += String.Format("Data type: {0}\r\n", dataType.ToString());
            else
                s += String.Format("Data type: unknown\r\n");

            if (state != null)
                s += String.Format("State: {0}\r\n", state);
            else
                s += String.Format("State: unknown\r\n");

            


            return s;
        }
    }
}
