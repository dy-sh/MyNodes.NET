/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace SerialController.Windows.Code.MySensors
{
    public class Node
    {

        //DB Propertys
        [PrimaryKey, AutoIncrement]        public int Id { get; set; }


        public int nodeId { get; set; }
        public DateTime registered { get; set; }
        public DateTime lastSeen { get; set; }
        public bool? isRepeatingNode { get; set; }
        public string name { get; set; }
        public string version { get; set; }
        public int? batteryLevel { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
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
            string s = String.Format("Node ID {0}\r\n", nodeId);


            if (!String.IsNullOrEmpty(name))
                s += String.Format("Name: {0}\r\n", name);

            if (!String.IsNullOrEmpty(version))
                s += String.Format("Version: {0}\r\n", version);

            s += String.Format("Registered {0}\r\n", registered);
            s += String.Format("Last seen {0}\r\n", lastSeen);

            if (isRepeatingNode == null)
                s += String.Format("Repeating: unknown\r\n");
            else if (isRepeatingNode.Value)
                s += String.Format("Repeating: Yes\r\n");
            else
                s += String.Format("Repeating: No\r\n");

            if (batteryLevel != null)
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
    }
}
