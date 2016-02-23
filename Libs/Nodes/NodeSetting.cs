/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.Nodes
{
    public enum NodeSettingType
    {
        Text,
        Number,
        Checkbox
    }

    public class NodeSetting
    {
        public NodeSettingType Type { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }

        public NodeSetting()
        {
            
        }

        public NodeSetting(NodeSettingType type, string label, string value)
        {
            Type = type;
            Label = label;
            Value = value;
        }
    }
}
