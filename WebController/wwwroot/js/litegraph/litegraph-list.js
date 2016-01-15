(function () {



    //Panel: a node that contains a subgraph
    function Panel() {
        this.properties = { 'objectType': "MyNetSensors.LogicalNodes.LogicalNodePanel" };
        this.bgcolor = "#565656";
    }
    Panel.title = "Panel";
    Panel.prototype.getExtraMenuOptions = function (graphcanvas) {
        var that = this;
        return [{
            content: "Open", callback:
                function () {
                    window.location = "/NodesEditor/?panelId=" + that.id;
                }
        }, null];//null for horizontal line
    }
    LiteGraph.registerNodeType("Main/Panel", Panel);


    //Panel Input
    function PanelInput() {
        this.properties = { objectType: "MyNetSensors.LogicalNodes.LogicalNodePanelInput" };
        this.bgcolor = "#151515";

    }
    PanelInput.title = "Panel Input";
    LiteGraph.registerNodeType("Main/Panel Input", PanelInput);



    //Panel Output
    function PanelOutput() {
        this.properties = { objectType: "MyNetSensors.LogicalNodes.LogicalNodePanelOutput" };
        this.bgcolor = "#151515";
    }
    PanelOutput.title = "Panel Output";
    LiteGraph.registerNodeType("Main/Panel Output", PanelOutput);











    //HardwareNode
    function HardwareNode() {
        this.properties = { 'objectType': "MyNetSensors.LogicalNodes.LogicalHardwareNode" };
        this.clonable = false;
        this.removable = false;
    }
    LiteGraph.registerNodeType("Nodes/HardwareNode", HardwareNode);







    //SystemConsole
    function SystemConsole() {
        this.properties = { 'objectType': "MyNetSensors.LogicalNodes.LogicalNodeConsole" };
    }
    SystemConsole.title = "Console";
    LiteGraph.registerNodeType("System/Console", SystemConsole);



    //LogicCounter
    function LogicCounter() {
        this.properties = { 'objectType': "MyNetSensors.LogicalNodes.LogicalNodeCounter" };
    }
    LogicCounter.title = "Counter";
    LiteGraph.registerNodeType("Logic/Counter", LogicCounter);



    //LogicInvert
    function LogicInvert() {
        this.properties = { 'objectType': "MyNetSensors.LogicalNodes.LogicalNodeInvert" };
    }
    LogicInvert.title = "Invert";
    LiteGraph.registerNodeType("Logic/Invert", LogicInvert);



    //MathPlus
    function MathPlus() {
        this.properties = { 'objectType': "MyNetSensors.LogicalNodes.LogicalNodeMathPlus" };
    }
    MathPlus.title = "Plus";
    LiteGraph.registerNodeType("Math/Plus", MathPlus);








    //UI Label
    function UILabel() {
        this.properties = { 'objectType': "MyNetSensors.LogicalNodesUI.LogicalNodeUILabel" };
    }
    UILabel.title = "Label";
    LiteGraph.registerNodeType("UI/Label", UILabel);

    //UI State
    function UIState() {
        this.properties = { 'objectType': "MyNetSensors.LogicalNodesUI.LogicalNodeUIState" };
    }
    UIState.title = "State";
    LiteGraph.registerNodeType("UI/State", UIState);

    //UI Progress
    function UIProgress() {
        this.properties = { 'objectType': "MyNetSensors.LogicalNodesUI.LogicalNodeUIProgress" };
    }
    UIProgress.title = "Progress";
    LiteGraph.registerNodeType("UI/Progress", UIProgress);

    //UI Log
    function UILog() {
        this.properties = { 'objectType': "MyNetSensors.LogicalNodesUI.LogicalNodeUILog" };
    }
    UILog.title = "Log";
    LiteGraph.registerNodeType("UI/Log", UILog);










    //UI Button
    function UIButton() {
        this.properties = { 'objectType': "MyNetSensors.LogicalNodesUI.LogicalNodeUIButton" };
    }
    UIButton.title = "Button";
    LiteGraph.registerNodeType("UI/Button", UIButton);



    //UI Toggle Button
    function UIToggleButton() {
        this.properties = { 'objectType': "MyNetSensors.LogicalNodesUI.LogicalNodeUIToggleButton" };
    }
    UIToggleButton.title = "Toggle Button";
    LiteGraph.registerNodeType("UI/Toggle Button", UIToggleButton);



    //UI Switch
    function UISwitch() {
        this.properties = { 'objectType': "MyNetSensors.LogicalNodesUI.LogicalNodeUISwitch" };
    }
    UISwitch.title = "Switch";
    LiteGraph.registerNodeType("UI/Switch", UISwitch);



    //UI TextBox
    function UITextBox() {
        this.properties = { 'objectType': "MyNetSensors.LogicalNodesUI.LogicalNodeUITextBox" };
    }
    UITextBox.title = "TextBox";
    LiteGraph.registerNodeType("UI/TextBox", UITextBox);






    //UI Slider
    function UISlider() {
        this.properties = { 'objectType': "MyNetSensors.LogicalNodesUI.LogicalNodeUISlider" };
    }

    UISlider.prototype.getExtraMenuOptions = function (graphcanvas) {
        var that = this;
        return [{ content: "Settings", callback: function () { UISliderSettings(that) } }, null];
    }
    function UISliderSettings(that) {
        $('#node-settings-title').html(that.title);

        $('#node-settings-body').html(
            '<div class="ui form"><div class="fields">' +
            '<div class="field">Name: <input type="text" id="node-settings-name"></div>' +
            '<div class="field">Min Value:<input type="text" id="node-settings-min"></div>' +
            '<div class="field">Max Value:<input type="text" id="node-settings-max"></div>' +
            '</div></div>'
        );

        $('#node-settings-name').val(that.title);
        $('#node-settings-min').val(that.properties['min']);
        $('#node-settings-max').val(that.properties['max']);


        $('#node-settings-panel').modal({
            dimmerSettings: {opacity: 0.3},
            onApprove: function () {
                $.ajax({
                    url: "/NodesEditorAPI/SliderSettings/",
                    type: "POST",
                    data: {
                        name: $('#node-settings-name').val(),
                        min: $('#node-settings-min').val(),
                        max: $('#node-settings-max').val(),
                        id: that.id
                    }
                });
            }
        }).modal('setting', 'transition', 'fade up').modal('show');
    }

    UISlider.title = "Slider";
    LiteGraph.registerNodeType("UI/Slider", UISlider);





    //UI RGB Sliders
    function UIRGBSliders() {
        this.properties = { 'objectType': "MyNetSensors.LogicalNodesUI.LogicalNodeUIRGBSliders" };
    }
    UIRGBSliders.title = "RGB Sliders";
    LiteGraph.registerNodeType("UI/RGB Sliders", UIRGBSliders);



    //UI RGBW Sliders
    function UIRGBWSliders() {
        this.properties = { 'objectType': "MyNetSensors.LogicalNodesUI.LogicalNodeUIRGBWSliders" };
    }
    UIRGBWSliders.title = "RGBW Sliders";
    LiteGraph.registerNodeType("UI/RGBW Sliders", UIRGBWSliders);







})();