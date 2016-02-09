(function () {


            //BasicConstantNode
            function BasicConstantNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.BasicConstantNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            BasicConstantNode.title = 'Constant';
            LiteGraph.registerNodeType('Basic/Constant', BasicConstantNode);

            

            //ConnectionLocalReceiverNode
            function ConnectionLocalReceiverNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.ConnectionLocalReceiverNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            ConnectionLocalReceiverNode.title = 'Local Receiver';
            LiteGraph.registerNodeType('Connection/Local Receiver', ConnectionLocalReceiverNode);

            

            //ConnectionLocalTransmitterNode
            function ConnectionLocalTransmitterNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.ConnectionLocalTransmitterNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            ConnectionLocalTransmitterNode.title = 'Local Transmitter';
            LiteGraph.registerNodeType('Connection/Local Transmitter', ConnectionLocalTransmitterNode);

            

            //ConnectionRemoteReceiverNode
            function ConnectionRemoteReceiverNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.ConnectionRemoteReceiverNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            ConnectionRemoteReceiverNode.title = 'Remote Receiver';
            LiteGraph.registerNodeType('Connection/Remote Receiver', ConnectionRemoteReceiverNode);

            

            //ConnectionRemoteTransmitterNode
            function ConnectionRemoteTransmitterNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.ConnectionRemoteTransmitterNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            ConnectionRemoteTransmitterNode.title = 'Remote Transmitter';
            LiteGraph.registerNodeType('Connection/Remote Transmitter', ConnectionRemoteTransmitterNode);

            

            //LogicAndNode
            function LogicAndNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.LogicAndNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            LogicAndNode.title = 'AND';
            LiteGraph.registerNodeType('Logic/AND', LogicAndNode);

            

            //LogicNotNode
            function LogicNotNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.LogicNotNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            LogicNotNode.title = 'NOT';
            LiteGraph.registerNodeType('Logic/NOT', LogicNotNode);

            

            //LogicOrNode
            function LogicOrNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.LogicOrNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            LogicOrNode.title = 'OR';
            LiteGraph.registerNodeType('Logic/OR', LogicOrNode);

            

            //PanelNode
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

            

            //MathClampNode
            function MathClampNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.MathClampNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            MathClampNode.title = 'Clamp';
            LiteGraph.registerNodeType('Math/Clamp', MathClampNode);

            

            //MathCosNode
            function MathCosNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.MathCosNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            MathCosNode.title = 'Cos';
            LiteGraph.registerNodeType('Math/Cos', MathCosNode);

            

            //MathDivideNode
            function MathDivideNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.MathDivideNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            MathDivideNode.title = 'Divide';
            LiteGraph.registerNodeType('Math/Divide', MathDivideNode);

            

            //MathMinusNode
            function MathMinusNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.MathMinusNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            MathMinusNode.title = 'Minus';
            LiteGraph.registerNodeType('Math/Minus', MathMinusNode);

            

            //MathModulusNode
            function MathModulusNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.MathModulusNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            MathModulusNode.title = 'Modulus';
            LiteGraph.registerNodeType('Math/Modulus', MathModulusNode);

            

            //MathMultiplyNode
            function MathMultiplyNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.MathMultiplyNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            MathMultiplyNode.title = 'Multiply';
            LiteGraph.registerNodeType('Math/Multiply', MathMultiplyNode);

            

            //MathPlusNode
            function MathPlusNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.MathPlusNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            MathPlusNode.title = 'Plus';
            LiteGraph.registerNodeType('Math/Plus', MathPlusNode);

            

            //MathPowNode
            function MathPowNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.MathPowNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            MathPowNode.title = 'Pow';
            LiteGraph.registerNodeType('Math/Pow', MathPowNode);

            

            //MathRemapNode
            function MathRemapNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.MathRemapNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            MathRemapNode.title = 'Remap';
            LiteGraph.registerNodeType('Math/Remap', MathRemapNode);

            

            //MathRoundNode
            function MathRoundNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.MathRoundNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            MathRoundNode.title = 'Round';
            LiteGraph.registerNodeType('Math/Round', MathRoundNode);

            

            //MathSinNode
            function MathSinNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.MathSinNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            MathSinNode.title = 'Sin';
            LiteGraph.registerNodeType('Math/Sin', MathSinNode);

            

            //MathSqrtNode
            function MathSqrtNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.MathSqrtNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            MathSqrtNode.title = 'Sqrt';
            LiteGraph.registerNodeType('Math/Sqrt', MathSqrtNode);

            

            //MathTanNode
            function MathTanNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.MathTanNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            MathTanNode.title = 'Tan';
            LiteGraph.registerNodeType('Math/Tan', MathTanNode);

            

            //OperationAccumulatorNode
            function OperationAccumulatorNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.Nodes.OperationAccumulatorNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            OperationAccumulatorNode.title = 'Accumulator';
            LiteGraph.registerNodeType('Operation/Accumulator', OperationAccumulatorNode);

            

            //OperationCompareEqualNode
            function OperationCompareEqualNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.OperationCompareEqualNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            OperationCompareEqualNode.title = 'Compare Equal';
            LiteGraph.registerNodeType('Operation/Compare Equal', OperationCompareEqualNode);

            

            //OperationCompareGreaterNode
            function OperationCompareGreaterNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.OperationCompareGreaterNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            OperationCompareGreaterNode.title = 'Compare Greater';
            LiteGraph.registerNodeType('Operation/Compare Greater', OperationCompareGreaterNode);

            

            //OperationCompareLowerNode
            function OperationCompareLowerNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.OperationCompareLowerNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            OperationCompareLowerNode.title = 'Compare Lower';
            LiteGraph.registerNodeType('Operation/Compare Lower', OperationCompareLowerNode);

            

            //OperationCompareNotEqualNode
            function OperationCompareNotEqualNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.OperationCompareNotEqualNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            OperationCompareNotEqualNode.title = 'Compare NotEqual';
            LiteGraph.registerNodeType('Operation/Compare NotEqual', OperationCompareNotEqualNode);

            

            //OperationCounterNode
            function OperationCounterNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.OperationCounterNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            OperationCounterNode.title = 'Counter';
            LiteGraph.registerNodeType('Operation/Counter', OperationCounterNode);

            

            //OperationCrossfadeNode
            function OperationCrossfadeNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.OperationCrossfadeNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            OperationCrossfadeNode.title = 'Crossfade';
            LiteGraph.registerNodeType('Operation/Crossfade', OperationCrossfadeNode);

            

            //OperationEventCounterNode
            function OperationEventCounterNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.OperationEventCounterNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            OperationEventCounterNode.title = 'Event Counter';
            LiteGraph.registerNodeType('Operation/Event Counter', OperationEventCounterNode);

            

            //OperationEventsDelayMeterNode
            function OperationEventsDelayMeterNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.OperationEventsDelayMeterNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            OperationEventsDelayMeterNode.title = 'Events Delay Meter';
            LiteGraph.registerNodeType('Operation/Events Delay Meter', OperationEventsDelayMeterNode);

            

            //OperationEventsFreqMeterNode
            function OperationEventsFreqMeterNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.OperationEventsFreqMeterNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            OperationEventsFreqMeterNode.title = 'Events Freq Meter';
            LiteGraph.registerNodeType('Operation/Events Freq Meter', OperationEventsFreqMeterNode);

            

            //OperationFlipflopNode
            function OperationFlipflopNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.OperationFlipflopNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            OperationFlipflopNode.title = 'Flip-Flop';
            LiteGraph.registerNodeType('Operation/Flip-Flop', OperationFlipflopNode);

            

            //OperationGateNode
            function OperationGateNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.OperationGateNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            OperationGateNode.title = 'Gate';
            LiteGraph.registerNodeType('Operation/Gate', OperationGateNode);

            

            //OperationMixerNode
            function OperationMixerNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.OperationMixerNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            OperationMixerNode.title = 'Mixer';
            LiteGraph.registerNodeType('Operation/Mixer', OperationMixerNode);

            

            //OperationRandomNode
            function OperationRandomNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.OperationRandomNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            OperationRandomNode.title = 'Random';
            LiteGraph.registerNodeType('Operation/Random', OperationRandomNode);

            

            //OperationSeparatorNode
            function OperationSeparatorNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.Nodes.OperationSeparatorNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            OperationSeparatorNode.title = 'Separator';
            LiteGraph.registerNodeType('Operation/Separator', OperationSeparatorNode);

            

            //OperationSwitchInNode
            function OperationSwitchInNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.OperationSwitchInNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            OperationSwitchInNode.title = 'Switch in';
            LiteGraph.registerNodeType('Operation/Switch in', OperationSwitchInNode);

            

            //OperationSwitchOutNode
            function OperationSwitchOutNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.OperationSwitchOutNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            OperationSwitchOutNode.title = 'Switch out';
            LiteGraph.registerNodeType('Operation/Switch out', OperationSwitchOutNode);

            

            //SystemBeepNode
            function SystemBeepNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.SystemBeepNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            SystemBeepNode.title = 'Beep';
            LiteGraph.registerNodeType('System/Beep', SystemBeepNode);

            

            //SystemBeepAdvancedNode
            function SystemBeepAdvancedNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.SystemBeepAdvancedNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            SystemBeepAdvancedNode.title = 'Beep Advanced';
            LiteGraph.registerNodeType('System/Beep Advanced', SystemBeepAdvancedNode);

            

            //SystemFileNode
            function SystemFileNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.SystemFileNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            SystemFileNode.title = 'File';
            LiteGraph.registerNodeType('System/File', SystemFileNode);

            

            //SystemJsonFileNode
            function SystemJsonFileNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.SystemJsonFileNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            SystemJsonFileNode.title = 'Json File';
            LiteGraph.registerNodeType('System/Json File', SystemJsonFileNode);

            

            //SystemRunNode
            function SystemRunNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.SystemRunNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            SystemRunNode.title = 'Run';
            LiteGraph.registerNodeType('System/Run', SystemRunNode);

            

            //TimeDelayTimerNode
            function TimeDelayTimerNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.TimeDelayTimerNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            TimeDelayTimerNode.title = 'Delay Timer';
            LiteGraph.registerNodeType('Time/Delay Timer', TimeDelayTimerNode);

            

            //TimeGeneratorNode
            function TimeGeneratorNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.TimeGeneratorNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            TimeGeneratorNode.title = 'Generator';
            LiteGraph.registerNodeType('Time/Generator', TimeGeneratorNode);

            

            //UiAudioNode
            function UiAudioNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.UiAudioNode',
                    'Assembly': 'Nodes.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            UiAudioNode.title = 'UI Audio';
            LiteGraph.registerNodeType('UI/Audio', UiAudioNode);

            

            //UiButtonNode
            function UiButtonNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.UiButtonNode',
                    'Assembly': 'Nodes.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            UiButtonNode.title = 'UI Button';
            LiteGraph.registerNodeType('UI/Button', UiButtonNode);

            

            //UiChartNode
            function UiChartNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.UiChartNode',
                    'Assembly': 'Nodes.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            UiChartNode.title = 'UI Chart';
            LiteGraph.registerNodeType('UI/Chart', UiChartNode);

            

            //UiLabelNode
            function UiLabelNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.UiLabelNode',
                    'Assembly': 'Nodes.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            UiLabelNode.title = 'UI Label';
            LiteGraph.registerNodeType('UI/Label', UiLabelNode);

            

            //UiLogNode
            function UiLogNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.UiLogNode',
                    'Assembly': 'Nodes.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            UiLogNode.title = 'UI Log';
            LiteGraph.registerNodeType('UI/Log', UiLogNode);

            

            //UiProgressNode
            function UiProgressNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.UiProgressNode',
                    'Assembly': 'Nodes.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            UiProgressNode.title = 'UI Progress';
            LiteGraph.registerNodeType('UI/Progress', UiProgressNode);

            

            //UiRgbSlidersNode
            function UiRgbSlidersNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.UiRgbSlidersNode',
                    'Assembly': 'Nodes.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            UiRgbSlidersNode.title = 'UI RGB Sliders';
            LiteGraph.registerNodeType('UI/RGB Sliders', UiRgbSlidersNode);

            

            //UiRgbwSlidersNode
            function UiRgbwSlidersNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.UiRgbwSlidersNode',
                    'Assembly': 'Nodes.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            UiRgbwSlidersNode.title = 'UI RGBW Sliders';
            LiteGraph.registerNodeType('UI/RGBW Sliders', UiRgbwSlidersNode);

            

            //UiSliderNode
            function UiSliderNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.UiSliderNode',
                    'Assembly': 'Nodes.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            UiSliderNode.title = 'UI Slider';
            LiteGraph.registerNodeType('UI/Slider', UiSliderNode);

            

            //UiStateNode
            function UiStateNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.UiStateNode',
                    'Assembly': 'Nodes.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            UiStateNode.title = 'UI State';
            LiteGraph.registerNodeType('UI/State', UiStateNode);

            

            //UiSwitchNode
            function UiSwitchNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.UiSwitchNode',
                    'Assembly': 'Nodes.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            UiSwitchNode.title = 'UI Switch';
            LiteGraph.registerNodeType('UI/Switch', UiSwitchNode);

            

            //UiTextBoxNode
            function UiTextBoxNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.UiTextBoxNode',
                    'Assembly': 'Nodes.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            UiTextBoxNode.title = 'UI TextBox';
            LiteGraph.registerNodeType('UI/TextBox', UiTextBoxNode);

            

            //UiTimerNode
            function UiTimerNode() {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.UiTimerNode',
                    'Assembly': 'Nodes.UITimer'
                };
            }
            UiTimerNode.prototype.getExtraMenuOptions = function(graphcanvas)
            {
                var that = this;
                return [
                { content: 'Open interface', callback: function() { var win = window.open('/UITimer/Tasks/' + that.id, '_blank'); win.focus(); } }
                    , null
                ];
            }
            UiTimerNode.title = 'UI Timer';
            LiteGraph.registerNodeType('UI/Timer', UiTimerNode);

            

            //UiToggleButtonNode
            function UiToggleButtonNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.UiToggleButtonNode',
                    'Assembly': 'Nodes.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            UiToggleButtonNode.title = 'UI Toggle';
            LiteGraph.registerNodeType('UI/Toggle Button', UiToggleButtonNode);

            

            //UiVoiceGoogleNode
            function UiVoiceGoogleNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.UiVoiceGoogleNode',
                    'Assembly': 'Nodes.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            UiVoiceGoogleNode.title = 'UI Voice Google';
            LiteGraph.registerNodeType('UI/Voice Google', UiVoiceGoogleNode);

            

            //UiVoiceYandexNode
            function UiVoiceYandexNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.UiVoiceYandexNode',
                    'Assembly': 'Nodes.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            UiVoiceYandexNode.title = 'UI Voice Yandex';
            LiteGraph.registerNodeType('UI/Voice Yandex', UiVoiceYandexNode);

            
})();