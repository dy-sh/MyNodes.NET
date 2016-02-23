/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

var MAIN_PANEL_ID = "Main";

var elementsFadeTime = 300;


var voiceTemplate = Handlebars.compile($('#voiceTemplate').html());


function createVoiceYandex(node) {
    $(voiceTemplate(node)).hide().appendTo("#uiContainer-" + node.PanelId).fadeIn(elementsFadeTime);
}


var playlist = [];
var audioSpeech = new Audio();

var yandex_api_key;

function updateVoiceYandex(node) {
    $('#voiceName-' + node.Id).html(node.Settings["Name"].Value);

    yandex_api_key = node.Settings["APIKey"].Value;

    if (node.Value == null || node.Value == "" || yandex_api_key == null || yandex_api_key=="")
        return;


    playlist.push(node.Value);

    if (audioSpeech.paused)
        playNextTrack();

}


audioSpeech.addEventListener('ended', playNextTrack);

function playNextTrack() {
    if (playlist.length == 0)
        return;

    var text = playlist.shift();

    var url = "https://tts.voicetech.yandex.net/generate?key="+yandex_api_key+"&text=" + text;

    audioSpeech.src = url;
    audioSpeech.load();
    audioSpeech.play();

    //var msg = new SpeechSynthesisUtterance(text);
    //window.speechSynthesis.speak(msg);
}