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
    public class LogicalNodeConsole:LogicalNode
    {
        /// <summary>
        /// Console (1 input).
        /// </summary>
        public LogicalNodeConsole() : base(1, 0)
        {
            this.Title = "System Console";
            this.Type = "System/Console";
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            //Log($"Console: {input.Value}");

            Console.WriteLine($"LOGICAL NODE CONSOLE: {input.Value}");
        }
    }
}
