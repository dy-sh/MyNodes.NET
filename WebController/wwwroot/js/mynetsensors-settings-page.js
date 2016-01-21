/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/



$(function () {
    getGatewayInfo();
    setInterval(getGatewayInfo, 1000);
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
    $("#serial-gateway-state").removeClass("blue");
    $("#serial-gateway-state").removeClass("red");

    switch (gatewayInfo.state) {
        case 0:
            $("#serial-gateway-state").html('Disconnected');
            $("#serial-gateway-state").addClass('red');
            $("#serial-gateway-connect").show();
            $("#serial-gateway-disconnect").hide();
            break;
        case 1:
            $("#serial-gateway-state").html('Connecting to port...');
            $("#serial-gateway-state").addClass('red');
            $("#serial-gateway-connect").hide();
            $("#serial-gateway-disconnect").show();
            break;
        case 2:
            $("#serial-gateway-state").html('Connecting to gateway...');
            $("#serial-gateway-state").addClass('red');
            $("#serial-gateway-connect").hide();
            $("#serial-gateway-disconnect").show();
            break;
        case 3:
            $("#serial-gateway-state").html('Connected');
            $("#serial-gateway-state").addClass('blue');
            $("#serial-gateway-connect").hide();
            $("#serial-gateway-disconnect").show();
            break;
        default:
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
        $.ajax({
            type: "POST",
            url: "/Config/ConnectSerialController/",
            success:function() {

            }
        });
    });

    $("#serial-gateway-disconnect").click(function () {
        $.ajax({
            type: "POST",
            url: "/Config/DisconnectSerialController/",
            success: function () {

            }
        });
    });

});