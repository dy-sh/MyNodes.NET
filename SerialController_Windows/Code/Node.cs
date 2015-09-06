using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialController_Windows.Code
{
    public class Node
    {
        public int nodeId;
        public DateTime firstSeen;
        public DateTime lastSeen;
        public bool? isRepeatingNode =null;
        public string name;
        public string version;
        public int? batteryLevel;
        public List<Sensor> sensors = new List<Sensor>();

        public Node(int nodeId)
        {
            this.nodeId = nodeId;
            firstSeen = DateTime.Now;
            lastSeen = DateTime.Now;
        }

        public void UpdateLastSeenNow()
        {
            lastSeen = DateTime.Now;
        }

        public override string ToString()
        {
            string s = String.Format("Node ID {0}\r\n", nodeId);


            if (!String.IsNullOrEmpty(name))
                s += String.Format("Name: {0}\r\n", name);

            if (!String.IsNullOrEmpty(version))
                s += String.Format("Version: {0}\r\n", version);

            s += String.Format("First seen {0}\r\n", firstSeen);
            s += String.Format("Last seen {0}\r\n", lastSeen);

            if (isRepeatingNode==null)
            s += String.Format("Repeating: unknown\r\n");
            else if (isRepeatingNode.Value)
                s += String.Format("Repeating: Yes\r\n");
            else 
                s += String.Format("Repeating: No\r\n");

            if (batteryLevel!=null)
                s += String.Format("Battery: {0} %\r\n", batteryLevel.Value);

            if (sensors.Any())
            {
                foreach (Sensor sensor in sensors)
                {
                    s += "-------------------\r\n";
                    s += sensor.ToString();
                }
            }

            return s;
        }


        public Sensor GetSensor( int sensorId)
        {
            Sensor sensor = sensors.FirstOrDefault(x => x.sensorId == sensorId);
            return sensor;
        }

        public Sensor AddSensor(int sensorId)
        {
            Sensor sensor = GetSensor(sensorId);
            if (sensor != null) return sensor;

            sensor = new Sensor(sensorId, this);
            sensors.Add(sensor);
            return sensor;
        }
    }
}
