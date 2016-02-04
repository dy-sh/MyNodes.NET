/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;

namespace MyNetSensors.Gateways.MySensors
{
    public class SensorData:ICloneable
    {
        public int Id { get; set; }
        public int nodeId { get; set; }
        public int sensorId { get; set; }
        public SensorDataType? dataType { get; set; }
        public string state { get; set; }
        public DateTime dateTime { get; set; }


        public SensorData()
        {
            //for DB materialization
        }

        //public SensorData(int nodeId, int sensorId, SensorDataType? dataType, string state)
        //{
        //    this.nodeId = nodeId;
        //    this.sensorId = sensorId;
        //    this.dataType = dataType;
        //    this.state = state;
        //    dateTime = DateTime.Now;
        //}

        public SensorData(Sensor sensor)
        {
            this.nodeId = sensor.nodeId;
            this.sensorId = sensor.sensorId;
            this.dataType = sensor.dataType;
            this.state = sensor.state;
            dateTime = DateTime.Now;
        }

        public override string ToString()
        {
            string s="";

            if (dataType != null)
                s += $"Data type: {dataType.ToString()}, ";
            else
                s += "Data type: unknown, ";

            if (state != null)
                s += $"State: {state}\r\n";
            else
                s += "State: unknown\r\n";

            return s;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public bool IsBinary()
        {
            return (dataType == SensorDataType.V_STATUS ||
                    dataType == SensorDataType.V_LIGHT ||
                    dataType == SensorDataType.V_ARMED ||
                    dataType == SensorDataType.V_TRIPPED ||
                    dataType == SensorDataType.V_LOCK_STATUS);
        }

        public bool IsPercentage()
        {
            return (dataType == SensorDataType.V_PERCENTAGE ||
                 dataType == SensorDataType.V_DIMMER ||
                 dataType == SensorDataType.V_LIGHT_LEVEL);
        }
    }
}