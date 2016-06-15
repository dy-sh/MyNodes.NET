/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace MyNodes.Gateways.MySensors
{
    public class Node
    {
        public int Id { get; set; }

        public DateTime registered { get; set; }
        public DateTime lastSeen { get; set; }
        public bool? isRepeatingNode { get; set; }
        public string name { get; set; }
        public string version { get; set; }
        public int? batteryLevel { get; set; }

        private List<Sensor> sensors = new List<Sensor>();
        public int SensorCount => sensors.Count;
        public IEnumerable<Sensor> Sensors => sensors.AsEnumerable();

        public Node(int id)
        {
            Id = id;
            registered = DateTime.Now;
            lastSeen = DateTime.Now;
        }

        public void UpdateLastSeen(DateTime lastSeen)
        {
            this.lastSeen = lastSeen;
        }

        public override string ToString()
        {
            string s = $"Node ID {Id}\r\n";


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


        public Sensor GetSensor(int sensorId) => sensors.FirstOrDefault(x => x.sensorId == sensorId);

        public Sensor AddSensor(int sensorId)
        {
            if (sensors.Exists(x=>x.Id == sensorId))
                return sensors.First(x => x.sensorId == sensorId);

            var sensor = new Sensor(sensorId, this);
            sensors.Add(sensor);
            return sensor;
        }

        public void AddSensor(Sensor sensor)
        {
            sensors.Add(sensor);
        }

        public string GetSimpleName1()
        {
            if (!String.IsNullOrEmpty(name))
                return name;

            return $"Node{Id}";
        }

        public string GetSimpleName2()
        {
            if (!String.IsNullOrEmpty(name))
                return $"Node{Id} [{name}]";

            return $"Node{Id}";
        }
    }
}
