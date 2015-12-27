/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

var gatewayHardwareConnected = null;
var signalRServerConnected = null;


var editor = new LiteGraph.Editor("main");
window.graph = editor.graph;
window.addEventListener("resize", function () { editor.graphcanvas.resize(); });
getNodes();




$(function () {

    //configure signalr
    var clientsHub = $.connection.clientsHub;

    clientsHub.client.OnConnectedEvent = function () {
        hardwareStateChanged(true);
    };

    clientsHub.client.OnDisconnectedEvent = function () {
        hardwareStateChanged(false);
    };



    clientsHub.client.OnLogicalNodeDeleteEvent = function (node) {
        var oldNode = graph.getNodeById(node.Id);
        graph.remove(oldNode);
        graph.setDirtyCanvas(true, true);
    };

    clientsHub.client.OnLogicalNodeUpdatedEvent = function (node) {
        createOrUpdateNode(node);
    };

    clientsHub.client.OnNewLogicalNodeEvent = function (node) {
        createOrUpdateNode(node);
    };


    $.connection.hub.start();

    $.connection.hub.stateChanged(function (change) {
        if (change.newState === $.signalR.connectionState.reconnecting) {
            noty({ text: 'Web server is not responding!', type: 'error', timeout: false });
            signalRServerConnected = false;
        }
        else if (change.newState === $.signalR.connectionState.connected) {
            if (signalRServerConnected == false) {
                noty({ text: 'Connected to web server.', type: 'alert', timeout: false });
                getIsHardwareConnected();
                getNodes();
            }
            signalRServerConnected = true;
        }
    });

    getIsHardwareConnected();
    getNodes();
});

function getIsHardwareConnected() {
    $.ajax({
        url: "/GatewayAPI/IsHardwareConnected/",
        type: "POST",
        success: function (connected) {
            hardwareStateChanged(connected);
        }
    });
}



function hardwareStateChanged(connected) {
    if (connected && gatewayHardwareConnected === false) {
        noty({ text: 'Gateway hardware is online.', type: 'alert', timeout: false });
    } else if (!connected) {
        noty({ text: 'Gateway hardware is offline!', type: 'error', timeout: false });
    }

    gatewayHardwareConnected = connected;
}






$("#sendButton").click(function () {
    //console.log(graph);
    var gr = JSON.stringify(graph.serialize());
    $.ajax({
        url: '/NodesEditorAPI/PutGraph',
        type: 'POST',
        data: { json: gr.toString() }
    }).done(function () {

    });
});




function getGraph() {

    $.ajax({
        url: "/NodesEditorAPI/GetGraph",
        type: "POST",
        success: function (loadedGraph) {
            graph.configure(loadedGraph);
        }
    });
}

function getNodes() {

    $.ajax({
        url: "/NodesEditorAPI/GetNodes",
        type: "POST",
        success: function (nodes) {
            onReturnNodes(nodes);
        }
    });
}



function onReturnNodes(nodes) {
    //console.log(nodes);
    if (!nodes) return;

    for (var i = 0; i < nodes.length; i++) {
        createOrUpdateNode(nodes[i]);
    }

    getLinks();
}


function createOrUpdateNode(node) {
    var newNode = LiteGraph.createNode(node.type);
    newNode.pos = node.pos;
    //newNode.title = node.title + " [" + node.id+"]";
    newNode.title = node.title;
    newNode.inputs = node.inputs;
    newNode.outputs = node.outputs;
    newNode.size = node.size;
    newNode.id = node.id;
    newNode.properties = node.properties;
    graph.add(newNode);
}



function getLinks() {

    $.ajax({
        url: "/NodesEditorAPI/GetLinks",
        type: "POST",
        success: function (links) {
            onReturnLinks(links);
        }
    });
}



function onReturnLinks(links) {
    //console.log(nodes);

    if (!links) return;

    for (var i = 0; i < links.length; i++) {
        createOrUpdateLink(links[i]);
    }
}


function createOrUpdateLink(link) {
    var target = graph.getNodeById(link.target_id);
    graph.getNodeById(link.origin_id)
        .connect(link.origin_slot, target, link.target_slot);
}






