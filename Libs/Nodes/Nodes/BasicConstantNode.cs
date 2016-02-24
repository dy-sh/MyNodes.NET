/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;

namespace MyNetSensors.Nodes
{
    public class BasicConstantNode : Node
    {
        public BasicConstantNode() : base("Basic", "Constant")
        {
            AddOutput();
            Settings.Add("Value", new NodeSetting(NodeSettingType.Text, "Constant Value", ""));
        }


        public void SetValue(string value)
        {
            Settings["Value"].Value = value;

            Outputs[0].Value = value;

            UpdateMe();
            UpdateMeInDb();
        }

        public override bool SetSettings(Dictionary<string, string> data)
        {
            SetValue(data["Value"]);
            return true;
        }

        public override string GetNodeDescription()
        {
            return "This node simply stores the specified value." +
                   "You can set the value in the settings.";
        }
    }
}