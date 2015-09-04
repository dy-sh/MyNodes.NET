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
        public List<SensorData> sensorData = new List<SensorData>();

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


            if (sensorData.Any())
                foreach (var data in sensorData)
                    s += data.ToString();

            return s;
        }


        public SensorData GetData(SensorDataType dataType)
        {
            SensorData data = sensorData.FirstOrDefault(x => x.dataType == dataType);
            return data;
        }

        public string GetState(SensorDataType dataType)
        {
            SensorData data = GetData(dataType);
            return data.state;
        }
    }
}
