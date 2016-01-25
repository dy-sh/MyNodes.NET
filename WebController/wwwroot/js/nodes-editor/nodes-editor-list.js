(function () {




    /*
     -------------------------------- PANELS -------------------------------------
    */







    //Panel
    function Panel() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.PanelNode",
            'Assembly': "Nodes"
        };
        this.bgcolor = "#565656";
    }
    Panel.title = "Panel";
    Panel.prototype.getExtraMenuOptions = function (graphcanvas) {
        var that = this;
        return [
            { content: "Open", callback: function () { window.location = "/NodesEditor/Panel/" + that.id; } },
            null, //null for horizontal line
            { content: "Settings", callback: function () { PanelSettings(that) } },
            null,
            { content: "Show on Dashboard", callback: function () { var win = window.open("/Dashboard/Panel/" + that.id, '_blank'); win.focus(); } },
            null,
            { content: "Export to file", callback: function () { var win = window.open("/NodesEditorAPI/SerializePanelToFile/" + that.id, '_blank'); win.focus(); } },
            { content: "Export to script", callback: function () { editor.exportPanelToScript(that.id) } },
            { content: "Export URL", callback: function () { editor.exportPanelURL(that.id) } },
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

        $('#node-settings-name').val(node.properties['PanelName']);


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
        this.properties = {
            ObjectType: "MyNetSensors.Nodes.PanelInputNode",
            'Assembly': "Nodes"
        };
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
        this.properties = {
            ObjectType: "MyNetSensors.Nodes.PanelOutputNode",
            'Assembly': "Nodes"
        };
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

        $('#node-settings-name').val(node.properties['Name']);


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









    //MySensorsNode
    function MySensorsNode() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.MySensorsNode",
            'Assembly': "Nodes.MySensors"
        };
        this.clonable = false;
        this.removable = false;
    }
    LiteGraph.registerNodeType("Nodes/Hardware", MySensorsNode);













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

        $('#node-settings-name').val(node.properties['Name']);
        $('#node-settings-show').prop('checked', node.properties['ShowOnMainPage'] == "true");


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
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.UiLabelNode",
            'Assembly': "Nodes.UI"
        };
    }
    UILabel.prototype.getExtraMenuOptions = function (graphcanvas) {
        var that = this;
        return [{ content: "Settings", callback: function () { UINodeSettings(that) } }, null];
    }
    UILabel.title = "Label";
    LiteGraph.registerNodeType("UI/Label", UILabel);



    //UI State
    function UIState() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.UiStateNode",
            'Assembly': "Nodes.UI"
        };
    }
    UIState.prototype.getExtraMenuOptions = function (graphcanvas) {
        var that = this;
        return [{ content: "Settings", callback: function () { UINodeSettings(that) } }, null];
    }
    UIState.title = "State";
    LiteGraph.registerNodeType("UI/State", UIState);



    //UI Progress
    function UIProgress() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.UiProgressNode",
            'Assembly': "Nodes.UI"
        };
    }
    UIProgress.prototype.getExtraMenuOptions = function (graphcanvas) {
        var that = this;
        return [{ content: "Settings", callback: function () { UINodeSettings(that) } }, null];
    }
    UIProgress.title = "Progress";
    LiteGraph.registerNodeType("UI/Progress", UIProgress);



    //UI Log
    function UILog() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.UiLogNode",
            'Assembly': "Nodes.UI"
        };
    }
    UILog.prototype.getExtraMenuOptions = function (graphcanvas) {
        var that = this;
        return [{ content: "Settings", callback: function () { UINodeSettings(that) } }, null];
    }
    UILog.title = "Log";
    LiteGraph.registerNodeType("UI/Log", UILog);



    //UI Button
    function UIButton() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.UiButtonNode",
            'Assembly': "Nodes.UI"
        };
    }
    UIButton.prototype.getExtraMenuOptions = function (graphcanvas) {
        var that = this;
        return [{ content: "Settings", callback: function () { UINodeSettings(that) } }, null];
    }
    UIButton.title = "Button";
    LiteGraph.registerNodeType("UI/Button", UIButton);



    //UI Toggle Button
    function UIToggleButton() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.UiToggleButtonNode",
            'Assembly': "Nodes.UI"
        };
    }
    UIToggleButton.prototype.getExtraMenuOptions = function (graphcanvas) {
        var that = this;
        return [{ content: "Settings", callback: function () { UINodeSettings(that) } }, null];
    }
    UIToggleButton.title = "Toggle Button";
    LiteGraph.registerNodeType("UI/Toggle Button", UIToggleButton);



    //UI Switch
    function UISwitch() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.UiSwitchNode",
            'Assembly': "Nodes.UI"
        };
    }
    UISwitch.prototype.getExtraMenuOptions = function (graphcanvas) {
        var that = this;
        return [{ content: "Settings", callback: function () { UINodeSettings(that) } }, null];
    }
    UISwitch.title = "Switch";
    LiteGraph.registerNodeType("UI/Switch", UISwitch);



    //UI TextBox
    function UITextBox() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.UiTextBoxNode",
            'Assembly': "Nodes.UI"
        };
    }
    UITextBox.prototype.getExtraMenuOptions = function (graphcanvas) {
        var that = this;
        return [{ content: "Settings", callback: function () { UINodeSettings(that) } }, null];
    }
    UITextBox.title = "TextBox";
    LiteGraph.registerNodeType("UI/TextBox", UITextBox);






    //UI Slider
    function UISlider() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.UiSliderNode",
            'Assembly': "Nodes.UI"
        };
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
            '</div><div class="fields">' +
            '<div class="field"><div class="ui toggle checkbox"><input type="checkbox" id="node-settings-show"><label>Show on Dashboard main page</label></div></div>' +
            '</div><div class="ui divider"></div><div class="fields">' +
            '<div class="field">Min Value:<input type="text" id="node-settings-min"></div>' +
            '<div class="field">Max Value:<input type="text" id="node-settings-max"></div>' +
            '</div></div>'
        );

        $('#node-settings-name').val(node.properties['Name']);
        $('#node-settings-min').val(node.properties['Min']);
        $('#node-settings-max').val(node.properties['Max']);
        $('#node-settings-show').prop('checked', node.properties['ShowOnMainPage'] == "true");


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
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.UiRgbSlidersNode",
            'Assembly': "Nodes.UI"
        };
    }
    UIRGBSliders.prototype.getExtraMenuOptions = function (graphcanvas) {
        var that = this;
        return [{ content: "Settings", callback: function () { UINodeSettings(that) } }, null];
    }
    UIRGBSliders.title = "RGB Sliders";
    LiteGraph.registerNodeType("UI/RGB Sliders", UIRGBSliders);



    //UI RGBW Sliders
    function UIRGBWSliders() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.UiRgbwSlidersNode",
            'Assembly': "Nodes.UI"
        };
    }
    UIRGBWSliders.prototype.getExtraMenuOptions = function (graphcanvas) {
        var that = this;
        return [{ content: "Settings", callback: function () { UINodeSettings(that) } }, null];
    }
    UIRGBWSliders.title = "RGBW Sliders";
    LiteGraph.registerNodeType("UI/RGBW Sliders", UIRGBWSliders);








    //UI Chart
    function UIChart() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.UiChartNode",
            'Assembly': "Nodes.UI"
        };
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
            '</div><div class="ui divider"></div><div class="fields">' +
            '<div class="field"><div class="ui toggle checkbox"><input type="checkbox" id="node-settings-usedb"><label>Write data in database</label></div></div>' +
             '</div><div class="ui divider"></div><div class="fields">' +
            '<div class="field">Update interval (ms): <input type="text" id="node-settings-update"></div>' +

            '</div></div>'
        );

        $('#node-settings-name').val(node.properties['Name']);
        $('#node-settings-show').prop('checked', node.properties['ShowOnMainPage'] == "true");
        $('#node-settings-usedb').prop('checked', node.properties['WriteInDatabase'] == "true");
        $('#node-settings-update').val(node.properties['UpdateInterval']);



        $('#node-settings-panel').modal({
            dimmerSettings: { opacity: 0.3 },
            onApprove: function () {
                $.ajax({
                    url: "/NodesEditorAPI/UIChartSettings/",
                    type: "POST",
                    data: {
                        name: $('#node-settings-name').val(),
                        show: $('#node-settings-show').prop('checked'),
                        writeInDatabase: $('#node-settings-usedb').prop('checked'),
                        updateInterval: $('#node-settings-update').val(),
                        id: node.id
                    }
                });
            }
        }).modal('setting', 'transition', 'fade up').modal('show');
    }
    UIChart.title = "Chart";
    LiteGraph.registerNodeType("UI/Chart", UIChart);





    //UI Timer
    function UITimer() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.UiTimerNode",
            'Assembly': "Nodes.UITimer"
        };
    }
    UITimer.prototype.getExtraMenuOptions = function (graphcanvas) {
        var that = this;
        return [{ content: "Settings", callback: function () { UINodeSettings(that) } }, null,
            { content: "Open interface", callback: function () { var win = window.open("/UITimer/Tasks/" + that.id, '_blank'); win.focus(); } }, null
        ];
    }
    UITimer.title = "Timer";
    LiteGraph.registerNodeType("UI/Timer", UITimer);









    /*
     -------------------------------- OTHER -------------------------------------
    */



    //Constant
    function Constant() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.ConstantNode",
            'Assembly': "Nodes"
        };
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

        $('#node-settings-value').val(node.properties['Value']);


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
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.ConsoleNode",
            'Assembly': "Nodes"
        };
    }
    SystemConsole.title = "Console";
    LiteGraph.registerNodeType("System/Console", SystemConsole);



    //LogicCounter
    function LogicCounter() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.CounterNode",
            'Assembly': "Nodes"
        };
    }
    LogicCounter.title = "Counter";
    LiteGraph.registerNodeType("Operation/Counter", LogicCounter);




  //---------------------------------------------------------------------------------



    //LogicAnd
    function LogicAnd() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.LogicAndNode",
            'Assembly': "Nodes"
        };
    }
    LogicAnd.title = "AND";
    LiteGraph.registerNodeType("Logic/AND", LogicAnd);

    //LogicOr
    function LogicOr() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.LogicOrNode",
            'Assembly': "Nodes"
        };
    }
    LogicOr.title = "OR";
    LiteGraph.registerNodeType("Logic/OR", LogicOr);

    //LogicNot
    function LogicNot() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.LogicNotNode",
            'Assembly': "Nodes"
        };
    }
    LogicNot.title = "NOT";
    LiteGraph.registerNodeType("Logic/NOT", LogicNot);  
    


    //MathPlus
    function MathPlus() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.MathPlusNode",
            'Assembly': "Nodes"
        };
    }
    MathPlus.title = "Plus";
    LiteGraph.registerNodeType("Math/Plus", MathPlus);

    //MathMinus
    function MathMinus() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.MathMinusNode",
            'Assembly': "Nodes"
        };
    }
    MathMinus.title = "Minus";
    LiteGraph.registerNodeType("Math/Minus", MathMinus);

    //MathMultiply
    function MathMultiply() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.MathMultiplyNode",
            'Assembly': "Nodes"
        };
    }
    MathMultiply.title = "Multiply";
    LiteGraph.registerNodeType("Math/Multiply", MathMultiply);

    //MathDivide
    function MathDivide() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.MathDivideNode",
            'Assembly': "Nodes"
        };
    }
    MathDivide.title = "Divide";
    LiteGraph.registerNodeType("Math/Divide", MathDivide);

    //MathSin
    function MathSin() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.MathSinNode",
            'Assembly': "Nodes"
        };
    }
    MathSin.title = "Sin";
    LiteGraph.registerNodeType("Math/Sin", MathSin);

    //MathCos
    function MathCos() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.MathCosNode",
            'Assembly': "Nodes"
        };
    }
    MathCos.title = "Cos";
    LiteGraph.registerNodeType("Math/Cos", MathCos);

    //MathTan
    function MathTan() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.MathTanNode",
            'Assembly': "Nodes"
        };
    }
    MathTan.title = "Tan";
    LiteGraph.registerNodeType("Math/Tan", MathTan);

    //MathModulus
    function MathModulus() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.MathModulusNode",
            'Assembly': "Nodes"
        };
    }
    MathModulus.title = "Modulus";
    LiteGraph.registerNodeType("Math/Modulus", MathModulus);

    //MathSqrt
    function MathSqrt() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.MathSqrtNode",
            'Assembly': "Nodes"
        };
    }
    MathSqrt.title = "Sqrt";
    LiteGraph.registerNodeType("Math/Sqrt", MathSqrt);

    //MathPow
    function MathPow() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.MathPowNode",
            'Assembly': "Nodes"
        };
    }
    MathPow.title = "Pow";
    LiteGraph.registerNodeType("Math/Pow", MathPow);

    //MathRound
    function MathRound() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.MathRoundNode",
            'Assembly': "Nodes"
        };
    }
    MathRound.title = "Round";
    LiteGraph.registerNodeType("Math/Round", MathRound);

    //MathRemap
    function MathRemap() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.MathRemap",
            'Assembly': "Nodes"
        };
    }
    MathRemap.title = "Remap";
    LiteGraph.registerNodeType("Math/Remap", MathRemap);

    //MathClamp
    function MathClamp() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.MathClamp",
            'Assembly': "Nodes"
        };
    }
    MathClamp.title = "Clamp";
    LiteGraph.registerNodeType("Math/Clamp", MathClamp);

    //OperationGeneratorNode
    function OperationGeneratorNode() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.OperationGeneratorNode",
            'Assembly': "Nodes"
        };
    }
    OperationGeneratorNode.title = "Generator";
    LiteGraph.registerNodeType("Operation/Generator", OperationGeneratorNode);

    //OperationCompareEqual
    function OperationCompareEqual() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.OperationCompareEqualNode",
            'Assembly': "Nodes"
        };
    }
    OperationCompareEqual.title = "Compare Equal";
    LiteGraph.registerNodeType("Operation/Compare Equal", OperationCompareEqual);

    //OperationCompareNotEqual
    function OperationCompareNotEqual() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.OperationCompareNotEqualNode",
            'Assembly': "Nodes"
        };
    }
    OperationCompareNotEqual.title = "Compare NotEqual";
    LiteGraph.registerNodeType("Operation/Compare NotEqual", OperationCompareNotEqual);

    //OperationCompareGreater
    function OperationCompareGreater() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.OperationCompareGreaterNode",
            'Assembly': "Nodes"
        };
    }
    OperationCompareGreater.title = "Compare Greater";
    LiteGraph.registerNodeType("Operation/Compare Greater", OperationCompareGreater);

    //OperationCompareLower
    function OperationCompareLower() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.OperationCompareLowerNode",
            'Assembly': "Nodes"
        };
    }
    OperationCompareLower.title = "Compare Lower";
    LiteGraph.registerNodeType("Operation/Compare Lower", OperationCompareLower);

})();