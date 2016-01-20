/*  MyNetSensors 
    Copyright (C) 2015-2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/


//window.this_panel_id initialized from ViewBag



var signalRServerConnected;





$(function () {

    //configure signalr
    var clientsHub = $.connection.clientsHub;

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


    clientsHub.client.OnNewUINodeEvent = function (node) {
        if (this_panel_id != null && this_panel_id != "") {
            if (node.PanelId != this_panel_id)
                return;
        } else if (!node.ShowOnMainPage)
            return;

        createNode(node);
    };

    clientsHub.client.OnUINodeUpdatedEvent = function (node) {
        if (this_panel_id != null && this_panel_id != "") {
            if (node.PanelId != this_panel_id)
                return;
        } else if (!node.ShowOnMainPage) {
            //if ShowOnMainPage changed to false
            if ($('#node-' + node.Id).length != 0)
                removeNode(node);
            return;
        }

        updateNode(node);
    };

    clientsHub.client.OnUINodeRemoveEvent = function (node) {
        if (this_panel_id != null && this_panel_id != "") {
            if (node.PanelId != this_panel_id)
                return;
        } else if (!node.ShowOnMainPage)
            return;

        removeNode(node);
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
                getNodes();
            }
            signalRServerConnected = true;
        }
    });

    // var connection = $.connection(clientsHub);
    // connection.stateChanged(signalrConnectionStateChanged);
    //connection.start({ waitForPageLoad: true });

    getGatewayInfo();
    getNodes();
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






function getNodes() {
    if (window.this_panel_id == null || window.this_panel_id == "") {
        $.ajax({
            url: "/DashboardAPI/GetUINodesForMainPage/",
            type: "POST",
            success: function (nodes) {
                onReturnNodes(nodes);
            }
        });
    } else {
        $.ajax({
            url: "/DashboardAPI/GetUINodesForPanel/",
            type: "POST",
            data: { 'panelId': window.this_panel_id },
            success: function (nodes) {
                onReturnNodes(nodes);
            }
        });
    }
}

function onReturnNodes(nodes) {
    if (!nodes || nodes.length == 0) {
        $('#empty-message').show();
        return;
    }

    for (var i = 0; i < nodes.length; i++) {
        createNode(nodes[i]);
    }
}

