/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

var signalRServerConnected;


var editor = new LiteGraph.Editor("main");
window.graph = editor.graph;
window.addEventListener("resize", function () { editor.graphcanvas.resize(); });
//getNodes();

var START_POS = 50;
var FREE_SPACE_UNDER = 30;
var NODE_WIDTH = 150;


$(function () {

    //configure signalr
    var clientsHub = $.connection.nodeEditorHub;

    clientsHub.client.OnGatewayConnected = function () {
        noty({ text: 'Gateway is connected.', type: 'alert', timeout: false });
    };

    clientsHub.client.OnGatewayDisconnected = function () {
        noty({ text: 'Gateway is disconnected!', type: 'error', timeout: false });
    };

    clientsHub.client.OnRemoveAllNodesAndLinks = function () {
        graph.clear();
        window.location.replace("/NodeEditor/");
        noty({ text: 'All nodes have been deleted!', type: 'error' });
    };

    clientsHub.client.OnNodeActivity = function (nodeId) {
        var node = graph.getNodeById(nodeId);
        if (node == null)
            return;

        node.boxcolor = LiteGraph.NODE_ACTIVE_BOXCOLOR;
        node.setDirtyCanvas(true, true);
        setTimeout(function () {
            node.boxcolor = LiteGraph.NODE_DEFAULT_BOXCOLOR;
            node.setDirtyCanvas(true, true);
        }, 100);
    };



    clientsHub.client.OnRemoveNode = function (nodeId) {
        //if current panel removed
        if (nodeId == this_panel_id) {
            window.location = "/NodeEditor/";
        }

        var node = graph.getNodeById(nodeId);
        if (node == null)
            return;

        graph.remove(node);
        graph.setDirtyCanvas(true, true);
    };


    clientsHub.client.OnNodeUpdated = function (node) {
        if (node.panel_id != window.this_panel_id)
            return;

        createOrUpdateNode(node);
    };


    clientsHub.client.OnNewNode = function (node) {
        if (node.panel_id != window.this_panel_id)
            return;

        createOrUpdateNode(node);
    };


    clientsHub.client.OnRemoveLink = function (link) {
        if (link.panel_id != window.this_panel_id)
            return;

        //var node = graph.getNodeById(link.origin_id);
        var targetNode = graph.getNodeById(link.target_id);
        //node.disconnectOutput(link.target_slot, targetNode);
        targetNode.disconnectInput(link.target_slot);
    };

    clientsHub.client.OnNewLink = function (link) {
        if (link.panel_id != window.this_panel_id)
            return;

        var node = graph.getNodeById(link.origin_id);
        var targetNode = graph.getNodeById(link.target_id);
        node.connect(link.origin_slot, targetNode, link.target_slot, link.id);
        //  graph.change();

    };


    $.connection.hub.start();

    $.connection.hub.stateChanged(function (change) {
        if (change.newState === $.signalR.connectionState.reconnecting) {
            $("#main").fadeOut(300);
            noty({ text: 'Web server is not responding!', type: 'error' });
            signalRServerConnected = false;
        }
        else if (change.newState === $.signalR.connectionState.connected) {
            if (signalRServerConnected == false) {
                noty({ text: 'Connected to web server.', type: 'alert' });
                //waiting while server initialized and read db
                setTimeout(function () {
                    getNodes();
                    getGatewayInfo();
                    $("#main").fadeIn(300);
                }, 2000);


            }
            signalRServerConnected = true;
        }
    });

    getNodes();
    getGatewayInfo();
});


function getGatewayInfo() {
    $.ajax({
        url: "/GatewayAPI/GetGatewayInfo/",
        type: "POST",
        success: function (gatewayInfo) {
            if (gatewayInfo.state == 1 || gatewayInfo.state == 2) {
                noty({ text: 'Gateway is not connected!', type: 'error', timeout: false });
            }
        }
    });
}






$("#sendButton").click(function () {
    //console.log(graph);
    var gr = JSON.stringify(graph.serialize());
    $.ajax({
        url: '/NodeEditorAPI/PutGraph',
        type: 'POST',
        data: { json: gr.toString() }
    }).done(function () {

    });
});




$("#fullscreen-button").click(function () {
    // editor.goFullscreen();

    var elem = document.documentElement;

    var fullscreenElement =
  document.fullscreenElement ||
  document.mozFullscreenElement ||
  document.webkitFullscreenElement;

    if (fullscreenElement == null) {
        if (elem.requestFullscreen) {
            elem.requestFullscreen();
        } else if (elem.mozRequestFullScreen) {
            elem.mozRequestFullScreen();
        } else if (elem.webkitRequestFullscreen) {
            elem.webkitRequestFullscreen();
        }
    } else {
        if (document.cancelFullScreen) {
            document.cancelFullScreen();
        } else if (document.mozCancelFullScreen) {
            document.mozCancelFullScreen();
        } else if (document.webkitCancelFullScreen) {
            document.webkitCancelFullScreen();
        }
    }
});


