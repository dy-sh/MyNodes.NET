/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNodes.Nodes
{
    public class LogicNotNode : Node
    {
        public LogicNotNode() : base("Logic", "NOT")
        {
            AddInput( DataType.Logical);
            AddOutput( DataType.Logical);

            options.ResetOutputsIfAnyInputIsNull = true;
        }


        public override void OnInputChange(Input input)
        {
            Outputs[0].Value = Inputs[0].Value == "0" ? "1" : "0";
        }

        public override string GetNodeDescription()
        {
            return "This node performs a logical \"NOT\" operation.<br/>" +
                   "It accepts only logical values (\"0\"/\"1\").";
        }
    }
}