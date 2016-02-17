//planer-pro copyright 2015 GPL - license.

using System.Collections.Generic;
using System.Linq;

namespace MyNetSensors.Nodes
{
    public class ConnectionRouterMultipleToOneNode : Node
    {
        public ConnectionRouterMultipleToOneNode() : base("Connection", "Router Multiple-One")
        {
            AddInput("Input Number", DataType.Number);
            AddInput("In 1", DataType.Text, true);
            AddInput("In 2", DataType.Text, true);
            AddOutput();

            options.ResetOutputsIfAnyInputIsNull = true;

            Settings.Add("inputs", new NodeSetting(NodeSettingType.Number, "Inputs count", (Inputs.Count-1).ToString()));
        }


        public override void OnInputChange(Input input)
        {
            try
            {
                int index = (int) double.Parse(Inputs[0].Value);

                if(input!=Inputs[0] && input!=Inputs[index])
                    return;

                if (index < 1 || index > Inputs.Count-1)
                {
                    LogError("Input Number is out of range");
                    return;
                }

                Outputs[0].Value = Inputs[index].Value;
            }
            catch
            {
                LogIncorrectInputValueError(Inputs[0]);
            }
        }

        public override bool SetSettings(Dictionary<string, string> data)
        {
            int count = int.Parse(data["inputs"]);

            if (count < 2)
                count = 2;

            else if (count > 1000)
                count = 1000;

            data["inputs"] = count.ToString();

            if (count > Inputs.Count-1)
            {
                int addCount = count - Inputs.Count+1;

                for (int i = 0; i < addCount; i++)
                    AddInput("In " + Inputs.Count , DataType.Text, true);

                LogInfo($"Added {addCount} new inputs");
                UpdateMe();
                UpdateMeInDb();
            }
            else if (count < Inputs.Count-1)
            {
                int remCount = Inputs.Count-1 - count;

                for (int i = 0; i < remCount; i++)
                    RemoveInput(Inputs.Last());

                LogInfo($"Removed {remCount} inputs");
                UpdateMe();
                UpdateMeInDb();
            }


            return base.SetSettings(data);
        }
    }
}