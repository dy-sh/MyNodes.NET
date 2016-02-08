/*  MyNetSensors 
    Copyright (C) 2015-2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

var SLIDERS_UPDATE_INTERVAL = 50; //increase this interval if you get excaption on moving slider

var sliderTemplate = Handlebars.compile($('#sliderTemplate').html());

var slidersArray = [];
setInterval(sendSliders, SLIDERS_UPDATE_INTERVAL);



function createSlider(node) {


        $(sliderTemplate(node)).hide().appendTo("#uiContainer-" + node.PanelId).fadeIn(elementsFadeTime);

        var slider = $("#slider-" + node.Id)[0];
        noUiSlider.create(slider,
            {
                start: 0,
                connect: 'lower',
                animate: false,
                range: {
                    'min': Number(node.Settings["Min"].Value),
                    'max': Number(node.Settings["Max"].Value)
                }
            });

        slidersArray.push({
            Id: node.Id,
            lastVal: node.Value
        });

}


function updateSlider(node) {
    $('#sliderName-' + node.Id).html(node.Settings["Name"].Value);
    $("#slider-" + node.Id)[0].noUiSlider.updateOptions({
        range: {
            'min': Number(node.Settings["Min"].Value),
            'max': Number(node.Settings["Max"].Value)
        }
    });

    $("#slider-" + node.Id)[0].noUiSlider.set(node.Value);

    updateSliderInArray(node.Id, node.Value);
}



function updateSliderInArray(sliderId, lastVal) {
    for (var i = 0; i < slidersArray.length; i++) {
        if (slidersArray[i].Id == sliderId)
            slidersArray[i].lastVal = lastVal;
    }
}




function sendSliders() {

    for (var i = 0; i < slidersArray.length; i++) {
        var id = slidersArray[i].Id;

        //if slider was removed
        if ($("#slider-" + id)[0] == null) {
            slidersArray.splice(i, 1);
            i--;
            continue;
        }

        var currentVal = $("#slider-" + id)[0].noUiSlider.get();
        currentVal = Math.round(currentVal);

        if (!isNaN(currentVal) && currentVal != slidersArray[i].lastVal) {

            slidersArray[i].lastVal = currentVal;
            sendSliderChange(slidersArray[i].Id, slidersArray[i].lastVal);
        }
    }
}





function sendSliderChange(nodeId, value) {
    $.ajax({
        url: "/DashboardAPI/SliderChange/",
        type: "POST",
        data: { 'nodeId': nodeId, 'value': value }
    });
}


