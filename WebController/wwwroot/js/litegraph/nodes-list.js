(function () {



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


    //UI Log
    function UILog() {
        this.properties = { 'objectType': "MyNetSensors.LogicalNodesUI.LogicalNodeUILog" };
    }
    UILog.title = "Log";
    LiteGraph.registerNodeType("UI/Log", UILog);


    //UI Progress
    function UIProgress() {
        this.properties = { 'objectType': "MyNetSensors.LogicalNodesUI.LogicalNodeUIProgress" };
    }
    UIProgress.title = "Progress";
    LiteGraph.registerNodeType("UI/Progress", UIProgress);



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



    //UI Slider
    function UISlider() {
        this.properties = { 'objectType': "MyNetSensors.LogicalNodesUI.LogicalNodeUISlider" };
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