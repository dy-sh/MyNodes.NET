/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

$.noty.defaults.layout = 'bottomRight';
$.noty.defaults.theme = 'relax';
$.noty.defaults.timeout = 3000;
$.noty.defaults.animation = {
    open: 'animated bounceInRight', // Animate.css class names
    close: 'animated flipOutX', // Animate.css class names
    easing: 'swing', // unavailable - no need
    speed: 500 // unavailable - no need
};

var clientsHub;
var gatewayHardwareConnected = false;
var gatewayServiceConnected = false;

var sliderUpdateInterval = 40; //increase this interval if you get excaption on moving slider
var elementsFadeTime = 500;

var nodes;

var slidersArray = [];
var rgbSlidersArray = [];
var rgbwSlidersArray = [];
setInterval(sendSliders, sliderUpdateInterval);
var ignoreSendingSwitchId;


$(function () {
    clientsHub = $.connection.clientsHub;

    clientsHub.client.onGatewayConnected = function () {
        var n = noty({ text: 'Gateway hardware is online.', type: 'alert', timeout: false });
        gatewayHardwareConnected = true;
        gatewayStatusChanged();
    };

    clientsHub.client.onGatewayDisconnected = function () {
        var n = noty({ text: 'Gateway hardware is offline!', type: 'error', timeout: false });
        gatewayHardwareConnected = false;
        gatewayStatusChanged();
    };

    clientsHub.client.onGatewayServiceConnected = function () {
        var n = noty({ text: 'Gateway service is online.', type: 'alert', timeout: false });
        gatewayServiceConnected = true;
        gatewayStatusChanged();
        clientsHub.server.getGatewayHardwareConnected();
    };

    clientsHub.client.onGatewayServiceDisconnected = function () {
        var n = noty({ text: 'Gateway service is offline!', type: 'error', timeout: false });
        gatewayServiceConnected = false;
        gatewayHardwareConnected = false;
        gatewayStatusChanged();
    };

    clientsHub.client.returnGatewayServiceConnected = function (isConnected) {
        gatewayServiceConnected = isConnected;
        gatewayStatusChanged();
        clientsHub.server.getGatewayHardwareConnected();

    };


    clientsHub.client.returnGatewayHardwareConnected = function (isConnected) {
        gatewayHardwareConnected = isConnected;
        gatewayStatusChanged();
        clientsHub.server.getNodes();

        if (!gatewayServiceConnected)
            var n = noty({ text: 'Gateway service is offline!', type: 'error', timeout: false });
        else if (!gatewayHardwareConnected)
            var n = noty({ text: 'Gateway hardware is offline!', type: 'error', timeout: false });
    };

    clientsHub.client.onClearNodesList = function () {
        var n = noty({ text: 'Nodes deleted from the database!', type: 'error' });
        $('#nodesContainer').html(null);
    };



    clientsHub.client.returnNodes = function (nodes) {
        onReturnNodes(nodes);
    };

    clientsHub.client.onNewNode = function (node) {
        createOrUpdateNode(node);
    };

    clientsHub.client.onNodeUpdated = function (node) {
        createOrUpdateNode(node);
    };

    clientsHub.client.onNodeLastSeenUpdated = function (node) {
        updateLastSeen(node);
    };

    clientsHub.client.onNodeBatteryUpdated = function (node) {
        updateBattery(node);
    };

    clientsHub.client.onSensorUpdated = function (sensor) {
        createOrUpdateSensor(sensor);
    };

    clientsHub.client.onNewSensor = function (sensor) {
        createOrUpdateSensor(sensor);
    };

    $.connection.hub.start().done(function () {
        clientsHub.server.getGatewayServiceConnected();
    });

});



function gatewayStatusChanged() {
    if (gatewayHardwareConnected && gatewayServiceConnected)
        $('#nodesContainer').fadeIn(800);
    else
        $('#nodesContainer').fadeOut(800);
}


function onReturnNodes(nodes) {
    var temp = elementsFadeTime;
    elementsFadeTime = 0;
    $('#nodesContainer').html(null);
    slidersArray.length = 0;
    rgbSlidersArray.length = 0;
    rgbwSlidersArray.length = 0;

    for (var i = 0; i < nodes.length; i++) {
        createOrUpdateNode(nodes[i]);
    }
    elementsFadeTime = temp;

}




