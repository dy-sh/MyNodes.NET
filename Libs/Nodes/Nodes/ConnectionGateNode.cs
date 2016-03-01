/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNodes.Nodes
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

        public override string GetNodeDescription()
        {
            return "This node can block the transfer of messages from one node to another. <br/>" +
                   "Send \"1\" to \"Key\" to allow the transfer or \"0\" to block the transfer. <br/>" +
                   "If you enable the option \"Send null when closed\" in the settings node, " +
                   "then the node will send null to the output when the transmission is locked.";
        }
    }
}