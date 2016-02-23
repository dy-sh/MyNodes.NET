/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
    public class UiTimerNode:UiNode
    {

        public UiTimerNode():base("UI", "Timer")
        {
            AddOutput();
        }

        public void SetState(string state)
        {
            Outputs[0].Value = state;
        }

        public override string GetJsListGenerationScript()
        {
            return @"

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
            UiTimerNode.title = 'Timer';
            LiteGraph.registerNodeType('UI/Timer', UiTimerNode);

            ";
        }
    }
}
