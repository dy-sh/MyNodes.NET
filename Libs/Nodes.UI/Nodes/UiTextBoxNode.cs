/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;
using System.Linq;

namespace MyNodes.Nodes
{
    public class UiTextBoxNode : UiNode
    {
        public string Value { get; set; }

        public UiTextBoxNode() : base("UI", "TextBox")
        {
            AddOutput();
        }


        public override bool SetValues(Dictionary<string, string> values)
        {
            Value = values.FirstOrDefault().Value;
            Outputs[0].Value = Value;

            UpdateMe();
            UpdateMeInDb();

            return true;
        }

        public override string GetNodeDescription()
        {
            return "This is a UI node. It displays an input text box on the dashboard. ";
        }
    }
}
