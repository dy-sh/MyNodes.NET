/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/



var logTemplate = Handlebars.compile($('#logTemplate').html());

var elementsFadeTime = 300;


function createLog(node) {
    $(logTemplate(node)).hide().appendTo("#uiContainer-" + node.PanelId).fadeIn(elementsFadeTime);
    $('#clear-log-' + node.Id).click(function () {
        sendClearLog(node.Id);
    });

    //Loading data frow server
    $.ajax({
        url: "/DashboardAPI/GetValue/",
        data: { 'nodeId': node.Id, 'name': "log" },
        dataType: "json",
        success: function (log) {
            $('#log-' + node.Id).empty();
            if (log != null) {
                for (var i in log) {
                    addLogData(log[i], node.Id, node.Settings.MaxRecords.Value);
                }
            }
        }
    });
}


function sendClearLog(nodeId) {
    $.ajax({
        url: "/DashboardAPI/SetValues/",
        type: "POST",
        data: { 'nodeId': nodeId, 'values': { Clear: "true" } },
        success: function () {
            $('#log-' + nodeId).empty();
        }
    });
}

var lastLog = {};

function updateLog(node) {
    $('#logName-' + node.Id).html(node.Settings["Name"].Value);

    //var t = moment(node.LastRecord.DateTime).format("DD.MM.YYYY H:mm:ss");
    if (node.LastRecord == null
        || (lastLog[node.Id] != null
        && lastLog[node.Id].Value == node.LastRecord.Value
        && lastLog[node.Id].DateTime == node.LastRecord.DateTime))
        return;

    addLogData(node.LastRecord, node.Id, node.Settings.MaxRecords.Value);

    if ($('#log-' + node.Id).get(0) != null)
        $('#log-' + node.Id).animate({ scrollTop: $('#log-' + node.Id).get(0).scrollHeight }, 0);

}


function addLogData(record, nodeId, maxRecords) {
    lastLog[nodeId] = record;

    $('#log-' + nodeId).append(
        '<div class="log-record">'
        + moment(record.DateTime).format("DD.MM.YYYY H:mm:ss.SSS")
        + ": " + record.Value + "<br>"
        + '</div>'
        );

    var records = $('#log-' + nodeId).children();
    var unwanted = records.length - maxRecords;
    for (var i = 0; i < unwanted; i++) {
        records[i].remove();
    }
}