var nodeTemplate = Handlebars.compile($('#nodeTemplate').html());
var sensorTemplate = Handlebars.compile($('#sensorTemplate').html());
var dataTemplate = Handlebars.compile($('#dataTemplate').html());
var toggleTemplate = Handlebars.compile($('#toggleTemplate').html());
var sliderTemplate = Handlebars.compile($('#sliderTemplate').html());
var rgbSlidersTemplate = Handlebars.compile($('#rgbSlidersTemplate').html());
var rgbwSlidersTemplate = Handlebars.compile($('#rgbwSlidersTemplate').html());
var irSendTemplate = Handlebars.compile($('#irSendTemplate').html());
var nodeMenuTemplate = Handlebars.compile($('#nodeMenuTemplate').html());


Handlebars.registerHelper("sensor-id", function (sensor) {
    return sensor.nodeId + "-" + sensor.sensorId;
});

Handlebars.registerHelper("sensor-title", function (sensor) {
    return getSensorTitle(sensor);
});

Handlebars.registerHelper("sensordata-id", function (data) {
    return data.nodeId + "-" + data.sensorId + "-" + data.dataType;
});


function getSensorTitle(sensor) {
    if (sensor.description != null)
        return sensor.description;
    else {
        var sensorType = Object.keys(mySensors.sensorTypeSimple)[sensor.sensorType];
        if (sensorType == null) sensorType = "Unknown";
        return sensorType;
    }
}


function createOrUpdateNode(node) {
    var id = node.nodeId;

    var nodePanel = $('#nodePanel' + node.nodeId);

    if (nodePanel.length == 0) {
        //create new
        $(nodeTemplate(node)).hide().appendTo("#nodesContainer").fadeIn(elementsFadeTime);
    } else {
        //update
        nodePanel.html(nodeTemplate(node));
    }

    updateNodeMenu(node);

    for (var i = 0; i < node.sensors.length; i++) {
            createOrUpdateSensor(node.sensors[i]);
    }
}


function updateLastSeen(node) {
    $('#activity' + node.nodeId).show().fadeOut(500);
}

function updateBattery(node) {
    var nodeBattery = $('#nodeBattery' + node.nodeId);

    if (nodeBattery.length == 0)
        createOrUpdateNode(node);
    else nodeBattery.html(node.batteryLevel);
}

function updateNodeMenu(node) {
    var nodeMenu = $('#nodeMenu' + node.nodeId);
    nodeMenu.html(nodeMenuTemplate(node));
}


function createOrUpdateSensor(sensor) {
    var id = sensor.nodeId + "-" + sensor.sensorId;

    if ($('#sensorPanel' + id).length == 0) {
        //create new
        $(sensorTemplate(sensor)).hide().appendTo("#sensorsContainer" + sensor.nodeId).fadeIn(elementsFadeTime);
    }
    else {
        //update
        $('#sensorTitle' + id).html(getSensorTitle(sensor));
    }

    createOrUpdateSensorData(sensor);
}


