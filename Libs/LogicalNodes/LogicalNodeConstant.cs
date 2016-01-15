/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.LogicalNodes
{
    public class LogicalNodeConstant : LogicalNode
    {
        public string Value { get; set; }

        public LogicalNodeConstant() : base(0,1)
        {
            this.Title = "Constant";
            this.Type = "Basic/Constant";
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
        }

        public void SetValue(string value)
        {
            Value = value;
            LogInfo($"Constant changed: [{Value??"NULL"}]");
            Outputs[0].Value = Value;
        }
    }
}
