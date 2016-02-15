/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
    public class LogicAndNode : Node
    {
        public LogicAndNode() : base("Logic", "AND", 2, 1)
        {
            Inputs[0].Type = DataType.Logical;
            Inputs[1].Type = DataType.Logical;
            Outputs[0].Type = DataType.Logical;

            options.ResetOutputsWhenAnyInputIsNull = true;
        }


        public override void OnInputChange(Input input)
        {
            var result = "0";

            if (Inputs[0].Value == "1" && Inputs[1].Value == "1")
                result = "1";

            Outputs[0].Value = result;
        }
    }
}