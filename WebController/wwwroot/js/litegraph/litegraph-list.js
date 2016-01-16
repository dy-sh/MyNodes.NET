(function () {




    /*
     -------------------------------- PANELS -------------------------------------
    */







    //Panel
    function Panel() {
        this.properties = { 'objectType': "MyNetSensors.LogicalNodes.LogicalNodePanel" };
        this.bgcolor = "#565656";
    }
    Panel.title = "Panel";
    Panel.prototype.getExtraMenuOptions = function (graphcanvas) {
        var that = this;
        return [
            { content: "Open", callback: function () { window.location = "/NodesEditor/Panel/" + that.id; } },
            null, //null for horizontal line
            { content: "Show on Dashboard", callback: function () { var win = window.open("/Dashboard/Panel/" + that.id, '_blank'); win.focus(); } },
            null, //null for horizontal line
            { content: "Settings", callback: function () { PanelSettings(that) } },
            null
        ];
    }
    function PanelSettings(node) {
        $('#node-settings-title').html(node.type);

        $('#node-settings-body').html(
            '<div class="ui form"><div class="fields">' +
            '<div class="field">Name: <input type="text" id="node-settings-name"></div>' +
            '</div></div>'
        );

        $('#node-settings-name').val(node.properties['panelname']);


        $('#node-settings-panel').modal({
            dimmerSettings: { opacity: 0.3 },
            onApprove: function () {
                $.ajax({
                    url: "/NodesEditorAPI/PanelSettings/",
                    type: "POST",
                    data: {
                        panelname: $('#node-settings-name').val(),
                        id: node.id
                    }
                });
            }
        }).modal('setting', 'transition', 'fade up').modal('show');
    }
    LiteGraph.registerNodeType("Main/Panel", Panel);




    //Panel Input
    function PanelInput() {
        this.properties = { objectType: "MyNetSensors.LogicalNodes.LogicalNodePanelInput" };
        this.bgcolor = "#151515";

    }
    PanelInput.prototype.getExtraMenuOptions = function (graphcanvas) {
        var that = this;
        return [{ content: "Settings", callback: function () { InputOutputSettings(that) } }, null];
    }
    PanelInput.title = "Panel Input";
    LiteGraph.registerNodeType("Main/Panel Input", PanelInput);





    //Panel Output
    function PanelOutput() {
        this.properties = { objectType: "MyNetSensors.LogicalNodes.LogicalNodePanelOutput" };
        this.bgcolor = "#151515";
    }
    PanelOutput.prototype.getExtraMenuOptions = function (graphcanvas) {
        var that = this;
        return [{ content: "Settings", callback: function () { InputOutputSettings(that) } }, null];
    }
    PanelOutput.title = "Panel Output";
    LiteGraph.registerNodeType("Main/Panel Output", PanelOutput);


    function InputOutputSettings(node) {
        $('#node-settings-title').html(node.type);

        $('#node-settings-body').html(
            '<div class="ui form"><div class="fields">' +
            '<div class="field">Name: <input type="text" id="node-settings-name"></div>' +
            '</div></div>'
        );

        $('#node-settings-name').val(node.properties['name']);


        $('#node-settings-panel').modal({
            dimmerSettings: { opacity: 0.3 },
            onApprove: function () {
                $.ajax({
                    url: "/NodesEditorAPI/InputOutputSettings/",
                    type: "POST",
                    data: {
                        name: $('#node-settings-name').val(),
                        id: node.id
                    }
                });
            }
        }).modal('setting', 'transition', 'fade up').modal('show');
    }





    /*
     -------------------------------- HARDWARE -------------------------------------
    */









    //HardwareNode
    function HardwareNode() {
        this.properties = { 'objectType': "MyNetSensors.LogicalNodes.LogicalHardwareNode" };
        this.clonable = false;
        this.removable = false;
    }
    LiteGraph.registerNodeType("Nodes/HardwareNode", HardwareNode);













    /*
     -------------------------------- UI -------------------------------------
    */




    function UINodeSettings(node) {
        $('#node-settings-title').html(node.type);

        $('#node-settings-body').html(
            '<div class="ui form"><div class="fields">' +
            '<div class="field">Name: <input type="text" id="node-settings-name"></div>' +
            '</div><div class="fields">' +
            '<div class="field"><div class="ui toggle checkbox"><input type="checkbox" id="node-settings-show"><label>Show on Dashboard main page</label></div></div>' +

            '</div></div>'
        );

        $('#node-settings-name').val(node.properties['name']);
        $('#node-settings-show').prop('checked', node.properties['show'] == "true");


        $('#node-settings-panel').modal({
            dimmerSettings: { opacity: 0.3 },
            onApprove: function () {
                $.ajax({
                    url: "/NodesEditorAPI/UINodeSettings/",
                    type: "POST",
                    data: {
                        name: $('#node-settings-name').val(),
                        show: $('#node-settings-show').prop('checked'),
                        id: node.id
                    }
                });
            }
        }).modal('setting', 'transition', 'fade up').modal('show');
    }




    //UI Label
    function UILabel() {
        this.properties = { 'objectType': "MyNetSensors.LogicalNodesUI.LogicalNodeUILabel" };
    }
    UILabel.prototype.getExtraMenuOptions = function (graphcanvas) {
        var that = this;
        return [{ content: "Settings", callback: function () { UINodeSettings(that) } }, null];
    }
    UILabel.title = "Label";
    LiteGraph.registerNodeType("UI/Label", UILabel);



    //UI State
    function UIState() {
        this.properties = { 'objectType': "MyNetSensors.LogicalNodesUI.LogicalNodeUIState" };
    }
    UIState.prototype.getExtraMenuOptions = function (graphcanvas) {
        var that = this;
        return [{ content: "Settings", callback: function () { UINodeSettings(that) } }, null];
    }
    UIState.title = "State";
    LiteGraph.registerNodeType("UI/State", UIState);



    //UI Progress
    function UIProgress() {
        this.properties = { 'objectType': "MyNetSensors.LogicalNodesUI.LogicalNodeUIProgress" };
    }
    UIProgress.prototype.getExtraMenuOptions = function (graphcanvas) {
        var that = this;
        return [{ content: "Settings", callback: function () { UINodeSettings(that) } }, null];
    }
    UIProgress.title = "Progress";
    LiteGraph.registerNodeType("UI/Progress", UIProgress);



    //UI Log
    function UILog() {
        this.properties = { 'objectType': "MyNetSensors.LogicalNodesUI.LogicalNodeUILog" };
    }
    UILog.prototype.getExtraMenuOptions = function (graphcanvas) {
        var that = this;
        return [{ content: "Settings", callback: function () { UINodeSettings(that) } }, null];
    }
    UILog.title = "Log";
    LiteGraph.registerNodeType("UI/Log", UILog);



    //UI Button
    function UIButton() {
        this.properties = { 'objectType': "MyNetSensors.LogicalNodesUI.LogicalNodeUIButton" };
    }
    UIButton.prototype.getExtraMenuOptions = function (graphcanvas) {
        var that = this;
        return [{ content: "Settings", callback: function () { UINodeSettings(that) } }, null];
    }
    UIButton.title = "Button";
    LiteGraph.registerNodeType("UI/Button", UIButton);



    //UI Toggle Button
    function UIToggleButton() {
        this.properties = { 'objectType': "MyNetSensors.LogicalNodesUI.LogicalNodeUIToggleButton" };
    }
    UIToggleButton.prototype.getExtraMenuOptions = function (graphcanvas) {
        var that = this;
        return [{ content: "Settings", callback: function () { UINodeSettings(that) } }, null];
    }
    UIToggleButton.title = "Toggle Button";
    LiteGraph.registerNodeType("UI/Toggle Button", UIToggleButton);



    //UI Switch
    function UISwitch() {
        this.properties = { 'objectType': "MyNetSensors.LogicalNodesUI.LogicalNodeUISwitch" };
    }
    UISwitch.prototype.getExtraMenuOptions = function (graphcanvas) {
        var that = this;
        return [{ content: "Settings", callback: function () { UINodeSettings(that) } }, null];
    }
    UISwitch.title = "Switch";
    LiteGraph.registerNodeType("UI/Switch", UISwitch);



    //UI TextBox
    function UITextBox() {
        this.properties = { 'objectType': "MyNetSensors.LogicalNodesUI.LogicalNodeUITextBox" };
    }
    UITextBox.prototype.getExtraMenuOptions = function (graphcanvas) {
        var that = this;
        return [{ content: "Settings", callback: function () { UINodeSettings(that) } }, null];
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
    function UISliderSettings(node) {
        $('#node-settings-title').html(node.type);

        $('#node-settings-body').html(
            '<div class="ui form"><div class="fields">' +
            '<div class="field">Name: <input type="text" id="node-settings-name"></div>' +
            '<div class="field">Min Value:<input type="text" id="node-settings-min"></div>' +
            '<div class="field">Max Value:<input type="text" id="node-settings-max"></div>' +
            '</div><div class="fields">' +
            '<div class="field"><div class="ui toggle checkbox"><input type="checkbox" id="node-settings-show"><label>Show on Dashboard main page</label></div></div>' +

            '</div></div>'
        );

        $('#node-settings-name').val(node.properties['name']);
        $('#node-settings-min').val(node.properties['min']);
        $('#node-settings-max').val(node.properties['max']);
        $('#node-settings-show').prop('checked', node.properties['show'] == "true");


        $('#node-settings-panel').modal({
            dimmerSettings: { opacity: 0.3 },
            onApprove: function () {
                $.ajax({
                    url: "/NodesEditorAPI/UISliderSettings/",
                    type: "POST",
                    data: {
                        name: $('#node-settings-name').val(),
                        min: $('#node-settings-min').val(),
                        max: $('#node-settings-max').val(),
                        show: $('#node-settings-show').prop('checked'),
                        id: node.id
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
    UIRGBSliders.prototype.getExtraMenuOptions = function (graphcanvas) {
        var that = this;
        return [{ content: "Settings", callback: function () { UINodeSettings(that) } }, null];
    }
    UIRGBSliders.title = "RGB Sliders";
    LiteGraph.registerNodeType("UI/RGB Sliders", UIRGBSliders);



    //UI RGBW Sliders
    function UIRGBWSliders() {
        this.properties = { 'objectType': "MyNetSensors.LogicalNodesUI.LogicalNodeUIRGBWSliders" };
    }
    UIRGBWSliders.prototype.getExtraMenuOptions = function (graphcanvas) {
        var that = this;
        return [{ content: "Settings", callback: function () { UINodeSettings(that) } }, null];
    }
    UIRGBWSliders.title = "RGBW Sliders";
    LiteGraph.registerNodeType("UI/RGBW Sliders", UIRGBWSliders);








    //UI Chart
    function UIChart() {
        this.properties = { 'objectType': "MyNetSensors.LogicalNodesUI.LogicalNodeUIChart" };
    }
    UIChart.prototype.getExtraMenuOptions = function (graphcanvas) {
        var that = this;
        return [{ content: "Settings", callback: function () { UIChartSettings(that) } }, null];
    }
    function UIChartSettings(node) {
        $('#node-settings-title').html(node.type);

        $('#node-settings-body').html(
            '<div class="ui form"><div class="fields">' +
            '<div class="field">Name: <input type="text" id="node-settings-name"></div>' +
            '</div><div class="fields">' +
            '<div class="field"><div class="ui toggle checkbox"><input type="checkbox" id="node-settings-show"><label>Show on Dashboard main page</label></div></div>' +

            '</div></div>'
        );

        $('#node-settings-name').val(node.properties['name']);
        $('#node-settings-show').prop('checked', node.properties['show'] == "true");


        $('#node-settings-panel').modal({
            dimmerSettings: { opacity: 0.3 },
            onApprove: function () {
                $.ajax({
                    url: "/NodesEditorAPI/UIChartSettings/",
                    type: "POST",
                    data: {
                        name: $('#node-settings-name').val(),
                        show: $('#node-settings-show').prop('checked'),
                        id: node.id
                    }
                });
            }
        }).modal('setting', 'transition', 'fade up').modal('show');
    }
    UIChart.title = "Chart";
    LiteGraph.registerNodeType("UI/Chart", UIChart);







    /*
     -------------------------------- OTHER -------------------------------------
    */



    //Constant
    function Constant() {
        this.properties = { 'objectType': "MyNetSensors.LogicalNodes.LogicalNodeConstant" };
    }
    Constant.prototype.getExtraMenuOptions = function (graphcanvas) {
        var that = this;
        return [{ content: "Settings", callback: function () { ConstantSettings(that) } }, null];
    }
    function ConstantSettings(node) {
        $('#node-settings-title').html(node.type);

        $('#node-settings-body').html(
            '<div class="ui form"><div class="fields">' +
            '<div class="field">Value: <input type="text" id="node-settings-value"></div>' +
            '</div></div>'
        );

        $('#node-settings-value').val(node.properties['value']);


        $('#node-settings-panel').modal({
            dimmerSettings: { opacity: 0.3 },
            onApprove: function () {
                $.ajax({
                    url: "/NodesEditorAPI/ConstantSettings/",
                    type: "POST",
                    data: {
                        value: $('#node-settings-value').val(),
                        id: node.id
                    }
                });
            }
        }).modal('setting', 'transition', 'fade up').modal('show');
    }
    Constant.title = "Constant";
    LiteGraph.registerNodeType("Basic/Constant", Constant);




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









})();