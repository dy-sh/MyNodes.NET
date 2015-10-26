/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MyNetSensors.Gateway;
using MyNetSensors.GatewayRepository;
using MyNetSensors.NodesLinks;
using MyNetSensors.NodeTasks;
using MyNetSensors.SensorsHistoryRepository;
using MyNetSensors.SoftNodes;


namespace MyNetSensors.SerialController_Console
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            SerialController.SerialController.OnDebugStateMessage += Console.WriteLine;
            SerialController.SerialController.OnDebugTxRxMessage += Console.WriteLine;
            SerialController.SerialController.Start();
            while (true)
                Console.ReadLine();
        }
    }
}
