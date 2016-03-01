/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

var MAIN_PANEL_ID = "Main";

var elementsFadeTime = 300;




var audioTemplate = Handlebars.compile($('#audioTemplate').html());


function createAudio(node) {
    $(audioTemplate(node)).hide().appendTo("#uiContainer-" + node.PanelId).fadeIn(elementsFadeTime);
}


var audioPlayer = new Audio();

function updateAudio(node) {
    $('#audioName-' + node.Id).html(node.Settings["Name"].Value);

    var address = node.Address;
    var play = node.Play;

    if (!audioPlayer.paused)
        audioPlayer.pause();

    //if (audioPlayer.src != address) {
        audioPlayer.src = address;
        audioPlayer.load();
    //}

    if (play)
        audioPlayer.play();
}


