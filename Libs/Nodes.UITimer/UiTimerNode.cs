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
            LogInfo($"UI Timer [{Settings["Name"].Value}]: {state}");
            Outputs[0].Value = state;
        }
    }
}
