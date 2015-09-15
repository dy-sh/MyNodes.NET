var mySensors = {

    messageType:
    {
        C_PRESENTATION: 0,
        C_SET: 1,
        C_REQ: 2,
        C_INTERNAL: 3,
        C_STREAM: 4
    },

    sensorType:
    {
        S_DOOR: 0, // Door sensor, V_TRIPPED, V_ARMED
        S_MOTION: 1, // Motion sensor, V_TRIPPED, V_ARMED 
        S_SMOKE: 2, // Smoke sensor, V_TRIPPED, V_ARMED
        //  S_LIGHT: 3, // Binary light or relay, V_STATUS (or V_LIGHT), V_WATT
        S_BINARY: 3, // Binary light or relay, V_STATUS (or V_LIGHT), V_WATT (same as S_LIGHT)
        S_DIMMER: 4, // Dimmable light or fan device, V_STATUS (on/off), V_DIMMER (dimmer level 0-100), V_WATT
        S_COVER: 5, // Blinds or window cover, V_UP, V_DOWN, V_STOP, V_DIMMER (open/close to a percentage)
        S_TEMP: 6, // Temperature sensor, V_TEMP
        S_HUM: 7, // Humidity sensor, V_HUM
        S_BARO: 8, // Barometer sensor, V_PRESSURE, V_FORECAST
        S_WIND: 9, // Wind sensor, V_WIND, V_GUST
        S_RAIN: 10, // Rain sensor, V_RAIN, V_RAINRATE
        S_UV: 11, // Uv sensor, V_UV
        S_WEIGHT: 12, // Personal scale sensor, V_WEIGHT, V_IMPEDANCE
        S_POWER: 13, // Power meter, V_WATT, V_KWH
        S_HEATER: 14, // Header device, V_HVAC_SETPOINT_HEAT, V_HVAC_FLOW_STATE, V_TEMP
        S_DISTANCE: 15, // Distance sensor, V_DISTANCE
        S_LIGHT_LEVEL: 16, // Light level sensor, V_LIGHT_LEVEL (uncalibrated in percentage),  V_LEVEL (light level in lux)
        S_ARDUINO_NODE: 17, // Used (internally) for presenting a non-repeating Arduino node
        S_ARDUINO_REPEATER_NODE: 18, // Used (internally) for presenting a repeating Arduino node 
        S_LOCK: 19, // Lock device, V_LOCK_STATUS
        S_IR: 20, // Ir device, V_IR_SEND, V_IR_RECEIVE
        S_WATER: 21, // Water meter, V_FLOW, V_VOLUME
        S_AIR_QUALITY: 22, // Air quality sensor, V_LEVEL
        S_CUSTOM: 23, // Custom sensor 
        S_DUST: 24, // Dust sensor, V_LEVEL
        S_SCENE_CONTROLLER: 25, // Scene controller device, V_SCENE_ON, V_SCENE_OFF. 
        S_RGB_LIGHT: 26, // RGB light. Send color component data using V_RGB. Also supports V_WATT 
        S_RGBW_LIGHT: 27, // RGB light with an additional White component. Send data using V_RGBW. Also supports V_WATT
        S_COLOR_SENSOR: 28, // Color sensor, send color information using V_RGB
        S_HVAC: 29,
        // Thermostat/HVAC device. V_HVAC_SETPOINT_HEAT, V_HVAC_SETPOINT_COLD, V_HVAC_FLOW_STATE, V_HVAC_FLOW_MODE, V_TEMP
        S_MULTIMETER: 30, // Multimeter device, V_VOLTAGE, V_CURRENT, V_IMPEDANCE 
        S_SPRINKLER: 31, // Sprinkler, V_STATUS (turn on/off), V_TRIPPED (if fire detecting device)
        S_WATER_LEAK: 32, // Water leak sensor, V_TRIPPED, V_ARMED
        S_SOUND: 33, // Sound sensor, V_TRIPPED, V_ARMED, V_LEVEL (sound level in dB)
        S_VIBRATION: 34, // Vibration sensor, V_TRIPPED, V_ARMED, V_LEVEL (vibration in Hz)
        S_MOISTURE: 35 // Moisture sensor, V_TRIPPED, V_ARMED, V_LEVEL (water content or moisture in percentage?) 
    },

    sensorTypeSimple:
    {
        "Door": 0, // Door sensor, V_TRIPPED, V_ARMED
        "Motion": 1, // Motion sensor, V_TRIPPED, V_ARMED 
        "Smoke": 2, // Smoke sensor, V_TRIPPED, V_ARMED
        //  S_LIGHT": 3, // Binary light or relay, V_STATUS (or V_LIGHT), V_WATT
        "Binary": 3, // Binary light or relay, V_STATUS (or V_LIGHT), V_WATT (same as S_LIGHT)
        "Dimmer": 4, // Dimmable light or fan device, V_STATUS (on/off), V_DIMMER (dimmer level 0-100), V_WATT
        "Cover": 5, // Blinds or window cover, V_UP, V_DOWN, V_STOP, V_DIMMER (open/close to a percentage)
        "Temperature": 6, // Temperature sensor, V_TEMP
        "Humidity": 7, // Humidity sensor, V_HUM
        "Baro": 8, // Barometer sensor, V_PRESSURE, V_FORECAST
        "Wind": 9, // Wind sensor, V_WIND, V_GUST
        "Rain": 10, // Rain sensor, V_RAIN, V_RAINRATE
        "UV": 11, // Uv sensor, V_UV
        "Weight": 12, // Personal scale sensor, V_WEIGHT, V_IMPEDANCE
        "Power": 13, // Power meter, V_WATT, V_KWH
        "Heater": 14, // Header device, V_HVAC_SETPOINT_HEAT, V_HVAC_FLOW_STATE, V_TEMP
        "Distance": 15, // Distance sensor, V_DISTANCE
        "Light level": 16, // Light level sensor, V_LIGHT_LEVEL (uncalibrated in percentage),  V_LEVEL (light level in lux)
        "Non-repeater node": 17, // Used (internally) for presenting a non-repeating Arduino node
        "Repeater node": 18, // Used (internally) for presenting a repeating Arduino node 
        "Lock": 19, // Lock device, V_LOCK_STATUS
        "IR": 20, // Ir device, V_IR_SEND, V_IR_RECEIVE
        "Water": 21, // Water meter, V_FLOW, V_VOLUME
        "Air quality": 22, // Air quality sensor, V_LEVEL
        "Custom": 23, // Custom sensor 
        "Dust": 24, // Dust sensor, V_LEVEL
        "Scene controller": 25, // Scene controller device, V_SCENE_ON, V_SCENE_OFF. 
        "RGB Light": 26, // RGB light. Send color component data using V_RGB. Also supports V_WATT 
        "RGBW Light": 27, // RGB light with an additional White component. Send data using V_RGBW. Also supports V_WATT
        "Color sensor": 28, // Color sensor, send color information using V_RGB
        "HVAC": 29,
        // Thermostat/HVAC device. V_HVAC_SETPOINT_HEAT, V_HVAC_SETPOINT_COLD, V_HVAC_FLOW_STATE, V_HVAC_FLOW_MODE, V_TEMP
        "Multimiter": 30, // Multimeter device, V_VOLTAGE, V_CURRENT, V_IMPEDANCE 
        "Sprinkler": 31, // Sprinkler, V_STATUS (turn on/off), V_TRIPPED (if fire detecting device)
        "Water leak": 32, // Water leak sensor, V_TRIPPED, V_ARMED
        "Sound": 33, // Sound sensor, V_TRIPPED, V_ARMED, V_LEVEL (sound level in dB)
        "Vibration": 34, // Vibration sensor, V_TRIPPED, V_ARMED, V_LEVEL (vibration in Hz)
        "Moisture": 35 // Moisture sensor, V_TRIPPED, V_ARMED, V_LEVEL (water content or moisture in percentage?) 
    },

    sensorDataType:
    {
        V_TEMP: 0, // S_TEMP. Temperature S_TEMP, S_HEATER, S_HVAC
        V_HUM: 1, // S_HUM. Humidity
        V_STATUS: 2, //  S_LIGHT, S_DIMMER, S_SPRINKLER, S_HVAC, S_HEATER. Used for setting/reporting binary (on/off) status. 1=on, 0=off  
        //  V_LIGHT: 2, // Same as V_STATUS
        V_PERCENTAGE: 3, // S_DIMMER. Used for sending a percentage value 0-100 (%). 
        //  V_DIMMER: 3, // S_DIMMER. Same as V_PERCENTAGE.  
        V_PRESSURE: 4, // S_BARO. Atmospheric Pressure
        V_FORECAST: 5, // S_BARO. Whether forecast. string of "stable", "sunny", "cloudy", "unstable", "thunderstorm" or "unknown"
        V_RAIN: 6, // S_RAIN. Amount of rain
        V_RAINRATE: 7, // S_RAIN. Rate of rain
        V_WIND: 8, // S_WIND. Wind speed
        V_GUST: 9, // S_WIND. Gust
        V_DIRECTION: 10, // S_WIND. Wind direction 0-360 (degrees)
        V_UV: 11, // S_UV. UV light level
        V_WEIGHT: 12, // S_WEIGHT. Weight(for scales etc)
        V_DISTANCE: 13, // S_DISTANCE. Distance
        V_IMPEDANCE: 14, // S_MULTIMETER: , S_WEIGHT. Impedance value
        V_ARMED: 15, // S_DOOR: , S_MOTION: , S_SMOKE: , S_SPRINKLER. Armed status of a security sensor. 1 = Armed: , 0 = Bypassed
        V_TRIPPED: 16, // S_DOOR: , S_MOTION: , S_SMOKE: , S_SPRINKLER: , S_WATER_LEAK: , S_SOUND: , S_VIBRATION: , S_MOISTURE. Tripped status of a security sensor. 1 = Tripped: , 0
        V_WATT: 17, // S_POWER: , S_LIGHT: , S_DIMMER: , S_RGB: , S_RGBW. Watt value for power meters
        V_KWH: 18, // S_POWER. Accumulated number of KWH for a power meter
        V_SCENE_ON: 19, // S_SCENE_CONTROLLER. Turn on a scene
        V_SCENE_OFF: 20, // S_SCENE_CONTROLLER. Turn of a scene
        //   V_HEATER: 21, // Deprecated. Use V_HVAC_FLOW_STATE instead.
        V_HVAC_FLOW_STATE: 21, // S_HEATER: , S_HVAC. HVAC flow state ("Off": , "HeatOn": , "CoolOn": , or "AutoChangeOver") 
        V_HVAC_SPEED: 22, // S_HVAC: , S_HEATER. HVAC/Heater fan speed ("Min": , "Normal": , "Max": , "Auto") 
        V_LIGHT_LEVEL: 23, // S_LIGHT_LEVEL. Uncalibrated light level. 0-100%. Use V_LEVEL for light level in lux
        V_VAR1: 24,
        V_VAR2: 25,
        V_VAR3: 26,
        V_VAR4: 27,
        V_VAR5: 28,
        V_UP: 29, // S_COVER. Window covering. Up
        V_DOWN: 30, // S_COVER. Window covering. Down
        V_STOP: 31, // S_COVER. Window covering. Stop
        V_IR_SEND: 32, // S_IR. Send out an IR-command
        V_IR_RECEIVE: 33, // S_IR. This message contains a received IR-command
        V_FLOW: 34, // S_WATER. Flow of water (in meter)
        V_VOLUME: 35, // S_WATER. Water volume
        V_LOCK_STATUS: 36, // S_LOCK. Set or get lock status. 1=Locked: , 0=Unlocked
        V_LEVEL: 37, // S_DUST: , S_AIR_QUALITY: , S_SOUND (dB): , S_VIBRATION (hz): , S_LIGHT_LEVEL (lux)
        V_VOLTAGE: 38, // S_MULTIMETER 
        V_CURRENT: 39, // S_MULTIMETER
        V_RGB: 40, // S_RGB_LIGHT: , S_COLOR_SENSOR. 
        // Used for sending color information for multi color LED lighting or color sensors. 
        // Sent as ASCII hex: RRGGBB (RR=red: , GG=green: , BB=blue component)
        V_RGBW: 41, // S_RGBW_LIGHT
        // Used for sending color information to multi color LED lighting. 
        // Sent as ASCII hex: RRGGBBWW (WW=white component)
        V_ID: 42, // S_TEMP
        // Used for sending in sensors hardware ids (i.e. OneWire DS1820b). 
        V_UNIT_PREFIX: 43, // S_DUST: , S_AIR_QUALITY
        // Allows sensors to send in a string representing the 
        // unit prefix to be displayed in GUI: , not parsed by controller! E.g. cm: , m: , km: , inch.
        // Can be used for S_DISTANCE or gas concentration
        V_HVAC_SETPOINT_COOL: 44, // S_HVAC. HVAC cool setpoint (Integer between 0-100)
        V_HVAC_SETPOINT_HEAT: 45, // S_HEATER: , S_HVAC. HVAC/Heater setpoint (Integer between 0-100)
        V_HVAC_FLOW_MODE: 46 // S_HVAC. Flow mode for HVAC ("Auto": , "ContinuousOn": , "PeriodicOn")

    },


    internalDataType:
    {
        I_BATTERY_LEVEL: 0,
        I_TIME: 1,
        I_VERSION: 2,
        I_ID_REQUEST: 3,
        I_ID_RESPONSE: 4,
        I_INCLUSION_MODE: 5,
        I_CONFIG: 6,
        I_FIND_PARENT: 7,
        I_FIND_PARENT_RESPONSE: 8,
        I_LOG_MESSAGE: 9,
        I_CHILDREN: 10,
        I_SKETCH_NAME: 11,
        I_SKETCH_VERSION: 12,
        I_REBOOT: 13,
        I_GATEWAY_READY: 14,
        I_REQUEST_SIGNING: 15,
        I_GET_NONCE: 16,
        I_GET_NONCE_RESPONSE: 17
    }
}
/*

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
*/