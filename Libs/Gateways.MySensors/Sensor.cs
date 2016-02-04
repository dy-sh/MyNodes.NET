/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;

namespace MyNetSensors.Gateways.MySensors
{
    public class Sensor
    {

        public int Id { get; set; }


        public int nodeId { get; set; }
        public int sensorId { get; set; }
        public string description { get; set; }
        public SensorType? type { get; set; }
        public SensorDataType? dataType { get; set; }
        public string state { get; set; }


        public Sensor()
        {

        }

        public Sensor(int sensorId, Node ownerNode)
        {
            this.sensorId = sensorId;
            this.nodeId = ownerNode.Id;
        }

        public override string ToString()
        {
            string s = $"Sensor ID: {sensorId}\r\n";

            if (type != null)
                s += $"Type: {type.ToString()}\r\n";
            else
                s += "Type: unknown\r\n";

            if (description != null)
                s += $"Description: {description}\r\n";

            if (type != null)
                s += $"Data Type: {dataType.ToString()}\r\n";
            else
                s += "Data Type: unknown\r\n";

            if (state != null)
                s += $"State: {state.ToString()}\r\n";
            else
                s += "State: unknown\r\n";

            return s;
        }




        public void SetDefaultDataType()
        {
            switch (type)
            {
                case SensorType.S_DOOR:
                    dataType = SensorDataType.V_TRIPPED;
                    break;
                case SensorType.S_MOTION:
                    dataType = SensorDataType.V_TRIPPED;
                    break;
                case SensorType.S_SMOKE:
                    dataType = SensorDataType.V_TRIPPED;
                    break;
                case SensorType.S_BINARY:
                    dataType = SensorDataType.V_STATUS;
                    break;
                case SensorType.S_DIMMER:
                    dataType = SensorDataType.V_DIMMER;
                    break;
                case SensorType.S_COVER:
                    dataType = SensorDataType.V_PERCENTAGE;
                    break;
                case SensorType.S_TEMP:
                    dataType = SensorDataType.V_TEMP;
                    break;
                case SensorType.S_HUM:
                    dataType = SensorDataType.V_HUM;
                    break;
                case SensorType.S_BARO:
                    dataType = SensorDataType.V_PRESSURE;
                    break;
                case SensorType.S_WIND:
                    dataType = SensorDataType.V_WIND;
                    break;
                case SensorType.S_RAIN:
                    dataType = SensorDataType.V_RAIN;
                    break;
                case SensorType.S_UV:
                    dataType = SensorDataType.V_UV;
                    break;
                case SensorType.S_WEIGHT:
                    dataType = SensorDataType.V_WEIGHT;
                    break;
                case SensorType.S_POWER:
                    dataType = SensorDataType.V_WATT;
                    break;
                case SensorType.S_HEATER:
                    dataType = SensorDataType.V_TEMP;
                    break;
                case SensorType.S_DISTANCE:
                    dataType = SensorDataType.V_DISTANCE;
                    break;
                case SensorType.S_LIGHT_LEVEL:
                    dataType = SensorDataType.V_LIGHT_LEVEL;
                    break;
                case SensorType.S_ARDUINO_NODE:
                    dataType = null;
                    break;
                case SensorType.S_ARDUINO_REPEATER_NODE:
                    dataType = null;
                    break;
                case SensorType.S_LOCK:
                    dataType = SensorDataType.V_LOCK_STATUS;
                    break;
                case SensorType.S_IR:
                    dataType = SensorDataType.V_IR_SEND;
                    break;
                case SensorType.S_WATER:
                    dataType = SensorDataType.V_VOLUME;
                    break;
                case SensorType.S_AIR_QUALITY:
                    dataType = SensorDataType.V_LEVEL;
                    break;
                case SensorType.S_CUSTOM:
                    dataType = null;
                    break;
                case SensorType.S_DUST:
                    dataType = SensorDataType.V_LEVEL;
                    break;
                case SensorType.S_SCENE_CONTROLLER:
                    dataType = SensorDataType.V_SCENE_ON;
                    break;
                case SensorType.S_RGB_LIGHT:
                    dataType = SensorDataType.V_RGB;
                    break;
                case SensorType.S_RGBW_LIGHT:
                    dataType = SensorDataType.V_RGBW;
                    break;
                case SensorType.S_COLOR_SENSOR:
                    dataType = SensorDataType.V_RGB;
                    break;
                case SensorType.S_HVAC:
                    dataType = SensorDataType.V_HVAC_SETPOINT_HEAT;
                    break;
                case SensorType.S_MULTIMETER:
                    dataType = SensorDataType.V_VOLTAGE;
                    break;
                case SensorType.S_SPRINKLER:
                    dataType = SensorDataType.V_STATUS;
                    break;
                case SensorType.S_WATER_LEAK:
                    dataType = SensorDataType.V_TRIPPED;
                    break;
                case SensorType.S_SOUND:
                    dataType = SensorDataType.V_LEVEL;
                    break;
                case SensorType.S_VIBRATION:
                    dataType = SensorDataType.V_LEVEL;
                    break;
                case SensorType.S_MOISTURE:
                    dataType = SensorDataType.V_LEVEL;
                    break;
            }
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

        public string GetSimpleName1()
        {
            if (!String.IsNullOrEmpty(description))
                return description;

            return $"Sensor{sensorId}";
        }

        public string GetSimpleName2()
        {
            if (!String.IsNullOrEmpty(description))
                return $"{sensorId} [{description}]";

            return $"{sensorId}";
        }

        public string GetSimpleName3()
        {
            if (!String.IsNullOrEmpty(description))
                return $"[{description}] {sensorId} ";

            if (type!=null)
                return $"[{type}] {sensorId}";

            if (dataType != null)
                return $"[{dataType}] {sensorId}";

            return $"{sensorId}";
        }
    }
}
