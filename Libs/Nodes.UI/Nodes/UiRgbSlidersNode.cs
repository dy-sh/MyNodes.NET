/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;
using System.Linq;

namespace MyNodes.Nodes
{
    public class UiRgbSlidersNode : UiNode
    {
        public string Value { get; set; }

        public UiRgbSlidersNode() : base("UI", "RGB Sliders")
        {
            AddOutput();
            Value = "000000";
            Outputs[0].Value = Value.ToString();
        }


        public override bool SetValues(Dictionary<string, string> values)
        {
            Value = values.FirstOrDefault().Value;
            Outputs[0].Value = Value;

            UpdateMe();
            UpdateMeInDb();

            return true;
        }

        public override string GetNodeDescription()
        {
            return "This is a UI node. It displays three sliders on the dashboard " +
                   "with which you can specify an RGB color.";
        }
    }
}
