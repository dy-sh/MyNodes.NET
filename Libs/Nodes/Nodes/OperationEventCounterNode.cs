/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
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
    }
}