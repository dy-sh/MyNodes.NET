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

    //UI Switch Button
    function UISwitchButton() {
        this.properties = { 'objectType': "MyNetSensors.LogicalNodesUI.LogicalNodeUISwitchButton" };
    }
    UISwitchButton.title = "Switch Button";
    LiteGraph.registerNodeType("UI/Switch Button", UISwitchButton);

    //UI Slider
    function UISlider() {
        this.properties = { 'objectType': "MyNetSensors.LogicalNodesUI.LogicalNodeUISlider" };
    }
    UISlider.title = "Slider";
    LiteGraph.registerNodeType("UI/Slider", UISlider);

})();