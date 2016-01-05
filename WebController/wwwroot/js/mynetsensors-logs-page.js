/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/



var gatewayHardwareConnected = null;
var signalRServerConnected = null;

$(function () {
    //configure signalr
    var clientsHub = $.connection.clientsHub;

    //var logType set from ViewBag in View

    if (logType == "All" || logType == "GatewayState")
        clientsHub.client.OnGatewayStateLog = addMessage;
    if (logType == "All" || logType == "GatewayMessages")
        clientsHub.client.OnGatewayTxRxLog = addMessage;
    if (logType == "All" || logType == "GatewayRawMessages")
        clientsHub.client.OnGatewayRawTxRxLog = addMessage;
    if (logType == "All" || logType == "DataBaseState")
        clientsHub.client.OnDataBaseStateLog = addMessage;
    if (logType == "All" || logType == "LogicalNodesEngine")
        clientsHub.client.OnLogicalNodesEngineLog = addMessage;
    if (logType == "All" || logType == "LogicalNodes")
        clientsHub.client.OnLogicalNodesLog = addMessage;
    if (logType == "All" || logType == "Controller")
        clientsHub.client.OnSerialControllerLog = addMessage;


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
        $.ajax({
            url: "/Logs/ClearLogs/",
            data: { logType: logType },
            success: function () {
                $('#log').html();
            }
        });
    });

    getIsHardwareConnected();
    getLogs();



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

function getLogs() {
    $.ajax({
        url: "/Logs/GetLogs/",
        type: "POST",
        data: { logType: logType },
        success: function (messages) {
            onReturnMessages(messages);
        }
    });
}

function onReturnMessages(messages) {
    for (var i = 0; i < messages.length; i++) {
        $('#log').append(messages[i] + "<br/>");
    }
    $('#log').animate({ scrollTop: $('#log').get(0).scrollHeight }, 0);
};


function addMessage(message) {
    $('#log').append(message + "<br/>");
    $('#log').animate({ scrollTop: $('#log').get(0).scrollHeight }, 0);
};