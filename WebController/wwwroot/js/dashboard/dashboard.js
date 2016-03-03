/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
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
        case "Label":
            createLabel(node);
            break;
        case "State":
            createState(node);
            break;
        case "Log":
            createLog(node);
            break;
        case "TextBox":
            createTextBox(node);
            break;
        case "Progress":
            createProgress(node);
            break;
        case "Button":
            createButton(node);
            break;
        case "Toggle":
            createToggleButton(node);
            break;
        case "Switch":
            createSwitch(node);
            break;
        case "Slider":
            createSlider(node);
            break;
        case "RGB Sliders":
            createRGBSliders(node);
            break;
        case "RGBW Sliders":
            createRGBWSliders(node);
            break;
        case "Chart":
            createChart(node);
            break;
        case "Timer":
            createTimer(node);
            break;
        case "Voice Yandex":
            createVoiceYandex(node);
            break;
        case "Voice Google":
            createVoiceGoogle(node);
            break;
        case "Audio":
            createAudio(node);
            break;
        default:
    }

    updateNode(node);
}





function updateNode(node) {
    //if ShowOnHomePage option changed to true
    if ($('#node-' + node.Id).length == 0)
        createNode(node);

    $('#activity-' + node.PanelId).show().fadeOut(150);

    switch (node.Type) {
        case "Label":
            updateLabel(node);
            break;
        case "State":
            updateState(node);
            break;
        case "Log":
            updateLog(node);
            break;
        case "TextBox":
            updateTextBox(node);
            break;
        case "Progress":
            updateProgress(node);
            break;
        case "Button":
            updateButton(node);
            break;
        case "Toggle":
            updateToggleButton(node);
            break;
        case "Switch":
            updateSwitch(node);
            break;
        case "Slider":
            updateSlider(node);
            break;
        case "RGB Sliders":
            updateRGBSliders(node);
            break;
        case "RGBW Sliders":
            updateRGBWSliders(node);
            break;
        case "Chart":
            updateChart(node);
            break;
        case "Timer":
            updateTimer(node);
            break;
        case "Voice Yandex":
            updateVoiceYandex(node);
            break;
        case "Voice Google":
            updateVoiceGoogle(node);
            break;
        case "Audio":
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
            case "Chart":
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

function updatePanel(node) {
    var settings =  JSON.parse(node.properties["Settings"]);
    $('#panelTitle-' + node.id).html(settings.Name.Value);
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