function createOrUpdateSensorData(sensor) {

    var sensorData = JSON.parse(sensor.sensorDataJson);

    if (sensorData == null || sensorData.length == 0)
        return;

    var sensorId = sensor.nodeId + "-" + sensor.sensorId;

    for (var i = 0; i < sensorData.length; i++) {
        var data = sensorData[i];
        var id = data.nodeId + "-" + data.sensorId + "-" + data.dataType;

        if ($('#dataPanel' + id).length == 0) {
            //create new
            $(dataTemplate(data)).hide().appendTo("#sensorPanel" + sensorId).fadeIn(elementsFadeTime);
        }

        //update body

        //ON-OFF BUTTON
        if (data.dataType == mySensors.sensorDataType.V_TRIPPED
            || data.dataType == mySensors.sensorDataType.V_STATUS) {
            if ($("[name='toggle-" + id + "']").length == 0) {
                //create new
                $(toggleTemplate(data)).hide().appendTo("#dataPanel" + id).fadeIn(elementsFadeTime);

                $("[name='toggle-" + id + "']").bootstrapSwitch('state', data.state == "1");

                $("[name='toggle-" + id + "']").on('switchChange.bootstrapSwitch', function (event, state) {
                    if (ignoreSendingSwitchId == id)
                        return;
                    var toggle = this.name.split("-");
                    var val = state == true ? 1 : 0;
                    sendSensor(toggle[1], toggle[2], toggle[3], val);
                });
            } else {
                //update
                ignoreSendingSwitchId = id;
                $("[name='toggle-" + id + "']").bootstrapSwitch('state', data.state == "1");
                ignoreSendingSwitchId = null;
            }
        }
            //0-100% SLIDER
        else if (data.dataType == mySensors.sensorDataType.V_PERCENTAGE
            || data.dataType == mySensors.sensorDataType.V_LIGHT_LEVEL) {

            if (isNaN(data.state)) data.state = 0;

            if ($("[name='slider-" + id + "']").length == 0) {
                //create new
                $(sliderTemplate(data)).hide().appendTo("#dataPanel" + id).fadeIn(elementsFadeTime);

                $("[name='slider-" + id + "']").slider({ value: data.state, range: "min" });

                $('#dataPanel' + id).removeClass("pull-right");


                slidersArray.push({
                    sliderId: id,
                    nodeId: sensor.nodeId,
                    sensorId: sensor.sensorId,
                    dataType: data.dataType,
                    lastVal: data.state
                });
            } else {
                //update
                $("[name='slider-" + id + "']").slider("value", data.state);
                updateSliderInArray(id, data.state);

            }
        }
            //RGB SLIDERS
        else if (data.dataType == mySensors.sensorDataType.V_RGB) {

            var r = hexToRgb(data.state).r;
            var g = hexToRgb(data.state).g;
            var b = hexToRgb(data.state).b;
            if (isNaN(r)) r = 0;
            if (isNaN(g)) g = 0;
            if (isNaN(b)) b = 0;


            if ($("[name='slider-" + id + "-r']").length == 0) {
                //create new
                $(rgbSlidersTemplate(data)).hide().appendTo("#dataPanel" + id).fadeIn(elementsFadeTime);


                $("[name='slider-" + id + "-r']").slider({ value: r, range: "min", max: 255 });
                $("[name='slider-" + id + "-g']").slider({ value: g, range: "min", max: 255 });
                $("[name='slider-" + id + "-b']").slider({ value: b, range: "min", max: 255 });

                $('#dataPanel' + id).removeClass("pull-right");


                rgbSlidersArray.push({
                    sliderId: id,
                    nodeId: sensor.nodeId,
                    sensorId: sensor.sensorId,
                    dataType: data.dataType,
                    lastR: r,
                    lastG: g,
                    lastB: b
                });
            } else {
                //update
                $("[name='slider-" + id + "-r']").slider({ value: r });
                $("[name='slider-" + id + "-g']").slider({ value: g });
                $("[name='slider-" + id + "-b']").slider({ value: b });
                updateRgbSlidersInArray(id, data.state);
            }

        }
            //RGBW SLIDERS
        else if (data.dataType == mySensors.sensorDataType.V_RGBW) {

            var r = hexToRgbw(data.state).r;
            var g = hexToRgbw(data.state).g;
            var b = hexToRgbw(data.state).b;
            var w = hexToRgbw(data.state).w;
            if (isNaN(r)) r = 0;
            if (isNaN(g)) g = 0;
            if (isNaN(b)) b = 0;
            if (isNaN(w)) w = 0;

            if ($("[name='slider-" + id + "-r']").length == 0) {
                //create new
                $(rgbwSlidersTemplate(data)).hide().appendTo("#dataPanel" + id).fadeIn(elementsFadeTime);


                $("[name='slider-" + id + "-r']").slider({ value: r, range: "min", max: 255 });
                $("[name='slider-" + id + "-g']").slider({ value: g, range: "min", max: 255 });
                $("[name='slider-" + id + "-b']").slider({ value: b, range: "min", max: 255 });
                $("[name='slider-" + id + "-w']").slider({ value: w, range: "min", max: 255 });

                $('#dataPanel' + id).removeClass("pull-right");


                rgbwSlidersArray.push({
                    sliderId: id,
                    nodeId: sensor.nodeId,
                    sensorId: sensor.sensorId,
                    dataType: data.dataType,
                    lastR: r,
                    lastG: g,
                    lastB: b,
                    lastW: w
                });
            } else {
                //update
                $("[name='slider-" + id + "-r']").slider({ value: r });
                $("[name='slider-" + id + "-g']").slider({ value: g });
                $("[name='slider-" + id + "-b']").slider({ value: b });
                updateRgbSlidersInArray(id, data.state);
            }

        }
            //IR SEND
        else if (data.dataType == mySensors.sensorDataType.V_IR_SEND) {


            if ($("[name='textbox-" + id + "']").length == 0) {
                //create new
                $(irSendTemplate(data)).hide().appendTo("#dataPanel" + id).fadeIn(elementsFadeTime);

                $('#dataPanel' + id).removeClass("pull-right");
                $('#dataPanel' + id).removeClass("pull-up");

                $("[name='button-" + id + "']").click(function () {
                    var code = $("[name='textbox-" + id + "']").val();
                    sendSensor(sensor.nodeId, sensor.sensorId, data.dataType, code);
                });

            } else {
                //update

            }
        }
            //IR RECEIVE
        else if (data.dataType == mySensors.sensorDataType.V_IR_RECEIVE) {
            $('#dataPanel' + id)
                .html("<br/>IR receive: " + data.state);

            $('#dataPanel' + id).removeClass("pull-right");
            $('#dataPanel' + id).removeClass("pull-up");

        }
            //Simple text
        else {
            $('#dataPanel' + id)
                .html(data.state);
        }
    }

}

