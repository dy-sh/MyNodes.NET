/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

var SLIDERS_UPDATE_INTERVAL = 50; //increase this interval if you get excaption on moving slider

var rgbSlidersTemplate = Handlebars.compile($('#rgbSlidersTemplate').html());
var rgbwSlidersTemplate = Handlebars.compile($('#rgbwSlidersTemplate').html());

var rgbSlidersArray = [];
var rgbwSlidersArray = [];
setInterval(sendSliders, SLIDERS_UPDATE_INTERVAL);




function createRGBSliders(node) {

    $(rgbSlidersTemplate(node)).hide().appendTo("#uiContainer-" + node.PanelId).fadeIn(elementsFadeTime);

    var r = hexToRgb(node.Value).r;
    var g = hexToRgb(node.Value).g;
    var b = hexToRgb(node.Value).b;
    if (isNaN(r)) r = 0;
    if (isNaN(g)) g = 0;
    if (isNaN(b)) b = 0;

    var sliderR = $("#slider-" + node.Id + "-r")[0];
    var sliderG = $("#slider-" + node.Id + "-g")[0];
    var sliderB = $("#slider-" + node.Id + "-b")[0];

    noUiSlider.create(sliderR, { start: 0, connect: 'lower', animate: false, range: { 'min': 0, 'max': 255 } });
    noUiSlider.create(sliderG, { start: 0, connect: 'lower', animate: false, range: { 'min': 0, 'max': 255 } });
    noUiSlider.create(sliderB, { start: 0, connect: 'lower', animate: false, range: { 'min': 0, 'max': 255 } });

    rgbSlidersArray.push({
        Id: node.Id,
        lastR: r,
        lastG: g,
        lastB: b
    });
}






function createRGBWSliders(node) {

    $(rgbwSlidersTemplate(node)).hide().appendTo("#uiContainer-" + node.PanelId).fadeIn(elementsFadeTime);

    var r = hexToRgbw(node.Value).r;
    var g = hexToRgbw(node.Value).g;
    var b = hexToRgbw(node.Value).b;
    var w = hexToRgbw(node.Value).w;
    if (isNaN(r)) r = 0;
    if (isNaN(g)) g = 0;
    if (isNaN(b)) b = 0;
    if (isNaN(w)) w = 0;

    var sliderR = $("#slider-" + node.Id + "-r")[0];
    var sliderG = $("#slider-" + node.Id + "-g")[0];
    var sliderB = $("#slider-" + node.Id + "-b")[0];
    var sliderW = $("#slider-" + node.Id + "-w")[0];

    noUiSlider.create(sliderR, { start: 0, connect: 'lower', animate: false, range: { 'min': 0, 'max': 255 } });
    noUiSlider.create(sliderG, { start: 0, connect: 'lower', animate: false, range: { 'min': 0, 'max': 255 } });
    noUiSlider.create(sliderB, { start: 0, connect: 'lower', animate: false, range: { 'min': 0, 'max': 255 } });
    noUiSlider.create(sliderW, { start: 0, connect: 'lower', animate: false, range: { 'min': 0, 'max': 255 } });

    rgbwSlidersArray.push({
        Id: node.Id,
        lastR: r,
        lastG: g,
        lastB: b,
        lastW: b
    });

}




function updateRGBSliders(node) {
    var r = hexToRgb(node.Value).r;
    var g = hexToRgb(node.Value).g;
    var b = hexToRgb(node.Value).b;
    if (isNaN(r)) r = 0;
    if (isNaN(g)) g = 0;
    if (isNaN(b)) b = 0;

    $('#sliderName-' + node.Id).html(node.Settings["Name"].Value);
    $("#slider-" + node.Id + "-r")[0].noUiSlider.set(r);
    $("#slider-" + node.Id + "-g")[0].noUiSlider.set(g);
    $("#slider-" + node.Id + "-b")[0].noUiSlider.set(b);
    updateRgbSlidersInArray(node.Id, node.Value);
}


