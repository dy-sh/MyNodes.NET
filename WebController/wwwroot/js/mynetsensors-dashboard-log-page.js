/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/


var clientsHub;
var signalRServerConnected;

var elementsFadeTime = 300;


$(function () {
    //configure signalr
    var clientsHub = $.connection.nodesEngineHub;

    clientsHub.client.OnConnectedEvent = function () {
        noty({ text: 'Serial Gateway is connected.', type: 'alert', timeout: false });
    };

    clientsHub.client.OnDisconnectedEvent = function () {
        noty({ text: 'Serial Gateway is disconnected!', type: 'error', timeout: false });
    };

    clientsHub.client.OnRemoveAllNodesEvent = function () {
        var n = noty({ text: 'Nodes deleted from the database!', type: 'error' });
        $('#panelsContainer').html(null);
    };




    clientsHub.client.OnNodeUpdatedEvent = function (node) {
        if (node.Id == nodeId)//nodeId initialized from ViewBag
            updateLog(node);
    };

    clientsHub.client.OnRemoveNodeEvent = function (node) {
        if (node.Id == nodeId) {
            var n = noty({ text: 'This Node was removed!', type: 'error', timeout: false });
        }
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
                getNodes();
                getGatewayInfo();
            }
            signalRServerConnected = true;
        }
    });

    // var connection = $.connection(clientsHub);
    // connection.stateChanged(signalrConnectionStateChanged);
    //connection.start({ waitForPageLoad: true });

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


function updateLog(node) {

    var now = moment().format("DD.MM.YYYY H:mm:ss");
    $('#history-table').append("<tr><td>" + now + "</td><td>" + node.State + "</td></tr>");
}

$('#clear-button').click(function () {
    $.ajax({
        url: "/DashboardAPI/ClearLog/",
        type: "POST",
        data: { nodeId: nodeId },
        success: function (connected) {
            $('#history-table').html(null);
        }
    });
});