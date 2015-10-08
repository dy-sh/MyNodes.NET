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
             .html(" Node " + id);

        $('#nodePanel' + id)
            .find('#activity')
            .attr("id", "activity" + id);

        $('#nodePanel' + id)
            .find('#nodeBody')
            .attr("id", "nodeBody" + id);

        $('#nodePanel' + id)
            .find('#sensorsContainer')
            .attr("id", "sensorsContainer" + id);

        $('#nodePanel' + id)
            .find('#footer')
            .attr("id", "footer" + id);

        //$('#nodePanel' + id)
        //.find('#settingsButton')
        //.attr("id", "settingsButton" + id)
        //    .attr("href", "../Node/Settings/" + id);


        //create dropdown menu
        $('#nodePanel' + id)
            .find('#dropdownMenu')
            .attr("id", "dropdownMenu" + id);

        $('#nodePanel' + id)
        .find('#dropdownMenuList')
        .attr("id", "dropdownMenuList" + id)
        .attr("aria-labelledby", "dropdownMenu" + id);

        updateDDMenuFromNode(node);
    }

    //update body


    //update name
    if (node.name != null && $('#nodeName' + id).length == 0)
        $("<div></div>")
            .attr("id", "nodeName" + id).hide().fadeIn(elementsFadeTime)
            .appendTo('#nodeBody' + id);

    if (node.name == null && $('#nodeName' + id).length != 0)
        $('#nodeName' + id).remove();

    if (node.name != null)
        $('#nodeName' + id)
            .html(node.name + "<br/>");


    //update battery
    if (node.batteryLevel != null && $('#nodeBattery' + id).length == 0)
        $("<div></div>")
            .attr("id", "nodeBattery" + id).hide().fadeIn(elementsFadeTime)
            .appendTo('#footer' + id);

    if (node.batteryLevel == null && $('#nodeBattery' + id).length != 0)
        $('#nodeBattery' + id).remove();

    if (node.batteryLevel != null)
        $('#nodeBattery' + id)
            .html("Battery: " + node.batteryLevel + "<br/>");



    for (var i = 0; i < node.sensors.length; i++) {
            createOrUpdateSensor(node.sensors[i]);
    }
}


function updateLastSeen(node) {
    var id = node.nodeId;
    $('#activity' + id).show().fadeOut(500);
}

function updateBattery(node) {
    var id = node.nodeId;
    $('#nodeBattery' + id)
        .html("Battery: " + node.batteryLevel + "<br/>");
}


function createOrUpdateSensor(sensor) {

    var id = sensor.nodeId + "-" + sensor.sensorId;

    if ($('#sensorPanel' + id).length == 0) {
        //create new
        $('#sensorsContainer' + sensor.nodeId)
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

    var sensorId = sensor.nodeId + "-" + sensor.sensorId;

    for (var i = 0; i < sensorData.length; i++) {
        var data = sensorData[i];
        var id = sensor.nodeId + "-" + sensor.sensorId + "-" + data.dataType;


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


function updateDDMenuFromNode(node) {
    var id = node.nodeId;

    $('#dropdownMenuList' + id)
        .append("<li><a href='../Node/Settings/" + id + "'>Settings</a></li>")
        .append("<li><a href='#'> . . .</a></li>");

    for (var i = 0; i < node.sensors.length; i++) {
        addHistoryToDDMenu(node.sensors[i]);
    }

    $('#dropdownMenuList' + id).append("<li><a href='#'> . . .</a></li>");

    for (var i = 0; i < node.sensors.length; i++) {
        addTasksToDDMenu(node.sensors[i]);
    }

    $('#dropdownMenuList' + id).append("<li><a href='#'> . . .</a></li>");

    for (var i = 0; i < node.sensors.length; i++) {
        addLinksToDDMenu(node.sensors[i]);
    }
}

function addHistoryToDDMenu(sensor) {
    var id = sensor.nodeId + "-" + sensor.sensorId;

    var sensorType = Object.keys(mySensors.sensorTypeSimple)[sensor.sensorType];

    var sensorName;
    if (sensor.description != null)
        sensorName = sensor.description;
    else
        sensorName = sensorType;

    if ($('#dropdownMenuHistory' + id).length == 0) {
        $('#dropdownMenuList' + sensor.nodeId)
            .append("<li id='dropdownMenuHistory" + id + "'><a href='../History/Chart/" + sensor.nodeId + "/" + sensor.sensorId + "'>" + sensorName + " History</a></li>");
    } else {
        $('#dropdownMenuHistory' + id)
            .html("<a href='../History/Chart/" + sensor.nodeId + "/" + sensor.sensorId + "'>" + sensorName + " History</a>");
    }
}

function addTasksToDDMenu(sensor) {
    var id = sensor.nodeId + "-" + sensor.sensorId;

    var sensorType = Object.keys(mySensors.sensorTypeSimple)[sensor.sensorType];

    var sensorName;
    if (sensor.description != null)
        sensorName = sensor.description;
    else
        sensorName = sensorType;

    if ($('#dropdownMenuTasks' + id).length == 0) {
        $('#dropdownMenuList' + sensor.nodeId)
            .append("<li id='dropdownMenuTasks" + id + "'><a href='../Tasks/List/" + sensor.nodeId + "/" + sensor.sensorId + "'>" + sensorName + " Tasks</a></li>");
    } else {
        $('#dropdownMenuTasks' + id)
            .html("<a href='../Tasks/List/" + sensor.nodeId + "/" + sensor.sensorId + "'>" + sensorName + " Tasks</a>");
    }
}

function addLinksToDDMenu(sensor) {
    var id = sensor.nodeId + "-" + sensor.sensorId;

    var sensorType = Object.keys(mySensors.sensorTypeSimple)[sensor.sensorType];

    var sensorName;
    if (sensor.description != null)
        sensorName = sensor.description;
    else
        sensorName = sensorType;

    if ($('#dropdownMenuLinks' + id).length == 0) {
        $('#dropdownMenuList' + sensor.nodeId)
            .append("<li id='dropdownMenuLinks" + id + "'><a href='../Links/List/" + sensor.nodeId + "/" + sensor.sensorId + "'>" + sensorName + " Links</a></li>");
    } else {
        $('#dropdownMenuLinks' + id)
            .html("<a href='../Links/List/" + sensor.nodeId + "/" + sensor.sensorId + "'>" + sensorName + " Links</a>");
    }
}