/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

var sliderUpdateInterval = 40; //increase this interval if you get excaption on moving slider
var elementsFadeTime = 500;

var gatewayHub;
var slidersArray = [];
var rgbSlidersArray = [];
var rgbwSlidersArray = [];
setInterval(sendSliders, sliderUpdateInterval);
var ignoreSendingSwitchId;

$.noty.defaults.layout = 'bottomRight';
$.noty.defaults.theme = 'relax';
$.noty.defaults.timeout = 3000;
$.noty.defaults.animation = {
    open: 'animated bounceInRight', // Animate.css class names
    close: 'animated flipOutX', // Animate.css class names
    easing: 'swing', // unavailable - no need
    speed: 500 // unavailable - no need
};


$(function () {
 


    gatewayHub = $.connection.gatewayHub;

    gatewayHub.client.returnNodes = function (nodes) {
        onReturnNodes(nodes);
    };

    gatewayHub.client.onNewNodeEvent = function (node) {
        createOrUpdateNode(node);
    };

    gatewayHub.client.onNodeUpdatedEvent = function (node) {
        createOrUpdateNode(node);
    };

    gatewayHub.client.onNodeLastSeenUpdatedEvent = function (node) {
        updateLastSeen(node);
    };

    gatewayHub.client.onNodeBatteryUpdatedEvent = function (node) {
        updateBattery(node);
    };

    gatewayHub.client.onSensorUpdatedEvent = function (sensor) {
        createOrUpdateSensor(sensor);
    };

    gatewayHub.client.onNewSensorEvent = function (sensor) {
        createOrUpdateSensor(sensor);
    };

    gatewayHub.client.onGatewayDisconnectedEvent = function () {
        onGatewayDisconnected();
    };

    gatewayHub.client.onGatewayConnectedEvent = function () {
        onGatewayConnected();
    };

    gatewayHub.client.onClearNodesListEvent = function (sensor) {
        $('#nodesContainer').html(null);
    };

    $.connection.hub.start().done(function () {
        $.get("GetNodes/");
    });

});

function onGatewayDisconnected() {
    var n = noty({
        text: 'Gateway disconnected!',
        type: 'error',
        timeout: false
    });
    $('#nodesContainer').fadeOut(300);
}

