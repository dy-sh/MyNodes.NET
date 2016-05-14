var stateSwitchTemplate = Handlebars.compile($('#stateSwitchTemplate').html());

function createStateSwitch(node) {
    $(stateSwitchTemplate(node)).hide().appendTo("#uiContainer-" + node.PanelId).fadeIn(elementsFadeTime);

    $('#stateSwitch-' + node.Id).click(function () {
        sendStateSwitchClick(node.Id);
    });
}

function sendStateSwitchClick(nodeId) {
    $.ajax({
        url: "/DashboardAPI/SetValues/",
        type: "POST",
        data: { 'nodeId': nodeId, 'values': { click: "true" } }
    });
}

function updateStateSwitch(node) {
    $('#stateSwitchName-' + node.Id).html(node.Settings["Name"].Value);
    $('#stateSwitch-' + node.Id).html(node.Settings["Name"].Value);
    $('#stateSwitch-' + node.Id).prop('checked', node.Value == "1");
}