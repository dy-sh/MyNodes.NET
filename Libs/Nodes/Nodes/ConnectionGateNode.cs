/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
    public class ConnectionGateNode : Node
    {
        public ConnectionGateNode() : base("Connection", "Gate")
        {
            AddInput("Value");
            AddInput("Key",DataType.Logical);
            AddOutput();

            options.ResetOutputsIfAnyInputIsNull = true;

            Settings.Add("sendnull", new NodeSetting(NodeSettingType.Checkbox, "Send null when closed", "false"));
        }


        public override void OnInputChange(Input input)
        {
            if (Inputs[1].Value== "1")
                Outputs[0].Value = Inputs[0].Value;
            else if (Settings["sendnull"].Value=="true")
                ResetOutputs();
        }
    }
}