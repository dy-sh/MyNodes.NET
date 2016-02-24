/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
    public class FiltersOnlyOneNode : Node
    {
        public FiltersOnlyOneNode() : base("Filters", "Only One")
        {
            AddInput();
            AddOutput();
        }


        public override void OnInputChange(Input input)
        {
            if (input.Value== "1")
                Outputs[0].Value = "1";
        }

        public override string GetNodeDescription()
        {
            return "";
        }
    }
}