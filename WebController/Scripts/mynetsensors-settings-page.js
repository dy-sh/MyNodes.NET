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

var nodesCount;
var sensorsCount;


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
    };

    clientsHub.client.onClearNodesList = function () {
        var n = noty({ text: 'Nodes deleted from the database!', type: 'error' });
    };



   
    

    clientsHub.client.returnNodes = function (nodes) {
        onReturnNodes(nodes);
    };

    clientsHub.client.onNewNode = function (node) {
        nodesCount++;
        updateNodesAndSensorsCounts();
    };
    
    clientsHub.client.onNewSensor = function (sensor) {
        sensorsCount++;
        updateNodesAndSensorsCounts();
    };

 
    clientsHub.client.returnConnectedUsersCount = function (count) {
        $('#users-online').text(count);
    };

    clientsHub.client.returnGatewayInfo = function (gatewayInfo) {
        nodesCount = gatewayInfo.gatewayNodesRegistered;
        sensorsCount = gatewayInfo.gatewaySensorsRegistered;
        updateNodesAndSensorsCounts();
    };


    $.connection.hub.start().done(function () {
        clientsHub.server.getGatewayServiceConnected();
        clientsHub.server.getGatewayInfo();
        clientsHub.server.getConnectedUsersCount();
        setInterval(updateData, 1000);
    });

});


function updateData() {
    clientsHub.server.getConnectedUsersCount();
}




function gatewayStatusChanged() {
    if (gatewayHardwareConnected) {
        $('#gateway-hardware-online').html("<p class='text-success'>Gateway hardware is online <span class='glyphicon glyphicon-ok' aria-hidden='true'></span></p>");
    } else {
        $('#gateway-hardware-online').html("<p class='text-danger'>Gateway hardware is offline <span class='glyphicon glyphicon-remove' aria-hidden='true'></span></p>");
    }

    if (gatewayServiceConnected) {
        $('#gateway-service-online').html("<p class='text-success'>Gateway service is online <span class='glyphicon glyphicon-ok' aria-hidden='true'></span></p>");
    } else {
        $('#gateway-service-online').html("<p class='text-danger'>Gateway service is offline <span class='glyphicon glyphicon-remove' aria-hidden='true'></span></p>");
    }

    if (gatewayHardwareConnected && gatewayServiceConnected)
        $('#gateway-info').fadeIn(300);
    else
        $('#gateway-info').fadeOut(300);
}



function onReturnNodes(nodes) {
    $('#nodesContainer').html(null);

    nodesCount = 0;
    if (nodes != null) nodesCount = nodes.length;

    sensorsCount = 0;
    if (nodes != null) {
        for (var i = 0; i < nodes.length; i++) {
            sensorsCount += nodes[i].sensors.length;
        }
    }
    updateNodesAndSensorsCounts();
}

function updateNodesAndSensorsCounts() {
    $('#nodes-registered').html(nodesCount);
    $('#sensors-registered').html(sensorsCount);
}