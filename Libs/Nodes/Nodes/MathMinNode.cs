/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace MyNodes.Nodes
{
    public class MathMinNode : Node
    {
        private double? min { get; set; }

        public MathMinNode() : base("Math", "Min")
        {
            AddInput("Value", DataType.Number,true);
            AddInput("Reset", DataType.Logical, true);
            AddOutput("Out",DataType.Number);
        }
        

        public override void OnInputChange(Input input)
        {
            if (input == Inputs[0] && input.Value != null)
            {
                double val = double.Parse(input.Value);

                if (min == null || val < min)
                {
                    min = val;
                    Outputs[0].Value = min.ToString();
                }
            }

            if (input == Inputs[1] && input.Value == "1")
            {
                min = null;
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