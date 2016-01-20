/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

var LogRecord = {
    LogRecordOwner:
    {
        Gateway:0,
        Node:1,
        DataBase:2,
        LogicalNodesEngine:3,
        LogicalNode:4,
        SerialController:5

    },

    LogRecordType:
    {
        Info:0,
        Error:1
    }
}



var signalRServerConnected;

$(function () {
    //configure signalr
    var clientsHub = $.connection.clientsHub;

    //var logType set from ViewBag in View

    clientsHub.client.OnLogRecord = OnLogRecord;


    clientsHub.client.OnConnectedEvent = function () {
        noty({ text: 'Serial Gateway is connected.', type: 'alert', timeout: false });
    };

    clientsHub.client.OnDisconnectedEvent = function () {
        noty({ text: 'Serial Gateway is disconnected!', type: 'error', timeout: false });
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
        $.ajax({
            url: "/Logs/ClearLogs/",
            data: { logType: logType },
            success: function () {
                $('#log').html();
            }
        });
    });





    //autoscroll
    $(function () {
        var window_height = $(window).height(),
            content_height = window_height - 320;
        $('#log').height(content_height);
    });

    $(window).resize(function () {
        var window_height = $(window).height(),
            content_height = window_height - 320;
        $('#log').height(content_height);
    });


    getLogs();
    getGatewayInfo();
});


function getGatewayInfo() {
    $.ajax({
        url: "/GatewayAPI/GetGatewayInfo/",
        type: "POST",
        success: function (gatewayInfo) {
            if (gatewayInfo.state == 1 || gatewayInfo.state == 2) {
                noty({ text: 'Serial Gateway is not connected!', type: 'error', timeout: false });
            }
        }
    });
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

function onReturnMessages(logRecords) {
    for (var i = 0; i < logRecords.length; i++) {
        addRecord(logRecords[i]);
    }
    $('#log').animate({ scrollTop: $('#log').get(0).scrollHeight }, 0);
};


function OnLogRecord(logRecord) {
    addRecord(logRecord);
    $('#log').animate({ scrollTop: $('#log').get(0).scrollHeight }, 0);
};


function addRecord(logRecord) {
    if (logType == "AllErrors" && logRecord.Type != LogRecord.LogRecordType.Error)
        return;
    if (logType == "Controller" && logRecord.Owner != LogRecord.LogRecordOwner.SerialController)
        return;
    if (logType == "Gateway" && logRecord.Owner != LogRecord.LogRecordOwner.Gateway)
        return;
    if (logType == "GatewayMessages" && logRecord.Owner != LogRecord.LogRecordOwner.Node)
        return;
    if (logType == "LogicalNodes" && logRecord.Owner != LogRecord.LogRecordOwner.LogicalNode)
        return;
    if (logType == "LogicalNodesEngine" && logRecord.Owner != LogRecord.LogRecordOwner.LogicalNodesEngine)
        return;
    if (logType == "DataBase" && logRecord.Owner != LogRecord.LogRecordOwner.DataBase)
        return;

    if (logType == "All")
        addOwner(logRecord);

    addDate(logRecord);



    if (logRecord.Type == LogRecord.LogRecordType.Error) {
        $('#log').append("<span class='error-log-message'>" + logRecord.Message + "</span><br/>");
    } else {
        $('#log').append( logRecord.Message+"<br/>");
    }
};

function addOwner(logRecord) {
    switch (logRecord.Owner)
    {
        case LogRecord.LogRecordOwner.Gateway:
            logRecord.Message= "GATEWAY: " + logRecord.Message;
            break;
        case LogRecord.LogRecordOwner.Node:
            logRecord.Message = "GATEWAY: " + logRecord.Message;
            break;
        case LogRecord.LogRecordOwner.DataBase:
            logRecord.Message = "DATABASE: " + logRecord.Message;
            break;
        case LogRecord.LogRecordOwner.LogicalNodesEngine:
            logRecord.Message = "LOGICAL NODES ENGINE: " + logRecord.Message;
            break;
        case LogRecord.LogRecordOwner.LogicalNode:
            logRecord.Message = "LOGICAL NODE: " + logRecord.Message;
            break;
        case LogRecord.LogRecordOwner.SerialController:
            logRecord.Message = "CONTROLLER: " + logRecord.Message;
            break;
    }
}

function addDate(logRecord) {
    logRecord.Message = moment(logRecord.Date).format("DD.MM.YYYY H:mm:ss") + ": " + logRecord.Message;
    return logRecord;
}