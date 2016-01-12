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



})();