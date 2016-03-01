/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNodes.Nodes
{
    public class OperationAnyToOneNode : Node
    {
        public int Count { get; set; }

        public OperationAnyToOneNode() : base("Operation", "Any To One")
        {
            AddInput();
            AddOutput(DataType.Logical);

            Settings.Add("zero", new NodeSetting(NodeSettingType.Checkbox, "Generate Zero", "false"));

            options.ResetOutputsIfAnyInputIsNull = true;
        }

        public override void OnInputChange(Input input)
        {
            Outputs[0].Value = "1";

            if (Settings["zero"].Value == "true")
                Outputs[0].Value = "0";
        }

        public override string GetNodeDescription()
        {
            return "This node sends \"1\" to the output, when anything comes to the input except null.<br/><br/>" +
                   "In the settings of the node you can enable the option to send \"0\" immediately after \"1\".";
        }
    }
}