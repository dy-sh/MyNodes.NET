/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/



$(function () {
    getGatewayInfo();
    getNodesEngineInfo();
    setInterval(getGatewayInfo, 1000);
    setInterval(getNodesEngineInfo, 1000);
});


$(document).ready(function () {


    $("#serial-gateway-delete-nodes").click(function () {
        $('#serial-gateway-delete-nodes-nonfirm').modal({
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
            success: function () {
                getGatewayInfo();
            }
        });
    });

    $("#serial-gateway-disconnect").click(function () {
        $.ajax({
            type: "POST",
            url: "/Config/DisconnectSerialController/",
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
                        if (result) noty({ text: 'Nodes were deleted.' });
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
        },
        error: function () {
            $('#main-content').hide();
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
        },
        error: function () {
            $('#main-content').hide();
        }
    });
}



function updateGatewayInfo(gatewayInfo) {
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


    $('#serial-gateway-nodes-count').html(gatewayInfo.gatewayNodesRegistered);
    $('#serial-gateway-sensors-count').html(gatewayInfo.gatewaySensorsRegistered);
}
