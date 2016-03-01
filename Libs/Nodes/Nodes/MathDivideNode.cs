//planer-pro copyright 2015 GPL - license.

namespace MyNodes.Nodes
{
    public class MathDivideNode : Node
    {
        public MathDivideNode() : base("Math", "Divide")
        {
            AddInput(DataType.Number);
            AddInput(DataType.Number);
            AddOutput(DataType.Number);

            options.ResetOutputsIfAnyInputIsNull = true;
        }


        public override void OnInputChange(Input input)
        {
            try
            {
                var a = double.Parse(Inputs[0].Value);
                var b = double.Parse(Inputs[1].Value);
                var c = a/b;

                Outputs[0].Value = c.ToString();
            }
            catch
            {
                Outputs[0].Value = null;
            }
        }

        public override string GetNodeDescription()
        {
            return "This node divides one number by another.";
        }
    }
}