//planer-pro copyright 2015 GPL - license.

namespace MyNetSensors.Nodes
{
    public class LogicCompareLowerNode : Node
    {
        public LogicCompareLowerNode() : base("Logic", "Compare Lower")
        {
            AddInput(DataType.Number);
            AddInput(DataType.Number);
            AddOutput(DataType.Logical);

            options.ResetOutputsIfAnyInputIsNull = true;
        }

        public override void OnInputChange(Input input)
        {
            var a = double.Parse(Inputs[0].Value);
            var b = double.Parse(Inputs[1].Value);

            Outputs[0].Value = a < b ? "1" : "0";
        }
    }
}