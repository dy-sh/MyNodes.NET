using MyNetSensors.LogicalNodes;
using MyNetSensors.LogicalNodesUI;

namespace MyNetSensors.Nodes
{
    public class UITimer:LogicalNodeUI
    {

        public UITimer():base(0,1)
        {
            this.Title = "UI Timer";
            this.Type = "UI/Timer";
            this.Name = "Timer";
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
        }

        public void SetState(string state)
        {
            LogInfo($"UI Timer [{Name}]: {state}");
            Outputs[0].Value = state;
        }
    }
}
