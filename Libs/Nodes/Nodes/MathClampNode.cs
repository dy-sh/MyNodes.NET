/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
    public class MathClampNode : Node
    {
        public MathClampNode() : base("Math", "Clamp")
        {
            AddInput("Value", DataType.Number);
            AddInput("Min", DataType.Number, true);
            AddInput("Max", DataType.Number, true);
            AddOutput(DataType.Number);

            options.ResetOutputsIfAnyInputIsNull = true;
        }


        public override void OnInputChange(Input input)
        {
            double min = 0;
            double max = 100;

            double value = double.Parse(Inputs[0].Value);

            if (Inputs[1].Value != null)
                min = double.Parse(Inputs[1].Value);

            if (Inputs[2].Value != null)
                max = double.Parse(Inputs[2].Value);

            if (min > max)
            {
                LogError($"Min must be less than Max. Min is [{min}] Max is [{max}]");
                Outputs[0].Value = null;
                return;
            }

            var result = value < min ? min : value > max ? max : value;

            Outputs[0].Value = result.ToString();
        }

        public override string GetNodeDescription()
        {
            return "This node limits the value to the specified range. " +
                   "For example, Min=3, Max=5. " +
                   "Now, if the \"Value\" input is 1, the output will be 3. " +
                   "If the value is 6, the output will be 5. " +
                   "If the value is 2.5, the output will be 2.5. ";
        }
    }
}