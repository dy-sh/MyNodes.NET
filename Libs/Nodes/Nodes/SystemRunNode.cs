/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.Nodes
{
    public class SystemRunNode : Node
    {

        public SystemRunNode() : base(2, 0)
        {
            this.Title = "Run";
            this.Type = "System/Run";

            Inputs[0].Name = "Path";
            Inputs[1].Name = "Start";

            Inputs[0].Type = DataType.Text;
            Inputs[1].Type = DataType.Logical;
        }

        public override void Loop()
        {
        }

        public override void OnInputChange(Input input)
        {
            if (input == Inputs[1] && Inputs[1].Value == "1")
            {
                try
                {
                    String path = Inputs[0].Value;

                    var proc1 = new ProcessStartInfo();
                    proc1.UseShellExecute = true;
                    proc1.WorkingDirectory = @"C:\Windows\System32";
                    proc1.FileName = @"C:\Windows\System32\cmd.exe";
                    proc1.Arguments = "/c " + path;
                    proc1.WindowStyle = ProcessWindowStyle.Hidden;
                    Process.Start(proc1);

                    LogInfo("Executed");
                }
                catch (Exception)
                {
                    LogInfo("Incorrect path");
                }
            }
        }
    }
}
