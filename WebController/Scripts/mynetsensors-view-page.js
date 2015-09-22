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

var gatewayHub;
var gatewayHardwareConnected = false;
var gatewayServiceConnected = false;


$(function () {
    gatewayHub = $.connection.gatewayHub;

    gatewayHub.client.onGatewayHardwareConnected = function () {
        var n = noty({ text: 'Gateway hardware is online.', type: 'alert', timeout: false });
        gatewayHardwareConnected = true;
        gatewayStatusChanged();
    };

    gatewayHub.client.onGatewayHardwareDisconnected = function () {
        var n = noty({ text: 'Gateway hardware is offline!', type: 'error', timeout: false });
        gatewayHardwareConnected = false;
        gatewayStatusChanged();
    };

    gatewayHub.client.onGatewayServiceConnected = function () {
        var n = noty({ text: 'Gateway service is online.', type: 'alert', timeout: false });
        gatewayServiceConnected = true;
        gatewayStatusChanged();
    };

    gatewayHub.client.onGatewayServiceDisconnected = function () {
        var n = noty({ text: 'Gateway service is offline!', type: 'error', timeout: false });
        gatewayServiceConnected = false;
        gatewayStatusChanged();
    };

    gatewayHub.client.onClearNodesListEvent = function (sensor) {
        var n = noty({ text: 'Nodes deleted from the database!', type: 'error' });
        $('#nodesContainer').html(null);
    };


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

   
    $.connection.hub.start().done(function () {
        $.get("GetNodes/");
    });

});





function gatewayStatusChanged() {
    if (gatewayHardwareConnected && gatewayServiceConnected)
        $('#nodesContainer').fadeIn(300);
    else
        $('#nodesContainer').fadeOut(300);
}

function onReturnNodes(nodes) {
    $('#nodesContainer').html(null);

    for (var i = 0; i < nodes.length; i++) {
        createOrUpdateNode(nodes[i]);
    }
}

function createOrUpdateNode(node) {
    var id = node.nodeId;

    if ($('#nodePanel' + id).length == 0) {
        //create new
        $('#nodeTemplate')
            .clone()
            .attr("id", "nodePanel" + id)
            //.css('display', 'block')
            .fadeIn(1000)
            .appendTo('#nodesContainer');

        $('#nodePanel' + id)
            .find('#nodeTitle')
            .attr("id", "nodeTitle" + id)
            .html("Node id: " + id);

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
            .append("Name: " + node.name + "<br/>");

    if (node.version != null)
        $('#nodeBody' + id)
            .append("Version: " + node.version + "<br/>");

    $('#nodeBody' + id)
        .append("Reg.: " + moment(node.registered).format("DD/MM/YYYY HH:mm:ss") + "<br/>")
        .append("<div id='nodeLastSeen" + id + "'>"
            + "Seen: " + moment(node.lastSeen).format("DD/MM/YYYY HH:mm:ss")
            + "</div>");

    if (node.isRepeatingNode == null)
        $('#nodeBody' + id)
            .append("Repeating: Unknown <br/>");
    else if (node.isRepeatingNode)
        $('#nodeBody' + id)
            .append("Repeating: Yes <br/>");
    else if (!node.isRepeatingNode)
        $('#nodeBody' + id)
            .append("Repeating: No <br/>");

    if (node.batteryLevel != null)
        $('#nodeBody' + id)
            .append("<div id='nodeBattery" + id + "'>"
                + "Battery: " + node.batteryLevel
                + "</div>");

    for (var i = 0; i < node.sensors.length; i++) {
        createOrUpdateSensor(node.sensors[i]);
    }
}


function updateLastSeen(node) {
    var id = node.nodeId;
    $('#nodeLastSeen' + id)
        .html("Seen: " + moment(node.lastSeen).format("DD/MM/YYYY HH:mm:ss"));
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
                + "</li>");
    }

    //update body
    $('#sensorPanel' + id)
        .html("Sensor Id: " + sensor.sensorId + "<br/>");

    var sensorType = Object.keys(mySensors.sensorType)[sensor.sensorType];
    if (sensorType == null) sensorType = "Unknown";
    $('#sensorPanel' + id)
        .append("Type: " + sensorType + "<br/>");

    if (sensor.description != null)
        $('#sensorPanel' + id)
            .append("Description: " + sensor.description + "<br/>");

    $('#sensorPanel' + id)
        .append("<hr/>");

    createOrUpdateSensorData(sensor);
}


function createOrUpdateSensorData(sensor) {

    var sensorData = JSON.parse(sensor.sensorDataJson);

    var sensorId = sensor.ownerNodeId + "-" + sensor.sensorId;

    for (var i = 0; i < sensorData.length; i++) {
        var data = sensorData[i];
        var id = sensor.ownerNodeId + "-" + sensor.sensorId + "-" + data.dataType;

        if ($('#dataPanel' + id).length == 0) {
            //create new
            $('#sensorPanel' + sensorId)
                .append("<div id='dataPanel" + id + "'>"
                    + "</div>");
        }

        var dataState = data.state;
        if (dataState == "" || dataState == null)
            dataState = "null";

        //update body

        var dataType = Object.keys(mySensors.sensorDataType)[data.dataType];

        $('#dataPanel' + id)
            .html(dataType + " : " + dataState + "<br/>");
    }
}