//planer-pro copyright 2015 GPL - license.

namespace MyNetSensors.Nodes
{
    public class OperationCompareNotEqualNode : Node
    {
        public OperationCompareNotEqualNode() : base("Operation", "Compare NotEqual", 2, 1)
        {
            Inputs[0].Type = DataType.Text;
            Inputs[1].Type = DataType.Text;
            Outputs[0].Type = DataType.Logical;

            options.ResetOutputsWhenAnyInputIsNull = true;
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
    }
}