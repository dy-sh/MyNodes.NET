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

    gatewayHub.client.onGatewayDisconnectedEvent = function () {
        onGatewayDisconnected();
    };

    gatewayHub.client.onGatewayConnectedEvent = function () {
        onGatewayConnected();
    };

    gatewayHub.client.onClearNodesListEvent = function (sensor) {
        onClearNodesList();
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

function onGatewayDisconnected() {
    var n = noty({
        text: 'Gateway disconnected!',
        type: 'error',
        timeout: false
    });
    $('#gateway-online').html("<p class='text-danger'>Gateway is offline</p>");
    $('#gateway-info').fadeOut(300);
}

function onGatewayConnected() {
    var n = noty({
        text: 'Gateway connected.',
        type: 'alert',
        timeout: false
    });
    $('#gateway-online').html("<p class='text-success'>Gateway is online</p>");
    $('#gateway-info').fadeIn(300);
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