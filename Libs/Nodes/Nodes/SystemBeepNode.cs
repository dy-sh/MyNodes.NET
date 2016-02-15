/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Threading.Tasks;

namespace MyNetSensors.Nodes
{
    public class SystemBeepNode : Node
    {
        public SystemBeepNode() : base("System", "Beep", 1, 0)
        {
            Inputs[0].Name = "Start";
            Inputs[0].Type = DataType.Logical;
        }

        public override void OnInputChange(Input input)
        {
            if (input.Value == "1")
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