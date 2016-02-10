//planer-pro copyright 2015 GPL - license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.Nodes
{

    public class OperationEventCounterNode : Node
    {
        public int Count { get; set; }

        public OperationEventCounterNode() : base(2, 1)
        {
            this.Title = "Event Counter";
            this.Type = "Operation/Event Counter";

            Inputs[0].Name = "Value";
            Inputs[1].Name = "Reset";

            Inputs[0].Type = DataType.Text;
            Inputs[1].Type = DataType.Logical;
            Outputs[0].Type = DataType.Number;
        }

        public override void Loop()
        {
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