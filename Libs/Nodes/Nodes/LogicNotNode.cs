/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
    public class LogicNotNode : Node
    {
        public LogicNotNode() : base("Logic", "NOT", 1, 1)
        {
            Inputs[0].Type = DataType.Logical;
            Outputs[0].Type = DataType.Logical;

            options.ResetOutputsWhenAnyInputIsNull = true;
        }


        public override void OnInputChange(Input input)
        {
            Outputs[0].Value = Inputs[0].Value == "0" ? "1" : "0";
        }
    }
}