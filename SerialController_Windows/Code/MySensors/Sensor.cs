using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace SerialController_Windows.Code
{
    public class Sensor
    {
        //DB Propertys
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [ForeignKey(typeof(Node))]
        public int NodeId { get; set; }


        public int ownerNodeId { get; set; }
        public int sensorId { get; set; }
        public SensorType? sensorType { get; set; }
        public string description { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
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
                    AddOrUpdateData(SensorDataType.V_STATUS, "0");
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
