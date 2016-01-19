/*  MyNetSensors 
    Copyright (C) 2015-2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

var MAIN_PANEL_ID = "Main";

var elementsFadeTime = 300;


var timerTemplate = Handlebars.compile($('#timerTemplate').html());

function createTimer(node) {
    $(timerTemplate(node)).hide().appendTo("#uiContainer-" + node.PanelId).fadeIn(elementsFadeTime);
}


function updateTimer(node) {
    $('#timer-name-' + node.Id).html(node.Name);
}

