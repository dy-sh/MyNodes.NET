/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
    public class CheckInRangeNode : Node
    {
        public CheckInRangeNode() : base("Logic", "Check In Range")
        {
            AddInput("Value", DataType.Number);
            AddInput("From", DataType.Number);
            AddInput("To", DataType.Number);
            AddOutput(DataType.Logical);

            options.ResetOutputsIfAnyInputIsNull = true;
        }

        public override void OnInputChange(Input input)
        {
            var val = double.Parse(Inputs[0].Value);
            var from = double.Parse(Inputs[1].Value);
            var to = double.Parse(Inputs[2].Value);

            Outputs[0].Value = (val >= from && val <= to) ? "1" : "0";
        }
    }
}