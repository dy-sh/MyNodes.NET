/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;

namespace MyNodes.Nodes
{
    public class UiSwitchNode : UiNode
    {
        private string _value;

        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                Outputs[0].Value = _value;

                UpdateMeOnDashboard();
                UpdateMeInDb();
            }
        }

        public UiSwitchNode() : base("UI", "Switch")
        {
            AddInput();
            AddOutput();
            Value = "0";
            Outputs[0].Value = Value;
        }

        public override void OnInputChange(Input input)
        {
            Value = input.Value;
        }

        public override bool SetValues(Dictionary<string, string> values)
        {
            Value = Value == "0" ? "1" : "0";
            return true;
        }

        public override string GetNodeDescription()
        {
            return "This is a UI node. It displays a switch on the dashboard. ";
        }
    }
}
