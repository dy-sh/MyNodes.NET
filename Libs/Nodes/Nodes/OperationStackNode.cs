/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;
using System.Linq;

namespace MyNetSensors.Nodes.Nodes
{
    public class OperationStackNode : Node
    {
       public Stack<string> Values { get; set; }

        public OperationStackNode() : base("Operation", "Stack")
        {
            AddInput("Add Value");
            AddInput("Get Value", DataType.Logical,true);
            AddOutput("Value");
            AddOutput("Count",DataType.Number);

            Values =new Stack<string>();
            Outputs[1].Value = Values.Count.ToString();

            options.LogOutputChanges = false;
        }

        public override void OnInputChange(Input input)
        {
            if (input == Inputs[0] && input.Value != null)
            {
                Values.Push(input.Value);
                Outputs[1].Value = Values.Count.ToString();
                UpdateMeInDb();
            }

            if (input == Inputs[1] && input.Value =="1")
            {
                LogInfo($"{ Outputs[0].Name}: [{ input.Value ?? "NULL"}]");
                Outputs[0].Value = Values.Any()? Values.Pop():null;
                Outputs[1].Value = Values.Count.ToString();
                UpdateMeInDb();
            }
        }

        public override string GetNodeDescription()
        {
            return "This node stores all the incoming values, and puts them in a stack. \n" +
                   "You can read the values from the stack at any time. \n" +
                   "Node can be used as a buffer. \n" +
                   "Values are stored in the database and available after restart of the server.";
        }
    }
}