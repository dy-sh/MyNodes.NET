namespace MyNetSensors.Nodes.Nodes
{
    public class ConnectionSeparatorNode : Node
    {
        public ConnectionSeparatorNode() : base("Connection", "Separator")
        {
            AddInput("Treshold", DataType.Number);
            AddInput("Value", DataType.Number);

            AddOutput("Hi", DataType.Number);
            AddOutput("Lo", DataType.Number);

            options.ResetOutputsIfAnyInputIsNull = true;
        }


        public override void OnInputChange(Input input)
        {
            if (input == Inputs[1])
            {
                var threshold = double.Parse(Inputs[0].Value);
                var val = double.Parse(Inputs[1].Value);

                if (val >= threshold)
                    Outputs[0].Value = val.ToString();
                else
                    Outputs[1].Value = val.ToString();
            }
        }
    }
}