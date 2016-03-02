/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;
using System.Linq;

namespace MyNodes.Nodes
{
    public class ConnectionRouterOneToMultipleNode : Node
    {
        public ConnectionRouterOneToMultipleNode() : base("Connection", "Router One-Multiple")
        {
            AddInput("Active Output", DataType.Number);
            AddInput("Value", DataType.Text, true);
            AddOutput("Out 0");
            AddOutput("Out 1");

            options.ResetOutputsIfAnyInputIsNull = true;

            Settings.Add("outs", new NodeSetting(NodeSettingType.Number, "Outputs count", Outputs.Count.ToString()));
        }


        public override void OnInputChange(Input input)
        {
            try
            {
                int index = (int)double.Parse(Inputs[0].Value) + 1;


                if (index < 1 || index > Outputs.Count)
                {
                    LogError("Output Number is out of range");
                    return;
                }

                Outputs[index - 1].Value = Inputs[1].Value;
            }
            catch
            {
                LogIncorrectInputValueError(Inputs[0]);
            }
        }

        public override bool SetSettings(Dictionary<string, string> data)
        {
            int count = int.Parse(data["outs"]);

            if (count < 2)
                count = 2;

            else if (count > 1000)
                count = 1000;

            data["outs"] = count.ToString();

            if (count > Outputs.Count)
            {
                int addCount = count - Outputs.Count;

                for (int i = 0; i < addCount; i++)
                    AddOutput("Out " + Outputs.Count);

                LogInfo($"Added {addCount} new outputs");
                UpdateMe();
                UpdateMeInDb();
            }
            else if (count < Outputs.Count)
            {
                int remCount = Outputs.Count - count;

                for (int i = 0; i < remCount; i++)
                    RemoveOutput(Outputs.Last());

                LogInfo($"Removed {remCount} outputs");
                UpdateMe();
                UpdateMeInDb();
            }


            return base.SetSettings(data);
        }

        public override string GetNodeDescription()
        {
            return "This node can be used to link one node with several nodes. <br/>" +
                   "You can change which node will receive messages (using input \"Active Output\"). " +
                   "The other nodes will not receive anything. <br/>" +
                   "In the settings you can specify the number of outputs.";
        }
    }
}