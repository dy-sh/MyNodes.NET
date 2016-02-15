/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;
using System.Linq;

namespace MyNetSensors.Nodes
{
    public class UiRgbSlidersNode : UiNode
    {
        public string Value { get; set; }

        public UiRgbSlidersNode() : base("RGB Sliders", 0, 1)
        {
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
    }
}
