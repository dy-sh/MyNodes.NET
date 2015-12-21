/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace MyNetSensors.Gateway
{
    public class Node
    {

        public int db_Id { get; set; }


        public int nodeId { get; set; }
        public DateTime registered { get; set; }
        public DateTime lastSeen { get; set; }
        public bool? isRepeatingNode { get; set; }
        public string name { get; set; }
        public string version { get; set; }
        public int? batteryLevel { get; set; }

        public List<Sensor> sensors { get; set; }




        public Node()
        {
            sensors = new List<Sensor>();
        }

        public Node(int nodeId)
        {
            this.nodeId = nodeId;
            registered = DateTime.Now;
            lastSeen = DateTime.Now;
            sensors=new List<Sensor>();
        }

        public void UpdateLastSeenNow()
        {
            lastSeen = DateTime.Now;
        }

        public override string ToString()
        {
            string s = $"Node ID {nodeId}\r\n";


            if (!String.IsNullOrEmpty(name))
                s += $"Name: {name}\r\n";

            if (!String.IsNullOrEmpty(version))
                s += $"Version: {version}\r\n";

            s += $"Registered {registered}\r\n";
            s += $"Last seen {lastSeen}\r\n";

            if (isRepeatingNode == null)
                s += "Repeating: unknown\r\n";
            else if (isRepeatingNode.Value)
                s += "Repeating: Yes\r\n";
            else
                s += "Repeating: No\r\n";

            if (batteryLevel != null)
                s += $"Battery: {batteryLevel.Value} %\r\n";

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


        public Sensor GetSensor(int sensorId)
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

        public string GetSimpleName1()
        {
            if (!String.IsNullOrEmpty(name))
                return name;

            return "Node "+nodeId;
        }

        public string GetSimpleName2()
        {
            if (!String.IsNullOrEmpty(name))
                return $"Node {nodeId} ({name})";

            return "Node " + nodeId;
        }
    }
}
