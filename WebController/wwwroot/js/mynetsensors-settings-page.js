/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/



$(function () {
    //clientsHub = $.connection.clientsHub;

    //clientsHub.client.onGatewayConnected = function () {
    //    var n = noty({ text: 'Gateway hardware is online.', type: 'alert', timeout: false });
    //    gatewayHardwareConnected = true;
    //    gatewayStatusChanged();
    //};

    //clientsHub.client.onGatewayDisconnected = function () {
    //    var n = noty({ text: 'Gateway hardware is offline!', type: 'error', timeout: false });
    //    gatewayHardwareConnected = false;
    //    gatewayStatusChanged();
    //};

    //clientsHub.client.onGatewayServiceConnected = function () {
    //    var n = noty({ text: 'Gateway service is online.', type: 'alert', timeout: false });
    //    gatewayServiceConnected = true;
    //    gatewayStatusChanged();
    //    clientsHub.server.getGatewayHardwareConnected();
    //};

    //clientsHub.client.onGatewayServiceDisconnected = function () {
    //    var n = noty({ text: 'Gateway service is offline!', type: 'error', timeout: false });
    //    gatewayServiceConnected = false;
    //    gatewayHardwareConnected = false;
    //    gatewayStatusChanged();
    //};

    //clientsHub.client.returnGatewayServiceConnected = function (isConnected) {
    //    gatewayServiceConnected = isConnected;
    //    gatewayStatusChanged();
    //    clientsHub.server.getGatewayHardwareConnected();
    //};


    //clientsHub.client.returnGatewayHardwareConnected = function (isConnected) {
    //    gatewayHardwareConnected = isConnected;
    //    gatewayStatusChanged();
    //};

    //clientsHub.client.onClearNodesList = function () {
    //    var n = noty({ text: 'Nodes deleted from the database!', type: 'error' });
    //};



   
    

    //clientsHub.client.returnNodes = function (nodes) {
    //    onReturnNodes(nodes);
    //};

    //clientsHub.client.onNewNode = function (node) {
    //    nodesCount++;
    //    updateNodesAndSensorsCounts();
    //};
    
    //clientsHub.client.onNewSensor = function (sensor) {
    //    sensorsCount++;
    //    updateNodesAndSensorsCounts();
    //};

 
    //clientsHub.client.returnConnectedUsersCount = function (count) {
    //    $('#users-online').text(count);
    //};

    //clientsHub.client.returnGatewayInfo = function (gatewayInfo) {
    //    nodesCount = gatewayInfo.gatewayNodesRegistered;
    //    sensorsCount = gatewayInfo.gatewaySensorsRegistered;
    //    updateNodesAndSensorsCounts();
    //};


    //$.connection.hub.start().done(function () {
    //    clientsHub.server.getGatewayServiceConnected();
    //    clientsHub.server.getGatewayInfo();
    //    clientsHub.server.getConnectedUsersCount();
    //    
    //});

    getGatewayInfo();
    setInterval(getGatewayInfo, 5000);
});



function getGatewayInfo() {
    $.ajax({
        url: "/GatewayAPI/GetGatewayInfo/",
        type: "POST",
        success: function (gatewayInfo) {
            updateInfo(gatewayInfo);
        },
        error: function () {
            $('#gateway-hardware-online').html("<p class='text-danger'>Web server is not responding <span class='glyphicon glyphicon-remove' aria-hidden='true'></span></p>");
        }
    });
}



function updateInfo(gatewayInfo) {
    if (gatewayInfo.isGatewayConnected) {
        $('#gateway-hardware-online').html("<p class='text-success'>Gateway hardware is online <span class='glyphicon glyphicon-ok' aria-hidden='true'></span></p>");
    } else {
        $('#gateway-hardware-online').html("<p class='text-danger'>Gateway hardware is offline <span class='glyphicon glyphicon-remove' aria-hidden='true'></span></p>");
    }

    $('#nodes-registered').html(gatewayInfo.gatewayNodesRegistered);
    $('#sensors-registered').html(gatewayInfo.gatewaySensorsRegistered);
}

