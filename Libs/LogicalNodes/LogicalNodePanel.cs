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
    public class LogicalNodePanel:LogicalNode
    {
        public LogicalNodePanel() : base(0, 0)
        {
            this.Title = "Panel";
            this.Type = "Main/Panel";
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
        }
    }
}
