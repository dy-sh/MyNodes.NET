/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialController_Windows.Code
{
    public enum MessageType
    {
        C_PRESENTATION = 0,
        C_SET = 1,
        C_REQ = 2,
        C_INTERNAL = 3,
        C_STREAM = 4 // For Firmware and other larger chunks of data that need to be divided into pieces.
    };

    public enum SensorType
    {
        S_DOOR, // Door sensor, V_TRIPPED, V_ARMED
        S_MOTION, // Motion sensor, V_TRIPPED, V_ARMED 
        S_SMOKE, // Smoke sensor, V_TRIPPED, V_ARMED
        S_LIGHT, // Binary light or relay, V_STATUS (or V_LIGHT), V_WATT
        S_BINARY = 3, // Binary light or relay, V_STATUS (or V_LIGHT), V_WATT (same as S_LIGHT)
        S_DIMMER, // Dimmable light or fan device, V_STATUS (on/off), V_DIMMER (dimmer level 0-100), V_WATT
        S_COVER, // Blinds or window cover, V_UP, V_DOWN, V_STOP, V_DIMMER (open/close to a percentage)
        S_TEMP, // Temperature sensor, V_TEMP
        S_HUM, // Humidity sensor, V_HUM
        S_BARO, // Barometer sensor, V_PRESSURE, V_FORECAST
        S_WIND, // Wind sensor, V_WIND, V_GUST
        S_RAIN, // Rain sensor, V_RAIN, V_RAINRATE
        S_UV, // Uv sensor, V_UV
        S_WEIGHT, // Personal scale sensor, V_WEIGHT, V_IMPEDANCE
        S_POWER, // Power meter, V_WATT, V_KWH
        S_HEATER, // Header device, V_HVAC_SETPOINT_HEAT, V_HVAC_FLOW_STATE, V_TEMP
        S_DISTANCE, // Distance sensor, V_DISTANCE
        S_LIGHT_LEVEL,
        // Light level sensor, V_LIGHT_LEVEL (uncalibrated in percentage),  V_LEVEL (light level in lux)
        S_ARDUINO_NODE, // Used (internally) for presenting a non-repeating Arduino node
        S_ARDUINO_REPEATER_NODE, // Used (internally) for presenting a repeating Arduino node 
        S_LOCK, // Lock device, V_LOCK_STATUS
        S_IR, // Ir device, V_IR_SEND, V_IR_RECEIVE
        S_WATER, // Water meter, V_FLOW, V_VOLUME
        S_AIR_QUALITY, // Air quality sensor, V_LEVEL
        S_CUSTOM, // Custom sensor 
        S_DUST, // Dust sensor, V_LEVEL
        S_SCENE_CONTROLLER, // Scene controller device, V_SCENE_ON, V_SCENE_OFF. 
        S_RGB_LIGHT, // RGB light. Send color component data using V_RGB. Also supports V_WATT 
        S_RGBW_LIGHT, // RGB light with an additional White component. Send data using V_RGBW. Also supports V_WATT
        S_COLOR_SENSOR, // Color sensor, send color information using V_RGB
        S_HVAC,
        // Thermostat/HVAC device. V_HVAC_SETPOINT_HEAT, V_HVAC_SETPOINT_COLD, V_HVAC_FLOW_STATE, V_HVAC_FLOW_MODE, V_TEMP
        S_MULTIMETER, // Multimeter device, V_VOLTAGE, V_CURRENT, V_IMPEDANCE 
        S_SPRINKLER, // Sprinkler, V_STATUS (turn on/off), V_TRIPPED (if fire detecting device)
        S_WATER_LEAK, // Water leak sensor, V_TRIPPED, V_ARMED
        S_SOUND, // Sound sensor, V_TRIPPED, V_ARMED, V_LEVEL (sound level in dB)
        S_VIBRATION, // Vibration sensor, V_TRIPPED, V_ARMED, V_LEVEL (vibration in Hz)
        S_MOISTURE, // Moisture sensor, V_TRIPPED, V_ARMED, V_LEVEL (water content or moisture in percentage?) 
    };

    public enum SensorDataType
    {
        V_TEMP, // S_TEMP. Temperature S_TEMP, S_HEATER, S_HVAC
        V_HUM, // S_HUM. Humidity
        V_STATUS,
        //  S_LIGHT, S_DIMMER, S_SPRINKLER, S_HVAC, S_HEATER. Used for setting/reporting binary (on/off) status. 1=on, 0=off  
        V_LIGHT = 2, // Same as V_STATUS
        V_PERCENTAGE, // S_DIMMER. Used for sending a percentage value 0-100 (%). 
        V_DIMMER = 3, // S_DIMMER. Same as V_PERCENTAGE.  
        V_PRESSURE, // S_BARO. Atmospheric Pressure
        V_FORECAST,
        // S_BARO. Whether forecast. string of "stable", "sunny", "cloudy", "unstable", "thunderstorm" or "unknown"
        V_RAIN, // S_RAIN. Amount of rain
        V_RAINRATE, // S_RAIN. Rate of rain
        V_WIND, // S_WIND. Wind speed
        V_GUST, // S_WIND. Gust
        V_DIRECTION, // S_WIND. Wind direction 0-360 (degrees)
        V_UV, // S_UV. UV light level
        V_WEIGHT, // S_WEIGHT. Weight(for scales etc)
        V_DISTANCE, // S_DISTANCE. Distance
        V_IMPEDANCE, // S_MULTIMETER, S_WEIGHT. Impedance value
        V_ARMED,
        // S_DOOR, S_MOTION, S_SMOKE, S_SPRINKLER. Armed status of a security sensor. 1 = Armed, 0 = Bypassed
        V_TRIPPED,
        // S_DOOR, S_MOTION, S_SMOKE, S_SPRINKLER, S_WATER_LEAK, S_SOUND, S_VIBRATION, S_MOISTURE. Tripped status of a security sensor. 1 = Tripped, 0
        V_WATT, // S_POWER, S_LIGHT, S_DIMMER, S_RGB, S_RGBW. Watt value for power meters
        V_KWH, // S_POWER. Accumulated number of KWH for a power meter
        V_SCENE_ON, // S_SCENE_CONTROLLER. Turn on a scene
        V_SCENE_OFF, // S_SCENE_CONTROLLER. Turn of a scene
        V_HEATER, // Deprecated. Use V_HVAC_FLOW_STATE instead.
        V_HVAC_FLOW_STATE = 21,
        // S_HEATER, S_HVAC. HVAC flow state ("Off", "HeatOn", "CoolOn", or "AutoChangeOver") 
        V_HVAC_SPEED, // S_HVAC, S_HEATER. HVAC/Heater fan speed ("Min", "Normal", "Max", "Auto") 
        V_LIGHT_LEVEL, // S_LIGHT_LEVEL. Uncalibrated light level. 0-100%. Use V_LEVEL for light level in lux
        V_VAR1,
        V_VAR2,
        V_VAR3,
        V_VAR4,
        V_VAR5,
        V_UP, // S_COVER. Window covering. Up
        V_DOWN, // S_COVER. Window covering. Down
        V_STOP, // S_COVER. Window covering. Stop
        V_IR_SEND, // S_IR. Send out an IR-command
        V_IR_RECEIVE, // S_IR. This message contains a received IR-command
        V_FLOW, // S_WATER. Flow of water (in meter)
        V_VOLUME, // S_WATER. Water volume
        V_LOCK_STATUS, // S_LOCK. Set or get lock status. 1=Locked, 0=Unlocked
        V_LEVEL, // S_DUST, S_AIR_QUALITY, S_SOUND (dB), S_VIBRATION (hz), S_LIGHT_LEVEL (lux)
        V_VOLTAGE, // S_MULTIMETER 
        V_CURRENT, // S_MULTIMETER
        V_RGB, // S_RGB_LIGHT, S_COLOR_SENSOR. 
        // Used for sending color information for multi color LED lighting or color sensors. 
        // Sent as ASCII hex: RRGGBB (RR=red, GG=green, BB=blue component)
        V_RGBW, // S_RGBW_LIGHT
        // Used for sending color information to multi color LED lighting. 
        // Sent as ASCII hex: RRGGBBWW (WW=white component)
        V_ID, // S_TEMP
        // Used for sending in sensors hardware ids (i.e. OneWire DS1820b). 
        V_UNIT_PREFIX, // S_DUST, S_AIR_QUALITY
        // Allows sensors to send in a string representing the 
        // unit prefix to be displayed in GUI, not parsed by controller! E.g. cm, m, km, inch.
        // Can be used for S_DISTANCE or gas concentration
        V_HVAC_SETPOINT_COOL, // S_HVAC. HVAC cool setpoint (Integer between 0-100)
        V_HVAC_SETPOINT_HEAT, // S_HEATER, S_HVAC. HVAC/Heater setpoint (Integer between 0-100)
        V_HVAC_FLOW_MODE, // S_HVAC. Flow mode for HVAC ("Auto", "ContinuousOn", "PeriodicOn")

    };


    public enum InternalDataType
    {
        I_BATTERY_LEVEL,
        I_TIME,
        I_VERSION,
        I_ID_REQUEST,
        I_ID_RESPONSE,
        I_INCLUSION_MODE,
        I_CONFIG,
        I_FIND_PARENT,
        I_FIND_PARENT_RESPONSE,
        I_LOG_MESSAGE,
        I_CHILDREN,
        I_SKETCH_NAME,
        I_SKETCH_VERSION,
        I_REBOOT,
        I_GATEWAY_READY,
        I_REQUEST_SIGNING,
        I_GET_NONCE,
        I_GET_NONCE_RESPONSE
    }


    public static class MySensors
    {
        public static string GetSimpleSensorType(SensorType? sensorType)
        {
            switch (sensorType)
            {
                case SensorType.S_DOOR:
                    return "Door";
                case SensorType.S_MOTION:
                    return "Motion";
                case SensorType.S_SMOKE:
                    return "Smoke";
                case SensorType.S_LIGHT:
                    return "Light";
                case SensorType.S_DIMMER:
                    return "Dimmer";
                case SensorType.S_COVER:
                    return "Cover";
                case SensorType.S_TEMP:
                    return "Temp";
                case SensorType.S_HUM:
                    return "Humidity";
                case SensorType.S_BARO:
                    return "Baro";
                case SensorType.S_WIND:
                    return "Wind";
                case SensorType.S_RAIN:
                    return "Rain";
                case SensorType.S_UV:
                    return "UV";
                case SensorType.S_WEIGHT:
                    return "Weight";
                case SensorType.S_POWER:
                    return "Power";
                case SensorType.S_HEATER:
                    return "Heater";
                case SensorType.S_DISTANCE:
                    return "Distance";
                case SensorType.S_LIGHT_LEVEL:
                    return "Light Level";
                case SensorType.S_ARDUINO_NODE:
                    return "Simple node";
                case SensorType.S_ARDUINO_REPEATER_NODE:
                    return "Repeater node";
                case SensorType.S_LOCK:
                    return "Lock";
                case SensorType.S_IR:
                    return "IR";
                case SensorType.S_WATER:
                    return "Water";
                case SensorType.S_AIR_QUALITY:
                    return "Air";
                case SensorType.S_CUSTOM:
                    return "Custom";
                case SensorType.S_DUST:
                    return "Dust";
                case SensorType.S_SCENE_CONTROLLER:
                    return "Scene controller";
                case SensorType.S_RGB_LIGHT:
                    return "RGB Light";
                case SensorType.S_RGBW_LIGHT:
                    return "RGMW Light";
                case SensorType.S_COLOR_SENSOR:
                    return "Color";
                case SensorType.S_HVAC:
                    return "HRAC";
                case SensorType.S_MULTIMETER:
                    return "Multimiter";
                case SensorType.S_SPRINKLER:
                    return "Sprinkler";
                case SensorType.S_WATER_LEAK:
                    return "Water leak";
                case SensorType.S_SOUND:
                    return "Sound";
                case SensorType.S_VIBRATION:
                    return "Vibration";
                case SensorType.S_MOISTURE:
                    return "Moisture";
            }

            return "Unknown";
        }
    }
}