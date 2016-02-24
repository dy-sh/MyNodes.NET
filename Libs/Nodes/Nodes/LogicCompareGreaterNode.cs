//planer-pro copyright 2015 GPL - license.

namespace MyNetSensors.Nodes
{
    public class LogicCompareGreaterNode : Node
    {
        public LogicCompareGreaterNode() : base("Logic", "Compare Greater")
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

            Outputs[0].Value = a > b ? "1" : "0";
        }

        public override string GetNodeDescription()
        {
            return "This node compares two values and sends \"1\" to the output " +
                   "if the first value is greater than the second, or \"0\" if not. <br/>" +
                   "It can compare only numbers. ";
        }
    }
}