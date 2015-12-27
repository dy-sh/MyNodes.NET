(function () {



    //HardwareNode
    function HardwareNode() {
        this.size = [150, 20];
        this.properties = { 'objectType': "MyNetSensors.LogicalNodes.LogicalNodeMySensors" };
        this.clonable = false;
        this.removable = false;
    }

    LiteGraph.registerNodeType("Nodes/HardwareNode", HardwareNode);


    //SystemConsole
    function SystemConsole() {
        this.addInput("in", "string");
        this.size = [150, 30];
        this.properties = { 'objectType': "MyNetSensors.LogicalNodes.LogicalNodeConsole" };
    }
    SystemConsole.title = "System Console";

    LiteGraph.registerNodeType("System/Console", SystemConsole);


    //LogicCounter
    function LogicCounter() {
        this.addInput("freq", "string");
        this.addOutput("out", "string");
        this.size = [150, 30];
        this.properties = { 'objectType': "MyNetSensors.LogicalNodes.LogicalNodeCounter" };

    }
    LogicCounter.title = "Logic Counter";

    LiteGraph.registerNodeType("Logic/Counter", LogicCounter);


    //LogicInvert
    function LogicInvert() {
        this.addInput("in", "string");
        this.addOutput("out", "string");
        this.size = [150, 30];
        this.properties = { 'objectType': "MyNetSensors.LogicalNodes.LogicalNodeInvert" };

    }
    LogicInvert.title = "Logic Invert";

    LiteGraph.registerNodeType("Logic/Invert", LogicInvert);


    //MathPlus
    function MathPlus() {
        this.addInput("in1", "string");
        this.addInput("in2", "string");
        this.addOutput("out", "string");
        this.size = [150, 45];
        this.properties = { 'objectType': "MyNetSensors.LogicalNodes.LogicalNodeMathPlus" };

    }
    MathPlus.title = "MathPlus";

    LiteGraph.registerNodeType("Math/Plus", MathPlus);



})();