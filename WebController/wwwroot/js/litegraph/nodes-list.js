(function () {



    //HardwareNode
    function HardwareNode() {
        this.size = [150, 20];
        this.properties = { min: 0, max: 1 };
    }

    LiteGraph.registerNodeType("Nodes/HardwareNode", HardwareNode);


    //SystemConsole
    function SystemConsole() {
        this.addInput("in", "string");
        this.size = [150, 30];
        this.properties = { min: 0, max: 1 };
    }
    SystemConsole.title = "System Console";

    LiteGraph.registerNodeType("System/Console", SystemConsole);


    //LogicCounter
    function LogicCounter() {
        this.addInput("freq", "string");
        this.addOutput("out", "string");
        this.size = [150, 30];
        this.properties = { min: 0, max: 1 };
    }
    LogicCounter.title = "Logic Counter";

    LiteGraph.registerNodeType("Logic/Counter", LogicCounter);


    //LogicInvert
    function LogicInvert() {
        this.addInput("in", "string");
        this.addOutput("out", "string");
        this.size = [150, 30];
        this.properties = { min: 0, max: 1 };
    }
    LogicInvert.title = "Logic Invert";

    LiteGraph.registerNodeType("Logic/Invert", LogicInvert);


    //MathPlus
    function MathPlus() {
        this.addInput("in1", "string");
        this.addInput("in2", "string");
        this.addOutput("out", "string");
        this.size = [150, 45];
        this.properties = { min: 0, max: 1 };
    }
    MathPlus.title = "MathPlus";

    LiteGraph.registerNodeType("Math/Plus", MathPlus);



})();