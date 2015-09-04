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
            s += String.Format("First seen {0}\r\n", firstSeen);
            s += String.Format("Last seen {0}\r\n", lastSeen);
            if (sensors.Any())
            {
                s += "Sensors:\r\n";
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
    }
}
