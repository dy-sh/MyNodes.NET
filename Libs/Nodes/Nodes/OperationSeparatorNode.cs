/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes.Nodes
{
    public class OperationSeparatorNode : Node
    {
        public OperationSeparatorNode() : base("Operation", "Separator")
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

        public override string GetNodeDescription()
        {
            return "All incoming values are compared with Treshold " +
                   "and are divided into two outputs. " +
                   "Values that are greater than or equal to the Treshold " +
                   "are sent to the output \"Hi\"." +
                   "Those that less, goes to \"Lo\". ";
        }
    }
}