//NOT FINISHED

function Editor(container_id, options)
{
	//fill container
	//var html = "<div class='header'><div class='tools tools-left'></div><div class='tools tools-right'></div></div>";
	var html = "<div class='content'><div class='editor-area'><canvas class='graphcanvas' width='1000' height='500' tabindex=10></canvas></div></div>";
	//html += "<div class='footer'><div class='tools tools-left'></div><div class='tools tools-right'></div></div>";
	
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

	//this.addToolsButton("minimap_button", "", "images/litegraph/icon-edit.png", this.onMinimapButton.bind(this), ".tools-right");
	//this.addToolsButton("reset_button", "", "images/litegraph/icon-stop.png", this.onResetButton.bind(this), ".tools-right");
	//this.addToolsButton("maximize_button", "", "images/litegraph/icon-maximize.png", this.onFullscreenButton.bind(this), ".tools-right");

	this.addMiniWindow(200,200);

	//append to DOM
	var	parent = document.getElementById(container_id);
	if(parent)
		parent.appendChild(root);

	graphcanvas.resize();
	//graphcanvas.draw(true,true);
}

var minimap_opened = false;


Editor.prototype.createPanel = function(title, options)
{

	var root = document.createElement("div");
	root.className = "dialog";
	root.innerHTML = "<div class='dialog-header'><span class='dialog-title'>"+title+"</span></div><div class='dialog-content'></div><div class='dialog-footer'></div>";
	root.header = root.querySelector(".dialog-header");
	root.content = root.querySelector(".dialog-content");
	root.footer = root.querySelector(".dialog-footer");


	return root;
}

Editor.prototype.createButton = function(name, icon_url)
{
	var button = document.createElement("button");
	if(icon_url)
		button.innerHTML = "<img src='"+icon_url+"'/> ";
	button.innerHTML += name;
	return button;
}


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