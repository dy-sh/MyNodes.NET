//planer-pro copyright 2015 GPL - license.

namespace MyNetSensors.Nodes
{
    public class OperationEventCounterNode : Node
    {
        public OperationEventCounterNode() : base("Operation", "Event Counter", 2, 1)
        {
            Inputs[0].Name = "Value";
            Inputs[1].Name = "Reset";

            Inputs[0].Type = DataType.Text;
            Inputs[1].Type = DataType.Logical;
            Outputs[0].Type = DataType.Number;
        }

        public int Count { get; set; }

        public override void OnInputChange(Input input)
        {
            if (input == Inputs[1] && Inputs[1].Value == "1")
            {
                Count = 0;
                LogInfo($"Reset");
            }
            else if (input == Inputs[0])
                Count++;

            Outputs[0].Value = Count.ToString();
            UpdateMeInDb();
        }
    }
}