function send_create_link(link) {

    $.ajax({
        url: '/NodeEditorAPI/CreateLink',
        type: 'POST',
        data: { 'link': link }
    }).done(function () {

    });
};

function send_remove_link(link) {

    $.ajax({
        url: '/NodeEditorAPI/RemoveLink',
        type: 'POST',
        data: { 'link': link }
    }).done(function () {

    });
};

function send_create_node(node) {
    node.size = null;//reset size for autosizing

    var serializedNode = node.serialize();
    $.ajax({
        url: '/NodeEditorAPI/AddNode',
        type: 'POST',
        data: { 'node': serializedNode }
    }).done(function () {

    });
};

function send_clone_node(node) {
    $.ajax({
        url: '/NodeEditorAPI/CloneNode',
        type: 'POST',
        data: { 'id': node.id }
    }).done(function () {

    });
};

function send_remove_node(node) {

    var serializedNode = node.serialize();
    $.ajax({
        url: '/NodeEditorAPI/RemoveNode',
        type: 'POST',
        data: { 'node': serializedNode }
    }).done(function () {

    });
};

function send_remove_nodes(nodes) {

    var array = [];

    for (var n in nodes) {
        array.push(nodes[n].id);
    }

    $.ajax({
        url: '/NodeEditorAPI/RemoveNodes',
        type: 'POST',
        data: { 'nodes': array }
    }).done(function () {

    });
};

function send_update_node(node) {

    var serializedNode = node.serialize();
    $.ajax({
        url: '/NodeEditorAPI/UpdateNode',
        type: 'POST',
        data: { 'node': serializedNode }
    }).done(function () {

    });
};


function getGraph() {

    $.ajax({
        url: "/NodeEditorAPI/GetGraph",
        type: "POST",
        success: function (loadedGraph) {
            graph.configure(loadedGraph);
        }
    });
}

function getNodes() {

    $.ajax({
        url: "/NodeEditorAPI/GetNodes",
        type: "POST",
        data: { 'panelId': window.this_panel_id },
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

        if (newNode==null) {
            console.error("Can`t create node of type: ["+node.type+"]");
            return;
        };

        newNode.title = node.title;

        newNode.inputs = node.inputs;
        newNode.outputs = node.outputs;
        newNode.id = node.id;
        newNode.properties = node.properties;

        //calculate size
        if (node.size)
            newNode.size = node.size;
        else
            newNode.size = newNode.computeSize();

        newNode.size[1] = calculateNodeMinHeight(newNode);

        //calculate pos
        if (node.pos)
            newNode.pos = node.pos;
        else
            newNode.pos = [START_POS, findFreeSpaceY(newNode)];

        graph.add(newNode);
    } else {
        //update
        oldNode.title = node.title;

        if (node.properties['Name'] != null)
            oldNode.title += " [" + node.properties['Name'] + "]";

        if (node.properties['PanelName'] != null)
            oldNode.title = node.properties['PanelName'];

        oldNode.inputs = node.inputs;
        oldNode.outputs = node.outputs;
        oldNode.id = node.id;
        oldNode.properties = node.properties;

        //calculate size
        if (node.size)
            oldNode.size = node.size;
        else
            oldNode.size = oldNode.computeSize();

        oldNode.size[1] = calculateNodeMinHeight(oldNode);

        //calculate pos

        if (node.pos) {
            if(!editor.graphcanvas.node_dragged)
                oldNode.pos = node.pos;
            else if(!editor.graphcanvas.selected_nodes[node.id])
                oldNode.pos = node.pos;
        }

        oldNode.setDirtyCanvas(true, true);
    }
}



function getLinks() {

    $.ajax({
        url: "/NodeEditorAPI/GetLinks",
        type: "POST",
        data: { 'panelId': window.this_panel_id },
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
        .connect(link.origin_slot, target, link.target_slot, link.id);
}






function calculateNodeMinHeight(node) {

    var slotsMax = (node.outputs.length > node.inputs.length) ? node.outputs.length : node.inputs.length;
    if (slotsMax == 0)
        slotsMax = 1;

    var height = LiteGraph.NODE_SLOT_HEIGHT * slotsMax;

    return height + 5;
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



