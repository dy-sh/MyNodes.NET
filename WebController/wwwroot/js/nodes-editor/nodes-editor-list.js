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

            

            //ConnectionGateNode
            function ConnectionGateNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.ConnectionGateNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            ConnectionGateNode.title = 'Gate';
            LiteGraph.registerNodeType('Connection/Gate', ConnectionGateNode);

            

            //ConnectionHubNode
            function ConnectionHubNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.ConnectionHubNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            ConnectionHubNode.title = 'Hub';
            LiteGraph.registerNodeType('Connection/Hub', ConnectionHubNode);

            

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

            

            //ConnectionRouterMultipleToOneNode
            function ConnectionRouterMultipleToOneNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.ConnectionRouterMultipleToOneNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            ConnectionRouterMultipleToOneNode.title = 'Router Multiple-One';
            LiteGraph.registerNodeType('Connection/Router Multiple-One', ConnectionRouterMultipleToOneNode);

            

            //ConnectionRouterOneToMultipleNode
            function ConnectionRouterOneToMultipleNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.ConnectionRouterOneToMultipleNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            ConnectionRouterOneToMultipleNode.title = 'Router One-Multiple';
            LiteGraph.registerNodeType('Connection/Router One-Multiple', ConnectionRouterOneToMultipleNode);

            

            //FiltersOnlyFromRangeNode
            function FiltersOnlyFromRangeNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.FiltersOnlyFromRangeNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            FiltersOnlyFromRangeNode.title = 'Only From Range';
            LiteGraph.registerNodeType('Filters/Only From Range', FiltersOnlyFromRangeNode);

            

            //FiltersOnlyGreaterNode
            function FiltersOnlyGreaterNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.Nodes.FiltersOnlyGreaterNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            FiltersOnlyGreaterNode.title = 'Only Greater';
            LiteGraph.registerNodeType('Filters/Only Greater', FiltersOnlyGreaterNode);

            

            //FiltersOnlyLogicNode
            function FiltersOnlyLogicNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.FiltersOnlyLogicNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            FiltersOnlyLogicNode.title = 'Only Logic';
            LiteGraph.registerNodeType('Filters/Only Logic', FiltersOnlyLogicNode);

            

            //FiltersOnlyLowerNode
            function FiltersOnlyLowerNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.Nodes.FiltersOnlyLowerNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            FiltersOnlyLowerNode.title = 'Only Lower';
            LiteGraph.registerNodeType('Filters/Only Lower', FiltersOnlyLowerNode);

            

            //FiltersOnlyNumbersNode
            function FiltersOnlyNumbersNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.FiltersOnlyNumbersNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            FiltersOnlyNumbersNode.title = 'Only Numbers';
            LiteGraph.registerNodeType('Filters/Only Numbers', FiltersOnlyNumbersNode);

            

            //FiltersOnlyOneNode
            function FiltersOnlyOneNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.FiltersOnlyOneNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            FiltersOnlyOneNode.title = 'Only One';
            LiteGraph.registerNodeType('Filters/Only One', FiltersOnlyOneNode);

            

            //FiltersOnlySpecifiedNode
            function FiltersOnlySpecifiedNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.FiltersOnlySpecifiedNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            FiltersOnlySpecifiedNode.title = 'Only Specified';
            LiteGraph.registerNodeType('Filters/Only Specified', FiltersOnlySpecifiedNode);

            

            //FiltersOnlyZeroNode
            function FiltersOnlyZeroNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.FiltersOnlyZeroNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            FiltersOnlyZeroNode.title = 'Only Zero';
            LiteGraph.registerNodeType('Filters/Only Zero', FiltersOnlyZeroNode);

            

            //FiltersPreventDuplicationNode
            function FiltersPreventDuplicationNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.FiltersPreventDuplicationNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            FiltersPreventDuplicationNode.title = 'Prevent Duplication';
            LiteGraph.registerNodeType('Filters/Prevent Duplication', FiltersPreventDuplicationNode);

            

            //FiltersPreventNullNode
            function FiltersPreventNullNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.FiltersPreventNullNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            FiltersPreventNullNode.title = 'Prevent Null';
            LiteGraph.registerNodeType('Filters/Prevent Null', FiltersPreventNullNode);

            

            //FiltersPreventSpecifiedNode
            function FiltersPreventSpecifiedNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.FiltersPreventSpecifiedNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            FiltersPreventSpecifiedNode.title = 'Prevent Specified';
            LiteGraph.registerNodeType('Filters/Prevent Specified', FiltersPreventSpecifiedNode);

            

            //FiltersReduceEventsNode
            function FiltersReduceEventsNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.FiltersReduceEventsNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            FiltersReduceEventsNode.title = 'Reduce Events';
            LiteGraph.registerNodeType('Filters/Reduce Events', FiltersReduceEventsNode);

            

            function MySensorsNode() {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.MySensorsNode',
                    'Assembly': 'Nodes.MySensors'
                };
                this.clonable = false;
            }
            MySensorsNode.title = 'MySensors Node';
            MySensorsNode.skip_list = true;
            LiteGraph.registerNodeType('Hardware/MySensors', MySensorsNode);

            

            //LogicAndNode
            function LogicAndNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.LogicAndNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            LogicAndNode.title = 'AND';
            LiteGraph.registerNodeType('Logic/AND', LogicAndNode);

            

            //CheckInRangeNode
            function CheckInRangeNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.CheckInRangeNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            CheckInRangeNode.title = 'Check In Range';
            LiteGraph.registerNodeType('Logic/Check In Range', CheckInRangeNode);

            

            //LogicCompareEqualNode
            function LogicCompareEqualNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.LogicCompareEqualNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            LogicCompareEqualNode.title = 'Compare Equal';
            LiteGraph.registerNodeType('Logic/Compare Equal', LogicCompareEqualNode);

            

            //LogicCompareGreaterNode
            function LogicCompareGreaterNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.LogicCompareGreaterNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            LogicCompareGreaterNode.title = 'Compare Greater';
            LiteGraph.registerNodeType('Logic/Compare Greater', LogicCompareGreaterNode);

            

            //LogicCompareLowerNode
            function LogicCompareLowerNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.LogicCompareLowerNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            LogicCompareLowerNode.title = 'Compare Lower';
            LiteGraph.registerNodeType('Logic/Compare Lower', LogicCompareLowerNode);

            

            //LogicCompareNotEqualNode
            function LogicCompareNotEqualNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.LogicCompareNotEqualNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            LogicCompareNotEqualNode.title = 'Compare NotEqual';
            LiteGraph.registerNodeType('Logic/Compare NotEqual', LogicCompareNotEqualNode);

            

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
            }
            PanelNode.title = 'Panel';
            PanelNode.prototype.getExtraMenuOptions = function (graphcanvas) {
                var that = this;
                return [
                    { content: 'Open', callback: function () { window.location = '/NodeEditor/Panel/' + that.id; } },
                    null, //null for horizontal line
                    { content: 'Show on Dashboard', callback: function () { var win = window.open('/Dashboard/Panel/' + that.id, '_blank'); win.focus(); } },
                    null,
                    { content: 'Export to file', callback: function () { var win = window.open('/NodeEditorAPI/SerializePanelToFile/' + that.id, '_blank'); win.focus(); } },
                    { content: 'Export to script', callback: function () { editor.exportPanelToScript(that.id) } },
                    { content: 'Export URL', callback: function () { editor.exportPanelURL(that.id) } },
                    null
                ];
            }
            LiteGraph.registerNodeType('Main/Panel', PanelNode);

            

            //PanelInputNode
            function PanelInputNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.PanelInputNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            PanelInputNode.title = 'Panel Input';
            LiteGraph.registerNodeType('Main/Panel Input', PanelInputNode);

            

            //PanelOutputNode
            function PanelOutputNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.PanelOutputNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            PanelOutputNode.title = 'Panel Output';
            LiteGraph.registerNodeType('Main/Panel Output', PanelOutputNode);

            

            //MathAverageNode
            function MathAverageNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.MathAverageNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            MathAverageNode.title = 'Average';
            LiteGraph.registerNodeType('Math/Average', MathAverageNode);

            

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

            

            //MathMaxNode
            function MathMaxNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.MathMaxNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            MathMaxNode.title = 'Max';
            LiteGraph.registerNodeType('Math/Max', MathMaxNode);

            

            //MathMinNode
            function MathMinNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.MathMinNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            MathMinNode.title = 'Min';
            LiteGraph.registerNodeType('Math/Min', MathMinNode);

            

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

            

            //MathSumNode
            function MathSumNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.Nodes.MathSumNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            MathSumNode.title = 'Sum';
            LiteGraph.registerNodeType('Math/Sum', MathSumNode);

            

            //MathTanNode
            function MathTanNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.MathTanNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            MathTanNode.title = 'Tan';
            LiteGraph.registerNodeType('Math/Tan', MathTanNode);

            

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

            

            //OperationFlipflopNode
            function OperationFlipflopNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.OperationFlipflopNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            OperationFlipflopNode.title = 'Flip-Flop';
            LiteGraph.registerNodeType('Operation/Flip-Flop', OperationFlipflopNode);

            

            //OperationFreqDividerNode
            function OperationFreqDividerNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.OperationFreqDividerNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            OperationFreqDividerNode.title = 'Freq Divider';
            LiteGraph.registerNodeType('Operation/Freq Divider', OperationFreqDividerNode);

            

            //OperationLinearShaperNode
            function OperationLinearShaperNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.OperationLinearShaperNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            OperationLinearShaperNode.title = 'Linear Shaper';
            LiteGraph.registerNodeType('Operation/Linear Shaper', OperationLinearShaperNode);

            

            //OperationQueueNode
            function OperationQueueNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.Nodes.OperationQueueNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            OperationQueueNode.title = 'Queue';
            LiteGraph.registerNodeType('Operation/Queue', OperationQueueNode);

            

            //OperationRandomNode
            function OperationRandomNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.OperationRandomNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            OperationRandomNode.title = 'Random';
            LiteGraph.registerNodeType('Operation/Random', OperationRandomNode);

            

            //OperationStackNode
            function OperationStackNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.Nodes.OperationStackNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            OperationStackNode.title = 'Stack';
            LiteGraph.registerNodeType('Operation/Stack', OperationStackNode);

            

            //RgbNumbersToRgbNode
            function RgbNumbersToRgbNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.Nodes.RgbNumbersToRgbNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            RgbNumbersToRgbNode.title = 'Numbers to RGB';
            LiteGraph.registerNodeType('RGB/Numbers to RGB', RgbNumbersToRgbNode);

            

            //RgbNumbersToRgbwNode
            function RgbNumbersToRgbwNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.Nodes.RgbNumbersToRgbwNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            RgbNumbersToRgbwNode.title = 'Numbers to RGBW';
            LiteGraph.registerNodeType('RGB/Numbers to RGBW', RgbNumbersToRgbwNode);

            

            //RgbRgbRemapNode
            function RgbRgbRemapNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.Nodes.RgbRgbRemapNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            RgbRgbRemapNode.title = 'RGB Remap';
            LiteGraph.registerNodeType('RGB/RGB Remap', RgbRgbRemapNode);

            

            //RgbRgbToNumbersNode
            function RgbRgbToNumbersNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.Nodes.RgbRgbToNumbersNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            RgbRgbToNumbersNode.title = 'RGB to Numbers';
            LiteGraph.registerNodeType('RGB/RGB to Numbers', RgbRgbToNumbersNode);

            

            //RgbRgbwRemapNode
            function RgbRgbwRemapNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.Nodes.RgbRgbwRemapNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            RgbRgbwRemapNode.title = 'RGBW Remap';
            LiteGraph.registerNodeType('RGB/RGBW Remap', RgbRgbwRemapNode);

            

            //RgbRgbwToNumbersNode
            function RgbRgbwToNumbersNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.Nodes.RgbRgbwToNumbersNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            RgbRgbwToNumbersNode.title = 'RGBW to Numbers';
            LiteGraph.registerNodeType('RGB/RGBW to Numbers', RgbRgbwToNumbersNode);

            

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

            

            //TextASCIICharNode
            function TextASCIICharNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.TextASCIICharNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            TextASCIICharNode.title = 'ASCII Char';
            LiteGraph.registerNodeType('Text/ASCII Char', TextASCIICharNode);

            

            //TextASCIICodeNode
            function TextASCIICodeNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.TextASCIICodeNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            TextASCIICodeNode.title = 'ASCII Code';
            LiteGraph.registerNodeType('Text/ASCII Code', TextASCIICodeNode);

            

            //TextConcatenationNode
            function TextConcatenationNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.TextConcatenationNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            TextConcatenationNode.title = 'Concatenation';
            LiteGraph.registerNodeType('Text/Concatenation', TextConcatenationNode);

            

            //TextSplitStringsNode
            function TextSplitStringsNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.TextSplitStringsNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            TextSplitStringsNode.title = 'Split Strings';
            LiteGraph.registerNodeType('Text/Split Strings', TextSplitStringsNode);

            

            //TimeClockNode
            function TimeClockNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.TimeClockNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            TimeClockNode.title = 'Clock';
            LiteGraph.registerNodeType('Time/Clock', TimeClockNode);

            

            //TimeDelayNode
            function TimeDelayNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.TimeDelayNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            TimeDelayNode.title = 'Delay';
            LiteGraph.registerNodeType('Time/Delay', TimeDelayNode);

            

            //TimeDelayMeterNode
            function TimeDelayMeterNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.TimeDelayMeterNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            TimeDelayMeterNode.title = 'Delay Meter';
            LiteGraph.registerNodeType('Time/Delay Meter', TimeDelayMeterNode);

            

            //TimeFadeNode
            function TimeFadeNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.TimeFadeNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            TimeFadeNode.title = 'Fade';
            LiteGraph.registerNodeType('Time/Fade', TimeFadeNode);

            

            //TimeFrequencyMeterNode
            function TimeFrequencyMeterNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.TimeFrequencyMeterNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            TimeFrequencyMeterNode.title = 'Frequency Meter';
            LiteGraph.registerNodeType('Time/Frequency Meter', TimeFrequencyMeterNode);

            

            //TimeIntervalTimerNode
            function TimeIntervalTimerNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.TimeIntervalTimerNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            TimeIntervalTimerNode.title = 'Interval Timer';
            LiteGraph.registerNodeType('Time/Interval Timer', TimeIntervalTimerNode);

            

            //TimeIteratorNode
            function TimeIteratorNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.TimeIteratorNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            TimeIteratorNode.title = 'Iterator';
            LiteGraph.registerNodeType('Time/Iterator', TimeIteratorNode);

            

            //TimeSmoothByRangeNode
            function TimeSmoothByRangeNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.TimeSmoothByRangeNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            TimeSmoothByRangeNode.title = 'Smooth By Range';
            LiteGraph.registerNodeType('Time/Smooth By Range', TimeSmoothByRangeNode);

            

            //TimeSmoothByTimeNode
            function TimeSmoothByTimeNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.TimeSmoothByTimeNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            TimeSmoothByTimeNode.title = 'Smooth By Time';
            LiteGraph.registerNodeType('Time/Smooth By Time', TimeSmoothByTimeNode);

            

            //TimeTickerNode
            function TimeTickerNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.TimeTickerNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            TimeTickerNode.title = 'Ticker';
            LiteGraph.registerNodeType('Time/Ticker', TimeTickerNode);

            

            //UiAudioNode
            function UiAudioNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.UiAudioNode',
                    'Assembly': 'Nodes.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            UiAudioNode.title = 'Audio';
            LiteGraph.registerNodeType('UI/Audio', UiAudioNode);

            

            //UiButtonNode
            function UiButtonNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.UiButtonNode',
                    'Assembly': 'Nodes.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            UiButtonNode.title = 'Button';
            LiteGraph.registerNodeType('UI/Button', UiButtonNode);

            

            //UiChartNode
            function UiChartNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.UiChartNode',
                    'Assembly': 'Nodes.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            UiChartNode.title = 'Chart';
            LiteGraph.registerNodeType('UI/Chart', UiChartNode);

            

            //UiLabelNode
            function UiLabelNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.UiLabelNode',
                    'Assembly': 'Nodes.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            UiLabelNode.title = 'Label';
            LiteGraph.registerNodeType('UI/Label', UiLabelNode);

            

            //UiLogNode
            function UiLogNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.UiLogNode',
                    'Assembly': 'Nodes.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            UiLogNode.title = 'Log';
            LiteGraph.registerNodeType('UI/Log', UiLogNode);

            

            //UiProgressNode
            function UiProgressNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.UiProgressNode',
                    'Assembly': 'Nodes.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            UiProgressNode.title = 'Progress';
            LiteGraph.registerNodeType('UI/Progress', UiProgressNode);

            

            //UiRgbSlidersNode
            function UiRgbSlidersNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.UiRgbSlidersNode',
                    'Assembly': 'Nodes.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            UiRgbSlidersNode.title = 'RGB Sliders';
            LiteGraph.registerNodeType('UI/RGB Sliders', UiRgbSlidersNode);

            

            //UiRgbwSlidersNode
            function UiRgbwSlidersNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.UiRgbwSlidersNode',
                    'Assembly': 'Nodes.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            UiRgbwSlidersNode.title = 'RGBW Sliders';
            LiteGraph.registerNodeType('UI/RGBW Sliders', UiRgbwSlidersNode);

            

            //UiSliderNode
            function UiSliderNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.UiSliderNode',
                    'Assembly': 'Nodes.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            UiSliderNode.title = 'Slider';
            LiteGraph.registerNodeType('UI/Slider', UiSliderNode);

            

            //UiStateNode
            function UiStateNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.UiStateNode',
                    'Assembly': 'Nodes.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            UiStateNode.title = 'State';
            LiteGraph.registerNodeType('UI/State', UiStateNode);

            

            //UiSwitchNode
            function UiSwitchNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.UiSwitchNode',
                    'Assembly': 'Nodes.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            UiSwitchNode.title = 'Switch';
            LiteGraph.registerNodeType('UI/Switch', UiSwitchNode);

            

            //UiTextBoxNode
            function UiTextBoxNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.UiTextBoxNode',
                    'Assembly': 'Nodes.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            UiTextBoxNode.title = 'TextBox';
            LiteGraph.registerNodeType('UI/TextBox', UiTextBoxNode);

            

            //UiTimerNode
            function UiTimerNode() {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.UiTimerNode',
                    'Assembly': 'Nodes.UI'
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
            UiTimerNode.title = 'Timer';
            LiteGraph.registerNodeType('UI/Timer', UiTimerNode);

            

            //UiToggleButtonNode
            function UiToggleButtonNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.UiToggleButtonNode',
                    'Assembly': 'Nodes.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            UiToggleButtonNode.title = 'Toggle';
            LiteGraph.registerNodeType('UI/Toggle', UiToggleButtonNode);

            

            //UiVoiceGoogleNode
            function UiVoiceGoogleNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.UiVoiceGoogleNode',
                    'Assembly': 'Nodes.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            UiVoiceGoogleNode.title = 'Voice Google';
            LiteGraph.registerNodeType('UI/Voice Google', UiVoiceGoogleNode);

            

            //UiVoiceYandexNode
            function UiVoiceYandexNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.UiVoiceYandexNode',
                    'Assembly': 'Nodes.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            UiVoiceYandexNode.title = 'Voice Yandex';
            LiteGraph.registerNodeType('UI/Voice Yandex', UiVoiceYandexNode);

            

            //UtilityBeepNode
            function UtilityBeepNode () {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.UtilityBeepNode',
                    'Assembly': 'Nodes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
                };
            }
            UtilityBeepNode.title = 'Beep';
            LiteGraph.registerNodeType('Utility/Beep', UtilityBeepNode);

            
})();