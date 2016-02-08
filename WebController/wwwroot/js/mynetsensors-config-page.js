/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

var REFRESH_DELAY = 2000;

$(function () {
    getGatewayInfo();
    getNodesEngineInfo();
    getWebServerInfo();
});


$(document).ready(function () {


    $("#gateway-delete-nodes").click(function () {
        $('#gateway-delete-nodes-nonfirm').modal({
            onApprove: function () {
                $.ajax({
                    type: "POST", url: "/GatewayAPI/RemoveAllNodes",
                    success: function (result) {
                        if (result) noty({ text: 'Nodes have been deleted.' });
                    }
                });
            }
        }).modal('setting', 'transition', 'fade up').modal('show');
    });

    $("#serial-gateway-connect").click(function () {
        $.ajax({
            type: "POST",
            url: "/Config/ConnectSerialGateway/",
            success: function () {
                getGatewayInfo();
            }
        });
    });

    $("#serial-gateway-disconnect").click(function () {
        $.ajax({
            type: "POST",
            url: "/Config/DisconnectGateway/",
            success: function () {
                getGatewayInfo();
            }
        });
    });

    $("#ethernet-gateway-connect").click(function () {
        $.ajax({
            type: "POST",
            url: "/Config/ConnectEthernetGateway/",
            success: function () {
                getGatewayInfo();
            }
        });
    });

    $("#ethernet-gateway-disconnect").click(function () {
        $.ajax({
            type: "POST",
            url: "/Config/DisconnectGateway/",
            success: function () {
                getGatewayInfo();
            }
        });
    });


    $("#nodes-engine-start").click(function () {
        $.ajax({
            type: "POST",
            url: "/Config/StartNodesEngine/",
            success: function () {
                getNodesEngineInfo();
            }
        });
    });

    $("#nodes-engine-stop").click(function () {
        $.ajax({
            type: "POST",
            url: "/Config/StopNodesEngine/",
            success:function() {
                getNodesEngineInfo();
            }
        });
    });

    $("#nodes-engine-delete-nodes").click(function () {
        $('#nodes-engine-delete-nodes-confirm').modal({
            onApprove: function () {
                $.ajax({
                    type: "POST", url: "/NodesEditorAPI/RemoveAllNodesAndLinks",
                    success: function (result) {
                        if (result) noty({ text: 'Nodes have been deleted.' });
                    }
                });
            }
        }).modal('setting', 'transition', 'fade up').modal('show');
    });

});



function getNodesEngineInfo() {
    $.ajax({
        url: "/NodesEditorAPI/GetNodesEngineInfo/",
        type: "POST",
        success: function (info) {
            $('#main-content').show();
            updateNodesEngineInfo(info);
            setTimeout(getNodesEngineInfo, REFRESH_DELAY);
        },
        error: function () {
            $('#main-content').hide();
            setTimeout(getNodesEngineInfo, REFRESH_DELAY);
        }
    });
}



function updateNodesEngineInfo(info) {
    $("#nodes-engine-state").removeClass("blue");
    $("#nodes-engine-state").removeClass("red");
    if (info.Started) {
        $("#nodes-engine-state").html('Started');
        $("#nodes-engine-state").addClass('blue');
        $("#nodes-engine-start").hide();
        $("#nodes-engine-stop").show();
    }
    else{
        $("#nodes-engine-state").html('Stopped');
        $("#nodes-engine-state").addClass('red');
        $("#nodes-engine-start").show();
        $("#nodes-engine-stop").hide();
    }

    $('#nodes-engine-links-count').html(info.LinksCount);
    $('#nodes-engine-nodes-count').html(info.AllNodesCount);
    $('#nodes-engine-panels-count').html(info.PanelsNodesCount);
    $('#nodes-engine-io-count').html(info.InputsOutputsNodesCount);
    $('#nodes-engine-ui-count').html(info.UiNodesCount);
    $('#nodes-engine-hardware-count').html(info.HardwareNodesCount);
    $('#nodes-engine-other-count').html(info.OtherNodesCount);
}






function getGatewayInfo() {
    $.ajax({
        url: "/GatewayAPI/GetGatewayInfo/",
        type: "POST",
        success: function (gatewayInfo) {
            $('#main-content').show();
            updateGatewayInfo(gatewayInfo);
            setTimeout(getGatewayInfo, REFRESH_DELAY);
        },
        error: function () {
            $('#main-content').hide();
            setTimeout(getGatewayInfo, REFRESH_DELAY);
        }
    });
}



function updateGatewayInfo(gatewayInfo) {
    $("#gateway-state").removeClass("blue");
    $("#gateway-state").removeClass("red");

    switch (gatewayInfo.state) {
        case 0:
            $("#gateway-state").html('Disconnected');
            $("#gateway-state").addClass('red');
            break;
        case 1:
            $("#gateway-state").html('Connecting to port...');
            $("#gateway-state").addClass('red');
            break;
        case 2:
            $("#gateway-state").html('Connecting to gateway...');
            $("#gateway-state").addClass('red');
            break;
        case 3:
            $("#gateway-state").html('Connected');
            $("#gateway-state").addClass('blue');
            break;
        default:
    }

    if (gatewayInfo.state == 0) { //disconnected
        $("#serial-gateway-connect").show();
        $("#serial-gateway-disconnect").hide();
        $("#ethernet-gateway-connect").show();
        $("#ethernet-gateway-disconnect").hide();
    } else {//connecting or connected
        if (gatewayInfo.type == 0) { //serial
            $("#serial-gateway-connect").hide();
            $("#serial-gateway-disconnect").show();
            $("#ethernet-gateway-connect").show();
            $("#ethernet-gateway-disconnect").hide();
        }
        else if (gatewayInfo.type == 1) { //ethernet
            $("#serial-gateway-connect").show();
            $("#serial-gateway-disconnect").hide();
            $("#ethernet-gateway-connect").hide();
            $("#ethernet-gateway-disconnect").show();
        }
    }


    $('#gateway-nodes-count').html(gatewayInfo.gatewayNodesRegistered);
    $('#gateway-sensors-count').html(gatewayInfo.gatewaySensorsRegistered);
}


function getWebServerInfo() {
    $.ajax({
        url: "/Config/GetWebServerInfo/",
        type: "POST",
        success: function (serverInfo) {
            $('#main-content').show();
            $('#users-count').html(serverInfo.RegisteredUsersCount);
            setTimeout(getWebServerInfo, REFRESH_DELAY);
        },
        error: function () {
            $('#main-content').hide();
            setTimeout(getWebServerInfo, REFRESH_DELAY);
        }
    });
}