/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;

namespace MyNetSensors.Nodes
{
    public class ConsoleNode:Node
    {
        /// <summary>
        /// Console (1 input).
        /// </summary>
        public ConsoleNode() : base(1, 0)
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

            Console.WriteLine($"CONSOLE NODE: {input.Value}");
        }
    }
}
