//planer-pro copyright 2015 GPL - license.

namespace MyNodes.Nodes
{
    public class LogicCompareNotEqualNode : Node
    {
        public LogicCompareNotEqualNode() : base("Logic", "Compare NotEqual")
        {
            AddInput();
            AddInput();
            AddOutput(DataType.Logical);

            options.ResetOutputsIfAnyInputIsNull = true;
        }


        public override void OnInputChange(Input input)
        {
            try
            {
                var a = double.Parse(Inputs[0].Value);
                var b = double.Parse(Inputs[1].Value);

                Outputs[0].Value = a != b ? "1" : "0";
            }
            catch
            {
                Outputs[0].Value = Inputs[0].Value != Inputs[1].Value ? "1" : "0";
            }
        }

        public override string GetNodeDescription()
        {
            return "This node works the opposite of how the Compare Equal node.";
        }
    }
}