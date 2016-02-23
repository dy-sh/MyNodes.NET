/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

var MAIN_PANEL_ID = "Main";

var elementsFadeTime = 300;



var labelTemplate = Handlebars.compile($('#labelTemplate').html());
var progressTemplate = Handlebars.compile($('#progressTemplate').html());
var buttonTemplate = Handlebars.compile($('#buttonTemplate').html());
var toggleButtonTemplate = Handlebars.compile($('#toggleButtonTemplate').html());
var switchTemplate = Handlebars.compile($('#switchTemplate').html());
var logTemplate = Handlebars.compile($('#logTemplate').html());
var stateTemplate = Handlebars.compile($('#stateTemplate').html());
var textBoxTemplate = Handlebars.compile($('#textBoxTemplate').html());
var chartTemplate = Handlebars.compile($('#chartTemplate').html());






function createLabel(node) {
    $(labelTemplate(node)).hide().appendTo("#uiContainer-" + node.PanelId).fadeIn(elementsFadeTime);
}


function createState(node) {
    $(stateTemplate(node)).hide().appendTo("#uiContainer-" + node.PanelId).fadeIn(elementsFadeTime);
}


function createLog(node) {
    $(logTemplate(node)).hide().appendTo("#uiContainer-" + node.PanelId).fadeIn(elementsFadeTime);
    $('#clear-log-' + node.Id).click(function () {
        sendClearLog(node.Id);
    });
}


function createTextBox(node) {
    $(textBoxTemplate(node)).hide().appendTo("#uiContainer-" + node.PanelId).fadeIn(elementsFadeTime);
    $('#textBoxSend-' + node.Id).click(function () {
        sendTextBox(node.Id);
    });
}


function createProgress(node) {
    $(progressTemplate(node)).hide().appendTo("#uiContainer-" + node.PanelId).fadeIn(elementsFadeTime);
}


function createButton(node) {
    $(buttonTemplate(node)).hide().appendTo("#uiContainer-" + node.PanelId).fadeIn(elementsFadeTime);
    $('#button-' + node.Id).click(function () {
        sendButtonClick(node.Id);
    });
}


function createToggleButton(node) {
    $(toggleButtonTemplate(node)).hide().appendTo("#uiContainer-" + node.PanelId).fadeIn(elementsFadeTime);
    $('#button-' + node.Id).click(function () {
        sendToggleButtonClick(node.Id);
    });
}


function createSwitch(node) {
    $(switchTemplate(node)).hide().appendTo("#uiContainer-" + node.PanelId).fadeIn(elementsFadeTime);

    $('#switch-' + node.Id).click(function () {
        sendSwitchClick(node.Id);
    });
}


function updateLabel(node) {
    if (node.Value == null)
        node.Value = "NULL";

    $('#labelName-' + node.Id).html(node.Settings["Name"].Value);
    $('#labelValue-' + node.Id).html(node.Value);
}


function updateState(node) {
    if (node.Value == "1") {
        $('#state-on-' + node.Id).show();
        $('#state-off-' + node.Id).hide();
        $('#state-null-' + node.Id).hide();
    }
    else if (node.Value == "0") {
        $('#state-on-' + node.Id).hide();
        $('#state-off-' + node.Id).show();
        $('#state-null-' + node.Id).hide();
    } else {
        $('#state-on-' + node.Id).hide();
        $('#state-off-' + node.Id).hide();
        $('#state-null-' + node.Id).show();
    }

    $('#stateName-' + node.Id).html(node.Settings["Name"].Value);
}


function updateTextBox(node) {
    $('#textBoxName-' + node.Id).html(node.Settings["Name"].Value);
    $('#textBoxText-' + node.Id).val(node.Value);
}


function updateLog(node) {
    $('#logName-' + node.Id).html(node.Settings["Name"].Value);
    $('#log-' + node.Id).html(node.Log);

    if ($('#log-' + node.Id).get(0) != null)
        $('#log-' + node.Id).animate({ scrollTop: $('#log-' + node.Id).get(0).scrollHeight }, 0);
}


function updateProgress(node) {
    //if (uiNode.Value == null)
    //    uiNode.Value = 0;

    if (node.Value > 100)
        node.Value = 100;

    if (node.Value < 0)
        node.Value = 0;

    $('#progressName-' + node.Id).html(node.Settings["Name"].Value);
    $('#progressBar-' + node.Id).progress({
        percent: node.Value,
        showActivity: false
    });
}


function updateButton(node) {
    $('#buttonName-' + node.Id).html(node.Settings["Name"].Value);

}


function updateToggleButton(node) {
    $('#buttonName-' + node.Id).html(node.Settings["Name"].Value);
    if (node.Value == "1")
        $('#button-' + node.Id).addClass("blue");
    else
        $('#button-' + node.Id).removeClass("blue");
}


function updateSwitch(node) {
    $('#switchName-' + node.Id).html(node.Settings["Name"].Value);
    $('#switch-' + node.Id).html(node.Settings["Name"].Value);
    $('#switch-' + node.Id).prop('checked', node.Value == "1");
}






function sendTextBox(nodeId) {
    var value = $('#textBoxText-' + nodeId).val();
    $.ajax({
        url: "/DashboardAPI/SetValues/",
        type: "POST",
        data: { 'nodeId': nodeId, 'values': { text: value } }
    });
}



function sendClearLog(nodeId) {
    $.ajax({
        url: "/DashboardAPI/SetValues/",
        type: "POST",
        data: { 'nodeId': nodeId, 'values': { clear: "true" } }
    });
}

function sendButtonClick(nodeId) {
    $.ajax({
        url: "/DashboardAPI/SetValues/",
        type: "POST",
        data: { 'nodeId': nodeId, 'values': { click: "true" } }
    });
}

function sendToggleButtonClick(nodeId) {
    $.ajax({
        url: "/DashboardAPI/SetValues/",
        type: "POST",
        data: { 'nodeId': nodeId, 'values': { click: "true" } }
    });
}

function sendSwitchClick(nodeId) {
    $.ajax({
        url: "/DashboardAPI/SetValues/",
        type: "POST",
        data: { 'nodeId': nodeId, 'values': { click: "true" } }
    });
}

