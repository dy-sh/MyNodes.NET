/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/


var clientsHub;
var gatewayHardwareConnected = null;
var signalRServerConnected = null;

var elementsFadeTime = 300;


$(function () {
    clientsHub = $.connection.clientsHub;
    
    clientsHub.client.OnSensorUpdatedEvent = function (sensor) {
        OnSensorUpdatedEvent(sensor);
    };

    $.connection.hub.start();

    $.connection.hub.stateChanged(function (change) {
        if (change.newState === $.signalR.connectionState.reconnecting) {
            noty({ text: 'Web server is not responding!', type: 'error', timeout: false });
            signalRServerConnected = false;
        }
        else if (change.newState === $.signalR.connectionState.connected) {
            if (signalRServerConnected == false) {
                noty({ text: 'Connected to web server.', type: 'alert', timeout: false });
                getIsHardwareConnected();
            }
            signalRServerConnected = true;
        }
    });

    getIsHardwareConnected();
});


function getIsHardwareConnected() {
    $.ajax({
        url: "/GatewayAPI/IsHardwareConnected/",
        type: "POST",
        success: function (connected) {
            hardwareStateChanged(connected);
        }
    });
}


function hardwareStateChanged(connected) {
    if (connected && gatewayHardwareConnected === false) {
        noty({ text: 'Gateway hardware is online.', type: 'alert', timeout: false });
    } else if (!connected) {
        noty({ text: 'Gateway hardware is offline!', type: 'error', timeout: false });
    }

    gatewayHardwareConnected = connected;
}


function OnSensorUpdatedEvent(sensor) {
    if (sensor.nodeId != nodeId || sensor.sensorId != sensorId)
        return;

    var now = moment().format("DD.MM.YYYY H:mm:ss");
    $('#history-table').append("<tr><td>" + now + "</td><td>" + sensor.state+ "</td></tr>");
}

$('#clear-button').click(function() {
    $.ajax({
        url: "/History/ClearHistory/" + nodeId + "/" + sensorId,
        type: "POST",
        success: function (connected) {
            $('#history-table').html(null);
        }
    });
});