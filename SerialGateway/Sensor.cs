/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MyNetSensors.SerialGateway
{
    public class Sensor
    {
        [Key]
        public int db_Id { get; set; }


        public int ownerNodeId { get; set; }
        public int sensorId { get; set; }
        public SensorType? sensorType { get; set; }
        public string description { get; set; }

        public List<SensorData> sensorData { get; set; }


        public Sensor()
        {
            sensorData = new List<SensorData>();
        }

        public Sensor(int sensorId, Node ownerNode)
        {
            this.sensorId = sensorId;
            this.ownerNodeId = ownerNode.nodeId;
            sensorData = new List<SensorData>();
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


        public void AddOrUpdateData(SensorDataType dataType, string state)
        {
            SensorData data = GetData(dataType);
            if (data == null)
            {
                data = new SensorData(dataType, state);
                sensorData.Add(data);
            }
            else data.state = state;
        }

        public void AddOrUpdateData(SensorData newData)
        {
            AddOrUpdateData(newData.dataType.Value, newData.state);
        }

        public void SetSensorType(SensorType? sensorType)
        {
            if (this.sensorType==sensorType) return;

            this.sensorType = sensorType;

            switch (sensorType)
            {
                case SerialGateway.SensorType.S_DOOR:
                    AddOrUpdateData(SensorDataType.V_TRIPPED, "0");
                    //AddOrUpdateData(SensorDataType.V_ARMED, "0");
                    break;
                case SerialGateway.SensorType.S_MOTION:
                    break;
                case SerialGateway.SensorType.S_SMOKE:
                    break;
                case SerialGateway.SensorType.S_LIGHT:
                    AddOrUpdateData(SensorDataType.V_STATUS, "0");
                    //AddOrUpdateData(SensorDataType.V_WATT, "0");
                    break;
                case SerialGateway.SensorType.S_DIMMER:
                    AddOrUpdateData(SensorDataType.V_STATUS, "0");
                    AddOrUpdateData(SensorDataType.V_DIMMER, "0");
                    //AddOrUpdateData(SensorDataType.V_WATT, "0");
                    break;
                case SerialGateway.SensorType.S_COVER:
                    break;
                case SerialGateway.SensorType.S_TEMP:
                    break;
                case SerialGateway.SensorType.S_HUM:
                    break;
                case SerialGateway.SensorType.S_BARO:
                    break;
                case SerialGateway.SensorType.S_WIND:
                    break;
                case SerialGateway.SensorType.S_RAIN:
                    break;
                case SerialGateway.SensorType.S_UV:
                    break;
                case SerialGateway.SensorType.S_WEIGHT:
                    break;
                case SerialGateway.SensorType.S_POWER:
                    break;
                case SerialGateway.SensorType.S_HEATER:
                    break;
                case SerialGateway.SensorType.S_DISTANCE:
                    break;
                case SerialGateway.SensorType.S_LIGHT_LEVEL:
                    break;
                case SerialGateway.SensorType.S_ARDUINO_NODE:
                    break;
                case SerialGateway.SensorType.S_ARDUINO_REPEATER_NODE:
                    break;
                case SerialGateway.SensorType.S_LOCK:
                    break;
                case SerialGateway.SensorType.S_IR:
                    AddOrUpdateData(SensorDataType.V_IR_SEND, "");
                    break;
                case SerialGateway.SensorType.S_WATER:
                    break;
                case SerialGateway.SensorType.S_AIR_QUALITY:
                    break;
                case SerialGateway.SensorType.S_CUSTOM:
                    break;
                case SerialGateway.SensorType.S_DUST:
                    break;
                case SerialGateway.SensorType.S_SCENE_CONTROLLER:
                    break;
                case SerialGateway.SensorType.S_RGB_LIGHT:
                    AddOrUpdateData(SensorDataType.V_RGB, "000000");
                    //AddOrUpdateData(SensorDataType.V_WATT, "0");
                    break;
                case SerialGateway.SensorType.S_RGBW_LIGHT:
                    AddOrUpdateData(SensorDataType.V_RGBW, "00000000");
                    //AddOrUpdateData(SensorDataType.V_WATT, "0");
                    break;
                case SerialGateway.SensorType.S_COLOR_SENSOR:
                    break;
                case SerialGateway.SensorType.S_HVAC:
                    break;
                case SerialGateway.SensorType.S_MULTIMETER:
                    break;
                case SerialGateway.SensorType.S_SPRINKLER:
                    break;
                case SerialGateway.SensorType.S_WATER_LEAK:
                    break;
                case SerialGateway.SensorType.S_SOUND:
                    break;
                case SerialGateway.SensorType.S_VIBRATION:
                    break;
                case SerialGateway.SensorType.S_MOISTURE:
                    break;
                case null:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sensorType), sensorType, null);
            }
        }

        public SensorType? GetSensorType()
        {
            return sensorType;
        }
    }
}
