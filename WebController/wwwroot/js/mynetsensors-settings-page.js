/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/


var gatewayHardwareConnected = null;


$(function () {
    getGatewayInfo();
    setInterval(getGatewayInfo, 2000);
});



function getGatewayInfo() {
    $.ajax({
        url: "/GatewayAPI/GetGatewayInfo/",
        type: "POST",
        success: function (gatewayInfo) {
            $('#main-content').show();
            updateInfo(gatewayInfo);
        },
        error: function () {
            $('#main-content').hide();
        }
    });
}



function updateInfo(gatewayInfo) {
    gatewayHardwareConnected = gatewayInfo.isGatewayConnected;

    if (gatewayHardwareConnected) {
        $('#gateway-hardware-online').show();
        $('#gateway-hardware-offline').hide();
        $("#serial-gateway-connect").html('Disconnect');
    } else {
        $('#gateway-hardware-online').hide();
        $('#gateway-hardware-offline').show();
        $("#serial-gateway-connect").html('Connect');
    }

    $('#nodes-registered').html(gatewayInfo.gatewayNodesRegistered);
    $('#sensors-registered').html(gatewayInfo.gatewaySensorsRegistered);
}

$(document).ready(function () {


    $("#serial-gateway-delete-nodes").click(function () {
        $('#confirm-delete-nodes-dialog').modal({
            onApprove: function () {
                $.ajax({
                    type: "POST", url: "/GatewayAPI/RemoveAllNodes",
                    success: function (result) {
                        if (result) noty({ text: 'Nodes were deleted.' });
                    }
                });
            }
        }).modal('setting', 'transition', 'fade up').modal('show');
    });

    $("#serial-gateway-connect").click(function () {
        if (!gatewayHardwareConnected) {
            $.ajax({
                type: "POST",
                url: "/GatewayAPI/Connect/"
            });
        } else {
            $.ajax({
                type: "POST",
                url: "/GatewayAPI/Disconnect/"
            });
        }
    });

});