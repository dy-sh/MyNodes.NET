
var editor = new LiteGraph.Editor("main");
window.graph = editor.graph;
window.addEventListener("resize", function () { editor.graphcanvas.resize(); });
getNodes();

$("#sendButton").click(function () {
    console.log(graph);
    var gr = JSON.stringify(graph.serialize());
    $.ajax({
        url: '/NodesEditor/PutGraph',
        type: 'POST',
        data: { json: gr.toString() }
    }).done(function () {

    });
});

function test()
{
    //var node1 = LiteGraph.createNode("Nodes/SimpleNode");
    //node1.pos = [200,200];
    //graph.add(node1);

    //var node2 = LiteGraph.createNode("Nodes/SimpleNode");
    //node2.pos = [200,300];
    //graph.add(node2);

	
    //node1.connect(0, node2, 0);
}



function getGraph() {

    $.ajax({
        url: "/NodesEditor/GetGraph",
        type: "POST",
        success: function (loadedGraph) {
            graph.configure(loadedGraph);
        }
    });
}

function getNodes() {

    $.ajax({
        url: "/NodesEditor/GetNodes",
        type: "POST",
        success: function (nodes) {
            onReturnNodes(nodes);
        }
    });
}



function onReturnNodes(nodes) {
    console.log(nodes);
    for (var i = 0; i < nodes.length; i++) {
        createOrUpdateNode(nodes[i]);
    }
}


function createOrUpdateNode(node) {
    var newNode = LiteGraph.createNode(node.type);
    newNode.pos = node.pos;
    newNode.title = node.title + " [" + node.id+"]";
    newNode.inputs = node.inputs;
    newNode.outputs = node.outputs;
    newNode.size = node.size;
    newNode.id = node.id;
    graph.add(newNode);
}

//function onRetunrLinks(links) {

//}


//function createOrUpdateLinks(link) {
//    var newLink = LiteGraph.createNode(link.type);
//    newLink.pos = link.pos;
//    graph.add(newLink);
//}

