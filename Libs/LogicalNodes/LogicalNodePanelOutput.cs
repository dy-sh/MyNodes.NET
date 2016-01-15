﻿/*  MyNetSensors 
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
    public class LogicalNodePanelOutput : LogicalNode
    {
        public LogicalNodePanelOutput() : base(1, 0)
        {
            this.Title = "Panel Output";
            this.Type = "Main/Panel Output";
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
        }
    }
}