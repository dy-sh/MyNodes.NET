
function Editor(container_id, options)
{
	//fill container
	var html = "<div class='content'><div class='editor-area'><canvas class='graphcanvas' width='1000' height='500' tabindex=10></canvas></div></div>";
	
	var root = document.createElement("div");
	this.root = root;
	root.className = "litegraph-editor";
	root.innerHTML = html;

	var canvas = root.querySelector(".graphcanvas");

	//create graph
	var graph = this.graph = new LGraph();
	var graphcanvas = this.graphcanvas = new LGraphCanvas(canvas,graph);
	graphcanvas.background_image = "/images/litegraph/grid.png";
	graph.onAfterExecute = function() { graphcanvas.draw(true) };

	//add stuff

	this.addMiniWindow(200,200);

	//append to DOM
	var	parent = document.getElementById(container_id);
	if(parent)
		parent.appendChild(root);

	graphcanvas.resize();
	//graphcanvas.draw(true,true);
}

var minimap_opened = false;

// noty settings
$.noty.defaults.layout = 'bottomRight';
$.noty.defaults.theme = 'relax';
$.noty.defaults.timeout = 3000;
$.noty.defaults.animation = {
    open: 'animated bounceInRight', // Animate.css class names
    close: 'animated flipOutX', // Animate.css class names
    easing: 'swing', // unavailable - no need
    speed: 500 // unavailable - no need
};

Editor.prototype.addMiniWindow = function (w, h) {

    if (minimap_opened) 
        return;

    minimap_opened = true;

	var miniwindow = document.createElement("div");
	miniwindow.className = "litegraph miniwindow";
	miniwindow.innerHTML = "<canvas class='graphcanvas' width='"+w+"' height='"+h+"' tabindex=10></canvas>";
	var canvas = miniwindow.querySelector("canvas");

	var graphcanvas = new LGraphCanvas(canvas, this.graph);
	graphcanvas.background_image = "images/litegraph/grid.png";
    //derwish edit
	graphcanvas.scale = 0.1;
    //graphcanvas.allow_dragnodes = false;

	graphcanvas.offset = [0, 0];
	graphcanvas.scale = 0.1;
	graphcanvas.setZoom(0.1, [1, 1]);

	miniwindow.style.position = "absolute";
	miniwindow.style.top = "4px";
	miniwindow.style.right = "4px";

	var close_button = document.createElement("div");
	close_button.className = "corner-button";
	close_button.innerHTML = "X";
	close_button.addEventListener("click", function (e) {
	    minimap_opened = false;
		graphcanvas.setGraph(null);
		miniwindow.parentNode.removeChild(miniwindow);
	});
	miniwindow.appendChild(close_button);

    //derwiah added
	var reset_button = document.createElement("div");
	reset_button.className = "corner-button2";
	reset_button.innerHTML = "R";
	reset_button.addEventListener("click", function (e) {
	    graphcanvas.offset = [0, 0];
	    graphcanvas.scale = 0.1;
	    graphcanvas.setZoom (0.1, [1, 1]);
	});
	miniwindow.appendChild(reset_button);

	this.root.querySelector(".content").appendChild(miniwindow);

}


LiteGraph.Editor = Editor;