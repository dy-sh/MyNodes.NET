/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;
using System.Linq;

namespace MyNetSensors.Nodes
{
    public class ConnectionHubNode : Node
    {
        public ConnectionHubNode() : base("Connection", "Hub")
        {
            AddInput("In 1", DataType.Text, true);
            AddInput("In 2", DataType.Text, true);
            AddOutput();

            options.ResetOutputsIfAnyInputIsNull = true;

            Settings.Add("inputs", new NodeSetting(NodeSettingType.Number, "Inputs count", Inputs.Count.ToString()));
            Settings.Add("outputs", new NodeSetting(NodeSettingType.Number, "Outputs count", Outputs.Count.ToString()));
        }


        public override void OnInputChange(Input input)
        {
            foreach (var output in Outputs)
                output.Value = input.Value;
        }

        public override bool SetSettings(Dictionary<string, string> data)
        {
            int ins = int.Parse(data["inputs"]);
            int outs = int.Parse(data["outputs"]);

            if (ins < 1)
                ins = 1;
            else if (ins > 1000)
                ins = 1000;

            if (outs < 1)
                outs = 1;
            else if (outs > 1000)
                outs = 1000;

            data["inputs"] = ins.ToString();
            data["outputs"] = outs.ToString();

            ChangeInputsCount(ins);
            ChangeOutputsCount(outs);

            UpdateMe();
            UpdateMeInDb();


            return base.SetSettings(data);
        }

        private void ChangeOutputsCount(int count)
        {
            if (count > Outputs.Count)
            {
                int addCount = count - Outputs.Count;

                for (int i = 0; i < addCount; i++)
                    AddOutput();

                LogInfo($"Added {addCount} new outputs");
            }
            else if (count < Outputs.Count)
            {
                int remCount = Outputs.Count - count;

                for (int i = 0; i < remCount; i++)
                    RemoveOutput(Outputs.Last());

                LogInfo($"Removed {remCount} outputs");
            }

        }

        public void ChangeInputsCount(int count)
        {
            if (count > Inputs.Count)
            {
                int addCount = count - Inputs.Count;

                for (int i = 0; i < addCount; i++)
                    AddInput("In " + Inputs.Count, DataType.Text, true);

                LogInfo($"Added {addCount} new inputs");
            }
            else if (count < Inputs.Count)
            {
                int remCount = Inputs.Count - count;

                for (int i = 0; i < remCount; i++)
                    RemoveInput(Inputs.Last());

                LogInfo($"Removed {remCount} inputs");
            }
        }
    }
}