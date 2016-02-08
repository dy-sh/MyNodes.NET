/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;

namespace MyNetSensors.Nodes
{
    public class BasicConstantNode : Node
    {


        public BasicConstantNode() : base(0,1)
        {
            this.Title = "Constant";
            this.Type = "Basic/Constant";

            Settings.Add("Value",new NodeSetting(NodeSettingType.Text, "Constant Value",""));
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
        }

        public void SetValue(string value)
        {
            Settings["Value"].Value = value;

            LogInfo($"[{value ?? "NULL"}]");

            Outputs[0].Value = value;

            UpdateMe();
            UpdateMeInDb();
        }

        public override bool SetSettings(Dictionary<string, string> data)
        {
            SetValue(data["Value"]);
            return true;
        }
    }
}
