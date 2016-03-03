/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;

namespace MyNodes.Nodes
{
    public class UiButtonNode : UiNode
    {
        public UiButtonNode() : base("UI", "Button")
        {
            AddOutput();
            Outputs[0].Value = "0";

            Settings.Add("zero", new NodeSetting(NodeSettingType.Checkbox, "Generate Zero", "true"));
        }


        public override bool SetValues(Dictionary<string, string> values)
        {
            Outputs[0].Value = "1";

            if (Settings["zero"].Value == "true")
            {
                Outputs[0].Value = "0";
            }

            return true;
        }

        public override string GetNodeDescription()
        {
            return "This is a UI node. It displays a button on the dashboard. <br/>" +
                   "If \"Generate Zero\" option is enabled in the settings of the node, " +
                   "node will send a \"0\" right after \"1\". " +
                   "If the option is disabled, \"0\" will not be sent.";
        }
    }
}
