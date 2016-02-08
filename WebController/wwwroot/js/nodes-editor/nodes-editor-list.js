(function () {




    /*
     -------------------------------- PANELS -------------------------------------
    */







    //Panel
    function PanelNode() {
        this.properties = {
            'ObjectType': 'MyNetSensors.Nodes.PanelNode',
            'Assembly': 'Nodes'
        };
        this.bgcolor = '#565656';
    }
    PanelNode.title = 'Panel';
    PanelNode.prototype.getExtraMenuOptions = function (graphcanvas) {
        var that = this;
        return [
            { content: 'Open', callback: function () { window.location = '/NodesEditor/Panel/' + that.id; } },
            null, //null for horizontal line
            { content: 'Show on Dashboard', callback: function () { var win = window.open('/Dashboard/Panel/' + that.id, '_blank'); win.focus(); } },
            null,
            { content: 'Export to file', callback: function () { var win = window.open('/NodesEditorAPI/SerializePanelToFile/' + that.id, '_blank'); win.focus(); } },
            { content: 'Export to script', callback: function () { editor.exportPanelToScript(that.id) } },
            { content: 'Export URL', callback: function () { editor.exportPanelURL(that.id) } },
            null
        ];
    }
    LiteGraph.registerNodeType('Main/Panel', PanelNode);




    //PanelOutputNode
    function PanelInputNode() {
        this.properties = {
            ObjectType: 'MyNetSensors.Nodes.PanelInputNode',
            'Assembly': 'Nodes'
        };
        this.bgcolor = '#151515';

    }
    PanelInputNode.title = 'Panel Input';
    LiteGraph.registerNodeType('Main/Panel Input', PanelInputNode);





    //PanelOutputNode
    function PanelOutputNode() {
        this.properties = {
            ObjectType: 'MyNetSensors.Nodes.PanelOutputNode',
            'Assembly': 'Nodes'
        };
        this.bgcolor = '#151515';
    }
    PanelOutputNode.title = 'Panel Output';
    LiteGraph.registerNodeType('Main/Panel Output', PanelOutputNode);



    /*
     -------------------------------- HARDWARE -------------------------------------
    */






    //MySensorsNode
    function MySensorsNode() {
        this.properties = {
            'ObjectType': 'MyNetSensors.Nodes.MySensorsNode',
            'Assembly': 'Nodes.MySensors'
        };
        this.clonable = false;
    }
    MySensorsNode.title = 'MySensors Node';
    MySensorsNode.skip_list = true;
    LiteGraph.registerNodeType('Nodes/Hardware', MySensorsNode);













    /*
     -------------------------------- UI -------------------------------------
    */





    //UI Label
    function UILabel() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.UiLabelNode",
            'Assembly': "Nodes.UI"
        };
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
    UIState.title = "State";
    LiteGraph.registerNodeType("UI/State", UIState);



    //UI Progress
    function UIProgress() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.UiProgressNode",
            'Assembly': "Nodes.UI"
        };
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
    UILog.title = "Log";
    LiteGraph.registerNodeType("UI/Log", UILog);



    //UI Button
    function UIButton() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.UiButtonNode",
            'Assembly': "Nodes.UI"
        };
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
    UIToggleButton.title = "Toggle Button";
    LiteGraph.registerNodeType("UI/Toggle Button", UIToggleButton);



    //UI Switch
    function UISwitch() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.UiSwitchNode",
            'Assembly': "Nodes.UI"
        };
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
    UITextBox.title = "TextBox";
    LiteGraph.registerNodeType("UI/TextBox", UITextBox);






    //UI Slider
    function UISlider() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.UiSliderNode",
            'Assembly': "Nodes.UI"
        };
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
    UIRGBSliders.title = "RGB Sliders";
    LiteGraph.registerNodeType("UI/RGB Sliders", UIRGBSliders);



    //UI RGBW Sliders
    function UIRGBWSliders() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.UiRgbwSlidersNode",
            'Assembly': "Nodes.UI"
        };
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
        return [
        { content: "Open interface", callback: function () { var win = window.open("/UITimer/Tasks/" + that.id, '_blank'); win.focus(); } }
            , null
        ];
    }
    UITimer.title = "Timer";
    LiteGraph.registerNodeType("UI/Timer", UITimer);









    //UISpeech
    function UISpeech() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.UiSpeechNode",
            'Assembly': "Nodes.UI"
        };
    }
    UISpeech.title = "Speech";
    LiteGraph.registerNodeType("UI/Speech", UISpeech);






    //UIAudio
    function UIAudio() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.UiAudioNode",
            'Assembly': "Nodes.UI"
        };
    }
    UIAudio.title = "Audio";
    LiteGraph.registerNodeType("UI/Audio", UIAudio);




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
    Constant.title = "Constant";
    LiteGraph.registerNodeType("Basic/Constant", Constant);




    //LogicCounter
    function OperationCounter() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.OperationCounterNode",
            'Assembly': "Nodes"
        };
    }
    OperationCounter.title = "Counter";
    LiteGraph.registerNodeType("Operation/Counter", OperationCounter);










    //Connection Transmitter
    function ConnectionTransmitterNode() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.ConnectionTransmitterNode",
            'Assembly': "Nodes"
        };
    }
    ConnectionTransmitterNode.title = "Transmitter";
    LiteGraph.registerNodeType("Connection/Transmitter", ConnectionTransmitterNode);



    //Connection Receiver
    function ConnectionReceiverNode() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.ConnectionReceiverNode",
            'Assembly': "Nodes"
        };
    }
    ConnectionReceiverNode.title = "Receiver";
    LiteGraph.registerNodeType("Connection/Receiver", ConnectionReceiverNode);




    //ConnectionRemoteTransmitter
    function ConnectionRemoteTransmitter() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.ConnectionRemoteTransmitter",
            'Assembly': "Nodes"
        };
    }
    ConnectionRemoteTransmitter.title = "Remote Transmitter";
    LiteGraph.registerNodeType("Connection/Remote Transmitter", ConnectionRemoteTransmitter);




    //ConnectionRemoteReceiverNode
    function ConnectionRemoteReceiverNode() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.ConnectionRemoteReceiverNode",
            'Assembly': "Nodes"
        };
    }
    ConnectionRemoteReceiverNode.title = "Remote Receiver";
    LiteGraph.registerNodeType("Connection/Remote Receiver", ConnectionRemoteReceiverNode);




    //OperationFileNode
    function OperationFileNode() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.OperationFileNode",
            'Assembly': "Nodes"
        };
    }
    OperationFileNode.title = "File";
    LiteGraph.registerNodeType("Operation/File", OperationFileNode);



    //OperationJsonFileNode
    function OperationJsonFileNode() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.OperationJsonFileNode",
            'Assembly': "Nodes"
        };
    }
    OperationJsonFileNode.title = "Json File";
    LiteGraph.registerNodeType("Operation/Json File", OperationJsonFileNode);


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

    //MathRemapNode
    function MathRemap() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.MathRemapNode",
            'Assembly': "Nodes"
        };
    }
    MathRemap.title = "Remap";
    LiteGraph.registerNodeType("Math/Remap", MathRemap);

    //MathClampNode
    function MathClamp() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.MathClampNode",
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

    //OperationFlipFlop
    function OperationFlipFlop() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.OperationFlipflopNode",
            'Assembly': "Nodes"
        };
    }
    OperationFlipFlop.title = "Flip-Flop";
    LiteGraph.registerNodeType("Operation/Flip-Flop", OperationFlipFlop);

    //OperationGate
    function OperationGate() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.OperationGateNode",
            'Assembly': "Nodes"
        };
    }
    OperationGate.title = "Gate";
    LiteGraph.registerNodeType("Operation/Gate", OperationGate);

    //OperationRandom
    function OperationRandom() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.OperationRandomNode",
            'Assembly': "Nodes"
        };
    }
    OperationRandom.title = "Random";
    LiteGraph.registerNodeType("Operation/Random", OperationRandom);


    //OperationEventCounterNode
    function OperationEventCounterNode() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.OperationEventCounterNode",
            'Assembly': "Nodes"
        };
    }
    OperationEventCounterNode.title = "Event Counter";
    LiteGraph.registerNodeType("Operation/Event Counter", OperationEventCounterNode);


    //OperationEventsFreqMeterNode
    function OperationEventsFreqMeterNode() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.OperationEventsFreqMeterNode",
            'Assembly': "Nodes"
        };
    }
    OperationEventsFreqMeterNode.title = "Events Freq Meter";
    LiteGraph.registerNodeType("Operation/Events Freq Meter", OperationEventsFreqMeterNode);


    //OperationEventsDelayMeterNode
    function OperationEventsDelayMeterNode() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.OperationEventsDelayMeterNode",
            'Assembly': "Nodes"
        };
    }
    OperationEventsDelayMeterNode.title = "Events Delay Meter";
    LiteGraph.registerNodeType("Operation/Events Delay Meter", OperationEventsDelayMeterNode);



    //OperationMixerNode
    function OperationMixer() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.OperationMixerNode",
            'Assembly': "Nodes"
        };
    }
    OperationMixer.title = "Mixer";
    LiteGraph.registerNodeType("Operation/Mixer", OperationMixer);

    //OperationxFader
    function OperationxFader() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.OperationxFader",
            'Assembly': "Nodes"
        };
    }
    OperationxFader.title = "xFader";
    LiteGraph.registerNodeType("Operation/xFader", OperationxFader);

    //SystemBeep
    function SystemBeep() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.SystemBeepNode",
            'Assembly': "Nodes"
        };
    }
    SystemBeep.title = "Beep";
    LiteGraph.registerNodeType("System/Beep", SystemBeep);

    //SystemBeepAdvanced
    function SystemBeepAdvance() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.SystemBeepAdvancedNode",
            'Assembly': "Nodes"
        };
    }
    SystemBeepAdvance.title = "Beep Advanced";
    LiteGraph.registerNodeType("System/Beep Advanced", SystemBeepAdvance);

    //DelayTimer
    function DelayTimerNode() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.DelayTimerNode",
            'Assembly': "Nodes"
        };
    }
    DelayTimerNode.title = "Delay Timer";
    LiteGraph.registerNodeType("Delay/Delay Timer", DelayTimerNode);

    //OperationSwitchNode
    function OperationSwitchInNode() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.OperationSwitchInNode",
            'Assembly': "Nodes"
        };
    }
    OperationSwitchInNode.title = "Switch in";
    LiteGraph.registerNodeType("Operation/Switch in", OperationSwitchInNode);

    //OperationSwitchNode
    function OperationSwitchOutNode() {
        this.properties = {
            'ObjectType': "MyNetSensors.Nodes.OperationSwitchOutNode",
            'Assembly': "Nodes"
        };
    }
    OperationSwitchOutNode.title = "Switch out";
    LiteGraph.registerNodeType("Operation/Switch out", OperationSwitchOutNode);

})();