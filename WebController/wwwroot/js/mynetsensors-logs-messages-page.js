/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/



var gatewayHardwareConnected = null;
var signalRServerConnected = null;

$(function () {
    //configure signalr
    var clientsHub = $.connection.clientsHub;


    clientsHub.client.OnMessageRecievedEvent = function (message) {
        $('#log').append(message + "<br/>");
        $('#log').animate({ scrollTop: $('#log').get(0).scrollHeight }, 0);
    };

    clientsHub.client.OnMessageSendEvent = function (message) {
        $('#log').append(message + "<br/>");
        $('#log').animate({ scrollTop: $('#log').get(0).scrollHeight }, 0);
    };

    clientsHub.client.OnConnectedEvent = function () {
        hardwareStateChanged(true);
    };

    clientsHub.client.OnDisconnectedEvent = function () {
        hardwareStateChanged(false);
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


    //clear messages button
    $('#clear-log').on('click', function () {
        $('#log').html("");
        $.ajax({ url: "/GatewayAPI/ClearMessages/" });
    });

    getIsHardwareConnected();
    getLog();



    //autoscroll
    $(function () {
        var window_height = $(window).height(),
            content_height = window_height - 370;
        $('#log').height(content_height);
    });

    $(window).resize(function () {
        var window_height = $(window).height(),
            content_height = window_height - 370;
        $('#log').height(content_height);
    });
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

function getLog() {
    $.ajax({
        url: "/GatewayAPI/GetMessages/",
        type: "POST",
        success: function (messages) {
            onReturnMessages(messages);
        }
    });
}

function onReturnMessages(messages) {
    $('#log').html(messages);
    $('#log').animate({ scrollTop: $('#log').get(0).scrollHeight }, 0);
};
