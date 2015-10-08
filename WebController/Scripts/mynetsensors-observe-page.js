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

var lastSeens;

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
        lastSeens[node.nodeId] = node.lastSeen;
        updateLastSeen(node.nodeId, node.lastSeen);
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

    setInterval(updateAllLastSeens, 1000);
});





function gatewayStatusChanged() {
    if (gatewayHardwareConnected && gatewayServiceConnected)
        $('#nodesContainer').fadeIn(800);
    else
        $('#nodesContainer').fadeOut(800);
}

function onReturnNodes(nodes) {
    $('#nodesContainer').html(null);

    for (var i = 0; i < nodes.length; i++) {
        createOrUpdateNode(nodes[i]);
    }

    lastSeens = {};

    for (var i = 0; i < nodes.length; i++) {
        lastSeens[nodes[i].nodeId] = nodes[i].lastSeen;
    }
}



var nodeTemplate = Handlebars.compile($('#nodeTemplate').html());
var sensorTemplate = Handlebars.compile($('#sensorTemplate').html());
var dataTemplate = Handlebars.compile($('#dataTemplate').html());

Handlebars.registerHelper("fullDate", function (datetime) {
    return moment(datetime).format("DD/MM/YYYY HH:mm:ss");
});

Handlebars.registerHelper("yes-no", function (boolean) {
    if (boolean == null)
        return "Unknown";
    else if (boolean)
        return "Yes";
    else
        return "No";
});

Handlebars.registerHelper("sensor-id", function (sensor) {
    return sensor.nodeId + "-" + sensor.sensorId;
});

Handlebars.registerHelper("sensordata-id", function (data) {
    return data.nodeId + "-" + data.sensorId + "-" + data.dataType;
});

Handlebars.registerHelper("sensor-type", function (sensor) {
    var type = Object.keys(mySensors.sensorType)[sensor.sensorType];
    if (type == null)
        type = "Unknown";
    return type;
});




function createOrUpdateNode(node) {
    var id = node.nodeId;

    var nodePanel = $('#nodePanel' + id);

    if (nodePanel.length == 0) {
        //create new
        $(nodeTemplate(node)).hide().appendTo("#nodesContainer").fadeIn(1000);
    } else {
        //update
        nodePanel.html(nodeTemplate(node));
    }

    for (var i = 0; i < node.sensors.length; i++) {
        createOrUpdateSensor(node.sensors[i]);
    }
}


function updateBattery(node) {
    var nodeBattery = $('#nodeBattery' + node.nodeId);

    if (nodeBattery.length == 0)
        createOrUpdateNode(node);
    else nodeBattery.html(node.batteryLevel);
}


function createOrUpdateSensor(sensor) {
    var id = sensor.nodeId + "-" + sensor.sensorId;

    if ($('#sensorPanel' + id).length == 0) {
        //create new
        $(sensorTemplate(sensor)).hide().appendTo("#sensorsContainer" + sensor.nodeId).fadeIn(1000);
    }

    //update body
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
            $(dataTemplate(data)).hide().appendTo("#sensorPanel" + sensorId).fadeIn(1000);
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

function updateLastSeen(nodeId, lastSeen) {

    var date1 = new Date(lastSeen);
    var date2 = new Date();
    var diff = Math.abs(date2.getTime() - date1.getTime());
    var days = Math.floor(diff / (1000 * 60 * 60 * 24));

    diff -= days * (1000 * 60 * 60 * 24);

    var hours = Math.floor(diff / (1000 * 60 * 60));
    diff -= hours * (1000 * 60 * 60);

    var mins = Math.floor(diff / (1000 * 60));
    diff -= mins * (1000 * 60);

    var seconds = Math.floor(diff / (1000));
    diff -= seconds * (1000);

    var elspsed;
    if (days != 0)
        elapsed = days + "d " + hours + "h " + mins + "m " + seconds + "s";
    else if (hours != 0)
        elapsed = hours + "h " + mins + "m " + seconds + "s";
    else if (mins != 0)
        elapsed = mins + "m " + seconds + "s";
    else
        elapsed = seconds + "s";

    $('#nodeLastSeen' + nodeId)
        .html(elapsed);

}

function updateAllLastSeens(sensor) {
    for (var key in lastSeens) {
        updateLastSeen(key, lastSeens[key]);
    }
}

