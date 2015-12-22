/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/



$(function () {
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

$(document).ready(function(){
    $("#dropLinks").click(function(){
        $.ajax({type: "POST", url: "/GatewayAPI/DropLinks",
            success:function(result) {
                if (result) noty({ text: 'Sensors links were deleted.' });
            }});
    });

    $("#dropHistory").click(function () {
        $.ajax({
            type: "POST", url: "/GatewayAPI/DropHistory",
            success: function (result) {
                if (result) noty({ text: 'History was deleted.' });
            }
        });
    });

    $("#dropTasks").click(function () {
        $.ajax({
            type: "POST", url: "/GatewayAPI/DropTasks",
            success: function (result) {
                if (result) noty({ text: 'Tasks were deleted.' });
            }
        });
    });

    $("#dropNodes").click(function () {
        $.ajax({
            type: "POST", url: "/GatewayAPI/DropNodes",
            success: function (result) {
                if (result) noty({ text: 'Nodes were deleted.' });
            }
        });
    });

    $("#stopWritingHistory").click(function () {
        $.ajax({
            type: "POST", url: "/GatewayAPI/StopWritingHistory",
            success: function (result) {
                if (result) noty({ text: 'History writing is stopped.' });
            }
        });
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