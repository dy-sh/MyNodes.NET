/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
    public class OperationGateNode : Node
    {
        public OperationGateNode() : base("Operation", "Gate", 2, 1)
        {
            Inputs[0].Name = "Value";
            Inputs[1].Name = "Key";

            Inputs[0].Type = DataType.Text;
            Inputs[1].Type = DataType.Logical;
            Outputs[0].Type = DataType.Text;

            options.ResetOutputsWhenAnyInputIsNull = true;
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            Outputs[0].Value = Inputs[1].Value == "1" ? Inputs[0].Value : null;
        }
    }
}