function onGatewayConnected() {
    var n = noty({
        text: 'Gateway connected.',
        type: 'alert',
        timeout: false
    });
    $('#nodesContainer').fadeIn(300);
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

function createOrUpdateNode(node) {
    var id = node.nodeId;

    if ($('#nodePanel' + id).length == 0) {
        //create new
        $('#nodeTemplate')
            .clone()
            .attr("id", "nodePanel" + id)
            //.css('display', 'block')
            .fadeIn(elementsFadeTime)
            .appendTo('#nodesContainer');

        $('#nodePanel' + id)
            .find('#nodeTitle')
            .attr("id", "nodeTitle" + id)
            .html("Node " + id);

        $('#nodePanel' + id)
            .find('#nodeBody')
            .attr("id", "nodeBody" + id);

        $('#nodePanel' + id)
            .find('#sensorsContainer')
            .attr("id", "sensorsContainer" + id);
    }

    //update body
    $('#nodeBody' + id)
        .html(null);

    if (node.name != null)
        $('#nodeBody' + id)
            .append(node.name + "<br/>");

    if (node.batteryLevel != null)
        $('#nodeBody' + id)
            .append("<div id='nodeBattery" + id + "'>"
                + "Battery: " + node.batteryLevel
                + "</div>").hide().fadeIn(elementsFadeTime);

    for (var i = 0; i < node.sensors.length; i++) {
        createOrUpdateSensor(node.sensors[i]);
    }
}


function updateLastSeen(node) {

}

function updateBattery(node) {
    var id = node.nodeId;
    $('#nodeBattery' + id)
        .html("Battery: " + node.batteryLevel);
}


function createOrUpdateSensor(sensor) {

    var id = sensor.ownerNodeId + "-" + sensor.sensorId;

    if ($('#sensorPanel' + id).length == 0) {
        //create new
        $('#sensorsContainer' + sensor.ownerNodeId)
            .append("<li class='list-group-item' id='sensorPanel" + id + "'>"
                + "</li>").hide().fadeIn(elementsFadeTime);

        $('#sensorPanel' + id)
            .append("<div id='sensorTitle" + id + "'></div>");
    }

    //update body

    var sensorType = Object.keys(mySensors.sensorTypeSimple)[sensor.sensorType];
    if (sensorType == null) sensorType = "Unknown";

    if (sensor.description != null)
        $('#sensorTitle' + id)
            .html(sensor.description);
    else
        $('#sensorTitle' + id)
            .html(sensorType);

    createOrUpdateSensorData(sensor);

}


function createOrUpdateSensorData(sensor) {

    var sensorData = JSON.parse(sensor.sensorDataJson);

    if (sensorData == null || sensorData.length == 0)
        return;

    var sensorId = sensor.ownerNodeId + "-" + sensor.sensorId;

    for (var i = 0; i < sensorData.length; i++) {
        var data = sensorData[i];
        var id = sensor.ownerNodeId + "-" + sensor.sensorId + "-" + data.dataType;


        if ($('#dataPanel' + id).length == 0) {
            //create new
            $('#sensorPanel' + sensorId)
                .append('<div class="pull-right pull-up" id="dataPanel' + id + '">' + '</div>').hide().fadeIn(elementsFadeTime);

            //if (sensorData.length > 1)
            //    $('#sensorPanel' + sensorId)
            //        .append('<br/><br/>');
        }

        //update body

        //ON-OFF BUTTON
        if (data.dataType == mySensors.sensorDataType.V_TRIPPED
            || data.dataType == mySensors.sensorDataType.V_STATUS) {
            if ($("[name='toggle-" + id + "']").length == 0) {
                //create new
                $('#dataPanel' + id)
                    .html("<input type='checkbox' name='toggle-" + id + "' data-label-width='0' data-size='small'>");

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
                $('#dataPanel' + id)
                    .html("<div id='slider' name='slider-" + id + "'></div>");
                $("[name='slider-" + id + "']").slider({ value: data.state, range: "min" });

                $('#dataPanel' + id).removeClass("pull-right");


                slidersArray.push({
                    sliderId: id,
                    nodeId: sensor.ownerNodeId,
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
                $('#dataPanel' + id)
                    .html("<div id='slider' name='slider-" + id + "-r'></div>")
                    .append("<div id='slider' name='slider-" + id + "-g'></div>")
                    .append("<div id='slider' name='slider-" + id + "-b'></div>");

                $("[name='slider-" + id + "-r']").slider({ value: r, range: "min", max: 255 });
                $("[name='slider-" + id + "-g']").slider({ value: g, range: "min", max: 255 });
                $("[name='slider-" + id + "-b']").slider({ value: b, range: "min", max: 255 });

                $('#dataPanel' + id).removeClass("pull-right");


                rgbSlidersArray.push({
                    sliderId: id,
                    nodeId: sensor.ownerNodeId,
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
                $('#dataPanel' + id)
                    .html("<div id='slider' name='slider-" + id + "-r'></div>")
                    .append("<div id='slider' name='slider-" + id + "-g'></div>")
                    .append("<div id='slider' name='slider-" + id + "-b'></div>")
                    .append("<div id='slider' name='slider-" + id + "-w'></div>");

                $("[name='slider-" + id + "-r']").slider({ value: r, range: "min", max: 255 });
                $("[name='slider-" + id + "-g']").slider({ value: g, range: "min", max: 255 });
                $("[name='slider-" + id + "-b']").slider({ value: b, range: "min", max: 255 });
                $("[name='slider-" + id + "-w']").slider({ value: w, range: "min", max: 255 });

                $('#dataPanel' + id).removeClass("pull-right");


                rgbwSlidersArray.push({
                    sliderId: id,
                    nodeId: sensor.ownerNodeId,
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
                $('#dataPanel' + id)
                    .html("<br/><div class='input-group'>"
                        + "<input type='text' class='form-control' placeholder='IR code...' name='textbox-" + id + "'>"
                        + "<span class='input-group-btn'>"
                        + "<button class='btn btn-default' type='button' name='button-" + id + "'>Send</button>"
                        + "</span>"
                        + "</div>");


                $('#dataPanel' + id).removeClass("pull-right");
                $('#dataPanel' + id).removeClass("pull-up");


                $("[name='button-" + id + "']").click(function () {
                    var code = $("[name='textbox-" + id + "']").val();
                    sendSensor(sensor.ownerNodeId, sensor.sensorId, data.dataType, code);
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


    gatewayHub.server.sendMessage(message);
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

