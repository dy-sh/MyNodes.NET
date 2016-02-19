//planer-pro copyright 2015 GPL - license.

using System.Collections.Generic;
using System.Linq;

namespace MyNetSensors.Nodes
{
    public class OperationControlShaperNode : Node
    {
        public OperationControlShaperNode() : base("Operation", "Control Shaper")
        {
            AddInput("Value", DataType.Number);
            AddOutput();

            ChangeInputsCount(3);

            options.ResetOutputsIfAnyInputIsNull = true;

            Settings.Add("inputs", new NodeSetting(NodeSettingType.Number, "Points Count", (Inputs.Count - 1).ToString()));
        }


        public override void OnInputChange(Input input)
        {

            double position = double.Parse(Inputs[0].Value);//20

            position = Clamp(position, 0, 100);

            int pointsCount = Inputs.Count - 1; //5

            double stepSize = 100D / (pointsCount - 1);//25

            double stepIndex = (position / stepSize) + 1;//1,8


            double positionInStep = stepIndex - ((int)stepIndex);//0.8

            double startValue = ((int)stepIndex-1)* stepSize;
            if (Inputs[(int)stepIndex].Value != null)
                startValue = double.Parse(Inputs[(int)stepIndex].Value);//0

            if (position>=100)
            {
                Outputs[0].Value = startValue.ToString();
                return;
            }

            double endValue = startValue+ stepSize;
            if (Inputs[(int)stepIndex + 1].Value != null)
                endValue = double.Parse(Inputs[(int)stepIndex + 1].Value);//25

            double result = Remap(positionInStep, 0, 1, startValue, endValue);

            Outputs[0].Value = result.ToString();
        }

        public override bool SetSettings(Dictionary<string, string> data)
        {
            int count = int.Parse(data["inputs"]);

            if (count < 2)
                count = 2;

            else if (count > 1000)
                count = 1000;

            data["inputs"] = count.ToString();

            ChangeInputsCount(count);

            UpdateMe();
            UpdateMeInDb();

            return base.SetSettings(data);
        }


        public void ChangeInputsCount(int count)
        {
            int currentCount = Inputs.Count - 1;

            if (count > currentCount)
            {
                int addCount = count - currentCount;

                for (int i = 0; i < addCount; i++)
                    AddInput("In " + Inputs.Count, DataType.Number, true);

                LogInfo($"Added {addCount} new inputs");
            }
            else if (count < currentCount)
            {
                int remCount = currentCount - count;

                for (int i = 0; i < remCount; i++)
                    RemoveInput(Inputs.Last());

                LogInfo($"Removed {remCount} inputs");
            }

            count = Inputs.Count - 1;
            Inputs[1].Name = "0";
            for (int i = 1; i < count; i++)
            {
                double point = 100D / (count - 1) * i;
                Inputs[i + 1].Name = point.ToString("##.#");
            }
        }

        public double Remap(double value, double fromMin, double fromMax, double toMin, double toMax)
        {
            return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
        }

        public static double Clamp(double value, double min, double max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }
    }
}