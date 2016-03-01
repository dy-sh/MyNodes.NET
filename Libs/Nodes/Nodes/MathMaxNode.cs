/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace MyNodes.Nodes
{
    public class MathMaxNode : Node
    {
        private List<double> Values { get; set; }

        public MathMaxNode() : base("Math", "Max")
        {
            AddInput("Value", DataType.Number,true);
            AddInput("Reset", DataType.Logical, true);
            AddOutput("Out",DataType.Number);

            Values=new List<double>();
        }
        

        public override void OnInputChange(Input input)
        {
            if (input == Inputs[0] && input.Value != null)
            {
                Values.Add(double.Parse(input.Value));
                Outputs[0].Value = Values.Max().ToString("0.##");
            }

            if (input == Inputs[1] && input.Value == "1")
            {
                Values.Clear();
                Outputs[0].Value = null;
            }
        }

        public override string GetNodeDescription()
        {
            return "This node finds the minimum value of all input values.<br/>" +
                    "To reset node, send logical \"1\" to input named \"Reset\"";
        }
    }
}