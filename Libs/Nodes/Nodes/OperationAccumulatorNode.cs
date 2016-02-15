namespace MyNetSensors.Nodes.Nodes
{
    public class OperationAccumulatorNode : Node
    {
        public double Value { get; set; }

        public OperationAccumulatorNode() : base("Operation", "Accumulator")
        {
            AddInput("Add Value", DataType.Number);
            AddInput("Set Value", DataType.Number,true);
            AddOutput("Out", DataType.Number);

            Outputs[0].Value = Value.ToString();
        }

        public override void OnInputChange(Input input)
        {
            var oldValue = Value;

            if (input == Inputs[0] && input.Value != null)
                Value += double.Parse(input.Value);

            if (input == Inputs[1] && input.Value != null)
                Value = double.Parse(input.Value);

            if (oldValue != Value)
            {
                Outputs[0].Value = Value.ToString();
                UpdateMeInDb();
            }
        }
    }
}