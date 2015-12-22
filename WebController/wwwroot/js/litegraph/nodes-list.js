(function(){

//Simple Node
function SimpleNode()
{
	this.addInput("in","number");
	this.addOutput("out1", "number");
	this.size = [100,20];
	this.properties = {min:0, max:1};
}

SimpleNode.title = "SimpleNode";
    
LiteGraph.registerNodeType("Nodes/SimpleNode", SimpleNode );






//Simple Node IN
function SimpleIn() {
    this.addInput("in", "number");
    this.size = [100, 20];
    this.properties = { min: 0, max: 1 };
}

SimpleIn.title = "SimpleIn";

LiteGraph.registerNodeType("Nodes/SimpleIn", SimpleIn);






//Simple Node OUT
function SimpleOut() {
    this.addOutput("out", "number");
    this.size = [100, 20];
    this.properties = { min: 0, max: 1 };
}

SimpleOut.title = "SimpleOut";

LiteGraph.registerNodeType("Nodes/SimpleOut", SimpleOut);

})();