/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;

namespace MyNetSensors.Nodes
{
    public class UiButtonNode : UiNode
    {
        public UiButtonNode() : base("UI", "Button")
        {
            AddOutput();
            Outputs[0].Value = "0";
        }

    
        public override bool SetValues(Dictionary<string, string> values)
        {
            Outputs[0].Value = "1";
            UpdateMe();

            Outputs[0].Value = "0";
            UpdateMe();

            return true;
        }

        public override string GetNodeDescription()
        {
            return "This is a UI node. It displays a button on the dashboard.";
        }
    }
}
