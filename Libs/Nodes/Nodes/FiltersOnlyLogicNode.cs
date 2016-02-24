/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
    public class FiltersOnlyLogicNode : Node
    {
        public FiltersOnlyLogicNode() : base("Filters", "Only Logic")
        {
            AddInput();
            AddOutput("Out", DataType.Logical);
        }


        public override void OnInputChange(Input input)
        {
            if (input.Value == "1" || input.Value == "0")
                Outputs[0].Value = input.Value;
        }

        public override string GetNodeDescription()
        {
            return "";
        }
    }
}