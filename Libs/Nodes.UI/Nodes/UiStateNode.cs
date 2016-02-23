/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

namespace MyNetSensors.Nodes
{
    public class UiStateNode : UiNode
    {
        public string Value { get; set; }

        public UiStateNode() : base("UI", "State")
        {
            AddInput();
        }


        public override void OnInputChange(Input input)
        {
            Value = input.Value;
            UpdateMe();
        }


    }
}
