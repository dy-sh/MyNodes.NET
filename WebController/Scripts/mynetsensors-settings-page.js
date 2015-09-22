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

var nodesCount;
var sensorsCount;

var gatewayHardwareConnected = false;
var gatewayServiceConnected = false;

$(function () {



    gatewayHub = $.connection.gatewayHub;

    gatewayHub.client.returnNodes = function (nodes) {
        onReturnNodes(nodes);
    };

    gatewayHub.client.onNewNodeEvent = function (node) {
        nodesCount++;
        updateNodesAndSensorsCounts();
    };
    
    gatewayHub.client.onNewSensorEvent = function (sensor) {
        sensorsCount++;
        updateNodesAndSensorsCounts();
    };

    gatewayHub.client.onGatewayHardwareDisconnected = function () {
        onGatewayHardwareDisconnected();
    };

    gatewayHub.client.onGatewayHardwareConnected = function () {
        onGatewayHardwareConnected();
    };

    gatewayHub.client.onGatewayServiceDisconnected = function () {
        onGatewayServiceDisconnected();
    };

    gatewayHub.client.onGatewayServiceConnected = function () {
        onGatewayServiceConnected();
    };

    gatewayHub.client.onClearNodesListEvent = function (sensor) {
        onClearNodesList();
    };


 
    gatewayHub.client.updateUsersOnlineCount = function (count) {
        // Add the message to the page. 
        $('#users-online').text(count);
    };


    $.connection.hub.start().done(function () {
        $.get("GetNodes/");
    });

});

function onClearNodesList() {
    var n = noty({
        text: 'Nodes deleted from the database!',
        type: 'error'
    });
    
}

function onGatewayHardwareDisconnected() {
    var n = noty({
        text: 'Gateway hardware offline!',
        type: 'error',
        timeout: false
    });
    gatewayHardwareConnected = false;
    gatewayStatusChanged();
}

function onGatewayHardwareConnected() {
    var n = noty({
        text: 'Gateway hardware online.',
        type: 'alert',
        timeout: false
    });
    gatewayHardwareConnected = true;
    gatewayStatusChanged();
}

function onGatewayServiceDisconnected() {
    var n = noty({
        text: 'Gateway service offline!',
        type: 'error',
        timeout: false
    });
    gatewayServiceConnected = false;
    gatewayStatusChanged();
}

function onGatewayServiceConnected() {
    var n = noty({
        text: 'Gateway service online.',
        type: 'alert',
        timeout: false
    });
    gatewayServiceConnected = true;
    gatewayStatusChanged();
}

function gatewayStatusChanged() {
    if (gatewayHardwareConnected) {
        $('#gateway-info').fadeIn(300);
        $('#gateway-online').html("<p class='text-success'>Gateway hardware is online</p>");
    } else {
        $('#gateway-info').fadeOut(300);
        $('#gateway-online').html("<p class='text-danger'>Gateway hardware is offline</p>");
    }
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