/*  MyNetSensors 
    Copyright (C) 2015-2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

var MAIN_PANEL_ID = "Main";

var elementsFadeTime = 300;




var speechTemplate = Handlebars.compile($('#speechTemplate').html());


function createSpeech(node) {
    $(speechTemplate(node)).hide().appendTo("#uiContainer-" + node.PanelId).fadeIn(elementsFadeTime);
}


var playlist = [];
var audio = new Audio();

function updateSpeech(node) {
    $('#speechName-' + node.Id).html(node.Name);

    if (node.Value == null || node.Value == "")
        return;

    playlist.push(node.Value);

    if (audio.paused)
        playNextTrack();

}


audio.addEventListener('ended', playNextTrack);

function playNextTrack() {
    if (playlist.length == 0)
        return;

    var text = playlist.shift();
    var url = "https://tts.voicetech.yandex.net/generate?key=95134f06-ffa6-4e8d-8bf0-49f3d07a8918&text=" + text;

    audio.src = url;
    audio.load();
    audio.play();

    //var msg = new SpeechSynthesisUtterance(text);
    //window.speechSynthesis.speak(msg);
}