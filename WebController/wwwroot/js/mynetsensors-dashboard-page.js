/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/



var gatewayHardwareConnected = null;
var signalRServerConnected = null;

var sliderUpdateInterval = 50; //increase this interval if you get excaption on moving slider
var elementsFadeTime = 300;

//var nodes;

//var slidersArray = [];
//var rgbSlidersArray = [];
//var rgbwSlidersArray = [];
//setInterval(sendSliders, sliderUpdateInterval);
//var ignoreSendingSwitchId;


$(function () {

    //configure signalr
    var clientsHub = $.connection.clientsHub;

    clientsHub.client.OnConnectedEvent = function () {
        hardwareStateChanged(true);
    };

    clientsHub.client.OnDisconnectedEvent = function () {
        hardwareStateChanged(false);
    };

    clientsHub.client.OnClearNodesListEvent = function () {
        var n = noty({ text: 'Nodes deleted from the database!', type: 'error' });
        $('#panelsContainer').html(null);
    };


    clientsHub.client.OnNewUINodeEvent = function (node) {
        createNode(node);
    };

    clientsHub.client.OnUINodeUpdatedEvent = function (node) {
        updateNode(node);
    };

    clientsHub.client.OnUINodeDeleteEvent = function (sensor) {
        deleteNode(sensor);
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

    getIsHardwareConnected();
    getNodes();
});



function getIsHardwareConnected() {
    $.ajax({
        url: "/GatewayAPI/IsHardwareConnected/",
        type: "POST",
        success: function (connected) {
            hardwareStateChanged(connected);
        }
    });
}



function hardwareStateChanged(connected) {
    if (connected) {
        $('#panelsContainer').fadeIn(elementsFadeTime);
    } else {
        $('#panelsContainer').fadeOut(elementsFadeTime);
    }

    if (connected && gatewayHardwareConnected === false) {
        noty({ text: 'Gateway hardware is online.', type: 'alert', timeout: false });
    } else if (!connected) {
        noty({ text: 'Gateway hardware is offline!', type: 'error', timeout: false });
    }

    gatewayHardwareConnected = connected;
}



function getNodes() {
    $.ajax({
        url: "/Dashboard/GetUINodes/",
        type: "POST",
        success: function (nodes) {
            onReturnNodes(nodes);
        }
    });
}

function onReturnNodes(nodes) {
    //var temp = elementsFadeTime;
    //elementsFadeTime = 0;
    //$('#panelsContainer').html(null);
    //slidersArray.length = 0;
    //rgbSlidersArray.length = 0;
    //rgbwSlidersArray.length = 0;

    for (var i = 0; i < nodes.length; i++) {
        createNode(nodes[i]);
    }
    //elementsFadeTime = temp;
}



var panelTemplate = Handlebars.compile($('#panelTemplate').html());
var labelTemplate = Handlebars.compile($('#labelTemplate').html());
var progressTemplate = Handlebars.compile($('#progressTemplate').html());
var buttonTemplate = Handlebars.compile($('#buttonTemplate').html());
var toggleButtonTemplate = Handlebars.compile($('#toggleButtonTemplate').html());


function createPanel(node) {
    //create new
    $(panelTemplate(node)).hide().appendTo("#panelsContainer").fadeIn(elementsFadeTime);
}




function createNode(node) {
    //create new panel
    if ($('#panel' + node.PanelId).length == 0) {
        createPanel(node);
    }

    if (node.Type == "UI/Label") {
        $(labelTemplate(node)).hide().appendTo("#uiContainer" + node.PanelId).fadeIn(elementsFadeTime);
    }

    if (node.Type == "UI/Progress") {
        $(progressTemplate(node)).hide().appendTo("#uiContainer" + node.PanelId).fadeIn(elementsFadeTime);
    }

    if (node.Type == "UI/Button") {
        $(buttonTemplate(node)).hide().appendTo("#uiContainer" + node.PanelId).fadeIn(elementsFadeTime);
        $('#button-' + node.Id).click(function () {
            sendButtonClick(node.Id);
        });
    }

    if (node.Type == "UI/Toggle Button") {
        $(toggleButtonTemplate(node)).hide().appendTo("#uiContainer" + node.PanelId).fadeIn(elementsFadeTime);
        $('#button-' + node.Id).click(function () {
            sendToggleButtonClick(node.Id);
        });
    }

    updateNode(node);
}

function updateNode(node) {
    $('#activity' + node.PanelId).show().fadeOut(150);

    if (node.Type == "UI/Label") {
        if (node.Value == null)
            node.Value = "NULL";

        $('#labelName-' + node.Id).html(node.Name);
        $('#labelValue-' + node.Id).html(node.Value);
    }

    if (node.Type == "UI/Progress") {
        //if (node.Value == null)
        //    node.Value = 0;

        if (node.Value > 100)
            node.Value = 100;

        if (node.Value < 0)
            node.Value = 0;

        $('#progressName-' + node.Id).html(node.Name);
        $('#progressBar-' + node.Id).progress({
            percent: node.Value,
            showActivity: false
        });
    }

    if (node.Type == "UI/Button" || node.Type == "UI/Toggle Button") {
        $('#buttonName-' + node.Id).html(node.Name);
    }

    if (node.Type == "UI/Toggle Button") {
        if (node.Value=="1")
            $('#button-' + node.Id).addClass("blue");
        else
            $('#button-' + node.Id).removeClass("blue");
    }
}



function deleteNode(node) {
    $('#node' + node.Id).fadeOut(elementsFadeTime, function () { $(this).remove(); });
}





function sendButtonClick(nodeId) {
    $.ajax({
        url: "/Dashboard/ButtonClick/",
        type: "POST",
        data: { 'nodeId': nodeId }
    });
}

function sendToggleButtonClick(nodeId) {
    $.ajax({
        url: "/Dashboard/ToggleButtonClick/",
        type: "POST",
        data: { 'nodeId': nodeId }
    });
}








//function createOrUpdateSensorData(sensor) {


//    var id = sensor.nodeId + "-" + sensor.sensorId;

//    //update body

//    //ON-OFF BUTTON
//    if (sensor.dataType == mySensors.sensorDataType.V_TRIPPED
//            || sensor.dataType == mySensors.sensorDataType.V_STATUS) {
//        if ($("[name='toggle-" + id + "']").length == 0) {
//            //create new
//            $(toggleTemplate(sensor)).hide().appendTo("#dataPanel" + id).fadeIn(elementsFadeTime);

//            $("[name='toggle-" + id + "']").prop('checked', sensor.state == "1");

//            $("[name='toggle-" + id + "']").click(function () {
//                if (ignoreSendingSwitchId == id)
//                    return;

//                var state = $(this).is(":checked");

//                var toggle = this.name.split("-");
//                var val = state == true ? 1 : 0;
//                sendSensor(toggle[1], toggle[2], val);
//            });
//        } else {
//            //update
//            ignoreSendingSwitchId = id;
//            $("[name='toggle-" + id + "']").prop('checked', sensor.state == "1");
//            ignoreSendingSwitchId = null;
//        }
//    }
//        //0-100% SLIDER
//    else if (sensor.dataType == mySensors.sensorDataType.V_PERCENTAGE
//            || sensor.dataType == mySensors.sensorDataType.V_LIGHT_LEVEL) {

//        if (isNaN(sensor.state)) sensor.state = 0;

//        if ($("[name='slider-" + id + "']").length == 0) {
//            //create new
//            $(sliderTemplate(sensor)).hide().appendTo("#dataPanel" + id).fadeIn(elementsFadeTime);

//            var slider = $("[name='slider-" + id + "']")[0];
//            noUiSlider.create(slider, { start: [sensor.state], connect: 'lower', animate: false, range: { 'min': 0, 'max': 100 } });

//            $('#dataPanel' + id).removeClass("pull-right");


//            slidersArray.push({
//                sliderId: id,
//                nodeId: sensor.nodeId,
//                sensorId: sensor.sensorId,
//                lastVal: sensor.state
//            });
//        } else {
//            //update

//            var slider = $("[name='slider-" + id + "']")[0];
//            slider.noUiSlider.set(sensor.state);

//            updateSliderInArray(id, sensor.state);

//        }
//    }
//        //RGB SLIDERS
//    else if (sensor.dataType == mySensors.sensorDataType.V_RGB) {

//        var r = hexToRgb(sensor.state).r;
//        var g = hexToRgb(sensor.state).g;
//        var b = hexToRgb(sensor.state).b;
//        if (isNaN(r)) r = 0;
//        if (isNaN(g)) g = 0;
//        if (isNaN(b)) b = 0;


//        if ($("[name='slider-" + id + "-r']").length == 0) {
//            //create new
//            $(rgbSlidersTemplate(sensor)).hide().appendTo("#dataPanel" + id).fadeIn(elementsFadeTime);

//            var sliderR = $("[name='slider-" + id + "-r']")[0];
//            var sliderG = $("[name='slider-" + id + "-g']")[0];
//            var sliderB = $("[name='slider-" + id + "-b']")[0];

//            noUiSlider.create(sliderR, { start: [r], connect: 'lower', animate: false, range: { 'min': 0, 'max': 255 } });
//            noUiSlider.create(sliderG, { start: [g], connect: 'lower', animate: false, range: { 'min': 0, 'max': 255 } });
//            noUiSlider.create(sliderB, { start: [b], connect: 'lower', animate: false, range: { 'min': 0, 'max': 255 } });

//            $('#dataPanel' + id).removeClass("pull-right");


//            rgbSlidersArray.push({
//                sliderId: id,
//                nodeId: sensor.nodeId,
//                sensorId: sensor.sensorId,
//                lastR: r,
//                lastG: g,
//                lastB: b
//            });
//        } else {

//            $("[name='slider-" + id + "-r']")[0].noUiSlider.set(r);
//            $("[name='slider-" + id + "-g']")[0].noUiSlider.set(g);
//            $("[name='slider-" + id + "-b']")[0].noUiSlider.set(b);

//            updateRgbSlidersInArray(id, sensor.state);
//        }

//    }
//        //RGBW SLIDERS
//    else if (sensor.dataType == mySensors.sensorDataType.V_RGBW) {

//        var r = hexToRgbw(sensor.state).r;
//        var g = hexToRgbw(sensor.state).g;
//        var b = hexToRgbw(sensor.state).b;
//        var w = hexToRgbw(sensor.state).w;
//        if (isNaN(r)) r = 0;
//        if (isNaN(g)) g = 0;
//        if (isNaN(b)) b = 0;
//        if (isNaN(w)) w = 0;

//        if ($("[name='slider-" + id + "-r']").length == 0) {
//            //create new
//            $(rgbwSlidersTemplate(sensor)).hide().appendTo("#dataPanel" + id).fadeIn(elementsFadeTime);

//            var sliderR = $("[name='slider-" + id + "-r']")[0];
//            var sliderG = $("[name='slider-" + id + "-g']")[0];
//            var sliderB = $("[name='slider-" + id + "-b']")[0];
//            var sliderW = $("[name='slider-" + id + "-w']")[0];

//            noUiSlider.create(sliderR, { start: [r], connect: 'lower', animate: false, range: { 'min': 0, 'max': 255 } });
//            noUiSlider.create(sliderG, { start: [g], connect: 'lower', animate: false, range: { 'min': 0, 'max': 255 } });
//            noUiSlider.create(sliderB, { start: [b], connect: 'lower', animate: false, range: { 'min': 0, 'max': 255 } });
//            noUiSlider.create(sliderW, { start: [w], connect: 'lower', animate: false, range: { 'min': 0, 'max': 255 } });

//            $('#dataPanel' + id).removeClass("pull-right");


//            rgbwSlidersArray.push({
//                sliderId: id,
//                nodeId: sensor.nodeId,
//                sensorId: sensor.sensorId,
//                lastR: r,
//                lastG: g,
//                lastB: b,
//                lastW: w
//            });
//        } else {
//            //update
//            $("[name='slider-" + id + "-r']")[0].noUiSlider.set(r);
//            $("[name='slider-" + id + "-g']")[0].noUiSlider.set(g);
//            $("[name='slider-" + id + "-b']")[0].noUiSlider.set(b);
//            $("[name='slider-" + id + "-w']")[0].noUiSlider.set(w);
//            updateRgbSlidersInArray(id, sensor.state);
//        }

//    }
//        //IR SEND
//    else if (sensor.dataType == mySensors.sensorDataType.V_IR_SEND) {


//        if ($("[name='textbox-" + id + "']").length == 0) {
//            //create new
//            $(irSendTemplate(sensor)).hide().appendTo("#dataPanel" + id).fadeIn(elementsFadeTime);

//            $('#dataPanel' + id).removeClass("pull-right");
//            $('#dataPanel' + id).removeClass("pull-up");

//            $("[name='button-" + id + "']").click(function () {
//                var code = $("[name='textbox-" + id + "']").val();
//                sendSensor(sensor.nodeId, sensor.sensorId, code);
//            });

//        } else {
//            //update

//        }
//    }
//        //IR RECEIVE
//    else if (sensor.dataType == mySensors.sensorDataType.V_IR_RECEIVE) {
//        $('#dataPanel' + id)
//            .html("<br/>IR receive: " + sensor.state);

//        $('#dataPanel' + id).removeClass("pull-right");
//        $('#dataPanel' + id).removeClass("pull-up");

//    }
//        //Simple text
//    else {
//        $('#dataPanel' + id)
//            .html(sensor.state);
//    }


//}



//function sendSliders() {

//    for (var i = 0; i < slidersArray.length; i++) {
//        var id = slidersArray[i].sliderId;
//        var currentVal = $("[name='slider-" + id + "']")[0].noUiSlider.get();
//        currentVal = Math.round(currentVal);

//        if (!isNaN(currentVal) && currentVal != slidersArray[i].lastVal) {

//            slidersArray[i].lastVal = currentVal;
//            sendSensor(slidersArray[i].nodeId,
//                slidersArray[i].sensorId,
//                slidersArray[i].lastVal);
//        }
//    }

//    for (var i = 0; i < rgbSlidersArray.length; i++) {
//        var id = rgbSlidersArray[i].sliderId;
//        var currentR = $("[name='slider-" + id + "-r']")[0].noUiSlider.get();
//        var currentG = $("[name='slider-" + id + "-g']")[0].noUiSlider.get();
//        var currentB = $("[name='slider-" + id + "-b']")[0].noUiSlider.get();

//        currentR = Math.round(currentR);
//        currentG = Math.round(currentG);
//        currentB = Math.round(currentB);


//        if (currentR != rgbSlidersArray[i].lastR ||
//            currentG != rgbSlidersArray[i].lastG ||
//            currentB != rgbSlidersArray[i].lastB) {

//            var hex = RgbToHex(currentR, currentG, currentB);
//            updateRgbSlidersInArray(id, hex);

//            sendSensor(rgbSlidersArray[i].nodeId,
//                rgbSlidersArray[i].sensorId,
//                hex);
//        }
//    }

//    for (var i = 0; i < rgbwSlidersArray.length; i++) {
//        var id = rgbwSlidersArray[i].sliderId;
//        var currentR = $("[name='slider-" + id + "-r']")[0].noUiSlider.get();
//        var currentG = $("[name='slider-" + id + "-g']")[0].noUiSlider.get();
//        var currentB = $("[name='slider-" + id + "-b']")[0].noUiSlider.get();
//        var currentW = $("[name='slider-" + id + "-w']")[0].noUiSlider.get();

//        currentR = Math.round(currentR);
//        currentG = Math.round(currentG);
//        currentB = Math.round(currentB);
//        currentW = Math.round(currentW);

//        if (currentR != rgbwSlidersArray[i].lastR ||
//            currentG != rgbwSlidersArray[i].lastG ||
//            currentB != rgbwSlidersArray[i].lastB ||
//            currentW != rgbwSlidersArray[i].lastW) {

//            var hex = RgbwToHex(currentR, currentG, currentB);
//            updateRgbwSlidersInArray(id, hex);

//            sendSensor(rgbwSlidersArray[i].nodeId,
//                rgbwSlidersArray[i].sensorId,
//                hex);
//        }
//    }
//}

//function updateSliderInArray(sliderId, lastVal) {
//    for (var i = 0; i < slidersArray.length; i++) {
//        if (slidersArray[i].sliderId == sliderId)
//            slidersArray[i].lastVal = lastVal;
//    }
//}

//function updateRgbSlidersInArray(sliderId, lastHex) {
//    for (var i = 0; i < rgbSlidersArray.length; i++) {
//        if (rgbSlidersArray[i].sliderId == sliderId) {
//            rgbSlidersArray[i].lastR = hexToRgb(lastHex).r;
//            rgbSlidersArray[i].lastG = hexToRgb(lastHex).g;
//            rgbSlidersArray[i].lastB = hexToRgb(lastHex).b;
//        }
//    }
//}


//function updateRgbwSlidersInArray(sliderId, lastHex) {
//    for (var i = 0; i < rgbwSlidersArray.length; i++) {
//        if (rgbwSlidersArray[i].sliderId == sliderId) {
//            rgbwSlidersArray[i].lastR = hexToRgbw(lastHex).r;
//            rgbwSlidersArray[i].lastG = hexToRgbw(lastHex).g;
//            rgbwSlidersArray[i].lastB = hexToRgbw(lastHex).b;
//            rgbwSlidersArray[i].lastW = hexToRgbw(lastHex).w;
//        }
//    }
//}

