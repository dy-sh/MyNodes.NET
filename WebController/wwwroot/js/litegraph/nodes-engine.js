/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

var gatewayHardwareConnected = null;
var signalRServerConnected = null;


var editor = new LiteGraph.Editor("main");
window.graph = editor.graph;
window.addEventListener("resize", function () { editor.graphcanvas.resize(); });
//getNodes();

var START_POS = 50;
var FREE_SPACE_UNDER = 30;
var NODE_WIDTH = 150;


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
        var oldNode = graph.getNodeById(node.id);
        graph.remove(oldNode);
        graph.setDirtyCanvas(true, true);
    };

    clientsHub.client.OnLogicalNodeUpdatedEvent = function (node) {
        createOrUpdateNode(node);
    };

    clientsHub.client.OnNewLogicalNodeEvent = function (node) {
        createOrUpdateNode(node);
    };


    clientsHub.client.OnDeleteLinkEvent = function (link) {
        var node = graph.getNodeById(link.origin_id);
        var targetNode = graph.getNodeById(link.target_id);
        node.disconnectOutput(link.target_slot, targetNode);
        targetNode.disconnectInput(link.target_slot);
    };

    clientsHub.client.OnNewLinkEvent = function (link) {
        var node = graph.getNodeById(link.origin_id);
        var targetNode = graph.getNodeById(link.target_id);
        node.connect(link.origin_slot, targetNode, link.target_slot);
      //  graph.change();

    };

    clientsHub.client.OnLinksUpdatedEvent = function (links) {

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


function send_create_link(link) {

    $.ajax({
        url: '/NodesEditorAPI/CreateLink',
        type: 'POST',
        data: { 'link': link }
    }).done(function () {

    });
};

function send_delete_link(link) {

    $.ajax({
        url: '/NodesEditorAPI/DeleteLink',
        type: 'POST',
        data: { 'link': link }
    }).done(function () {

    });
};

function send_create_node(node) {

    var serializedNode = node.serialize();
    $.ajax({
        url: '/NodesEditorAPI/CreateNode',
        type: 'POST',
        data: { 'node': serializedNode }
    }).done(function () {

    });
};

function send_delete_node(node) {

    var serializedNode = node.serialize();
    $.ajax({
        url: '/NodesEditorAPI/DeleteNode',
        type: 'POST',
        data: { 'node': serializedNode }
    }).done(function () {

    });
};


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
    var oldNode = graph.getNodeById(node.id);
    if (!oldNode) {
        //create new
        var newNode = LiteGraph.createNode(node.type);
        //newNode.title = node.title + " [" + node.id+"]";
        newNode.title = node.title;
        newNode.inputs = node.inputs;
        newNode.outputs = node.outputs;
        newNode.id = node.id;
        newNode.properties = node.properties;

        newNode.size = [NODE_WIDTH, calculateNodeHeight(newNode)];

        newNode.pos = node.pos;
        if (!newNode.pos)
            newNode.pos = [START_POS, findFreeSpaceY(newNode)];

        graph.add(newNode);
    } else {
        //update
        //newNode.title = node.title + " [" + node.id+"]";
        oldNode.title = node.title;
        oldNode.inputs = node.inputs;
        oldNode.outputs = node.outputs;
        oldNode.id = node.id;
        oldNode.properties = node.properties;

        oldNode.size = [NODE_WIDTH, calculateNodeHeight(oldNode)];

        //if (node.pos)
        //    oldNode.pos = node.pos;
        //if (!oldNode.pos)
        //    oldNode.pos = [START_POS, findFreeSpaceY(oldNode)];

        oldNode.setDirtyCanvas(true, true);
    }
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






function calculateNodeHeight(node) {
    var SLOT_SIZE = 15;
    var sizeOutY = 0, sizeInY = 0;

    if (node.outputs)
        sizeOutY = SLOT_SIZE + (SLOT_SIZE * node.outputs.length);
    if (node.inputs)
        sizeInY = SLOT_SIZE + (SLOT_SIZE * node.inputs.length);

    return (sizeOutY > sizeInY) ? sizeOutY : sizeInY;
}



function findFreeSpaceY(node) {


    var nodes = graph._nodes;


    node.pos = [0, 0];

    var result = START_POS;


    for (var i = 0; i < nodes.length; i++) {
        var needFromY = result;
        var needToY = result + node.size[1];

        if (node.id == nodes[i].id)
            continue;

        if (!nodes[i].pos)
            continue;

        if (nodes[i].pos[0] > NODE_WIDTH + 20 + START_POS)
            continue;

        var occupyFromY = nodes[i].pos[1] - FREE_SPACE_UNDER;
        var occupyToY = nodes[i].pos[1] + nodes[i].size[1];

        if (occupyFromY <= needToY && occupyToY >= needFromY) {
            result = occupyToY + FREE_SPACE_UNDER;
            i = -1;
        }
    }

    return result;

}