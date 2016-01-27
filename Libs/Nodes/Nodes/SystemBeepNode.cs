/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.Nodes
{
    public class SystemBeepNode : Node
    {
        /// <summary>
        /// Beep (1 input). Beep when input change, exclude NULL.
        /// </summary>
        public SystemBeepNode() : base(1, 0)
        {
            this.Title = "Beep";
            this.Type = "System/Beep";

            Inputs[0].Type = DataType.Logical;
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            if (Inputs[0].Value == "1")
            {
                Beep();
                LogInfo("Beep");
            }
        }

        public async void Beep()
        {
            await Task.Run(() => Console.Beep());
        }
    }
}