function sendSensor(nodeId, sensorId, dataType, val) {
    var message = "" +
        nodeId + ";"
        + sensorId + ";"
        + mySensors.messageType.C_SET + ";"
        + "0" + ";"
        + dataType + ";"
        + val;

    console.log(message);


    clientsHub.server.sendMessage(message);
}

function sendSliders() {

    for (var i = 0; i < slidersArray.length; i++) {
        var id = slidersArray[i].sliderId;
        var currentVal = $("[name='slider-" + id + "']").slider("value");

        if (!isNaN(currentVal) && currentVal != slidersArray[i].lastVal) {

            slidersArray[i].lastVal = currentVal;
            sendSensor(slidersArray[i].nodeId,
                slidersArray[i].sensorId,
                slidersArray[i].dataType,
                slidersArray[i].lastVal);
        }
    }

    for (var i = 0; i < rgbSlidersArray.length; i++) {
        var id = rgbSlidersArray[i].sliderId;
        var currentR = $("[name='slider-" + id + "-r']").slider("value");
        var currentG = $("[name='slider-" + id + "-g']").slider("value");
        var currentB = $("[name='slider-" + id + "-b']").slider("value");

        if (currentR != rgbSlidersArray[i].lastR ||
            currentG != rgbSlidersArray[i].lastG ||
            currentB != rgbSlidersArray[i].lastB) {

            var hex = RgbToHex(currentR, currentG, currentB);
            updateRgbSlidersInArray(id, hex);

            sendSensor(rgbSlidersArray[i].nodeId,
                rgbSlidersArray[i].sensorId,
                rgbSlidersArray[i].dataType,
                hex);
        }
    }

    for (var i = 0; i < rgbwSlidersArray.length; i++) {
        var id = rgbwSlidersArray[i].sliderId;
        var currentR = $("[name='slider-" + id + "-r']").slider("value");
        var currentG = $("[name='slider-" + id + "-g']").slider("value");
        var currentB = $("[name='slider-" + id + "-b']").slider("value");
        var currentW = $("[name='slider-" + id + "-w']").slider("value");

        if (currentR != rgbwSlidersArray[i].lastR ||
            currentG != rgbwSlidersArray[i].lastG ||
            currentB != rgbwSlidersArray[i].lastB ||
            currentW != rgbwSlidersArray[i].lastW) {

            var hex = RgbwToHex(currentR, currentG, currentB);
            updateRgbwSlidersInArray(id, hex);

            sendSensor(rgbwSlidersArray[i].nodeId,
                rgbwSlidersArray[i].sensorId,
                rgbwSlidersArray[i].dataType,
                hex);
        }
    }
}

function updateSliderInArray(sliderId, lastVal) {
    for (var i = 0; i < slidersArray.length; i++) {
        if (slidersArray[i].sliderId == sliderId)
            slidersArray[i].lastVal = lastVal;
    }
}

function updateRgbSlidersInArray(sliderId, lastHex) {
    for (var i = 0; i < rgbSlidersArray.length; i++) {
        if (rgbSlidersArray[i].sliderId == sliderId) {
            rgbSlidersArray[i].lastR = hexToRgb(lastHex).r;
            rgbSlidersArray[i].lastG = hexToRgb(lastHex).g;
            rgbSlidersArray[i].lastB = hexToRgb(lastHex).b;
        }
    }
}


function updateRgbwSlidersInArray(sliderId, lastHex) {
    for (var i = 0; i < rgbwSlidersArray.length; i++) {
        if (rgbwSlidersArray[i].sliderId == sliderId) {
            rgbwSlidersArray[i].lastR = hexToRgbw(lastHex).r;
            rgbwSlidersArray[i].lastG = hexToRgbw(lastHex).g;
            rgbwSlidersArray[i].lastB = hexToRgbw(lastHex).b;
            rgbwSlidersArray[i].lastW = hexToRgbw(lastHex).w;
        }
    }
}

