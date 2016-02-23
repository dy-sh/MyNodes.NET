/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
    public class OperationFlipflopNode : Node
    {
        private int part;
        private string result;

        public OperationFlipflopNode() : base("Operation", "Flip-Flop")
        {
            AddInput(DataType.Logical);
            AddOutput(DataType.Logical);

            options.ResetOutputsIfAnyInputIsNull = true;
        }

        public override void OnInputChange(Input input)
        {
            switch (part)
            {
                case 0:
                    if (Inputs[0].Value == "1")
                    {
                        result = "1";
                        part++;
                    }
                    break;

                case 1:
                    if (Inputs[0].Value == "0")
                    {
                        result = "1";
                        part++;
                    }
                    break;

                case 2:
                    if (Inputs[0].Value == "1")
                    {
                        result = "0";
                        part++;
                    }
                    break;

                case 3:
                    if (Inputs[0].Value == "0")
                    {
                        result = "0";
                        part = 0;
                    }
                    break;
            }

            Outputs[0].Value = result;
        }
    }
}