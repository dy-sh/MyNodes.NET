/*  MyNetSensors 
    Copyright (C) 2015-2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

var MAIN_PANEL_ID = "Main";

var elementsFadeTime = 300;

var panelTemplate = Handlebars.compile($('#panelTemplate').html());




function createNode(node) {
    //create new panel
    if ($('#panel-' + node.PanelId).length == 0) {
        createPanel(node);
    }
    switch (node.Type) {
        case "UI/Label":
            createLabel(node);
            break;
        case "UI/State":
            createState(node);
            break;
        case "UI/Log":
            createLog(node);
            break;
        case "UI/TextBox":
            createTextBox(node);
            break;
        case "UI/Progress":
            createProgress(node);
            break;
        case "UI/Button":
            createButton(node);
            break;
        case "UI/Toggle Button":
            createToggleButton(node);
            break;
        case "UI/Switch":
            createSwitch(node);
            break;
        case "UI/Slider":
            createSlider(node);
            break;
        case "UI/RGB Sliders":
            createRGBSliders(node);
            break;
        case "UI/RGBW Sliders":
            createRGBWSliders(node);
            break;
        case "UI/Chart":
            createChart(node);
            break;
        case "UI/Timer":
            createTimer(node);
            break;
        case "UI/Speech":
            createSpeech(node);
            break;
        case "UI/Audio":
            createAudio(node);
            break;
    default:
    }
    updateNode(node);
}




function updateNode(node) {
    //if ShowOnMainPage option changed to true
    if ($('#node-' + node.Id).length==0)
        createNode(node);

    $('#activity-' + node.PanelId).show().fadeOut(150);

    switch (node.Type) {
        case "UI/Label":
            updateLabel(node);
            break;
        case "UI/State":
            updateState(node);
            break;
        case "UI/Log":
            updateLog(node);
            break;
        case "UI/TextBox":
            updateTextBox(node);
            break;
        case "UI/Progress":
            updateProgress(node);
            break;
        case "UI/Button":
            updateButton(node);
            break;
        case "UI/Toggle Button":
            updateToggleButton(node);
            break;
        case "UI/Switch":
            updateSwitch(node);
            break;
        case "UI/Slider":
            updateSlider(node);
            break;
        case "UI/RGB Sliders":
            updateRGBSliders(node);
            break;
        case "UI/RGBW Sliders":
            updateRGBWSliders(node);
            break;
        case "UI/Chart":
            updateChart(node);
            break;
        case "UI/Timer":
            updateTimer(node);
            break;
        case "UI/Speech":
            updateSpeech(node);
            break;
        case "UI/Audio":
            updateAudio(node);
            break;
        default:
    }
}



function removeNode(node) {
    $('#node-' + node.Id).fadeOut(elementsFadeTime, function () {

        switch (node.Type) {
        case "UI/Chart":
            removeChart(node);
            break;
        default:
        }


        $(this).remove();
        checkPanelForRemove(node.PanelId);
    });
}

function checkPanelForRemove(panelId) {
    var panelBody = $('#uiContainer-' + panelId);
    if (panelBody.children().length == 0)
        removePanel(panelId);
}


function createPanel(node) {
    $('#empty-message').hide();

    //create new
    $(panelTemplate(node)).hide().appendTo("#panelsContainer").fadeIn(elementsFadeTime);

    $('#panelTitle-' + node.PanelId).html(node.PanelName);
}

function removePanel(panelId) {
    $('#panel-' + panelId).fadeOut(elementsFadeTime, function () {
        $(this).remove();
    });
}