function updateRGBWSliders(node) {
    var r = hexToRgbw(node.Value).r;
    var g = hexToRgbw(node.Value).g;
    var b = hexToRgbw(node.Value).b;
    var w = hexToRgbw(node.Value).w;
    if (isNaN(r)) r = 0;
    if (isNaN(g)) g = 0;
    if (isNaN(b)) b = 0;
    if (isNaN(w)) w = 0;

    $('#sliderName-' + node.Id).html(node.Settings["Name"].Value);
    $("#slider-" + node.Id + "-r")[0].noUiSlider.set(r);
    $("#slider-" + node.Id + "-g")[0].noUiSlider.set(g);
    $("#slider-" + node.Id + "-b")[0].noUiSlider.set(b);
    $("#slider-" + node.Id + "-w")[0].noUiSlider.set(w);
    updateRgbwSlidersInArray(node.Id, node.Value);
}




function updateRgbSlidersInArray(sliderId, lastHex) {
    for (var i = 0; i < rgbSlidersArray.length; i++) {
        if (rgbSlidersArray[i].Id == sliderId) {
            rgbSlidersArray[i].lastR = hexToRgb(lastHex).r;
            rgbSlidersArray[i].lastG = hexToRgb(lastHex).g;
            rgbSlidersArray[i].lastB = hexToRgb(lastHex).b;
        }
    }
}


function updateRgbwSlidersInArray(sliderId, lastHex) {
    for (var i = 0; i < rgbwSlidersArray.length; i++) {
        if (rgbwSlidersArray[i].Id == sliderId) {
            rgbwSlidersArray[i].lastR = hexToRgbw(lastHex).r;
            rgbwSlidersArray[i].lastG = hexToRgbw(lastHex).g;
            rgbwSlidersArray[i].lastB = hexToRgbw(lastHex).b;
            rgbwSlidersArray[i].lastW = hexToRgbw(lastHex).w;
        }
    }
}




function sendSliders() {


    for (var i = 0; i < rgbSlidersArray.length; i++) {
        var id = rgbSlidersArray[i].Id;

        //if slider was removed
        if ($("#slider-" + id + "-r")[0] == null) {
            rgbSlidersArray.splice(i, 1);
            i--;
            continue;
        }

        var currentR = $("#slider-" + id + "-r")[0].noUiSlider.get();
        var currentG = $("#slider-" + id + "-g")[0].noUiSlider.get();
        var currentB = $("#slider-" + id + "-b")[0].noUiSlider.get();

        currentR = Math.round(currentR);
        currentG = Math.round(currentG);
        currentB = Math.round(currentB);


        if (currentR != rgbSlidersArray[i].lastR ||
            currentG != rgbSlidersArray[i].lastG ||
            currentB != rgbSlidersArray[i].lastB) {

            var hex = RgbToHex(currentR, currentG, currentB);
            updateRgbSlidersInArray(id, hex);

            sendRGBSlidersChange(rgbSlidersArray[i].Id, hex);
        }
    }

    for (var i = 0; i < rgbwSlidersArray.length; i++) {
        var id = rgbwSlidersArray[i].Id;

        //if slider was removed
        if ($("#slider-" + id + "-r")[0] == null) {
            rgbwSlidersArray.splice(i, 1);
            i--;
            continue;
        }

        var currentR = $("#slider-" + id + "-r")[0].noUiSlider.get();
        var currentG = $("#slider-" + id + "-g")[0].noUiSlider.get();
        var currentB = $("#slider-" + id + "-b")[0].noUiSlider.get();
        var currentW = $("#slider-" + id + "-w")[0].noUiSlider.get();

        currentR = Math.round(currentR);
        currentG = Math.round(currentG);
        currentB = Math.round(currentB);
        currentW = Math.round(currentW);

        if (currentR != rgbwSlidersArray[i].lastR ||
            currentG != rgbwSlidersArray[i].lastG ||
            currentB != rgbwSlidersArray[i].lastB ||
            currentW != rgbwSlidersArray[i].lastW) {

            var hex = RgbwToHex(currentR, currentG, currentB, currentW);
            updateRgbwSlidersInArray(id, hex);

            sendRGBWSlidersChange(rgbwSlidersArray[i].Id, hex);
        }
    }
}



function sendRGBSlidersChange(nodeId, value) {
    $.ajax({
        url: "/DashboardAPI/SetValues/",
        type: "POST",
        data: { 'nodeId': nodeId, 'values': { sliders: value } }
    });
}

function sendRGBWSlidersChange(nodeId, value) {
    $.ajax({
        url: "/DashboardAPI/SetValues/",
        type: "POST",
        data: { 'nodeId': nodeId, 'values': { sliders: value } }
    });
}

