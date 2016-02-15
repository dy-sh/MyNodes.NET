namespace MyNetSensors.Nodes.Nodes
{
    public class OperationAccumulatorNode : Node
    {
        public OperationAccumulatorNode() : base("Operation", "Accumulator", 2, 1)
        {
            Inputs[0].Type = DataType.Number;
            Inputs[1].Type = DataType.Number;
            Outputs[0].Type = DataType.Number;

            Inputs[0].Name = "Set Value";
            Inputs[1].Name = "Add Value";
            Outputs[0].Value = Value.ToString();
        }

        public double Value { get; set; }

        public override void OnInputChange(Input input)
        {
            var oldValue = Value;

            if (input == Inputs[0] && input.Value != null)
                Value = double.Parse(input.Value);

            if (input == Inputs[1] && input.Value != null)
                Value += double.Parse(input.Value);

            if (oldValue != Value)
            {
                Outputs[0].Value = Value.ToString();
                UpdateMeInDb();
            }
        }
    }
}