/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;

namespace MyNodes.Nodes
{
    public class UiToggleButtonNode : UiNode
    {
        public string Value { get; set; }

        public UiToggleButtonNode() : base("UI", "Toggle")
        {
            AddOutput();
            Value = "0";
            Outputs[0].Value = Value;
        }


        public override bool SetValues(Dictionary<string, string> values)
        {
            Value = Value == "0" ? "1" : "0";
            Outputs[0].Value = Value;

            UpdateMe();
            UpdateMeInDb();

            return true;
        }

        public override string GetNodeDescription()
        {
            return "This is a UI node. It displays a toggle button on the dashboard. ";
        }
    }
}
