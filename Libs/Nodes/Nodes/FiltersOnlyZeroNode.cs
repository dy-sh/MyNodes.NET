/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
    public class FiltersOnlyZeroNode : Node
    {
        public FiltersOnlyZeroNode() : base("Filters", "Only Zero")
        {
            AddInput();
            AddOutput();
        }


        public override void OnInputChange(Input input)
        {
            if (input.Value== "0")
                Outputs[0].Value = "0";
        }

        public override string GetNodeDescription()
        {
            return "";
        }
    }
}