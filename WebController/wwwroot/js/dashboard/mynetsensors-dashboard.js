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
        case "UI/Voice Yandex":
            createVoiceYandex(node);
            break;
        case "UI/Voice Google":
            createVoiceGoogle(node);
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
    if ($('#node-' + node.Id).length == 0)
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
        case "UI/Voice Yandex":
            updateVoiceYandex(node);
            break;
        case "UI/Voice Google":
            updateVoiceGoogle(node);
            break;
        case "UI/Audio":
            updateAudio(node);
            break;
        default:
    }

    var oldPanelIndex = $('#node-' + node.Id).attr("panelIndex");
    if (oldPanelIndex != node.Settings["PanelIndex"].Value) {
        $('#node-' + node.Id).attr("panelIndex", node.Settings["PanelIndex"].Value);
        sortPanel(node.PanelId);
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


function sortPanel(panelId) {

    var elements = $('#uiContainer-' + panelId).children();
    var count = 0;

    // sort based on timestamp attribute
    elements.sort(function (a, b) {

        // convert to integers from strings
        a = parseInt($(a).attr("panelIndex"), 10);
        b = parseInt($(b).attr("panelIndex"), 10);
        count += 2;
        // compare
        if (a > b) {
            return 1;
        } else if (a < b) {
            return -1;
        } else {
            return 0;
        }
    });

    var panel = document.getElementById('uiContainer-' + panelId);
    for (i = 0; i < elements.length; ++i) {
        panel.appendChild(elements[i]);
    }
};