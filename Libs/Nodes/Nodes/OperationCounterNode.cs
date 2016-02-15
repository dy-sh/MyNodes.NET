/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
    public class OperationCounterNode : Node
    {
        public double Value { get; set; }

        public OperationCounterNode() : base("Operation", "Counter")
        {
            AddInput("Set Value", DataType.Number,true);
            AddInput("Count Up", DataType.Logical, true);
            AddInput("Count Down", DataType.Logical, true);
            AddOutput("Out",DataType.Number);

            Outputs[0].Value = Value.ToString();
        }
        

        public override void OnInputChange(Input input)
        {
            var oldValue = Value;

            if (input == Inputs[0] && input.Value != null)
                Value = double.Parse(input.Value);

            if (input == Inputs[1] && input.Value == "1")
                Value++;

            if (input == Inputs[2] && input.Value == "1")
                Value--;

            if (oldValue != Value)
            {
                Outputs[0].Value = Value.ToString();
                UpdateMeInDb();
            }
        }
    }
}