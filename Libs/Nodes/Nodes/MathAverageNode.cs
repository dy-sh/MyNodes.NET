/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace MyNodes.Nodes
{
    public class MathAverageNode : Node
    {
        private List<double> Values { get; set; }

        public MathAverageNode() : base("Math", "Average")
        {
            AddInput("Value", DataType.Number, true);
            AddInput("Reset", DataType.Logical, true);
            AddOutput("Out", DataType.Number);

            Values = new List<double>();

            Settings.Add("maxvalues", new NodeSetting(NodeSettingType.Number, "Max count of the last values", "1000"));
        }


        public override void OnInputChange(Input input)
        {
            if (input == Inputs[0] && input.Value != null)
            {
                int maxValues=10;
                int.TryParse(Settings["maxvalues"].Value, out maxValues);

                Values.Add(double.Parse(input.Value));

                if (Values.Count > maxValues)
                {
                    int more = Values.Count - maxValues;
                    for (int i = 0; i < more; i++)
                        Values.RemoveAt(0);
                }

                Outputs[0].Value = (Values.Sum() / Values.Count).ToString("0.##");
            }

            if (input == Inputs[1] && input.Value == "1")
            {
                Values.Clear();
                Outputs[0].Value = null;
            }
        }

        public override string GetNodeDescription()
        {
            return "This node calculates an average value from all input values. <br/>" +
                    "To reset node, send logical \"1\" to input named \"Reset\"";
        }
    }
}