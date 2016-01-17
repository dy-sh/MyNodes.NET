/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/



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
    if (gatewayInfo.isGatewayConnected) {
        $('#gateway-hardware-online').show();
        $('#gateway-hardware-offline').hide();
    } else {
        $('#gateway-hardware-online').hide();
        $('#gateway-hardware-offline').show();
    }

    $('#nodes-registered').html(gatewayInfo.gatewayNodesRegistered);
    $('#sensors-registered').html(gatewayInfo.gatewaySensorsRegistered);
}

$(document).ready(function () {


    $("#dropTasks").click(function () {
        $('#confirm-delete-tasks-dialog').modal({
            onApprove: function () {
                $.ajax({
                    type: "POST", url: "/GatewayAPI/RemoveAllTasks",
                    success: function (result) {
                        if (result) noty({ text: 'Tasks were deleted.' });
                    }
                });
            }
        })
        .modal('setting', 'transition', 'fade up').modal('show');
    });

    $("#dropNodes").click(function () {
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


    $("#disableTasks").click(function () {
        $.ajax({
            type: "POST", url: "/GatewayAPI/DisableTasks",
            success: function (result) {
                if (result) noty({ text: 'Tasks were disabled.' });
            }
        });
    });
});