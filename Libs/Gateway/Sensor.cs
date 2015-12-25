/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using MyNetSensors.Utils;


namespace MyNetSensors.Gateway
{
    public class Sensor
    {

        public int db_Id { get; set; }


        public int nodeId { get; set; }
        public int sensorId { get; set; }
        public string description { get; set; }
        public SensorType? type { get; set; }
        public SensorDataType? dataType { get; set; }
        public string state { get; set; }



        public bool storeHistoryEnabled { get; set; }
        //interval in seconds. if 0, will not store by timer
        public int storeHistoryWithInterval { get; set; }
        public bool storeHistoryEveryChange { get; set; }
        public DateTime storeHistoryLastDate { get; set; }


        public bool invertData { get; set; }
        public bool remapEnabled { get; set; }
        public string remapFromMin { get; set; }
        public string remapFromMax { get; set; }
        public string remapToMin { get; set; }
        public string remapToMax { get; set; }

        public Sensor()
        {

        }

        public Sensor(int sensorId, Node ownerNode)
        {
            this.sensorId = sensorId;
            this.nodeId = ownerNode.nodeId;
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




        public void SetSensorType(SensorType? sensorType)
        {
            if (this.type == sensorType) return;
            if (sensorType < 0 || sensorType > Enum.GetValues(typeof (SensorType)).Cast<SensorType>().Max())
            {
                Console.WriteLine("This exception occurs when the serial port does not have time to write the data");
                throw new ArgumentOutOfRangeException("This exception occurs when the serial port does not have time to write the data");
            }


            this.type = sensorType;

            switch (sensorType)
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

      
        public string GetSimpleName1()
        {
            if (description != null)
                return description;
            else
                return MySensors.GetSimpleSensorType(type);
        }

        public string GetSimpleName2()
        {
            return $"Sensor {sensorId} ({GetSimpleName1()})";
        }


        public void RemapSensorData()
        {
            if (!remapEnabled && !invertData)
                return;

            try
            {
                if (IsBinary(dataType))
                {
                    if (invertData)
                    {
                        if (state == "0")
                            state = "1";
                        else state = "0";
                    }
                }

                if (IsPercentage(dataType))
                {
                    int val = Int32.Parse(state);

                    if (remapEnabled)
                    {
                        int fromMin = Int32.Parse(remapFromMin);
                        int fromMax = Int32.Parse(remapFromMax);
                        int toMin = Int32.Parse(remapToMin);
                        int toMax = Int32.Parse(remapToMax);

                        val = MathUtils.Map(val, fromMin, fromMax, toMin, toMax);
                    }

                    val = MathUtils.Clamp(val, 0, 100);

                    if (invertData)
                    {
                        val = 100 - val;
                    }

                    state = val.ToString();
                }

                if (dataType == SensorDataType.V_RGB)
                {
                    int[] val = ColorUtils.ConvertRGBHexStringToIntArray(state);

                    if (remapEnabled)
                    {
                        int[] fromMin = ColorUtils.ConvertRGBHexStringToIntArray(remapFromMin);
                        int[] fromMax = ColorUtils.ConvertRGBHexStringToIntArray(remapFromMax);
                        int[] toMin = ColorUtils.ConvertRGBHexStringToIntArray(remapToMin);
                        int[] toMax = ColorUtils.ConvertRGBHexStringToIntArray(remapToMax);

                        for (int i = 0; i < val.Length; i++)
                        {
                            val[i] = MathUtils.Map(val[i], fromMin[i], fromMax[i], toMin[i], toMax[i]);
                        }
                    }

                    for (int i = 0; i < val.Length; i++)
                    {
                        val[i] = MathUtils.Clamp(val[i], 0, 255);
                    }

                    if (invertData)
                    {
                        for (int i = 0; i < val.Length; i++)
                        {
                            val[i] = 255 - val[i];
                        }
                    }

                    state = ColorUtils.ConvertRGBIntArrayToHexString(val);

                }


                if (dataType == SensorDataType.V_RGBW)
                {
                    int[] val = ColorUtils.ConvertRGBWHexStringToIntArray(state);

                    if (remapEnabled)
                    {
                        int[] fromMin = ColorUtils.ConvertRGBWHexStringToIntArray(remapFromMin);
                        int[] fromMax = ColorUtils.ConvertRGBWHexStringToIntArray(remapFromMax);
                        int[] toMin = ColorUtils.ConvertRGBWHexStringToIntArray(remapToMin);
                        int[] toMax = ColorUtils.ConvertRGBWHexStringToIntArray(remapToMax);

                        for (int i = 0; i < val.Length; i++)
                        {
                            val[i] = MathUtils.Map(val[i], fromMin[i], fromMax[i], toMin[i], toMax[i]);
                        }
                    }

                    for (int i = 0; i < val.Length; i++)
                    {
                        val[i] = MathUtils.Clamp(val[i], 0, 255);
                    }

                    if (invertData)
                    {
                        for (int i = 0; i < val.Length; i++)
                        {
                            val[i] = 255 - val[i];
                        }
                    }

                    state = ColorUtils.ConvertRGBWIntArrayToHexString(val);

                }
            }
            catch
            {
                Console.WriteLine($"Can't remap data from Node{nodeId} Sensor{sensorId}");
            }
        }

        public void UnRemapSensorData()
        {

            try
            {
                if (IsBinary(dataType))
                {
                    if (invertData)
                    {
                        if (state == "0")
                            state = "1";
                        else state = "0";
                    }
                }

                if (IsPercentage(dataType))
                {
                    int val = Int32.Parse(state);

                    if (remapEnabled)
                    {
                        int fromMin = Int32.Parse(remapToMin);
                        int fromMax = Int32.Parse(remapToMax);
                        int toMin = Int32.Parse(remapFromMin);
                        int toMax = Int32.Parse(remapFromMax);

                        val = MathUtils.Map(val, fromMin, fromMax, toMin, toMax);
                    }

                    val = MathUtils.Clamp(val, 0, 100);

                    if (invertData)
                    {
                        val = 100 - val;
                    }

                    state = val.ToString();
                }

                if (dataType == SensorDataType.V_RGB)
                {
                    int[] val = ColorUtils.ConvertRGBHexStringToIntArray(state);

                    if (remapEnabled)
                    {
                        int[] fromMin = ColorUtils.ConvertRGBHexStringToIntArray(remapToMin);
                        int[] fromMax = ColorUtils.ConvertRGBHexStringToIntArray(remapToMax);
                        int[] toMin = ColorUtils.ConvertRGBHexStringToIntArray(remapFromMin);
                        int[] toMax = ColorUtils.ConvertRGBHexStringToIntArray(remapFromMax);

                        for (int i = 0; i < val.Length; i++)
                        {
                            val[i] = MathUtils.Map(val[i], fromMin[i], fromMax[i], toMin[i], toMax[i]);
                        }
                    }

                    for (int i = 0; i < val.Length; i++)
                    {
                        val[i] = MathUtils.Clamp(val[i], 0, 255);
                    }

                    if (invertData)
                    {
                        for (int i = 0; i < val.Length; i++)
                        {
                            val[i] = 255 - val[i];
                        }
                    }

                    state = ColorUtils.ConvertRGBIntArrayToHexString(val);

                }


                if (dataType == SensorDataType.V_RGBW)
                {
                    int[] val = ColorUtils.ConvertRGBWHexStringToIntArray(state);

                    if (remapEnabled)
                    {
                        int[] fromMin = ColorUtils.ConvertRGBWHexStringToIntArray(remapToMin);
                        int[] fromMax = ColorUtils.ConvertRGBWHexStringToIntArray(remapToMax);
                        int[] toMin = ColorUtils.ConvertRGBWHexStringToIntArray(remapFromMin);
                        int[] toMax = ColorUtils.ConvertRGBWHexStringToIntArray(remapFromMax);

                        for (int i = 0; i < val.Length; i++)
                        {
                            val[i] = MathUtils.Map(val[i], fromMin[i], fromMax[i], toMin[i], toMax[i]);
                        }
                    }

                    for (int i = 0; i < val.Length; i++)
                    {
                        val[i] = MathUtils.Clamp(val[i], 0, 255);
                    }

                    if (invertData)
                    {
                        for (int i = 0; i < val.Length; i++)
                        {
                            val[i] = 255 - val[i];
                        }
                    }

                    state = ColorUtils.ConvertRGBWIntArrayToHexString(val);

                }
            }
            catch
            {
                Console.WriteLine($"Can't remap data from Node{nodeId} Sensor{sensorId}");
            }
        }

        public string ConvertSensorData(SensorDataType? newDataType)
        {

            //convert binary to percentage 
            if (IsBinary(this.dataType)
                && IsPercentage(newDataType))
            {
                if (this.state == "0")
                    return ("0");
                else
                    return ("100");
            }

            //convert  percentage to binary
            if (IsBinary(newDataType)
                && IsPercentage(this.dataType))
            {
                if (this.state == "0")
                    return ("0");
                else
                    return ("1");
            }

            return this.state;
        }

        public bool IsBinary(SensorDataType? dataType)
        {
            return (dataType == SensorDataType.V_STATUS ||
                    dataType == SensorDataType.V_LIGHT ||
                    dataType == SensorDataType.V_ARMED ||
                    dataType == SensorDataType.V_TRIPPED ||
                    dataType == SensorDataType.V_LOCK_STATUS);
        }

        public bool IsPercentage(SensorDataType? dataType)
        {
            return (dataType == SensorDataType.V_PERCENTAGE ||
                 dataType == SensorDataType.V_DIMMER ||
                 dataType == SensorDataType.V_LIGHT_LEVEL);
        }
    }
}
