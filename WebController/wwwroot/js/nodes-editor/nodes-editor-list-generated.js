(function () {


            //ConstantNode
            function ConstantNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.ConstantNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            ConstantNode.title = 'Constant';
            LiteGraph.registerNodeType('Basic/Constant', ConstantNode);

            

            //ConnectionReceiverNode
            function ConnectionReceiverNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.ConnectionReceiverNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            ConnectionReceiverNode.title = 'Receiver';
            LiteGraph.registerNodeType('Connection/Receiver', ConnectionReceiverNode);

            

            //ConnectionRemoteReceiverNode
            function ConnectionRemoteReceiverNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.ConnectionRemoteReceiverNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            ConnectionRemoteReceiverNode.title = 'Remote Receiver';
            LiteGraph.registerNodeType('Connection/Remote Receiver', ConnectionRemoteReceiverNode);

            

            //ConnectionRemoteTransmitter
            function ConnectionRemoteTransmitter () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.ConnectionRemoteTransmitter',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            ConnectionRemoteTransmitter.title = 'Remote Transmitter';
            LiteGraph.registerNodeType('Connection/Remote Transmitter', ConnectionRemoteTransmitter);

            

            //ConnectionTransmitterNode
            function ConnectionTransmitterNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.ConnectionTransmitterNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            ConnectionTransmitterNode.title = 'Transmitter';
            LiteGraph.registerNodeType('Connection/Transmitter', ConnectionTransmitterNode);

            

            //DelayTimerNode
            function DelayTimerNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.DelayTimerNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            DelayTimerNode.title = 'Delay Timer';
            LiteGraph.registerNodeType('Delay/Delay Timer', DelayTimerNode);

            

            //LogicAndNode
            function LogicAndNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.LogicAndNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            LogicAndNode.title = 'Logic AND';
            LiteGraph.registerNodeType('Logic/AND', LogicAndNode);

            

            //LogicNotNode
            function LogicNotNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.LogicNotNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            LogicNotNode.title = 'Logic NOT';
            LiteGraph.registerNodeType('Logic/NOT', LogicNotNode);

            

            //LogicOrNode
            function LogicOrNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.LogicOrNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            LogicOrNode.title = 'Logic OR';
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
            MathCosNode.title = 'Math Cos';
            LiteGraph.registerNodeType('Math/Cos', MathCosNode);

            

            //MathDivideNode
            function MathDivideNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.MathDivideNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            MathDivideNode.title = 'Math Divide';
            LiteGraph.registerNodeType('Math/Divide', MathDivideNode);

            

            //MathMinusNode
            function MathMinusNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.MathMinusNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            MathMinusNode.title = 'Math Minus';
            LiteGraph.registerNodeType('Math/Minus', MathMinusNode);

            

            //MathModulusNode
            function MathModulusNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.MathModulusNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            MathModulusNode.title = 'Math Modulus';
            LiteGraph.registerNodeType('Math/Modulus', MathModulusNode);

            

            //MathMultiplyNode
            function MathMultiplyNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.MathMultiplyNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            MathMultiplyNode.title = 'Math Multiply';
            LiteGraph.registerNodeType('Math/Multiply', MathMultiplyNode);

            

            //MathPlusNode
            function MathPlusNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.MathPlusNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            MathPlusNode.title = 'Math Plus';
            LiteGraph.registerNodeType('Math/Plus', MathPlusNode);

            

            //MathPowNode
            function MathPowNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.MathPowNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            MathPowNode.title = 'Math Pow';
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
            MathRoundNode.title = 'Math Round';
            LiteGraph.registerNodeType('Math/Round', MathRoundNode);

            

            //MathSinNode
            function MathSinNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.MathSinNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            MathSinNode.title = 'Math Sin';
            LiteGraph.registerNodeType('Math/Sin', MathSinNode);

            

            //MathSqrtNode
            function MathSqrtNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.MathSqrtNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            MathSqrtNode.title = 'Math Sqrt';
            LiteGraph.registerNodeType('Math/Sqrt', MathSqrtNode);

            

            //MathTanNode
            function MathTanNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.MathTanNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            MathTanNode.title = 'Math Tan';
            LiteGraph.registerNodeType('Math/Tan', MathTanNode);

            

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

            

            //OperationFileNode
            function OperationFileNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.OperationFileNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            OperationFileNode.title = 'File';
            LiteGraph.registerNodeType('Operation/File', OperationFileNode);

            

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

            

            //OperationGeneratorNode
            function OperationGeneratorNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.OperationGeneratorNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            OperationGeneratorNode.title = 'Generator';
            LiteGraph.registerNodeType('Operation/Generator', OperationGeneratorNode);

            

            //OperationJsonFileNode
            function OperationJsonFileNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.OperationJsonFileNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            OperationJsonFileNode.title = 'Json File';
            LiteGraph.registerNodeType('Operation/Json File', OperationJsonFileNode);

            

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

            

            //OperationxFader
            function OperationxFader () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.OperationxFader',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            OperationxFader.title = 'xFader';
            LiteGraph.registerNodeType('Operation/xFader', OperationxFader);

            

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

            
})();