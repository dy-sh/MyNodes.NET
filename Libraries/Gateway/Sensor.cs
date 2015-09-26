/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;


namespace MyNetSensors.Gateway
{
    public class Sensor
    {
        [Key]
        public int db_Id { get; set; }


        public int ownerNodeId { get; set; }
        public int sensorId { get; set; }
        public SensorType? sensorType { get; set; }
        public string description { get; set; }

        public string sensorDataJson { get; set; }

        public bool logToDbEnabled { get; set; }
        //interval in seconds. if 0, will not store by timer
        public int logToDbWithInterval { get; set; }
        public bool logToDbEveryChange { get; set; }


        public Sensor()
        {

        }

        public Sensor(int sensorId, Node ownerNode)
        {
            this.sensorId = sensorId;
            this.ownerNodeId = ownerNode.nodeId;
        }

        public override string ToString()
        {
            string s = String.Format("Sensor ID: {0}\r\n", sensorId);

            if (sensorType != null)
                s += String.Format("Type: {0}\r\n", sensorType.ToString());
            else
                s += String.Format("Type: unknown\r\n");

            if (description != null)
                s += String.Format("Description: {0}\r\n", description);

            List<SensorData> dataList = GetAllData();
            if (dataList.Any())
                foreach (var data in dataList)
                    s += data.ToString();

            return s;
        }

        public List<SensorData> GetAllData()
        {
            if (sensorDataJson == null) return null;
            List<SensorData> dataList = JsonConvert.DeserializeObject<List<SensorData>>(sensorDataJson);
            return dataList;
        }

        public SensorData GetData(SensorDataType dataType)
        {
            List<SensorData> dataList = GetAllData();
            if (dataList == null) return null;
            SensorData data = dataList.FirstOrDefault(x => x.dataType == dataType);
            return data;
        }

        public string GetState(SensorDataType dataType)
        {
            SensorData data = GetData(dataType);
            if (data == null) return null;
            return data.state;
        }


        public void AddOrUpdateData(SensorDataType dataType, string state)
        {
            List<SensorData> dataList = GetAllData();
            if (dataList == null)
                dataList = new List<SensorData>();

            SensorData data = dataList.FirstOrDefault(x => x.dataType == dataType);

            if (data == null)
            {
                data = new SensorData(dataType, state);
                dataList.Add(data);
            }
            else data.state = state;

            sensorDataJson = JsonConvert.SerializeObject(dataList);
        }

        public void AddOrUpdateData(SensorData newData)
        {
            AddOrUpdateData(newData.dataType.Value, newData.state);
        }

        public void SetSensorType(SensorType? sensorType)
        {
            if (this.sensorType == sensorType) return;
            if (sensorType < 0 || sensorType > Enum.GetValues(typeof(SensorType)).Cast<SensorType>().Max())
            { throw new ArgumentOutOfRangeException("This exception occurs when the serial port does not have time to write the data"); }


            this.sensorType = sensorType;

            switch (sensorType)
            {
                case SensorType.S_DOOR:
                    AddOrUpdateData(SensorDataType.V_TRIPPED, "0");
                    //AddOrUpdateData(SensorDataType.V_ARMED, "0");
                    break;
                case SensorType.S_MOTION:
                    break;
                case SensorType.S_SMOKE:
                    break;
                case SensorType.S_LIGHT:
                    AddOrUpdateData(SensorDataType.V_STATUS, "0");
                    //AddOrUpdateData(SensorDataType.V_WATT, "0");
                    break;
                case SensorType.S_DIMMER:
                    //AddOrUpdateData(SensorDataType.V_STATUS, "0");
                    AddOrUpdateData(SensorDataType.V_DIMMER, "0");
                    //AddOrUpdateData(SensorDataType.V_WATT, "0");
                    break;
                case SensorType.S_COVER:
                    break;
                case SensorType.S_TEMP:
                    break;
                case SensorType.S_HUM:
                    break;
                case SensorType.S_BARO:
                    break;
                case SensorType.S_WIND:
                    break;
                case SensorType.S_RAIN:
                    break;
                case SensorType.S_UV:
                    break;
                case SensorType.S_WEIGHT:
                    break;
                case SensorType.S_POWER:
                    break;
                case SensorType.S_HEATER:
                    break;
                case SensorType.S_DISTANCE:
                    break;
                case SensorType.S_LIGHT_LEVEL:
                    break;
                case SensorType.S_ARDUINO_NODE:
                    break;
                case SensorType.S_ARDUINO_REPEATER_NODE:
                    break;
                case SensorType.S_LOCK:
                    break;
                case SensorType.S_IR:
                    AddOrUpdateData(SensorDataType.V_IR_SEND, "");
                    break;
                case SensorType.S_WATER:
                    break;
                case SensorType.S_AIR_QUALITY:
                    break;
                case SensorType.S_CUSTOM:
                    break;
                case SensorType.S_DUST:
                    break;
                case SensorType.S_SCENE_CONTROLLER:
                    break;
                case SensorType.S_RGB_LIGHT:
                    AddOrUpdateData(SensorDataType.V_RGB, "000000");
                    //AddOrUpdateData(SensorDataType.V_WATT, "0");
                    break;
                case SensorType.S_RGBW_LIGHT:
                    AddOrUpdateData(SensorDataType.V_RGBW, "00000000");
                    //AddOrUpdateData(SensorDataType.V_WATT, "0");
                    break;
                case SensorType.S_COLOR_SENSOR:
                    break;
                case SensorType.S_HVAC:
                    break;
                case SensorType.S_MULTIMETER:
                    break;
                case SensorType.S_SPRINKLER:
                    break;
                case SensorType.S_WATER_LEAK:
                    break;
                case SensorType.S_SOUND:
                    break;
                case SensorType.S_VIBRATION:
                    break;
                case SensorType.S_MOISTURE:
                    break;
            }
        }

        public SensorType? GetSensorType()
        {
            return sensorType;
        }

        public string GetDescrirtionOrType()
        {
            if (description != null)
                return description;
            else
                return MySensors.GetSimpleSensorType(sensorType);
        }
    }
}
