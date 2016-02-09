namespace MyNetSensors.Nodes
{
    public class UiTimerNode:UiNode
    {

        public UiTimerNode():base(0,1)
        {
            this.Title = "UI Timer";
            this.Type = "UI/Timer";
            this.SetDefaultName("Timer");
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
        }

        public void SetState(string state)
        {
            LogInfo($"[{state}]");
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
            UiTimerNode.title = 'UI Timer';
            LiteGraph.registerNodeType('UI/Timer', UiTimerNode);

            ";
        }
    }
}
