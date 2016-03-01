/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNodes.Nodes
{
    public class OperationEventCounterNode : Node
    {
        public int Count { get; set; }

        public OperationEventCounterNode() : base("Operation", "Event Counter")
        {
            AddInput("Value", DataType.Text);
            AddInput("Reset", DataType.Logical, true);
            AddOutput("Out", DataType.Number);
        }

        public override void OnInputChange(Input input)
        {
            if (input == Inputs[1] && Inputs[1].Value == "1")
            {
                Count = 0;
                LogInfo($"Reset");
            }
            else if (input == Inputs[0])
                Count++;

            Outputs[0].Value = Count.ToString();
            UpdateMeInDb();
        }

        public override string GetNodeDescription()
        {
            return "This node counts how many events occurred at the \"Value\" input. <br/>" +
                   "Any incoming value, including null, will be taken.";
        }
    }
}