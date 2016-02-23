/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
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
            AddInput("Reset", DataType.Logical, true);
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

            if (input == Inputs[3] && input.Value == "1")
                Value=0;

            if (oldValue != Value)
            {
                Outputs[0].Value = Value.ToString();
                UpdateMeInDb();
            }
        }

        public override string GetNodeDescription()
        {
            return "This node increases by 1 an internal counter " +
                   "when a logical \"1\" comes  to the input \"Count Up\". \n" +
                   "The counter decreases by 1 " +
                   "when a logical \"1\" comes  to the input \"Count Down\". \n" +
                   "You can override internal value to the specified value (Set Value). \n" +
                   "Logical \"1\" on Reset input will set internal value to 0.";
        }
    }
}