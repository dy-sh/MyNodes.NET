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
        S_DOOR: 0,
        S_MOTION: 1,
        S_SMOKE: 2,
        //  S_LIGHT: 3, 
        S_BINARY: 3,
        S_DIMMER: 4,
        S_COVER: 5,
        S_TEMP: 6,
        S_HUM: 7,
        S_BARO: 8,
        S_WIND: 9,
        S_RAIN: 10,
        S_UV: 11,
        S_WEIGHT: 12,
        S_POWER: 13,
        S_HEATER: 14,
        S_DISTANCE: 15,
        S_LIGHT_LEVEL: 16,
        S_ARDUINO_NODE: 17,
        S_ARDUINO_REPEATER_NODE: 18,
        S_LOCK: 19,
        S_IR: 20,
        S_WATER: 21,
        S_AIR_QUALITY: 22,
        S_CUSTOM: 23,
        S_DUST: 24,
        S_SCENE_CONTROLLER: 25,
        S_RGB_LIGHT: 26,
        S_RGBW_LIGHT: 27,
        S_COLOR_SENSOR: 28,
        S_HVAC: 29,
        S_MULTIMETER: 30,
        S_SPRINKLER: 31,
        S_WATER_LEAK: 32,
        S_SOUND: 33,
        S_VIBRATION: 34,
        S_MOISTURE: 35
    },

    sensorTypeSimple:
    {
        "Door": 0,
        "Motion": 1,
        "Smoke": 2,
        //  S_LIGHT": 3, 
        "Binary": 3,
        "Dimmer": 4,
        "Cover": 5,
        "Temperature": 6,
        "Humidity": 7,
        "Baro": 8,
        "Wind": 9,
        "Rain": 10,
        "UV": 11,
        "Weight": 12,
        "Power": 13,
        "Heater": 14,
        "Distance": 15,
        "Light level": 16,
        "Non-repeater node": 17,
        "Repeater node": 18,
        "Lock": 19,
        "IR": 20,
        "Water": 21,
        "Air quality": 22,
        "Custom": 23,
        "Dust": 24,
        "Scene controller": 25,
        "RGB Light": 26,
        "RGBW Light": 27,
        "Color sensor": 28,
        "HVAC": 29,
        "Multimiter": 30,
        "Sprinkler": 31,
        "Water leak": 32,
        "Sound": 33,
        "Vibration": 34,
        "Moisture": 35
    },

    sensorDataType:
    {
        V_TEMP: 0,
        V_HUM: 1,
        V_STATUS: 2,
        //  V_LIGHT: 2,
        V_PERCENTAGE: 3,
        //  V_DIMMER: 3,
        V_PRESSURE: 4,
        V_FORECAST: 5,
        V_RAIN: 6,
        V_RAINRATE: 7,
        V_WIND: 8,
        V_GUST: 9,
        V_DIRECTION: 10,
        V_UV: 11,
        V_WEIGHT: 12,
        V_DISTANCE: 13,
        V_IMPEDANCE: 14,
        V_ARMED: 15,
        V_TRIPPED: 16,
        V_WATT: 17,
        V_KWH: 18,
        V_SCENE_ON: 19,
        V_SCENE_OFF: 20,
        //   V_HEATER: 21, 
        V_HVAC_FLOW_STATE: 21,
        V_HVAC_SPEED: 22,
        V_LIGHT_LEVEL: 23,
        V_VAR1: 24,
        V_VAR2: 25,
        V_VAR3: 26,
        V_VAR4: 27,
        V_VAR5: 28,
        V_UP: 29,
        V_DOWN: 30,
        V_STOP: 31,
        V_IR_SEND: 32,
        V_IR_RECEIVE: 33,
        V_FLOW: 34,
        V_VOLUME: 35,
        V_LOCK_STATUS: 36,
        V_LEVEL: 37,
        V_VOLTAGE: 38,
        V_CURRENT: 39,
        V_RGB: 40,
        V_RGBW: 41,
        V_ID: 42,
        V_UNIT_PREFIX: 43,
        V_HVAC_SETPOINT_COOL: 44,
        V_HVAC_SETPOINT_HEAT: 45,
        V_HVAC_FLOW_MODE: 46

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

function hexFromRGB(r, g, b) {
    var hex = [
      r.toString(16),
      g.toString(16),
      b.toString(16)
    ];
    $.each(hex, function (nr, val) {
        if (val.length === 1) {
            hex[nr] = "0" + val;
        }
    });
    return hex.join("").toUpperCase();
}