/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNodes.Nodes
{
    public class FiltersPreventDuplicationNode : Node
    {
        private string val = null;

        public FiltersPreventDuplicationNode() : base("Filters", "Prevent Duplication")
        {
            AddInput();
            AddOutput();
        }


        public override void OnInputChange(Input input)
        {
            if (input.Value == val)
                return;

            val = input.Value;
            Outputs[0].Value = val;
        }

        public override string GetNodeDescription()
        {
            return "This node filters the input values. " +
                   "It transmits the value only if it is not the same " +
                   "that was already sent to the output.";
        }
    }
}