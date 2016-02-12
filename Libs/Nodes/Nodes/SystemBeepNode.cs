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

        public SystemBeepNode() : base("System","Beep",1, 0)
        {
            Inputs[0].Type = DataType.Logical;
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            if (Inputs.Any(i => i.Value == null))
            {
                return;
            }

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
