namespace MyNetSensors.Nodes.Nodes
{
    public class OperationSeparatorNode : Node
    {
        public OperationSeparatorNode() : base("Operation", "Separator", 2, 2)
        {
            Inputs[0].Type = DataType.Number;
            Inputs[1].Type = DataType.Number;
            Outputs[0].Type = DataType.Number;
            Outputs[0].Type = DataType.Number;

            Inputs[0].Name = "Treshold";
            Inputs[1].Name = "Value";
            Outputs[0].Name = "Hi";
            Outputs[1].Name = "Lo";

            options.ResetOutputsWhenAnyInputIsNull = true;
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