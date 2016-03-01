/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/


//window.this_panel_id initialized from ViewBag



var signalRServerConnected;





$(function () {

    //configure signalr
    var clientsHub = $.connection.nodesEngineHub;

    clientsHub.client.OnConnected = function () {
        noty({ text: 'Gateway is connected.', type: 'alert', timeout: false });
    };

    clientsHub.client.OnDisconnected = function () {
        noty({ text: 'Gateway is disconnected!', type: 'error', timeout: false });
    };

    clientsHub.client.OnRemoveAllNodesAndLinks = function () {
        window.location.replace("/Dashboard/");
        noty({ text: 'All nodes have been deleted!', type: 'error' });
        //$('#panelsContainer').empty();
    };


  

    clientsHub.client.OnNewUiNode = function (node) {
        if (this_panel_id != null && this_panel_id != "") {
            if (node.PanelId != this_panel_id)
                return;
        } else if (node.Settings["ShowOnMainPage"].Value!="true")
            return;

        createNode(node);
    };

    clientsHub.client.OnUiNodeUpdated = function (node) {
        if (this_panel_id != null && this_panel_id != "") {
            if (node.PanelId != this_panel_id)
                return;
        } else if (node.Settings["ShowOnMainPage"].Value != "true") {
            //if ShowOnMainPage changed to false
            if ($('#node-' + node.Id).length != 0)
                removeNode(node);
            return;
        }

        updateNode(node);
    };


    clientsHub.client.OnPanelNodeUpdated = function (node) {
        updatePanel(node);
    };


    clientsHub.client.OnRemoveUiNode = function (node) {
        if (this_panel_id != null && this_panel_id != "") {
            if (node.PanelId != this_panel_id)
                return;
        } else if (node.Settings["ShowOnMainPage"].Value != "true")
            return;

        removeNode(node);
    };


    $.connection.hub.start();

    $.connection.hub.stateChanged(function (change) {
        if (change.newState === $.signalR.connectionState.reconnecting) {
            $("#panelsContainer").fadeOut(300);
            noty({ text: 'Web server is not responding!', type: 'error' });
            signalRServerConnected = false;
        }
        else if (change.newState === $.signalR.connectionState.connected) {
            if (signalRServerConnected == false) {
                noty({ text: 'Connected to web server.', type: 'alert' });
                //waiting while server initialized and read db
                setTimeout(function () {
                    $("#panelsContainer").empty();
                    $("#panelsContainer").show();
                    getNodes();
                    getGatewayInfo();
                }, 2000);


            }
            signalRServerConnected = true;
        }
    });

    // var connection = $.connection(clientsHub);
    // connection.stateChanged(signalrConnectionStateChanged);
    //connection.start({ waitForPageLoad: true });

    getNodes();
    getGatewayInfo();
});


function getGatewayInfo() {
    $.ajax({
        url: "/GatewayAPI/GetGatewayInfo/",
        type: "POST",
        success: function (gatewayInfo) {
            if (gatewayInfo.state == 1 || gatewayInfo.state == 2) {
                noty({ text: 'Gateway is not connected!', type: 'error', timeout: false });
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
    $("#panelsContainer").empty();

    if (!nodes || nodes.length == 0) {
        $('#empty-message').show();
        return;
    }

    for (var i = 0; i < nodes.length; i++) {
        createNode(nodes[i]);
    }
}

