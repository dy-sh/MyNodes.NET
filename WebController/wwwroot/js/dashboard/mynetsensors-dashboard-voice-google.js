/*  MyNetSensors 
    Copyright (C) 2015-2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

var MAIN_PANEL_ID = "Main";

var elementsFadeTime = 300;

var voiceTemplate = Handlebars.compile($('#voiceTemplate').html());


function createVoiceGoogle(node) {
    $(voiceTemplate(node)).hide().appendTo("#uiContainer-" + node.PanelId).fadeIn(elementsFadeTime);
}


function updateVoiceGoogle(node) {
    $('#voiceName-' + node.Id).html(node.Settings["Name"].Value);

    if (node.Value == null || node.Value == "")
        return;

    var msg = new SpeechSynthesisUtterance(node.Value);
    window.speechSynthesis.speak(msg);
}

